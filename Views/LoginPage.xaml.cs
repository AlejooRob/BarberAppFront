using BarberAppFront.ViewModels;
using Microsoft.Maui.Controls;

namespace BarberAppFront.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel) // Inyectamos el ViewModel
    {
        InitializeComponent();
        BindingContext = viewModel; // Establecemos el BindingContext
    }
}