using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrescendoCollectiveInterviewAPI
{
    public class YelpBusiness
    {
        public object[] Categories { get; set; }
        public object Coordinates { get; set; }
        public string Display_phone { get; set; }
        public object[] Hours { get; set; }
        public string Id { get; set; }
        public string Alias { get; set; }
        public string Image_url { get; set; }
        public bool Is_claimed { get; set; }
        public bool Is_closed { get; set; }
        public BusinessLocation Location { get; set; }
        public object Messaging { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public decimal Rating { get; set; }
        public int Review_count { get; set; }
        public string Url { get; set; }
        public string[] Transactions { get; set; }
        public object[] Special_hours { get; set; }
        public object attributes { get; set; }
    }
}
