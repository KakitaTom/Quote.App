using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quote.App.Models
{
    public class Quote
    {
        public string id { get; set; }

        [Required]
        public string text { get; set; }

        [Required]
        public string author { get; set; }
    }
}