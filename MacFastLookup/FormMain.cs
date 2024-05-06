using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MacFastLookup
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            if (File.Exists("mac_vendors.sqlite3") == false)
            {
                MessageBox.Show("mac_vendors.sqlite3 not found. Please download it first");
                Environment.Exit(1);
            }
            sqliteHelper = new SQLiteHelper("mac_vendors.sqlite3");
        }

        private SQLiteHelper sqliteHelper;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // get mac address from textbox
            string mac = macAddressTextBox.Text;
            if (mac.Length < 6)
            {
                return;
            }

            // remove special characters from mac address
            mac = mac.Replace(":", "").Replace("-", "").Replace(".", "").Replace(" ", "");
            if (mac.Length < 6)
            {
                return;
            }

            // find mac address prefix
            mac = mac.Substring(0, 6);

            // convert to uppercase
            mac = mac.ToUpper();

            // add : to mac address
            mac = mac.Insert(2, ":").Insert(5, ":");

            MacAddress address = sqliteHelper.QueryMacAddressByPrefix(mac);
            if (address != null)
            {
                prefixTextBox.Text = address.Prefix;
                vendorTextBox.Text = address.VendorName;
                privateCheckBox.Checked = address.Private;
                blockTypeTextBox.Text = address.BlockType;
                lastUpdateTextBox.Text = address.LastUpdate;
            }
            else
            {
                prefixTextBox.Text = "Not found";
                vendorTextBox.Text = "";
                privateCheckBox.Checked = false;
                blockTypeTextBox.Text = "";
                lastUpdateTextBox.Text = "";
            }
        }
    }
}
