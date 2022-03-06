using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Sdb
{
    /**
     * Sdb, Flat database library.
     * Copyright (C)devsimsek
     * version: v1.0
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
        private bool debugMode = true;

        private DatabaseObject _dbp;
        private List<ParseJson> _db;

        // <summary>Connect to database</summary>
        // <param>file: The file that sdb will connect</param>
        // <param>directory: The directory that sdb will look for database.</param>
        // <param>force: Force creation. If true database file will be forcefully created.</param>
        public void Connect(string file, string directory = null, bool force = false)
        {
            Debug($"Trying to connect {directory ?? ""}{file}.");
            var path = directory ?? Directory.GetCurrentDirectory();
            var read = OpenDatabaseFile(file, path, force);
            _db = new List<ParseJson>(JsonConvert.DeserializeObject<ParseJson[]>(read) ??
                                      Array.Empty<ParseJson>());
            _dbp = new DatabaseObject
            {
                File = file,
                Path = path,
            };
        }

        // <summary>Create field</summary>
        // <param>field: The field of variable</param>
        // <param>value: The value inserted in field.</param>
        public void Create(object field, object value = null)
        {
            if (_db.FindIndex(x => x.Field == field) == -1)
            {
                _db.Add(new ParseJson() {Field = field, Value = value});
            }
            else
            {
                Debug("Field already exists.", "Warning", true);
            }
        }

        // <summary>Read field</summary>
        // <param>field: The field of variable</param>
        public object Read(object field = null)
        {
            if (field == null) return _db;
            var obj = _db.Find(obj => obj.Field == field);
            return obj.Value;
        }

        // <summary>Update field</summary>
        // <param>field: The field of variable</param>
        // <param>value: The value inserted in field.</param>
        public void Update(object field, object value)
        {
            var parseJson = _db.FindIndex(obj => obj.Field == field);
            if (parseJson != -1)
            {
                _db[parseJson] = new ParseJson()
                {
                    Field = field,
                    Value = value
                };
            }
        }

        // <summary>Delete field</summary>
        // <param>field: The field of variable</param>
        public bool Delete(string field)
        {
            var parseJson = _db.FindIndex(obj => (string) obj.Field == field);
            return _db.Remove(_db[parseJson]);
        }

        // <summary>Save database</summary>
        public void Save()
        {
            var o = JsonConvert.SerializeObject(_db);
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
                    sw.WriteLine("[]");
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

        private void Debug(string message, string flag = "Debug", bool force = false)
        {
            if (force) Console.WriteLine($"[{DateTime.Now}-{flag}]: {message}");
            if (debugMode) Console.WriteLine($"[{DateTime.Now}-{flag}]: {message}");
        }
    }
}