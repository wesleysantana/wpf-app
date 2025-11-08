using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models;

namespace WpfApp.Services.Repositories
{
    public class ProdutoRepository
    {
        private readonly IDataStore _ds;
        private const string FILE = "produtos.json";
        private readonly List<Produto> _produtos;

        public ProdutoRepository(IDataStore ds)
        {
            _ds = ds;
            _produtos = _ds.Load<Produto>(FILE).Where(x => x.Ativo).ToList() ?? new List<Produto>();
        }

        public IEnumerable<Produto> Query(string nome = null, string codigo = null, 
            decimal? min = null, decimal? max = null, bool incluirInativos = false)
        {
            var q = _produtos.AsEnumerable();
            if (!incluirInativos) q = q.Where(p => p.Ativo);
            if (!string.IsNullOrWhiteSpace(nome)) q = q.Where(p => p.Nome?.IndexOf(nome, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(codigo)) q = q.Where(p => p.Codigo?.IndexOf(codigo, StringComparison.OrdinalIgnoreCase) >= 0);
            if (min.HasValue) q = q.Where(p => p.Valor >= min.Value);
            if (max.HasValue) q = q.Where(p => p.Valor <= max.Value);
            return q;
        }

        public Produto Add(Produto p)
        {
            p.Id = _produtos.Any() ? _produtos.Max(x => x.Id) + 1 : 1;
            p.Ativo = true;
            _produtos.Add(p);
            _ds.Save(FILE, _produtos);
            return p;
        }

        public Produto Update(Produto p)
        {
            var idx = _produtos.FindIndex(x => x.Id == p.Id);
            if (idx < 0) return null;
            _produtos[idx] = p;
            _ds.Save(FILE, _produtos);
            return p;
        }

        // Exclusão lógica
        public bool SoftDelete(int id)
        {
            var p = _produtos.FirstOrDefault(x => x.Id == id);
            if (p == null) return false;
            p.Ativo = false;
            _ds.Save(FILE, _produtos);
            return true;
        }

        public List<Produto> All() => _produtos.ToList();
    }
}