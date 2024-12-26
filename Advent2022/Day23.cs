using System.Diagnostics;

namespace Advent2022;

internal class Day23
{
    public void Run()
    {
        var input = File.ReadAllLines("Day23Input.txt");
        /*input =
        [
            ".....",
            "..##.",
            "..#..",
            ".....",
            "..##.",
            ".....",
        ];
        input =
        [
            "..............",
            "..............",
            ".......#......",
            ".....###.#....",
            "...#...#.#....",
            "....#...##....",
            "...#.###......",
            "...##.#.##....",
            "....#..#......",
            "..............",
            "..............",
            "..............",
        ];*/

        //Part1(input);

        Part2(input);
    }

    private static void Part1(string[] input)
    {
        var positions = ReadElevElves(input);

        var direction = Direction.North;
        for (var round = 1; round <= 10; round++)
        {
            var ruesult = ProposeMove(positions, direction); ;

            positions = ruesult.positions;
            direction = Next(direction);
        }

        var count = CountEmptyTiles(positions);
        Console.WriteLine(count);
    }

    private static void Part2(string[] input)
    {
        var positions = ReadElevElves(input);

        var direction = Direction.North;
        for (var round = 1; ; round++)
        {
            var result = ProposeMove(positions, direction);

            if (!result.anyoneMoved)
            {
                Console.WriteLine(round);
                break;
            }

            positions = result.positions;
            direction = Next(direction);
        }
    }

    private static (HashSet<Elve> positions, bool anyoneMoved) ProposeMove(HashSet<Elve> positions, Direction direction)
    {
        var newPositions = new HashSet<Elve>();
        var invalidPositions = new HashSet<Elve>();
        var anyoneMoved = false;

        foreach (var position in positions)
        {
            if (!NeedsToMove(position, positions))
            {
                newPositions.Add(position);
                continue;
            }

            anyoneMoved = true;
            var newPosition = Move(position, positions, direction);

            if (!newPositions.Add(newPosition))
            {
                invalidPositions.Add(newPosition);

                newPositions.Add(newPosition.GoBack());
            }
        }

        foreach (var position in invalidPositions)
        {
            newPositions.TryGetValue(position, out var elve);
            newPositions.Remove(elve!);

            newPositions.Add(elve!.GoBack());
        }

        return (newPositions, anyoneMoved);
    }

    private static Elve Move(Elve elve, HashSet<Elve> positions, Direction direction)
    {
        for (var directionIndex = 0; directionIndex <= 3; directionIndex++)
        {
            if (!TryMove(elve, direction, positions, out var newPosition))
            {
                direction = Next(direction);
                continue;
            }

            return newPosition!;
        }

        return elve;
    }

    private static bool TryMove(Elve elve, Direction direction, HashSet<Elve> positions, out Elve? newPosition)
    {
        if (direction == Direction.North)
        {
            if (!positions.Contains(elve.North()) &&
                !positions.Contains(elve.NorthEast()) &&
                !positions.Contains(elve.NorthWest()))
            {
                newPosition = elve.North();

                return true;
            }
        }
        else if (direction == Direction.South)
        {
            if (!positions.Contains(elve.South()) &&
                !positions.Contains(elve.SouthEast()) &&
                !positions.Contains(elve.SouthWest()))
            {
                newPosition =  elve.South();

                return true;
            }
        }
        else if (direction == Direction.West)
        {
            if (!positions.Contains(elve.West()) &&
            !positions.Contains(elve.NorthWest()) &&
            !positions.Contains(elve.SouthWest()))
            {
                newPosition =  elve.West();

                return true;
            }
        }
        else if (direction == Direction.East)
        {
            if (!positions.Contains(elve.East()) &&
            !positions.Contains(elve.NorthEast()) &&
            !positions.Contains(elve.SouthEast()))
            {
                newPosition =  elve.East();

                return true;
            }
        }

        newPosition = null;
        return false;
    }

