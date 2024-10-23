using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    internal class Schiff
    {

        // Orientierung
        // Startkoordinate
        // Länge
        public Schiff(ushort length, string name)
        {
            Length = length;
            Name = name;

        }

        private ushort Length { get; set; }
        private string Name { get; set; }
        private ushort Direction { get; set; }
        private ushort Location { get; set; }

        public string GetName()
        {
            return Name;
        }

        public ushort GetLength()
        {
            return Length;
        }


    }

}
