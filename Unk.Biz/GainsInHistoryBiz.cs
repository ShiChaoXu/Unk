using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unk.Biz.Entity;
using Dapper;
using System.Data.SqlClient;
using Unk.Core.ViewModel;

namespace Unk.Biz
{
    public class GainsInHistoryBiz
    {
        public List<EveryDataViewGainsEntity> GetEveryDataPrice()
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                var _results = new List<EveryDataViewGainsEntity>();
                var aList = conn.Query<TokenStoreEntity>("SELECT * FROM [TokenStore] where [Status] = 1");

                foreach (var item in aList)
                {
                    var rList = conn.Query<GainsInHistoryEntity>($@"select top(2) b.TokenID,b.CurrentPrice,b.CreateTime from dbo.TokenStore as a 
left join
GainsInHistory as b 
on a.TokenHearderText = b.TokenID
where a.TokenHearderText = '{item.TokenHearderText}'
order by b.CreateTime desc").ToList();
                    if (rList.Count > 0)
                    {
                        EveryDataViewGainsEntity entity = new EveryDataViewGainsEntity();
                        entity.TokenID = item.TokenHearderText;
                        entity.TokenIcon = item.TokenIco;
                        entity.CurrentDescript = item.TokenDescription;
                        entity.CurrentPrice = rList[0].CurrentPrice;
                        entity.YesterdayPrice = rList[1].CurrentPrice;
                        entity.IncreaseThan = Math.Round(((entity.CurrentPrice - entity.YesterdayPrice) / entity.YesterdayPrice) * 100, 2);
                        _results.Add(entity);
                    }
                }

                return _results;
            }
        }

        public List<TokenDetailsEntity> GetUserTokenList(int p_UserID) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                var rList = conn.Query<TokenDetailsEntity>($@"
SELECT * FROM TokenDetails WHERE UserID = {p_UserID} and [Stauts] = 1
").ToList();
                rList = rList.GroupBy(x => x.TokenType).Select(x => new TokenDetailsEntity()
                {
                    TokenType = x.FirstOrDefault().TokenType,
                    CurrentIcon = x.Sum(y => y.CurrentIcon)
                }).ToList();
                return rList;
            }
        }

        public List<GainsInHistoryEntity> GetGainsInHisotryList(string p_type) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<GainsInHistoryEntity>($@"SELECT * FROM [GainsInHistory]
  where TokenID = '{p_type}' 
  order by CreateTime desc").ToList();
            }
        }
        


        public Int64 GetTokenCount(string p_type, string p_userid)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<int>($"SELECT ISNULL(SUM([CurrentIcon]),0) FROM TokenDetails WHERE UserID = {p_userid} and TokenType = '{p_type}' and [Stauts] = 1").FirstOrDefault();
            }
        }

        public bool CheckHasSign(string p_type, string p_userid)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                var sql = $@"select COUNT(id) from dbo.TokenDetails where UserID = {p_userid} and [Stauts] = 1 and TokenType = '{p_type}' and CreateTime BETWEEN '{DateTime.Now.ToString("yyyy-MM-dd")}' and '{DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")}'";
                return conn.Query<int>(sql).FirstOrDefault() > 0;
            }
        }

        public bool UpdateAccountCoin(string p_type, string p_userid, string p_total, string p_desc, string p_status = "1")
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Execute($@"INSERT INTO TokenDetails(UserID,TokenType,CurrentIcon,CurrentDescription,Stauts) VALUES
                                           ({p_userid},'{p_type}',{p_total},'{p_desc}',{p_status})") > 0;
            }
        }

        public bool SetTransferMoney(TransferMoney transfer)
        {
            //减少自己的UNK
            decimal myToken = transfer.p_TransferMoney * 1.01M * -1;
            UpdateAccountCoin(transfer.p_Type, transfer.p_FormUserID.ToString(), myToken.ToString(), $"给 [ {transfer.p_TargetUserPhone} ] 转入 [ { transfer.p_TransferMoney * 1.01M} 份 ] UNK");
            //公共账户增加UNK
            decimal publicToken =  transfer.p_TransferMoney * 0.01M;
            UpdateAccountCoin(transfer.p_Type, "1", publicToken.ToString(), $"[ {transfer.p_FormUserPhone} ] 转入 [ { publicToken} 份 ] UNK", "0");
            //目标账户增加UNK
            UpdateAccountCoin(transfer.p_Type, transfer.p_TargetUserID, transfer.p_TransferMoney.ToString(), $"[ {transfer.p_FormUserPhone} ] 转入 [ { transfer.p_TransferMoney.ToString()} 份 ] UNK", "1");
            return true;
        }

        public List<TokenDetailsEntity> GetUserTokenDetailsList(string p_id) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<TokenDetailsEntity>($@"SELECT * FROM [TokenDetails] WHERE UserID = {p_id} and [Stauts] = 1 ORDER BY CreateTime Desc").ToList();
            }
        }
    } 
}
