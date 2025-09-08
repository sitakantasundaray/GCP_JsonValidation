using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIDValidator
{
    public class VendorMetadata
    {
        public string LabelVendorID { get; set; }
        public string VendorFacilityID { get; set; }
        public string ShipToDate { get; set; }
        public string ShipToLocation { get; set; }
        public string ProductSKU { get; set; }
        public string LotNumber { get; set; }
        public List<string> RollNumbers { get; set; } = new();
    }
}
