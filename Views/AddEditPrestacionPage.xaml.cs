using BarberAppFront.ViewModels;
using Microsoft.Maui.Controls;

namespace BarberAppFront.Views;

public partial class AddEditPrestacionPage : ContentPage
{
    // Constructor para la creaci�n y edici�n, inyectando el ViewModel
    public AddEditPrestacionPage(AddEditPrestacionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Opcional: Si necesitas pasar un objeto para editar al navegar
    // Puedes usar un m�todo en el ViewModel o pasar en el constructor de la p�gina
    // protected override void OnNavigatedTo(NavigatedToEventArgs args)
    // {
    //     base.OnNavigatedTo(args);
    //     if (BindingContext is AddEditPrestacionViewModel viewModel)
    //     {
    //         // Ejemplo si pasas un objeto por query parameter o de otra forma
    //         // viewModel.Initialize(args.Parameter as PrestacionServicio);
    //     }
    // }
}