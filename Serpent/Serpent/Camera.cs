using System;
using Microsoft.Xna.Framework;


namespace Serpent
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        private readonly BaseSerpent _serpent;

        private Vector3 _position;

        public Camera(Game game, PlayerSerpent serpent, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            _serpent = serpent;
            // Initialize view matrix
            View = Matrix.CreateLookAt(pos, target, up);

            // Initialize projection matrix
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float) Game.Window.ClientBounds.Width/
                (float) Game.Window.ClientBounds.Height,
                1, 100);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            var target = _serpent.LookAtPosition;
            var t2 = new Vector2(target.X, target.Z);
            var qaz = this.moveTo(
                t2,
                t2 - _serpent.Direction.DirectionAsVector2() * 9,
                gameTime.ElapsedGameTime.TotalMilliseconds);
 
            _position = new Vector3(
                qaz.X,
                target.Y + 5,
                qaz.Y );

            View = Matrix.CreateLookAt(
                 _position,
                 new Vector3(target.X, target.Y, target.Z), 
                 Vector3.Up);

            base.Update(gameTime);
        }

        private Vector2 moveTo( Vector2 target, Vector2 desired, double elapsedTime )
        {
            var camera = new Vector2(_position.X, _position.Z);
            var d2TargetDesired = Vector2.DistanceSquared(target, desired);
            var d2CameraDesired = Vector2.DistanceSquared(camera, desired);
            var d2TargetCamera = Vector2.DistanceSquared(target, camera);

            if (d2CameraDesired < 0.0001f || d2TargetCamera < 0.0001f )
                return desired;

            var d1 = d2TargetDesired + d2TargetCamera - d2CameraDesired;
            var d2 = 2 * Math.Sqrt(d2TargetDesired) * Math.Sqrt(d2TargetCamera);
            var div = d1/d2;
            if (div <= -1f)
                div += 2;
            else if (div >= 1)
                div -= 2;
            var angle = (float) Math.Acos(div);

            var v1 = camera - target;
            var v2 = desired - target;
 
            if ( v1.X*v2.Y - v2.X*v1.Y > 0 )
                angle = -angle;

            var angleFraction = angle * elapsedTime / 2000;

            var cosA = (float)Math.Cos(angleFraction);
            var sinA = (float)Math.Sin(angleFraction);
            var direction = new Vector2(
                v1.X * cosA + v1.Y * sinA,
                -v1.X * sinA + v1.Y * cosA);
            direction.Normalize();
            return target + direction * v2.Length();
        }

    }

}
