using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Tracktv.DTO
{
    class Actor
    {
        public string Gender { get; set; }
        public string Name { get; set; }
        public int aID { get; set; }
        public double Average { get; set; }
        public int AmountOfAwards { get; set; }
        public Collection<Show> starsIn = new Collection<Show>();


        //// non database attributes

        public int timesInUserHistory { get; set; }
    }
}
