using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FileService
{
    private const string DataFolder = "Data";

    public static string CreateFolder(string folderName)
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;
        string folderPath = Path.Combine(projectDirectory, folderName);

        Directory.CreateDirectory(folderPath);

        return folderPath;
    }

    public static void SaveDataToJson<T>(List<T> data, string fileName)
    {
        string folderPath = CreateFolder(DataFolder);
        string filePath = Path.Combine(folderPath, fileName + ".json");
        string jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, jsonData);
            Console.WriteLine($"Dosya '{filePath}' başarıyla kaydedildi.");
        }
        else
        {
            string existingContent = File.ReadAllText(filePath);
            string updatedContent = existingContent.TrimEnd('}') + "\n" + jsonData.TrimStart('{') + "}";
            File.WriteAllText(filePath, updatedContent);
            Console.WriteLine($"Dosya '{filePath}' güncellendi.");
        }
    }
}

