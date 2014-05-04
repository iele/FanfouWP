using FanfouWP.API.Items;
using System.Windows;
using System.Windows.Controls;

namespace FanfouWP.ItemControls
{
    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }
    public class StatusItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StatusTemplate { get; set; }

        public DataTemplate RefreshTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var i = item as Status;
            if (i.is_refresh == true)
                return RefreshTemplate;
            return StatusTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}