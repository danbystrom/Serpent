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
        // Game camera
        private GraphicsDeviceManager _graphics;
        Camera _camera;

        // Vertex data
        VertexBuffer _vertexBuffer1;
        private PlayingField _playingField;

        // Effect
        BasicEffect effect;

        // Movement and rotation stuff
        readonly Matrix worldTranslation = Matrix.Identity;
        Matrix _worldRotation = Matrix.Identity;

        // Texture info
        Texture2D _texture1;
        Texture2D _texture2;

        private Serpent _serpent;
        private Serpent _serpentEnemy;

        private ModelManager _modelManager;

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
            // Initialize camera
            _camera = new Camera(
                this,
                new Vector3(0, 20, 2),
                Vector3.Zero,
                Vector3.Up );
            Components.Add(_camera);

            _playingField = new PlayingField(GraphicsDevice, 2, 20, 20);
            _serpent = new Serpent(this,
                    _playingField,
                    Content.Load<Model>(@"Models\SerpentHead"),
                    Content.Load<Model>(@"Models\serpentsegment"),
                    _camera);
            Components.Add(_serpent);
            _camera._serpent = _serpent;

            _serpentEnemy = new EnemySerpent(this,
                _playingField,
                Content.Load<Model>(@"Models\SerpentHead"),
                Content.Load<Model>(@"Models\serpentsegment"),
                _camera);
            Components.Add(_serpentEnemy);

            _modelManager = new ModelManager(this,_camera);
            Components.Add(_modelManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Initialize vertices
            var verts1 = new VertexPositionTexture[6];
            verts1[0] = new VertexPositionTexture( new Vector3(-1, 3, 0), new Vector2(0, 0));
            verts1[1] = new VertexPositionTexture(new Vector3(1, 3, 0), new Vector2(1, 0));
            verts1[2] = new VertexPositionTexture( new Vector3(-1, 1, 0), new Vector2(0, 0.5f));
            verts1[3] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0.5f));

            verts1[4] = new VertexPositionTexture(new Vector3(-1, 1, 2), new Vector2(0, 1));
            verts1[5] = new VertexPositionTexture(new Vector3(1, 1, 2), new Vector2(1, 1));

            // Set vertex data in VertexBuffer
            _vertexBuffer1 = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), verts1.Length, BufferUsage.None);
            _vertexBuffer1.SetData(verts1);

            // Initialize the BasicEffect
            effect = new BasicEffect(GraphicsDevice);

            // Load texture
            _texture1 = Content.Load<Texture2D>(@"Textures\trees");
            _texture2 = Content.Load<Texture2D>(@"Textures\grass");

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

            // Rotation
            _worldRotation *= Matrix.CreateFromYawPitchRoll(
                MathHelper.PiOver4/(60*10),
                0,
                0);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Set object and camera info
            effect.World = _worldRotation * worldTranslation * _worldRotation;
            effect.View = _camera.View;
            effect.Projection = _camera.Projection;
            effect.GraphicsDevice.SamplerStates[0] = new SamplerState() { Filter = TextureFilter.Linear };

             effect.Texture = _texture1;
             effect.TextureEnabled = true;
             // Set the vertex buffer on the GraphicsDevice
            GraphicsDevice.SetVertexBuffer(_vertexBuffer1);
            // Begin effect and draw for each pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
//                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);
            }

            effect.World = Matrix.Identity;
            effect.Texture = _texture2;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _playingField.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
