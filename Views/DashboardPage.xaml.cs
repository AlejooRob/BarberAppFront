using BarberAppFront.ViewModels;
using Microsoft.Maui.Controls;

namespace BarberAppFront.Views;

public partial class DashboardPage : ContentPage
{
    // Constructor que acepta el ViewModel inyectado
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}