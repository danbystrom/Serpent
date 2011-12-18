using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Serpent
{
    public struct Whereabouts
    {
        public int Floor;
        public Point Location;
        public Direction Direction;
        public float Fraction;

        public Whereabouts( int floor, Point location, Direction direction )
        {
            Floor = floor;
            Location = location;
            Direction = direction;
            Fraction = 0;
        }

        public Point NextLocation
        {
            get { return Location.Add(Direction.DirectionAsPoint());  }    
        }

        public float DistanceSquared(Whereabouts other, PlayingField pf )
        {
            var dx = Location.X - other.Location.X;
            var dy = Location.X - other.Location.X;
            var dz = pf.GetElevation(this) - pf.GetElevation(other);
            return dx*dx + dy*dy + dz*dz;
        }

    }

}
