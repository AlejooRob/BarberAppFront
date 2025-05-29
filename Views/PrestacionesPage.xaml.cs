using BarberAppFront.ViewModels;
using Microsoft.Maui.Controls;

namespace BarberAppFront.Views;

public partial class PrestacionesPage : ContentPage
{
    public PrestacionesPage(PrestacionesViewModel viewModel) // Inyecci�n del ViewModel
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Opcional: Para recargar datos cada vez que la p�gina aparece
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((PrestacionesViewModel)BindingContext).LoadPrestacionesCommand.Execute(null);
    }
}