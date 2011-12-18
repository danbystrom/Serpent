using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    public abstract class BaseSerpent : DrawableGameComponent
    {
        private readonly PlayingField _pf;

        protected Whereabouts _whereabouts = new Whereabouts();
        protected Camera _camera;

        private double _fractionAngle;

        private readonly Model _modelHead;
        private readonly Model _modelSegment;

        private VertexBuffer _vb;

        private SerpentTailSegment _tail;

        private readonly Dictionary<Direction, Matrix> _headRotation = new Dictionary<Direction, Matrix>();

        protected abstract void takeDirection();

        protected BaseSerpent(
            Game game,
            PlayingField pf,
            Model modelHead,
            Model modelSegment)
            : base(game)
        {
            _pf = pf;
            _modelHead = modelHead;
            _modelSegment = modelSegment;
            
            _headDirection  = _whereabouts.Direction = Direction.East;
            _whereabouts.Location = Point.Zero;

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
            if (_whereabouts.Direction != Direction.None )
            {
                _fractionAngle += gameTime.ElapsedGameTime.TotalMilliseconds*0.003;
                if (_fractionAngle >= 1)
                {
                    _fractionAngle = 0;
                    _whereabouts.Location = _whereabouts.NextLocation;
                    takeDirection();
                }
                _whereabouts.Fraction = (float) Math.Sin(_fractionAngle*MathHelper.PiOver2);
            }
            else
                takeDirection();

            if (_tail != null)
                _tail.Update(gameTime, GetPosition());

            if (_whereabouts.Direction != Direction.None)
                _headDirection = _whereabouts.Direction;

            base.Update(gameTime);
        }

        private Direction _headDirection;

        protected bool tryMove(Direction dir)
        {
            if (dir == Direction.None)
                return false;
            var possibleLocationTo = _whereabouts.Location.Add(dir);
            if (!_pf.CanMoveHere(ref _whereabouts.Floor, _whereabouts.Location, possibleLocationTo))
                return false;
            _whereabouts.Direction = dir;
            _tail.AddPathToWalk(_whereabouts.Location);
            return true;
        }

        public override void Draw(GameTime gameTime)
        {
            var p = GetPosition();
            drawModel(
                _modelHead,
                new[]
                    {
                        _headRotation[_headDirection]*
                        Matrix.CreateScale(0.5f)*
                        Matrix.CreateTranslation(
                            0.5f + p.X,
                            0.4f + _pf.GetElevation(_whereabouts),
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
                        0.3f + _pf.GetElevation(_whereabouts),
                        0.5f + (p.Y + p2.Y) / 2));
                worlds.Add(
                    Matrix.CreateScale(0.4f)*
                    Matrix.CreateTranslation(
                        0.5f + p2.X,
                        0.3f + _pf.GetElevation(_whereabouts),
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
            var d = _whereabouts.Direction.DirectionAsPoint();
            return new Vector2(
                _whereabouts.Location.X + d.X * _whereabouts.Fraction,
                _whereabouts.Location.Y + d.Y * _whereabouts.Fraction);
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

        public Direction Direction
        {
            get { return _headDirection; }
        }

        public void HitTest( Whereabouts where )
        {
            if ( _whereabouts.DistanceSquared(where,_pf) < 1 )
            {
                
            }
        }
    }

}
