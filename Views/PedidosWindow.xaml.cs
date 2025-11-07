using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    /// <summary>
    /// Interaction logic for PedidosWindow.xaml
    /// </summary>
    public partial class PedidosWindow : Window
    {
        public PedidosWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is PedidosViewModel vm)
                    vm.CloseRequested += (s2, ok) => { DialogResult = ok; Close(); };
            };
        }

        public string WindowTitle => "Pedidos";
    }
}