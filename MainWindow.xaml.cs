using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfApp.Services;
using WpfApp.Services.Repositories;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly JsonDataStore _ds = new JsonDataStore("Data");

        public MainWindow()
        {
            InitializeComponent();
            CarregarImagemBenner();
        }

        private void OpenPeople(object s, RoutedEventArgs e)
        {
            var vm = new PessoasViewModel(new PessoaRepository(_ds), new PedidoRepository(_ds), new ProdutoRepository(_ds));

            var win = new PessoasWindow
            {
                Owner = this,
                DataContext = vm
            };
            win.ShowDialog();
        }

        private void OpenProducts(object s, RoutedEventArgs e)
        {
            var win = new ProdutosWindow
            {
                DataContext = new ProdutosViewModel(new ProdutoRepository(_ds)),
                Owner = this
            };
            win.ShowDialog();
        }

        private void OpenOrders(object s, RoutedEventArgs e)
        {
            var win = new PedidosWindow
            {
                DataContext = new PedidosViewModel(
                    new PessoaRepository(_ds),
                    new PedidoRepository(_ds),
                    new ProdutoRepository(_ds)
                ),
                Owner = this
            };
            win.ShowDialog();
        }

        private void OpenConsultationOrders(object s, RoutedEventArgs e)
        {
            var win = new Views.PedidosConsultaWindow
            {
                DataContext = new PedidosConsultaViewModel(
                    new PessoaRepository(_ds),
                    new PedidoRepository(_ds),
                    new ProdutoRepository(_ds)
                ),
                Owner = this
            };
            win.ShowDialog();
        }

        private void CarregarImagemBenner()
        {
            string caminhoUri = "pack://application:,,,/Resources/Benner.png";

            Uri uriImagem = new Uri(caminhoUri, UriKind.Absolute);

            BitmapImage bitmap = new BitmapImage(uriImagem);

            imgBenner.Source = bitmap;
        }
    }
}