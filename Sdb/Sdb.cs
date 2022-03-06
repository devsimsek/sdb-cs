using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Sdb
{
    /**
     * Sdb, Flat database library.
     * Copyright (C)devsimsek
     * version: v1.0.2
     * see: https://github.com/devsimsek/sdb-cs
     */
    struct DatabaseObject
    {
        public string File;
        public string Path;
    }

    struct ParseJson
    {
        public object Field { get; set; }
        public object Value { get; set; }
    }

    public class Database
    {
        private bool debugMode;

        private DatabaseObject _dbp;
        private static Dictionary<string, object> _db;

        // <summary>Connect to database</summary>
        // <param>file: The file that sdb will connect</param>
        // <param>directory: The directory that sdb will look for database.</param>
        // <param>force: Force creation. If true database file will be forcefully created.</param>
        public void Connect(string file, string directory = null, bool debug = false, bool force = false)
        {
            debugMode = debug;
            Debug($"Trying to connect {(directory != null ? directory + "/" : "")}{file}.");
            var path = directory ?? Directory.GetCurrentDirectory();
            var read = OpenDatabaseFile(file, path, force);

            if (read != "{}")
            {
                _db = new Dictionary<string, object>(JsonSerializer.Deserialize<Dictionary<string, dynamic>>(read)!);
            }
            else
                _db = new Dictionary<string, object>();

            _dbp = new DatabaseObject
            {
                File = file,
                Path = path,
            };
        }

        // <summary>Create field</summary>
        // <param>field: The field of variable</param>
        // <param>value: The value inserted in field.</param>
        public void Create(string field, object value = null)
        {
            if (!_db.ContainsKey(field))
            {
                _db[field] = value;
            }
        }

        // <summary>Read field</summary>
        // <param>field: The field of variable</param>
        public object Read(string field = null)
        {
            if (field == null) return _db;
            return _db[field];
        }

        // <summary>Update field</summary>
        // <param>field: The field of variable</param>
        // <param>value: The value inserted in field.</param>
        public void Update(string field, object value)
        {
            _db[field] = value;
        }

        // <summary>Delete field</summary>
        // <param>field: The field of variable</param>
        public bool Delete(string field)
        {
            return _db.Remove(field);
        }

        // <summary>Save database</summary>
        public void Save()
        {
            var o = JsonSerializer.Serialize(_db);
            using StreamWriter sw = File.CreateText($"{_dbp.Path}/{_dbp.File}");
            sw.WriteLine(o);
        }

        private string OpenDatabaseFile(string file, string path, bool force)
        {
            string output = null;

            if (!File.Exists($"{path}/{file}"))
            {
                if (force)
                {
                    Debug("Forcing to create new database file.");
                    using StreamWriter sw = File.CreateText($"{path}/{file}");
                    sw.WriteLine("{}");
                }
            }

            if (File.Exists($"{path}/{file}"))
            {
                using StreamReader sr = File.OpenText($"{path}/{file}");
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    output += s;
                }
            }
            else
            {
                Debug("Warning. Database file does not exists.", "Error", true);
                Environment.Exit(0);
            }

            return output;
        }

        private void Debug(Object message, string flag = "Debug", bool force = false)
        {
            if (force) Console.WriteLine($"[{DateTime.Now}-{flag}]: {message}");
            if (debugMode) Console.WriteLine($"[{DateTime.Now}-{flag}]: {message}");
        }
    }
}