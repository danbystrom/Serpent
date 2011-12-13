using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Serpent
{
    public struct Location
    {
        public int Floor;
        public Point P;

        public Location( int floor, Point p )
        {
            Floor = floor;
            P = p;
        }
    }
}
