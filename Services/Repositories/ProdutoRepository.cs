using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models;

namespace WpfApp.Services.Repositories
{
    public class ProdutoRepository
    {
        private readonly IDataStore _ds;
        private const string File = "produtos.json";
        private List<Produto> _listProdutos;
        private readonly List<string> _listErrors = new List<string>();
        public IReadOnlyCollection<string> ListErrors => _listErrors;

        public ProdutoRepository(IDataStore ds)
        {
            _ds = ds;
            _listProdutos = _ds.Load<Produto>(File);
        }

        public IEnumerable<Produto> Query(string nome = null, string codigo = null, decimal? min = null, decimal? max = null)
        {
            return _listProdutos.Where(p =>
                (string.IsNullOrWhiteSpace(nome) || p.Nome.IndexOf(nome, StringComparison.OrdinalIgnoreCase) >= 0)
                &&
                (string.IsNullOrWhiteSpace(codigo) || p.Codigo.IndexOf(codigo, StringComparison.OrdinalIgnoreCase) >= 0)
                &&
                (!min.HasValue || p.Valor >= min.Value)
                &&
                (!max.HasValue || p.Valor <= max.Value)
            );
        }

        private void ValidaProduto(Produto p)
        {
            var resultName = NameValidator.Validator(p.Nome);
            if (!resultName.Item1) _listErrors.AddRange(resultName.Item2);
            
            if (string.IsNullOrWhiteSpace(p.Codigo)) _listErrors.Add("Código obrigatório");
            
            if (p.Valor <= 0) _listErrors.Add("Valor inválido");
        }

        public Produto Add(Produto p)
        {
            ValidaProduto(p);

            if (_listErrors.Any()) return null;

            p.Id = _listProdutos.Any() ? _listProdutos.Max(x => x.Id) + 1 : 1;
            _listProdutos.Add(p); 
            _ds.Save(File, _listProdutos); 
            return p;
        }

        public Produto Update(Produto p)
        {
            var idx = _listProdutos.FindIndex(x => x.Id == p.Id); 
            if (idx < 0) _listErrors.Add("Produto não encontrado");

            ValidaProduto(p);

            if (_listErrors.Any()) return null;

            _listProdutos[idx] = p; _ds.Save(File, _listProdutos);
            return p;
        }

        public void Delete(int id) 
        { 
            _listProdutos.RemoveAll(x => x.Id == id); 
            _ds.Save(File, _listProdutos); 
        }
        public List<Produto> All() => _listProdutos.ToList();
    }
}