using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracktv.DTO
{
    class HistoryEntry
    {
        public DateTime Datetime { get; set; }
        public int Episodes_eID { get; set; }
        public int Users_uID { get; set; }

        //// non database attributes

        public string episodeName { get; set; }
        public string showName { get; set; }
    }
}
