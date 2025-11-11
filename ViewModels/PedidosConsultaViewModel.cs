using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.Services.Repositories;
using WpfApp.ViewModels.Base;
using WpfApp.ViewModels.Helpers;

namespace WpfApp.ViewModels
{
    public class PedidosConsultaViewModel : BaseViewModel
    {
        private readonly PessoaRepository _pessoasRepo;
        private readonly PedidoRepository _pedidosRepo;
        private readonly ProdutoRepository _produtosRepo;
        private bool isInit = true;

        private readonly Pessoa _all = new Pessoa { Id = 0, Nome = "(Todos)" };

        public ObservableRangeCollection<Pessoa> PessoasCollection { get; } = new ObservableRangeCollection<Pessoa>();
        public ObservableRangeCollection<Pedido> PedidosCollection { get; } = new ObservableRangeCollection<Pedido>();

        
        public ObservableCollection<KeyValuePair<string, FormaPagamento?>> PagamentosFiltro { get; }
            = new ObservableCollection<KeyValuePair<string, FormaPagamento?>>();

        
        private FormaPagamento? _filtroFormaPagamento;
        public FormaPagamento? FiltroFormaPagamento
        {
            get => _filtroFormaPagamento;
            set
            { 
                _filtroFormaPagamento = value; 
                OnPropertyChanged();
            }
        }


        private Pessoa _pessoaSelecionada;

        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set
            {
                _pessoaSelecionada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PodeNovoPedido));
            }
        }

        public bool IsPendentes { get; set; }
        public bool IsPagos { get; set; }        
        public bool IsEnviados { get; set; }
        public bool IsRecebidos { get; set; }
        public decimal TotalValorPedidos { get; set; }

        public int TotalPedidos => PedidosCollection.Count;    
        public bool PodeNovoPedido => PessoaSelecionada != null && PessoaSelecionada.Id > 0;

        // Commands
        public RelayCommand FiltrarCmd { get; }
        public RelayCommand NovoPedidoCmd { get; }
        public RelayCommand MarcarPagoCmd { get; }
        public RelayCommand MarcarEnviadoCmd { get; }
        public RelayCommand MarcarRecebidoCmd { get; }

        public PedidosConsultaViewModel(PessoaRepository pessoasRepo, 
            PedidoRepository pedidosRepo, ProdutoRepository produtosRepo, Pessoa pessoa = null)
        {
            _pessoasRepo = pessoasRepo;
            _pedidosRepo = pedidosRepo;
            _produtosRepo = produtosRepo;           

            FiltrarCmd = new RelayCommand(_ => CarregarPedidos());
            NovoPedidoCmd = new RelayCommand(_ => AbrirNovoPedido(), _ => PodeNovoPedido);

            MarcarPagoCmd = new RelayCommand(id => { _pedidosRepo.UpdateStatus((int)id, StatusPedido.Pago); CarregarPedidos(); });
            MarcarEnviadoCmd = new RelayCommand(id => { _pedidosRepo.UpdateStatus((int)id, StatusPedido.Enviado); CarregarPedidos(); });
            MarcarRecebidoCmd = new RelayCommand(id => { _pedidosRepo.UpdateStatus((int)id, StatusPedido.Recebido); CarregarPedidos(); });

            // carregar pessoas e pedidos iniciais
            PessoasCollection.Add(_all); // adiciona item vazio
            PessoasCollection.AddRange(_pessoasRepo.All());
            PessoaSelecionada = pessoa ?? _all;
            CarregarPedidos();
        }

        private void CarregarPedidos()
        {
            if (isInit && PessoaSelecionada == null)
            {
                // evita carregamento de todos os pedidos na inicialização
                isInit = false;

                PagamentosFiltro.Add(new KeyValuePair<string, FormaPagamento?>("(Todos)", null));
                foreach (FormaPagamento fp in Enum.GetValues(typeof(FormaPagamento)))
                    PagamentosFiltro.Add(new KeyValuePair<string, FormaPagamento?>(fp.ToString(), fp));

                FiltroFormaPagamento = null;

                return;
            }
            PedidosCollection.Clear();
            if (PessoaSelecionada == null)
            {
                OnPropertyChanged(nameof(TotalPedidos));
                return;
            }

            IEnumerable<Pedido> q;

            // Filtro por pessoa
            if (PessoaSelecionada == null || PessoaSelecionada.Id == 0)
                q = _pedidosRepo.All();        
            else
                q = _pedidosRepo.GetPessoaId(PessoaSelecionada.Id);

            // Filtros de status 
            var statusSelecionados = new List<StatusPedido>();
            if (IsPendentes) statusSelecionados.Add(StatusPedido.Pendente);
            if (IsPagos) statusSelecionados.Add(StatusPedido.Pago);
            if (IsEnviados) statusSelecionados.Add(StatusPedido.Enviado);
            if (IsRecebidos) statusSelecionados.Add(StatusPedido.Recebido);

            if (statusSelecionados.Count > 0)
                q = q.Where(p => statusSelecionados.Contains(p.Status));

            // Filtro por forma de pagamento
            if (FiltroFormaPagamento.HasValue)
                q = q.Where(p => p.FormaPagamento == FiltroFormaPagamento.Value);


            PedidosCollection.AddRange(q.OrderByDescending(p => p.DataVenda));
            TotalValorPedidos = q.Sum(p => p.ValorTotal);
            OnPropertyChanged(nameof(TotalValorPedidos));
            OnPropertyChanged(nameof(TotalPedidos));
        }

        private void AbrirNovoPedido()
        {
            var vm = new PedidosViewModel(_pessoasRepo, _pedidosRepo, _produtosRepo, PessoaSelecionada);
            var win = new Views.PedidosWindow { DataContext = vm, Owner = Application.Current.MainWindow };
            var ok = win.ShowDialog();
            if (ok == true) CarregarPedidos();
        }
    }
}