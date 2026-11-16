using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MSSSStaffDic
{
    public partial class GeneralForm : Form
    {

        /// <summary>
        /// Stores staff IDs and names in a shared Dictionary.
        /// The staff ID is the key and the staff name is the value.
        /// </summary>
        public static Dictionary<int, string> MasterFile = new Dictionary<int, string>();

        /// <summary>
        /// Reads staff records from the CSV file and loads them into the Dictionary.
        /// </summary>
        private void LoadStaff()
        {
            try
            {

                string filePath = "MalinStaffNamesV3.csv";
                MasterFile.Clear();

                using (StreamReader reader = new StreamReader(filePath))
                {

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] data = line.Split(',');
                        if (data.Length == 2 && int.TryParse(data[0], out int id))
                        {
                            string Name = data[1];

                            if (!MasterFile.ContainsKey(id))
                            {
                                MasterFile.Add(id, Name);
                            }
                        }

                    }
                }

                statusStripGeneral.Items[0].Text = "Staff data loaded successfully.";

            }
            catch (Exception ex)
            {
                statusStripGeneral.Items[0].Text = "Failed to load staff data.";
                MessageBox.Show(ex.Message);

            }
        }

        /// <summary>
        /// Displays all records from the Dictionary in the read-only ListBox.
        /// </summary>
        private void DisplayStaff()
        {
            lstAllStaff.Items.Clear();

            foreach (KeyValuePair<int, string> staff in MasterFile)
            {
                lstAllStaff.Items.Add($"{staff.Key} : {staff.Value}");
            }
            statusStripGeneral.Items[0].Text = "Staff list displayed successfully.";

        }

        /// <summary>
        /// Initialises the GeneralForm, loads staff data and displays all staff records.
        /// </summary>
        public GeneralForm()
        {
            InitializeComponent();

            txtSearchID.MaxLength = 9;

            LoadStaff();
            DisplayStaff();

        }

        /// <summary>
        /// Validates keyboard input in the staff ID search field.
        /// </summary>
        /// <param name="sender">The control that raised the KeyPress event.</param>
        /// <param name="e">Provides information about the pressed key.</param>
        private void txtSearchID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
             
                MessageBox.Show("Please enter numbers only.");
                return;
            }
        }

        /// <summary>
        /// Searches the Dictionary using a partial staff ID and name.
        /// Displays matching records and the number of search results.
        /// </summary>
        private void SearchStaff()
        {
            lstSearchResults.Items.Clear();
            txtInfoID.Clear();
            txtInfoName.Clear();

            string searchID = txtSearchID.Text.Trim();
            string searchName = txtSearchName.Text.Replace(" ", "").ToLower();

            if (searchID == "" && searchName == "") return;

            foreach (KeyValuePair<int, string> staff in MasterFile)
            { 
                string staffID = staff.Key.ToString();
                string staffName = staff.Value.ToLower().Replace(" ", "");
                if (staffID.Contains(searchID) && staffName.Contains(searchName))
                {
                    lstSearchResults.Items.Add($"{staff.Key} : {staff.Value}");
                }
            }
            if (lstSearchResults.Items.Count > 0)
            {
                statusStripGeneral.Items[0].Text = $"Search completed successfully. {lstSearchResults.Items.Count} staff found.";
            }
            else
            {
                statusStripGeneral.Items[0].Text = "No matching staff found.";
            }

        }

        /// <summary>
        /// Updates the search results when the staff ID search text changes.
        /// </summary>
        /// <param name="sender">The control that raised the TextChanged event.</param>
        /// <param name="e">Provides data for the TextChanged event.</param>
        private void txtSearchID_TextChanged(object sender, EventArgs e)
        {
           
            SearchStaff();
        }

        /// <summary>
        /// Updates the search results when the staff name search text changes.
        /// </summary>
        /// <param name="sender">The control that raised the TextChanged event.</param>
        /// <param name="e">Provides data for the TextChanged event.</param>
        private void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            SearchStaff();
        }

        /// <summary>
        /// Handles keyboard shortcuts in the GeneralForm.
        /// F1 clears and focuses the ID search field.
        /// F2 clears and focuses the name search field.
        /// Alt+A opens the AdminForm and reloads staff data after a successful save.
        /// </summary>
        /// <param name="sender">The control that raised the KeyDown event.</param>
        /// <param name="e">Provides information about the pressed key and modifiers.</param>
        private void GeneralForm_KeyDown(object sender, KeyEventArgs e)
        {
           
            if (e.KeyCode == Keys.F1)
            {
                txtSearchID.Clear();
                txtSearchID.Focus();
                

            }
            else if (e.KeyCode == Keys.F2)
            {
                txtSearchName.Clear();
                txtSearchName.Focus();
            }
            else if (e.Alt && e.KeyCode == Keys.A)
            {
                
                AdminForm admin = new AdminForm(txtInfoID.Text,txtInfoName.Text);

                if (admin.ShowDialog(this) == DialogResult.OK)
                {
                    // Refresh staff lists
                    LoadStaff();
                    DisplayStaff();

                    // Clear search fields
                    txtSearchID.Clear();
                    txtSearchName.Clear();

                    // Clear search results
                    lstSearchResults.Items.Clear();

                    // Clear staff details
                    txtInfoID.Clear();
                    txtInfoName.Clear();

                    statusStripGeneral.Items[0].Text = "Staff data saved successfully.";
                }
            }
        }

        /// <summary>
        /// Retrieves the selected staff record from the search results
        /// and displays the staff ID and name in the detail fields.
        /// </summary>
        /// <param name="sender">The control that raised the SelectedIndexChanged event.</param>
        /// <param name="e">Provides data for the SelectedIndexChanged event.</param>
        private void lstSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSearchResults.SelectedItem == null)
            {
                return;
            }

            string selectedStaff = lstSearchResults.SelectedItem.ToString();
            string[] data = selectedStaff.Split(':');

            if (data.Length != 2)
            {
                statusStripGeneral.Items[0].Text = "Failed to display staff details.";
                return;
            }

            txtInfoID.Text = data[0].Trim();
            txtInfoName.Text = data[1].Trim();

            statusStripGeneral.Items[0].Text  = "Staff details displayed successfully.";

        }
    }
}
