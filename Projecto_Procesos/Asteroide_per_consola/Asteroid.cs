using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroide_per_consola
{
    class Asteroid
    {
        public int X { get; }
        public int Y { get; private set; }

        public Asteroid(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Update()
        {
            Y++;
        }
    }
}
