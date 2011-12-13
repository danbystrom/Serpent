using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    public class EnemySerpent : Serpent
    {
        private Random _rnd = new Random();

        public EnemySerpent(
            Game game,
            PlayingField pf,
            Model modelHead,
            Model modelSegment,
            Camera camera)
            : base( game, pf, modelHead, modelSegment, camera )
        {
        }

        protected override void takeDirection()
        {
            if (_rnd.NextDouble() < 0.33 && tryMove(_direction.Left))
                return;
            if (_rnd.NextDouble() < 0.66 && tryMove(_direction.Right))
                return;
            if (!tryMove(_direction))
            {
                if (_rnd.NextDouble() < 0.5 && tryMove(_direction.Left))
                    return;
                if ( tryMove(_direction.Right))
                    return;
                if (tryMove(_direction.Left))
                    return;
                tryMove(_direction.Backward);
            }
        }

    }

}
