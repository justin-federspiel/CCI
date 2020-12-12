using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrescendoCollectiveInterviewAPI
{
    public class ReviewResult
    {
        public ResultReviewer Reviewer { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public BusinessLocation Business_location { get; set; }
    }
}
