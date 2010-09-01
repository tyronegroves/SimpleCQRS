using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdDinner.Models;

namespace NerdDinner.Models
{
    public class FlairViewModel
    {
        public IList<Dinner> Dinners { get; set; }
        public string LocationName { get; set; }
    }
}
