using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame
{
    public enum Direction
    {
        UP = 270,
        DOWN = 90,
        LEFT = 180,
        RIGHT = 0
    }

    public class Section
    {
        public Direction direction;
        public int xCoord;
        public int yCoord;

        public Section(int xCoord, int yCoord, Direction direction)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.direction = direction;
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Texture2D snakeHead;
        Texture2D snakeBody;
        Texture2D snakeTurn;
        Texture2D snakeTail;

        private static List<Section> Snake = new List<Section>();

        private static Direction keyDirection = Direction.RIGHT;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Snake.Add(new Section(4, 2, Direction.RIGHT)); // the head
            Snake.Add(new Section(1, 2, Direction.RIGHT)); // the tail 
            Task task = UpdateSnake(TimeSpan.FromSeconds(0.25));
            base.Initialize();
        }

        async Task UpdateSnake(TimeSpan timeSpan)
        {
            var timer = new PeriodicTimer(timeSpan);
            while (await timer.WaitForNextTickAsync())
            {
                if (keyDirection != Snake[0].direction)
                {
                    Section newSection = new Section(Snake[0].xCoord, Snake[0].yCoord, keyDirection);
                    Snake.Insert(1, newSection);
                    Snake[0].direction = keyDirection;
                }
                UpdateHeadTail(Snake[0].direction, 0);
                UpdateHeadTail(Snake[^1].direction, Snake.Count - 1);
                if (Snake[^1].xCoord == Snake[^2].xCoord && Snake[^1].yCoord == Snake[^2].yCoord)
                {
                    Snake.Remove(Snake[^1]);
                }
            }
        }

        private void UpdateHeadTail(Direction orientation, int index)
        {
            switch (orientation)
            {
                case Direction.UP:
                    Snake[index].yCoord--;
                    break;
                case Direction.DOWN:
                    Snake[index].yCoord++;
                    break;
                case Direction.LEFT:
                    Snake[index].xCoord--;
                    break;
                case Direction.RIGHT:
                    Snake[index].xCoord++;
                    break;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            snakeHead = Content.Load<Texture2D>("snakeHead");
            snakeBody = Content.Load<Texture2D>("snakeBody");
            snakeTurn = Content.Load<Texture2D>("snakeTurn");
            snakeTail = Content.Load<Texture2D>("snakeTail");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up) && Snake[0].direction != Direction.DOWN)
            {
                keyDirection = Direction.UP;
            }
            if (keyState.IsKeyDown(Keys.Down) && Snake[0].direction != Direction.UP)
            {
                keyDirection = Direction.DOWN;
            }
            if (keyState.IsKeyDown(Keys.Left) && Snake[0].direction != Direction.RIGHT)
            {
                keyDirection = Direction.LEFT;
            }
            if (keyState.IsKeyDown(Keys.Right) && Snake[0].direction != Direction.LEFT)
            {
                keyDirection = Direction.RIGHT;
            }
            base.Update(gameTime);
        }

        private void UpdateXYpos(Direction orientation, ref int xPos, ref int yPos)
        {
            switch (orientation)
            {
                case Direction.UP:
                    yPos++;
                    break;
                case Direction.DOWN:
                    yPos--;
                    break;
                case Direction.LEFT:
                    xPos++;
                    break;
                case Direction.RIGHT:
                    xPos--;
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Chartreuse);
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            for (int i = 0; i < Snake.Count; i++)
            {
                int repeat = GetRepeat(i);
                int xPos = Snake[i].xCoord;
                int yPos = Snake[i].yCoord;
                do
                {
                    Texture2D texture = GetTexture(i, xPos, yPos);
                    _spriteBatch.Draw(
                        texture,
                        new Vector2((xPos * 64), (yPos * 64)),
                        null,
                        Color.White,
                        MathHelper.ToRadians((int)Snake[i].direction),
                        new Vector2(32, 32),
                        Vector2.One,
                        SpriteEffects.None,
                        0
                    );
                    UpdateXYpos(Snake[i].direction, ref xPos, ref yPos);
                    repeat--;
                } while (repeat > 0);
            }


            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private Texture2D GetTexture(int index, int xPos, int yPos)
        {
            if (index == 0 && xPos == Snake[0].xCoord && yPos == Snake[0].yCoord)
            {
                return snakeHead;
            }else if (index == Snake.Count - 1)
            {
                return snakeTail;
            }else if (xPos == Snake[index].xCoord && yPos == Snake[index].yCoord)
            {
                return snakeTurn;
            }else
            {
                return snakeBody;
            }
        }

        private int GetRepeat(int index)
        {
            if (index == Snake.Count - 1)
            {
                return 0;
            }
            else
            {
                int i = 0;
                if (Snake[i].direction == Direction.UP || Snake[i].direction == Direction.DOWN)
                {
                    int y1 = Snake[i].yCoord;
                    int y2 = Snake[i + 1].yCoord;
                    i = y1 > y2 ? y1 - y2 : y2 - y1;
                }
                else
                {
                    int x1 = Snake[i].xCoord;
                    int x2 = Snake[i + 1].xCoord;
                    i = x1 > x2 ? x1 - x2 : x2 - x1;
                }
                return i;
            }
        }
    }
}