using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serpent
{
    class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;

        public BasicModel(Model m)
        {
            model = m;
        }

        public virtual void Update()
        {

        }

        public void Draw(Camera camera)
        {
            var transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            for (var i = 0; i < 20; i++ )
                foreach (var mesh in model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = camera.Projection;
                        be.View = camera.View;
                        be.World = GetWorld() *
                            mesh.ParentBone.Transform *
                            Matrix.CreateRotationZ(MathHelper.Pi) *
                            Matrix.CreateScale(0.15f) *
                            Matrix.CreateTranslation(i, 0, -1);
                    }
                    mesh.Draw();
                }

            for (var i = 0; i < 20; i++)
                foreach (var mesh in model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = camera.Projection;
                        be.View = camera.View;
                        be.World = GetWorld() *
                            mesh.ParentBone.Transform *
                            Matrix.CreateRotationZ(MathHelper.Pi) *
                            Matrix.CreateRotationY(MathHelper.Pi) *
                            Matrix.CreateScale(0.15f) *
                            Matrix.CreateTranslation(i+0.5f, 0, -1);
                    }
                    mesh.Draw();
                }

        }

        public virtual Matrix GetWorld()
        {
            return world;
        }
    }
}
