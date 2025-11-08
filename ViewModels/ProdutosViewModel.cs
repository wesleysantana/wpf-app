using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services.Repositories;
using WpfApp.ViewModels.Base;

namespace WpfApp.ViewModels
{
    public class ProdutosViewModel : BaseViewModel
    {
        private readonly ProdutoRepository _repo;
        private bool _isNovo;

        public ObservableCollection<Produto> ProdutosCollection { get; } = new ObservableCollection<Produto>();

        // Filtros
        public string FiltroNome { get; set; }
        public string FiltroCodigo { get; set; }
        public decimal? FiltroMin { get; set; }
        public decimal? FiltroMax { get; set; }

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set
            {
                _produtoSelecionado = value;
                OnPropertyChanged();

                if (_produtoSelecionado != null)
                {
                    ProdutoEdicao = CloneProduto(_produtoSelecionado);
                    IsCamposHabilitados = true;
                    _isNovo = false;
                }
                else
                {
                    ProdutoEdicao = new Produto();
                    IsCamposHabilitados = false;
                    _isNovo = false;
                }

                CommandManager.InvalidateRequerySuggested(); // reavalia CanExecute
            }
        }

        private Produto _produtoEdicao = new Produto();
        public Produto ProdutoEdicao
        {
            get => _produtoEdicao;
            set
            {
                _produtoEdicao = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PodeSalvar));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool _isCamposHabilitados;
        public bool IsCamposHabilitados
        {
            get => _isCamposHabilitados;
            set
            {
                _isCamposHabilitados = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Commands
        public RelayCommand FiltrarCmd { get; }
        public RelayCommand IncluirCmd { get; }
        public RelayCommand SalvarCmd { get; }
        public RelayCommand CancelarCmd { get; }
        public RelayCommand ExcluirCmd { get; }
        public RelayCommand LimparFiltrosCmd { get; }

        public ProdutosViewModel(ProdutoRepository repo)
        {
            _repo = repo;

            FiltrarCmd = new RelayCommand(_ => CarregarProdutos());
            LimparFiltrosCmd = new RelayCommand(_ => { LimparFiltros(); CarregarProdutos(); });

            IncluirCmd = new RelayCommand(_ => IncluirProduto());
            SalvarCmd = new RelayCommand(_ => SalvarProduto(), _ => PodeSalvar);
            CancelarCmd = new RelayCommand(_ => CancelarEdicao());
            ExcluirCmd = new RelayCommand(_ => ExcluirProduto(), _ => ProdutoSelecionado != null);

            IsCamposHabilitados = false;
            ProdutoEdicao = new Produto();
            CarregarProdutos();
        }

        public bool PodeSalvar
        {
            get
            {
                if (!IsCamposHabilitados || ProdutoEdicao == null) return false;

                // Notificação do Model, INotifyDataErrorInfo
                if (ProdutoEdicao is INotifyDataErrorInfo notifier && notifier.HasErrors) return false;
                
                if (ProdutoEdicao.Valor <= 0) return false;

                return true;
            }
        }

        private void IncluirProduto()
        {
            _isNovo = true;
            IsCamposHabilitados = true;
            ProdutoEdicao = new Produto
            {
                // Força mostrar obrigatórios vazios (INotifyDataErrorInfo no Model)
                Nome = string.Empty,
                Codigo = string.Empty,
                Valor = 0
            };

            OnPropertyChanged(nameof(ProdutoEdicao));
        }

        private void CancelarEdicao()
        {
            ProdutoEdicao = new Produto();
            IsCamposHabilitados = false;
            _isNovo = false;
        }

        private void SalvarProduto()
        {
            // Validação via model (se houver) + checagem extra
            if (!PodeSalvar)
            {
                MessageBox.Show("Corrija os erros antes de salvar.", "Validação", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Produto result;
                if (_isNovo)
                {
                    result = _repo.Add(ProdutoEdicao);
                }
                else
                {
                    if (ProdutoSelecionado == null)
                    {
                        MessageBox.Show("Nenhum produto selecionado.", "Atenção", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    
                    ProdutoSelecionado.Nome = ProdutoEdicao.Nome;
                    ProdutoSelecionado.Codigo = ProdutoEdicao.Codigo;
                    ProdutoSelecionado.Valor = ProdutoEdicao.Valor;
                    
                    result = _repo.Update(ProdutoSelecionado);
                }

                if (result == null)
                {
                    MessageBox.Show("Não foi possível salvar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CarregarProdutos();
                ProdutoSelecionado = ProdutosCollection.FirstOrDefault(p => p.Id == result.Id);

                ProdutoEdicao = new Produto();
                IsCamposHabilitados = false;
                _isNovo = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExcluirProduto()
        {
            if (ProdutoSelecionado == null) return;

            // Exclusão lógica (inativar)
            if (MessageBox.Show("Marcar o produto como INATIVO?", "Confirmação",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            var ok = _repo.SoftDelete(ProdutoSelecionado.Id);
            if (!ok)
            {
                MessageBox.Show("Não foi possível inativar o produto.", "Erro", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CarregarProdutos();
            ProdutoSelecionado = null;
            ProdutoEdicao = new Produto();
            IsCamposHabilitados = false;
            _isNovo = false;
        }

        private void CarregarProdutos()
        {
            ProdutosCollection.Clear();
            var query = _repo.Query(FiltroNome, FiltroCodigo, FiltroMin, FiltroMax);
            foreach (var p in query)
                ProdutosCollection.Add(p);

            // Atualiza CanExecute de Excluir/Salvar
            CommandManager.InvalidateRequerySuggested();
        }

        private void LimparFiltros()
        {
            FiltroNome = null;
            FiltroCodigo = null;
            FiltroMin = null;
            FiltroMax = null;
            OnPropertyChanged(nameof(FiltroNome));
            OnPropertyChanged(nameof(FiltroCodigo));
            OnPropertyChanged(nameof(FiltroMin));
            OnPropertyChanged(nameof(FiltroMax));
        }

        private static Produto CloneProduto(Produto p)
        {
            if (p == null) return null;
            return new Produto
            {
                Id = p.Id,
                Nome = p.Nome,
                Codigo = p.Codigo,
                Valor = p.Valor,
                Ativo = p.Ativo
            };
        }
    }
}