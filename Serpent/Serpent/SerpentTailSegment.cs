using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Serpent
{
    public class SerpentTailSegment
    {
        public readonly List<Whereabouts> PathToWalk;
 
        public float Fraction;

        public SerpentTailSegment Next;

        private PlayingField _pf;

        public SerpentTailSegment(PlayingField pf, Whereabouts w)
        {
            _pf = pf;
            PathToWalk = new List<Whereabouts> { w };
        }

        public void Update(GameTime gameTime, Vector3 prevPos)
        {
            var pos = GetPosition();
            if (PathToWalk.Count != 1)
            {
                var distance = Vector3.DistanceSquared(pos, prevPos);
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

        public Vector3 GetPosition()
        {
            var v = new Vector3(
                PathToWalk[0].Location.X,
                _pf.GetElevation(PathToWalk[0]),
                PathToWalk[0].Location.Y);
            if (PathToWalk.Count == 1)
                return v;
            return v + new Vector3(
                (PathToWalk[1].Location.X - PathToWalk[0].Location.X) * Fraction,
                0,
                (PathToWalk[1].Location.Y - PathToWalk[0].Location.Y) * Fraction);
        }

        public void AddPathToWalk(Whereabouts w)
        {
            if (PathToWalk[PathToWalk.Count - 1].Location != w.Location)
            {
                PathToWalk.Add(w);
                if (Next != null)
                    Next.AddPathToWalk(PathToWalk[0]);
            }
        }
    }
}
