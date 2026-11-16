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
    public partial class AdminForm : Form
    {
        /// <summary>
        /// Stores the current administration mode: Create, Update or Delete.
        /// An empty string indicates that no operation has been selected.
        /// </summary>
        private string currentMode = "";

        /// <summary>
        /// Indicates whether a valid staff record was selected in GeneralForm.
        /// </summary>
        private bool hasSelectedStaff = false;

        /// <summary>
        /// Stores the original staff ID for use during deletion.
        /// </summary>
        private string originalStaffID = "";

        /// <summary>
        /// Initialises the AdminForm with the selected staff information. Sets the input fields to read-only
        /// </summary>
        /// <param name="staffID">The ID of the selected staff member.</param>
        /// <param name="staffName">The name of the selected staff member.</param>
        public AdminForm(string staffID, string staffName)
        {
            InitializeComponent();

            txtAdminID.MaxLength = 9;

            txtAdminID.Text = staffID;
            txtAdminName.Text = staffName;

            txtAdminID.ReadOnly = true;
            txtAdminName.ReadOnly = true;

            hasSelectedStaff = int.TryParse(staffID, out int id) &&
                       GeneralForm.MasterFile.ContainsKey(id);

            originalStaffID = staffID;
        }

        /// <summary>
        /// Activates Create mode and enables the staff ID and name fields.
        /// </summary>
        private void CreateInputStaff()
        {
            if (currentMode != "")
            {
                MessageBox.Show("Please complete the current operation first.");
                return;
            }

            currentMode = "Create";
            statusStripAdmin.Items[0].Text = "Enter a new Staff ID and name.";

            txtAdminID.ReadOnly = false;
            txtAdminName.ReadOnly = false;

            txtAdminID.Text = "77";
            txtAdminName.Clear();

            txtAdminID.Focus();
            txtAdminID.SelectionStart = txtAdminID.Text.Length;
           
        }
        /// <summary>
        /// Adds the validated staff record to the Dictionary.
        /// </summary>
        /// <returns>
        /// True if the staff record is successfully added;
        /// otherwise, false if validation fails.
        /// </returns>
        private bool CreateStaff()
        {
            // Validate Staff ID
            if (!int.TryParse(txtAdminID.Text, out int staffID) ||
                txtAdminID.Text.Length != 9 ||
                !txtAdminID.Text.StartsWith("77"))
            {
                MessageBox.Show("Staff ID must start with 77 and be 9 digits.");
                return false;
            }

            // Validate Staff Name
            if (string.IsNullOrWhiteSpace(txtAdminName.Text))
            {
                MessageBox.Show("Please enter a staff name.");
                return false;
            }

            // Check duplicate Staff ID
            if (GeneralForm.MasterFile.ContainsKey(staffID))
            {
                MessageBox.Show("Staff ID already exists.");
                return false;
            }

            // Add staff to Dictionary
            GeneralForm.MasterFile.Add(staffID, txtAdminName.Text.Trim());

            return true;

        }

        /// <summary>
        /// Activates Update mode and enables editing of the selected staff name.
        /// </summary>
        private void UpdateInputStaff()
        {
            if (currentMode != "")
            {
                MessageBox.Show("Please complete the current operation first.");
                return;
            }

            currentMode = "Update";
            statusStripAdmin.Items[0].Text = "Enter the updated staff name.";

            txtAdminName.ReadOnly = false;
            txtAdminName.Focus();

        }

        /// <summary>
        /// Updates the existing staff record in the Dictionary using its staff ID.
        /// </summary>
        /// <returns>
        /// True if the staff name is updated
        /// otherwise, false if the name is empty or contains only whitespace.
        /// </returns>
        private bool UpdateStaff()
        {
            int staffID = int.Parse(txtAdminID.Text);

            // Validate Staff Name
            if (string.IsNullOrWhiteSpace(txtAdminName.Text))
            {
                MessageBox.Show("Please enter a staff name.");
                return false;
            }

            // Update staff name in Dictionary
            GeneralForm.MasterFile[staffID] = txtAdminName.Text.Trim();

            return true;
        }

        /// <summary>
        /// Activates Delete mode and clears the displayed staff details.
        /// </summary>
        private void DeleteInputStaff()
        {
            if (currentMode != "")
            {
                MessageBox.Show("Please complete the current operation first.");
                return;
            }

            currentMode = "Delete";
            statusStripAdmin.Items[0].Text = "The selected staff member will be deleted.";

            txtAdminID.Clear();
            txtAdminName.Clear();

        }

        /// <summary>
        /// Removes the originally selected staff record from the Dictionary
        /// using the preserved staff ID.
        /// </summary>
        /// <returns>
        /// True if the staff record is successfully removed;
        /// otherwise, false if the staff ID cannot be found.
        /// </returns>
        private bool DeleteStaff()
        {
            int staffID = int.Parse(originalStaffID);

            if (!GeneralForm.MasterFile.Remove(staffID))
            {
                MessageBox.Show("Failed to delete staff.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles keyboard shortcuts for administration operations.
        /// Alt+C activates Create mode.
        /// Alt+U activates Update mode when a valid staff record is selected.
        /// Alt+D activates Delete mode when a valid staff record is selected.
        /// Alt+L validates, applies and saves the selected operation before closing.
        /// Escape closes the AdminForm without applying pending changes.
        /// </summary>
        /// <param name="sender">The control that raised the KeyDown event.</param>
        /// <param name="e">Provides information about the pressed key and modifiers.</param>
        private void AdminForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.L)
            {
                if (currentMode == "Create")
                {
                    if (!CreateStaff())
                    {
                        return;
                    }
                }
                else if (currentMode == "Update")
                {
                    if (!UpdateStaff())
                    {
                        return;
                    }
                }
                else if (currentMode == "Delete")
                {
                    if (!DeleteStaff())
                    {
                        return;
                    }
                }
                if (currentMode != "")
                {
                    if (!SaveStaff())
                    {
                        return;
                    }
                    //  Notify GeneralForm that changes were saved successfully.
                    this.DialogResult = DialogResult.OK;
                }

                this.Close();
            }
            else if (e.Alt && e.KeyCode == Keys.C)
            {
                CreateInputStaff();
            }
            else if (e.Alt && e.KeyCode == Keys.U)
            {
                if (hasSelectedStaff)
                {
                    UpdateInputStaff();
                }
            }
            else if (e.Alt && e.KeyCode == Keys.D)
            {
                if (hasSelectedStaff)
                {
                    DeleteInputStaff();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

        }

        /// <summary>
        /// Validates keyboard input in the staff ID field.
        /// Allows numeric characters and control keys when the field is editable.
        /// </summary>
        /// <param name="sender">The control that raised the KeyPress event.</param>
        /// <param name="e">Provides information about the pressed key.</param>
        private void txtAdminID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtAdminID.ReadOnly)
            {
                return;
            }

            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Please enter numbers only.");
                return;
            }
            

        }

        /// <summary>
        /// Saves all staff records from the Dictionary to the CSV file.
        /// Overwrites the existing file with the updated staff data.
        /// </summary>
        /// <returns>
        /// True if all staff records are successfully saved to the CSV file;
        /// otherwise, false if an exception occurs during saving.
        /// </returns>
        private bool SaveStaff()
        {
            try
            {
                string filePath = "MalinStaffNamesV3.csv";

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (KeyValuePair<int, string> staff in GeneralForm.MasterFile)
                    {
                        writer.WriteLine($"{staff.Key},{staff.Value}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                statusStripAdmin.Items[0].Text = "Failed to save staff data.";
                MessageBox.Show("Error saving staff data: " + ex.Message);
                return false;
            }
        }
    }
}
