using System;
using System.Windows.Input;
using SampleApp.Views;
using TinyNavigationHelper.Abstraction;
using TinyNavigationHelper.Forms;
using Xamarin.Forms;

namespace SampleApp.ViewModels
{
    public class MainViewModel
    {
        public ICommand NavigateTo { get; } = new Command(async (obj) =>
        {
            await NavigationHelper.Current.NavigateToAsync(obj.ToString());
        });

        public ICommand NavigateWithParameter { get; } = new Command<string>(async (parameter) =>
        {
            await NavigationHelper.Current.NavigateToAsync("ParameterView", parameter);
        });

        public ICommand SetRootView { get; } = new Command((obj) =>
        {
            NavigationHelper.Current.SetRootView(obj.ToString(), false);
        });

        public ICommand NavigateToNews { get; } = new Command(async () =>
        {
            // This is using an extension method on the Forms implementation
            // of the INavigationHelper so we can pass already created views
            // if we are in a forms enviroment.
            var page = new NewsView();
            await NavigationHelper.Current.NavigateToAsync(page);
        });

        public ICommand GenericNavigation { get; } = new Command(async () =>
        {
            await NavigationHelper.Current.NavigateToAsync<AboutView>(); 
        });
    }
}
