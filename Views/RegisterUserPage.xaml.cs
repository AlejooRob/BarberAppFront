using BarberAppFront.ViewModels;
using Microsoft.Maui.Controls;

namespace BarberAppFront.Views;

public partial class RegisterUserPage : ContentPage
{
    public RegisterUserPage(RegisterUserViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}