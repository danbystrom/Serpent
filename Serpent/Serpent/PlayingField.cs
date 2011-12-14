using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serpent
{
    public class PlayingField
    {
        public readonly int Floors, Width, Height;
        public readonly VertexBuffer VertexBuffer;
        public readonly VertexBuffer VertexBufferShadow;
        public readonly PlayingFieldSquare[, ,] TheField;

        private readonly BasicEffect _effect;
        private readonly Camera _camera;
        private readonly Texture2D _texture;

        public PlayingField(GraphicsDevice graphicsDevice, Camera camera, Texture2D texture, int floors, int width, int height)
        {
            _effect = new BasicEffect(graphicsDevice);
            _camera = camera;
            _texture = texture;

            Floors = floors;
            Width = width;
            Height = height;
            TheField = new PlayingFieldSquare[Floors, height, width];

            var builder = new PlayingFieldBuilder(TheField);
            builder.ConstructOneFloor(
                0,
                new[]
                    {
                        "XXXXXXXXXXXXXXXXXXXX",
                        "X   X              X",
                        "XXXXXXXXXXXXXXXXXXXX",
                        "X X X  X           X",
                        "X X X  X           X",
                        "XXX XXXXUUU        X",
                        "X                  X",
                        "XUUU               X",
                        "X                  X",
                        "X                  X",
                        "XXXXXXXXXXXXXXXXXXXX",
                        "X               X  X",
                        "X               X  X",
                        "X               X  X",
                        "XXXXXXXXXXXXXX  X  X",
                        "X            XXXXXXX",
                        "X    U       X X   X",
                        "X    U       XXX   X",
                        "X    U        X    X",
                        "XXXXXXXXXXXXXXXXXXXX"
                    });
            builder.ConstructOneFloor(
                1,
                new[]
                    {
                        "                    ",
                        "                    ",
                        "                    ",
                        "  XXXXXXXXXXXXXXXX  ",
                        "  X              X  ",
                        "  XXXX     DXXXXXX  ",
                        "     X           X  ",
                        "    DXXXXXXXXXXXXX  ",
                        "     X           X  ",
                        "  XXXXXXXXXXXXXXXX  ",
                        "  X X X X           ",
                        "  XXX XXX           ",
                        "   X   X            ",
                        "   XX XX            ",
                        "    XXX             ",
                        "     D              ",
                        "                    ",
                        "                    ",
                        "                    ",
                        "                    ",
                    });

            var verts = new List<VertexPositionNormalTexture>();
            var vertsShadow = new List<VertexPositionColor>();
            for (var z = 0; z < 2; z++)
                for (var y = 0; y < height; y++ )
                     for (var x = 0; x < width; x++)
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
            int z,
            int zh,
            int x,
            int y,
            int w,
            int h)
        {
            verts.Add(new VertexPositionNormalTexture(
                new Vector3((x+w), (z+zh) / 3f, (y+h)),
                Vector3.Up,
                new Vector2((x+w)/2f, (y+h)/2f)));
            vertsShadow.Add(new VertexPositionColor(
                new Vector3(x - (w == 0 ? 0.1f : -0.1f), (z+zh) / 3f - 0.01f, y - (h == 0 ? 0.1f : -0.1f)),
                Color.Black));
        }

        public void Draw()
        {
            //Set object and camera info
            _effect.View = _camera.View;
            _effect.Projection = _camera.Projection;
            _effect.GraphicsDevice.SamplerStates[0] = new SamplerState() {Filter = TextureFilter.Linear};

            _effect.World = Matrix.Identity;
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
            if (fieldValue(floor + 1, newLocation).IsPortal)
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
            Direction dir,
            int floor,
            Point p, 
            float fraction)
        {
            var square = TheField[floor, p.Y, p.X];
            switch ( square.PlayingFieldSquareType )
            {
                case PlayingFieldSquareType.None:
                    throw new Exception();
                case PlayingFieldSquareType.Flat:
                    return floor*1.333f;
                default:
                    if (square.SlopeDirection.Backward == dir)
                        fraction = 1-fraction;
                    else if (square.SlopeDirection != dir)
                        throw new Exception();
                    return floor*1.33f + (square.Elevation + fraction)/3f;
            }
        }

    }

}
