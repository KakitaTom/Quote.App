using System;
using System.Collections.Generic;

namespace Quote.App.Models.Covid19.Historical
{
    public class Deaths
    {
        public Dictionary<DateTime, long> dDeath { get; set; }
    }
}