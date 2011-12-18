using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Serpent
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Vertex data
        private PlayingField _playingField;

        private PlayerSerpent _serpent;
        private EnemySerpent _serpentEnemy;

        private ModelManager _modelManager;
        private GraphicsDeviceManager _graphics;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var texture = Content.Load<Texture2D>(@"Textures\grass");
            _playingField = new PlayingField(
                GraphicsDevice,
                texture,
                2, 20, 20);

            _serpent = new PlayerSerpent(this,
                    _playingField,
                    Content.Load<Model>(@"Models\SerpentHead"),
                    Content.Load<Model>(@"Models\serpentsegment"));
            Components.Add(_serpent);
            Components.Add(_serpent.Camera);

            _serpentEnemy = new EnemySerpent(this,
                _playingField,
                Content.Load<Model>(@"Models\SerpentHead"),
                Content.Load<Model>(@"Models\serpentsegment"),
                _serpent.Camera,0);
            Components.Add(_serpentEnemy);
            _serpentEnemy = new EnemySerpent(this,
                _playingField,
                Content.Load<Model>(@"Models\SerpentHead"),
                Content.Load<Model>(@"Models\serpentsegment"),
                _serpent.Camera,1);
            Components.Add(_serpentEnemy);
            _serpentEnemy = new EnemySerpent(this,
                _playingField,
                Content.Load<Model>(@"Models\SerpentHead"),
                Content.Load<Model>(@"Models\serpentsegment"),
                _serpent.Camera,2);
            Components.Add(_serpentEnemy);

            _modelManager = new ModelManager(this, _serpent.Camera);
            Components.Add(_modelManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _playingField.Draw(_serpent.Camera);
            base.Draw(gameTime);
        }
    }
}
