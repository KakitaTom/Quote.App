using System;
using System.Collections.Generic;

namespace Quote.App.Models.Covid19.Historical
{
    public class Recovered
    {
        public Dictionary<DateTime, long> dRecovered { get; set; }
    }
}