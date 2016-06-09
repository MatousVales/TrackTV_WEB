using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Tracktv.DTO
{
    class Show
    {
        public string Genre { get; set; }
        public bool hasGoldenGlobe { get; set; }
        public string Director { get; set; }
        public string Name { get; set; }
        public int sID { get; set; }
        public int Stars { get; set; }
        public Collection<Actor> starring = new Collection<Actor>();
        public Collection<Episode> episodes = new Collection<Episode>();
        public Collection<Rating> ratings = new Collection<Rating>();
        public Collection<Comment> comments = new Collection<Comment>();


        //// non database attributes

        public int timesInUserHistory { get; set; }
        public double average { get; set; }

    }
}
