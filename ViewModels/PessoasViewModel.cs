using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.Services.Repositories;
using WpfApp.ViewModels.Base;

namespace WpfApp.ViewModels
{
    public class PessoasViewModel : BaseViewModel
    {
        private readonly PessoaRepository _pessoas;
        private readonly PedidoRepository _pedidos;
        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();
        public ObservableCollection<Pedido> PedidosDaPessoa { get; } = new ObservableCollection<Pedido>();
        public string FiltroNome { get; set; }
        public string FiltroCpf { get; set; }

        private Pessoa _selecionada;
        public Pessoa PessoaSelecionada { 
            get => _selecionada; 
            set 
            { 
                _selecionada = value; 
                OnPropertyChanged(); 
                CarregarPedidos(); 
            }
        }

        public RelayCommand FiltrarCmd { get; }
        public RelayCommand IncluirCmd { get; }
        public RelayCommand SalvarCmd { get; }
        public RelayCommand ExcluirCmd { get; }
        public RelayCommand IncluirPedidoCmd { get; }

        public RelayCommand MarcarPagoCmd { get; }
        public RelayCommand MarcarEnviadoCmd { get; }
        public RelayCommand MarcarRecebidoCmd { get; }

        public bool SomenteEntregues { get; set; }
        public bool SomentePagos { get; set; }
        public bool SomentePendentesPgto { get; set; }
        public RelayCommand AplicarFiltrosPedidosCmd { get; }

        public PessoasViewModel(PessoaRepository pessoas, PedidoRepository pedidos)
        {
            _pessoas = pessoas; _pedidos = pedidos;
            FiltrarCmd = new RelayCommand(_ => CarregarPessoas());
            IncluirCmd = new RelayCommand(_ => IncluirPessoa());
            SalvarCmd = new RelayCommand(_ => SalvarPessoa());
            ExcluirCmd = new RelayCommand(_ => DeletePessoa());

            IncluirPedidoCmd = new RelayCommand(_ => IncluirPedido());

            MarcarPagoCmd = new RelayCommand(id => _pedidos.UpdateStatus((int)id, StatusPedido.Pago));
            MarcarEnviadoCmd = new RelayCommand(id => _pedidos.UpdateStatus((int)id, StatusPedido.Enviado));
            MarcarRecebidoCmd = new RelayCommand(id => _pedidos.UpdateStatus((int)id, StatusPedido.Recebido));

            AplicarFiltrosPedidosCmd = new RelayCommand(_ => CarregarPedidos());
            CarregarPessoas();
        }

        private void IncluirPessoa()
        {
            var p = new Pessoa { Nome = "", CPF = "" };
            Pessoas.Add(p);
            PessoaSelecionada = p;
        }

        private void SalvarPessoa()
        {
            if (PessoaSelecionada.Id == 0) 
                _pessoas.Add(PessoaSelecionada);
            else 
                _pessoas.Update(PessoaSelecionada);

            CarregarPessoas();
        }

        private void DeletePessoa()
        {
            if (PessoaSelecionada == null) return;
            _pessoas.Delete(PessoaSelecionada.Id);
            CarregarPessoas();
        }

        private void IncluirPedido()
        {
            if (PessoaSelecionada == null) return;
            AbrirTelaPedidosParaPessoa(PessoaSelecionada);
        }       

        private void CarregarPessoas()
        {
            Pessoas.Clear();
            foreach (var p in _pessoas.Query(FiltroNome, FiltroCpf)) Pessoas.Add(p);
        }

        private void CarregarPedidos()
        {
            PedidosDaPessoa.Clear();
            if (PessoaSelecionada == null) return;
            var q = _pedidos.GetPessoaId(PessoaSelecionada.Id);
            if (SomenteEntregues) q = q.Where(p => p.Status == StatusPedido.Recebido);
            if (SomentePagos) q = q.Where(p => p.Status == StatusPedido.Pago || p.Status == StatusPedido.Enviado || p.Status == StatusPedido.Recebido);
            if (SomentePendentesPgto) q = q.Where(p => p.Status == StatusPedido.Pendente);
            foreach (var ped in q.OrderByDescending(p => p.DataVenda)) PedidosDaPessoa.Add(ped);
        }

        private void AbrirTelaPedidosParaPessoa(Pessoa pessoa)
        {
            var vm = new PedidosViewModel(_pedidos, new ProdutoRepository(new JsonDataStore("Data")), pessoa);
            var win = new Views.PedidosWindow { DataContext = vm, Owner = Application.Current.MainWindow };
            var ok = win.ShowDialog();
            if (ok == true) CarregarPedidos();
        }
    }
}