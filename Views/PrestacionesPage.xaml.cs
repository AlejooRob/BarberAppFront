using BarberAppFront.ViewModels;
using Microsoft.Maui.Controls;

namespace BarberAppFront.Views;

public partial class PrestacionesPage : ContentPage
{
    public PrestacionesPage(PrestacionesViewModel viewModel) // Inyección del ViewModel
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Opcional: Para recargar datos cada vez que la página aparece
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((PrestacionesViewModel)BindingContext).LoadPrestacionesCommand.Execute(null);
    }
}