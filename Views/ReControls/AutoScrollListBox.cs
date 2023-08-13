using System.Windows.Controls;

namespace WebChatClientApp.Views.ReControls
{

    public class AutoScrollListBox : ListBox
    {
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            // Прокручиваем к последнему элементу при добавлении новых элементов
            if (e.NewItems != null && Items.Count > 0)
            {
                ScrollIntoView(Items[Items.Count - 1]);
            }
        }
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            // Прокручиваем к последнему элементу при изменении ItemsSource
            if (newValue != null && Items.Count > 0)
            {
                ScrollIntoView(Items[Items.Count - 1]);
            }
        }
    }

}
