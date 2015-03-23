using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHolders
{
    //Order items, their subcategories and price modifiers increase/decrease in tandem
    [Serializable]
    public class OrderInfo
    {
        public List<string> productNames;
        public List<int> productQuantities;
        public List<double> productPrices;
        public List<double> priceModifiers;

        public OrderInfo()
        {
            productNames = new List<string>();
            productQuantities = new List<int>();
            productPrices = new List<double>();
            priceModifiers = new List<double>();
        }

        public void removeAtIndex(int index, bool deleteMoneyInfo)
        {
            productNames.RemoveAt(index);
            productQuantities.RemoveAt(index);

            if (deleteMoneyInfo == true)
            {
                productPrices.RemoveAt(index);
                priceModifiers.RemoveAt(index);
            }
        }
    }
}
