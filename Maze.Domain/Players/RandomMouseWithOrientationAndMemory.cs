﻿using System;
using System.Collections.Generic;
using System.Linq;
using MazeSharp.Game;

namespace MazeSharp.Domain.Players
{
    /// <summary>
    /// A random direction is chosen every move.
    /// The algorithm remembers where it has been.
    /// The algorithm considers its orientation.
    /// The algorithm considers whether there is a wall in the chosen direction.
    /// </summary>
    public class RandomMouseWithOrientationAndMemory : IPlayer
    {
        #region Fields
        private readonly HashSet<ICell> visited;
        private Direction orientation;
        private readonly List<Direction> possibleDirections;
        #endregion

        public RandomMouseWithOrientationAndMemory()
        {
            visited = new HashSet<ICell>();
            possibleDirections = new List<Direction>();
        }

        public ICell Move(IMaze maze)
        {
            // Remember current location
            visited.Add(maze.CurrentPosition);

            GetPossibleDirections(maze);

            // Try to continue in the same direction as previous move
            return GoOrientation(maze);
        }

        private void GetPossibleDirections(IMaze maze)
        {
            possibleDirections.Clear();

            if (!maze.CurrentPosition.HasNorthWall && !HasNorthBeenVisited(maze))
            {
                possibleDirections.Add(Direction.North);
            }
            if (!maze.CurrentPosition.HasEastWall && !HasEastBeenVisited(maze))
            {
                possibleDirections.Add(Direction.East);
            }
            if (!maze.CurrentPosition.HasSouthWall && !HasSouthBeenVisited(maze))
            {
                possibleDirections.Add(Direction.South);
            }
            if (!maze.CurrentPosition.HasWestWall && !HasWestBeenVisited(maze))
            {
                possibleDirections.Add(Direction.West);
            }
        }

        private void GetPossibleDirectionsWithVisited(IMaze maze)
        {
            possibleDirections.Clear();

            if (!maze.CurrentPosition.HasNorthWall)
            {
                possibleDirections.Add(Direction.North);
            }
            if (!maze.CurrentPosition.HasEastWall)
            {
                possibleDirections.Add(Direction.East);
            }
            if (!maze.CurrentPosition.HasSouthWall)
            {
                possibleDirections.Add(Direction.South);
            }
            if (!maze.CurrentPosition.HasWestWall)
            {
                possibleDirections.Add(Direction.West);
            }
        }

        private bool HasWestBeenVisited(IMaze maze)
        {
            return visited.Any(cell => cell.X == maze.CurrentPosition.X - 1 && cell.Y == maze.CurrentPosition.Y);
        }

        private bool HasSouthBeenVisited(IMaze maze)
        {
            return visited.Any(cell => cell.X == maze.CurrentPosition.X && cell.Y == maze.CurrentPosition.Y + 1);
        }

        private bool HasEastBeenVisited(IMaze maze)
        {
            return visited.Any(cell => cell.X == maze.CurrentPosition.X + 1 && cell.Y == maze.CurrentPosition.Y);
        }

        private bool HasNorthBeenVisited(IMaze maze)
        {
            return visited.Any(cell => cell.X == maze.CurrentPosition.X && cell.Y == maze.CurrentPosition.Y - 1);
        }

        private ICell GoOrientation(IMaze maze)
        {
            switch (orientation)
            {
                case Direction.North:
                    return !maze.CurrentPosition.HasNorthWall ? maze.GoNorth() : GoRandomDirection(maze);
                case Direction.East:
                    return !maze.CurrentPosition.HasEastWall ? maze.GoEast() : GoRandomDirection(maze);
                case Direction.South:
                    return !maze.CurrentPosition.HasSouthWall ? maze.GoSouth() : GoRandomDirection(maze);
                default:
                    return !maze.CurrentPosition.HasWestWall ? maze.GoWest() : GoRandomDirection(maze);
            }
        }

        private ICell GoRandomDirection(IMaze maze)
        {
            var randomiser = new Random(DateTime.Now.Millisecond);
            if (possibleDirections.Count == 0)
            {
                GetPossibleDirectionsWithVisited(maze);
            }
            var randomDirection = randomiser.Next(possibleDirections.Count);
            return GoDirection(maze, randomDirection);

        }

        private ICell GoDirection(IMaze maze, int randomDirection)
        {
            switch (possibleDirections[randomDirection])
            {
                case Direction.North:
                    orientation = Direction.North;
                    return maze.GoNorth();
                case Direction.East:
                    orientation = Direction.East;
                    return maze.GoEast();
                case Direction.South:
                    orientation = Direction.South;
                    return maze.GoSouth();
                default:
                    orientation = Direction.West;
                    return maze.GoWest();
            }
        }
    }
}