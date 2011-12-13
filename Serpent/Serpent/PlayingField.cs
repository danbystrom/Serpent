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
        public readonly PlayingFieldSquare[,,] TheField;

        private int[][] _rampInfo = new[]
                               {
                                   new [] {0,0,0,0},
                                   new [] {0,0,0,0},
                                   new [] {0,0,1,1},
                                   new [] {1,1,2,2},
                                   new [] {2,2,3,3},
                                   new [] {-1,-1,0,0},
                                   new [] {0,1,0,1},
                                   new [] {1,2,1,2},
                                   new [] {2,3,2,3},
                                   new [] {-1,0,-1,0},
                               };

        public PlayingField(GraphicsDevice graphicsDevice, int floors, int width, int height)
        {
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
            for (var z = 0; z < 2; z++)
                for (var y = 0; y < height; y++ )
                     for (var x = 0; x < width; x++)
                        if (!TheField[z, y, x].IsNone )
                        {
                            var start = x;
                            //for (x++; x < width && TheField[z, y, x - 1].PlayingFieldSquareType == TheField[z, y, x].PlayingFieldSquareType; x++)
                            //    ;
                            x++;
                            foobar(verts, z, start, y, x - start, 1, TheField[z, y, x - 1].Corners);
                            x--;
                        }

            VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), verts.Count, BufferUsage.None);
            VertexBuffer.SetData(verts.ToArray(), 0, verts.Count);
        }

        private void foobar(
            IList<VertexPositionNormalTexture> verts,
            int z,
            int x,
            int y,
            int w,
            int h,
            int[] ri)
        {
            z *= 4;
            foobar2(verts, z + ri[1], x, y);            //NW
            foobar2(verts, z + ri[3], x + w, y);        //NE
            foobar2(verts, z + ri[0], x, y + h);        //SW
            foobar2(verts, z + ri[0], x, y + h);        //SW
            foobar2(verts, z + ri[3], x + w, y);        //NE
            foobar2(verts, z + ri[2], x + w, y + h);    //SE
        }

        private void foobar2(
            IList<VertexPositionNormalTexture> verts,
            int ramp,
            int x,
            int y)
        {
            verts.Add(new VertexPositionNormalTexture(
                new Vector3(x, ramp / 3f, y),
                Vector3.Up,
                new Vector2(x/2f, y/2f)));
        }


        public void Draw()
        {
            VertexBuffer.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            VertexBuffer.GraphicsDevice.DrawPrimitives(
                PrimitiveType.TriangleList,
                0,
                VertexBuffer.VertexCount);
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
            var ramp = TheField[floor, p.Y, p.X].Corners;
            if ( dir.Equals( Direction.East ) )
            {
                return floor * 1.33f + (ramp[0] + (ramp[2] - ramp[0]) * fraction) / 3;
            }
            if (dir.Equals(Direction.West))
            {
                return floor * 1.33f + (ramp[2] + (ramp[0] - ramp[2]) * fraction) / 3;
            }
            return floor * 1.33f + ramp[0];
        }

    }

}
