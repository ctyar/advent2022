using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent2022;

internal class Day19
{
    private static readonly Resource[] Resources = [Resource.Ore, Resource.Clay, Resource.Obsidian, Resource.Geode];

    public void Run()
    {
        var input = File.ReadAllLines("Day19Input.txt");
        /*input =
        [
            "Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.",
            "Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.",
        ];*/

        //Part1(input);

        Part2(input);
    }

    private static void Part1(string[] input)
    {
        var blueprints = GetBlueprints(input);

        var result = 0;

        foreach (var blueprint in blueprints)
        {
            var state = GetMostGeodes(blueprint, 25);

            result += state.GeodCount * blueprint.Id;
        }

        Console.WriteLine(result);
    }

    private static void Part2(string[] input)
    {
        var blueprints = GetBlueprints(input);
        blueprints = blueprints.Take(3).ToList();

        var result = 1;

        foreach (var blueprint in blueprints)
        {
            var state = GetMostGeodes(blueprint, 33);
            Console.WriteLine(state.GeodCount);
            result *= state.GeodCount;
        }

        Console.WriteLine(result);
    }

    private static State GetMostGeodes(Blueprint blueprint, int timeLimit)
    {
        var toVisit = new Stack<State>();
        var startingState = new State();
        toVisit.Push(startingState);

        var visited = new HashSet<string>();

        var bestState = startingState;

        while (toVisit.Count != 0)
        {
            var state = toVisit.Pop();

            if (state.GeodCount > bestState.GeodCount)
            {
                Console.WriteLine($"{state.Key} {state.GeodCount}");
                bestState = state;
            }

            var children = ProduceChildren(blueprint, state, timeLimit);

            foreach (var child in children)
            {
                if (visited.Contains(child.Key))
                {
                    continue;
                }

                if (child.GetMaxGeodCount(timeLimit) <= bestState.GeodCount)
                {
                    continue;
                }

                toVisit.Push(child);
                visited.Add(child.Key);
            }
        }

        Console.WriteLine($"{visited.Count:n0} {bestState.Key}");
        return bestState;
    }

    private static List<State> ProduceChildren(Blueprint blueprint, State state, int timeLimit)
    {
        if (state.Time == timeLimit)
        {
            return [];
        }

        var result = new List<State>();

        var newResources = CollectResources(state.Robots, state.Resources);
        result.Add(state.With(newResources));

        var children = CreateNewRobots(state, blueprint);
        result.AddRange(children);
        result.First().DontBuild.AddRange(children.Select(i => i.Robots.Last().Type));

        return result;
    }

    private static List<State> CreateNewRobots(State state, Blueprint blueprint)
    {
        var result = new List<State>();

        foreach (var robotType in Resources)
        {
            var shouldBuild = ShouldBuildRobot(state, blueprint, robotType);

            if (!shouldBuild)
            {
                continue;
            }

            var newState = BuildRobot(state, blueprint, robotType);

            result.Add(newState);
        }

        return result;
    }

    private static State BuildRobot(State state, Blueprint blueprint, Resource robotType)
    {
        var newResources = CollectResources(state.Robots, state.Resources);

        var requirements = blueprint.Requirements[robotType];
        foreach (var requirement in requirements)
        {
            newResources[requirement.Key] -= requirement.Value;
        }

        return state.With(newResources, robotType);
    }

    private static bool ShouldBuildRobot(State state, Blueprint blueprint, Resource robotType)
    {
        var canBuild = CanBuildRobot(state, blueprint, robotType);

        if (!canBuild)
        {
            return false;
        }

        var maxRequired = blueprint.GetMaxResourceRequired(robotType);

        if (state.Robots.Count(r => r.Type == robotType) >= maxRequired)
        {
            return false;
        }

        if (state.DontBuild.Contains(robotType))
        {
            return false;
        }

        return true;
    }

    private static bool CanBuildRobot(State state, Blueprint blueprint, Resource resource)
    {
        var requirements = blueprint.Requirements[resource];

        var canBuild = true;
        foreach (var requirement in requirements)
        {
            if (state.Resources[requirement.Key] < requirement.Value)
            {
                canBuild = false;
                break;
            };
        }

        return canBuild;
    }

    private static Dictionary<Resource, int> CollectResources(List<Robot> robots, Dictionary<Resource, int> resources)
    {
        var result = resources.ToDictionary();

        foreach (var robot in robots)
        {
            result[robot.Type] = result[robot.Type] + 1;
        }

        return result;
    }

