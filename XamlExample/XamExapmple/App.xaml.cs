using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteX.Sketch.XamExapmple
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var managerManager = DependencyService.Get<IManagerManager>();
            //MainPage = new BioshockGamePad(managerManager.InputManager);
            MainPage = new PPTControllerPage(managerManager.InputManager);
            //MainPage = new MainPage();
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
