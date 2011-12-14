using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    public class Serpent : DrawableGameComponent
    {
        private readonly PlayingField _pf;
        private Point _locationTo;
        private Point _locationFrom;
        private int _floor;
        protected Point _diff; // this is were we're heading (0,0),(1,0),(-1,0),(0,1),(0,-1)
        protected Direction _direction;

        private float _fraction;
        private double _fractionAngle;

        private RelativeDirection _nextKbdDirection;

        private readonly Model _modelHead;
        private readonly Model _modelSegment;
        private readonly Camera _camera;

        private VertexBuffer _vb;

        private SerpentTailSegment _tail;

        private Dictionary<Direction, Matrix> _headRotation = new Dictionary<Direction, Matrix>();

        public Serpent(
            Game game,
            PlayingField pf,
            Model modelHead,
            Model modelSegment,
            Camera camera)
            : base(game)
        {
            _pf = pf;
            _modelHead = modelHead;
            _modelSegment = modelSegment;
            _camera = camera;

            _direction = Direction.East;
            _diff = _direction.DirectionAsPoint();
            _locationFrom = Point.Zero;
            _locationTo = _locationFrom.Add(_diff);

            _headRotation.Add(Direction.West,
                              Matrix.CreateRotationY(MathHelper.PiOver2)*Matrix.CreateRotationY(MathHelper.Pi));
            _headRotation.Add(Direction.East,
                              Matrix.CreateRotationY(MathHelper.PiOver2));
            _headRotation.Add(Direction.South,
                              Matrix.CreateRotationY(MathHelper.PiOver2)*Matrix.CreateRotationY(MathHelper.PiOver2));
            _headRotation.Add(Direction.North,
                              Matrix.CreateRotationY(MathHelper.PiOver2)*Matrix.CreateRotationY(-MathHelper.PiOver2));

            _tail = new SerpentTailSegment(_pf,Point.Zero);
            _tail.Next = new SerpentTailSegment(_pf,Point.Zero);
        }

        protected override void LoadContent()
        {
            var cube = Cube.CreateCube(0.8f);
            _vb = new VertexBuffer(GraphicsDevice, typeof (VertexPositionNormalTexture), cube.Length, BufferUsage.None);
            _vb.SetData(cube);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            checkKeyboardForDirection();

            if (_diff != Point.Zero)
            {
                _fractionAngle += gameTime.ElapsedGameTime.TotalMilliseconds*0.003;
                if (_fractionAngle >= 1)
                {
                    _fractionAngle = 0;
                    _locationFrom = _locationTo;
                    takeDirection();
                }
                _fraction = (float) Math.Sin(_fractionAngle*MathHelper.PiOver2);
            }
            else
                takeDirection();

            if (_tail != null)
                _tail.Update(gameTime, GetPosition());

            base.Update(gameTime);
        }

        private KeyboardState _lastKbdState;

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

        protected virtual void takeDirection()
        {
            if ( !tryMove(_direction.Turn(_nextKbdDirection)))
                if (!tryMove(_direction))
                    _diff = Point.Zero;
            _nextKbdDirection = RelativeDirection.None;
        }

        protected bool tryMove(Direction dir)
        {
            var p = dir.DirectionAsPoint();
            var possibleLocationTo = _locationFrom.Add(p);
            if (!_pf.CanMoveHere(ref _floor, _locationFrom, possibleLocationTo))
                return false;
            _direction = dir;
            _diff = p;
            _locationTo = possibleLocationTo;
            _tail.AddPathToWalk(_locationFrom);
            return true;
        }

        public override void Draw(GameTime gameTime)
        {
            var p = GetPosition();
            drawModel(
                _modelHead,
                new[]
                    {
                        _headRotation[Direction]*
                        Matrix.CreateScale(0.5f)*
                        Matrix.CreateTranslation(
                            0.5f + p.X,
                            0.4f + _pf.GetElevation(_direction, _floor, _locationTo, _fraction),
                            0.5f + p.Y)
                    });

            var worlds = new List<Matrix>();
            for (var segment = _tail; segment != null; segment = segment.Next)
            {
                var p2 = segment.GetPosition();
                worlds.Add(
                    Matrix.CreateScale(0.4f)*
                    Matrix.CreateTranslation(
                        0.5f + (p.X + p2.X)/2,
                        0.3f + _pf.GetElevation(_direction, _floor, _locationTo, _fraction),
                        0.5f + (p.Y + p2.Y) / 2));
                worlds.Add(
                    Matrix.CreateScale(0.4f)*
                    Matrix.CreateTranslation(
                        0.5f + p2.X,
                        0.3f + _pf.GetElevation(_direction, _floor, _locationTo, _fraction),
                        0.5f + p2.Y));
                p = p2;
            }
            drawModel(_modelSegment, worlds);

            base.Draw(gameTime);
        }

        private void drawModel(Model model, IEnumerable<Matrix> worlds)
        {
            var transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (var world in worlds)
                foreach (var mesh in model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = _camera.Projection;
                        be.View = _camera.View;
                        be.World = transforms[mesh.ParentBone.Index]*world;
                    }
                    mesh.Draw();
                }
        }

        public Vector2 GetPosition()
        {
            return new Vector2(
                _locationFrom.X + _diff.X*_fraction,
                _locationFrom.Y + _diff.Y*_fraction);
        }

        public Vector3 LookAtPosition
        {
            get
            {
                return new Vector3(
                    _locationFrom.X + _diff.X*(float) _fractionAngle,
                    _pf.GetElevation(_direction,_floor,_locationTo,(float)_fractionAngle),
                    _locationFrom.Y + _diff.Y*(float) _fractionAngle);
            }
        }

        public Direction Direction
        {
            get { return _direction; }
        }

    }

}
