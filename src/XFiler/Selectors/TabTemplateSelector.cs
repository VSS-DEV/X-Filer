﻿using System.Windows;
using System.Windows.Controls;
using XFiler.SDK;

namespace XFiler
{
    internal class TabTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ITabItem tabItem)
                return tabItem.Template;
            
            return base.SelectTemplate(item, container);
        }
    }
}