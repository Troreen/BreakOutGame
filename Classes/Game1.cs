using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace BreakOutGame
{

    public enum GameState
    {
        Menu,
        Playing,
        HighScores,
        GameOver
    }

    public class Game1 : Game
    {

        private GameState currentState;
        private List<Button> menuButtons;
        private List<Button> gameOverButtons;
        private SpriteFont buttonFont;


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game components
        private Paddle paddle;
        private Ball ball;
        private List<Brick> bricks;
        private int score;
        private List<int> highScores = new List<int>();
        private int maxLives = 1;
        private int lives;

        public int gameState = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800; 
            _graphics.PreferredBackBufferHeight = 600;  
        }

        protected override void Initialize()
        {
            paddle = new Paddle(this);
            ball = new Ball(this);
            bricks = new List<Brick>();
            lives = maxLives;
            score = 0;

            LoadHighScores();

            int brickWidth = 45;
            int brickHeight = 25;
            int numBricksX = _graphics.PreferredBackBufferWidth / brickWidth;
            int numBricksY = 9;

            for (int i = 0; i < numBricksY; i++)
            {
                for (int j = 0; j < numBricksX; j++)
                {
                    bricks.Add(new Brick(this, new Vector2(j * brickWidth, i * brickHeight)));
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            buttonFont = Content.Load<SpriteFont>("Arial");  // Ensure you have an Arial spritefont in your Content folder

            // Initialize menu buttons
            menuButtons = new List<Button>
            {
                new Button(GraphicsDevice, buttonFont, "Start Game", new Vector2(350, 200), () => { currentState = GameState.Playing; }),
                new Button(GraphicsDevice, buttonFont, "High Scores", new Vector2(350, 250), () => { currentState = GameState.HighScores; }),
                new Button(GraphicsDevice, buttonFont, "Exit", new Vector2(350, 300), Exit)
            };

            // Initialize game over buttons
            gameOverButtons = new List<Button>
            {
                new Button(GraphicsDevice, buttonFont, "Restart", new Vector2(350, 400), RestartGame),
                new Button(GraphicsDevice, buttonFont, "High Scores", new Vector2(350, 450), () => { currentState = GameState.HighScores; }),
                new Button(GraphicsDevice, buttonFont, "Exit", new Vector2(350, 500), Exit)
            };

            paddle.LoadContent();
            ball.LoadContent();
            bricks.ForEach(b => b.LoadContent());
        }


        protected override void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.Menu:
                    foreach (var button in menuButtons)
                        button.Update(Mouse.GetState());
                    break;
                case GameState.Playing:
                    paddle.Update(gameTime);
                    ball.Update(gameTime);
                    HandleCollisions();
                    break;
                case GameState.HighScores:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        currentState = GameState.Menu;  // Return to menu from high scores
                    break;
                case GameState.GameOver:
                    foreach (var button in gameOverButtons)
                        button.Update(Mouse.GetState());
                    break;
            }

            base.Update(gameTime);
        }




        private void HandleCollisions()
        {
            // Collision with paddle
            if (paddle.Bounds.Intersects(ball.Bounds))
            {
                // Calculate the hit position on the paddle
                float hitPosition = (ball.Position.X + ball.Radius) - (paddle.Position.X + paddle.Bounds.Width / 2);

                // Normalize hit position to [-1, 1] (0 is center, -1 is left end, 1 is right end)
                float normalizedPosition = hitPosition / (paddle.Bounds.Width / 2);

                // Adjust the reflection angle
                float bounceAngle = normalizedPosition * (MathHelper.PiOver4); // PiOver4 (45 degrees) can be adjusted for sharper angles

                // Update the ball's velocity
                ball.Velocity.X = ball.Speed * (float)Math.Cos(bounceAngle);
                ball.Velocity.Y = -Math.Abs(ball.Speed * (float)Math.Sin(bounceAngle)); // Always bounce up

                // Ensure the ball moves upward, not downward
                if (ball.Velocity.Y > 0)
                    ball.Velocity.Y *= -1;
            }

            // Collision with bricks
            for (int i = 0; i < bricks.Count; i++)
            {
                if (ball.Bounds.Intersects(bricks[i].Bounds))
                {
                    // Determine if collision is vertical or horizontal
                    Rectangle intersection = Rectangle.Intersect(ball.Bounds, bricks[i].Bounds);
                    if (intersection.Width >= intersection.Height)
                    {
                        ball.Velocity.Y = -ball.Velocity.Y; // Vertical bounce
                    }
                    else
                    {
                        ball.Velocity.X = -ball.Velocity.X; // Horizontal bounce
                    }

                    bricks.RemoveAt(i);
                    score += 10;
                    break;
                }
            }

            // Ball dropping below the screen
            if (ball.Position.Y > _graphics.PreferredBackBufferHeight)
            {
                lives--;
                if (lives <= 0)
                {
                    SaveHighScores();
                    currentState = GameState.GameOver;  // Game over
                }
                else
                {
                    // Reset ball position and velocity
                    ball.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
                    ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                }
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            switch (currentState)
            {
                case GameState.Menu:
                    // Draw each menu button
                    foreach (var button in menuButtons)
                        button.Draw(_spriteBatch);
                    break;
                case GameState.Playing:
                    paddle.Draw(_spriteBatch);
                    ball.Draw(_spriteBatch);
                    bricks.ForEach(b => b.Draw(_spriteBatch));

                    // Draw the score and lives within the playing state
                    string scoreText = $"Score: {score}";
                    string livesText = $"Lives: {lives}";
                    _spriteBatch.DrawString(Content.Load<SpriteFont>("Arial"), scoreText, new Vector2(20, 450), Color.White);
                    _spriteBatch.DrawString(Content.Load<SpriteFont>("Arial"), livesText, new Vector2(20, 500), Color.White);
                    break;
                    break;
                case GameState.HighScores:
                    DrawHighScores();
                    break;
                case GameState.GameOver:
                    DrawGameOverScreen();
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawHighScores()
        {
            // Load and display high scores
            string highScoresTitle = "High Scores:";
            _spriteBatch.DrawString(buttonFont, highScoresTitle, new Vector2(100, 100), Color.White);

            for (int i = 0; i < highScores.Count; i++)
            {
                _spriteBatch.DrawString(buttonFont, $"{i + 1}. {highScores[i]}", new Vector2(100, 150 + i * 30), Color.White);
            }
        }

        private void DrawGameOverScreen()
        {
            _spriteBatch.DrawString(buttonFont, "Game Over!", new Vector2(350, 200), Color.White);
            foreach (var button in gameOverButtons)
                button.Draw(_spriteBatch);
        }


        public void BallLost()
        {
            lives--;
            if (lives <= 0)
            {
                currentState = GameState.GameOver;  // Change state to Game Over
            }
            else
            {
                // Reset ball position and possibly paddle position
                ball.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 30);
                ball.Velocity = new Vector2(ball.Speed, -ball.Speed);  // Reset ball speed and direction
            }
        }


        private void LoadHighScores()
        {
            if (File.Exists("highscores.txt"))
            {
                var lines = File.ReadAllLines("highscores.txt");
                highScores = lines.Select(int.Parse).ToList();
            }
        }

        private void SaveHighScores()
        {
            highScores.Add(score);
            highScores = highScores.OrderByDescending(x => x).Take(10).ToList();
            string filePath = "C:\\Users\\bedir\\BreakOutGame\\bin\\highscores.txt";
            File.WriteAllLines(filePath, highScores.Select(x => x.ToString()));

            // Debug read
            var lines = File.ReadAllLines(filePath);
            Console.WriteLine("Currently saved scores:");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }



        private void RestartGame()
        {
            Initialize();  // Reinitialize the game or reset necessary components
            currentState = GameState.Playing;
        }

    }

    public class Button
    {
        public Rectangle Bounds;
        public string Text;
        public Action OnClick;

        private SpriteFont font;
        private Texture2D texture;

        public Button(GraphicsDevice graphicsDevice, SpriteFont font, string text, Vector2 position, Action onClick)
        {
            this.font = font;
            this.Text = text;
            this.OnClick = onClick;
            Vector2 dimensions = font.MeasureString(text);
            this.Bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X + 20, (int)dimensions.Y + 10);

            // Create a simple texture
            this.texture = new Texture2D(graphicsDevice, 1, 1);
            this.texture.SetData(new Color[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.Gray); // Button background
            spriteBatch.DrawString(font, Text, new Vector2(Bounds.X + 10, Bounds.Y + 5), Color.Black);
        }

        public void Update(MouseState mouse)
        {
            if (Bounds.Contains(mouse.X, mouse.Y) && mouse.LeftButton == ButtonState.Pressed)
            {
                OnClick?.Invoke();
            }
        }
    }

}
