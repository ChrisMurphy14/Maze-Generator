//////////////////////////////////////////////////
// File:				MazeNode.cs
// Author:				Chris Murphy
// Date Created:		06/11/2014
// Date Last Edited:	16/11/2015
//////////////////////////////////////////////////
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

// The namespace used to contain the code for the Maze Generator project.
namespace Maze_Generator
{
    // The class used to represent a potential path tile within a Maze.
    class MazeNode
    {     
        // The constructor for the MazeNode class.
        public MazeNode(Vector2 coordinateInMaze)
        {
            this.coordinateInMaze = coordinateInMaze;

            visited = false;
            walkDistance = 0;
        }

        // The property used to get the coordinate position of the MazeNode within the Maze.
        public Vector2 CoordinateInMaze
        {
            get { return coordinateInMaze; }
        }

        // The property used to get and set whether the MazeNode is displayed as visited or not.
        public bool Visited
        {
            get { return visited; }
            set { visited = value; }
        }

        // The property used to get and set the walk distance of the MazeNode.
        public uint WalkDistance
        {
            get { return walkDistance; }
            set { walkDistance = value; }
        }

        // Returns a list containing the unvisited nodes adjacent to this MazeNode within the Maze.
        public List<MazeNode> GetUnvisitedAdjacentNodes(Maze maze)
        {
            // The list to be returned.
            List<MazeNode> adjacentNodes = new List<MazeNode>();

            // Checks each of the nodes in the maze and adds them to the list if they are adjacent to this MazeNode.
            for (int i = 0; i < maze.Nodes.Count; ++i)
            {
                if (maze.Nodes[i].CoordinateInMaze.X == coordinateInMaze.X - 1 && maze.Nodes[i].CoordinateInMaze.Y == coordinateInMaze.Y ||
                    maze.Nodes[i].CoordinateInMaze.X == coordinateInMaze.X + 1 && maze.Nodes[i].CoordinateInMaze.Y == coordinateInMaze.Y ||
                    maze.Nodes[i].CoordinateInMaze.X == coordinateInMaze.X && maze.Nodes[i].CoordinateInMaze.Y == coordinateInMaze.Y - 1 ||
                    maze.Nodes[i].CoordinateInMaze.X == coordinateInMaze.X && maze.Nodes[i].CoordinateInMaze.Y == coordinateInMaze.Y + 1)
                    adjacentNodes.Add(maze.Nodes[i]);
            }

            // Removes any nodes from the list that have been visited.
            for (int i = adjacentNodes.Count - 1; i > -1; --i)
            {
                if (adjacentNodes[i].Visited)
                    adjacentNodes.RemoveAt(i);
            }

            return adjacentNodes;
        }
        
        // Draws the MazeNode on the game window.
        public void Draw(Color color, Maze maze, SpriteBatch spriteBatch, Texture2D texture)
        {
            // The horizontal position of the upper-leftmost point of the MazeNode on the game window.
            float nodeHorizontalWindowPosition = maze.PositionOnWindow.X + (coordinateInMaze.X * maze.CoordinateSize) + maze.CoordinateSize / 2 + maze.WallDrawThickness / 2 - maze.NodeDrawSize / 2;
            // The vertical position of the upper-leftmost point of the MazeNode on the game window.
            float nodeVerticalWindowPosition = maze.PositionOnWindow.Y + coordinateInMaze.Y * maze.CoordinateSize + maze.CoordinateSize / 2 + maze.WallDrawThickness / 2 - maze.NodeDrawSize / 2;

            // Draws the MazeNode on the game window.
            spriteBatch.Draw(texture, new Rectangle((int)nodeHorizontalWindowPosition, (int)nodeVerticalWindowPosition, (int)maze.NodeDrawSize, (int)maze.NodeDrawSize), color);
        }

        // The coordinate position of the MazeNode within the maze (e.g. (0, 0) is the upper-leftmost coordinate position in the maze).
        private Vector2 coordinateInMaze;        
        // Whether or not this MazeNode has been visited during the maze generation process.
        private bool visited;
        // The distance in coordinates which must be traveled (accounting for walls) from the origin node in the maze to this node.
        private uint walkDistance;
    }
}
