using System;
using System.Collections.Generic;
using System.Text;

namespace TinyNavigationHelper
{
    public interface INavigationHelper
    {
        void SetRootView(string key, bool withNavigation = true);
        void SetRootView(string key, object parameter, bool withNavigation = true);
        void NavigateTo(string key);
        void NavigateTo(string key, object parameter);
        void OpenModal(string key, bool withNavigation = false);
        void OpenModal(string key, object parameter, bool withNavigation = false);
        void CloseModal();
        void Back();
    }
}
