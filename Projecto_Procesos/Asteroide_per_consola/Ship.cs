using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroide_per_consola
{
    class Ship
    {
        public int X { get; private set; }
        public int Y { get; }

        public Ship()
        {
            X = Console.WindowWidth / 2;
            Y = Console.WindowHeight - 1;
        }

        public void MoveLeft()
        {
            if (X > 0)
                X--;
        }

        public void MoveRight()
        {
            if (X < Console.WindowWidth - 1)
                X++;
        }
    }
}
