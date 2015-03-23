using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHolders
{
    public class CustomerInfo
    {
        public string customerName;
        public string emailAddress;
        public string primaryPhone;
        public string secondaryPhone;

        public string unitNum;
        public string street;
        public string city;
        public string state;
        public string postalCode;

        public string notes;
        public string orderType;

        public void fillWithEmptyStrings()
        {
            customerName = "";
            emailAddress = "";
            primaryPhone = "";
            secondaryPhone = "";
            unitNum = "";
            street = "";
            city = "";
            state = "";
            postalCode = "";
            notes = "";
            orderType = "";
        }
    }
}
