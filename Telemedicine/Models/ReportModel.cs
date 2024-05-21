using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telemedicine.Viewmodels;
using Xceed.Words.NET;

namespace Telemedicine.Models
{
    public class ReportModel
    {
        private readonly ConnectionModel connectionModel;

        public ReportModel()
        {
            connectionModel = new ConnectionModel();
        }

        public List<ReportViewModel> GetCombinedRecordByRecordId(string recordId)
        {
            List<ReportViewModel> results = new List<ReportViewModel>();

            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT r.VCHRECORDID, r.VCHDIAGNOSIS, r.VCHTREATMENT, r.DVISITDATE, 
                        p.VCHPATIENTID, p.VCHPATIENTNAME, p.VCHPATIENTAGE, p.VCHPATIENTPHONE, p.VCHPATIENTGENDER,
                        d.VCHDOCTORID, d.VCHDOCTORNAME, d.VCHSPECIALTY FROM RECORD r INNER JOIN PATIENT p ON  r.VCHPATIENTID = p.VCHPATIENTID
                        INNER JOIN  DOCTOR d ON r.VCHDOCTORID = d.VCHDOCTORID WHERE r.VCHRECORDID = :recordId";

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("recordId", recordId));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReportViewModel result = new ReportViewModel
                                {

                                    RecordId = reader["VCHRECORDID"].ToString(),
                                    Diagnosis = reader["VCHDIAGNOSIS"].ToString(),
                                    Treatment = reader["VCHTREATMENT"].ToString(),
                                    VisitDate = Convert.ToDateTime(reader["DVISITDATE"]),
                                    PatientId = reader["VCHPATIENTID"].ToString(),
                                    PatientName = reader["VCHPATIENTNAME"].ToString(),
                                    PatientAge = reader["VCHPATIENTAGE"].ToString(),
                                    PatientPhone = reader["VCHPATIENTPHONE"].ToString(),
                                    PatientGender = reader["VCHPATIENTGender"].ToString(),
                                    DoctorId = reader["VCHDOCTORID"].ToString(),
                                    DoctorName = reader["VCHDOCTORNAME"].ToString(),
                                    Specialty = reader["VCHSPECIALTY"].ToString()


                                };
                                results.Add(result);

                            }
                        }
                    }

                }

            }

            catch (OracleException ex)
            {
                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                // 您可以返回空列表，抛出異常，或記錄錯誤
                throw new ApplicationException("Error querying records", ex); // 重新抛出異常
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("General Error: " + ex.Message);
                // 一般異常處理
                throw new ApplicationException("An error occurred while retrieving records", ex); // 重新抛出異常
            }

            return results;

        }



            public static void GenerateWordReport(ReportViewModel record, string outputPath, string templatePath)
            {
                {
                    try
                    {



                        //載入模板
                        using (DocX doc = DocX.Load(templatePath))
                        {

                            Dictionary<string, string> replacements = new Dictionary<string, string>
                            {
                                { "RecordId", record.RecordId },
                                { "Diagnosis", record.Diagnosis },
                                { "Treatment", record.Treatment },
                                { "VisitDate", record.VisitDate.ToString("yyyy-MM-dd") },
                                { "PatientId", record.PatientId },
                                { "PatientName", record.PatientName },
                                { "PatientAge", record.PatientAge },
                                { "PatientPhone", record.PatientPhone },
                                { "PatientGender", record.PatientGender },
                                { "DoctorId", record.DoctorId },
                                { "DoctorName", record.DoctorName },
                                { "Specialty", record.Specialty },
                            };

                            foreach (KeyValuePair<string, string> item in replacements)
                            {
                                doc.ReplaceText("{" + item.Key + "}", item.Value);  // 替換占位符
                            }

                            doc.SaveAs(outputPath);  // 儲存最終報告
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("生成 Word 報告時發生錯誤: " + ex.Message);
                    }
                }

            }


    }
}