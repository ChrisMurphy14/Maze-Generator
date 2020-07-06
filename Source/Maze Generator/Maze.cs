//////////////////////////////////////////////////
// File:				Maze.cs
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
    // The class used to contain a series of MazeNodes and MazeWalls which make up a maze.
    class Maze
    {
        // Enumerated values representing the possible generation states of the Maze.
        public enum GenerationState
        {
            NotGenerated,
            BeingGenerated,
            Generated
        }        

        // The constructor for the Maze class.
        public Maze(Texture2D texture, Vector2 positionOnWindow, uint coordinateSize, uint nodeDrawSize, uint wallDrawThickness)
        {
            this.texture = texture;
            this.positionOnWindow = positionOnWindow;
            this.coordinateSize = coordinateSize;
            this.nodeDrawSize = nodeDrawSize;
            this.wallDrawThickness = wallDrawThickness;            

            nodes = new List<MazeNode>();
            walls = new List<MazeWall>();
            generationState = GenerationState.NotGenerated;
        }
        
        // The property used to get the current generation state of the Maze.
        public GenerationState CurrentGenerationState
        {
            get { return generationState; }
        }

        // The property used to get a list containing the nodes within the Maze.
        public List<MazeNode> Nodes
        {
            get { return nodes; }
        }

        // The property used to get the position of the upper-leftmost point of the Maze on the game window.
        public Vector2 PositionOnWindow
        {
            get { return positionOnWindow; }
        }

        // The property used to get the width and height of the Maze in coordinates.
        public Vector2 SizeInCoordinates
        {
            get { return new Vector2(widthInCoordinates, heightInCoordinates); }
        }

        // The property used to get the size of each of the coordinate spaces within the Maze.
        public uint CoordinateSize
        {
            get { return coordinateSize; }
        }

        // The property used to get the draw size of the nodes of the Maze.
        public uint NodeDrawSize
        {
            get { return nodeDrawSize; }
        }

        // The property used to get the draw thickness of the walls of the Maze.
        public uint WallDrawThickness
        {
            get { return wallDrawThickness; }
        }

        // Sets the Maze to begin the generation process.
        public void StartGenerating(Vector2 generationOriginCoordinate, uint widthInCoordinates, uint heightInCoordinates, uint generationSpeed, int generationSeed = 0)
        {
            // Sets the generation origin coordinate to the parameter value if it is a valid coordinate within the Maze, else sets it to (0, 0).
            if (generationOriginCoordinate.X >= 0 && generationOriginCoordinate.X <= widthInCoordinates - 1 && generationOriginCoordinate.Y >= 0 && generationOriginCoordinate.Y <= heightInCoordinates - 1)
                this.generationOriginCoordinate = generationOriginCoordinate;
            else
                generationOriginCoordinate = Vector2.Zero;
            this.generationSeed = generationSeed;
            this.widthInCoordinates = widthInCoordinates;
            this.heightInCoordinates = heightInCoordinates;
            this.generationSpeed = generationSpeed;

            // Empties any nodes and walls from the list.
            nodes.Clear(); 
            walls.Clear();
            longestWalkDistance = 0;
            // Fills the Maze with the required amount of nodes and walls connecting each of them.
            for (int x = 0; x < widthInCoordinates; ++x)
            {
                for (int y = 0; y < heightInCoordinates; ++y)
                {
                    // Adds a node in the current position.
                    nodes.Add(new MazeNode(new Vector2(x, y)));

                    // If the current x-coordinate is not at the rightmost side of the Maze, adds a wall between the current coordinate and the one to the right.
                    if (x < widthInCoordinates - 1)
                        walls.Add(new MazeWall(new Vector2(x, y), new Vector2(x + 1, y)));

                    // If the current y-coordinate is not at the bottom side of the Maze, adds a wall between the current coordinate and the one to the bottom.
                    if (y < heightInCoordinates - 1)
                        walls.Add(new MazeWall(new Vector2(x, y), new Vector2(x, y + 1)));
                }
            }

            // Initialises the random number generator with either no seed or the specified seed depending on the generation seed value.
            if (generationSeed == 0)
                rand = new Random();
            else
                rand = new Random(generationSeed);

            // Initialises the queue used to hold the Maze nodes that are still to be visited.
            unvisitedNodesQueue = new Queue<MazeNode>();
            // Sets the current Maze node to be used in the Maze generation as the node which matches the generation origin coordinate.
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (nodes[i].CoordinateInMaze == generationOriginCoordinate)
                {
                    currentGenerationNode = nodes[i];
                    break;
                }
            }

            generationState = GenerationState.BeingGenerated;
        }

        // Updates the Maze generation if it is currently being generated.
        public void UpdateGeneration()
        {
            // The counter used to tell how many times the generation loop has looped.
            int generationCounter = 0;            
            // Continues generation while the Maze has not finished generating and the loop hasn't looped more times than the generation speed value.
            while (generationState == GenerationState.BeingGenerated && generationCounter < generationSpeed)
            {
                if (generationState == GenerationState.BeingGenerated)
                {
                    // Adds the current node to the nodes queue.
                    unvisitedNodesQueue.Enqueue(currentGenerationNode);
                    // Marks the current node as visited.
                    currentGenerationNode.Visited = true;

                    // A bool used to tell whether the nodes queue is empty and the generation loop should be broken.
                    bool endLoop = false;
                    // A while loop used to check if there are unvisited nodes adjacent to the current node and update the nodes queue accordingly.
                    while (true)
                    {
                        // Gets a list containing the unvisited nodes adjacent to the current node.
                        List<MazeNode> unvisitedAdjacentNodes = currentGenerationNode.GetUnvisitedAdjacentNodes(this);
                        // If there are no unvisited nodes adjacent to the current node, continues to pop nodes off the queue until a node fulfils these requirements and makes this node the current node.
                        if (unvisitedAdjacentNodes.Count == 0)
                        {
                            // A bool used to tell whether the outer generation loop should be continued (jumps back to the beginning of the loop).
                            bool continueLoop = false;
                            // While the nodes queue is still not empty, pops nodes off the queue until one is found with at least one unvisited adjacent node.
                            while (unvisitedNodesQueue.Count > 0)
                            {
                                // The node which has been popped off the beginning of the queue.
                                MazeNode tempNode = unvisitedNodesQueue.Dequeue();

                                // If the temp node has more than one unvisited adjacent node, sets it as the current generation node, sets the outer generation loop to be continued, and breaks the inner loop. 
                                if (tempNode.GetUnvisitedAdjacentNodes(this).Count > 0)
                                {
                                    currentGenerationNode = tempNode;
                                    continueLoop = true;
                                    break;
                                }
                            }
                            // If the continue loop bool is true, continues the outer generation loop.
                            if (continueLoop)
                                continue;
                            // Else, sets the outer generation loop to be broken.
                            else
                            {
                                endLoop = true;
                                break;
                            }
                        }
                        // Else, if there is at least one unvisited node adjacent to the current node breaks this inner loop.
                        else
                            break;
                    }
                    if (endLoop)
                        generationState = GenerationState.Generated;

                    // If the Maze is still being generated, removes the appropriate wall.
                    if (generationState == GenerationState.BeingGenerated)
                    {
                        // A list containing all of the univisited nodes adjacent to the current node.
                        List<MazeNode> adjacentNodes = currentGenerationNode.GetUnvisitedAdjacentNodes(this);
                        // Randomly selects one of the unvisited adjacent nodes of the current node.
                        MazeNode adjacentNode = adjacentNodes[rand.Next(0, adjacentNodes.Count)];
                        // Finds the wall between the current node and the adjacent node within the Maze and removes it.
                        for (int i = 0; i < walls.Count; ++i)
                        {
                            if (walls[i].FirstNodeCoordinate == currentGenerationNode.CoordinateInMaze && walls[i].SecondNodeCoordinate == adjacentNode.CoordinateInMaze ||
                                walls[i].FirstNodeCoordinate == adjacentNode.CoordinateInMaze && walls[i].SecondNodeCoordinate == currentGenerationNode.CoordinateInMaze)
                            {
                                walls.RemoveAt(i);

                                adjacentNode.WalkDistance = currentGenerationNode.WalkDistance + 1;
                                if (adjacentNode.WalkDistance > longestWalkDistance)
                                    longestWalkDistance = adjacentNode.WalkDistance;

                                break;
                            }
                        }
                        // Sets the current node to be the adjacent node.
                        currentGenerationNode = adjacentNode;
                    }

                    // If the generation has finished, adds walls around the sides of the Maze.
                    if (generationState == GenerationState.Generated)
                    {
                        // Adds walls to the left and right sides of the Maze.
                        for (int i = 0; i < heightInCoordinates; ++i)
                        {
                            // Adds the vertical walls on the left side of the Maze.
                            walls.Add(new MazeWall(new Vector2(-1, i), new Vector2(0, i)));
                            // Adds the vertical walls on the right side of the Maze.
                            walls.Add(new MazeWall(new Vector2(widthInCoordinates - 1, i), new Vector2(widthInCoordinates, i)));
                        }
                        // Adds walls to the top and bottom of the Maze.
                        for (int i = 0; i < widthInCoordinates; ++i)
                        {
                            // Adds the horizontal walls on the top side of the Maze.
                            walls.Add(new MazeWall(new Vector2(i, -1), new Vector2(i, 0)));
                            // Adds the horizontal walls on the bottom side of the Maze.
                            walls.Add(new MazeWall(new Vector2(i, heightInCoordinates - 1), new Vector2(i, heightInCoordinates)));
                        }
                    }
                }

                generationCounter++;
            }
        }

        // If it exists, removes the wall between the specified nodes within the Maze (must be either horizontally or vertically adjacent).
        public void RemoveWall(Vector2 firstNodeCoordinate, Vector2 secondNodeCoordinate)
        {
            for (uint i = 0; i < walls.Count; ++i)
            {
                if (walls[(int)i].FirstNodeCoordinate == firstNodeCoordinate && walls[(int)i].SecondNodeCoordinate == secondNodeCoordinate)
                    walls.RemoveAt((int)i);
            }
        }

        // Draws the objects that make up the Maze on the game window using the specified texture.
        public void Draw(Color coordinateOriginBlendColor, Color coordinateDistanceBlendColor, Color wallColor, SpriteBatch spriteBatch)
        {
            // Draws all of the nodes within the Maze.
            for (int i = 0; i < nodes.Count; ++i)
            {
                // The lerp value used to blend between the two colors of the maze depending on how far the current node is away from the origin relative to the furthest node from the origin.
                float lerpValue = ((float)nodes[i].WalkDistance + 1.0f) / longestWalkDistance;

                // Uses this lerp value to blend between the two maze colors.
                Color drawColor = new Color(MathHelper.Lerp(coordinateOriginBlendColor.R, coordinateDistanceBlendColor.R, lerpValue) / 255, MathHelper.Lerp(coordinateOriginBlendColor.G, 
                    coordinateDistanceBlendColor.G, lerpValue) / 255, MathHelper.Lerp(coordinateOriginBlendColor.B, coordinateDistanceBlendColor.B, lerpValue) / 255);

                // Draws the current node with the resulting color.
                nodes[i].Draw(drawColor, this, spriteBatch, texture);
            }

            // Draws all of the walls within the Maze.
            for (int i = 0; i < walls.Count; ++i)
                walls[i].Draw(wallColor, this, spriteBatch, texture);
        }
        
        // The current generation state of the Maze.
        private GenerationState generationState;
        // A list containing each of the nodes within the Maze.
        private List<MazeNode> nodes;
        // A list containing each of the walls within the Maze.
        private List<MazeWall> walls;
        // The current Maze node to be used in the Maze generation process.
        private MazeNode currentGenerationNode;
        // The queue used to hold the Maze nodes that are still to be visited in the Maze generation process.
        private Queue<MazeNode> unvisitedNodesQueue;
        // The random number generator used to generate the Maze.
        private Random rand;
        // The texture used to draw the nodes and walls of the Maze.
        private Texture2D texture;
        // The position of the upper-leftmost point of the Maze on the game window.
        private Vector2 positionOnWindow;
        // The coordinate of the node at which the generation of the Maze begins.
        private Vector2 generationOriginCoordinate;
        // The seed used by the random number generator to generate the Maze, set to 0 if no seed is to be used.
        private int generationSeed;
        // The width of the Maze in coordinate spaces.
        private uint widthInCoordinates;
        // The height of the Maze in coordinate spaces.
        private uint heightInCoordinates;
        // The width and height of each of the coordinate spaces within the Maze.
        private uint coordinateSize;
        // The length in coordinates of the longest walk distance of any path originating at the generation origin node within the Maze.
        private uint longestWalkDistance;
        // The number of walls that are to be removed during every generation update.
        private uint generationSpeed;
        // The draw size of the nodes.
        private uint nodeDrawSize;
        // The draw thickness of the walls.
        private uint wallDrawThickness;
    }
}
