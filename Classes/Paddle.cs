using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BreakOutGame
{
    public class Paddle
    {
        private Texture2D texture;
        public Vector2 Position;
        private Game1 game;
        private float paddleSpeed = 5.0f;  // Speed of paddle movement

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);

        public Paddle(Game1 game)
        {
            this.game = game;
            Position = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 45, 570); // Centered initially
        }

        public void LoadContent()
        {
            texture = game.Content.Load<Texture2D>("player");
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Use both mouse and keyboard for movement
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                Position.X -= paddleSpeed;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                Position.X += paddleSpeed;
            }
            else
            {
                // Follow the mouse with some constraints to avoid erratic jumps
                Position.X += (mouse.X - Position.X) * 0.1f;
            }

            // Clamp Position to keep the paddle on screen
            Position.X = Math.Clamp(Position.X, 0, game.GraphicsDevice.Viewport.Width - texture.Width);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }
    }
}
