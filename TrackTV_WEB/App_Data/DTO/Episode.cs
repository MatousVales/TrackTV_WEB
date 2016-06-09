using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracktv.DTO
{
    class Episode
    {
        public DateTime Airingdate { get; set; }
        public string Name { get; set; }
        public int Shows_sID { get; set; }
        public int eID { get; set; }

        //// non database attributes

        public string ShowName { get; set; }
    }
}
