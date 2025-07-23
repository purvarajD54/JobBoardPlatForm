using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobBoardPlatform.Models
{
    public class ApplicationViewModel
    {
        public string CandidateName { get; set; }
        public string JobTitle { get; set; }
        public DateTime AppliedDate { get; set; }
        public string Status { get; set; }


    }
}