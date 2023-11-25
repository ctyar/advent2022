namespace Advent2022;

internal class Day18
{
    public void Run()
    {
        var input = File.ReadAllLines("Day18Input.txt");
        /*input = new[]
        {
            "2,2,2",
            "1,2,2",
            "3,2,2",
            "2,1,2",

            "2,3,2",
            "2,2,1",
            "2,2,3",
            "2,2,4",
            "2,2,6",

            "1,2,5",
            "3,2,5",
            "2,1,5",
            "2,3,5",
        };*/

        //input = ["1,1,1", "2,1,1"];

        //Part1(input);

        Part2(input);
    }

    private void Part1(string[] input)
    {
        var points = GetLavas(input);

        var surfaceArea = 0;
        foreach (var point in points)
        {
            surfaceArea += 6;

            var neighboursCount = GetNeighbors(point, points);
            surfaceArea -= neighboursCount;
        }

        Console.WriteLine(surfaceArea);
    }

    private static List<Point> GetLavas(string[] input)
    {
        var points = new List<Point>();

        foreach (var line in input)
        {
            var numbers = line.Split(',').Select(int.Parse).ToList();
            var point = new Point(numbers[0], numbers[1], numbers[2]);
            points.Add(point);
        }

        return points;
    }

    private int GetNeighbors(Point point, List<Point> points)
    {
        List<Point> adjacentPoints =
        [
            new Point(point.X - 1, point.Y, point.Z),
            new Point(point.X + 1, point.Y, point.Z),
            new Point(point.X, point.Y - 1, point.Z),
            new Point(point.X, point.Y + 1, point.Z),
            new Point(point.X, point.Y, point.Z - 1),
            new Point(point.X, point.Y, point.Z + 1)
        ];

        return points.Count(p => adjacentPoints.Contains(p));
    }

    public void Part2(string[] input)
    {
        var lavas = GetLavas(input);

        var box = GetBox(lavas);

        var start = new Point(box.MinX, box.MinY, box.MinZ);

        var droplets = FloodFill(start, box, lavas);

        var result = GetLavasTouchingWater(lavas, droplets, box);

        Console.WriteLine(result);
    }

    private static int GetLavasTouchingWater(List<Point> lavas, HashSet<Point> droplets, Box box)
    {
        var result = lavas.SelectMany(l => GetNeighbors(l, box)).Count(droplets.Contains);

        return result;
    }

    private static Box GetBox(List<Point> points)
    {
        var result = new Box
        {
            MinX = points.MinBy(p => p.X)!.X - 1,
            MaxX = points.MaxBy(p => p.X)!.X + 1,

            MinY = points.MinBy(p => p.Y)!.Y - 1,
            MaxY = points.MaxBy(p => p.Y)!.Y + 1,

            MinZ = points.MinBy(p => p.Z)!.Z - 1,
            MaxZ = points.MaxBy(p => p.Z)!.Z + 1
        };

        return result;
    }

    private static HashSet<Point> FloodFill(Point start, Box box, List<Point> lavas)
    {
        var toVisit = new Queue<Point>();
        toVisit.Enqueue(start);

        var visited = new HashSet<Point>
        {
            start
        };

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();

            var neighbors = GetNeighbors(current, box);

            var unvisitedNeighbors = neighbors.Where(c => !visited.Contains(c) && !lavas.Contains(c)).ToList();

            foreach (var neighbor in unvisitedNeighbors)
            {
                toVisit.Enqueue(neighbor);
                visited.Add(neighbor);
            }
        }

        return visited;
    }

    private static List<Point> GetNeighbors(Point current, Box box)
    {
        var result = new List<Point>();

        var point = current.MoveTop();
        if (box.Contains(point))
        {
            result.Add(point);
        }

        point = current.MoveBottom();
        if (box.Contains(point))
        {
            result.Add(point);
        }

        point = current.MoveFront();
        if (box.Contains(point))
        {
            result.Add(point);
        }

        point = current.MoveBack();
        if (box.Contains(point))
        {
            result.Add(point);
        }

        point = current.MoveLeft();
        if (box.Contains(point))
        {
            result.Add(point);
        }

        point = current.MoveRight();
        if (box.Contains(point))
        {
            result.Add(point);
        }

        return result;
    }

    private record Point(int X, int Y, int Z)
    {
        public Point MoveLeft()
        {
            return new Point(X - 1, Y, Z);
        }

        public Point MoveRight()
        {
            return new Point(X + 1, Y, Z);
        }

        public Point MoveFront()
        {
            return new Point(X, Y + 1, Z);
        }

        public Point MoveBack()
        {
            return new Point(X, Y - 1, Z);
        }

        public Point MoveTop()
        {
            return new Point(X, Y, Z + 1);
        }

        public Point MoveBottom()
        {
            return new Point(X, Y, Z - 1);
        }
    }

    private class Box
    {
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MinZ { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int MaxZ { get; set; }

        public bool Contains(Point point)
        {
            return MinX <= point.X && point.X <= MaxX &&
                MinY <= point.Y && point.Y <= MaxY &&
                MinZ <= point.Z && point.Z <= MaxZ;
        }
    }
}