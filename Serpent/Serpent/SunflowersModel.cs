using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serpent
{
    class SunflowersModel : BasicModel
    {

        public SunflowersModel(Model m) : base(m)
        {
        }

        public override void Update()
        {

        }

        public override void Draw(Camera camera)
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
