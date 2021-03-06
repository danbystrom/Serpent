using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private ModelManager _modelManager;
        private readonly GraphicsDeviceManager _graphics;

        private Data _data;
        private bool _paused;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            startGame();
            base.Initialize();
        }

        private void startGame()
        {
            Components.Clear();
            _data = new Data(this);
            _modelManager = new ModelManager(this, _data.PlayerSerpent.Camera);
            Components.Add(_modelManager);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
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
            base.Update(gameTime);

            _data.UpdateKeyboard();

            if ( _data.KeyboardState.IsKeyDown(Keys.Escape))
                this.Exit();
            if ( _data.HasKeyToggled(Keys.Enter) && _data.KeyboardState.IsKeyDown(Keys.LeftAlt))
            {
                _graphics.IsFullScreen ^= true;
                _graphics.ApplyChanges();
            }

            if (_data.HasKeyToggled(Keys.C) )
                _data.PlayerSerpent.Camera.CameraBehavior = _data.PlayerSerpent.Camera.CameraBehavior ==
                                                            CameraBehavior.FollowSerpent
                                                                ? CameraBehavior.Static
                                                                : CameraBehavior.FollowSerpent;
            if (_data.HasKeyToggled(Keys.F))
                _data.PlayerSerpent.Camera.CameraBehavior = _data.PlayerSerpent.Camera.CameraBehavior ==
                                                            CameraBehavior.FollowSerpent
                                                                ? CameraBehavior.FreeFlying
                                                                : CameraBehavior.FollowSerpent;
            if (_data.HasKeyToggled(Keys.P))
                _paused ^= true;

            if (_paused)
            {
                _data.PlayerSerpent.UpdateCameraOnly(gameTime);
                return;
            }

            _data.PlayerSerpent.Update(gameTime);
            foreach (var enemy in _data.Enemies )
            {
                enemy.Update(gameTime);
                if (enemy.EatAt(_data.PlayerSerpent))
                    startGame();
                else if (enemy.SerpentStatus == SerpentStatus.Alive && _data.PlayerSerpent.EatAt(enemy))
                    enemy.SerpentStatus = SerpentStatus.Ghost;
            }
            _data.Enemies.RemoveAll(e => e.SerpentStatus == SerpentStatus.Finished);
                    if ( _data.Enemies.Count==0 )
                        startGame();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //_data.PlayingField.Draw(_data.PlayerSerpent.Camera);
            Data.PlayingField.Draw(_data.PlayerSerpent.Camera);
            _data.PlayerSerpent.Draw(gameTime);
            foreach (var enemy in _data.Enemies)
                enemy.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
