using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serpent
{
    public class PlayingField : IDisposable
    {
        public readonly int Floors, Width, Height;
        public readonly VertexBuffer VertexBuffer;
        public readonly VertexBuffer VertexBufferShadow;
        public readonly PlayingFieldSquare[, ,] TheField;

        private readonly BasicEffect _effect;
        private readonly Texture2D _texture;

        public PlayingField(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            _effect = new BasicEffect(graphicsDevice);
            _texture = texture;

            var field = PlayingFields.GetQ();

            Floors = field.Count;
            Height = field[0].Length;
            Width = field[0][0].Length;
            TheField = new PlayingFieldSquare[Floors, Height, Width];

            var builder = new PlayingFieldBuilder(TheField);
            for (var i = 0; i < field.Count; i++)
                builder.ConstructOneFloor(
                    i,
                    field[i]);

            var verts = new List<VertexPositionNormalTexture>();
            var vertsShadow = new List<VertexPositionColor>();
            for (var z = 0; z < Floors; z++)
                for (var y = 0; y < Height; y++ )
                     for (var x = 0; x < Width; x++)
                        if (!TheField[z, y, x].IsNone )
                        {
                            var start = x;
                            //for (x++; x < width && TheField[z, y, x - 1].PlayingFieldSquareType == TheField[z, y, x].PlayingFieldSquareType; x++)
                            //    ;
                            x++;
                            foobar(verts, vertsShadow, z, start, y, x - start, 1, TheField[z, y, x - 1].Corners);
                            x--;
                        }

            VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), verts.Count, BufferUsage.None);
            VertexBuffer.SetData(verts.ToArray(), 0, verts.Count);
 
            VertexBufferShadow = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), vertsShadow.Count, BufferUsage.None);
            VertexBufferShadow.SetData(vertsShadow.ToArray(), 0, vertsShadow.Count);
        }

        private void foobar(
            IList<VertexPositionNormalTexture> verts,
            IList<VertexPositionColor> vertsShadow,
            int z,
            int x,
            int y,
            int w,
            int h,
            int[] ri)
        {
            z *= 4;
            addVertex(verts, vertsShadow, z, ri[1], x, y, 0, 0); //NW
            addVertex(verts, vertsShadow, z, ri[3], x, y, w, 0); //NE
            addVertex(verts, vertsShadow, z, ri[0], x, y, 0, h); //SW
            addVertex(verts, vertsShadow, z, ri[0], x, y, 0, h); //SW
            addVertex(verts, vertsShadow, z, ri[3], x, y, w, 0); //NE
            addVertex(verts, vertsShadow, z, ri[2], x, y, w, h); //SE
        }

        private void addVertex(
            IList<VertexPositionNormalTexture> verts,
            IList<VertexPositionColor> vertsShadow,
            int floor,
            int z,
            int x,
            int y,
            int w,
            int h)
        {
            verts.Add(new VertexPositionNormalTexture(
                new Vector3((x+w), (floor+z) / 3f, (y+h)),
                Vector3.Up,
                new Vector2((x+w)/2f, (y+h)/2f)));

            var dx = w == 0 ? -1 : 1;
            var dy = h == 0 ? -1 : 1;
            float fx = x;
            float fy = y;
            var tmpFloor = floor;
            if (!CanMoveHere(ref tmpFloor, new Point(x,y), new Point(x+dx, y)))
                fx += dx/20f;
            tmpFloor = floor;
            if (!CanMoveHere(ref tmpFloor, new Point(x, y), new Point(x, y+dy)))
                fy += dy/20f;

            vertsShadow.Add(new VertexPositionColor(
                new Vector3(fx+w, (floor+z) / 3f - 0.05f, fy+h),
                Color.Black));
        }

        public void Draw( Camera camera )
        {
            //Set object and camera info
            _effect.View = camera.View;
            _effect.Projection = camera.Projection;
            _effect.GraphicsDevice.SamplerStates[0] = new SamplerState() {Filter = TextureFilter.Linear};

            _effect.World = Matrix.CreateTranslation( -0.5f, 0, -0.5f );
            _effect.Texture = _texture;
            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = false;
            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                VertexBuffer.GraphicsDevice.SetVertexBuffer(VertexBuffer);
                VertexBuffer.GraphicsDevice.DrawPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    VertexBuffer.VertexCount);
            }

            _effect.TextureEnabled = false;
            _effect.VertexColorEnabled = true;
            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                VertexBufferShadow.GraphicsDevice.SetVertexBuffer(VertexBufferShadow);
                VertexBufferShadow.GraphicsDevice.DrawPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    VertexBufferShadow.VertexCount);
            }

        }

        private PlayingFieldSquare fieldValue( int floor, Point p)
        {
            if (floor < 0 || floor >= Floors)
                return new PlayingFieldSquare();
            if (p.Y < 0 || p.Y >= Height)
                return new PlayingFieldSquare();
            if (p.X < 0 || p.X >= Width)
                return new PlayingFieldSquare();
            return TheField[floor, p.Y, p.X];
        }

        public bool CanMoveHere( ref int floor, Point currentLocation, Point newLocation )
        {
            if (!fieldValue(floor, newLocation).IsNone)
                return true;
            if (fieldValue(floor, currentLocation).IsSlope && fieldValue(floor + 1, newLocation).IsPortal)
            {
                floor++;
                return true;
            }
            if (fieldValue(floor, currentLocation).IsPortal && !fieldValue(floor - 1, newLocation).IsNone)
            {
                floor--;
                return true;
            }
            return false;
        }

        public float GetElevation(
            Whereabouts whereabouts )
        {
            if ( whereabouts.Direction == Direction.None )
            {
                
            }
            var p = whereabouts.NextLocation;
            var square = fieldValue(whereabouts.Floor, p);
            if (square.IsNone)
                return 0;
            switch ( square.PlayingFieldSquareType )
            {
                case PlayingFieldSquareType.None:
                    throw new Exception();
                case PlayingFieldSquareType.Flat:
                    return whereabouts.Floor * 1.333f;
                default:
                    var fraction = whereabouts.Fraction;
                    if (square.SlopeDirection.Backward == whereabouts.Direction )
                        fraction = 1-fraction;
                    else if (square.SlopeDirection != whereabouts.Direction)
                        throw new Exception();
                    return whereabouts.Floor * 1.33f + (square.Elevation + fraction) / 3f;
            }
        }

        public void Dispose()
        {
            //_effect.Dispose();
            //VertexBuffer.Dispose();
            //VertexBufferShadow.Dispose();
        }
    }

}
