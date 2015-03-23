using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataHolders;

namespace OrderEntrySystem
{
    public partial class FinalOrderForm : Form
    {
        public delegate void GoBack(Form prevForm, string toDo);
        public GoBack goBack;

        public delegate void ModPizza(string pizzaString);
        public ModPizza modifyPizza;

        CustomerInfo customerInfo;
        OrderInfo orderInfo;

        public FinalOrderForm()
        {
            InitializeComponent();
            final_orderItemList.SelectedIndex = -1;
        }

        public void sendData(CustomerInfo customerInfo, OrderInfo orderInfo)
        {
            this.customerInfo = customerInfo;
            this.orderInfo = orderInfo;
        }

        public void refresh()
        {
            final_orderItemList.Items.Clear();
            double finalTotal = 0.0;

            for(int i = 0; i < orderInfo.productNames.Count; i++)
            {
                final_orderItemList.Items.Add(orderInfo.productNames[i] + "\t" + orderInfo.productQuantities[i] + "\t" + orderInfo.productQuantities[i] 
                    * orderInfo.productPrices[i]);
                finalTotal += orderInfo.productQuantities[i] * orderInfo.productPrices[i];
            }

            final_orderTotalLabel.Text = "" + finalTotal;
        }

        private void final_backBtn_Click(object sender, EventArgs e)
        {
            goBack(this, "previous");
        }

        public void displayQuantity(object sender, EventArgs e)
        {
            if (final_orderItemList.SelectedIndex >= 0)
            {
                final_quantityUpdateInput.Value = orderInfo.productQuantities[final_orderItemList.SelectedIndex];
                final_priceInput.Text = "" + orderInfo.productPrices[final_orderItemList.SelectedIndex];
                final_discountInput.Text = "";

                if (orderInfo.productNames[final_orderItemList.SelectedIndex].Contains("Pizza"))
                {
                    final_modifyBtn.Visible = true;
                }
                else
                {
                    final_modifyBtn.Visible = false;
                }
            }            
        }

        private void final_deleteBtn_Click(object sender, EventArgs e)
        {
            if (final_orderItemList.SelectedIndex >= 0)
            {
                orderInfo.removeAtIndex(final_orderItemList.SelectedIndex, true);
                refresh();
            }            
        }

        private void final_updateBtn_Click(object sender, EventArgs e)
        {
            if (final_orderItemList.SelectedIndex >= 0)
            {

                orderInfo.productQuantities[final_orderItemList.SelectedIndex] = (int)final_quantityUpdateInput.Value;

                if (final_discountInput.Text == "")
                {
                    orderInfo.productPrices[final_orderItemList.SelectedIndex] = double.Parse(final_priceInput.Text);
                }
                else
                {
                    orderInfo.productPrices[final_orderItemList.SelectedIndex] *= 1 - (double.Parse(final_discountInput.Text) / 100);
                }

                refresh();
            }            
        }

        private void saveDataToFile()
        {
            string customerFile = customerInfo.primaryPhone + ".txt";
            if(File.Exists(customerFile))
            {
                File.Delete(customerFile);
            }

            StreamWriter streamWriter = new StreamWriter(customerFile);
            streamWriter.Write(customerInfo.customerName + "~" + customerInfo.emailAddress + "~" + customerInfo.primaryPhone + "~" +
                customerInfo.secondaryPhone + "~" + customerInfo.unitNum + "~" + customerInfo.street + "~" +
                customerInfo.city + "~" + customerInfo.state + "~" + customerInfo.postalCode + "~" + customerInfo.notes);
            streamWriter.Close();
        }

        private void final_placeOrderBtn_Click(object sender, EventArgs e)
        {
            saveDataToFile();
            goBack(this, "next");
        }

        private void final_modifyBtn_Click(object sender, EventArgs e)
        {
            if (final_orderItemList.SelectedIndex >= 0)
            {
                modifyPizza(orderInfo.productNames[final_orderItemList.SelectedIndex]);
                orderInfo.removeAtIndex(final_orderItemList.SelectedIndex, true);
                this.Hide();
            }
        }

        private void FinalOrderForm_VisibleChanged(object sender, EventArgs e)
        {
            refresh();
        }

        private void FinalOrderForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            goBack(this, "end");
        }
    }
}
