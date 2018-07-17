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
    /// Interaction logic for SearchForm.xaml
    /// </summary>
    /// not sure what to do.. 
    public partial class SearchForm : Window
    {
        /// <summary>
        /// Public string stores the selected invoice id <-- tihs is what we are goning t
        /// </summary>
        public string SelectedInvoice { get; set; }
        InvoiceData id = new InvoiceData();
        DataTable dt = new DataTable();
        
        /// <summary>
        /// Gets all db invoices on load
        /// </summary>
        private void getInvoices()
        {
            dt = id.GetAllInvoices();
            GenerateDataGrid();
        }
        /// <summary>
        /// Loads datagrid information
        /// </summary>
        private void GenerateDataGrid()
        {
            dgvSearchList.DataContext = dt;
            generateComboBoxLists();
        }

        /// <summary>
        /// Methods generated the three combobox list options based on the current datatable
        /// </summary>
        private void generateComboBoxLists()
        {
            cmbInvoiceNumber.Items.Clear();
            cmbInvoiceDate.Items.Clear();
            cmbInvoiceTotalCharge.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++ )
            {
                cmbInvoiceNumber.Items.Add(dt.Rows[i][0].ToString());             
            }
            dt = id.GetAllDistinctInvoiceDates();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cmbInvoiceDate.Items.Add(dt.Rows[i][0].ToString());
            }
            dt = id.GetAllDistinctCharges();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                cmbInvoiceTotalCharge.Items.Add(dt.Rows[j][0].ToString());
            }            
        }

        /// <summary>
        /// Search form constructor, builds default list
        /// </summary>
        public SearchForm()
        {
            InitializeComponent();
            SelectedInvoice = null;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            getInvoices();
        }

        /// <summary>
        /// Enabled the selecte invoice option when a row is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSelectInvoice.IsEnabled = true;
        }

        /// <summary>
        /// Gets new search results based on the  three filter fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            bool check = false;//if all parameters are null dont do anything
            string invoiceId = "";
            string date = ""; 
            string cost = "";

            if (cmbInvoiceNumber.SelectedItem != null)
            {
                check = true;
                invoiceId = cmbInvoiceNumber.SelectedItem.ToString();
            }
            if (cmbInvoiceDate.SelectedItem != null)
            {
                check = true;
                date = cmbInvoiceDate.SelectedItem.ToString();
            }
            if(cmbInvoiceTotalCharge.SelectedItem != null){
                check = true;
                cost = cmbInvoiceTotalCharge.SelectedItem.ToString();
            }

            if (check)
            {
                dt = id.GetInvoices(invoiceId, date, cost);
                GenerateDataGrid();
            }
        }

        /// <summary>
        /// Method reset search form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearSearchForm_Click(object sender, RoutedEventArgs e)
        {
            dt = id.GetAllInvoices();
            GenerateDataGrid();
            btnSelectInvoice.IsEnabled = false;
        }

        /// <summary>
        /// Method gets info from select dgv item and returns to main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView test = (DataRowView)dgvSearchList.SelectedItem;
                SelectedInvoice = test[0].ToString();
                this.Close();

            }
            catch
            {
                //Do nothing as SelectedInvoice will remain null
            }
        }        
    }
}
