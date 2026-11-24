using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

//6. Сохранение и загрузка JSON-файла
void SaveResults(string path, Dictionary<string, int> results)
{
    JsonSerializerOptions options = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    string json = JsonSerializer.Serialize(results, options);
    File.WriteAllText(path, json, Encoding.UTF8);
}

//Загрузка и проверка значений
Dictionary<string, int> LoadResults(string path)
{
    string json = File.ReadAllText(path, Encoding.UTF8);
    Dictionary<string, int> results =
    JsonSerializer.Deserialize<Dictionary<string, int>>(json)
    ?? throw new JsonException("Ожидается объект результатов, а не null");
    foreach (KeyValuePair<string, int> entry in results)
    {
        if (entry.Key.Length == 0 ||
        entry.Key != entry.Key.Trim().ToLowerInvariant() ||
        entry.Value < 0 || entry.Value > 100)
        {
            throw new InvalidDataException("Недопустимое имя или результат в журнале");
        }
    }
    return results;
}


//8. Основная часть — «Журнал результатов в JSON»
int ReadScore()
{
    while (true)
    {
        string input = Console.ReadLine() ?? "";
        if (int.TryParse(input, out int value) && value >= 0 && value <= 100)
        {
            return value;
        }
        Console.WriteLine("Введите целое число от 0 до 100");
    }
} 

void PrintResults(Dictionary<string, int> results)
{
    if (results.Count == 0)
    {
        Console.WriteLine("Журнал пуст");
        return;
    }
    foreach (KeyValuePair<string, int> entry in results)
    {
        Console.WriteLine($"{entry.Key}: {entry.Value}");
    }
} 

string dataDirectory = "data";
string path = Path.Combine(dataDirectory, "results.json");

try
{
    Directory.CreateDirectory(dataDirectory);
    Dictionary<string, int> results;

    try
    {
        results = LoadResults(path);
    }
    catch (FileNotFoundException)
    {
        results = new Dictionary<string, int>();
        Console.WriteLine("Файл журнала отсутствует. Создан новый журнал в памяти");
    }

    Console.WriteLine($"Загружено участников: {results.Count}");
    PrintResults(results);

    Console.Write("Имя для добавления или обновления: ");
    string name = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    int score = ReadScore();

    if (results.ContainsKey(name))
    {
        results[name] = score;
        Console.WriteLine("Результат обновлён");
    }
    else
    {
        results[name] = score;
        Console.WriteLine("Участник добавлен");
    }

    Console.Write("Имя для удаления: ");
    string nameToRemove = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (results.Remove(nameToRemove))
    {
        Console.WriteLine("Участник удалён");
    }
    else
    {
        Console.WriteLine("Участник не найден");
    }

    Console.Write("Имя для поиска: ");
    string nameToFind = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (results.TryGetValue(nameToFind, out int foundScore))
    {
        Console.WriteLine($"Результат: {foundScore}");
    }
    else
    {
        Console.WriteLine("Участник не найден");
    }

    SaveResults(path, results);
    Dictionary<string, int> restored = LoadResults(path);

    Console.WriteLine(">>> Журнал после чтения из файла <<");
    Console.WriteLine($"Количество участников: {restored.Count}");
    PrintResults(restored);
    Console.WriteLine($"Журнал сохранён: {Path.GetFullPath(path)}");

//Вариант 1. Отбор по результату
    Console.Write("Порог результата (0-100): ");
    int limit = int.Parse(Console.ReadLine() ?? "0");

    Dictionary<string, int> selected = SelectResults(restored, limit);

    string selectedPath = Path.Combine(dataDirectory, "selected-results.json");
    SaveResults(selectedPath, selected);

    Console.WriteLine($">>> Отобрано участников: {selected.Count}");
    PrintResults(selected);
    Console.WriteLine($"Отбор сохранён: {Path.GetFullPath(selectedPath)}");
}
catch (JsonException)
{
    Console.WriteLine("Ошибка JSON: проверьте содержимое журнала");
}
catch (InvalidDataException)
{
    Console.WriteLine("Недопустимые данные журнала: проверьте имена и диапазон 0-100");
}
catch (FileNotFoundException)
{
    Console.WriteLine("Файл не найден при повторном чтении журнала");
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("Нет доступа к файлу или папке");
}
catch (IOException)
{
    Console.WriteLine("Ошибка чтения или записи файла");
} 

//13. Самостоятельное задание
//Вариант 1. Отбор по результату
Dictionary<string, int> SelectResults(Dictionary<string, int> results, int limit)
{
    Dictionary<string, int> selected = new Dictionary<string, int>();
    foreach (KeyValuePair<string, int> entry in results)
    {
        if (entry.Value >= limit)
        {
            selected[entry.Key] = entry.Value;
        }
    }
    return selected;
}