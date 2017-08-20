using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public async Task NavigateToAsync(string key, object parameter)
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
                            await selected.Navigation.PushAsync(page);

                            return;
                        }
                    }

                    await _app.MainPage.Navigation.PushAsync(page); 
                }
                else
                {

                }
            }
        }

        public async Task NavigateToAsync(string key)
        {
            await NavigateToAsync(key, null);
        }

        public async Task OpenModalAsync(string key, object parameter, bool withNavigation = false)
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
                    await _app.MainPage.Navigation.PushModalAsync(page);
                }
                else
                {
                    _modalNavigationPage = new NavigationPage(page);
                    await _app.MainPage.Navigation.PushModalAsync(_modalNavigationPage);
                }
            }
        }

        public async Task OpenModalAsync(string key, bool withNavigation = false)
        {
            await OpenModalAsync(key, null, withNavigation);
        }

        public async Task CloseModalAsync()
        {           
            await _app.MainPage.Navigation.PopModalAsync();
            _modalNavigationPage = null;
        }

        public async Task BackAsync()
        {
            if (_app.MainPage is TabbedPage tabbedpage)
            {
                var selected = (Page)tabbedpage.SelectedItem;

                if (selected.Navigation != null)
                {
                    await selected.Navigation.PopAsync();

                    return;
                }
            }

            await _app.MainPage.Navigation.PopAsync();
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
