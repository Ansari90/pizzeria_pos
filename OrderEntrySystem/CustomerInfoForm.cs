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
using DataHolders;

namespace OrderEntrySystem
{
    public partial class CustomerInfoForm : Form
    {
        public delegate void GoBack(Form prevForm, string toDo);
        public GoBack goBack;

        CustomerInfo customerInfo;

        public CustomerInfoForm()
        {
            InitializeComponent();
            info_takeoutRadio.Checked = true;
        }

        private void CustomerInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            goBack(this, "end");
        }

        public void setCustomerInfo(CustomerInfo customerInfo)
        {
            this.customerInfo = customerInfo;
        }

        public void reset()
        {
            info_nameInput.Text = null;
            info_emailInput.Text = null;
            info_primaryPhoneInput.Text = null;
            info_secondaryPhoneInput.Text = null;

            info_unitInput.Text = null;
            info_streetInput.Text = null;
            info_stateInput.Text = null;
            info_cityInput.Text = null;
            info_postalCodeInput.Text = null;
            info_takeoutRadio.Checked = true;
        }

        private void bundleInfo()
        {            
            customerInfo.customerName = info_nameInput.Text;
            customerInfo.emailAddress = info_emailInput.Text;
            customerInfo.primaryPhone = info_primaryPhoneInput.Text;
            customerInfo.secondaryPhone = info_secondaryPhoneInput.Text;

            customerInfo.unitNum = info_unitInput.Text;
            customerInfo.street = info_streetInput.Text;
            customerInfo.state = info_stateInput.Text;
            customerInfo.city = info_cityInput.Text;
            customerInfo.postalCode = info_postalCodeInput.Text;

            customerInfo.notes = info_notesInput.Text;
            customerInfo.orderType = info_deliveryRadio.Checked ? "Delivery" : "Takeout";
        }

        private void info_takeOrderBtn_Click(object sender, EventArgs e)
        {
            if(info_primaryPhoneInput.Text.Equals(""))
            {
                MessageBox.Show("Please enter the customer's primary phone number!");
            }
            else
            {
                if (info_deliveryRadio.Checked && info_streetInput.Text.Equals("")) 
                {
                    MessageBox.Show("For delivery orders, please enter a valid street name/number combination");
                }
                else
                {
                    bundleInfo();
                    goBack(this, "next");
                }
            }
        }

        public void searchForFile(object sender, EventArgs e)
        { 
            if(File.Exists(info_primaryPhoneInput.Text + ".txt"))
            {                
                StreamReader streamReader = new StreamReader(info_primaryPhoneInput.Text + ".txt");
                reset();

                int tildeCounter = 0;
                char theChar;
                while(streamReader.Peek() != -1)
                {
                    theChar = (char)streamReader.Read();
                    if(theChar.Equals('~'))
                    {
                        tildeCounter++;
                        continue;
                    }

                    switch(tildeCounter)
                    {
                        case 0:
                            info_nameInput.Text += theChar;
                            break;
                        case 1:
                            info_emailInput.Text += theChar;
                            break;
                        case 2:
                            info_primaryPhoneInput.Text += theChar;
                            break;
                        case 3:
                            info_primaryPhoneInput.Text += theChar;
                            break;
                        case 4:
                            info_unitInput.Text += theChar;
                            break;
                        case 5:
                            info_streetInput.Text += theChar;
                            break;
                        case 6:
                            info_cityInput.Text += theChar;
                            break;
                        case 7:
                            info_stateInput.Text += theChar;
                            break;
                        case 8:
                            info_postalCodeInput.Text += theChar;
                            break;
                        case 9:
                            info_notesInput.Text += theChar;
                            break;
                    }
                }

                streamReader.Close();                
                bundleInfo();
            }
        } 
    }
}
