using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Tests.ViewModels
{
    class AccountViewModelTests
    {
        //Userinfo
        public string VCHUSERID { get; set; } // 用戶 ID
        public string VCHUSERNAME { get; set; } // 用户名
        public string VCHPASSWORD { get; set; } // 加密後的密碼
        public string VCHROLE { get; set; } // 用戶角色
        public DateTime? DCREATEDATE { get; set; } // 用户創建日期
    }
}
