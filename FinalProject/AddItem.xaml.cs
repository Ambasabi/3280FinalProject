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
using System.Data;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for AddItem.xaml
    /// </summary>
    public partial class AddItem : Window
    {
        /// <summary>
        /// Description of the selected item
        /// </summary>
        public string ItemDescription;

        /// <summary>
        /// Quantity Selected by user
        /// </summary>
        public int Quantity;

        /// <summary>
        /// Private list to hold a list of items
        /// </summary>
        private List<Item> allItems = new List<Item>();

        /// <summary>
        /// Represents one row from the ItemDescription table
        /// </summary>
        private struct Item
        {
            public string ItemCode { get; set; }
            public string ItemDesc { get; set; }
            public string Cost { get; set; }
        }

        /// <summary>
        /// used to get ItemDescription entries
        /// </summary>
        private InvoiceData id = new InvoiceData();

        /// <summary>
        /// Default constructor for additem.
        /// </summary>
        public AddItem() 
        {
            InitializeComponent();
            InitializeWindow();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// additem constructor for add (new) item pop-up 
        /// </summary>
        /// <param name="itemName">selected item name</param>
        /// <param name="quantity">selected quantity</param>
        public AddItem(string itemName, string quantity)
            : this() // calling default constructor
        {
            cbItems.SelectedItem = itemName;
            tbQty.Text = quantity;
        }

        /// <summary>
        /// init all items in inventory
        /// </summary>
        private void InitializeWindow()
        {
            DataTable items = id.GetAllItemsForBusiness();
            foreach (DataRow dr in items.Rows)
            {
                cbItems.Items.Add(dr[2]);
                allItems.Add(new Item { ItemCode = dr[0].ToString(), Cost = dr[1].ToString(), ItemDesc = dr[2].ToString() });
            }
        }

        /// <summary>
        /// submission of entries from new item pop up. checks to make sure that all values are valid and an item was selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (cbItems.SelectedIndex > -1) // is the selection valid?
            {
                try
                {
                    ItemDescription = allItems[cbItems.SelectedIndex].ItemCode;
                    Quantity = 1; // default qty of 1

                    if (!int.TryParse(tbQty.Text, out Quantity) || tbQty.Text == "0")
                    {
                        MessageBox.Show("Invalid quantity. Please enter a number greater than 0.");
                    }
                    else
                    {
                        this.Close();
                    }
              
                }
                catch (Exception qtyEx)
                {
                    MessageBox.Show("Invalid selection. Please try again or report the following error: \n\n" + qtyEx + "\n");
                }
                
            }
            else
            {
                MessageBox.Show("Please select an item.");
            }
        }

        /// <summary>
        /// display total cost of selected item in add new item pop-up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbItems.SelectedIndex > -1) // is selection valid?
                {
                    tbCost.Text = allItems[cbItems.SelectedIndex].Cost;
                }
            }
            catch (Exception changeSel_ex)
            {
                MessageBox.Show("Something went wrong when changing selections. Re-try or report the following error: \n\n" + changeSel_ex + "\n");
            }        
        }

      

    }
}
