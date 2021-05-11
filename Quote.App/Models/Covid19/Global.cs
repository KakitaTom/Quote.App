using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quote.App.Models.Covid19
{
    public class Global
    {
        public long cases { get; set; }
        public long todayCases { get; set; }
        public long deaths { get; set; }
        public long todayDeaths { get; set; }
        public long recovered { get; set; }
        public long todayRecovered { get; set; }
        public long active { get; set; }
        public long critical { get; set; }
        public long tests { get; set; }
        public long population { get; set; }
    }
}