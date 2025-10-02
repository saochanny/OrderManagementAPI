using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OrderManagementAPI.Utilizes;

public static class AesEncryptionUtil
{
    private static readonly byte[] Iv = "1234567890123456"u8.ToArray(); // 16 bytes = 128-bit IV

    // Encrypt generic data
    public static string Encrypt <T>(T data, string key)
    {
        var json = JsonSerializer.Serialize(data);
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = Iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using var streamWriter = new StreamWriter(cryptoStream);
        streamWriter.Write(json);
        return Convert.ToBase64String(memoryStream.ToArray());

    }

    // Decrypt encryption data
    public static T? Decrypt<T>(string encryptData, string key)
    {
        var buffer = Convert.FromBase64String(encryptData);
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = Iv;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        var json = streamReader.ReadToEnd();
        return JsonSerializer.Deserialize<T>(json);
    }
}