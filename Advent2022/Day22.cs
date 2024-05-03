using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent2022;

internal class Day22
{
    private static Dictionary<(Position, DirectionType), (Position, DirectionType)> _lookupTable = null!;

    public void Run()
    {
        var input = File.ReadAllLines("Day22Input.txt");
        /*input =
        [
            "        ...#",
            "        .#..",
            "        #...",
            "        ....",
            "...#.......#",
            "........#...",
            "..#....#....",
            "..........#.",
            "        ...#....",
            "        .....#..",
            "        .#......",
            "        ......#.",
            "",
            "10R5L5R10L4R5L5",
        ];*/

        //Part1(input);

        Part2(input);
    }

    private static void Part1(string[] input)
    {
        var map = GetMap(input);
        var moves = GetMoves(input);

        var position = GetPosition(map);
        var direction = new Position(0, 1);

        foreach (var move in moves)
        {
            (position, direction) = Move(position, direction, move, WrapType.Normal, map);
        }

        var result = GetPassword(position, direction);

        Console.WriteLine(result);
    }

    private static void Part2(string[] input)
    {
        var map = GetMap(input);
        var moves = GetMoves(input);

        var position = GetPosition(map);
        var direction = new Position(0, 1);

        foreach (var move in moves)
        {
            var oldPosition = position;
            var oldDirection = direction;

            (position, direction) = Move(position, direction, move, WrapType.InCube, map);
        }

        var result = GetPassword(position, direction);

        Console.WriteLine(result);
    }

    private static int GetPassword(Position position, Position direction)
    {
        var directionValue = (int)direction.GetDirectionType();
        var result = (1000 * (position.Row + 1)) + (4 * (position.Column + 1)) + directionValue;
        return result;
    }

    private static (Position position, Position direction) Move(Position position, Position direction, string move, WrapType wrapType, char[][] map)
    {
        if (move == "R")
        {
            return direction switch
            {
                (0, 1) => (position, new(1, 0)),
                (1, 0) => (position, new(0, -1)),
                (0, -1) => (position, new(-1, 0)),
                (-1, 0) => (position, new(0, 1)),
                _ => throw new Exception()
            };
        }

        if (move == "L")
        {
            return direction switch
            {
                (0, 1) => (position, new(-1, 0)),
                (1, 0) => (position, new(0, 1)),
                (0, -1) => (position, new(1, 0)),
                (-1, 0) => (position, new(0, -1)),
                _ => throw new Exception()
            };
        }

        var steps = int.Parse(move);

        for (var i = 1; i <= steps; i++)
        {
            (position, direction) = Move(position, direction, wrapType, map);
        }

        return (position, direction);
    }

    private static (Position, Position) Move(Position position, Position direction, WrapType wrapType, char[][] map)
    {
        var newPosition = position + direction;
        var newDirection = direction;

        if (wrapType == WrapType.Normal)
        {
            newPosition = WrapIfNeeded(newPosition, map.Length, map[0].Length);
        }
        else
        {
            (newPosition, newDirection) = WrapInCubeIfNeeded(newPosition, direction, map);
        }

        if (map[newPosition.Row][newPosition.Column] == '#')
        {
            return (position, direction);
        }

        return (newPosition, newDirection);
    }

    private static Position WrapIfNeeded(Position position, int rowLength, int columnLength)
    {
        position = new Position(position.Row % rowLength, position.Column % columnLength);
        if (position.Row < 0)
        {
            position = position with { Row = position.Row + rowLength };
        }
        if (position.Column < 0)
        {
            position = position with { Column = position.Column + columnLength };
        }

        return position;
    }

    private static (Position, Position) WrapInCubeIfNeeded(Position position, Position direction, char[][] map)
    {
        if (_lookupTable is null)
        {
            _lookupTable = GetLookupTable(map.Length);
        }

        if (!_lookupTable.TryGetValue((position, direction.GetDirectionType()), out var result))
        {
            return (position, direction);
        }

        return (result.Item1, Position.Create(result.Item2));
    }

