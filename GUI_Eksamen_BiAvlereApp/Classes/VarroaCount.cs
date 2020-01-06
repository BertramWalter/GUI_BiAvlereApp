using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Eksamen_BiAvlereApp.Classes
{
    public class VarroaCount
    { 
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public int NumberOfMites { get; set; }
        public string Comment { get; set; }

        public VarroaCount(string name, DateTime dt, int numOfMites, string comment)
        {
            Name = name;
            Datetime = dt;
            NumberOfMites = numOfMites;
            Comment = comment;
        }

        public VarroaCount()
        {
            //Empty constructor
        }

        public VarroaCount Clone()
        {
            return this.MemberwiseClone() as VarroaCount;
        }
    }
}
