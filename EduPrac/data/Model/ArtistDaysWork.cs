using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduPrac.data.Model
{
    /// <summary>
    /// Name, Month, Day as Range of work days, Count as count of work days in total.
    /// </summary>
    internal class ArtistDaysWork
    {
        public string Name { get; set; }
        public string Month { get; set; }
        public string Count { get; set; }
        public string Day { get; set; }
    }
}
