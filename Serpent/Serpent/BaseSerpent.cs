﻿using System;
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
        protected readonly PlayingField _pf;

        protected Whereabouts _whereabouts = new Whereabouts();
        protected Direction _headDirection;
        protected Camera _camera;

        protected double _fractionAngle;

        protected readonly Model _modelHead;
        protected readonly Model _modelSegment;

        protected VertexBuffer _vb;

        protected readonly SerpentTailSegment _tail;
        protected int _serpentLength;

        protected readonly Dictionary<Direction, Matrix> _headRotation = new Dictionary<Direction, Matrix>();

        protected abstract void takeDirection();

        public bool IsDead;

        protected BaseSerpent(
            Game game,
            PlayingField pf,
            Model modelHead,
            Model modelSegment,
            Whereabouts whereabouts)
            : base(game)
        {
            _pf = pf;
            _modelHead = modelHead;
            _modelSegment = modelSegment;

            _whereabouts = whereabouts;
            _headDirection = _whereabouts.Direction;

            _headRotation.Add(Direction.West,
                              Matrix.CreateRotationY(MathHelper.PiOver2)*Matrix.CreateRotationY(MathHelper.Pi));
            _headRotation.Add(Direction.East,
                              Matrix.CreateRotationY(MathHelper.PiOver2));
            _headRotation.Add(Direction.South,
                              Matrix.CreateRotationY(MathHelper.PiOver2)*Matrix.CreateRotationY(MathHelper.PiOver2));
            _headRotation.Add(Direction.North,
                              Matrix.CreateRotationY(MathHelper.PiOver2)*Matrix.CreateRotationY(-MathHelper.PiOver2));

            _tail = new SerpentTailSegment(_pf, _whereabouts);
            _serpentLength = 1;
        }

        protected override void LoadContent()
        {
            var cube = Cube.CreateCube(0.8f);
            _vb = new VertexBuffer(GraphicsDevice, typeof (VertexPositionNormalTexture), cube.Length, BufferUsage.None);
            _vb.SetData(cube);
            base.LoadContent();
        }

        protected virtual float modifySpeed()
        {
            return 1f;
        }

        public override void Update(GameTime gameTime)
        {
            var lengthSpeed = (11 - _serpentLength)/10f;
            var speed = (float) gameTime.ElapsedGameTime.TotalMilliseconds*0.004f*lengthSpeed*modifySpeed();

            if (_whereabouts.Direction != Direction.None)
            {
                _fractionAngle += speed;
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
                _tail.Update(gameTime, GetPosition(), speed);

            if (_whereabouts.Direction != Direction.None)
                _headDirection = _whereabouts.Direction;

            base.Update(gameTime);
        }


        protected bool tryMove(Direction dir)
        {
            if (dir == Direction.None)
                return false;
            var possibleLocationTo = _whereabouts.Location.Add(dir);
            if (!_pf.CanMoveHere(ref _whereabouts.Floor, _whereabouts.Location, possibleLocationTo))
                return false;
            _whereabouts.Direction = dir;
            _tail.AddPathToWalk(_whereabouts);
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
                            0.4f + p.Y,
                            0.5f + p.Z)
                    });

            var worlds = new List<Matrix>();
            for (var segment = _tail; segment != null; segment = segment.Next)
            {
                var p2 = segment.GetPosition();
                worlds.Add(
                    Matrix.CreateScale(0.4f)*
                    Matrix.CreateTranslation(
                        0.5f + (p.X + p2.X)/2,
                        0.3f + (p.Y + p2.Y)/2,
                        0.5f + (p.Z + p2.Z)/2));
                worlds.Add(
                    Matrix.CreateScale(0.4f)*
                    Matrix.CreateTranslation(
                        0.5f + p2.X,
                        0.3f + p2.Y,
                        0.5f + p2.Z));
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

        public Vector3 GetPosition()
        {
            return _whereabouts.GetPosition(_pf);
        }


        private int _pendingEatenSegments = 4;


        private void grow(int length)
        {
            _pendingEatenSegments += length;
            for (var count = _pendingEatenSegments/5; count > 0; count--)
                addTail();
            _pendingEatenSegments %= 5;
        }

        public bool EatAt(BaseSerpent other)
        {
            if (Vector3.DistanceSquared(GetPosition(), other.GetPosition()) < 0.8f)
            {
                if (other._serpentLength > _serpentLength)
                    return false;
                grow(other._serpentLength + 1);
                return true;
            }
            for (var tail = other._tail; tail != null; tail = tail.Next)
                if (Vector3.DistanceSquared(GetPosition(), tail.GetPosition()) < 0.2f)
                {
                    if (tail == other._tail)
                    {
                        grow(other._serpentLength + 1);
                        return true;
                    }
                    grow(other.removeTail(tail));
                    return false;
                }
            return false;
        }

        private int removeTail(SerpentTailSegment tail)
        {
            if (tail != _tail && tail != null)
            {
                var length = 1;
                for (var test = _tail; test != null; test = test.Next, length++)
                    if (test.Next == tail)
                    {
                        test.Next = null;
                        var removedSegments = _serpentLength - length;
                        _serpentLength = length;
                        return removedSegments;
                    }
            }
            throw new Exception();
        }

        protected void addTail()
        {
            var tail = _tail;
            while (tail.Next != null)
                tail = tail.Next;
            tail.Next = new SerpentTailSegment(_pf, tail.Whereabouts);
            _serpentLength++;
        }

    }

}
