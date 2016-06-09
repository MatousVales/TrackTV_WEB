using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Tracktv.DTO
{
    class User
    {
        public string Name { get; set; }
        public bool isAdmin { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public int uID { get; set; }
        public int DailyScore { get; set; }
        public Collection<HistoryEntry> history = new Collection<HistoryEntry>();
        public Collection<ShowsLibraryEntry> usersLibrary = new Collection<ShowsLibraryEntry>();
        
        //// non database attributes

        public Comment bestComment { get; set; }
        public Actor mostWatchedActor { get; set; }
        public Show mostWatchedShow { get; set; }
        public Collection<Show> suggestedShows = new Collection<Show>();
        public Collection<Episode> upcomingEpisodes = new Collection<Episode>();

    }
}
