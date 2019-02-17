using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unk.Biz.Entity
{
    public class TokenStoreEntity : Entity
    {
        public string TokenHearderText { get; set; }
        public string TokenDescription { get; set; }
        public string TokenIco { get; set; }
        public Int64 TokenCount { get; set; }
    }
}
