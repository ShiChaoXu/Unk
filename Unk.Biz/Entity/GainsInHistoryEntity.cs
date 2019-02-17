using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unk.Biz.Entity
{
    public class GainsInHistoryEntity:Entity
    {
        public string TokenID { get; set; }
        public decimal CurrentPrice { get; set; }
        public string CurrentDescript { get; set; }
        public string TokenIcon { get; set; }
    }

    public class EveryDataViewGainsEntity : GainsInHistoryEntity
    {
        public decimal YesterdayPrice { get; set; }
        public decimal IncreaseThan { get; set; }
    }
}
