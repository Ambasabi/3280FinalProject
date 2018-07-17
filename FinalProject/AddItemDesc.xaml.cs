using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for AddItemDesc.xaml
    /// </summary>
    public partial class AddItemDesc : Window
    {
        /// <summary>
        /// Global data access object
        /// </summary>
        InvoiceData id = new InvoiceData();

        /// <summary>
        /// Global data storaage object
        /// </summary>
        DataTable dt = new DataTable();
        /// <summary>
        /// Constructor
        /// </summary>
        public AddItemDesc()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        /// <summary>
        /// Submits the new item, first check if exists, will display error message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmitItem_Click(object sender, RoutedEventArgs e)
        {
            if (id.checkItemCode(txtItemCode.Text) == true)
            {
                MessageBox.Show("This item code already exists. Please enter a new code.");
            }
            else
            {
                id.AddItem(txtItemCode.Text, txtItemDescription.Text, txtItemCost.Text);
                this.Close();
            }
        }
        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
