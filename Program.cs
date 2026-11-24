using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

//3. Сериализация списка
List<int> scores = new List<int> { 80, 90, 70 };
string numbersJson = JsonSerializer.Serialize(scores);
Console.WriteLine(numbersJson);

JsonSerializerOptions options = new JsonSerializerOptions
{
    WriteIndented = true
};
string formattedJson = JsonSerializer.Serialize(scores, options);
Console.WriteLine(formattedJson);

//4. Десериализация и выбор типа
string inputJson = "[1,2]";
List<int> restoredScores = JsonSerializer.Deserialize<List<int>>(inputJson)
 ?? throw new JsonException("Ожидается массив чисел, а не null");
Console.WriteLine(restoredScores.Count);
Console.WriteLine(restoredScores[1]);

//5. Сериализация словаря
Dictionary<string, int> resultsExample = new Dictionary<string, int>
{
 { "анна", 80 },
 { "борис", 90 }
};
// string dictionaryJson = JsonSerializer.Serialize(
//  resultsExample, new JsonSerializerOptions { WriteIndented = true });
// Console.WriteLine(dictionaryJson);
// Dictionary<string, int> restoredResults =
//  JsonSerializer.Deserialize<Dictionary<string, int>>(dictionaryJson)
//  ?? throw new JsonException("Ожидается объект результатов");
// Console.WriteLine(restoredResults["анна"]);

//Найдите ошибку: ToString() вместо JSON
string dictionaryJson = JsonSerializer.Serialize(
    resultsExample, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(dictionaryJson);
Dictionary<string, int> restoredResults =
    JsonSerializer.Deserialize<Dictionary<string, int>>(dictionaryJson)
    ?? throw new JsonException("Ожидается объект результатов");

Console.WriteLine(restoredResults["анна"]);

// восстановление прошло без исключения — значит, это корректный JSON
if (restoredResults.Count != 2 ||
    restoredResults["анна"] != 80 ||
    restoredResults["борис"] != 90)
{
    throw new InvalidDataException("Словарь восстановлен неверно");
}

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

string examplePath = Path.Combine("data", "example-results.json");
SaveResults(examplePath, resultsExample);
Dictionary<string, int> loadedExample = LoadResults(examplePath);

//7. Отсутствующий и повреждённый файл
//Найдите ошибку: повреждённый журнал перезаписан
string path = Path.Combine("data", "damaged-example.json");

Dictionary<string, int> loaded;
try
{
    loaded = LoadResults(path);
}
catch (JsonException)
{
    Console.WriteLine("Ошибка JSON: журнал не изменён");
    return;  
}
SaveResults(path, loaded);
