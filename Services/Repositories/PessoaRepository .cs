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

        public PessoaRepository(IDataStore ds)
        {
            _ds = ds ?? throw new ArgumentNullException(nameof(ds));
            _listPessoas = _ds.Load<Pessoa>(FILE) ?? new List<Pessoa>();
        }

        public Pessoa GetPessoaCPF(string cpf) => _listPessoas.Where(p => p.CPF == cpf).FirstOrDefault();
        
        public IEnumerable<Pessoa> Query(string nome = null, string cpf = null)
        {
            return _listPessoas.Where(p =>
                (string.IsNullOrWhiteSpace(nome) || p.Nome?.IndexOf(nome, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (string.IsNullOrWhiteSpace(cpf) || (p.CPF?.Contains(cpf) ?? false))
            );
        }

        public Pessoa Add(Pessoa p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            p.Id = _listPessoas.Any() ? _listPessoas.Max(x => x.Id) + 1 : 1;
            _listPessoas.Add(p);
            _ds.Save(FILE, _listPessoas);
            return p;
        }

        public Pessoa Update(Pessoa p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            var idx = _listPessoas.FindIndex(x => x.Id == p.Id);
            if (idx < 0) throw new InvalidOperationException("Pessoa não encontrada.");

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