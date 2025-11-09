using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.Services.Repositories;
using WpfApp.ViewModels.Base;

namespace WpfApp.ViewModels
{
    public class PedidosViewModel : BaseViewModel
    {
        private readonly PedidoRepository _pedidosRepo;
        private readonly ProdutoRepository _produtosRepo;
        private readonly PessoaRepository _pessoasRepo;
        public bool IsFinalizado { get; private set; }

        public ObservableCollection<Pessoa> PessoasCollection { get; }
        public ObservableCollection<Produto> ProdutosCollection { get; }
        public ObservableCollection<PedidoItem> ItensCollection { get; } = new ObservableCollection<PedidoItem>();

        public Pessoa PessoaSelecionada { get; set; }
        public Produto ProdutoSelecionado { get; set; }
        public int Quantidade { get; set; } = 1;
        public FormaPagamento FormaPagamento { get; set; }
        public decimal ValorTotal => ItensCollection.Sum(i => i.Subtotal);

        public RelayCommand AdicionarItemCmd { get; }
        public RelayCommand RemoverItemCmd { get; }
        public RelayCommand FinalizarCmd { get; }
        public RelayCommand CancelarCmd { get; }

        public PedidosViewModel(PessoaRepository pessoaRepo, PedidoRepository pedidos, ProdutoRepository produtos, Pessoa pessoaPreSelecionada = null)
        {
            _pedidosRepo = pedidos;
            _produtosRepo = produtos;
            _pessoasRepo = pessoaRepo;

            // Carrega pessoas e produtos
            PessoasCollection = new ObservableCollection<Pessoa>(_pessoasRepo.All());
            ProdutosCollection = new ObservableCollection<Produto>(_produtosRepo.All());

            PessoaSelecionada = pessoaPreSelecionada;

            AdicionarItemCmd = new RelayCommand(_ => AdicionarItem(), _ => PodeAdicionarItem);

            RemoverItemCmd = new RelayCommand(item => RemoverItem((PedidoItem)item), _ => !IsFinalizado);

            FinalizarCmd = new RelayCommand(_ => Finalizar(), _ => PodeFinalizar);

            CancelarCmd = new RelayCommand(_ => CloseRequested?.Invoke(this, false));
        }       

        public bool PodeAdicionarItem => ProdutoSelecionado != null && Quantidade > 0 && !IsFinalizado;

        public bool PodeFinalizar =>
            PessoaSelecionada != null &&
            ItensCollection.Any() &&
            !IsFinalizado;

        private void AdicionarItem()
        {
            if (!PodeAdicionarItem) return;
            ItensCollection.Add(new PedidoItem
            {
                ProdutoId = ProdutoSelecionado.Id,
                ProdutoNome = ProdutoSelecionado.Nome,
                ValorUnitario = ProdutoSelecionado.Valor,
                Quantidade = Quantidade
            });
            OnPropertyChanged(nameof(ValorTotal));
            CommandManager.InvalidateRequerySuggested();
        }

        private void RemoverItem(PedidoItem item)
        {
            if (item == null || IsFinalizado) return;
            ItensCollection.Remove(item);
            OnPropertyChanged(nameof(ValorTotal));
            CommandManager.InvalidateRequerySuggested();
        }

        private void Finalizar()
        {
            if (!PodeFinalizar) return;

            if (PessoaSelecionada == null)
            {
                System.Windows.MessageBox.Show("Selecione a pessoa.");
                return;
            }

            if (!ItensCollection.Any())
            {
                System.Windows.MessageBox.Show("Adicione ao menos um produto.");
                return;
            }

            var pedido = new Pedido
            {
                PessoaId = PessoaSelecionada.Id,
                PessoaNome = PessoaSelecionada.Nome,
                Itens = ItensCollection.ToList(),
                FormaPagamento = FormaPagamento,
                Status = StatusPedido.Pendente,
                Finalizado = true
            };
            _pedidosRepo.Add(pedido);
            IsFinalizado = true;
            CommandManager.InvalidateRequerySuggested();
            CloseRequested?.Invoke(this, true);
        }

        public event EventHandler<bool?> CloseRequested;
    }
}