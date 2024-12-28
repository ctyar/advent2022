using System.Diagnostics;

namespace Advent2022;

internal class Day24
{
    public void Run()
    {
        var input = File.ReadAllLines("Day24Input.txt");
        /*input =
        [
            "#.#####",
            "#.....#",
            "#>....#",
            "#.....#",
            "#...v.#",
            "#.....#",
            "#####.#",
        ];*/
        /*input =
        [
            "#.######",
            "#>>.<^<#",
            "#.<..<<#",
            "#>v.><>#",
            "#<^v^^>#",
            "######.#",
        ];*/

        //Part1(input);

        Part2(input);
    }

    private static void Part1(string[] input)
    {
        var (windArchive, lastXIndex, lastYIndex) = ReadMap(input);

        var source = new State
        {
            Position = new Position(0, -1),
            Time = 0,
        };

        var destination = new Position(lastXIndex, lastYIndex);

        var time = GetTime(windArchive, lastXIndex, lastYIndex, source, destination);

        Console.WriteLine(time);
    }

    private static void Part2(string[] input)
    {
        var (windArchive, lastXIndex, lastYIndex) = ReadMap(input);

        var source = new State
        {
            Position = new Position(0, -1),
            Time = 0,
        };
        var destination = new Position(lastXIndex, lastYIndex);
        var time1 = GetTime(windArchive, lastXIndex, lastYIndex, source, destination);

        var source2 = new State
        {
            Time = time1,
            Position = new Position(lastXIndex, lastYIndex + 1),
        };
        var destination2 = new Position(0, 0);
        var time2 = GetTime(windArchive, lastXIndex, lastYIndex, source2, destination2);

        var source3 = new State
        {
            Position = new Position(0, -1),
            Time = time2,
        };
        var destination3 = new Position(lastXIndex, lastYIndex);
        var time3 = GetTime(windArchive, lastXIndex, lastYIndex, source3, destination3);

        Console.WriteLine(time3);
    }

    private static int GetTime(Dictionary<int, Blizzard> windArchive, int lastXIndex, int lastYIndex, State source, Position destination)
    {
        var toVisit = new PriorityQueue<State, int>();
        toVisit.Enqueue(source, GetPriority(source, lastXIndex, lastYIndex));
        var visited = new HashSet<string>
        {
            source.Key
        };

        var bestSolution = int.MaxValue;

        while (toVisit.Count > 0)
        {
            var currentState = toVisit.Dequeue();

            var isBestSolutionChanged = false;
            if (IsGoal(currentState, destination))
            {
                if (currentState.Time < bestSolution)
                {
                    bestSolution = currentState.Time;
                    isBestSolutionChanged = true;
                }
            }

            var blizzard = GetOrCalculateBlizzard(currentState.Time + 1, windArchive, lastXIndex, lastYIndex);

            var newStates = CalculateStates(blizzard, lastXIndex, lastYIndex, currentState);

            foreach (var newState in newStates)
            {
                if (!visited.Contains(newState.Key))
                {
                    toVisit.Enqueue(newState, GetPriority(newState, lastXIndex, lastYIndex));
                    visited.Add(newState.Key);
                }
            }

            if (isBestSolutionChanged)
            {
                toVisit = Trim(toVisit, bestSolution);
            }
        }

        return bestSolution + 1;
    }

    private static Blizzard GetOrCalculateBlizzard(int minute, Dictionary<int, Blizzard> blizzards,
        int lastXIndex, int lastYIndex)
    {
        if (blizzards.TryGetValue(minute, out var blizzard))
        {
            return blizzard;
        }

        var lastCalculatetMap = blizzards.Keys.Max();

        if (lastCalculatetMap != minute - 1)
        {
            throw new Exception();
        }

        var previousMap = blizzards[lastCalculatetMap];
        blizzard = CalculateNextWind(previousMap, lastXIndex, lastYIndex);

        blizzards.Add(minute, blizzard);

        return blizzard;
    }

    private static List<State> CalculateStates(Blizzard blizzard, int lastXIndex, int lastYIndex, State state)
    {
        var result = new List<State>();

        if (state.Position.X == 0 && state.Position.Y == -1)
        {
            if (IsFree(new Position(0, 0), blizzard))
            {
                result.Add(new State
                {
                    Position = new Position(0, 0),
                    Time = state.Time + 1,
                });
            }
        }
        else if (state.Position.X == lastXIndex && state.Position.Y == lastYIndex + 1)
        {
            if (IsFree(new Position(lastXIndex, lastYIndex), blizzard))
            {
                result.Add(new State
                {
                    Position = new Position(lastXIndex, lastYIndex),
                    Time = state.Time + 1,
                });
            }
        }
        else
        {
            if (state.Position.X < lastXIndex && IsFree(new Position(state.Position.X + 1, state.Position.Y), blizzard))
            {
                result.Add(new State
                {
                    Position = new Position(state.Position.X + 1, state.Position.Y),
                    Time = state.Time + 1,
                });
            }
            if (state.Position.Y < lastYIndex &&IsFree(new Position(state.Position.X, state.Position.Y + 1), blizzard))
            {
                result.Add(new State
                {
                    Position = new Position(state.Position.X, state.Position.Y + 1),
                    Time = state.Time + 1,
                });
            }
            if (state.Position.X >= 1 && IsFree(new Position(state.Position.X - 1, state.Position.Y), blizzard))
            {
                result.Add(new State
                {
                    Position = new Position(state.Position.X - 1, state.Position.Y),
                    Time = state.Time + 1,
                });
            }
            if (state.Position.Y >= 1 && IsFree(new Position(state.Position.X, state.Position.Y - 1), blizzard))
            {
                result.Add(new State
                {
                    Position = new Position(state.Position.X, state.Position.Y - 1),
                    Time = state.Time + 1,
                });
            }
        }

        if (IsFree(new Position(state.Position.X, state.Position.Y), blizzard))
        {
            result.Add(new State
            {
                Position = new Position(state.Position.X, state.Position.Y),
                Time = state.Time + 1,
            });
        }

        return result;
    }

