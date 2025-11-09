using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WpfApp.ViewModels.Helpers
{
    public sealed class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> collection)
        {
            // Impede notificações durante a adição
            CheckReentrancy();

            foreach (var item in collection)
            {
                Items.Add(item);
            }

            // Dispara um único evento de notificação
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}