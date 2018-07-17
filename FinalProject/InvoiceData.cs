using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
namespace FinalProject
{
    /// <summary>
    /// Class used to construct and run queires in conjunction with the Data Access layer
    /// </summary>
    class InvoiceData
    {
        /// <summary>
        /// data access object uised for this 
        /// </summary>
        private clsDataAccess _dataAccess = new clsDataAccess();

        /// <summary>
        /// Globale datatable object
        /// </summary>
        private DataTable dt = new DataTable();

        /// <summary>
        /// Global int object for db return value
        /// </summary>
        private int rowCountThatNoOneCaresAbout = 0;

        #region constants
        /// <summary>
        /// creates the invoice by inserting it into the database
        /// </summary>
        private const string SQL_CREATE_INVOICE = "INSERT INTO Invoices (InvoiceDate, TotalCharge) VALUES ('{0}', 0)";
        /// <summary>
        /// Updates the total charge of the invoice
        /// </summary>
        private const string SQL_UPDATE_INVOICECHARGE = "UPDATE Invoices SET TotalCharge={0} WHERE InvoiceNum={1}";
        /// <summary>
        /// Deletes an item from the invoice by deleting it from the database
        /// </summary>
        private const string SQL_DELETE_INVOICE = "DELETE FROM Invoices WHERE InvoiceNum={0}";
        /// <summary>
        /// Deletes an item from the invoice
        /// </summary>
        private const string SQL_DELETE_INVOICELINEITEMS = "DELETE FROM LineItems WHERE InvoiceNum={0}";
        /// <summary>
        /// Deletes an item from the invoice
        /// </summary>
        private const string SQL_DELETE_INVOICEITEM = "DELETE FROM LineItems WHERE (InvoiceNum = {0}) AND (ItemCode = (SELECT ItemCode FROM ItemDesc WHERE (ItemDesc = '{1}')))";
        /// <summary>
        /// inserts a new item into a specific invoice
        /// </summary>
        private const string SQL_INSERT_ITEMTOINVOICE = "Insert INTO LineItems VALUES({0},{1}, '{2}') ";
        /// <summary>
        /// Adds a new item into the database
        /// </summary>
        private const string SQL_ADD_ITEMDESC = "INSERT INTO ItemDesc (ItemCode, ItemDesc, Cost) VALUES ('{0}', '{1}', {2})";
        /// <summary>
        /// updates an items cost and description in the database based on the itemcode
        /// </summary>
        private const string SQL_UPDATE_ITEMDESC = "UPDATE ItemDesc SET description='{0}', cost={1} where ItemCode='{2}'";
        /// <summary>
        /// Deletes an item from the database itemdesc table
        /// </summary>
        private const string SQL_DELETE_ITEMS = "DELETE FROM ItemDesc WHERE ItemCode = '{0}'";
        /// <summary>
        /// gets the total charge for an invoice based on the invoice number
        /// </summary>
        private const string SQL_GET_TOTALINVOICECHARGE = "SELECT SUM(Cost) AS Charge FROM ItemDesc id INNER JOIN LineItems li on id.ItemCode = li.ItemCode WHERE li.InvoiceNum={0}";
        /// <summary>
        /// gets all distinct invoices from the lineitems table
        /// </summary>
        private const string SQL_GET_INVOICES_FOR_LINEITEMS = "SELECT DISTINCT InvoiceNum FROM LineItems WHERE ItemCode = '{0}'";
        /// <summary>
        /// gets all entries from the itemdesc table
        /// </summary>
        private const string SQL_GET_ITEMDESC = "SELECT ItemCode AS Name, Cost, ItemDesc AS Description FROM ItemDesc";
        /// <summary>
        /// gets the last invoice that was inserted
        /// </summary>
        private const string SQL_GET_LASTINVOICEKEY = "SELECT @@Identity from Invoices";
       // private const string SQL_GET_ITEMCODEBYDESCR = "SELECT TOP 1 ItemCode FROM ItemDesc WHERE ItemDesc='{0}'";
        /// <summary>
        /// gets all invoices from the invoice table and formats the date to only show month day year
        /// </summary>
        private const string SQL_GET_INVOICES = "SELECT InvoiceNum, Format(InvoiceDate, 'mm/dd/yyyy') AS [Date], TotalCharge FROM Invoices";
        /// <summary>
        /// selects all distinct charges from the database for use in a dropdown
        /// </summary>
        private const string SQL_GET_CHARGES_DISTINCT = "SELECT DISTINCT TotalCharge FROM Invoices";
        /// <summary>
        /// selects all invoices after filtering by the combo boxes
        /// </summary>
        private const string SQL_GET_FILTERED_INVOICES = "SELECT * FROM INVOICES WHERE InvoiceNum = '{0}' AND InvoiceDate = '{1}' AND TotalCharge = '{2}'";
        /// <summary>
        /// Selects all discting date entries from the database to load into the combobox
        /// </summary>
        private const string SQL_GET_DISTINCT_INV_DATES = "SELECT DISTINCT Format(InvoiceDate, 'mm/dd/yyyy') AS [Date] FROM Invoices";
        /// <summary>
        /// selects all items from itemdesc table
        /// </summary>
        private const string SQL_GET_ITEMS = "SELECT ItemCode AS Name, Cost, ItemDesc AS Description FROM ItemDesc";
        /// <summary>
        /// shows how many line items there are based on invoice number
        /// </summary>
        private const string SQL_GET_MAXLINEITEM = "SELECT MAX(LineItemNum) FROM LineItems WHERE InvoiceNum={0}";
        /// <summary>
        /// gets an invocie and all of its information based on the invoice number
        /// </summary>
        private const string SQL_GET_INVOICEITEMS = "SELECT 0 AS selected, ID.ItemDesc, COUNT(ID.ItemDesc) AS qty, SUM(ID.Cost) AS Cost FROM (LineItems LI INNER JOIN ItemDesc ID ON LI.ItemCode = ID.ItemCode) WHERE (LI.InvoiceNum = {0}) GROUP BY ID.ItemDesc";
        /// <summary>
        /// gets a formatted invoice date based on the invoice number
        /// </summary>
        private const string SQL_GET_INVOICEDATE = "SELECT Format(InvoiceDate, 'mm/dd/yyyy') AS [Date] from Invoices where InvoiceNum={0}";
        /// <summary>
        /// Gets the total charge of the invoice and formats it appropriately if there are decimal values
        /// </summary>
        private const string SQL_GET_INVOICETOTALCHARGE = "SELECT ROUND(TotalCharge,2) from Invoices where InvoiceNum={0}";
        #endregion

