using SampleApp.Views;
using TinyNavigationHelper.Forms;
using Xamarin.Forms;

namespace SampleApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Register views
			var navigationHelper = new FormsNavigationHelper(this);
			navigationHelper.RegisterView<MainView>("MainView");
            navigationHelper.RegisterView<AboutView>("AboutView");
            navigationHelper.RegisterView<SettingsView>("SettingsView");

            MainPage = new NavigationPage(new MainView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
