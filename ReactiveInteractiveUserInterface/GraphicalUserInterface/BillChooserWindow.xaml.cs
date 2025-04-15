using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TP.ConcurrentProgramming.PresentationView
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class BillChooserWindow : Window
    {

        public int numberOfBalls { get; private set; } = 0;

        public BillChooserWindow()
        {
            InitializeComponent();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(InputBox.Text, out int count) && count > 0 && count <=15)
            {
                numberOfBalls = count;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Podaj prawidłową wartość!(1-15)");
                numberOfBalls = 0;
            }
        }

        private void InputBox_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartButton_Click(sender, e);
            }
        }

    }
}
