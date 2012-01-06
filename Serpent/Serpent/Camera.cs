using System;
using Microsoft.Xna.Framework;


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

        public Camera(Rectangle clientBounds, Vector3 pos, Vector3 target, CameraBehavior cameraBehavior)
        {
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
                _desiredUpVector = CameraBehavior == CameraBehavior.FollowSerpent
                    ? Vector3.Up
                    : Vector3.Forward;
}
        }

        public void Update( GameTime gameTime, Vector3 target, Direction direction)
        {
            _upVector = _upVector*99/100 + _desiredUpVector/100;

            if (CameraBehavior == CameraBehavior.FollowSerpent)
            {
                var target2D = new Vector2(target.X, target.Z);
                var position2D = moveTo(
                    new Vector2(_position.X, _position.Z),
                    target2D,
                    target2D - direction.DirectionAsVector2()*9,
                    gameTime.ElapsedGameTime.TotalMilliseconds);

                _position = new Vector3(
                    position2D.X,
                    _position.Y*99/100 + (target.Y + 5)/100,
                    position2D.Y);

            }
            else
            {
                _position = _position*99/100 + new Vector3(10, 30, 10)/100;
                target = _target*99/100 + new Vector3(10, 0, 10)/100;
            }

            _target = target;
            View = Matrix.CreateLookAt(
                _position,
                _target,
                _upVector);
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

            var angleFraction = angle*elapsedTime/2000;

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
