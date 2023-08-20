using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using WebChatClientApp.Models.DTO;

namespace WebChatClientApp.Services;


public class UserCredentialsService
{
    private readonly string _filePath;
    private ICollection<User> _users;
    private readonly string _encryptionKey; 

    public UserCredentialsService()
    {
        _filePath = "UsersData/users.json";
        _encryptionKey = Environment.GetEnvironmentVariable("WebChatAppSecretKey");

        string directoryPath = Path.GetDirectoryName(_filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        LoadUsers();
    }

    private void LoadUsers()
    {
        if (File.Exists(_filePath))
        {
            
            string json = Decrypt(File.ReadAllText(_filePath));
            _users = JsonConvert.DeserializeObject<List<User>>(json);
        }
        else
        {
            //_encryptionKey = GenerateAndSetSecretKey();
            _users = new List<User>();
        }
    }

    public ICollection<User> GetUsers()
    {
        return _users;
    }

    public void RemoveUserByUsername(string username)
    {
        User userToRemove = _users.FirstOrDefault(u => u.UserName == username);

        if (userToRemove != null)
        {
            _users.Remove(userToRemove);
            SaveUsers();
        }
    }

    private void SaveUsers()
    {
        string json = JsonConvert.SerializeObject(_users, Formatting.Indented);
        File.WriteAllText(_filePath, Encrypt(json));
    }

    public void AddUser(User user)
    {
        _users.Add(user);
        SaveUsers();
    }

    private byte[] NormalizeKey()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] hashedBytes = sha256.ComputeHash(keyBytes);
            return hashedBytes;
        }
    }

    private string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = NormalizeKey();
            aesAlg.IV = new byte[aesAlg.BlockSize / 8]; 

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    private string Decrypt(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = NormalizeKey();
            aesAlg.IV = new byte[aesAlg.BlockSize / 8]; 

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
