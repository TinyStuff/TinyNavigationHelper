using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TinyNavigationHelper.Forms
{
    public class FormsNavigationHelper : INavigationHelper
    {
        private Application _app;
        private Dictionary<string, Type> _views = new Dictionary<string, Type>();
        private NavigationPage _modalNavigationPage;

        public FormsNavigationHelper(Application app)
        {
            _app = app;

            Abstraction.NavigationHelper.Current = this;
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
        public void NavigateTo(string key, object parameter)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                var type = _views[key.ToLower()];

                Page page = null;

                if (parameter == null)
                {
                    page = (Page)Activator.CreateInstance(type);
                }
                else
                {
                    page = (Page)Activator.CreateInstance(type, parameter);
                }


                if(_modalNavigationPage == null)
                {
                    if (_app.MainPage is TabbedPage tabbedpage)
                    {
                        var selected = (Page)tabbedpage.SelectedItem;

                        if (selected.Navigation != null)
                        {
                            selected.Navigation.PushAsync(page);

                            return;
                        }
                    }

                    _app.MainPage.Navigation.PushAsync(page); 
                }
                else
                {

                }
            }
        }

        public void NavigateTo(string key)
        {
            NavigateTo(key, null);
        }

        public void OpenModal(string key, object parameter, bool withNavigation = false)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                var type = _views[key.ToLower()];

                Page page = null;

                if (parameter == null)
                {
                    page = (Page)Activator.CreateInstance(type); 
                }
                else
                {
                    page = (Page)Activator.CreateInstance(type, parameter);
                }

                if (withNavigation)
                {
                    _app.MainPage.Navigation.PushModalAsync(page);
                }
                else
                {
                    _modalNavigationPage = new NavigationPage(page);
                    _app.MainPage.Navigation.PushModalAsync(_modalNavigationPage);
                }
            }
        }

        public void OpenModal(string key, bool withNavigation = false)
        {
            OpenModal(key, null, withNavigation);
        }

        public void CloseModal()
        {           
            _app.MainPage.Navigation.PopModalAsync();
            _modalNavigationPage = null;
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

        public void SetRootView(string key, object parameter, bool withNavigation = true)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                var type = _views[key.ToLower()];

                Page page = null;

                if (parameter == null)
                {
                    page = (Page)Activator.CreateInstance(type);
                }
                else
                {
                    page = (Page)Activator.CreateInstance(type, parameter);
                }

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

        public void SetRootView(string key, bool withNavigation = true)
        {
            SetRootView(key, null, withNavigation);
        }
    }
}
