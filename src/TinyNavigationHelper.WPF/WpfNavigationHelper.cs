using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TinyNavigationHelper.WPF
{
    public class WpfNavigationHelper : INavigationHelper
    {
        private NavigationService _navigationService;
        private Dictionary<string, Type> _views = new Dictionary<string, Type>();

        public WpfNavigationHelper(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void RegisterView(string key, Type type)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                _views[key.ToLower()] = type;
            }
            else
            {
                _views.Add(key.ToLower(), type);
            }
        }

        public async Task BackAsync()
        {
            _navigationService.GoBack();
        }

        public async Task CloseModalAsync()
        {
            await BackAsync();
        }

        public async Task NavigateToAsync(string key)
        {
            await NavigateToAsync(key, null);
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

                _navigationService.Navigate(page);
            }  
        }

        public async Task OpenModalAsync(string key, bool withNavigation = false)
        {
            await NavigateToAsync(key);
        }

        public async Task OpenModalAsync(string key, object parameter, bool withNavigation = false)
        {
            await NavigateToAsync(key, parameter);
        }

        public void SetRootView(string key, bool withNavigation = true)
        {
            SetRootView(key, null);
        }

        public void SetRootView(string key, object parameter, bool withNavigation = true)
        {
            var entry = _navigationService.RemoveBackEntry();
            while (entry != null)
            {
                entry = _navigationService.RemoveBackEntry();
            }

            NavigateToAsync(key, parameter);
        }

        /// <summary>
        /// Registers the views in assembly that inherit from Page
        /// </summary>
        /// <param name="assembly">The assembly to inspect</param>
        public void RegisterViewsInAssembly(Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes.Where(e => e.IsSubclassOf(typeof(Page))))
            {
                RegisterView(type.Name, type);
            }
        }
    }
}
