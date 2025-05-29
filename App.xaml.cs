using BarberAppFront.Views;
namespace BarberAppFront
{
    public partial class App : Application
    {
        public App(LoginPage loginPage)
        {
            InitializeComponent();
            MainPage = new NavigationPage(loginPage);
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
    }
}