using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataHolders;

namespace OrderEntrySystem
{
    public partial class Form1 : Form
    {
        List<Form> orderProcessForms;

        CustomerInfoForm customerInfoForm;
        OrderEntryForm orderEntryForm;
        FinalOrderForm finalOrderForm;

        IFormatter formatter;
        TcpClient client;

        System.Drawing.Printing.PrintDocument printDoc;

        CustomerInfo customerInfo;
        OrderInfo orderInfo;

        public Form1()
        {
            InitializeComponent();
            createForms();
            assignDataHolders();

            formatter = new BinaryFormatter();

            printDoc = new System.Drawing.Printing.PrintDocument();
            printDoc.DocumentName = "tempOrder.txt";
            printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printPage);
        }

        public void createForms()
        {
            orderProcessForms = new List<Form>();

            customerInfoForm = new CustomerInfoForm();
            customerInfoForm.goBack = changeActiveForm;
            orderProcessForms.Add(customerInfoForm);

            orderEntryForm = new OrderEntryForm();
            orderEntryForm.goBack = changeActiveForm;
            orderProcessForms.Add(orderEntryForm);

            finalOrderForm = new FinalOrderForm();
            finalOrderForm.goBack = changeActiveForm;
            finalOrderForm.modifyPizza = modifyPizza;
            orderProcessForms.Add(finalOrderForm);

            assignDataHolders();
        }

        public void changeActiveForm(Form prevForm, string toDo)
        {
            prevForm.Hide();
            switch (toDo)
            {
                case "next":
                    if (orderProcessForms.IndexOf(prevForm) == 2)
                    {
                        this.Show();
                        sendToMakeLine();
                        makeOrderReceipt();
                        assignDataHolders();
                    }
                    else
                    {
                        orderProcessForms[orderProcessForms.IndexOf(prevForm) + 1].Show();
                    }
                    break;
                case "previous":
                    orderProcessForms[orderProcessForms.IndexOf(prevForm) - 1].Show();
                    break;
                case "end":
                    this.Show();
                    createForms();
                    assignDataHolders();
                    break;
            }
        }

        public void sendToMakeLine()
        {
            StreamReader reader = new StreamReader("networkData.txt");
            string targetCompName = reader.ReadLine();
            int portNum = Int32.Parse(reader.ReadLine());
            reader.Close();

            client = new TcpClient(targetCompName, portNum);
            formatter.Serialize(client.GetStream(), orderInfo);
            client.Close();
        }

        public void makeOrderReceipt()
        {
            StreamWriter streamWriter = new StreamWriter("tempOrder.txt");
            double total = 0.0;

            streamWriter.WriteLine(customerInfo.customerName + " :: " + customerInfo.primaryPhone);
            streamWriter.WriteLine(customerInfo.unitNum + " - " + customerInfo.street);
            streamWriter.WriteLine("-----------------------------------------");
            for(int i = 0; i < orderInfo.productNames.Count; i++)
            {
                streamWriter.WriteLine(orderInfo.productNames[i] + " :: " + orderInfo.productQuantities[i]);
                total += orderInfo.productQuantities[i] * orderInfo.productPrices[i];
            }
            streamWriter.WriteLine("-----------------------------------------");
            streamWriter.Write("Total: $" + total);
            streamWriter.Close();
            
            printDoc.Print();
        }

        public void printPage(object sender, System.Drawing.Printing.PrintPageEventArgs printArgs)
        {
            StreamReader streamReader = new StreamReader("tempOrder.txt");
            string printString = "";

            while(streamReader.Peek() != -1)
            {
                printString += (char)streamReader.Read();
            }
            streamReader.Close();

            printArgs.Graphics.DrawString(printString, new Font("Arial", 10), Brushes.Black, new PointF(0, 0));
        }

        public void exit(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("Good Bye!");
        }

        public void showForm()
        {
            this.Show();
        }

        public void assignDataHolders()
        {
            customerInfo = new CustomerInfo();
            orderInfo = new OrderInfo();

            customerInfoForm.setCustomerInfo(customerInfo);
            orderEntryForm.setOrderInfo(orderInfo);
            finalOrderForm.sendData(customerInfo, orderInfo);
        }  

        private void begin(object sender, EventArgs e)
        {
            this.Hide();
            customerInfoForm.reset();
            customerInfoForm.Show();
        }

        public void modifyPizza(string pizzaString)
        {
            orderEntryForm.pizzaForm.rebuildPizzaString(pizzaString);
            orderEntryForm.pizzaForm.Show();
        }

        private void login_reprintButton_Click(object sender, EventArgs e)
        {
            printDoc.Print();
        }        
    }
}
