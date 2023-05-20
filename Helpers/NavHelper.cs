// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace OCRStudio.Helpers
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
