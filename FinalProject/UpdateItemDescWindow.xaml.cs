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
    /// Interaction logic for UpdateItemDescWindow.xaml
    /// </summary>
    public partial class UpdateItemDescWindow : Window
    {
        /// <summary>
        /// Holds existing code
        /// </summary>
        string existingItemCode;
        /// <summary>
        /// holds existing descition
        /// </summary>
        string existingItemDesc;
        /// <summary>
        /// holds existing cost
        /// </summary>
        string existingItemCost;
        public UpdateItemDescWindow(string a, string b, string c)
        {
            existingItemCode = a;
            existingItemDesc = b;
            existingItemCost = c;
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            setTextBoxes();
           
        }

        /// <summary>
        /// isnerts the existing values in the dispalyed boxed
        /// </summary>
        public void setTextBoxes()
        {
            txtUpdateItemCode.Text = existingItemCode;
            txtUpdateItemDescription.Text = existingItemDesc;
            txtUpdateItemCost.Text = existingItemCost;
        }

        /// <summary>
        /// Closes the form without updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// update the item and close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmitItem_Click(object sender, RoutedEventArgs e)
        {
            new InvoiceData().UpdateItem(existingItemCode, txtUpdateItemDescription.Text, txtUpdateItemCost.Text);
            this.Close();
        }
    }
}
