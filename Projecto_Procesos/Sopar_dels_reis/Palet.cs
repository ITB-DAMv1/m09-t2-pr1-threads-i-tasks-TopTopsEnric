using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sopar_dels_reis
{
    class Palet
    {
        public int Id { get; }
        public object Bloqueig { get; }

        public Palet(int id)
        {
            Id = id;
            Bloqueig = new object();
        }
    }
}
