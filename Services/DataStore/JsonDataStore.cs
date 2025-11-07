using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace WpfApp.Services
{
    public class JsonDataStore : IDataStore
    {
        private readonly string _basePath;

        public JsonDataStore(string basePath)
        {
            _basePath = basePath;
            if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
        }

        public List<T> Load<T>(string fileName)
        {
            var path = Path.Combine(_basePath, fileName);            
            if (!File.Exists(path)) return new List<T>();
            
            var json = File.ReadAllText(path);
            
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public void Save<T>(string fileName, List<T> data)
        {
            var path = Path.Combine(_basePath, fileName);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}