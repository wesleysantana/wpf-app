using System;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp.Models;
using WpfApp.Services.Repositories;
using WpfApp.ViewModels.Base;

namespace WpfApp.ViewModels
{
    public class PedidosViewModel : BaseViewModel
    {
        private readonly PedidoRepository _pedidos;
        private readonly ProdutoRepository _produtos;

        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();
        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();
        public ObservableCollection<PedidoItem> Itens { get; } = new ObservableCollection<PedidoItem>();

        public Pessoa PessoaSelecionada { get; set; }
        public Produto ProdutoSelecionado { get; set; }
        public int Quantidade { get; set; } = 1;
        public FormaPagamento FormaPagamento { get; set; }
        public decimal ValorTotal => Itens.Sum(i => i.Subtotal);

        public RelayCommand AdicionarItemCmd { get; }
        public RelayCommand RemoverItemCmd { get; }
        public RelayCommand FinalizarCmd { get; }
        public RelayCommand CancelarCmd { get; }

        public PedidosViewModel(PedidoRepository pedidos, ProdutoRepository produtos, Pessoa pessoaPreSelecionada = null)
        {
            _pedidos = pedidos;
            _produtos = produtos;

            AddProdutos();

            PessoaSelecionada = pessoaPreSelecionada;

            AdicionarItemCmd = new RelayCommand(_ => AdicionarItem());
            RemoverItemCmd = new RelayCommand(item => RemoverItem((PedidoItem)item));

            FinalizarCmd = new RelayCommand(_ => Finalizar());

            CancelarCmd = new RelayCommand(_ => CloseRequested?.Invoke(this, false));
        }

        private void AddProdutos()
        {
            foreach (var pr in _produtos.All())
                Produtos.Add(pr);
        }

        private void AdicionarItem()
        {
            if (ProdutoSelecionado == null || Quantidade <= 0) return;
            Itens.Add(new PedidoItem
            {
                ProdutoId = ProdutoSelecionado.Id,
                ProdutoNome = ProdutoSelecionado.Nome,
                ValorUnitario = ProdutoSelecionado.Valor,
                Quantidade = Quantidade
            });
            OnPropertyChanged(nameof(ValorTotal));
        }

        private void RemoverItem(PedidoItem item)
        {
            var it = item as PedidoItem;
            if (it == null) return;

            Itens.Remove(it);
            OnPropertyChanged(nameof(ValorTotal));
        }

        private void Finalizar()
        {
            if (PessoaSelecionada == null)
            {
                System.Windows.MessageBox.Show("Selecione a pessoa.");
                return;
            }

            if (!Itens.Any())
            {
                System.Windows.MessageBox.Show("Adicione ao menos um produto.");
                return;
            }

            var pedido = new Pedido
            {
                PessoaId = PessoaSelecionada.Id,
                PessoaNome = PessoaSelecionada.Nome,
                Itens = Itens.ToList(),
                FormaPagamento = FormaPagamento,
                Status = StatusPedido.Pendente,
                Finalizado = true
            };
            _pedidos.Add(pedido);
            CloseRequested?.Invoke(this, true);
        }

        public event EventHandler<bool?> CloseRequested;
    }
}