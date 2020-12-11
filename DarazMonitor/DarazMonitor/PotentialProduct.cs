using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarazMonitor
{
    public class PotentialProduct
    {

        public string LinkToProduct { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public float Ratings { get; set; }
        public List<DateTime> Reviews { get; set; }
        public string category { get; set; }
        public bool isValidCategory { get; set; }
        public bool isAgeRestricted { get; set; }
        public bool isBeingMonitored { get; set; }
        public int VariantTypes { get; set; }
        public List<PotentialProductStock> stockHistory { get; set; }
    }
    public class PotentialProductStock
    {
        public int Stock { get; set; }
        public DateTime Date { get; set; }
    }
}
