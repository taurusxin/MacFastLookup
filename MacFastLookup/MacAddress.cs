using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacFastLookup
{
    public class MacAddress
    {
        public string Id { get; set; }
        public string Prefix { get; set; }
        public string VendorName { get; set; }
        public bool Private { get; set; }
        public string BlockType { get; set; }
        public string LastUpdate { get; set; }
    }
}
