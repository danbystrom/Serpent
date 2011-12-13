using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Serpent
{
    public enum RelativeDirection
    {
        None,
        Left,
        Backward,
        Right,
        Forward
    }

    public struct Direction
    {
        public static readonly Direction[] AllDirections;

        public static readonly Direction None;
        public static readonly Direction South;
        public static readonly Direction West;
        public static readonly Direction North;
        public static readonly Direction East;

        private readonly int _dir;

        static Direction()
        {
            None = new Direction(0);
            South = new Direction(1);
            West = new Direction(2);
            North = new Direction(3);
            East = new Direction(4);
            AllDirections = new[] {South, West, North, East};
        }

        private Direction( int dir )
        {
            _dir = dir;
        }

        public Direction Turn( RelativeDirection rd )
        {
            if (_dir == 0)
                return None;
            return new Direction(1+((_dir+(int)rd-1) & 3));
        }

        public Direction Right
        {
            get { return Turn(RelativeDirection.Right); }
        }

        public Direction Left
        {
            get { return Turn(RelativeDirection.Left); }
        }

        public Direction Backward
        {
            get { return Turn(RelativeDirection.Backward); }
        }

        public Point DirectionAsPoint()
        {
            return new []
                       {
                           Point.Zero,
                           new Point( 0, -1), 
                           new Point( -1, 0), 
                           new Point( 0, 1), 
                           new Point( 1, 0), 
                       }[_dir];
        }

        public Vector2 DirectionAsVector2()
        {
            var p = DirectionAsPoint();
            return new Vector2(p.X, p.Y);
        }

        public Vector3 DirectionAsVector3()
        {
            var p = DirectionAsPoint();
            return new Vector3(p.X, 0, p.Y);
        }

        public static bool operator == (Direction d1, Direction d2)
        {
            return d1._dir == d2._dir;
        }

        public static bool operator !=(Direction d1, Direction d2)
        {
            return d1._dir != d2._dir;
        }

        public override bool Equals(object obj)
        {
            return (obj is Direction) && ((Direction) obj)._dir == _dir;
        }

        public override int GetHashCode()
        {
            return _dir;
        }

        public override string ToString()
        {
            return new[] {"South", "West", "North", "East"}[_dir];
        }

    }

}
