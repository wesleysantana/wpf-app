using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models;

namespace WpfApp.Services
{
    public class PessoaRepository
    {
        private readonly IDataStore _ds;
        private const string FILE = "pessoas.json";
        private readonly List<Pessoa> _listPessoas;
        private readonly List<string> _listErrors = new List<string>();
        public IReadOnlyCollection<string> ListErrors => _listErrors;        

        public PessoaRepository(IDataStore ds)
        {
            _ds = ds;
            _listPessoas = _ds.Load<Pessoa>(FILE);
        }

        public IEnumerable<Pessoa> Query(string nome = null, string cpf = null)
        {
            return _listPessoas.Where(p =>
                (string.IsNullOrWhiteSpace(nome) || p.Nome.IndexOf(nome, StringComparison.OrdinalIgnoreCase) >= 0)
                &&
                (string.IsNullOrWhiteSpace(cpf) || p.CPF.Contains(cpf))

            );
        }

        private void ValidaPessoa(Pessoa p)
        {
            var resultName = NameValidator.Validator(p.Nome);
            if (!resultName.Item1) _listErrors.AddRange(resultName.Item2);

            if (!CpfValidator.IsValid(p.CPF)) _listErrors.Add("CPF inválido.");
        }

        public Pessoa Add(Pessoa p)
        {
            ValidaPessoa(p);

            if (_listErrors.Any()) return null;

            p.Id = _listPessoas.Any() ? _listPessoas.Max(x => x.Id) + 1 : 1;
            _listPessoas.Add(p); 
            _ds.Save(FILE, _listPessoas); 

            return p;
        }

        public Pessoa Update(Pessoa p)
        {
            var idx = _listPessoas.FindIndex(x => x.Id == p.Id);             
            if (idx < 0) _listErrors.Add("Pessoa não encontrada");

            ValidaPessoa(p);

            if (_listErrors.Any()) return null;

            _listPessoas[idx] = p; 
            _ds.Save(FILE, _listPessoas);

            return p;
        }

        public void Delete(int id) 
        { 
            _listPessoas.RemoveAll(x => x.Id == id);
            _ds.Save(FILE, _listPessoas); 
        }

        public List<Pessoa> All() => _listPessoas.ToList();
    }
}