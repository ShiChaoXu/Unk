using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unk.WebApi.ViewModel
{
    

    public class UserViewModels
    {
        public string p_UserName { get; set; }
        public string p_UserPwd { get; set; }
    }


    public class UpdateAccountModels
    {
        public string p_type { get; set; }
        public string p_userid { get; set; }
        public string p_total { get; set; }
        public string p_desc { get; set; }
    }

    public class RegNewUserModels
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string CardName { get; set; }
        public string CardNo { get; set; }
        public string pUserID { get; set; }
    }
}