using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace BreakOutGame
{
    public class Brick
    {
        private Texture2D texture;
        public Vector2 position;
        private Game1 game;

        public Rectangle Bounds => new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

        public Brick(Game1 game, Vector2 position)
        {
            this.game = game;
            this.position = position;
        }

        public void LoadContent()
        {
            texture = game.Content.Load<Texture2D>("tile");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
