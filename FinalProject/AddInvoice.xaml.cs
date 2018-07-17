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

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for AddInvoice.xaml
    /// </summary>
    public partial class AddInvoice : Window
    {
        /// <summary>
        /// Declaring selecteddate DateTime that will be used to create a new invoice
        /// </summary>
        public DateTime SelectedDate = DateTime.Now;
        /// <summary>
        /// constructor
        /// </summary>
        public AddInvoice()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

        }
        /// <summary>
        /// saves the selected date into a new invoice. displays an error message if no date is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSubmit_Click(object sender, RoutedEventArgs e)
        {
            
            if (dpkSelectADate.SelectedDate != null)
            {
                SelectedDate = dpkSelectADate.SelectedDate ?? DateTime.Now;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a date.");
            }
            
        }
        /// <summary>
        /// Closes the window without saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