    private static Dictionary<(Position, DirectionType), (Position, DirectionType)> GetLookupTable(int rowLength)
    {
        var quarter = rowLength / 4;
        var half = 2 * quarter;
        var threeQuarter = 3 * quarter;
        var end = rowLength;

        // Base 0 indexing
        quarter--;
        half--;
        threeQuarter--;
        end--;

        // Map: https://github.com/user-attachments/assets/48b38b2a-5322-4839-bf35-e6a7192c9333
        var result = new Dictionary<(Position, DirectionType), (Position, DirectionType)>();

        // Back to top (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(-1, quarter + 1 + i);
            var direction = DirectionType.Top;

            var newPosition = new Position(threeQuarter + 1 + i, 0);
            var newDirection = DirectionType.Right;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Top to back (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(threeQuarter + 1 + i, -1);
            var direction = DirectionType.Left;

            var newPosition = new Position(0, quarter + 1 + i);
            var newDirection = DirectionType.Bottom;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Back to left (start to end)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(i, quarter);
            var direction = DirectionType.Left;

            var newPosition = new Position(threeQuarter - i, 0);
            var newDirection = DirectionType.Right;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Left to back (start to end)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(half + 1 + i, -1);
            var direction = DirectionType.Left;

            var newPosition = new Position(quarter - i, quarter + 1);
            var newDirection = DirectionType.Right;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Right to top (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(-1, half + 1 + i);
            var direction = DirectionType.Top;

            var newPosition = new Position(end, i);
            var newDirection = DirectionType.Top;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Top to right (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(end + 1, i);
            var direction = DirectionType.Bottom;

            var newPosition = new Position(0, half + 1 + i);
            var newDirection = DirectionType.Bottom;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Right to front (start to end)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(i, threeQuarter + 1);
            var direction = DirectionType.Right;

            var newPosition = new Position(threeQuarter - i, half);
            var newDirection = DirectionType.Left;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Front to right (start to end)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(threeQuarter - i, half + 1);
            var direction = DirectionType.Right;

            var newPosition = new Position(i, threeQuarter);
            var newDirection = DirectionType.Left;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Right to bottom (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(quarter + 1, half + 1 + i);
            var direction = DirectionType.Bottom;

            var newPosition = new Position(quarter + 1 + i, half);
            var newDirection = DirectionType.Left;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Bottom to right (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(quarter + 1 + i, half + 1);
            var direction = DirectionType.Right;

            var newPosition = new Position(quarter, half + 1 + i);
            var newDirection = DirectionType.Top;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Bottom to left (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(quarter + 1 + i, quarter);
            var direction = DirectionType.Left;

            var newPosition = new Position(half + 1, i);
            var newDirection = DirectionType.Bottom;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Left to bottom (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(half, i);
            var direction = DirectionType.Top;

            var newPosition = new Position(quarter + 1 + i, quarter + 1);
            var newDirection = DirectionType.Right;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Front to top (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(threeQuarter + 1, quarter + 1 + i);
            var direction = DirectionType.Bottom;

            var newPosition = new Position(threeQuarter + 1 + i, quarter);
            var newDirection = DirectionType.Left;
            result.Add((position, direction), (newPosition, newDirection));
        }

        // Top to Front (start to start)
        for (var i = 0; i <= quarter; i++)
        {
            var position = new Position(threeQuarter + 1 + i, quarter + 1);
            var direction = DirectionType.Right;

            var newPosition = new Position(threeQuarter, quarter + 1 + i);
            var newDirection = DirectionType.Top;
            result.Add((position, direction), (newPosition, newDirection));
        }

        return result;
    }

    private static Position GetPosition(char[][] map)
    {
        for (var column = 0; column < map[0].Length; column++)
        {
            if (map[0][column] == '.')
            {
                return new Position(0, column);
            }
        }

        throw new Exception();
    }

    private static char[][] GetMap(string[] input)
    {
        var mapCharachters = input.Take(input.Length - 2).ToList();
        var columnCount = mapCharachters.Max(i => i.Length);

        var map = new char[mapCharachters.Count][];

        for (var row = 0; row < mapCharachters.Count; row++)
        {
            map[row] = new char[columnCount];
            for (var column = 0; column < mapCharachters[row].Length; column++)
            {
                map[row][column] = mapCharachters[row][column];
            }
        }

        return map;
    }

    private static string[] GetMoves(string[] input)
    {
        var path = input.Last();
        var moves = Regex.Split(path, "(L|R)");

        return moves;
    }

    [DebuggerDisplay("({Row},{Column})")]
    private record Position(int Row, int Column)
    {
        public static Position operator +(Position a, Position b)
        {
            return new Position(a.Row + b.Row, a.Column + b.Column);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.Row - b.Row, a.Column - b.Column);
        }

        public DirectionType GetDirectionType()
        {
            return this switch
            {
                (0, 1) => DirectionType.Right,
                (1, 0) => DirectionType.Bottom,
                (0, -1) => DirectionType.Left,
                (-1, 0) => DirectionType.Top,
                _ => throw new Exception(),
            };
        }

        public static Position Create(DirectionType direction)
        {
            return direction switch
            {
                DirectionType.Right => new Position(0, 1),
                DirectionType.Bottom => new Position(1, 0),
                DirectionType.Left => new Position(0, -1),
                DirectionType.Top => new Position(-1, 0),
                _ => throw new Exception(),
            };
        }
    }

    private enum WrapType
    {
        Normal,
        InCube,
    }

    private enum DirectionType : byte
    {
        Right = 0,
        Bottom = 1,
        Left = 2,
        Top = 3,
    }
}