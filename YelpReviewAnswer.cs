using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrescendoCollectiveInterviewAPI
{
    public class YelpReviewAnswer
    {
        public int Total { get; set; }
        public string[] Possible_languages { get; set; }
        public YelpReview[] Reviews { get; set; }
    }
}
