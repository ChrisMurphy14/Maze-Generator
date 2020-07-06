//////////////////////////////////////////////////
// File:				MazeWall.cs
// Author:				Chris Murphy
// Date Created:		06/11/2014
// Date Last Edited:	16/11/2015
//////////////////////////////////////////////////
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

// The namespace used to contain the code for the Maze Generator project.
namespace Maze_Generator
{
    // The class used to represent a wall between two MazeNodes.
    class MazeWall
    {
        // Enumerated values used to represent the orientation of the MazeWall.
        public enum MazeOrientation
        {
            Horizontal,
            Vertical
        }

        // The constructor for the MazeWall class.
        public MazeWall(Vector2 firstNodeCoordinate, Vector2 secondNodeCoordinate)
        {
            this.firstNodeCoordinate = firstNodeCoordinate;
            this.secondNodeCoordinate = secondNodeCoordinate;

            // Sets the orientation of the MazeWall.
            if (firstNodeCoordinate.X != secondNodeCoordinate.X)
                orientation = MazeOrientation.Horizontal;
            else
                orientation = MazeOrientation.Vertical;
        }

        // The property used to get the orientation of the MazeWall.
        public MazeOrientation Orientation
        {
            get { return orientation; }
        }

        // The property used to get the coordinate position of the first MazeNode of the two which the MazeWall separates.
        public Vector2 FirstNodeCoordinate
        {
            get { return firstNodeCoordinate; }
        }

        // The property used to get the coordinate position of the second MazeNode of the two which the MazeWall separates.
        public Vector2 SecondNodeCoordinate
        {
            get { return secondNodeCoordinate; }
        }

        // Draws the MazeWall on the game window using the specified texture.
        public void Draw(Color color, Maze maze, SpriteBatch spriteBatch, Texture2D texture)
        {
            // If the MazeWall is horizontal, draws the wall horizontally between the specified coordinates of the maze.
            if (orientation == MazeOrientation.Horizontal)
            {
                // The horizontal draw position of the wall on the window.
                int wallHorizontalDrawPosition = (int)(maze.PositionOnWindow.X + firstNodeCoordinate.X * maze.CoordinateSize + maze.CoordinateSize);
                // The vertical draw position of the wall on the window.
                int wallVerticalDrawPosition = (int)(maze.PositionOnWindow.Y + firstNodeCoordinate.Y * maze.CoordinateSize);

                spriteBatch.Draw(texture, new Rectangle(wallHorizontalDrawPosition, wallVerticalDrawPosition, (int)maze.WallDrawThickness, (int)(maze.CoordinateSize + maze.WallDrawThickness)), color);
            }
            // Else if the MazeWall is vertical, draws the wall vertically.
            else if (orientation == MazeOrientation.Vertical)
            {
                // The horizontal draw position of the wall on the window.
                int wallHorizontalDrawPosition = (int)(maze.PositionOnWindow.X + firstNodeCoordinate.X * maze.CoordinateSize);
                // The vertical draw position of the wall on the window.
                int wallVerticalDrawPosition = (int)(maze.PositionOnWindow.Y + firstNodeCoordinate.Y * maze.CoordinateSize + maze.CoordinateSize);

                spriteBatch.Draw(texture, new Rectangle(wallHorizontalDrawPosition, wallVerticalDrawPosition, (int)(maze.CoordinateSize + maze.WallDrawThickness), (int)maze.WallDrawThickness), color);
            }
        }

        // The orientation of the MazeWall.
        private MazeOrientation orientation;
        // The coordinate position within the maze of the first MazeNode of the two which are separated by the MazeWall.
        private Vector2 firstNodeCoordinate;
        // The coordinate position within the maze of the second MazeNode of the two which are separated by the MazeWall.
        private Vector2 secondNodeCoordinate;
    }
}
