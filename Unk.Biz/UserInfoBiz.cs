using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Unk.Core.ViewModel;

namespace Unk.Biz
{
    public class UserInfoBiz
    {
        public List<Entity.UserInfoEntity> GetUserList()
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM UserInfo").ToList();
            }
        }

        public List<Entity.UserInfoDetails> GetUserListDetails() {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoDetails>($@"
select 
a.*,
(select UserName from UserInfo where UserPhone =a.Referrer ) as ReferrerName,
ISNULL((select SUM(CurrentIcon) from TokenDetails where TokenType = 'UNK' and [Stauts] = 1 and  UserID = a.id),0) as TotalUNK
from UserInfo as a
").ToList();
            }
        }

        public Entity.UserInfoDetails GetUserListDetailsByID(int userid) {

            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                Entity.UserInfoDetails userInfoDetails = conn.Query<Entity.UserInfoDetails>($@"
select 
a.*,
(select UserName from UserInfo where UserPhone =a.Referrer ) as ReferrerName,
ISNULL((select SUM(CurrentIcon) from TokenDetails where TokenType = 'UNK' and [Stauts] =1 and UserID = a.id),0) as TotalUNK
from UserInfo as a
where a.id = {userid}
").FirstOrDefault();
                return userInfoDetails;
            }
        }

        public List<Entity.UserInfoEntity> GetMyTeam(string userPhone)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM UserInfo where Referrer = '{userPhone}' order by CreateTime desc").ToList();
            }
        }

        public List<Entity.UserInfoDetails> GetUserListDetailsByDashBoard(string key)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoDetails>($@"
select 
a.*,
(select UserName from UserInfo where UserPhone =a.Referrer ) as ReferrerName,
ISNULL((select SUM(CurrentIcon) from TokenDetails where TokenType = 'UNK' and [Stauts] = 1 and UserID = a.id),0) as TotalUNK
from UserInfo as a
where a.UserName like N'%{key}%' or a.UserPhone like '%{key}%'
").ToList();
            }
        }



        public Entity.UserInfoEntity GetUserSingle(string p_id) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM UserInfo WHERE id = {p_id}").FirstOrDefault();
            }
        }
        public Entity.UserInfoEntity GetUserByPhone(string p_phone)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM UserInfo WHERE UserPhone = '{p_phone}'").FirstOrDefault();
            }
        }
        public Entity.UserInfoEntity UserLogin(string p_User, string p_Pwd)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM [UserInfo] where UserPhone = '{p_User}' and UserPwd = '{p_Pwd}'").FirstOrDefault();
            }
        }

        public bool UpdateUserIDInfo(Entity.UserInfoEntity userInfo) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                if (conn.Query<int>($@"select count(id) from UserInfo where IDName = N'{userInfo.IDName}' and IDCard = '{userInfo.IDCard}'").FirstOrDefault() > 0)
                {
                    return false;
                }
                return conn.Execute($@"UPDATE UserInfo SET  UserName = N'{userInfo.IDName}',IDName = N'{userInfo.IDName}', IDCard = '{userInfo.IDCard}' where id= {userInfo.ID}") > 0;
            }
        }

        public bool UpdateUserKeyWords(Entity.UserInfoEntity userInfo) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Execute($@"UPDATE UserInfo SET OnlyKeyword = '{userInfo.OnlyKeyword}',PayAddress = '{userInfo.PayAddress}' where id = {userInfo.ID}") > 0;
            }
        }

        public bool UpdateUserPwd(Entity.UserInfoEntity userInfo, bool IsPay = false) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                string sql = string.Empty;
                if (IsPay)
                {
                    sql = $"UPDATE UserInfo SET PayPassWord = '{userInfo.PayPassWord}' where id = {userInfo.ID}";
                }
                else {
                    sql = $"UPDATE UserInfo SET UserPwd = '{userInfo.UserPwd}' where id = {userInfo.ID}";
                }
                return conn.Execute(sql) > 0;
            }
        }


        public Entity.UserInfoEntity GetUserByName(string p_user)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM [UserInfo] where UserName = N'{p_user}'").FirstOrDefault();
            }
        }

        public bool RegUser(RegNewUserModels model)
        {
            var v_RefUser = GetUserSingle(model.pUserID);
            string CurrentID = string.Empty;
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                conn.Execute($@"
INSERT INTO [dbo].[UserInfo]
           ([UserName]
           ,[UserPhone]
           ,[UserPwd]
           ,[UserSex]
           ,[IDCard]
           ,[IDName]
           ,[IDBirthday]
           ,[Referrer]
           ,[Status]
)
     VALUES
           (
            '{model.CardName}'
           ,'{model.UserName}'
           ,'{model.UserPwd}'
           ,0
           ,'{model.CardNo}'
           ,'{model.CardName}'
           ,''
           ,'{v_RefUser.UserPhone}'
           ,1
           )

");
                CurrentID = conn.Query<string>($@"SELECT ID FROM UserInfo WHERE UserPhone = '{model.UserName}'").FirstOrDefault();
            }

            return true;
        }

        public bool DeleteUserData(string p_UserID) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                conn.Execute($"DELETE FROM UserInfo where ID = {p_UserID}");
                conn.Execute($"DELETE FROM TokenDetails where UserID = {p_UserID}");
            }
            return true;
        }

        public bool UpdateAccountAndAsset(RegNewUserModels pview)
        {
            /*
             *   1/ 先更新  姓名,手机,登录密码 
             *   2/ 再更新  相关联的手机号(推荐人手机号)
             *   3/ UNK算差值,并做更新
             */
            var v_UserInfo = GetUserSingle(pview.ID);

            var v_Current = new GainsInHistoryBiz().GetTokenCount("UNK", pview.ID);

            long v_PTotalUNK = (long)pview.TotalUNK;

            var v_Remaning = v_PTotalUNK - v_Current;

            bool IsGreaterThan = v_Remaning > 0 ? true : false;

            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                conn.Execute($@"
UPDATE Userinfo set UserName = N'{pview.CardName}', UserPhone = '{pview.UserName}', UserPwd = '{pview.UserPwd}'
    WHERE id  = {pview.ID}
");
                conn.Execute($@"
UPDATE Userinfo set Referrer = '{pview.UserName}' where Referrer = '{v_UserInfo.UserPhone}'
");
            }
            string v_desc = string.Empty;
            if (IsGreaterThan)
            {
                v_desc = $"新增 {v_Remaning} 份";
            }
            else
            {
                v_desc = $"减少 {v_Remaning} 份";
            }
            if (v_Remaning != 0)
            {
                new GainsInHistoryBiz().UpdateAccountCoin("UNK", pview.ID, v_Remaning.ToString(), v_desc);
            }
            return true;
        }

        public Entity.UserInfoEntity GetUserInfoByAddress(string p_MoneyAddress)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT TOP(1) * FROM UserInfo WHERE PayAddress = '{p_MoneyAddress}' ").FirstOrDefault();
            }
        }

        //News Controller.

        public bool InsertOrUpdateNews(Entity.NewsEntity entity)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                string sql = string.Empty;
                if (entity.ID == 0)
                {
                    sql = $@"
INSERT INTO News(Title,Description,CreateTime,[Status]) VALUES(
@Title,
@Description,
@CreateTime,
@Status
)
";
                }
                else {
                    sql = $@"
UPDATE News set Title = @Title, Description = @Description  WHERE ID = @ID
";
                }
                conn.Execute(sql, entity);
            }
            return true;
        }

        public List<NewsViewModels> GetNewsTitle()
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<NewsViewModels>($@"SELECT id,Title,CreateTime FROM News Order by ID desc").ToList();
            }
        }
        public Entity.NewsEntity GetNewsInfo(string id) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.NewsEntity>($"SELECT * FROM News where ID = {id}").FirstOrDefault();
            }
        }

        public bool DelNews(string id) {

            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                conn.Execute($@"DELETE FROM News where ID = {id}");
                return true;
            }
        }

        public static string GetTokenName(string name) {
            switch (name)
            {
                default:
                    return name;
                case "UTC":
                    return "VBX";
            }
        }

        public Tuple<bool, string> TransformToken(TransFormViewModels models)
        {
            bool results = true;
            string message = string.Empty;
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                decimal total = conn.Query<decimal>($"SELECT SUM(CurrentIcon)  from dbo.TokenDetails where UserID = {models.UserID} and TokenType = '{GetTokenName(models.TokenType)}'").FirstOrDefault();
                if (total - models.InputCount < 0)
                {
                    results = false;
                    message = "您输入的数量大于您的持有量，请重新输入";
                }
                if (results)
                {
                    conn.Execute($@"INSERT INTO [dbo].[TokenDetails]
           ([UserID]
           ,[TokenType]
           ,[CurrentIcon]
           ,[CurrentDescription]
           ,[Stauts]
           ,[CreateTime])
     VALUES
           ({models.UserID}
           ,'{GetTokenName(models.TokenType)}'
           ,{models.InputCount * -1}
           ,'通证转换，扣除{Math.Round(models.InputCount,4)}'
           ,1
           ,'{DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss")}')");

                    conn.Execute($@"INSERT INTO [dbo].[TokenDetails]
           ([UserID]
           ,[TokenType]
           ,[CurrentIcon]
           ,[CurrentDescription]
           ,[Stauts]
           ,[CreateTime])
     VALUES
           ({models.UserID}
           ,'{GetTokenName(models.ToTokenType)}'
           ,{models.TransCount}
           ,'通证转换，增加{Math.Round(models.TransCount, 4)}'
           ,1
           ,'{DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss")}')");
                }
            }
            return new Tuple<bool, string>(results, message);
        }

       
    }
}
