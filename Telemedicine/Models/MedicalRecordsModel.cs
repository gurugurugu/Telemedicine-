using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telemedicine.Viewmodels;

namespace Telemedicine.Models
{
    public class MedicalRecordsModel
    {
        private readonly ConnectionModel connectionModel;


        public MedicalRecordsModel()
        {
            connectionModel = new ConnectionModel();
        }

        public List<MedicalRecordsViewModel> GetMedicalRecords()
        {
            List<MedicalRecordsViewModel> records = new List<MedicalRecordsViewModel>();

            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT VCHRECORDID AS RecordId, VCHPATIENTID AS PatientId, VCHDOCTORID AS DoctorId, VCHDIAGNOSIS AS Diagnosis, VCHTREATMENT AS Treatment, DVISITDATE AS VisitDate, VCHISFINISHED AS IsFinished FROM RECORD";

                    using (OracleCommand command = new OracleCommand(sql, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MedicalRecordsViewModel record = new MedicalRecordsViewModel
                                {
                                    RecordId = reader["RecordId"].ToString(),
                                    PatientId = reader["PatientId"].ToString(),
                                    DoctorId = reader["DoctorId"].ToString(),
                                    Diagnosis = reader["Diagnosis"].ToString(),
                                    Treatment = reader["Treatment"].ToString(),
                                    VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                                    IsFinished = reader["IsFinished"].ToString()
                                };

                                records.Add(record);
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // Oracle 數據庫異常處理
                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                // 可以紀錄日誌、返回特定消息、或執行其他操作
            }
            catch (Exception ex)
            {
                // 捕獲其他異常
                Console.Error.WriteLine("General Error: " + ex.Message);
                // 可以紀錄日誌、返回特定消息、或執行其他操作
            }

            return records; // 返回结果
        }


        public void UpdateMedicalRecord(string recordId, string patientId, string doctorId, string diagnosis, string treatment, DateTime visitDate, string isFinished)
        {
            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open(); // 開啟連接資料庫

                    // UPDATE 查询


                    string commandText = @"UPDATE RECORD SET VCHPATIENTID = :patientId, VCHDOCTORID = :doctorId, VCHDIAGNOSIS = :diagnosis, VCHTREATMENT = :treatment, DVISITDATE = :visitDate,                        VCHISFINISHED = :isFinished WHERE VCHRECORDID = :recordId";


                    string updatesql = @"UPDATE RECORD SET VCHPATIENTID = :patientId ,     VCHDOCTORID = :doctorId,     VCHDIAGNOSIS = :diagnosis,   VCHTREATMENT = :treatment,   DVISITDATE = :visitDate, VCHISFINISHED = :isFinished           WHERE VCHRECORDID =:recordId ";

                    using (OracleCommand command = new OracleCommand(updatesql, connection))
                    {
                        // 設置參數
                        command.BindByName = true; //SQL 參數绑定是根據參數名稱而不是順序進行
                        command.Parameters.Add(new OracleParameter("patientId", patientId));
                        command.Parameters.Add(new OracleParameter("doctorId", doctorId));
                        command.Parameters.Add(new OracleParameter("diagnosis", diagnosis));
                        command.Parameters.Add(new OracleParameter("treatment", treatment));
                        command.Parameters.Add(new OracleParameter("visitDate", visitDate));
                        command.Parameters.Add(new OracleParameter("isFinished", isFinished));
                        command.Parameters.Add(new OracleParameter("recordId", recordId));




                        command.ExecuteNonQuery(); // 執行SQL


                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating record:", ex); // 抓取異常訊息
            }

        }


        public string CreateMedicalRecord(string patientId, string doctorId, string diagnosis, string treatment, DateTime visitDate, string isFinished)
        {
            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    decimal maxId;

                    // 獲取最大值
                    using (OracleCommand command = new OracleCommand("SELECT MAX(TO_NUMBER(VCHRECORDID)) FROM RECORD WHERE REGEXP_LIKE(VCHRECORDID, '^[0-9]+$')", conn))
                    {
                        object result = command.ExecuteScalar();

                        if (result == DBNull.Value)
                        {
                            maxId = 0; // 初始值
                        }
                        else
                        {
                            maxId = Convert.ToDecimal(result); // 將最大值轉為數值
                        }
                    }

                    decimal uniqueId = 0;
                    bool isUnique = false;

                    // 確保 RecordId 唯一
                    while (!isUnique)
                    {
                        uniqueId = maxId + 1; // 生成新的唯一值

                        using (OracleCommand checkCommand = new OracleCommand("SELECT COUNT(*) FROM RECORD WHERE VCHRECORDID = :id", conn))
                        {
                            checkCommand.Parameters.Add(new OracleParameter("id", uniqueId));
                            int count = (int)(decimal)checkCommand.ExecuteScalar();

                            isUnique = count == 0; // 如果 count 为 0，表示唯一
                        }

                        maxId++; // 更新最大值
                    }

                    string recordid = uniqueId.ToString();

                    // 插入新紀錄
                    string insertQuery = @"
            INSERT INTO RECORD (VCHRECORDID, VCHPATIENTID, VCHDOCTORID, VCHDIAGNOSIS, VCHTREATMENT, DVISITDATE, VCHISFINISHED)
            VALUES (:recordId, :patientId, :doctorId, :diagnosis, :treatment, :visitDate, :isFinished)";

                    using (OracleCommand command = new OracleCommand(insertQuery, conn))
                    {
                        command.BindByName = true;

                        command.Parameters.Add(new OracleParameter("recordId", recordid));
                        command.Parameters.Add(new OracleParameter("patientId", patientId));
                        command.Parameters.Add(new OracleParameter("doctorId", doctorId));
                        command.Parameters.Add(new OracleParameter("diagnosis", diagnosis));
                        command.Parameters.Add(new OracleParameter("treatment", treatment));
                        command.Parameters.Add(new OracleParameter("visitDate", visitDate));
                        command.Parameters.Add(new OracleParameter("isFinished", isFinished));

                        command.ExecuteNonQuery();
                    }

                    return recordid; // 返回唯一的 RecordId
                }
            }
            catch (OracleException ex)
            {

                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public void DeleteMedicalRecord(string recordId)
        {
            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    string deleteQuery = "DELETE FROM RECORD WHERE VCHRECORDID = :recordId";

                    using (OracleCommand command = new OracleCommand(deleteQuery, conn))
                    {
                        command.BindByName = true;
                        command.Parameters.Add(new OracleParameter("recordId", recordId));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OracleException ex)
            {

                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                throw;
            }

        }   

        // 獲取符合的ID列表紀錄
        public List<MedicalRecordsViewModel> GetRecordsByIds(List<string> ids)
        {
            List<MedicalRecordsViewModel> records = new List<MedicalRecordsViewModel>();

            if (ids == null || ids.Count == 0)
                return records; // 如果 ID 列表為空，直接返回

            try
            {
                // 使用 Oracle 參數化查询，防止 SQL 注入
                string inClause = string.Join(",", ids.Select((id, index) => $":id{index}"));

                string query = $"SELECT VCHRECORDID, VCHPATIENTID, VCHDOCTORID, VCHDIAGNOSIS, VCHTREATMENT, DVISITDATE, VCHISFINISHED FROM RECORD WHERE VCHRECORDID IN ({inClause})";

                string connectionString = connectionModel.DBTEST3con();

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open(); 

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        // 綁定參數
                        for (int i = 0; i < ids.Count; i++)
                        {
                            command.Parameters.Add(new OracleParameter($":id{i}", ids[i]));
                        }

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MedicalRecordsViewModel record = new MedicalRecordsViewModel
                                {
                                    RecordId = reader["VCHRECORDID"].ToString(),
                                    PatientId = reader["VCHPATIENTID"].ToString(),
                                    DoctorId = reader["VCHDOCTORID"].ToString(),
                                    Diagnosis = reader["VCHDIAGNOSIS"].ToString(),
                                    Treatment = reader["VCHTREATMENT"].ToString(),
                                    VisitDate = DateTime.Parse(reader["DVISITDATE"].ToString()),
                                    IsFinished = reader["VCHISFINISHED"].ToString()
                                };
                                records.Add(record);
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                // 您可以返回空列表，抛出異常，或記錄錯誤
                throw new ApplicationException("Error querying records", ex); // 重新拋出異常
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("General Error: " + ex.Message);
                // 一般異常處理
                throw new ApplicationException("An error occurred while retrieving records", ex); // 重新拋出異常
            }

            return records; // 返回结果
        }





        public List<string> GetPatientsForDoctor(string doctorName)
        {
            List<string> patientIds = new List<string>();

            string connectionString = connectionModel.DBTEST3con();

            string query = "SELECT RECORD.VCHPATIENTID " +
                           "FROM RECORD " +
                           "INNER JOIN DOCTOR ON RECORD.VCHDOCTORID = DOCTOR.VCHDOCTORID " +
                           "WHERE DOCTOR.VCHDOCTORNAME = :doctorName";


            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("doctorName", doctorName));

                    try
                    {
                        connection.Open();
                        OracleDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            string patientId = reader["VCHPATIENTID"].ToString(); 
                            patientIds.Add(patientId);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        // 處理異常
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
            }

            return patientIds;
        }

        public string GetDoctorId(string doctorName)
        {
            string doctorId = null;

            try
            {
                string connectionString = connectionModel.DBTEST3con();
                string query = "SELECT VCHDOCTORID FROM DOCTOR WHERE VCHDOCTORNAME = :doctorName";

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("doctorName", doctorName));

                        connection.Open();
                        OracleDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            doctorId = reader["VCHDOCTORID"].ToString();
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // 處理異常
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return doctorId;
        }

    }
}