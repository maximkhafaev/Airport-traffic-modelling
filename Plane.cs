using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportSimulation
{
    class Plane
    {
        private int fuelTank;
               
        public virtual int Fuel
        {
            get
            {
                return fuelTank;
            }
            set
            {
                if (value > 0)
                    fuelTank = value;
            }
        }

        public string Name { get; set; }

        public Plane(int fuel, string name)
        {
            Fuel = fuel;
            Name = name;
        }

    }
}
