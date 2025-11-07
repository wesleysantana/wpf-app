using System.Collections.Generic;

namespace WpfApp.Services
{
    public interface IDataStore
    {
        List<T> Load<T>(string fileName);

        void Save<T>(string fileName, List<T> data);
    }
}