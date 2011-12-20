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
            : base(game, pf, modelHead, modelSegment,new Whereabouts(0,Point.Zero,Direction.East))
        {
            _camera = new Camera(
                game,
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
            _camera.Update( gameTime, LookAtPosition, _headDirection);
            checkKeyboardForDirection();
            base.Update(gameTime);
        }

        public Vector3 LookAtPosition
        {
            get
            {
                var d = _whereabouts.Direction.DirectionAsPoint();
                return new Vector3(
                    _whereabouts.Location.X + d.X * (float)_fractionAngle,
                    _pf.GetElevation(_whereabouts),
                    _whereabouts.Location.Y + d.Y * (float)_fractionAngle);
            }
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
            if (!tryMove(_headDirection.Turn(_nextKbdDirection)))
                if (!tryMove(_whereabouts.Direction))
                    _whereabouts.Direction = Direction.None;
            _nextKbdDirection = RelativeDirection.None;
        }


    }
}
