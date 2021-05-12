using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quote.App.Models.Covid19;
using Quote.App.Models.Covid19.Historical;

namespace Quote.App.ViewModel
{
    public class CountryHistoricalViewModel
    {
        public CountryRoot cou { get; set; }
        public HistoricalRoot historical { get; set; }
    }
}