using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TinyNavigationHelper.Forms
{
    public class NavigationHelper : INavigationHelper
    {
        private Application _app;
        private Dictionary<string, Type> _views = new Dictionary<string, Type>();

        public NavigationHelper(Application app)
        {
            _app = app;
        }

        public void RegisterView<T>(string key)
        {
            var type = typeof(T);

            
                if (_views.ContainsKey(key.ToLower()))
                {
                    _views[key.ToLower()] = type;
                }
                else
                {
                    _views.Add(key.ToLower(), type);
                }           
        }
        public void NavigateTo(string key)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                var type = _views[key.ToLower()];

                var page = (Page)Activator.CreateInstance(type);

                if (_app.MainPage is TabbedPage tabbedpage)
                {
                    var selected = (Page)tabbedpage.SelectedItem;

                    if(selected.Navigation != null)
                    {
                        selected.Navigation.PushAsync(page);

                        return;
                    }
                }

                _app.MainPage.Navigation.PushAsync(page); 
            }
        }

        public void OpenModal(string key, bool withNavigation = false)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                var type = _views[key.ToLower()];

                var page = (Page)Activator.CreateInstance(type);

                if (withNavigation)
                {
                    _app.MainPage.Navigation.PushModalAsync(page); 
                }
                else
                {
                    _app.MainPage.Navigation.PushModalAsync(new NavigationPage(page));
                }
            }
        }

        public void CloseModal()
        {
            _app.MainPage.Navigation.PopModalAsync();
        }

        public void Back()
        {
            if (_app.MainPage is TabbedPage tabbedpage)
            {
                var selected = (Page)tabbedpage.SelectedItem;

                if (selected.Navigation != null)
                {
                    selected.Navigation.PopAsync();

                    return;
                }
            }

            _app.MainPage.Navigation.PopAsync();
        }

        public void SetRootView(string key, bool withNavigation = true)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                var type = _views[key.ToLower()];

                var page = (Page)Activator.CreateInstance(type);

                if (withNavigation)
                {
                    _app.MainPage = new NavigationPage(page); 
                }
                else
                {
                    _app.MainPage = page;
                }
            }
        }
    }
}
