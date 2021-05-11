using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quote.App.Models.Covid19;

namespace Quote.App.ViewModel
{
    public class ContinentCountriesViewModel
    {
        public ContinentRoot continent { get; set; }
        public IEnumerable<CountryRoot> countries { get; set; }
    }
}