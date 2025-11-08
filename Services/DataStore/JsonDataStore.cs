using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WpfApp.Services
{
    public class JsonDataStore : IDataStore
    {
        private readonly string _basePath;

        public JsonDataStore(string basePath)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;            
            var projectRoot = Directory.GetParent(baseDir).Parent.Parent.FullName;            
            _basePath = Path.Combine(projectRoot, basePath);

            Directory.CreateDirectory(_basePath);
        }

        public List<T> Load<T>(string fileName)
        {
            var path = Path.Combine(_basePath, fileName);

            if (!File.Exists(path)) return new List<T>();

            var json = File.ReadAllText(path, Encoding.UTF8);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public void Save<T>(string fileName, List<T> data)
        {
            var path = Path.Combine(_basePath, fileName);
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}