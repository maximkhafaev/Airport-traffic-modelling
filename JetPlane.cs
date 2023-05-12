using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportSimulation
{
    class JetPlane : Plane
    {
        private Random rnd = new Random();

        private int fuelTankL, fuelTankR, fuelTankC;
        public override int Fuel
        {
            get
            {
                return fuelTankL + fuelTankR + fuelTankC;
            }
            set
            {
                if (value > 0)
                {
                    fuelTankC = 100 - rnd.Next(1, value / 3);
                    fuelTankL = fuelTankR = (100 - fuelTankC) / 2;
                }
            }
        }

        public JetPlane(int fuel, string name) : base(fuel, name)
        {
        }
    }
}
