using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrescendoCollectiveInterviewAPI
{
    public class BusinessLocation
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Cross_streets { get; set; }
        public string[] Display_address { get; set; }
        public string State { get; set; }
        public string Zip_code { get; set; }        
    }
}
