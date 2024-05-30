using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Telemedicine.Viewmodels;

namespace Telemedicine.Models
{
    public class PatientModel
    {

        private readonly ConnectionModel connectionModel;

        public PatientModel()
        {
            connectionModel = new ConnectionModel();
        }

        public List<PatientViewModel> GetPatients()
        {
            List<PatientViewModel> patients = new List<PatientViewModel>();

            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT VCHPATIENTID, VCHPATIENTNAME, VCHPATIENTAGE, VCHPATIENTGENDER, VCHPATIENTPHONE, DUPDATE FROM PATIENT";

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PatientViewModel patient = new PatientViewModel
                                {
                                    PatientId = reader["VCHPATIENTID"].ToString(),
                                    PatientName = reader["VCHPATIENTNAME"].ToString(),
                                    PatientAge = reader["VCHPATIENTAGE"].ToString(),
                                    PatientGender = reader["VCHPATIENTGENDER"].ToString(),
                                    PatientPhone = reader["VCHPATIENTPHONE"].ToString(),
                                    UpdateDate = Convert.ToDateTime(reader["DUPDATE"])
                                };

                                patients.Add(patient);
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



            return patients;
        }


        public void ImportPatients(List<PatientViewModel> patients)
        {


            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    var now = DateTime.Now;

                    foreach (var p in patients)
                    {
                        var commandText = @"
                    INSERT INTO PATIENT (
                        VCHPATIENTID, VCHPATIENTNAME, 
                        VCHPATIENTAGE, VCHPATIENTGENDER, 
                        VCHPATIENTPHONE, DUPDATE) 
                    VALUES (
                        :patientId, :patientName, 
                        :patientAge, :patientGender, 
                        :patientPhone, :updateDate)";

                        using (var command = new OracleCommand(commandText, connection))
                        {

                            command.BindByName = true; // 使用名稱綁定
                            command.Parameters.Add(new OracleParameter("patientId", p.PatientId));
                            command.Parameters.Add(new OracleParameter("patientName", p.PatientName));
                            command.Parameters.Add(new OracleParameter("patientAge", p.PatientAge));
                            command.Parameters.Add(new OracleParameter("patientGender", p.PatientGender));
                            command.Parameters.Add(new OracleParameter("patientPhone", p.PatientPhone));
                            command.Parameters.Add(new OracleParameter("updateDate", now));

                            command.ExecuteNonQuery();
                        }
                    }
                }

            }
            catch (OracleException ex)
            {
                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                // 您可以返回空列表，抛出異常，或紀錄錯誤

                throw new ApplicationException(ex.Message); // 重新抛出異常
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("General Error: " + ex.Message);
                // 一般異常處理
                throw new ApplicationException("An error occurred while retrieving records", ex); // 重新抛出異常
            }


        }



    }
}