using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.Services.Repositories;
using WpfApp.ViewModels.Base;
using WpfApp.ViewModels.Helpers;

namespace WpfApp.ViewModels
{
    public class PessoasViewModel : BaseViewModel
    {
        private readonly PessoaRepository _pessoasRepo;
        private readonly PedidoRepository _pedidosRepo;
        private readonly ProdutoRepository _produtosRepo;

        private bool _isNovo;

        public ObservableRangeCollection<Pessoa> PessoasCollection { get; } = new ObservableRangeCollection<Pessoa>();
        public ObservableRangeCollection<Pedido> PedidosDaPessoaCollection { get; } = new ObservableRangeCollection<Pedido>();

        public string FiltroNome { get; set; }
        public string FiltroCpf { get; set; }

        private Pessoa _pessoaSelecionada;
        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set
            {
                _pessoaSelecionada = value;
                OnPropertyChanged();

                if (_pessoaSelecionada != null)
                {
                    PessoaEdicao = ClonePessoa(_pessoaSelecionada);
                    IsCamposHabilitados = true;
                    _isNovo = false;
                }
                else
                {
                    PessoaEdicao = new Pessoa();
                    IsCamposHabilitados = false;
                    _isNovo = false;
                }

                CarregarPedidos();

                // Atualiza CanExecute (Salvar/Excluir/Incl. Pedido)
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private Pessoa _pessoaEdicao = new Pessoa();
        public Pessoa PessoaEdicao
        {
            get => _pessoaEdicao;
            set
            {
                _pessoaEdicao = value;
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
                OnPropertyChanged(nameof(PodeSalvar));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsSomenteEntregues { get; set; }
        public bool IsSomentePagos { get; set; }
        public bool IsSomentePendentesPgto { get; set; }

        // Commands
        public RelayCommand FiltrarCmd { get; }
        public RelayCommand IncluirCmd { get; }
        public RelayCommand SalvarCmd { get; }
        public RelayCommand CancelarCmd { get; }
        public RelayCommand ExcluirCmd { get; }
        public RelayCommand IncluirPedidoCmd { get; }
        public RelayCommand ConsultaPedidosCmd { get; }

        public RelayCommand MarcarPagoCmd { get; }
        public RelayCommand MarcarEnviadoCmd { get; }
        public RelayCommand MarcarRecebidoCmd { get; }
        public RelayCommand AplicarFiltrosPedidosCmd { get; }

        public PessoasViewModel(PessoaRepository pessoasRepo, PedidoRepository pedidosRepo, ProdutoRepository produtosRepo)
        {
            _pessoasRepo = pessoasRepo;
            _pedidosRepo = pedidosRepo;
            _produtosRepo = produtosRepo;

            FiltrarCmd = new RelayCommand(_ => CarregarPessoas());
            IncluirCmd = new RelayCommand(_ => IncluirPessoa());
            SalvarCmd = new RelayCommand(_ => SalvarPessoa(), _ => PodeSalvar);
            CancelarCmd = new RelayCommand(_ => CancelarEdicao());
            ExcluirCmd = new RelayCommand(_ => ExcluirPessoa(), _ => PessoaSelecionada != null);

            IncluirPedidoCmd = new RelayCommand(_ => AbrirTelaPedidosParaPessoa(PessoaSelecionada), _ => !_isNovo);
            ConsultaPedidosCmd = new RelayCommand(_ => AbrirTelaConsultaPedidos(PessoaSelecionada), _ => PessoaSelecionada != null);

            MarcarPagoCmd = new RelayCommand(id => { _pedidosRepo.UpdateStatus((int)id, StatusPedido.Pago); CarregarPedidos(); });
            MarcarEnviadoCmd = new RelayCommand(id => { _pedidosRepo.UpdateStatus((int)id, StatusPedido.Enviado); CarregarPedidos(); });
            MarcarRecebidoCmd = new RelayCommand(id => { _pedidosRepo.UpdateStatus((int)id, StatusPedido.Recebido); CarregarPedidos(); });

            AplicarFiltrosPedidosCmd = new RelayCommand(_ => CarregarPedidos());

            // Estado inicial
            IsCamposHabilitados = false;
            PessoaEdicao = new Pessoa();
            CarregarPessoas();
            _produtosRepo = produtosRepo;
        }

        public bool PodeSalvar
        {
            get
            {
                if (!IsCamposHabilitados || PessoaEdicao == null) return false;

                var notifier = PessoaEdicao as System.ComponentModel.INotifyDataErrorInfo;

                // Pode salvar se NÃO há erros
                return notifier == null || !notifier.HasErrors;
            }
        }

        private void IncluirPessoa()
        {
            _isNovo = true;
            IsCamposHabilitados = true;

            // limpa e já "força" validação (campos obrigatórios em vermelho)
            PessoaEdicao = new Pessoa();
            PessoaEdicao.Nome = string.Empty; // dispara validação do Model
            PessoaEdicao.CPF = string.Empty; // dispara validação do Model            

            OnPropertyChanged(nameof(PessoaEdicao));
            OnPropertyChanged(nameof(PodeSalvar));
            CommandManager.InvalidateRequerySuggested();
        }

        private void CancelarEdicao()
        {
            PessoaEdicao = new Pessoa();
            IsCamposHabilitados = false;
            _isNovo = false;
            OnPropertyChanged(nameof(PodeSalvar));
            CommandManager.InvalidateRequerySuggested();
        }

        private void SalvarPessoa()
        { 
            var notifier = PessoaEdicao as System.ComponentModel.INotifyDataErrorInfo;
            if (notifier != null && notifier.HasErrors)
            {
                MessageBox.Show("Corrija os erros antes de salvar.", "Validação", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Pessoa result;
                if (_isNovo)
                {
                    if (_pessoasRepo.GetPessoaCPF(PessoaEdicao.CPF) != null)
                    {
                        MessageBox.Show("Já existe uma pessoa cadastrada com este CPF.",
                            "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    result = _pessoasRepo.Add(PessoaEdicao);
                }
                else
                {
                    if (PessoaSelecionada == null)
                    {
                        MessageBox.Show("Nenhuma pessoa selecionada para atualização.", "Atenção", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    PessoaSelecionada.Nome = PessoaEdicao.Nome;
                    PessoaSelecionada.CPF = PessoaEdicao.CPF;
                    PessoaSelecionada.Endereco = PessoaEdicao.Endereco;

                    result = _pessoasRepo.Update(PessoaSelecionada);                    
                }

                if (result == null)
                {
                    MessageBox.Show("Não foi possível salvar o registro.", "Erro", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Atualiza grade e seleção
                CarregarPessoas();

                // Limpa e desabilita
                PessoaEdicao = new Pessoa();
                IsCamposHabilitados = false;
                _isNovo = false;
                
                OnPropertyChanged(nameof(PodeSalvar));
                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExcluirPessoa()
        {
            if (PessoaSelecionada == null) return;

            if (MessageBox.Show("Confirma exclusão da pessoa selecionada?",
                "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            _pessoasRepo.Delete(PessoaSelecionada.Id);

            CarregarPessoas();
            PessoaSelecionada = null;
            PessoaEdicao = new Pessoa();
            IsCamposHabilitados = false;
            _isNovo = false;

            OnPropertyChanged(nameof(PodeSalvar));
            CommandManager.InvalidateRequerySuggested();
        }

        private void CarregarPessoas()
        {
            PessoasCollection.Clear();
            var query = _pessoasRepo.Query(FiltroNome, FiltroCpf);

            PessoasCollection.AddRange(query);
        }

        private void CarregarPedidos()
        {
            PedidosDaPessoaCollection.Clear();
            if (PessoaSelecionada == null) return;
            
            var q = _pedidosRepo.GetPessoaId(PessoaSelecionada.Id);

            if (IsSomenteEntregues)
                q = q.Where(p => p.Status == StatusPedido.Recebido);

            if (IsSomentePagos)
                q = q.Where(p => p.Status == StatusPedido.Pago
                                 || p.Status == StatusPedido.Enviado
                                 || p.Status == StatusPedido.Recebido);

            if (IsSomentePendentesPgto)
                q = q.Where(p => p.Status == StatusPedido.Pendente);
            

            PedidosDaPessoaCollection.AddRange(q.OrderByDescending(p => p.DataVenda));
        }

        private Pessoa ClonePessoa(Pessoa p)
        {
            if (p == null) return null;
            return new Pessoa
            {
                Id = p.Id,
                Nome = p.Nome,
                CPF = p.CPF,
                Endereco = p.Endereco
            };
        }

        private void AbrirTelaPedidosParaPessoa(Pessoa pessoa)
        {
            if (pessoa == null) return;

            var vm = new PedidosViewModel(_pessoasRepo, _pedidosRepo, _produtosRepo, pessoa);
            var win = new Views.PedidosWindow 
            { 
                DataContext = vm, 
                Owner = Application.Current.MainWindow 
            };
            var ok = win.ShowDialog();
            if (ok == true) CarregarPedidos();
        }

        private void AbrirTelaConsultaPedidos(Pessoa pessoa)
        {
            if (pessoa == null) return;

            var vm = new PedidosConsultaViewModel(_pessoasRepo, _pedidosRepo, _produtosRepo, pessoa);
            var win = new Views.PedidosConsultaWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            win.ShowDialog();
        }
    }
}