    private static bool NeedsToMove(Elve poistion, HashSet<Elve> poistions)
    {
        if (poistions.Contains(poistion.North()))
        {
            return true;
        }

        if (poistions.Contains(poistion.NorthEast()))
        {
            return true;
        }

        if (poistions.Contains(poistion.East()))
        {
            return true;
        }

        if (poistions.Contains(poistion.SouthEast()))
        {
            return true;
        }

        if (poistions.Contains(poistion.South()))
        {
            return true;
        }

        if (poistions.Contains(poistion.SouthWest()))
        {
            return true;
        }

        if (poistions.Contains(poistion.West()))
        {
            return true;
        }

        if (poistions.Contains(poistion.NorthWest()))
        {
            return true;
        }

        return false;
    }

    private static int CountEmptyTiles(HashSet<Elve> positions)
    {
        var minX = positions.MinBy(i => i.X)!.X;
        var maxX = positions.MaxBy(i => i.X)!.X;

        var minY = positions.MinBy(i => i.Y)!.Y;
        var maxY = positions.MaxBy(i => i.Y)!.Y;

        var width = Math.Abs(maxX - minX) + 1;
        var height = Math.Abs(maxY - minY) + 1;

        return (width * height) - positions.Count;
    }

    private static HashSet<Elve> ReadElevElves(string[] input)
    {
        var positions = new HashSet<Elve>();

        for (var lineIndex = 0; lineIndex < input.Length; lineIndex++)
        {
            var elves = input[lineIndex].Select((charachter, index) => new { charachter, index })
                .Where(i => i.charachter == '#')
                .Select(i => new Elve(i.index, lineIndex, 0, 0)).ToList();

            foreach (var elve in elves)
            {
                positions.Add(elve);
            }
        }

        return positions;
    }

    private static void Display(HashSet<Elve> positions)
    {
        var minX = positions.MinBy(i => i.X)!.X;
        var minY = positions.MinBy(i => i.Y)!.Y;

        minX = minX < 0 ? -minX : 0;
        minY = minY < 0 ? -minY : 0;

        Console.Clear();

        foreach (var elve in positions.OrderBy(i => i.X))
        {
            Console.SetCursorPosition(elve.X + minX, elve.Y + minY);
            Console.Write("#");
        }

        /*foreach (var item in positions)
        {
            foreach (var p in item.Directions)
            {
                Console.Write(p + " ");
            }
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine();*/
    }

    private static Direction Next(Direction direction)
    {
        return (Direction)(((int)direction + 1) % 4);
    }

    [DebuggerDisplay("({X},{Y})")]
    private sealed class Elve
    {
        public int X { get; }

        public int Y { get; }

        public int PreviousX { get; }
        public int PreviousY { get; }

        public Elve(int x, int y, int previousX, int previousY)
        {
            X = x;
            Y = y;
            PreviousX = previousX;
            PreviousY = previousY;
        }

        public Elve North()
        {
            return new Elve(X, Y - 1, X, Y);
        }

        public Elve NorthEast()
        {
            return new Elve(X + 1, Y - 1, X, Y);
        }

        public Elve East()
        {
            return new Elve(X + 1, Y, X, Y);
        }

        public Elve SouthEast()
        {
            return new Elve(X + 1, Y + 1, X, Y);
        }

        public Elve South()
        {
            return new Elve(X, Y + 1, X, Y);
        }

        public Elve SouthWest()
        {
            return new Elve(X - 1, Y + 1, X, Y);
        }

        public Elve West()
        {
            return new Elve(X - 1, Y, X, Y);
        }

        public Elve NorthWest()
        {
            return new Elve(X - 1, Y - 1, X, Y);
        }

        public Elve GoBack()
        {
            return new Elve(PreviousX, PreviousY, 0, 0);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is Elve elve && Equals(elve);
        }

        public bool Equals(Elve other)
        {
            return X == other.X && Y == other.Y;
        }
    }

    private enum Direction
    {
        North = 0,
        South = 1,
        West = 2,
        East = 3,
    }
}
