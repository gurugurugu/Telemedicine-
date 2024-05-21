using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Telemedicine.Viewmodels
{
    public class ReportViewModel
    {
        // 病歷相關屬性
        public string RecordId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime VisitDate { get; set; }

        // 病人相關屬性
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientAge { get; set; }
        public string PatientPhone { get; set; }

        //20240508新添加
        public string PatientGender { get; set; }

        // 醫生相關屬性
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
    }
}
