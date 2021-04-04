﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using TinyNavigationHelper.Abstraction;
using TinyMvvm.Forms;

namespace TinyNavigationHelper.Forms
{
    public class FormsNavigationHelper : INavigationHelper
    {
        protected Dictionary<string, Type> Views = new Dictionary<string, Type>();
        
        public IViewCreator<Page> ViewCreator { get; set; }

        public FormsNavigationHelper()
        {
            if(Assembly.GetExecutingAssembly().FullName.Contains("TinyMvvm"))
            {
                ViewCreator = new TinyMvvmViewCreator();
            }
            else
            {
                ViewCreator = new DefaultViewCreator();
            }

            

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

        protected virtual void InternalRegisterView(Type type, string key)
        {
            if (Views.ContainsKey(key.ToLower()))
            {
                Views[key.ToLower()] = type;
            }
            else
            {
                Views.Add(key.ToLower(), type);
            }
        }

        /// <summary>
        /// Registers the views in assembly that inherit from Page
        /// </summary>
        /// <param name="assembly">The assembly to inspect</param>
        public void RegisterViewsInAssembly(Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes.Where(e => e.IsSubclassOf(typeof(Page))))
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
            var modalNavigationPage = Application.Current.MainPage.Navigation.ModalStack.LastOrDefault() as NavigationPage;
            if (modalNavigationPage == null)
            {
                if (Application.Current.MainPage is TabbedPage tabbedpage)
                {
                    var selected = tabbedpage.CurrentPage;

                    if (resetStack)
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
                else if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
                {
                    if (resetStack)
                    {
                        masterDetailPage.Detail = new NavigationPage(page);
                        return;
                    }

                    if (masterDetailPage.Detail is TabbedPage tabbedPage)
                    {
                        if (tabbedPage.CurrentPage.Navigation != null)
                        {
                            await tabbedPage.CurrentPage.Navigation.PushAsync(page);
                            return;
                        }
                    }

                    if (masterDetailPage?.Detail.Navigation != null)
                    {
                        await masterDetailPage.Detail.Navigation.PushAsync(page);

                        return;
                    }
                }

                if (resetStack)
                {
                    Application.Current.MainPage = new NavigationPage(page);
                    return;
                }

                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
            else
            {
                await modalNavigationPage.PushAsync(page);
            }
        }

        public virtual async Task NavigateToAsync(string key, object parameter)
        {
            await NavigateTo(key, parameter);
        }

        public virtual async Task NavigateToAsync(string key)
        {
            await NavigateTo(key, null);
        }

        private async Task NavigateTo(string key, object? parameter)
        {
            if (Views.ContainsKey(key.ToLower()))
            {
                var type = Views[key.ToLower()];

                Page? page = null;

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

        public async Task OpenModalAsync(Page page, bool withNavigation = false)
        {
            if (withNavigation)
            {
                var modalNavigationPage = new NavigationPage(page);
                await Application.Current.MainPage.Navigation.PushModalAsync(modalNavigationPage);

            }
            else
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(page);
            }
        }

        public async Task OpenModalAsync(string key, object parameter, bool withNavigation = false)
        {
            await OpenModal(key, parameter, withNavigation);
        }

        public async Task OpenModalAsync(string key, bool withNavigation = false)
        {
            await OpenModal(key, null, withNavigation);
        }

        private async Task OpenModal(string key, object? parameter, bool withNavigation = false)
        {
            if (Views.ContainsKey(key.ToLower()))
            {
                var type = Views[key.ToLower()];

                Page? page = null;

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

        public async Task CloseModalAsync()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        public async Task BackAsync()
        {
            var modalNavigationPage = Application.Current.MainPage.Navigation.ModalStack.LastOrDefault() as NavigationPage;
            if (modalNavigationPage == null)
            {
                if (Application.Current.MainPage is TabbedPage tabbedpage)
                {
                    var selected = (Page)tabbedpage.CurrentPage;

                    if (selected.Navigation != null)
                    {
                        await selected.Navigation.PopAsync();

                        return;
                    }
                }
                else if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
                {
                    if (masterDetailPage.Detail is TabbedPage tabbedPage)
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

                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await modalNavigationPage.PopAsync();
            }
        }

        public void SetRootView(string key, object parameter, bool withNavigation = true)
        {
            SetRoot(key, parameter, withNavigation);
        }

        public void SetRootView(string key, bool withNavigation = true)
        {
            SetRoot(key, null, withNavigation);
        }

        private void SetRoot(string key, object? parameter, bool withNavigation)
        {
            if (Views.ContainsKey(key.ToLower()))
            {
                var type = Views[key.ToLower()];

                Page? page = null;

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
                    Application.Current.MainPage = new NavigationPage(page);
                }
                else
                {
                    Application.Current.MainPage = page;
                }
            }
            else
            {
                throw new ViewCreationException($"The view '{key}, you're trying to navigate to has not been registered");
            }
        }

        public async Task ResetStackWith(string key)
        {
            await ResetStack(key, null);
        }

        public async Task ResetStackWith(string key, object parameter)
        {
            await ResetStack(key, parameter);
        }

        private async Task ResetStack(string key, object? parameter)
        {
            if (Views.ContainsKey(key.ToLower()))
            {
                var type = Views[key.ToLower()];

                Page? page = null;

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
