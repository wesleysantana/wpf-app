using System.Collections.ObjectModel;
using WpfApp.Models;
using WpfApp.Services.Repositories;
using WpfApp.ViewModels.Base;

namespace WpfApp.ViewModels
{
    public class ProdutosViewModel : BaseViewModel
    {
        private readonly ProdutoRepository _repo;
        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();
        public string FiltroNome { get; set; }
        public string FiltroCodigo { get; set; }
        public decimal? FiltroMin { get; set; }
        public decimal? FiltroMax { get; set; }
        public Produto ProdutoSelecionado { get; set; }

        public RelayCommand FiltrarCmd { get; }
        public RelayCommand IncluirCmd { get; }
        public RelayCommand SalvarCmd { get; }
        public RelayCommand ExcluirCmd { get; }

        public ProdutosViewModel(ProdutoRepository repo)
        {
            _repo = repo;
            FiltrarCmd = new RelayCommand(_ => Carregar());
            IncluirCmd = new RelayCommand(_ => Incluir());
            SalvarCmd = new RelayCommand(_ => Salvar());
            ExcluirCmd = new RelayCommand(_ => Excluir());
            Carregar();
        }

        private void Carregar()
        {
            Produtos.Clear();
            foreach (var p in _repo.Query(FiltroNome, FiltroCodigo, FiltroMin, FiltroMax)) 
                Produtos.Add(p);
        }

        private void Incluir()
        {
            var p = new Produto();
            Produtos.Add(p);
            ProdutoSelecionado = p;
        }

        private void Salvar()
        {
            if (ProdutoSelecionado.Id == 0)
                _repo.Add(ProdutoSelecionado);
            else
                _repo.Update(ProdutoSelecionado);
            Carregar();
        }

        private void Excluir()
        {
            if (ProdutoSelecionado == null) return;
            _repo.Delete(ProdutoSelecionado.Id);
            Carregar();
        }
    }
}