using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    public class Data : IDisposable
    {
        public static Data Instance;

        public static PlayingField PlayingField;
        public readonly PlayerSerpent PlayerSerpent;
        public readonly List<EnemySerpent> Enemies = new List<EnemySerpent>();

        public KeyboardState KeyboardState;
        public KeyboardState PrevKeyboardState;

        public Data( Game game1 )
        {
            if ( Instance != null )
                Instance.Dispose();
            Instance = this;

            var texture = game1.Content.Load<Texture2D>(@"Textures\grass");

            if ( PlayingField == null )
                PlayingField = new PlayingField(
                    game1.GraphicsDevice,
                    texture );

            var serpentHead = new ModelWrapper( game1, game1.Content.Load<Model>(@"Models\SerpentHead") );
            var serpentSegment = new ModelWrapper( game1, game1.Content.Load<Model>(@"Models\serpentsegment") );

            PlayerSerpent = new PlayerSerpent(
                game1,
                PlayingField,
                serpentHead,
                serpentSegment);

            for (var i = 0; i < 5; i++)
            {
                var enemy = new EnemySerpent(
                    game1,
                    PlayingField,
                    serpentHead,
                    serpentSegment,
                    PlayerSerpent.Camera,
                    new Whereabouts(0, new Point(20, 0), Direction.West),
                    i);
                Enemies.Add(enemy);
            }
        }

        public void UpdateKeyboard()
        {
            PrevKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
        }

        public bool HasKeyToggled( Keys key )
        {
            return KeyboardState.IsKeyDown(key) && PrevKeyboardState.IsKeyUp(key);
        }

        public void Dispose()
        {
            PlayingField.Dispose();
        }

    }

}
