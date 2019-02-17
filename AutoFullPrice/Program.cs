using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unk.Core;
using Dapper;

namespace AutoFullPrice
{
    class Program
    {
        private static string SQLConnectionStr = Utils.GetAppSetting("SqlConnection");
        static void Main(string[] args)
        {
            string action = args[0];


            switch (action)
            {
                case "AutoFullPriceHistory":
                    
                    break;
                case "AutoFullPrice":
                    using (SqlConnection conn =new SqlConnection(SQLConnectionStr))
                    {
                        var vv_prices = conn.Query<decimal>($@"SELECT top(1) CurrentPrice FROM [GainsInHistory] WHERE TokenID = 'UNK' ORDER BY id DESC").FirstOrDefault();
                        vv_prices = vv_prices * 1.1M;
                        conn.Execute(GetSQl(vv_prices, DateTime.Now));
                    }
                    break;
                default:
                    break;
                
            }
        }

        static string GetSQl(decimal StartPice, DateTime StartTime) {
            var sql = $@"
INSERT INTO [dbo].[GainsInHistory]
           ([TokenID]
           ,[CurrentPrice]
           ,[CurrentDescript]
           ,[CreateTime])
     VALUES
           (
            'UNK'
           ,{Math.Round(StartPice, 4)}
           ,'{StartTime.ToString("yyyy-MM-dd")} 更新价格'
           ,'{StartTime.ToString("yyyy-MM-dd 12:00:00")}')
";
            return sql;
        }
    }
}
