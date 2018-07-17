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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Global data table object
        /// </summary>
        DataTable dt = new DataTable();

        /// <summary>
        /// Global invoice data object to access database
        /// </summary>
        InvoiceData id = new InvoiceData();

        /// <summary>
        /// init all main components
        /// </summary>
        public MainWindow()
        {
            dt = new DataTable();
            id = new InvoiceData();

            InitializeComponent();
            InitializeWindow(); // refreshes invoice create dgv columns
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// init invoice window components, refresh invoice and create datagrid columns
        /// </summary>
        private void InitializeWindow()
        {
            try
            {
                RefreshInvoiceCB(); // adds all invoices to the dropdown

                (dgvInvoice.Columns[0] as DataGridTextColumn).Binding = new Binding("ItemDescr");
                (dgvInvoice.Columns[1] as DataGridTextColumn).Binding = new Binding("Qty");
                (dgvInvoice.Columns[2] as DataGridTextColumn).Binding = new Binding("Price");             
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went wrong. Please try again or contact your administrator with the following error: \n" + e);
            }
            
        }

        /// <summary>
        /// struct to house the data to be used in datagrid
        /// </summary>
        private struct InvoiceLine
        {
            public object ItemDescr { get; set; }
            public object Qty { get; set; }
            public object Price { get; set; }
        }

        /// <summary>
        /// refresh the search by invoice number dropdown
        /// </summary>
        private void RefreshInvoiceCB()
        {
            this.cbSearchByInvoiceNum.Items.Clear(); // clears combobox
          
            try 
            {
                DataTable invoices = id.GetAllInvoices();

                foreach (DataRow dr in invoices.Rows) // adds combobox items
                {
                    cbSearchByInvoiceNum.Items.Add(dr["InvoiceNum"]);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Something went wrong. Please try again or contact your administrator with the following error: \n\n" + e + "\n");
            }

        }

        /// <summary>
        /// event listener that fills (or refills) the search by invoice number dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSearchByInvoiceNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbSearchByInvoiceNum.SelectedIndex > -1) // ensures that selection is a valid row number
                {
                    FillInvoice(cbSearchByInvoiceNum.SelectedItem.ToString());
                }

                btnAddNewItem.IsEnabled = true;
                btnEditItem.IsEnabled = true;
                btnDeleteItem.IsEnabled = true;
                btnDeleteInvoice.IsEnabled = true;
            }
            catch (Exception invalidSelect)
            {
                MessageBox.Show("Invalid Selection. Please try again or contact your administrator with the following information: \n\n" + invalidSelect + "\n");
            }
            
        }


        /// <summary>
        /// populates invoice info in datagrid view based on selection from 'Search By Invoice' combobox
        /// </summary>
        /// <param name="invoiceNum"></param>
        public void FillInvoice(string invoiceNum)
        {
            try
            {
                dgvInvoice.Items.Clear(); // clears dgv contents
                DataTable invoiceItems = id.GetInvoice(invoiceNum);

                foreach (DataRow dr in invoiceItems.Rows)
                {
                    dgvInvoice.Items.Add(new InvoiceLine {ItemDescr=dr[1],Qty=dr[2],Price=dr[3] });
                }

                // information labels
                lblInvoiceNum.Content = invoiceNum;
                lblInvoiceDate.Content = id.GetInvoiceDate(invoiceNum);
                lblInvoiceTotal.Content = id.GetInvoiceTotalCharge(invoiceNum);
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went wrong. Please report the following error to your administrator: \n\n" + e + "\n");
            }
        }

        /// <summary>
        /// opens the invoice search form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // string temp = null;
            SearchForm SF = new SearchForm();
            SF.ShowDialog();

            // cast the invoice objects to strings
            if (SF.SelectedInvoice != null) {
                try
                {
                    for (int i = 0; i < cbSearchByInvoiceNum.Items.Count; i++)
                    {
                        if (cbSearchByInvoiceNum.Items[i].ToString() == SF.SelectedInvoice)
                        {
                            cbSearchByInvoiceNum.SelectedIndex = i;
                            break;
                        }
                    }
                }
                catch (Exception bunkData)
                { 
                    MessageBox.Show("The Advanced Search author has provided invalid data.\n\n" + bunkData +"\n"); 
                }
            }
            
        }

        /// <summary>
        /// Opens the def table edit screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateDef_Click(object sender, RoutedEventArgs e)
        {
            new UpdateDefTable().ShowDialog();
        }

        /// <summary>
        /// event listener for adding new item to new or existing invoice 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (cbSearchByInvoiceNum.SelectedIndex > -1)
            {
                AddItem newItem = new AddItem(); // garbage collector should hopefully handle disposal of any old instances
                newItem.ShowDialog();              

                id.AddItemToInvoice(cbSearchByInvoiceNum.SelectedValue.ToString(), newItem.ItemDescription, newItem.Quantity);

                FillInvoice(cbSearchByInvoiceNum.SelectedItem.ToString());
            }
        }


        /// <summary>
        /// edit invoice item - essentially deletes selected row then adds in new one with modifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgvInvoice.SelectedIndex > -1)
            {
                string invoiceNumber = cbSearchByInvoiceNum.SelectedItem.ToString();
                // casts as an invoiceline struct
                string itemName =  ((InvoiceLine)dgvInvoice.Items[dgvInvoice.SelectedIndex]).ItemDescr.ToString();
                string quantity = ((InvoiceLine)dgvInvoice.Items[dgvInvoice.SelectedIndex]).Qty.ToString();

                // call add item second constructor so the selected values are populated in the popup
                AddItem ai = new AddItem(itemName, quantity);
                ai.ShowDialog();

                //delete old item line
                id.DeleteInvoiceItem(invoiceNumber, itemName);

                //inserting new item(s)
                id.AddItemToInvoice(invoiceNumber, ai.ItemDescription, ai.Quantity);
                FillInvoice(invoiceNumber);
            }
        }

        /// <summary>
        /// deletes currently selected item (row) from invoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        { 
            if (dgvInvoice.SelectedIndex > -1) // is selection valid?
            {
                try
                {
                    if (MessageBox.Show("Are you sure you would like to delete the selected item?",
                                        "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        id.DeleteInvoiceItem(cbSearchByInvoiceNum.SelectedItem.ToString(), ((InvoiceLine)dgvInvoice.Items[dgvInvoice.SelectedIndex]).ItemDescr.ToString());
                        FillInvoice(cbSearchByInvoiceNum.SelectedItem.ToString());
                    }
                }
                catch (Exception delItem_ex)
                {
                    MessageBox.Show("Item could not be deleted.\n\n" + delItem_ex + "\n");
                }
                
            }
        }

        /// <summary>
        /// delete current invoice (the one selected from combobox/dropdown)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (cbSearchByInvoiceNum.SelectedIndex > -1) // is the selection valid?
            {
                try
                {
                    // display 'are you sure' prompt
                    if (MessageBox.Show("Are you sure you would like to delete this invoice?",
                                        "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        id.DeleteInvoice(cbSearchByInvoiceNum.SelectedItem.ToString());
                        RefreshInvoiceCB();
                        if (cbSearchByInvoiceNum.Items.Count > 0) // is there an item to delete?
                        {
                            cbSearchByInvoiceNum.SelectedIndex = 0;
                        }
                    }
                }
                catch (Exception delInv_ex)
                {
                    MessageBox.Show("Invoice could not be deleted.\n\n" + delInv_ex + "\n");
                }
                
            }
        }

        /// <summary>
        /// create new invoice - 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateNewInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddInvoice ai = new AddInvoice(); // old instances cleaned up by loss of scope (hopefully)
                ai.ShowDialog();
                if (ai.DialogResult == true)
                {
                    string test = ai.DialogResult.ToString();
                    id.CreateInvoice(ai.SelectedDate);
                    RefreshInvoiceCB();
                    cbSearchByInvoiceNum.SelectedIndex = cbSearchByInvoiceNum.Items.Count - 1;
                }
            } 
            catch(Exception ex)
            {
                MessageBox.Show("A new invoice could not be created. Try again or report the following error to your admin: \n\n" + ex + "\n");
            }
        }
        
    }
}
