using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Serpent
{
    public class SerpentTailSegment
    {
        public readonly List<Point> PathToWalk;
 
        public float Fraction;

        public SerpentTailSegment Next;

        private PlayingField _pf;

        public SerpentTailSegment( PlayingField pf, Point p)
        {
            _pf = pf;
            PathToWalk = new List<Point> { p };
        }

        public void Update(GameTime gameTime, Vector2 prevPos)
        {
            var pos = GetPosition();
            if (PathToWalk.Count != 1)
            {
                var distance = Vector2.DistanceSquared(pos, prevPos);
                Fraction += (float) (gameTime.ElapsedGameTime.TotalMilliseconds*0.003); // *distance;
                if ( Fraction >= 1 )
                {
                    Fraction = 0;
                    PathToWalk.RemoveAt(0);
                }
            }
            if ( Next != null )
                Next.Update(gameTime, pos);
        }

        public Vector2 GetPosition()
        {
            if (PathToWalk.Count == 1)
                return new Vector2(PathToWalk[0].X, PathToWalk[0].Y);
            return new Vector2(
                PathToWalk[0].X+(PathToWalk[1].X-PathToWalk[0].X)*Fraction,
                PathToWalk[0].Y+(PathToWalk[1].Y-PathToWalk[0].Y)*Fraction
                );
        }

        public void AddPathToWalk(Point p)
        {
            if (PathToWalk[PathToWalk.Count - 1] != p)
            {
                PathToWalk.Add(p);
                if (Next != null)
                    Next.AddPathToWalk(PathToWalk[0]);
            }
        }
    }
}
