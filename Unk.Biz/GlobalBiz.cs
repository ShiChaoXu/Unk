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
    public class GlobalBiz
    {
        public Tuple<string, string, string, string> GetDashBoard()
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                string totalUser = conn.Query<string>("select count(id) from UserInfo").FirstOrDefault();
                string totalCurrentUser = conn.Query<string>($"select count(id) from UserInfo where CreateTime between '{DateTime.Now.ToString("yyyy-MM-dd")}' and '{DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")}'").FirstOrDefault();
                string totalUnk = conn.Query<string>("select SUM(CurrentIcon) from TokenDetails where TokenType = 'UNK'").FirstOrDefault();
                string currentPrice = conn.Query<string>("SELECT top 1 CurrentPrice FROM[UNK].[dbo].[GainsInHistory] order by id desc").FirstOrDefault();
                return new Tuple<string, string, string, string>(totalUnk, totalUser, currentPrice, totalCurrentUser);
            }            
        }
    }
}
