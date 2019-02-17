using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unk.Biz.Entity
{
    public class UserInfoEntity : Entity
    {
        public string UserName { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string UserPwd { get; set; } = string.Empty;
        public int UserSex { get; set; } = 0;
        public string IDCard { get; set; } = string.Empty;
        public string IDName { get; set; } = string.Empty;
        public DateTime IDBirthday { get; set; } = new DateTime();
        public string Referrer { get; set; } = string.Empty;
    }

    public class UserInfoDetails : UserInfoEntity
    {
        public string ReferrerName { get; set; }
        public string TotalUNK { get; set; }
    }
}
