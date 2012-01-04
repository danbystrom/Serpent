using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serpent
{
    public class Data
    {
        public static Data Instance;

        public readonly PlayingField PlayingField;
        public readonly PlayerSerpent PlayerSerpent;
        public readonly List<EnemySerpent> Enemies = new List<EnemySerpent>();

        public KeyboardState KeyboardState;
        public KeyboardState PrevKeyboardState;

        public Data( Game game1 )
        {
            var texture = game1.Content.Load<Texture2D>(@"Textures\grass");

            PlayingField = new PlayingField(
                game1.GraphicsDevice,
                texture,
                1, 25, 21);

            PlayerSerpent = new PlayerSerpent(
                game1,
                PlayingField,
                game1.Content.Load<Model>(@"Models\SerpentHead"),
                game1.Content.Load<Model>(@"Models\serpentsegment"));
            game1.Components.Add(PlayerSerpent);

            for (var i = 0; i < 5; i++)
            {
                var enemy = new EnemySerpent(
                    game1,
                    PlayingField,
                    game1.Content.Load<Model>(@"Models\SerpentHead"),
                    game1.Content.Load<Model>(@"Models\serpentsegment"),
                    PlayerSerpent.Camera,
                    new Whereabouts(0, new Point(20, 0), Direction.West),
                    i);
                Enemies.Add(enemy);
                game1.Components.Add(enemy);
            }

            Instance = this;
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

    }
}
