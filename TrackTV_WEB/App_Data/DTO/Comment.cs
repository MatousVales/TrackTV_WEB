using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracktv.DTO
{
    class Comment
    {
        public int Score { get; set; }
        public DateTime Datetime { get; set; }
        public string Text { get; set; }
        public int Shows_sID { get; set; }
        public int Users_uID { get; set; }
        public int cID { get; set; }

        //// non database attributes

        public string userLogin { get; set; }
    }
}
