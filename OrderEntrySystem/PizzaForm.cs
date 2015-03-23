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
    public partial class PizzaForm : Form
    {
        public class AmountAndSide
        {
            public AmountAndSide()
            {
                side = "W";
                amount = 1;
            }

            public int amount;
            public string side;
        }

        public delegate void GoBack();
        public GoBack goBack;

        OrderInfo orderInfo;

        private Dictionary<RadioButton, String> sizeRadioList;
        private Dictionary<string, string> sizePriceList;        

        private Dictionary<RadioButton, String> crustRadioList;
        private Dictionary<string, string> crustPriceList;

        private Dictionary<String, String> toppings;
        private Dictionary<string, string> toppingPriceList;
        private List<Button> toppingButtonList;

        private List<Button> sideButtonList;

        public Dictionary<String, AmountAndSide> pizzaString = null;

        private string sizeString;
        private string crustString;

        private int pageNum;
        private bool extraSelected;
        private string side = "W";

        public PizzaForm()
        {
            InitializeComponent();

            loadData();
            generateButtonGroups();
            generateButtonLabels();
            pageNum = 0;

            sizeString = "Medium";
            pizza_mediumRadio.Checked = true;
            crustString = "Regular";
            pizza_regularRadio.Checked = true;
            extraSelected = false;
        }

        public void sideSelected(object sender, EventArgs e)
        {
            Button tempButton = (Button)sender;
            foreach(Button b in sideButtonList)
            {
                if(!tempButton.Equals(b))
                {
                    b.BackColor = System.Drawing.Color.Gray; 
                }
                else
                {

                    b.BackColor = System.Drawing.Color.Yellow;
                }
            }
            side = tempButton.Text.ElementAt(0).ToString();
        }

        public void rebuildPizzaString(string pizzaString)
        {
            createNewPizza();
            char[] pizzaArray = pizzaString.Substring(pizzaString.IndexOf(':') + 2).ToCharArray();
            AmountAndSide amountAndSide;
            string sideToAdd = "W";
            int skipCounter = -1, amount = 1;

            foreach(char ch in pizzaArray)
            {
                switch(ch)
                {
                    case '-':
                        sideToAdd = "L";
                        skipCounter = 0;
                        break;
                    case '*':
                        sideToAdd = "R";
                        skipCounter = 0;
                        break;
                }

                if(skipCounter != -1 && skipCounter != 3)
                {
                    skipCounter++;
                    continue;
                }
                else
                {
                    skipCounter = -1;
                }
                
                if(Char.IsDigit(ch))
                {
                    amount = Int32.Parse(ch.ToString());
                }
                else
                {
                    amountAndSide = new AmountAndSide();
                    amountAndSide.amount = amount;
                    amountAndSide.side = sideToAdd;

                    foreach (string s in toppings.Keys)
                    {
                        if (ch.ToString().Equals(toppings[s]))
                        {
                            this.pizzaString.Add(s, amountAndSide);
                        }
                    }
                    amount = 1;
                }
            }

            string tempString = pizzaString.Substring(0, pizzaString.IndexOf('^'));
            foreach(RadioButton r in sizeRadioList.Keys)
            {
                if(sizeRadioList[r].Equals(tempString))
                {
                    r.Checked = true;
                    sizeString = tempString;
                }
            }

            tempString = pizzaString.Substring(pizzaString.IndexOf('^') + 1, pizzaString.IndexOf('_'));
            foreach(RadioButton r in crustRadioList.Keys)
            {
                if(crustRadioList[r].Equals(tempString))
                {
                    r.Checked = true;
                    crustString = tempString;
                }
            }

            setupPizzaForm();
        }

        public void updatePrice(string pizzaString)
        {

        }

        public void setOrderInfo(OrderInfo orderInfo)
        {
            this.orderInfo = orderInfo;
        }

        public void loadData()
        {
            sizePriceList = getFilledList("SizePrice.txt");
            crustPriceList = getFilledList("CrustPrice.txt");
            toppingPriceList = getFilledList("ToppingPrice.txt");
            toppings = getFilledList("Toppings.txt");
        }

        public Dictionary<string, string> getFilledList(string fileName)
        {
            string tempString;
            int midIndex;
            Dictionary<string, string> tempDict = new Dictionary<string, string>();
            StreamReader reader = new StreamReader(fileName);

            while (reader.Peek() != -1)
            {
                tempString = reader.ReadLine();
                midIndex = tempString.IndexOf(':');
                tempDict.Add(tempString.Substring(0, midIndex), tempString.Substring(midIndex + 1));
            }

            reader.Close();
            return tempDict;
        }

        public void generateButtonLabels()
        {
            for (int i = 0; i < 9 && i < toppings.Count - (pageNum * 9); i++)
            {
                toppingButtonList[i].Text = toppings.Keys.ElementAt((9 * pageNum) + i);
                toppingButtonList[i].Visible = true;
            }

            if (toppings.Count - (9 * (pageNum + 1)) > 0)
            {
                pizza_toppingNextBtn.Enabled = true;
            }
            if(pageNum > 0)
            {
                pizza_toppingPrevBtn.Enabled = true;
            }
        }

        public void createNewPizza()
        {
            pizzaString = new Dictionary<string, AmountAndSide>();
        }

        public void generateButtonGroups()
        {
            toppingButtonList = new List<Button>();
            toppingButtonList.Add(pizza_toppingBtn1);
            toppingButtonList.Add(pizza_toppingBtn2);
            toppingButtonList.Add(pizza_toppingBtn3);
            toppingButtonList.Add(pizza_toppingBtn4);
            toppingButtonList.Add(pizza_toppingBtn5);
            toppingButtonList.Add(pizza_toppingBtn6);
            toppingButtonList.Add(pizza_toppingBtn7);
            toppingButtonList.Add(pizza_toppingBtn8);
            toppingButtonList.Add(pizza_toppingBtn9);

            foreach (Button b in toppingButtonList)
            {
                b.BackColor = System.Drawing.Color.Gray;
                b.Click += new EventHandler(onToppingSelected);
                b.Visible = false;
            }

            sizeRadioList = new Dictionary<RadioButton, String>();
            sizeRadioList.Add(pizza_personalRadio, "Personal");
            sizeRadioList.Add(pizza_smallRadio, "Small");
            sizeRadioList.Add(pizza_mediumRadio, "Medium");
            sizeRadioList.Add(pizza_largeRadio, "Large");
            sizeRadioList.Add(pizza_xLargeRadio, "XLarge");
            sizeRadioList.Add(pizza_partyRadio, "Party");

            crustRadioList = new Dictionary<RadioButton, string>();
            crustRadioList.Add(pizza_regularRadio, "Regular");
            crustRadioList.Add(pizza_thinRadio, "Thin");
            crustRadioList.Add(pizza_crunchyRadio, "Crunchy");
            crustRadioList.Add(pizza_stuffedRadio, "Stuffed");

            sideButtonList = new List<Button>();
            sideButtonList.Add(pizza_wholeButton);
            sideButtonList.Add(pizza_rightButton);
            sideButtonList.Add(pizza_leftButton);
        }

        public void resetButtons()
        {
            foreach(Button b in toppingButtonList)
            {
                b.Visible = false;
                b.BackColor = System.Drawing.Color.Gray;                
            }

            pizza_toppingNextBtn.Enabled = false;
            pizza_toppingPrevBtn.Enabled = false;
        }

        public void onToppingSelected(object sender, EventArgs e)
        {
            if (pizzaString == null)
            {
                createNewPizza();
            }

            Button tempButton = ((Button)sender);

            if(tempButton.BackColor == System.Drawing.Color.Yellow)
            {
                tempButton.BackColor = System.Drawing.Color.Gray;
                pizzaString[tempButton.Text].amount = 0;
            }
            else
            {
                tempButton.BackColor = System.Drawing.Color.Yellow;
                if (!pizzaString.Keys.Contains(tempButton.Text))
                {
                    pizzaString.Add(tempButton.Text, new AmountAndSide());
                }
                int amountToAdd = 1;
                if(extraSelected)
                {
                    unSelectExtra();
                    amountToAdd = 2;
                }
                if (side != "W")
                {
                    pizzaString[tempButton.Text].side = side;
                    sideSelected(pizza_wholeButton, null);
                }
                pizzaString[tempButton.Text].amount = amountToAdd;
            }
        }

        public void setupPizzaForm()
        {
            resetButtons();
            generateButtonLabels();
            colorAppropriate();
        }

        private void order_prevBtn_Click(object sender, EventArgs e)
        {
            pageNum--;
            setupPizzaForm();
        }

        private void pizza_toppingNextBtn_Click(object sender, EventArgs e)
        {
            pageNum++;
            setupPizzaForm();
        }

        private void colorAppropriate()
        {
            foreach (Button b in toppingButtonList)
            {
                if (pizzaString.Keys.Contains(b.Text) && (pizzaString[b.Text].amount > 0))
                {
                    b.BackColor = System.Drawing.Color.Yellow;
                }
            }
        }

        private void pizza_backBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            pizzaString = null;
            goBack();
        }

        private void pizza_addBtn_Click(object sender, EventArgs e)
        {
            foreach(RadioButton r in sizeRadioList.Keys)
            {
                if(r.Checked)
                {
                    sizeString = sizeRadioList[r];
                }
            }

            foreach(RadioButton r in crustRadioList.Keys)
            {
                if(r.Checked)
                {
                    crustString = crustRadioList[r];
                }
            }

            string toppingString = "Pizza: ";
            string lString = "L:", rString = "R:", tempString = "";
            string tempAmount;
            foreach(string topping in toppings.Keys)
            {
                if(pizzaString.Keys.Contains(topping) && pizzaString[topping].amount > 0)
                {
                    tempAmount = pizzaString[topping].amount > 1 ? "2" : "";
                    switch(pizzaString[topping].side)
                    {
                        case "W":
                            tempString += tempAmount + toppings[topping];
                            break;
                        case "L":
                            lString += tempAmount + toppings[topping];
                            break;
                        case "R":
                            rString += tempAmount + toppings[topping];
                            break;
                    }       
                }
            }
            toppingString += tempString + "-" + lString + "*" + rString;
            orderInfo.productNames.Add(sizeString + "^" + crustString + "_" + toppingString);
            orderInfo.productQuantities.Add(1);
            double price = 0.0;
            foreach(AmountAndSide s in pizzaString.Values)
            {
                price += s.amount * (s.side.Equals("W") ? 1 : 0.5) * double.Parse(toppingPriceList[sizeString]);
            }
            orderInfo.productPrices.Add(double.Parse(sizePriceList[sizeString]) + double.Parse(crustPriceList[crustString]) + price);
            orderInfo.priceModifiers.Add((double)0);
            
            resetButtons();
            generateButtonLabels();

            pizza_backBtn_Click(null, null);
        }

        private void PizzaForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            goBack();
        }

        private void pizza_extraButton_Click(object sender, EventArgs e)
        {
            extraSelected = true;
            ((Button)sender).BackColor = System.Drawing.Color.Yellow;
        }

        private void unSelectExtra()
        {
            extraSelected = false;
            pizza_extraButton.BackColor = System.Drawing.Color.Gray;
        }
    }
}
