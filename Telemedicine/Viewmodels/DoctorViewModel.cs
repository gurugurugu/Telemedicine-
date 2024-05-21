using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Telemedicine.Viewmodels
{
        public class DoctorViewModel
        {
            public string DoctorId { get; set; }
            public string DoctorName { get; set; }
            public string Specialty { get; set; }
            public string SecName { get; set; }
        }
}