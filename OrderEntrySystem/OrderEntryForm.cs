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
    public partial class OrderEntryForm : Form
    {
        public delegate void GoBack(Form prevForm, string toDo);
        public GoBack goBack;

        OrderInfo orderInfo;
        public PizzaForm pizzaForm;

        private int categoryNum;
        private int pageNum;

        private List<Dictionary<string, float>> productList;
        private List<Button> productBtnList;
        private List<Button> categoryBtnList;

        public OrderEntryForm()
        {
            InitializeComponent();

            categoryNum = 0;
            pageNum = 0;
            
            productList = new List<Dictionary<string, float>>();
            productList.Add(getFilledList("Wings.txt"));
            productList.Add(getFilledList("Bread.txt"));
            productList.Add(getFilledList("Drinks.txt"));
            productList.Add(getFilledList("Condiments.txt"));

            createNewPizza();

            generateButtonGroups();
            populateMenu();
        }

        public void createNewPizza()
        {
            pizzaForm = new PizzaForm();
            pizzaForm.goBack = showForm;
        }

        public void showForm()
        {
            this.Show();
        }

        public void setOrderInfo(OrderInfo orderInfo)
        {
            this.orderInfo = orderInfo;
        }

        public void generateButtonGroups()
        {
            productBtnList = new List<Button>();
            productBtnList.Add(order_productBtn1);
            productBtnList.Add(order_productBtn2);
            productBtnList.Add(order_productBtn3);
            productBtnList.Add(order_productBtn4);
            productBtnList.Add(order_productBtn5);
            productBtnList.Add(order_productBtn6);
            productBtnList.Add(order_productBtn7);
            productBtnList.Add(order_productBtn8);
            productBtnList.Add(order_productBtn9);

            foreach(Button b in productBtnList)
            {
                b.Click += new EventHandler(displayProductInfo);
            }

            categoryBtnList = new List<Button>();
            categoryBtnList.Add(order_categoryBtn2);
            categoryBtnList.Add(order_categoryBtn3);
            categoryBtnList.Add(order_categoryBtn4);
            categoryBtnList.Add(order_categoryBtn5);

            foreach (Button b in categoryBtnList)
            {
                b.Click += new EventHandler(updateProducts);
            }
        }

        public Dictionary<string, float> getFilledList(string fileName)
        {
            string tempString;
            int midIndex;
            Dictionary<string, float> tempDict = new Dictionary<string, float>();
            StreamReader reader = new StreamReader(fileName);

            while (reader.Peek() != -1)
            {
                tempString = reader.ReadLine();
                midIndex = tempString.IndexOf(':');
                tempDict.Add(tempString.Substring(0, midIndex), float.Parse(tempString.Substring(midIndex + 1)));
            }

            reader.Close();
            return tempDict;
        }

        public void populateMenu() 
        {
            for(int i = 0; i < 9 && (i < productList[categoryNum].Count - pageNum * 9); i++)
            {
                productBtnList[i].Enabled = true;
                productBtnList[i].Visible = true;
                productBtnList[i].Text = productList[categoryNum].Keys.ElementAt((9 * pageNum) + i);
            }

            if (productList[categoryNum].Count - ((pageNum + 1) * 9) > 0)
            {
                order_nextBtn.Enabled = true;
            }
            else
            {
                order_nextBtn.Enabled = false;
            }

            if (pageNum > 0)
            {
                order_prevBtn.Enabled = true;
            }
            else
            {
                order_prevBtn.Enabled = false;
            }
        }        

        private void order_reviewBtn_Click(object sender, EventArgs e)
        {
            goBack(this, "next");
        }

        private void order_addBtn_Click(object sender, EventArgs e)
        {
            orderInfo.productNames.Add(order_infoLabel.Text + (order_infoLabel.Text.Length < 10 ? "\t" : ""));
            orderInfo.productQuantities.Add((int)order_quantityInput.Value);
            orderInfo.productPrices.Add(double.Parse(order_priceLabel.Text));
            orderInfo.priceModifiers.Add(0.0);
        }

        public void displayProductInfo(object sender, EventArgs e)
        {
            order_infoLabel.Text = ((Button)sender).Text;
            order_priceLabel.Text = "" + productList[categoryNum][((Button)sender).Text];

            order_quantityInput.Value = 1;
            quantity_Changed(null, null);
        }

        public void updateProducts(object sender, EventArgs e)
        {
            categoryNum = categoryBtnList.IndexOf((Button)sender);
            pageNum = 0;

            clearProductBtns();
            populateMenu();
        }

        public void clearProductBtns()
        {
            for(int i = 0; i < productBtnList.Count; i++)
            {
                productBtnList[i].Text = "";
                productBtnList[i].Visible = false;
            }
        }

        private void quantity_Changed(object sender, EventArgs e)
        {
            order_productTotalLabel.Text = "Total     $" + double.Parse(order_priceLabel.Text) * (double)order_quantityInput.Value;
        }

        private void order_nextBtn_Click(object sender, EventArgs e)
        {
            pageNum++;
            clearProductBtns();
            populateMenu();
        }

        private void order_prevBtn_Click(object sender, EventArgs e)
        {
            pageNum--;
            clearProductBtns();
            populateMenu();
        }

        private void order_categoryBtn1_Click(object sender, EventArgs e)
        {
            if(pizzaForm.IsDisposed)
            {
                createNewPizza();
            }
            pizzaForm.Show();
            pizzaForm.setOrderInfo(orderInfo);
            pizzaForm.createNewPizza();
            this.Hide();
        }

        public void formClose(object sender, FormClosedEventArgs e)
        {
            goBack(this, "end");
        }

        private void order_backBtn_Click(object sender, EventArgs e)
        {
            goBack(this, "previous");
        }
    }
}