using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    public enum SerpentStatus
    {
        Alive,
        Ghost,
        Finished
    }

    public abstract class BaseSerpent
    {
        public SerpentStatus SerpentStatus;

        protected readonly PlayingField _pf;

        protected Whereabouts _whereabouts = new Whereabouts();
        protected Direction _headDirection;
        protected Camera _camera;

        protected double _fractionAngle;

        protected readonly ModelWrapper _modelHead;
        protected readonly ModelWrapper _modelSegment;

        protected readonly SerpentTailSegment _tail;
        protected int _serpentLength;

        protected readonly Dictionary<Direction, Matrix> _headRotation = new Dictionary<Direction, Matrix>();

        protected abstract void takeDirection();

        private int _pendingEatenSegments = 4;

        private float _layingEgg;
        private const float TimeForLayingEggProcess = 5;

        protected BaseSerpent(
            Game game,
            PlayingField pf,
            ModelWrapper modelHead,
            ModelWrapper modelSegment,
            Whereabouts whereabouts)
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

            _layingEgg = (float)(-5 - new Random().NextDouble()*30);
        }

        protected virtual float modifySpeed()
        {
            return 1f;
        }

        public virtual void Update(GameTime gameTime)
        {
            var lengthSpeed = (11 - _serpentLength)/10f;
            var speed = (float) gameTime.ElapsedGameTime.TotalMilliseconds*0.0045f*lengthSpeed*modifySpeed();

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

        public virtual void Draw(GameTime gameTime)
        {
            var p = GetPosition();
            _modelHead.Draw(
                _camera,
                _headRotation[_headDirection]*
                Matrix.CreateScale(0.5f)*
                Matrix.CreateTranslation(
                    p.X,
                    0.4f + p.Y,
                    p.Z),
                tintColor(),
                0.5f,
                SerpentStatus == SerpentStatus.Alive ? 1 : 0.5f);

            var worlds = new List<Matrix>();

            var segment = _tail;
            while ( true )
            {
                var p2 = segment.GetPosition();
                worlds.Add(
                    Matrix.CreateScale(0.4f)*
                    Matrix.CreateTranslation(
                        (p.X + p2.X)/2,
                        0.3f + (p.Y + p2.Y)/2,
                        (p.Z + p2.Z)/2));
                worlds.Add(
                    Matrix.CreateScale(0.4f)*
                    Matrix.CreateTranslation(
                        p2.X,
                        0.3f + p2.Y,
                        p2.Z));
                p = p2;
                if ( segment.Next == null )
                    break;
                segment = segment.Next;
            }

            if (_layingEgg > -500)
            {
                var d = segment.Whereabouts.Direction;
                var t = d == Direction.North || d == Direction.South
                                   ? Matrix.CreateScale(0.6f, 0.6f, 0.8f)
                                   : Matrix.CreateScale(0.8f, 0.6f, 0.6f);
                var off = d.DirectionAsVector2()*(-0.3f);
                t *= Matrix.CreateTranslation(off.X, -0.3f, off.Y);
                _modelSegment.Draw(
                    _camera,
                    t*worlds[worlds.Count - 1],
                    Vector3.One,
                    1,
                    1);
                _modelSegment.Draw(
                    _camera,
                    worlds[worlds.Count - 1],
                    tintColor(),
                    0.5f,
                    0.9f);
                worlds.RemoveAt(worlds.Count - 1);
            }

            foreach ( var world in worlds )
                _modelSegment.Draw(
                    _camera,
                    world,
                    tintColor(),
                    0.5f,
                    SerpentStatus == SerpentStatus.Alive ? 1 : 0.5f);
        }

        protected virtual Vector3 tintColor()
        {
            return Vector3.Zero;
        }

        public Vector3 GetPosition()
        {
            return _whereabouts.GetPosition(_pf);
        }

        private void grow(int length)
        {
            _pendingEatenSegments += length;
            for (var count = _pendingEatenSegments/5; count > 0; count--)
                addTail();
            _pendingEatenSegments %= 5;
        }

        protected bool _isLonger;

        public bool EatAt(BaseSerpent other)
        {
            _isLonger = _serpentLength >= other._serpentLength;
            if (SerpentStatus != SerpentStatus.Alive)
                return false;
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
