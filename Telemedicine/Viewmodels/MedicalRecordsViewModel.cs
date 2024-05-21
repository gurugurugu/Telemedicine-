using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Telemedicine.Viewmodels
{
    public class MedicalRecordsViewModel
    {
        public string RecordId { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime VisitDate { get; set; }
        public string IsFinished { get; set; }
    }
}