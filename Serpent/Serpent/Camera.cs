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
        private Vector3 _goingFromUpVector;
        private Vector3 _desiredUpVector;
        private float _upVectorFactor;

        public Camera(Rectangle clientBounds, Vector3 pos, Vector3 target, CameraBehavior cameraBehavior)
        {
            CameraBehavior = cameraBehavior;
            _goingFromUpVector = _upVector = _desiredUpVector;
            _upVectorFactor = 1;

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
                _goingFromUpVector = _upVector;
                _upVectorFactor = 0;
                _desiredUpVector = CameraBehavior == CameraBehavior.FollowSerpent
                    ? Vector3.Up
                    : Vector3.Forward;
}
        }

        public void Update( GameTime gameTime, Vector3 target, Direction direction)
        {
            if ( _upVectorFactor < 1 )
            {
                _upVectorFactor = Math.Min( _upVectorFactor + (float)gameTime.ElapsedGameTime.TotalMilliseconds*0.003f, 1);
                Vector3.SmoothStep(ref _goingFromUpVector, ref _desiredUpVector, _upVectorFactor, out _upVector);
            }

            if (CameraBehavior == CameraBehavior.FollowSerpent)
            {
                var t2 = new Vector2(target.X, target.Z);
                var qaz = this.moveTo(
                    t2,
                    t2 - direction.DirectionAsVector2()*9,
                    gameTime.ElapsedGameTime.TotalMilliseconds);

                _position = new Vector3(
                    qaz.X,
                    target.Y + 5,
                    qaz.Y);

                View = Matrix.CreateLookAt(
                    _position,
                    new Vector3(target.X, target.Y, target.Z),
                    _upVector);
            }
            else
            {
                View = Matrix.CreateLookAt(
                    new Vector3(10, 30, 10),
                    new Vector3(10, 0, 10),
                    _upVector);
            }
        }

        private Vector2 moveTo(Vector2 target, Vector2 desired, double elapsedTime)
        {
            var camera = new Vector2(_position.X, _position.Z);
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
