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
    /// Interaction logic for UpdateDefTable.xaml
    /// </summary>
    public partial class UpdateDefTable : Window
    {
        /// <summary>
        /// Global data table object
        /// </summary>
        DataTable dt = new DataTable();
        /// <summary>
        /// Globale invoice data object to access database
        /// </summary>
        InvoiceData id = new InvoiceData();
        public UpdateDefTable()
        {
            InitializeComponent();       
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            getItemDesc();
         
        }

        /// <summary>
        /// Gets the items of the current data set and generatews the grid
        /// </summary>
        private void getItemDesc()
        {
            dt = id.GetAllItemsForBusiness();
            generateItemDataGrid();
        }

        /// <summary>
        /// Creates the displayed datagrid
        /// </summary>
        private void generateItemDataGrid()
        {
            dgvItemList.DataContext = dt;
        }

        /// <summary>
        /// Adds an item to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            new AddItemDesc().ShowDialog();
            getItemDesc();
        }

        /// <summary>
        /// updates and item in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditItem_Click(object sender, RoutedEventArgs e)
        {
            DataRowView test = (DataRowView)dgvItemList.SelectedItem;
            new UpdateItemDescWindow(test[0].ToString(), test[2].ToString(), test[1].ToString()).ShowDialog();
            getItemDesc();
            btnDeleteItem.IsEnabled = false;
            btnEditItem.IsEnabled = false;
        }

        /// <summary>
        /// Attempts to delete an item from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            DataRowView test = (DataRowView)dgvItemList.SelectedItem;
            id.DeleteItem(test[0].ToString());
            getItemDesc();
            btnDeleteItem.IsEnabled = false;
            btnEditItem.IsEnabled = false;
        }

        /// <summary>
        /// Enables options when an item is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditItem.IsEnabled = true;
            btnDeleteItem.IsEnabled = true;
        }
    }
}
