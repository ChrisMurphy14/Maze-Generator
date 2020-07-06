//////////////////////////////////////////////////
// File:				Game1.cs
// Author:				Chris Murphy
// Date Created:		06/11/2014
// Date Last Edited:	16/11/2015
//////////////////////////////////////////////////
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

// The namespace used to contain the code for the Maze Generator project.
namespace Maze_Generator
{
    public class Game1 : Game
    {
        // The constructor for the Game1 class.
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);

            // Sets the size of the game window.
            graphics.PreferredBackBufferWidth = 1190;
            graphics.PreferredBackBufferHeight = 690;

            // Sets the folder name for the content root directory.
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        // Used to initialise objects which don't require loading content.
        protected override void Initialize()
        {
            random = new Random();

            firstBlendColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), 255);
            secondBlendColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), 255);
            wallColor = Color.Black;
            mazeOrigin = Vector2.Zero;
            infoVisible = true;
            mazeWallsVisible = true;
            mazeCoordinateSize = 20;
            mazeWidth = 10;
            mazeHeight = 10;
            mazeWallThickness = 10;

            base.Initialize();
        }

        // Used to initialise objects which require loading content.
        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("Verdana");
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // The texture used to draw the objects within the Maze, initialised to be a single white pixel.
            texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData<Color>(new Color[] { Color.White });

            StartGeneratingMaze();
        }

        // Used to unload content before the application exits.
        protected override void UnloadContent() { }

        // Used to update the objects within the application.
        protected override void Update(GameTime gameTime)
        {
            // Updates the keyboard input states.
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            UpdateMazeInputs();

            // If the maze has been initialised and is being generated, updates its generation.
            if (maze != null && maze.CurrentGenerationState == Maze.GenerationState.BeingGenerated)
                maze.UpdateGeneration();

            base.Update(gameTime);
        }

        // Used to draw the objects within the application.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            if (maze != null)
            {
                // Draws the maze according to whether or not the walls are to be visible.
                if (mazeWallsVisible)
                    maze.Draw(firstBlendColor, secondBlendColor, wallColor, spriteBatch);
                else
                    maze.Draw(firstBlendColor, secondBlendColor, Color.Transparent, spriteBatch);
            }

            // If it is currently visible, draws the input instructions for controlling the maze.
            if (infoVisible)
            {
                spriteBatch.DrawString(font, "Coordinate size (G/T): " + mazeCoordinateSize.ToString(), new Vector2(5, 0), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Width (Left/Right): " + mazeWidth.ToString(), new Vector2(5, 25), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Height (Down/Up): " + mazeHeight.ToString(), new Vector2(5, 50), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Wall thickness (H/Y): " + mazeWallThickness.ToString(), new Vector2(5, 75), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Walls visible (E): " + mazeWallsVisible.ToString(), new Vector2(5, 100), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Generation origin coordinate (W/A/S/D): " + mazeOrigin.ToString(), new Vector2(5, 125), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Generate (Space)", new Vector2(5, 150), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Reset to default values (R)", new Vector2(5, 175), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Randomise generation origin coordinate (F)", new Vector2(5, 200), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Randomise first blend color (Z)", new Vector2(5, 225), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Randomise second blend color (X)", new Vector2(5, 250), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Randomise wall color (C)", new Vector2(5, 275), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Set wall color to black (V)", new Vector2(5, 300), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Toggle info display (Q)", new Vector2(5, 325), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Exit (Esc)", new Vector2(5, 350), Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // The first blend color of the maze which is strongest at the origin node.
        private Color firstBlendColor;
        // The second blend color of the maze which is strongest at end of the path which reaches furthest away from the origin of the node.
        private Color secondBlendColor;
        // The color used to draw the walls of the maze.
        private Color wallColor;
        // Used to manage the graphics context of the application (the game window).
        private GraphicsDeviceManager graphics;
        // The current state of the keyboard inputs.
        private KeyboardState currentKeyboardState;
        // The state of the keyboard inputs during the previous update.
        private KeyboardState previousKeyboardState;
        // The maze generated by the generator.
        private Maze maze;
        // A random number generator used to choose the colors of the maze.
        private Random random;
        // Used to draw textures on the window.
        private SpriteBatch spriteBatch;
        // The font used to draw text on the window.
        private SpriteFont font;
        // The texture used to draw the components of the maze.
        private Texture2D texture;
        // The generation origin coordinate within the maze.
        private Vector2 mazeOrigin;
        // Whether or not the maze info display is visible.
        private bool infoVisible;
        // Whether or not the walls of the maze are to be visible when drawn.
        private bool mazeWallsVisible;
        // The size of the coordinates within the maze.
        private uint mazeCoordinateSize;
        // The width of the maze in coordinates.
        private uint mazeWidth;
        // The height of the maze in coordinates.
        private uint mazeHeight;
        // The thickness of the maze walls.
        private uint mazeWallThickness;

        // Starts generating the maze using the current maze values..
        private void StartGeneratingMaze()
        {           
            // The width in pixels of the maze that will be generated with the current maze values.
            uint mazeDrawWidth = mazeWidth * (mazeCoordinateSize - mazeWallThickness) + mazeWallThickness * (mazeWidth + 1);
            // The height in pixels of the maze that will be generated with the current maze values.
            uint mazeDrawHeight = mazeHeight * (mazeCoordinateSize - mazeWallThickness) + mazeWallThickness * (mazeHeight + 1);
            // The position of the upper-left point of the maze on the window, set so that it is centered.
            Vector2 mazePositionOnWindow = new Vector2(graphics.PreferredBackBufferWidth / 2 - mazeDrawWidth / 2, graphics.PreferredBackBufferHeight / 2 - mazeDrawHeight / 2);

            maze = new Maze(texture, mazePositionOnWindow, mazeCoordinateSize, mazeCoordinateSize + mazeWallThickness, mazeWallThickness);
            maze.StartGenerating(mazeOrigin, mazeWidth, mazeHeight, 1);
        }

        // Updates the values of the maze according to the current input states.
        private void UpdateMazeInputs()
        {
            // If the 'Escape' key has just been pressed, exits the application.
            if (currentKeyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
                Exit();

            // If the 'R' key has just been pressed, resets the maze to its default values.
            if (currentKeyboardState.IsKeyDown(Keys.R) && previousKeyboardState.IsKeyUp(Keys.R))
            {
                mazeOrigin = Vector2.Zero;
                mazeWallsVisible = true;
                mazeCoordinateSize = 20;
                mazeWidth = 10;
                mazeHeight = 10;
                mazeWallThickness = 10;

                StartGeneratingMaze();
            }

            // If the 'Space' key has just been pressed, sets the maze to begin generating.
            if (currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
                StartGeneratingMaze();

            UpdateMazeSizeInputs();
            UpdateMazeVisibilityInputs();
            UpdateMazeOriginInputs();
            UpdateMazeColorInputs();
        }

        // Updates the maze according to the keyboard inputs which control the size of its components.
        private void UpdateMazeSizeInputs()
        {
            // If the 'Left' key has just been pressed, lowers the width of the maze to a minimum of 1 and sets the maze to begin generating.
            if (currentKeyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left))
            {
                if (mazeWidth > 1)
                {
                    mazeWidth--;

                    // Updates the origin coordinate if it no longer fits within the maze.
                    if (mazeOrigin.X > mazeWidth - 1)
                        mazeOrigin.X = mazeWidth - 1;

                    StartGeneratingMaze();
                }
            }
            // If the 'Right' key has just been pressed, increases the width of the maze and sets the maze to begin generating.
            if (currentKeyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right))
            {
                mazeWidth++;
                StartGeneratingMaze();
            }
            // If the 'Down' key has just been pressed, lowers the height of the maze to a minimum of 1 and sets the maze to begin generating.
            if (currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
            {
                if (mazeHeight > 1)
                {
                    mazeHeight--;

                    // Updates the origin coordinate if it no longer fits within the maze.
                    if (mazeOrigin.Y > mazeHeight - 1)
                        mazeOrigin.Y = mazeHeight - 1;

                    StartGeneratingMaze();
                }
            }
            // If the 'Up' key has just been pressed, increases the height of the maze and sets the maze to begin generating.
            if (currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
            {
                mazeHeight++;
                StartGeneratingMaze();
            }

            // If the 'G' key has just been pressed, shrinks the size of the maze coordinates with a minimum of 1.
            if (currentKeyboardState.IsKeyDown(Keys.G) && previousKeyboardState.IsKeyUp(Keys.G))
            {
                if (mazeCoordinateSize > 1)
                {
                    mazeCoordinateSize--;
                    StartGeneratingMaze();
                }
            }
            // If the 'T' key has just been pressed, grows the size of the maze coordinates.
            if (currentKeyboardState.IsKeyDown(Keys.T) && previousKeyboardState.IsKeyUp(Keys.T))
            {
                mazeCoordinateSize++;
                StartGeneratingMaze();
            }

            // If the 'H' key has just been pressed, shrinks the maze wall thickness with a minimum value of 1.
            if (currentKeyboardState.IsKeyDown(Keys.H) && previousKeyboardState.IsKeyUp(Keys.H))
            {
                if (mazeWallThickness > 1)
                {
                    mazeWallThickness--;
                    StartGeneratingMaze();
                }
            }
            // If the 'Y' key has just been pressed, the maze wall thickness.
            if (currentKeyboardState.IsKeyDown(Keys.Y) && previousKeyboardState.IsKeyUp(Keys.Y))
            {
                mazeWallThickness++;
                StartGeneratingMaze();
            }
        }

        // Updates the maze according to the keyboard inputs which control the visibility of its components.
        private void UpdateMazeVisibilityInputs()
        {
            // If the 'E' key has just been pressed, toggles the visibility of the maze walls when drawn.
            if (currentKeyboardState.IsKeyDown(Keys.E) && previousKeyboardState.IsKeyUp(Keys.E))
            {
                if (mazeWallsVisible)
                    mazeWallsVisible = false;
                else
                    mazeWallsVisible = true;
            }

            // If the 'Q' key has just been pressed, toggles whether the info display is visible.
            if (currentKeyboardState.IsKeyDown(Keys.Q) && previousKeyboardState.IsKeyUp(Keys.Q))
            {
                if (infoVisible)
                    infoVisible = false;
                else
                    infoVisible = true;
            }
        }

        // Updates the maze according to the keyboard inputs which control the position of the generation origin coordinate.
        private void UpdateMazeOriginInputs()
        {
            // If the 'A' key has just been pressed, moves the origin coordinate one space to the left if possible.
            if (currentKeyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyUp(Keys.A))
            {
                if (mazeOrigin.X > 0)
                {
                    mazeOrigin.X--;
                    StartGeneratingMaze();
                }
            }
            // If the 'D' key has just been pressed, moves the origin coordinate one space to the right if possible.
            if (currentKeyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyUp(Keys.D))
            {
                if (mazeOrigin.X < mazeWidth - 1)
                {
                    mazeOrigin.X++;
                    StartGeneratingMaze();
                }
            }
            // If the 'W' key has just been pressed, moves the origin coordinate one space upwards if possible.
            if (currentKeyboardState.IsKeyDown(Keys.W) && previousKeyboardState.IsKeyUp(Keys.W))
            {
                if (mazeOrigin.Y > 0)
                {
                    mazeOrigin.Y--;
                    StartGeneratingMaze();
                }
            }
            // If the 'S' key has just been pressed, moves the origin coordinate one space downwards if possible.
            if (currentKeyboardState.IsKeyDown(Keys.S) && previousKeyboardState.IsKeyUp(Keys.S))
            {
                if (mazeOrigin.Y < mazeHeight - 1)
                {
                    mazeOrigin.Y++;
                    StartGeneratingMaze();
                }
            }

            // If the 'F' key has just been pressed, randomises the position of the generation origin.
            if (currentKeyboardState.IsKeyDown(Keys.F) && previousKeyboardState.IsKeyUp(Keys.F))
            {
                mazeOrigin = new Vector2(random.Next((int)mazeWidth), random.Next((int)mazeHeight));

                StartGeneratingMaze();
            }
        }

        // Updates the maze according to the keyboard inputs which control the draw color of its components.
        private void UpdateMazeColorInputs()
        {
            // If the 'Z' key has just been pressed, randomises the first blend color.
            if (currentKeyboardState.IsKeyDown(Keys.Z) && previousKeyboardState.IsKeyUp(Keys.Z))
                firstBlendColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), 255);
            // If the 'X' key has just been pressed, randomises the second blend color.
            if (currentKeyboardState.IsKeyDown(Keys.X) && previousKeyboardState.IsKeyUp(Keys.X))
                secondBlendColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), 255);

            // If the 'C' key has just been pressed, randomises the wall color.
            if (currentKeyboardState.IsKeyDown(Keys.C) && previousKeyboardState.IsKeyUp(Keys.C))
                wallColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), 255);

            // If the 'V' key has just been pressed, sets the wall color to black.
            if (currentKeyboardState.IsKeyDown(Keys.V) && previousKeyboardState.IsKeyUp(Keys.V))
                wallColor = Color.Black;   
        }
    }
}
