using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SampleApp.Views
{
    public class TabbedView : TabbedPage
    {
        public TabbedView()
        {
            Title = "TabbedView";

            Children.Add(new NavigationPage(new MainView()) { Title = "Main" });
            Children.Add(new NavigationPage(new AboutView()) { Title = "About" });
            Children.Add(new NavigationPage(new SettingsView()) { Title = "Settings" });
        }
    }
}