using System;
using Xamarin.Forms;

namespace SampleApp.Views
{
    public class MasterDetailView : MasterDetailPage
    {
        public MasterDetailView()
        {
            Master = new MainView() { Title = "Menu" };
            Detail = new NavigationPage(new MainView());
        }
    }
}
