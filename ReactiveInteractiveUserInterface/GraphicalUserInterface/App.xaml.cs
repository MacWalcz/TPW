//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System.Windows;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //TODO: Zamiast wrzucać wszystko do MainWindow.xaml.cs, złączyć to tutaj

     /*   protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var chooser = new BillChooserWindow();
            if (chooser.ShowDialog() == true)
            {

                MessageBox.Show($"Wybrano {chooser.numberOfBalls} bil");


                var mainWindow = new MainWindow();
                if (mainWindow.DataContext is MainWindowViewModel vm)
                {
                    vm.Start(chooser.numberOfBalls); 
                }
            }
            else
            {
                Shutdown();
                return;
            }
        }*/
    }
}