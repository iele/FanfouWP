using FanfouWP.API.Items;
using FanfouWP.ItemControls.TemplateSelector;
using System.Windows;
using System.Windows.Controls;

namespace FanfouWP.ItemControls.TemplateSelector
{
    
    public class ConversationItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ConversationOtherTemplate { get; set; }

        public DataTemplate ConversationSelfTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var i = item as DirectMessageItem;
            if (i.dm.sender_id == FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
                return ConversationSelfTemplate;
            return ConversationOtherTemplate;
        }
    }
}