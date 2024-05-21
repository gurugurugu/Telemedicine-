using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telemedicine.Viewmodels;

namespace Telemedicine.Models
{
    public class DoctorModel
    {
        private readonly ConnectionModel connectionModel;
        public DoctorModel()
        {
            connectionModel = new ConnectionModel();
        }

        public List<DoctorViewModel> GetDoctors()
        {
            List<DoctorViewModel> doctors = new List<DoctorViewModel>();

            string connectionString = connectionModel.DBTEST3con();

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT VCHDOCTORID, VCHDOCTORNAME, VCHSPECIALTY, VCHSECNAME FROM DOCTOR"; // SQL 查询

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DoctorViewModel doctor = new DoctorViewModel
                                {
                                    DoctorId = reader["VCHDOCTORID"].ToString(),
                                    DoctorName = reader["VCHDOCTORNAME"].ToString(),
                                    Specialty = reader["VCHSPECIALTY"].ToString(),
                                    SecName = reader["VCHSECNAME"].ToString()
                                };

                                doctors.Add(doctor); // 添加到列表
                            }
                        }
                    }
                }

            }

            catch (OracleException ex)
            {
                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                // 您可以返回空列表，抛出異常，或紀錄錯誤
                throw new ApplicationException("Error querying records", ex); // 重新抛出異常
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("General Error: " + ex.Message);
                // 一般異常處理
                throw new ApplicationException("An error occurred while retrieving records", ex); // 重新抛出異常
            }



            return doctors; // 返回醫生列表
        }

    }
}