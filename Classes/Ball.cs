using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakOutGame
{
    public class Ball
    {
        private Texture2D texture;
        private Vector2 position;
        public Vector2 Velocity;
        private Vector2 scale;
        private Game1 game;

        public float Speed = 6;

        // Public property to get and set position
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }  // Add a setter if you need to modify Position from outside
        }

        // Add a public property for Radius
        public float Radius => texture.Width / 2 * scale.X;  // Assuming the texture is a square

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, (int)(texture.Width * scale.X), (int)(texture.Height * scale.Y));

        public Ball(Game1 game)
        {
            this.game = game;
            this.Position = new Vector2(230, 558); // Set initial Position
            this.Velocity = new Vector2(Speed, -Speed); // Use Speed for consistency
            this.scale = new Vector2(1f, 1f); // Set initial scale
        }

        public void LoadContent()
        {
            texture = game.Content.Load<Texture2D>("ball");
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity;

            // Check for collision with the screen bounds
            int textureScaledWidth = (int)(texture.Width * scale.X);
            int textureScaledHeight = (int)(texture.Height * scale.Y);
            if (Position.X < 0 || Position.X > game.GraphicsDevice.Viewport.Width - textureScaledWidth)
                Velocity.X = -Velocity.X;
            if (Position.Y < 0)
                Velocity.Y = -Velocity.Y;

            // Ball dropping below the screen (let Game1 handle game over)
            if (Position.Y > game.GraphicsDevice.Viewport.Height)
            {
                game.BallLost();  // Notify the game that the ball was lost
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void ReverseY()
        {
            Velocity.Y = -Velocity.Y;
        }
    }
}
