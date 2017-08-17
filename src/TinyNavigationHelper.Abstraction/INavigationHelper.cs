using System;
using System.Collections.Generic;
using System.Text;

namespace TinyNavigationHelper
{
    public interface INavigationHelper
    {
        void SetRootView(string key, bool withNavigation = true);
        void NavigateTo(string key);
        void OpenModal(string key, bool withNavigation = false);
        void CloseModal();
        void Back();
    }
}
