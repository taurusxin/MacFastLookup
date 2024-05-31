using System;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;

namespace MacFastLookup
{
    public partial class FormMain : Form
    {
        public static void LoadConfig()
        {
            if (!File.Exists("config.json"))
            {
                MessageBox.Show("config.json not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            string json = File.ReadAllText("config.json");
            Global.config = JsonSerializer.Deserialize<Config>(json);
            if (Global.config == null)
            {
                Environment.Exit(1);
            }
        }
        public FormMain()
        {
            InitializeComponent();
            LoadConfig();
            sqliteHelper = new SQLiteHelper("mac_vendors.sqlite3");
            LoadVersion();
        }

        private SQLiteHelper sqliteHelper;
        private string CurrentVersion = string.Empty;
        private HttpHelper httpHelper = new HttpHelper();

        private void LoadVersion()
        {
            CurrentVersion = sqliteHelper.QueryVersion();
            label7.Text = CurrentVersion;
        }

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

        private async void button1_Click(object sender, EventArgs e)
        {
            string LatestVersion = await httpHelper.GetAsync(Global.config.CheckUpdateUrl);
            if (LatestVersion == null)
            {
                MessageBox.Show("Failed to get latest version");
                return;
            }
            if (!string.IsNullOrEmpty(LatestVersion))
            {
                if (LatestVersion != CurrentVersion)
                {
                    var result = MessageBox.Show("New database version available: " + LatestVersion +
                        ". Do you want to download it?", "New version available", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Updater updater = new Updater();
                        updater.ShowDialog();
                        if(updater.finished)
                        if (File.Exists("mac_vendors.sqlite3.temp"))
                        {
                            sqliteHelper.Close();
                            File.Delete("mac_vendors.sqlite3");
                            File.Move("mac_vendors.sqlite3.temp", "mac_vendors.sqlite3");
                            sqliteHelper = new SQLiteHelper("mac_vendors.sqlite3");
                            LoadVersion();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You are using the latest version of database.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
