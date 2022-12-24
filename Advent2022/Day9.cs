namespace Advent2022;

internal class Day9
{
    public static void Run()
    {
        var moves = File.ReadAllLines("Day9Input.txt");

        var headCordinates = (0, 0);
        var tailCordinates = (0, 0);

        var allTailCordinates = new HashSet<(int, int)>
        {
            tailCordinates
        };

        foreach (var move in moves)
        {
            var split = move.Split(' ');
            var direction = split[0];
            var moveCount = int.Parse(split[1]);

            foreach (var _ in Enumerable.Range(1, moveCount))
            {
                headCordinates = MoveHead(headCordinates, direction);
                tailCordinates = AdjustTail(headCordinates, tailCordinates);
                allTailCordinates.Add(tailCordinates);
            }
        }

        Console.WriteLine(allTailCordinates.Count);

        Part2(moves);
    }

    public static void Part2(string[] moves)
    {
        var pieces = new (int x, int y)[10];

        var allTailCordinates = new HashSet<(int, int)>();

        foreach (var move in moves)
        {
            var split = move.Split(' ');
            var direction = split[0];
            var moveCount = int.Parse(split[1]);
            foreach (var _ in Enumerable.Range(1, moveCount))
            {
                pieces[0] = MoveHead(pieces[0], direction);
                for (var i = 1; i < pieces.Length; i++)
                {
                    pieces[i] = AdjustTail(pieces[i - 1], pieces[i]);
                }
                allTailCordinates.Add(pieces.Last());
            }
        }

        Console.WriteLine(allTailCordinates.Count);
    }

    private static (int x, int y) MoveHead((int x, int y) headCordinates, string direction)
    {
        headCordinates = direction switch
        {
            "R" => (headCordinates.x + 1, headCordinates.y),
            "U" => (headCordinates.x, headCordinates.y + 1),
            "L" => (headCordinates.x - 1, headCordinates.y),
            "D" => (headCordinates.x, headCordinates.y - 1),
        };

        return headCordinates;
    }

    private static (int, int) AdjustTail((int x, int y) headCordinates, (int x, int y) tailCordinates)
    {
        var xDiff = headCordinates.x - tailCordinates.x;
        var yDiff = headCordinates.y - tailCordinates.y;

        if (Math.Abs(xDiff) > 1 || Math.Abs(yDiff) > 1)
        {
            tailCordinates.x += Math.Sign(xDiff);

            tailCordinates.y += Math.Sign(yDiff);
        }

        return tailCordinates;
    }
}
