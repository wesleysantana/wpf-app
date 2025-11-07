using System;
using System.Collections.Generic;

namespace WpfApp.Services
{
    public class NameValidator
    {
        public static Tuple<bool, string[]> Validator(string name)
        {
            var listErros = new List<string>();
            if (string.IsNullOrWhiteSpace(name)) listErros.Add("Nome obrigatório");
            if (name != null && name.Length < 3) listErros.Add("Nome deve ter no mínimo 3 caracteres");

            return new Tuple<bool, string[]>(listErros.Count == 0, listErros.ToArray());
        }
    }
}