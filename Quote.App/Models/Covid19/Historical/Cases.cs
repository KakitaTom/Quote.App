using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quote.App.Models.Covid19.Historical
{
    public class Cases
    {
        public Dictionary<DateTime, long> dCase { get; set; }
    }
}
