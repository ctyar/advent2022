using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent2022;

internal class Day16
{
    private readonly List<Valve> _allValves = [];
    private List<Valve> _valvesWithFlowRate = [];
    private readonly Dictionary<string, int> _map = [];
    private Solution _bestSolution = null!;
    private int _bestSolutionScore = 0;

    public void Run()
    {
        var input = File.ReadAllLines("Day16Input.txt");
        /*input = new[]
        {
            "Valve AA has flow rate=0; tunnels lead to valves DD, II, BB",
            "Valve BB has flow rate=13; tunnels lead to valves CC, AA",
            "Valve CC has flow rate=2; tunnels lead to valves DD, BB",
            "Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE",
            "Valve EE has flow rate=3; tunnels lead to valves FF, DD",
            "Valve FF has flow rate=0; tunnels lead to valves EE, GG",
            "Valve GG has flow rate=0; tunnels lead to valves FF, HH",
            "Valve HH has flow rate=22; tunnel leads to valve GG",
            "Valve II has flow rate=0; tunnels lead to valves AA, JJ",
            "Valve JJ has flow rate=21; tunnel leads to valve II",
        };*/

        //Part1(input);

        Part2(input);
    }

    private void Part1(string[] input)
    {
        const int TimeLimit = 30;

        ReadInputAndInitilize(input);

        var solution = new Solution(false, 0, 0, _allValves.First(v => v.Name == "AA"), null, _valvesWithFlowRate);
        _bestSolution = solution;

        FindSolution(solution, TimeLimit);

        Console.WriteLine(_bestSolution.Key);
        Console.WriteLine(_bestSolution.GetPressureReleased(TimeLimit));
    }

    public void Part2(string[] input)
    {
        const int TimeLimit = 26;

        ReadInputAndInitilize(input);

        var solution = new Solution(false, 0, 0, _allValves.First(v => v.Name == "AA"), null, _valvesWithFlowRate);
        _bestSolution = solution;

        FindSolution(solution, TimeLimit);
        var myPath = _bestSolution;
        var myPressureReleased = _bestSolution.GetPressureReleased(TimeLimit);

        _valvesWithFlowRate = _bestSolution.RemainingValves;
        solution = new Solution(false, 0, 0, _allValves.First(v => v.Name == "AA"), null, _valvesWithFlowRate);
        _bestSolution = solution;
        _bestSolutionScore = 0;
        FindSolution(solution, TimeLimit);

        Console.WriteLine(myPath.Key);
        Console.WriteLine(myPressureReleased);

        Console.WriteLine(_bestSolution.Key);
        Console.WriteLine(_bestSolution.GetPressureReleased(TimeLimit));
    }

    private void ReadInputAndInitilize(string[] input)
    {
        foreach (var line in input)
        {
            var match = Regex.Match(line, "Valve (.*) has.*=(\\d+).*valves? (.*)");

            var valveName = match.Groups[1].Value;
            var flowRate = int.Parse(match.Groups[2].Value);
            var tunnels = match.Groups[3].Value.Split(", ");

            _allValves.Add(new Valve
            {
                Name = valveName,
                FlowRate = flowRate,
                TunnelNames = tunnels
            });
        }

        foreach (var valve in _allValves)
        {
            foreach (var tunnelName in valve.TunnelNames)
            {
                var tunnel = _allValves.First(v => v.Name == tunnelName);
                valve.Tunnels.Add(tunnel);
            }
        }

        _valvesWithFlowRate = _allValves.Where(v => v.FlowRate > 0).ToList();

        CalculateShortestPath();
    }

    private void CalculateShortestPath()
    {
        foreach (var source in _allValves)
        {
            foreach (var destination in _allValves)
            {
                _map.Add($"{source.Name}-{destination.Name}", source.GetPathLengthTo(destination));
            }
        }
    }

    private void FindSolution(Solution solution, int timeLimit)
    {
        var visited = new HashSet<string>();
        var toVisit = new Stack<Solution>();
        toVisit.Push(solution);

        while (toVisit.Count != 0)
        {
            var current = toVisit.Pop();

            if (current.GetPressureReleased(timeLimit) > _bestSolutionScore)
            {
                _bestSolution = current;
                _bestSolutionScore = current.GetPressureReleased(timeLimit);
            }

            foreach (var valve in current.RemainingValves)
            {
                var pathLength = _map[$"{current.Location.Name}-{valve.Name}"];
                if (current.Time <= timeLimit - pathLength)
                {
                    var openedAt = current.Time + pathLength + 1;
                    var child = new Solution(true, openedAt, openedAt, valve, current, _valvesWithFlowRate);

                    if (!visited.Contains(child.Key))
                    {
                        toVisit.Push(child);
                        visited.Add(child.Key);
                    }
                }
            }
        }
    }

    [DebuggerDisplay("{Name}")]
    private class Valve
    {
        public required string Name { get; init; }
        public required int FlowRate { get; init; }
        public required string[] TunnelNames { get; init; }
        public List<Valve> Tunnels { get; init; } = [];

        public override bool Equals(object? other)
        {
            var otherValve = other as Valve;

            return Name.Equals(otherValve?.Name);
        }

        public static bool operator ==(Valve valve1, Valve valve2)
        {
            return valve1.Equals(valve2);
        }

        public static bool operator !=(Valve valve1, Valve valve2)
        {
            return !valve1.Equals(valve2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public int GetPathLengthTo(Valve destination)
        {
            var visited = new HashSet<string>();
            var current = (this, 0);

            var toVisit = new Queue<(Valve, int)>();

            while (current.Item1 != destination)
            {
                foreach (var tunnel in current.Item1.Tunnels)
                {
                    if (!visited.Contains(tunnel.ToString()!))
                    {
                        toVisit.Enqueue((tunnel, current.Item2 + 1));
                    }
                }

                current = toVisit.Dequeue();
            }

            return current.Item2;
        }
    }

    private class Solution
    {
        public bool Opened { get; }

        public int OpenedAt { get; }

        public int Time { get; }

        public Valve Location { get; }

        public Solution? Parent { get; }

        public List<Valve> RemainingValves { get; }

        public string Key { get; }

        public Solution(bool opened, int openedAt, int time, Valve location, Solution? parent, List<Valve> valvesWithFlowRate)
        {
            Opened = opened;
            OpenedAt = openedAt;
            Time = time;
            Location = location;
            Parent = parent;

            RemainingValves = parent?.RemainingValves ?? valvesWithFlowRate;
            if (opened)
            {
                RemainingValves = RemainingValves.Where(v => v != location).ToList();
            }

            Key = (Parent?.Key ?? string.Empty) + "->" + Location.Name + Opened;
        }

        public int GetPressureReleased(int timeLimit)
        {
            var released = 0;

            if (Opened)
            {
                released = (timeLimit - OpenedAt) * Location.FlowRate;
            }

            return (Parent?.GetPressureReleased(timeLimit) ?? 0) + released;
        }
    }
}
