using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfApp.Models;
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
            //var win = new PessoasWindow
            //{
            //    DataContext = new PessoasViewModel(new PessoaRepository(_ds), new PedidoRepository(_ds)).PessoasCollection,
            //    Owner = this
            //};
            //win.ShowDialog();

            var vm = new PessoasViewModel(new PessoaRepository(_ds), new PedidoRepository(_ds));

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
                    new PedidoRepository(_ds),
                    new ProdutoRepository(_ds)
                ),
                Owner = this
            };
            win.ShowDialog();
        }

        private void CarregarImagemBenner()
        {            
            string caminhoUri = "/Resources/Benner.png";
           
            Uri uriImagem = new Uri(caminhoUri, UriKind.Relative);
            
            BitmapImage bitmap = new BitmapImage(uriImagem);
            
            imgBenner.Source = bitmap;
        }
    }
}