using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp.Services.Validators
{
    public abstract class ValidatableModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return;
            storage = value;
            OnPropertyChanged(propertyName);
            ValidateProperty(propertyName); // valida ao alterar
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();       

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            return _errors.TryGetValue(propertyName, out var list) ? list : null;
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errors.TryGetValue(propertyName, out var list))
            {
                list = new List<string>();
                _errors[propertyName] = list;
            }
            if (!list.Contains(error))
            {
                list.Add(error);
                RaiseErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
                RaiseErrorsChanged(propertyName);
        }

        protected void RaiseErrorsChanged(string propertyName)
            => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        /// <summary>
        /// Valide um único campo quando ele mudar.
        /// </summary>
        protected abstract void ValidateProperty(string propertyName);

        /// <summary>
        /// Valide tudo (útil antes de salvar).
        /// </summary>
        public void ValidateAll()
        {
            _errors.Clear();
            foreach (var prop in GetValidatableProperties())
                ValidateProperty(prop);
            // notifica UI para todos os campos
            foreach (var key in GetValidatableProperties())
                RaiseErrorsChanged(key);
        }

        /// <summary>
        /// Por padrão, retorna todas as propriedades públicas com setter.
        /// Você pode simplificar retornando um array fixo em cada Model.
        /// </summary>
        protected virtual IEnumerable<string> GetValidatableProperties() => Array.Empty<string>();
    }
}