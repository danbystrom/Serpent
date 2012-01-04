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

        public Vector3 GetPosition(PlayingField pf)
        {
            var d = Direction.DirectionAsPoint();
            return new Vector3(
                Location.X + d.X * Fraction,
                pf.GetElevation(this),
                Location.Y + d.Y * Fraction);
        }

    }

}
