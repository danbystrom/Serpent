using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Serpent
{
     public enum CameraBehavior
     {
        Static,
        FollowSerpent,
        FreeFlying
     }

    public class Camera
    {
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        private Vector3 _position;

        private CameraBehavior _cameraBehavior;
        private Vector3 _upVector;
        private Vector3 _desiredUpVector;
        private Vector3 _target;

        private Rectangle _clientBounds;

        public Camera(Rectangle clientBounds, Vector3 pos, Vector3 target, CameraBehavior cameraBehavior)
        {
            _clientBounds = clientBounds;
            CameraBehavior = cameraBehavior;
            _upVector = _desiredUpVector;

            View = Matrix.CreateLookAt(pos, target, _upVector);
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                clientBounds.Width/(float) clientBounds.Height,
                1, 100);
        }

        public CameraBehavior CameraBehavior
        {
            get { return _cameraBehavior; }
            set
            {
                _cameraBehavior = value;
                switch (CameraBehavior)
                {
                    case CameraBehavior.FollowSerpent:
                        _desiredUpVector = Vector3.Up;
                        break;
                    case CameraBehavior.Static:
                        _desiredUpVector = Vector3.Forward;
                        break;
                    case CameraBehavior.FreeFlying:
                        _angle.Y = -(float) Math.Atan2(_position.X - _target.X, _position.Z - _target.Z);
                        _angle.X = (float)Math.Asin((_position.Y - _target.Y) / Vector3.Distance(_position, _target));
                        Mouse.SetPosition(_clientBounds.Width/2, _clientBounds.Height/2);
                        break;
                }
            }
        }

        private float _acc;

        public void Update( GameTime gameTime, Vector3 target, Direction direction)
        {
            switch (CameraBehavior)
            {
                case CameraBehavior.FollowSerpent:
                    var target2D = new Vector2(target.X, target.Z);
                    var position2D = moveTo(
                        new Vector2(_position.X, _position.Z),
                        target2D,
                        target2D - direction.DirectionAsVector2()*9,
                        gameTime.ElapsedGameTime.TotalMilliseconds);

                    var newPosition = new Vector3(
                        position2D.X,
                        target.Y + 5,
                        position2D.Y);

                    _acc += (float) Math.Sqrt(Vector3.Distance(newPosition, _position))*
                            (float) gameTime.ElapsedGameTime.TotalMilliseconds*0.1f;
                    _acc *= 0.5f;
                    var v = MathHelper.Clamp(_acc, 0.1f, 0.3f);
                    _position = Vector3.Lerp(_position, newPosition, v);
                    _target = Vector3.Lerp(_target, target, v);
                    break;

                case CameraBehavior.Static:
                    _position = Vector3.Lerp(_position, new Vector3(10, 30, 10), 0.02f);
                    _target = Vector3.Lerp(_target, new Vector3(10, 0, 10), 0.02f);
                    break;

                case CameraBehavior.FreeFlying:
                    freeFlyingCamera(gameTime);
                    return;

                default:
                    return;
            }

            _upVector = Vector3.Lerp(_upVector, _desiredUpVector, 0.03f);
            View = Matrix.CreateLookAt(
                _position,
                _target,
                _upVector);
        }

        private Vector3 _angle;

        private void freeFlyingCamera(GameTime gameTime)
        {
            const int speed = 500;
            var delta = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            var centerX = _clientBounds.Width/2;
            var centerY = _clientBounds.Height/2;

            var mouse = Mouse.GetState();
            Mouse.SetPosition(centerX, centerY);

            var deltaX = mouse.X - centerX;
            var deltaY = mouse.Y - centerY;

            var forward = Vector3.Normalize(new Vector3((float)Math.Sin(-_angle.Y), (float)Math.Sin(_angle.X), (float)Math.Cos(-_angle.Y)));
            var left = Vector3.Normalize(new Vector3((float)Math.Cos(_angle.Y), 0f, (float)Math.Sin(_angle.Y)));

            if (mouse.MiddleButton == ButtonState.Released)
            {
                _angle.X += MathHelper.ToRadians(deltaY * 50 * 0.01f); // pitch
                _angle.Y += MathHelper.ToRadians(deltaX * 50 * 0.01f); // yaw
            }
            else
            {
                _position += forward * deltaY * 0.1f;
                _position += left * deltaX * 0.1f;

                if (Data.Instance.KeyboardState.IsKeyDown(Keys.PageUp))
                    _position += Vector3.Down*speed*delta;

                if (Data.Instance.KeyboardState.IsKeyDown(Keys.PageDown))
                    _position += Vector3.Up*speed*delta;
            }

            View = Matrix.CreateTranslation(-_position);
            View *= Matrix.CreateRotationZ(_angle.Z);
            View *= Matrix.CreateRotationY(_angle.Y);
            View *= Matrix.CreateRotationX(_angle.X);
        }

        private static Vector2 moveTo(
            Vector2 camera,
            Vector2 target,
            Vector2 desired,
            double elapsedTime)
        {
            var d2TargetDesired = Vector2.DistanceSquared(target, desired);
            var d2CameraDesired = Vector2.DistanceSquared(camera, desired);
            var d2TargetCamera = Vector2.DistanceSquared(target, camera);

            if (d2CameraDesired < 0.0001f || d2TargetCamera < 0.0001f)
                return desired;

            var d1 = d2TargetDesired + d2TargetCamera - d2CameraDesired;
            var d2 = Math.Sqrt(4*d2TargetDesired*d2TargetCamera);
            var div = d1/d2;
            if (div < -1f)
                div += 2;
            else if (div > 1)
                div -= 2;
            var angle = (float) Math.Acos(div);

            var v1 = camera - target;
            var v2 = desired - target;

            if (v1.X*v2.Y - v2.X*v1.Y > 0)
                angle = -angle;

            var angleFraction = angle*elapsedTime/100;

            var cosA = (float) Math.Cos(angleFraction);
            var sinA = (float) Math.Sin(angleFraction);
            var direction = new Vector2(
                v1.X*cosA + v1.Y*sinA,
                -v1.X*sinA + v1.Y*cosA);
            direction.Normalize();
            return target + direction*v2.Length();
        }

    }

}
