//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Windows;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
    /// <summary>
    /// View implementation
    /// </summary>
    public partial class MainWindow : Window
    {

        public int numberOfBalls { get;  set; }
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

            var chooser = new BillChooserWindow();

            if (chooser.ShowDialog() == true)
            {
                this.numberOfBalls = chooser.numberOfBalls;


                MessageBox.Show($"Wybrano {numberOfBalls} bil");
              
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.Start(numberOfBalls);
                }
               
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Raises the <seealso cref="System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.Dispose();
            base.OnClosed(e);
        }
    }
}


  