        /// <summary>
        /// gets all invoices from the database
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllInvoices()
        {
            try
            {                
                dt = _dataAccess.ExecuteSQLStatement(SQL_GET_INVOICES, ref rowCountThatNoOneCaresAbout).Tables[0];
                
                return dt;
            }
            catch (Exception selectException)
            {
                throw new InvoiceDataException("Unable to retrieve invoices from database.", selectException);
            }
        }
        /// <summary>
        /// Gets the proper invoice number and its information based on user selection
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <returns></returns>
        public DataTable GetInvoice(string invoiceNum)
        {
            try
            {
                int rowCount = -1;
                dt = _dataAccess.ExecuteSQLStatement(string.Format(SQL_GET_INVOICEITEMS,invoiceNum), ref rowCount).Tables[0];
                return dt;
            }
            catch (Exception selectException)
            {
                throw new InvoiceDataException("Unable to retrieve costs from database.", selectException);
            }
            
        }
        /// <summary>
        /// grabs the appropriate dates from the database depending on search criteria
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <returns></returns>
        public string GetInvoiceDate(string invoiceNum)
        {
            try
            {
                return _dataAccess.ExecuteScalarSQL(string.Format(SQL_GET_INVOICEDATE, invoiceNum));
            }
            catch (Exception e)
            {
                throw new InvoiceDataException(string.Format("Unable to retrieve invoice date for invoice {0}", invoiceNum), e);
            }
        }
        /// <summary>
        /// Calculates the total cost based on an invoice number
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <returns></returns>
        public string GetInvoiceTotalCharge(string invoiceNum)
        {
            //FYI, this is really poor form to have a calculated total... even if it is updated from a trigger (which it is not), is pretty gross
            try
            {
                return _dataAccess.ExecuteScalarSQL(string.Format(SQL_GET_INVOICETOTALCHARGE, invoiceNum));
            }
            catch (Exception e)
            {
                throw new InvoiceDataException(string.Format("Unable to retrieve invoice date for invoice {0}", invoiceNum), e);
            }
        }

