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
 
        public SerpentTailSegment Next;

        private readonly PlayingField _pf;

        public SerpentTailSegment(PlayingField pf, Whereabouts w)
        {
            _pf = pf;
            PathToWalk = new List<Whereabouts> { w };
        }

        public Whereabouts Whereabouts
        {
            get { return PathToWalk[0]; }
        }

        public void Update(GameTime gameTime, Vector3 prevPos, float speed)
        {
            var pos = GetPosition();
            if (PathToWalk.Count != 1)
            {
                var distance = Vector3.DistanceSquared(pos, prevPos);
                var w = PathToWalk[0];
                w.Fraction += speed; // *distance;
                if ( w.Fraction >= 1 )
                    PathToWalk.RemoveAt(0);
                else
                    PathToWalk[0] = w;
            }
            if ( Next != null )
                Next.Update(gameTime, pos, speed);
        }

        public Vector3 GetPosition()
        {
            return PathToWalk[0].GetPosition(_pf);
        }

        public void AddPathToWalk(Whereabouts w)
        {
            if (PathToWalk.Exists(e => e.Location == w.Location))
                return;

            w.Fraction = 0;
            PathToWalk.Add(w);
            if (Next != null)
                Next.AddPathToWalk(PathToWalk[0]);
        }
    }
}
