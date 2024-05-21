using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Telemedicine.Viewmodels
{
    public class PatientViewModel
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientAge { get; set; }
        public string PatientGender { get; set; }
        public string PatientPhone { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}