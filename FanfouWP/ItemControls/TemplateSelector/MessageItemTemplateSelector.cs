using FanfouWP.API.Items;
using FanfouWP.ItemControls.TemplateSelector;
using System.Windows;
using System.Windows.Controls;

namespace FanfouWP.ItemControls.TemplateSelector
{
    
    public class MessageItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessageOtherTemplate { get; set; }

        public DataTemplate MessageSelfTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var i = item as DirectMessage;
            if (i.sender_id == FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
                return MessageSelfTemplate;
            return MessageOtherTemplate;
        }
    }
}