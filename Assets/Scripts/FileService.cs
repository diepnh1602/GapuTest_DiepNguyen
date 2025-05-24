using System.IO;
using UnityEngine;

public static class FileService
{
    /// <summary>
    /// Lưu object bất kỳ dưới dạng JSON vào file .txt
    /// </summary>
    public static void Save<T>(string fileName, T data)
    {
        string path = GetFullPath(fileName);
        string json = JsonHelper.Serialize(data);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Đọc file JSON và ép kiểu về object T
    /// </summary>
    public static T Get<T>(string fileName)
    {
        string path = GetFullPath(fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (json.IsNullOrEmpty()) return default;
            return JsonHelper.Deserialize<T>(json);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy file: " + path);
            return default;
        }
    }

    /// <summary>
    /// Xoá file nếu tồn tại
    /// </summary>
    public static void Delete(string fileName)
    {
        string path = GetFullPath(fileName);
        if (File.Exists(path))
            File.Delete(path);
    }

    /// <summary>
    /// Kiểm tra file có tồn tại không
    /// </summary>
    public static bool Exists(string fileName)
    {
        return File.Exists(GetFullPath(fileName));
    }

    /// <summary>
    /// Trả về đường dẫn đầy đủ
    /// </summary>
    private static string GetFullPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
