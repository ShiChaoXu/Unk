using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unk.Core.ViewModel
{
    public class NewsViewModels
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreateTime { get; set; }
    }

    public class UserViewModels
    {
        public string p_UserName { get; set; }
        public string p_UserPwd { get; set; }
        public string p_MoneyAddress { get; set; }
    }

    public class TransferMoney
    {
        public string p_Type { get; set; }
        public string p_TargetUserID { get; set; }
        public string p_TargetUserName { get; set; }
        public string p_TargetUserPhone { get; set; }
        public decimal p_TransferMoney { get; set; }
        public int p_FormUserID { get; set; }
        public string p_FormUserPhone { get; set; }
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
        public decimal TotalUNK { get; set; }
        public string ID { get; set; }
    }

    public class TransFormViewModels
    {
        public int UserID { get; set; }
        public string TokenType { get; set; }
        public string ToTokenType { get; set; }
        public decimal InputCount { get; set; }
        public decimal TransCount { get; set; }
    }

}