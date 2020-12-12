using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrescendoCollectiveInterviewAPI
{
    public class YelpReview
    {
        public string Id { get; set; }
        public int Rating { get; set; }
        public YelpUser User { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public string Time_created { get; set; }
    }
}
