using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quote.App.Models.Covid19;

namespace Quote.App.ViewModel
{
    public class AllCountriesViewModel
    {
        public IList<CountryRoot> countries { get; set; }
    }
}