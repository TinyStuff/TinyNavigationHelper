using System;
using System.Windows.Input;

using TinyNavigationHelper.Abstraction;
using Xamarin.Forms;

namespace SampleApp.ViewModels
{
    public class MainViewModel
    {
        public ICommand NavigateTo { get; } = new Command(async (obj) =>
        {
            await NavigationHelper.Current.NavigateToAsync(obj.ToString());
        });
    }
}
