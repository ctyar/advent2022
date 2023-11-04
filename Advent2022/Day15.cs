using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent2022;

internal class Day15
{
    public void Run()
    {
        var input = File.ReadAllLines("Day15Input.txt");
        /*input = new[]
        {
            "Sensor at x=2, y=18: closest beacon is at x=-2, y=15",
            "Sensor at x=9, y=16: closest beacon is at x=10, y=16",
            "Sensor at x=13, y=2: closest beacon is at x=15, y=3",
            "Sensor at x=12, y=14: closest beacon is at x=10, y=16",
            "Sensor at x=10, y=20: closest beacon is at x=10, y=16",
            "Sensor at x=14, y=17: closest beacon is at x=10, y=16",
            "Sensor at x=8, y=7: closest beacon is at x=2, y=10",
            "Sensor at x=2, y=0: closest beacon is at x=2, y=10",
            "Sensor at x=0, y=11: closest beacon is at x=2, y=10",
            "Sensor at x=20, y=14: closest beacon is at x=25, y=17",
            "Sensor at x=17, y=20: closest beacon is at x=21, y=22",
            "Sensor at x=16, y=7: closest beacon is at x=15, y=3",
            "Sensor at x=14, y=3: closest beacon is at x=15, y=3",
            "Sensor at x=20, y=1: closest beacon is at x=15, y=3",
        };*/

        //Part1(input);

        Part2(input);
    }

    private void Part1(string[] input)
    {
        var sensors = new List<Point>();
        var beacons = new List<Point>();

        var row = 2000000;

        foreach (var line in input)
        {
            var matches = Regex.Matches(line, "\\-?\\d+");

            sensors.Add(new Point
            {
                X = int.Parse(matches[0].ValueSpan),
                Y = int.Parse(matches[1].ValueSpan)
            });

            beacons.Add(new Point
            {
                X = int.Parse(matches[2].ValueSpan),
                Y = int.Parse(matches[3].ValueSpan)
            });
        }

        var allItems = sensors.Union(beacons).ToList();
        var minX = allItems.MinBy(s => s.X)!.X;
        var maxX = allItems.MaxBy(s => s.X)!.X;

        var distances = new int[sensors.Count];
        for (var i = 0; i < sensors.Count; i++)
        {
            distances[i] = GetDistance(sensors[i], beacons[i]);
        }

        for (var i = 0; i < sensors.Count; i++)
        {
            minX = Math.Min(minX, sensors[i].X - distances[i]);
            maxX = Math.Max(maxX, sensors[i].X + distances[i]);
        }

        var inRangePoints = new List<Point>();

        for (var i = 0; i < maxX - minX + 1; i++)
        {
            var point = new Point
            {
                X = i + minX,
                Y = row
            };

            for (var j = 0; j < distances.Length; j++)
            {
                var pointDistance = GetDistance(point, sensors[j]);

                var isInRange = pointDistance <= distances[j];

                if (isInRange)
                {
                    inRangePoints.Add(point);
                    break;
                }
            }
        }

        var result = new List<Point>();
        foreach (var point in inRangePoints)
        {
            if (!allItems.Any(p => p.X == point.X && p.Y == point.Y))
            {
                result.Add(point);
            }
        }

        Console.WriteLine(result.Count());
    }

    public void Part2(string[] input)
    {
        var sensorList = new List<Point>();
        var beacons = new List<Point>();

        foreach (var line in input)
        {
            var matches = Regex.Matches(line, "\\-?\\d+");

            sensorList.Add(new Point
            {
                X = int.Parse(matches[0].ValueSpan),
                Y = int.Parse(matches[1].ValueSpan)
            });

            beacons.Add(new Point
            {
                X = int.Parse(matches[2].ValueSpan),
                Y = int.Parse(matches[3].ValueSpan)
            });
        }

        var min = 0;
        var max = 4000000;
        //max = 20;

        var sensors = new List<(int Range, Point Sensor)>();
        for (var i = 0; i < sensorList.Count; i++)
        {
            sensors.Add((GetDistance(sensorList[i], beacons[i]), sensorList[i]));
        }

        sensors = sensors.OrderByDescending(item => item.Range).ToList();

        var point = new Point
        {
            X = min,
            Y = min
        };

        while (true)
        {
            for (var i = 0; i < sensors.Count; i++)
            {
                var pointDistance = GetDistance(point, sensors[i].Sensor);
                var isInRange = pointDistance <= sensors[i].Range;

                if (isInRange)
                {
                    point.X += sensors[i].Range - pointDistance + 1;
                    point.Y += point.X / max;
                    point.X %= max;
                    break;
                }
                else if (i == sensors.Count - 1)
                {
                    Console.WriteLine($"({point.X},{point.Y})");
                    Console.WriteLine(point.X * (long)max + point.Y);
                    return;
                }
            }
        }
    }

    private static int GetDistance(Point first, Point second)
    {
        return Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);
    }

    [DebuggerDisplay("({X},{Y})")]
    private class Point
    {
        public required int X { get; set; }
        public required int Y { get; set; }
    }
}
