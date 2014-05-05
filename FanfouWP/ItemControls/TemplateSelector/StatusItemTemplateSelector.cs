using FanfouWP.API.Items;
using System.Windows;
using System.Windows.Controls;

namespace FanfouWP.ItemControls.TemplateSelector
{
  
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
        }
    }
}