using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostIndex
{
    public class Datum
    {
        public string Postcode { get; set; }
        public string OldPostcode { get; set; }
        public string Address { get; set; }
    }

    public class PostIndexData
    {
        public List<Datum> Data { get; set; }
    }
}
