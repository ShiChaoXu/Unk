using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unk.Biz.Entity
{
    public class NewsEntity : Entity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
