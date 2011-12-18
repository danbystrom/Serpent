using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    public class PlayerSerpent : BaseSerpent
    {
        private RelativeDirection _nextKbdDirection;
        private KeyboardState _lastKbdState;

        public PlayerSerpent(
            Game game,
            PlayingField pf,
            Model modelHead,
            Model modelSegment)
            : base(game, pf, modelHead, modelSegment)
        {
            _camera = new Camera(
                game,
                this,
                new Vector3(0, 20, 2),
                Vector3.Zero,
                Vector3.Up);
        }


        public Camera Camera
        {
            get { return _camera; }    
        }

        public override void Update(GameTime gameTime)
        {
            checkKeyboardForDirection();
            base.Update(gameTime);
        }

        private void checkKeyboardForDirection()
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
                _nextKbdDirection = RelativeDirection.Left;
            else if (keyboardState.IsKeyDown(Keys.Right))
                _nextKbdDirection = RelativeDirection.Right;
            else if (keyboardState.IsKeyDown(Keys.Down) && !_lastKbdState.IsKeyDown(Keys.Down))
                _nextKbdDirection = RelativeDirection.Backward;
            _lastKbdState = keyboardState;
        }

        protected override void takeDirection()
        {
            if (!tryMove(Direction.Turn(_nextKbdDirection)))
                if (!tryMove(_whereabouts.Direction))
                    _whereabouts.Direction = Direction.None;
            _nextKbdDirection = RelativeDirection.None;
        }


    }
}
