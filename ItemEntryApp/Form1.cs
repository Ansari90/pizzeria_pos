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

namespace ItemEntryApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void updateProductList(object sender, EventArgs e)
        {
            item_itemDisplayBox.Items.Clear();

            string fileName = (string)item_categoryComboBox.SelectedItem + ".txt";
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }
            StreamReader streamReader = new StreamReader(fileName);

            string tempString;
            int midIndex;
            while (streamReader.Peek() != -1)
            {
                tempString = streamReader.ReadLine();
                midIndex = tempString.IndexOf(':');
                item_itemDisplayBox.Items.Add(tempString.Substring(0, midIndex) + "\t" + tempString.Substring(midIndex + 1));
            }

            streamReader.Close();
        }

        public void deleteProduct(object sender, EventArgs e)
        {
            if (item_itemDisplayBox.SelectedIndex >= 0)
            {
                string fileName = (string)item_categoryComboBox.SelectedItem + ".txt";
                List<String> tempDataList = new List<String>();

                item_itemDisplayBox.Items.RemoveAt(item_itemDisplayBox.SelectedIndex);
                int midIndex;
                foreach (string s in item_itemDisplayBox.Items)
                {
                    midIndex = s.IndexOf('\t');
                    tempDataList.Add(s.Substring(0, midIndex) + ":" + s.Substring(midIndex + 1));
                }
                File.Delete(fileName);

                writeToFile(fileName, tempDataList);
            }            
        }

        public void updateProduct(object sender, EventArgs e)
        {
            if(item_itemDisplayBox.SelectedIndex >= 0)
            {
                item_itemDisplayBox.Items[item_itemDisplayBox.SelectedIndex] = item_nameInput.Text + "\t" + item_priceInput.Text;
                List<String> tempDataList = new List<String>();
                int midIndex;

                foreach (string s in item_itemDisplayBox.Items)
                {
                    midIndex = s.IndexOf('\t');
                    tempDataList.Add(s.Substring(0, midIndex) + ":" + s.Substring(midIndex + 1));
                }
                File.Delete((string)item_categoryComboBox.SelectedItem + ".txt");
                writeToFile((string)item_categoryComboBox.SelectedItem + ".txt", tempDataList);
            }
        }

        public void addProduct(object sender, EventArgs e)
        {
            if(item_categoryComboBox.SelectedIndex >= 0)
            {
                List<String> tempDataList = new List<String>();
                tempDataList.Add(item_nameInput.Text + ":" + item_priceInput.Text);
                item_itemDisplayBox.Items.Add((string)item_nameInput.Text + "\t" + item_priceInput.Text);
                writeToFile((string)item_categoryComboBox.SelectedItem + ".txt", tempDataList);
            }
        }

        public void writeToFile(string fileName, List<String> tempDataList)
        {
            StreamWriter streamWriter = new StreamWriter(fileName, true);
            foreach (String s in tempDataList)
            {
                streamWriter.WriteLine(s);
            }
            streamWriter.Close();
        }

        public void updateNamePriceFields(object sender, EventArgs e)
        {
            if (item_itemDisplayBox.SelectedIndex >= 0)
            {
                string selectedItem = (string)item_itemDisplayBox.Items[item_itemDisplayBox.SelectedIndex];
                int midIndex = selectedItem.IndexOf('\t');
                item_nameInput.Text = selectedItem.Substring(0, midIndex);
                item_priceInput.Text = selectedItem.Substring(midIndex + 1);
            }
        }
    }
}
