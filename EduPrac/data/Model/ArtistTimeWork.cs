using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduPrac.data.Model
{
    /// <summary>
    /// year, month, name artist, hours
    /// </summary>
    public class ArtistTimeWork
    {
        public int year { get; set; }
        public string month { get; set; }
        public string name {  get; set; }
        public double hours { get; set; }
    }
}