    private static List<Blueprint> GetBlueprints(string[] input)
    {
        var idRegex = new Regex("\\d+");
        var robotRegex = new Regex("costs (\\d.+)");

        var result = new List<Blueprint>();

        foreach (var line in input)
        {
            var parts = line.Split(":");

            var id = int.Parse(idRegex.Match(parts[0]).Value);

            var resources = parts[1].Split(".");
            var ore = GetResources(robotRegex.Match(resources[0]).Groups[1].Value);
            var clay = GetResources(robotRegex.Match(resources[1]).Groups[1].Value);
            var obsidian = GetResources(robotRegex.Match(resources[2]).Groups[1].Value);
            var geode = GetResources(robotRegex.Match(resources[3]).Groups[1].Value);

            result.Add(new Blueprint
            {
                Id = id,
                Requirements = new Dictionary<Resource, Dictionary<Resource, int>>
                {
                    { Resource.Ore, ore },
                    { Resource.Clay, clay },
                    { Resource.Obsidian, obsidian },
                    { Resource.Geode, geode },
                }
            });
        }

        return result;
    }

    private static Dictionary<Resource, int> GetResources(string resourceLine)
    {
        var resourceItems = resourceLine.Split(" and ");

        var resourceRegex = new Regex("(\\d+) (.+)");

        var result = new Dictionary<Resource, int>();

        foreach (var resourceItem in resourceItems)
        {
            var match = resourceRegex.Match(resourceItem);

            var count = int.Parse(match.Groups[1].Value);
            var resourceName = match.Groups[2].Value;
            var resource = resourceName switch
            {
                "ore" => Resource.Ore,
                "clay" => Resource.Clay,
                "obsidian" => Resource.Obsidian,
                _ => throw new ArgumentException()
            };

            result.Add(resource, count);
        }

        return result;
    }

    private class Robot
    {
        public Resource Type { get; set; }
        public int CreateTime { get; set; }
    }

    private class Blueprint
    {
        private Dictionary<Resource, int> _maxResourceRequired = null!;

        public int Id { get; set; }

        public Dictionary<Resource, Dictionary<Resource, int>> Requirements { get; set; } = [];

        public int GetMaxResourceRequired(Resource resource)
        {
            if (_maxResourceRequired is null)
            {
                _maxResourceRequired = new Dictionary<Resource, int>
                {
                    {
                        Resource.Ore, Requirements.SelectMany(r => r.Value)
                            .Where(r => r.Key == Resource.Ore)
                            .MaxBy(r => r.Value).Value
                    },
                    {
                        Resource.Clay, Requirements.SelectMany(r => r.Value)
                            .Where(r => r.Key == Resource.Clay)
                            .MaxBy(r => r.Value).Value
                    },
                    {
                        Resource.Obsidian, Requirements.SelectMany(r => r.Value)
                            .Where(r => r.Key == Resource.Obsidian)
                            .MaxBy(r => r.Value).Value
                    },
                    { Resource.Geode, int.MaxValue }
                };
            }

            return _maxResourceRequired[resource];
        }
    }

    private enum Resource
    {
        Ore,
        Clay,
        Obsidian,
        Geode,
    }

    [DebuggerDisplay("{Key}")]
    private class State
    {
        public List<Robot> Robots { get; set; } = [new Robot { Type = Resource.Ore }];
        public Dictionary<Resource, int> Resources { get; set; } = new()
        {
            { Resource.Ore, 0 },
            { Resource.Clay, 0 },
            { Resource.Obsidian, 0 },
            { Resource.Geode, 0 },
        };

        public int Time { get; set; } = 1;

        public List<Resource> DontBuild = [];

        public int GeodCount => Resources[Resource.Geode];

        public string Key => $"{Time}-{string.Join(",", Robots.Select(r => $"({r.Type}-{r.CreateTime})"))}";

        public int GetMaxGeodCount(int timeLimit)
        {
            var geodRobotCount = Robots.Count(r => r.Type == Resource.Geode);
            var geodCount = Resources.First(r => r.Key == Resource.Geode).Value;

            for (var time = Time; time <= timeLimit; time++)
            {
                geodCount += geodRobotCount;
                geodRobotCount++;
            }

            return geodCount;
        }

        public State With(Dictionary<Resource, int> resources)
        {
            return new State
            {
                Robots = Robots.ToList(),
                Resources = resources,
                Time = Time + 1
            };
        }

        public State With(Dictionary<Resource, int> resources, Resource robotType)
        {
            var newRobots = Robots.ToList();
            newRobots.Add(new Robot
            {
                Type = robotType,
                CreateTime = Time
            });

            return new State
            {
                Robots = newRobots,
                Resources = resources,
                Time = Time + 1
            };
        }
    }
}