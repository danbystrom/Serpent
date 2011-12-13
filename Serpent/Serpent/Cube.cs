using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serpent
{
    public static class Cube
    {
        public static VertexPositionNormalTexture[] CreateCube( float size )
        {
            var topLeft = new Vector2(0.0f, 0.0f);
            var topRight = new Vector2(1.0f/1000, 0.0f);
            var bottomLeft = new Vector2(0.0f, 1.0f / 1000);
            var bottomRight = new Vector2(1.0f / 1000, 1.0f / 1000);

            var front = Vector3.Backward;
            var back = Vector3.Forward;
            var left = Vector3.Left;
            var right = Vector3.Right;
            var top = Vector3.Up;
            var bottom = Vector3.Down;

            size /= 2;

            // Initialize the Rectangle's data (Do not need vertex colors)
            return new VertexPositionNormalTexture[36]
                                {
                                    // Front Surface
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, size), front, bottomLeft),
                                    new VertexPositionNormalTexture(new Vector3(-size, size, size), front, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, -size, size), front, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(size, -size, size), front, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(-size, size, size), front, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, size, size), front, topRight),

                                    // Back Surface
                                    new VertexPositionNormalTexture(new Vector3(size, -size, -size), back, bottomLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, size, -size), back, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, -size), back, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, -size), back, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(size, size, -size), back, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(-size, size, -size), back, topRight),

                                    // Left Surface
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, -size), left, bottomLeft),
                                    new VertexPositionNormalTexture(new Vector3(-size, size, -size), left, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, size), left, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, size), left, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(-size, size, -size), left, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(-size, size, size), left, topRight),

                                    // Right Surface
                                    new VertexPositionNormalTexture(new Vector3(size, -size, size), right, bottomLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, size, size), right, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, -size, -size), right, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(size, -size, -size), right, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(size, size, size), right, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, size, -size), right, topRight),

                                    // Top Surface
                                    new VertexPositionNormalTexture(new Vector3(-size, size, size), top, bottomLeft),
                                    new VertexPositionNormalTexture(new Vector3(-size, size, -size), top, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, size, size), top, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(size, size, size), top, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(-size, size, -size), top, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, size, -size), top, topRight),

                                    // Bottom Surface
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, -size), bottom, bottomLeft),
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, size), bottom, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, -size, -size), bottom, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(size, -size, -size), bottom, bottomRight),
                                    new VertexPositionNormalTexture(new Vector3(-size, -size, size), bottom, topLeft),
                                    new VertexPositionNormalTexture(new Vector3(size, -size, size), bottom, topRight),
                                };

        }
    }
}