    private static bool IsGoal(State state, Position destination)
    {
        return state.Position.X == destination.X && state.Position.Y == destination.Y;
    }

    private static PriorityQueue<State, int> Trim(PriorityQueue<State, int> toVisit, int bestSolution)
    {
        var result = new PriorityQueue<State, int>();

        foreach (var item in toVisit.UnorderedItems)
        {
            if (item.Priority < bestSolution)
            {
                result.Enqueue(item.Element, item.Priority);
            }
        }

        return result;
    }

    private static int GetPriority(State state, int lastXIndex, int lastYIndex)
    {
        return state.Time + GetDistanceToGoal(state.Position, lastXIndex, lastYIndex);
    }

    private static int GetDistanceToGoal(Position position, int lastXIndex, int lastYIndex)
    {
        return Math.Abs(lastXIndex - position.X) + Math.Abs(lastYIndex - position.Y);
    }

    private static Blizzard CalculateNextWind(Blizzard map, int lastXIndex, int lastYIndex)
    {
        var winds = new List<Wind>();

        foreach (var wind in map.Winds)
        {
            var newWind = wind.AdvanceTime(lastXIndex, lastYIndex);

            winds.Add(newWind);
        }

        return new Blizzard(winds);
    }

    private static bool IsFree(Position position, Blizzard blizzard)
    {
        return !blizzard.Positions.Contains(position);
    }

    private static (Dictionary<int, Blizzard> windArchive, int lastXIndex, int lastYIndex) ReadMap(string[] input)
    {
        var lines = input.Skip(1).Take(input.Length - 2).ToList();
        var rows = lines.Select(i => i.Skip(1).Take(i.Length - 2).ToList()).ToList();

        var winds = new List<Wind>();

        for (var i = 0; i < rows.Count; i++)
        {
            for (var j = 0; j < rows[i].Count; j++)
            {
                if (rows[i][j] == '>')
                {
                    var wind = new Wind(DirectionType.Right, new Position(j, i));

                    winds.Add(wind);
                }
                else if (rows[i][j] == '<')
                {
                    var wind = new Wind(DirectionType.Left, new Position(j, i));

                    winds.Add(wind);
                }
                else if (rows[i][j] == '^')
                {
                    var wind = new Wind(DirectionType.Top, new Position(j, i));

                    winds.Add(wind);
                }
                else if (rows[i][j] == 'v')
                {
                    var wind = new Wind(DirectionType.Down, new Position(j, i));

                    winds.Add(wind);
                }
            }
        }

        var lastXIndex = rows[1].Count - 1;
        var lastYIndex = rows.Count - 1;
        var blizzard = new Blizzard(winds);
        var blizzards = new Dictionary<int, Blizzard>
        {
            { 0, blizzard }
        };

        return (blizzards, lastXIndex, lastYIndex);
    }

    private class Blizzard
    {
        public List<Wind> Winds { get; }

        public HashSet<Position> Positions { get; }

        public Blizzard(List<Wind> winds)
        {
            Winds = winds;
            Positions = winds.Select(i => i.Position).Distinct().ToHashSet();
        }
    }

    [DebuggerDisplay("{Position}")]
    private class Wind
    {
        public DirectionType Direction { get; }

        public Position Position { get; private set; }

        public Wind(DirectionType direction, Position position)
        {
            Direction = direction;
            Position = position;
        }

        public Wind AdvanceTime(int lastXIndex, int lastYIndex)
        {
            Position position;

            if (Direction == DirectionType.Top)
            {
                position = new Position(Position.X, Position.Y - 1);
            }
            else if (Direction == DirectionType.Down)
            {
                position = new Position(Position.X, Position.Y + 1);
            }
            else if (Direction == DirectionType.Left)
            {
                position = new Position(Position.X - 1, Position.Y);
            }
            else if (Direction == DirectionType.Right)
            {
                position = new Position(Position.X + 1, Position.Y);
            }
            else
            {
                throw new Exception();
            }

            if (position.X == -1)
            {
                position = new Position(lastXIndex, position.Y);
            }
            else if (position.X == lastXIndex + 1)
            {
                position = new Position(0, position.Y);
            }

            if (position.Y == -1)
            {
                position = new Position(position.X, lastYIndex);
            }
            else if (position.Y == lastYIndex + 1)
            {
                position = new Position(position.X, 0);
            }

            return new Wind(Direction, position);
        }
    }

    [DebuggerDisplay("{Position}({Time})")]
    private class State
    {
        public int Time { get; set; }

        public Position Position { get; set; } = null!;

        public string Key => $"({Position.X},{Position.Y}):{Time}";
    }

    private record Position(int X, int Y);

    private enum DirectionType
    {
        Top,
        Down,
        Left,
        Right,
    }
}
