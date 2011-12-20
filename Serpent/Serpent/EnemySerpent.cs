using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    public class EnemySerpent : BaseSerpent
    {
        private readonly Random _rnd = new Random();

        public EnemySerpent(
            Game game,
            PlayingField pf,
            Model modelHead,
            Model modelSegment,
            Camera camera,
            Whereabouts whereabouts,
            int x)
            : base(game, pf, modelHead, modelSegment, whereabouts)
        {
            _whereabouts = whereabouts;
             _rnd.NextBytes(new byte[x]);
            _camera = camera;

            addTail();
            addTail();
            addTail();
        }

        protected override void takeDirection()
        {
            if (_rnd.NextDouble() < 0.33 && tryMove(_whereabouts.Direction.Left))
                return;
            if (_rnd.NextDouble() < 0.66 && tryMove(_whereabouts.Direction.Right))
                return;
            if (!tryMove(_whereabouts.Direction))
            {
                if (_rnd.NextDouble() < 0.5 && tryMove(_whereabouts.Direction.Left))
                    return;
                if (tryMove(_whereabouts.Direction.Right))
                    return;
                if (tryMove(_whereabouts.Direction.Left))
                    return;
                tryMove(_whereabouts.Direction.Backward);
            }
        }

    }

}