        /// <summary>
        /// Gets filtered invoice data from database based on passed in parameters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        public DataTable GetInvoices(string id, string date,  string cost)
        {
            string sql = "SELECT InvoiceNum, Format(InvoiceDate, 'mm/dd/yyyy') AS [Date], TotalCharge FROM Invoices WHERE ";
            if (!string.IsNullOrEmpty(id))
            {
                sql += "InvoiceNum = "+id+" " ;
            }
            if (!string.IsNullOrEmpty(date))
            {
                if (!string.IsNullOrEmpty(id))
                {
                    sql += " AND ";
                }
                sql += "InvoiceDate = Format('" + date + "', 'mm/dd/yyyy') ";
            }
            if (!string.IsNullOrEmpty(cost))
            {
                if (!string.IsNullOrEmpty(date))
                {
                    sql += " AND ";
                }
                sql += "TotalCharge = "+cost+" ";
            }
            dt = _dataAccess.ExecuteSQLStatement(sql, ref rowCountThatNoOneCaresAbout).Tables[0];
            return dt;
        }
        /// <summary>
        /// Geets distinct cost values
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDistinctCharges()
        {
            try
            {
                dt = _dataAccess.ExecuteSQLStatement(SQL_GET_CHARGES_DISTINCT, ref rowCountThatNoOneCaresAbout).Tables[0];
                return dt;
            }
            catch (Exception selectException)
            {
                throw new InvoiceDataException("Unable to retrieve costs from database.", selectException);
            }
        }
        /// <summary>
        /// Checks if an item already exists, returns true is yes
        /// </summary>
        /// <param name="ItemCode"></param>
        /// <returns></returns>
        public bool checkItemCode(string ItemCode)
        {
            string sql = "SELECT ItemCode FROM ItemDesc WHERE ItemCode = '" + ItemCode + "'";
            dt = _dataAccess.ExecuteSQLStatement(sql, ref rowCountThatNoOneCaresAbout).Tables[0];
            if(dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Gets all distinct invoice date values from the db
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDistinctInvoiceDates()
        {            
            try
            {
                dt = _dataAccess.ExecuteSQLStatement(SQL_GET_DISTINCT_INV_DATES, ref rowCountThatNoOneCaresAbout).Tables[0];
                return dt;
            }
            catch (Exception selectException)
            {
                throw new InvoiceDataException("Unable to retrieve costs from database.", selectException);
            }
        }

        /// <summary>
        /// Creates a new invoice after choosing a date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int CreateInvoice(DateTime date)
        {
            try
            {
                _dataAccess.ExecuteNonQuery(string.Format(SQL_CREATE_INVOICE, date));
            }
            catch (Exception queryException)
            {
                throw new InvoiceDataException("Unable to insert record into Invoices table.", queryException);
            }

            int invoiceNum;

            try
            {
                string invoice = _dataAccess.ExecuteScalarSQL(SQL_GET_LASTINVOICEKEY);
                if (!int.TryParse(invoice, out invoiceNum))
                {
                    throw new InvoiceDataException("Unable to interpret invoice number retrieved from table.");
                }
            }
            catch (Exception queryException)
            {
                throw new InvoiceDataException("Unable to retrieve last key from Invoices table.", queryException);
            }

            return invoiceNum;
        }

        /// <summary>
        /// deletes line items from the invoice and then deletes the invoice
        /// </summary>
        /// <param name="invoiceNum"></param>
        public void DeleteInvoice(string invoiceNum)
        {
            //delete line items for this invoice
            try
            {
                _dataAccess.ExecuteNonQuery(string.Format(SQL_DELETE_INVOICELINEITEMS, invoiceNum));
            }
            catch (Exception deleteException)
            {
                throw new InvoiceDataException(string.Format("Unable to delete line items for invoice {0}", invoiceNum), deleteException);
            }

            try
            {
                _dataAccess.ExecuteNonQuery(string.Format(SQL_DELETE_INVOICE, invoiceNum));
            }
            catch (Exception deleteException)
            {
                throw new InvoiceDataException(string.Format("Unable to delete invoice {0}", invoiceNum), deleteException); 
            }
        }

        /// <summary>
        /// Adds an item to the invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="itemCode"></param>
        public void AddItemToInvoice(string invoiceNumber, string itemCode, int quantity)
        {
            try
            {
                for (int i = 0; i < quantity; i++)
                {
                    _dataAccess.ExecuteNonQuery(string.Format(SQL_INSERT_ITEMTOINVOICE, invoiceNumber, GetMaxLineItemNum(invoiceNumber.ToString()) + 1, itemCode));
                }
                UpdateInvoiceTotalCharge(invoiceNumber);

            }
            catch (Exception insertException)
            {
                throw new InvoiceDataException(string.Format("Unable to insert itemCode '{0}' on invoice '{1}'", itemCode, invoiceNumber), insertException);
            }
        }
        /// <summary>
        /// updates the total charge of the invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        public void UpdateInvoiceTotalCharge(string invoiceNumber)
        {
            try
            {
                string charge = _dataAccess.ExecuteScalarSQL(string.Format(SQL_GET_TOTALINVOICECHARGE, invoiceNumber));
                if (charge == string.Empty)
                {
                    charge = "0";
                }
                double charges = double.Parse(charge);
                _dataAccess.ExecuteNonQuery(string.Format(SQL_UPDATE_INVOICECHARGE, charges, invoiceNumber));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// deletes an item from a specified invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="itemDescription"></param>
        public void DeleteInvoiceItem(string invoiceNumber, string itemDescription)
        {
            try
            {
                _dataAccess.ExecuteNonQuery(string.Format(SQL_DELETE_INVOICEITEM,invoiceNumber,itemDescription));
                UpdateInvoiceTotalCharge(invoiceNumber);
            }
            catch (Exception e)
            {
                throw new InvoiceDataException(string.Format("Unable to delete item '{1}' from invoice {0}.", invoiceNumber, itemDescription),e);
            }
        }
        //Saving this code in case needed later. collapsing in a region to save space
        #region GetItemCodeByDescription
        
        //public string GetItemCodeByDescription(string description)
        //{
        //    //escape characters for sql statement if required
        //    if (description.IndexOf("'") > -1)
        //    {
        //        description = description.Replace("'","''");
        //    }

        //    try
        //    {
        //        //TODO: THIS IS WRONG!!!
        //        return _dataAccess.ExecuteScalarSQL(string.Format(SQL_INSERT_ITEMTOINVOICE, description));
        //    }
        //    catch (Exception insertException)
        //    {
        //        throw new InvoiceDataException(string.Format("Unable to retrieve itemCode with description '{0}'",description), insertException);
        //    }

        //}
        #endregion

        /// <summary>
        /// finds how many items are in a particular invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public int GetMaxLineItemNum(string invoice)
        {
            try
            {
                string max = _dataAccess.ExecuteScalarSQL(string.Format(SQL_GET_MAXLINEITEM, invoice));
                max = (max == "") ? "0" : max;
                return int.Parse(max);
            }
            catch (Exception e)
            {
                throw new InvoiceDataException("DURRR", e);
            }
        }

        /// <summary>
        /// gets all of the items for the business and loads it into a datatable
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllItemsForBusiness()
        {
            try
            {
                int rowCountThatNoOneCaresAbout = 0;
                DataTable dt = _dataAccess.ExecuteSQLStatement(SQL_GET_ITEMDESC, ref rowCountThatNoOneCaresAbout).Tables[0];
                return dt;
            }
            catch (Exception selectException)
            {
                throw new InvoiceDataException("Unable to retrieve item descriptions from Invoice database.", selectException);
            }
        }

        /// <summary>
        /// SQL to update the item in the database. can alter the description and cost but not the itemcode
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="description"></param>
        /// <param name="cost"></param>
        public void UpdateItem(string itemCode, string description, string cost)
        {
            string sql = "UPDATE ItemDesc SET ItemDesc='"+description+"', cost="+cost+" where ItemCode='"+itemCode+"'";
            try
            {
                _dataAccess.ExecuteNonQuery(sql);
            }
            catch (Exception updateException)
            {
                throw new InvoiceDataException("Unable to update item code {0} with description '{1}' and cost of {2}", updateException);
            }
        }
            
        /// <summary>
        /// attempts to delete an item from the database. if the item is used on an invoice, do not allow the item
        /// to be deleted and display a messagebox showing which invoices the item is on
        /// </summary>
        /// <param name="itemCode"></param>
        public void DeleteItem(string itemCode)
        {
            int invoiceCount = 0;
            DataSet ds = _dataAccess.ExecuteSQLStatement(string.Format(SQL_GET_INVOICES_FOR_LINEITEMS, itemCode), ref invoiceCount);
            if (invoiceCount > 0)
            {
                StringBuilder sb = new StringBuilder();
                //forech row in ds.tables[0], add row to a string. this will be invoice numbers.
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sb.Append(" " + dr[0]);
                }
                MessageBox.Show("Error cannot delete, item exists in Invoices: " + sb.ToString());
            }
            else
            {
                try
                {
                    _dataAccess.ExecuteNonQuery(string.Format(SQL_DELETE_ITEMS, itemCode));
                }
                catch (Exception deleteException)
                {
                    throw new InvoiceDataException(string.Format("Unable to delete item {0}", itemCode), deleteException);
                }
            }
        }

        /// <summary>
        /// Adds an item to the item desc table
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="itemDesc"></param>
        /// <param name="itemCost"></param>
        public void AddItem(string itemCode, string itemDesc, string itemCost)
        {
            try
            {
                _dataAccess.ExecuteNonQuery(string.Format(SQL_ADD_ITEMDESC, itemCode, itemDesc, itemCost));
            }
            catch (Exception deleteException)
            {
                throw new InvoiceDataException(string.Format("Unable to insert item code.", itemCode), deleteException);
            }
        }
    }

    /// <summary>
    /// Simple exception class to wrap Exception
    /// </summary>
    public class InvoiceDataException : Exception
    {
        public InvoiceDataException() 
            : base()
        { }

        public InvoiceDataException(string message)
            : base(message)
        { }

        public InvoiceDataException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
