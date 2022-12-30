using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Advent2022;

internal class Day12
{
    public static void Run()
    {
        var input = File.ReadAllLines("Day12Input.txt");

        var map = new char[input.Length][];
        var startingPoint = new Point(0, 0);
        var destination = new Point(0, 0);
        for (var i = 0; i < input.Length; i++)
        {
            map[i] = input[i].ToCharArray();

            var indexOfS = Array.IndexOf(map[i], 'S');
            if (indexOfS != -1)
            {
                map[i][indexOfS] = 'a';
                startingPoint = new Point(i, indexOfS);
            }

            var indexOfE = Array.IndexOf(map[i], 'E');
            if (indexOfE != -1)
            {
                map[i][indexOfE] = 'z';
                destination = new Point(i, indexOfE);
            }
        }

        var shortestPathLength = GetShortestPathLength(map, startingPoint, destination);

        Console.WriteLine(shortestPathLength);

        Part2(map, startingPoint, destination);
    }

    private static void Part2(char[][] map, Point startingPoint, Point destination)
    {
        var allStartingPoints = new List<Point> { startingPoint };

        for (var i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                if (map[i][j] == 'a')
                {
                    allStartingPoints.Add(new Point(i, j));
                }
            }
        }

        var minLength = int.MaxValue;

        foreach (var currentStartingPoint in allStartingPoints)
        {
            var pathLength = GetShortestPathLength(map, currentStartingPoint, destination);

            if (pathLength.HasValue && pathLength.Value < minLength)
            {
                minLength = pathLength.Value;
            }
        }

        Console.WriteLine(minLength);
    }

    private static int? GetShortestPathLength(char[][] map, Point startingPoint, Point destination)
    {
        var currentNode = new Node(startingPoint, 0);

        var visited = new HashSet<Point>();
        var toVisit = new Queue<Node>();
        toVisit.Enqueue(currentNode);
        var foundAPath = false;

        while (toVisit.Count > 0)
        {
            currentNode = toVisit.Dequeue();
            visited.Add(currentNode.Location);

            if (currentNode.Location.Equals(destination))
            {
                foundAPath = true;
                break;
            }

            var children = GetChildren(map, currentNode, visited, toVisit);
            children.ForEach(toVisit.Enqueue);
        }

        if (!foundAPath)
        {
            return null;
        }

        return currentNode.PathLength;
    }

    private static List<Node> GetChildren(char[][] map, Node currentNode, HashSet<Point> visited, Queue<Node> toVisit)
    {
        var result = new List<Node>();

        for (var move = 0; move <= 3; move++)
        {
            var nextChild = GetNextChild(map, currentNode, move, visited, toVisit);

            if (nextChild is not null)
            {
                result.Add(nextChild);
            }
        }

        return result;
    }

    private static Node? GetNextChild(char[][] map, Node currentNode, int move, HashSet<Point> visited, Queue<Node> toVisit)
    {
        var nextLocation = move switch
        {
            0 => new Point(currentNode.Location.X + 1, currentNode.Location.Y),
            1 => new Point(currentNode.Location.X, currentNode.Location.Y + 1),
            2 => new Point(currentNode.Location.X -1, currentNode.Location.Y),
            3 => new Point(currentNode.Location.X, currentNode.Location.Y - 1),
        };

        if (nextLocation.X < 0 || nextLocation.X >= map.Length ||
            nextLocation.Y < 0 || nextLocation.Y >= map[nextLocation.X].Length)
        {
            return null;
        }

        if (map[nextLocation.X][nextLocation.Y] - map[currentNode.Location.X][currentNode.Location.Y] > 1)
        {
            return null;
        }

        if (visited.Contains(nextLocation))
        {
            return null;
        }

        if (toVisit.Any(item => item.Location.Equals(nextLocation)))
        {
            return null;
        }

        return new Node(nextLocation, currentNode.PathLength + 1);
    }

    private class Node
    {
        public Point Location { get; }

        public int PathLength { get; }

        public Node(Point location, int pathLength)
        {
            Location = location;
            PathLength = pathLength;
        }
    }

    [DebuggerDisplay("({X} {Y})")]
    private class Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not Point point)
            {
                return false;
            }

            return point.X == X && point.Y == Y;
        }
    }
}