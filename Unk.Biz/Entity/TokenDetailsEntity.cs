using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unk.Biz.Entity
{
    public class TokenDetailsEntity : Entity
    {
        public int UserID { get; set; }
        public string TokenType { get; set; }
        public Int64 CurrentIcon { get; set; }
        public string CurrentDescription { get; set; }

    } 
}
