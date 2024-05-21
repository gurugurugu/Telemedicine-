using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Moq;
using Xunit;
using Oracle.ManagedDataAccess.Client;
using MyProject.Tests.ViewModels;

namespace MyProject.Tests.Models
{
    public class AccountModelTests
    {

        [Fact]
        public void AddUser_ShouldInsertUserIntoDatabase()
        {
            // Arrange
            var mockConnection = new Mock<OracleConnection>("User Id=ITF0063;Password=DJ4KYWYJONL2;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=femhdev-scan.ad.femh.local)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=DBTEST3)))");
            var mockCommand = new Mock<OracleCommand>();


            mockConnection.Setup(m => m.CreateCommand()).Returns(mockCommand.Object);


            var accountModel = new AccountModel();
            var user = new AccountViewModel
            {
                VCHUSERNAME = "testuser",
                VCHPASSWORD = "testpassword",
                VCHROLE = "admin"
            };

            // Act
            accountModel.AddUser(user);

            // Assert
            mockConnection.Verify(m => m.CreateCommand(), Times.Once);
            mockCommand.VerifySet(m => m.CommandText = It.IsAny<string>(), Times.Once);
            mockCommand.Verify(m => m.ExecuteNonQuery(), Times.Once);
        }
    }
}