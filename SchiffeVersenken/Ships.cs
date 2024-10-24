using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    internal class Ships
    {

        // Orientierung
        // Startkoordinate
        // Länge
        public Ships(ushort length, string name)
        {
            Length = length;
            Name = name;
            health = Length;
        }

        private ushort Length { get; set; }
        private string Name { get; set; }
        public int health { get; set; }

        public string GetName()
        {
            return Name;
        }

        public ushort GetLength()
        {
            return Length;
        }

        public void LooseHealth()
        {
            health--;
        }

    }

}
