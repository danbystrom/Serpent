using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serpent
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ModelManager : DrawableGameComponent
    {
        // List of models
        private readonly List<BasicModel> _models = new List<BasicModel>();
        private readonly Camera _camera;

        public ModelManager(Game game, Camera camera)
            : base(game)
        {
            _camera = camera;
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

        protected override void LoadContent()
        {
            _models.Add(new TreesModel(
                Game.Content.Load<Model>(@"models\tree1")));
            _models.Add(new SunflowersModel(
                Game.Content.Load<Model>(@"models\sunflower3")));

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Loop through all models and call Update
            foreach (var t in _models)
            {
                t.Update();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Loop through and draw each model
            foreach (var bm in _models)
            {
                bm.Draw(_camera);
            }

            base.Draw(gameTime);
        }

    }
}
