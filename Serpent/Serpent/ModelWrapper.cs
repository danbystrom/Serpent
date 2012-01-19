using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serpent
{
    public class ModelWrapper
    {
        private readonly Model _model;
        private readonly Matrix[] _bones;

        public ModelWrapper( Game game, Model model)
        {
            _model = model;
            _bones = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(_bones);

            foreach (var mesh in _model.Meshes)
                foreach (var part in mesh.MeshParts)
                    if (part.Effect is BasicEffect)
                    {
                        var oldBe = part.Effect as BasicEffect;
                        var newBe = new StockEffects.BasicEffect(game.GraphicsDevice);
                        newBe.CopyBasicEffect(oldBe);
                        newBe.SpecularPower = 32;
                        newBe.OriginalDiffuseColor = oldBe.DiffuseColor;
                        newBe.EnableDefaultLighting();
                        //newBe.DiffuseColor = oldBe.DiffuseColor;
                        //newBe.OriginalDiffuseColor = oldBe.DiffuseColor;
                        //newBe.EmissiveColor = oldBe.EmissiveColor;
                        //newBe.AmbientLightColor = oldBe.AmbientLightColor;
                        part.Effect = newBe;
                    }
        }

        public void Draw(
            Camera camera,
            Matrix world,
            Vector3 tintColor,
            float tintFactor,
            float alpha )
        {
                foreach (var mesh in _model.Meshes)
                {
                    foreach (StockEffects.BasicEffect e in mesh.Effects)
                    {
                        e.DiffuseColor = Vector3.Lerp(e.OriginalDiffuseColor, tintColor, tintFactor);
                        e.Alpha = alpha;
                        e.View = camera.View;
                        e.Projection = camera.Projection;
                        e.World = _bones[mesh.ParentBone.Index]*world;
                    }
                    mesh.Draw();
                }
        }

    }

}
