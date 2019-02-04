using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using TinyNavigationHelper.Abstraction;

namespace TinyNavigationHelper.Forms
{
    public class FormsNavigationHelper : INavigationHelper
    {
        private Application _app;
        private Dictionary<string, Type> _views = new Dictionary<string, Type>();
        private NavigationPage _modalNavigationPage;

        public IViewCreator<Page> ViewCreator { get; set; }

        public FormsNavigationHelper(Application app)
        {
            _app = app;
            ViewCreator = new DefaultViewCreator();

            Abstraction.NavigationHelper.Current = this;
        }

        public void RegisterView<T>(string key) where T : Page
        {
            var type = typeof(T);
            InternalRegisterView(type, key); 
        }

        public void RegisterView(string key, Type type)
        {
            InternalRegisterView(type, key);
        }

        private void InternalRegisterView(Type type, string key)
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

        /// <summary>
        /// Registers the views in assembly that inherit from Page
        /// </summary>
        /// <param name="assembly">The assembly to inspect</param>
        public void RegisterViewsInAssembly(Assembly assembly)
        {
            foreach(var type in assembly.DefinedTypes.Where(e => e.IsSubclassOf(typeof(Page))))
            {
                InternalRegisterView(type.AsType(), type.Name);
            }
        }

        /// <summary>
        /// Navigates to async.
        /// </summary>
        /// <returns>The to async.</returns>
        /// <param name="page">Page.</param>
        /// <remarks>Not exposed by the interface but added as an extension</remarks>
        public async Task NavigateToAsync(Page page)
        {
            await NavigateToAsync(page, false);
        }
      
        private async Task NavigateToAsync(Page page, bool resetStack)
        {
            if (_modalNavigationPage == null)
            {
                if (_app.MainPage is TabbedPage tabbedpage)
                {
                    var selected = tabbedpage.CurrentPage;

                    if(resetStack)
                    {
                        var pages = selected.Navigation.NavigationStack.Count();


                        await selected.Navigation.PushAsync(page);

                        for (var i = pages - 1; i >= 0; i--)
                        {
                            var p = selected.Navigation.NavigationStack[i];
                            selected.Navigation.RemovePage(p);
                        }



                        return;
                    }

                    if (selected.Navigation != null)
                    {
                        await selected.Navigation.PushAsync(page);

                        return;
                    }
                }
                else if (_app.MainPage is MasterDetailPage masterDetailPage)
                {
                    if(resetStack)
                    {
                        masterDetailPage.Detail = new NavigationPage(page);
                        return;
                    }

                    if(masterDetailPage.Detail is TabbedPage tabbedPage)
                    {
                        if(tabbedPage.CurrentPage.Navigation != null)
                        {
                            await tabbedPage.CurrentPage.Navigation.PushAsync(page);
                            return;
                        }
                    }

                    if (masterDetailPage.Detail.Navigation != null)
                    {
                        await masterDetailPage.Detail.Navigation.PushAsync(page);

                        return;
                    }
                }

                if (resetStack)
                {
                    _app.MainPage = new NavigationPage(page);
                    return;
                }

                await _app.MainPage.Navigation.PushAsync(page);
            }
            else
            {
                await _modalNavigationPage.PushAsync(page);
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
                    page = ViewCreator.Create(type);
                }
                else
                {
                    page = ViewCreator.Create(type, parameter);
                }

                await NavigateToAsync(page);
            }
            else
            {
                throw new ViewCreationException($"The view '{key}, you're trying to navigate to has not been registered");
            }
        }

        public async Task NavigateToAsync(string key)
        {
            await NavigateToAsync(key, null);
        }

		public async Task OpenModalAsync(Page page, bool withNavigation = false)
        {
			if (withNavigation)
			{
                _modalNavigationPage = new NavigationPage(page);
                await _app.MainPage.Navigation.PushModalAsync(_modalNavigationPage);
				
			}
			else
			{
                await _app.MainPage.Navigation.PushModalAsync(page);
			}
        }

        public async Task OpenModalAsync(string key, object parameter, bool withNavigation = false)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                var type = _views[key.ToLower()];

                Page page = null;

                if (parameter == null)
                {
                    page = ViewCreator.Create(type);
                }
                else
                {
                    page = ViewCreator.Create(type, parameter);
                }

                await OpenModalAsync(page, withNavigation);
            }
            else
            {
                throw new ViewCreationException($"The view '{key}, you're trying to navigate to has not been registered");
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
                var selected = (Page)tabbedpage.CurrentPage;

                if (selected.Navigation != null)
                {
                    await selected.Navigation.PopAsync();

                    return;
                }
            }
            else if (_app.MainPage is MasterDetailPage masterDetailPage)
            {
                if(masterDetailPage.Detail is TabbedPage tabbedPage)
                {
                    if (tabbedPage.CurrentPage.Navigation != null)
                    {
                        await tabbedPage.CurrentPage.Navigation.PopAsync();
                        return;
                    }
                }
                if (masterDetailPage.Detail.Navigation != null)
                {
                    await masterDetailPage.Detail.Navigation.PopAsync();

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
                    page = ViewCreator.Create(type);
                }
                else
                {
                    page = ViewCreator.Create(type, parameter);
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
            else
            {
                throw new ViewCreationException($"The view '{key}, you're trying to navigate to has not been registered");
            }
        }

        public void SetRootView(string key, bool withNavigation = true)
        {
            SetRootView(key, null, withNavigation);
        }

        public async Task ResetStackWith(string key)
        {
            await ResetStackWith(key, null);
        }

        public async Task ResetStackWith(string key, object parameter)
        {
            if (_views.ContainsKey(key.ToLower()))
            {
                var type = _views[key.ToLower()];

                Page page = null;

                if (parameter == null)
                {
                    page = ViewCreator.Create(type);
                }
                else
                {
                    page = ViewCreator.Create(type, parameter);
                }

                await NavigateToAsync(page, true);
            }
            else
            {
                throw new ViewCreationException($"The view '{key}, you're trying to navigate to has not been registered");
            }
        }
    }
}
