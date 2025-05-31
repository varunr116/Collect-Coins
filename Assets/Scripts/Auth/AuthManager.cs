using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class AuthManager
{
    private static readonly string filePath =
        Path.Combine(Application.persistentDataPath, "users.json");

    [System.Serializable]
    private class UserData
    {
        public string phone;
        public string password;
    }

    [System.Serializable]
    private class UserList
    {
        public List<UserData> users = new List<UserData>();
    }

    // Load the user list (empty if no file)
    private static UserList LoadUsers()
    {
        if (!File.Exists(filePath))
            return new UserList();

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<UserList>(json) ?? new UserList();
    }

    // Save the user list back to disk
    private static void SaveUsers(UserList list)
    {
        string json = JsonUtility.ToJson(list, prettyPrint: true);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Returns false if phone is already registered.
    /// </summary>
    public static bool Register(string phone, string password)
    {
        var list = LoadUsers();
        if (list.users.Exists(u => u.phone == phone))
            return false; // duplicate

        list.users.Add(new UserData { phone = phone, password = password });
        SaveUsers(list);
        return true;
    }

    /// <summary>
    /// True if one of the stored credentials matches.
    /// </summary>
    public static bool Validate(string phone, string password)
    {
        var list = LoadUsers();
        return list.users.Exists(u => u.phone == phone && u.password == password);
    }
}
