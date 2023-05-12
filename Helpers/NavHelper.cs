using System;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace WifftOCR.Helpers
{
    internal static class NavHelper
    {
        public static Type GetNavigateTo(NavigationViewItem item)
        {
            return (Type) item?.GetValue(NavigateToProperty);
        }

        public static void SetNavigateTo(NavigationViewItem item, Type value)
        {
            item?.SetValue(NavigateToProperty, value);
        }

        public static readonly DependencyProperty NavigateToProperty =
            DependencyProperty.RegisterAttached("NavigateTo", typeof(Type), typeof(NavHelper), new PropertyMetadata(null));
    }
}
