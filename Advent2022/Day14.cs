namespace Advent2022;

internal class Day14
{
    private PointState[][] _state = null!;
    private int _minX;
    private int _maxX;
    private Point _source = null!;
    private bool _hasFloor;
    private bool _shouldDraw = false;

    public void Run()
    {
        var input = File.ReadAllLines("Day14Input.txt");
        /*input = new[]
        {
            "498,4 -> 498,6 -> 496,6",
            "503,4 -> 502,4 -> 502,9 -> 494,9",
        };*/

        //Part1(input);

        Part2(input);
    }

    private void Part1(string[] input)
    {
        CreateInitialState(input);

        var fallenOff = false;
        var rockCount = 0;
        while (!fallenOff)
        {
            rockCount++;
            AddRock();

            var rockLocation = _source;

            while (true)
            {
                var fallResult = TryRockFall(rockLocation!, out var newLocation);

                rockLocation = newLocation;

                if (fallResult == RockFallResult.FallenOff)
                {
                    fallenOff = true;
                    break;
                }
                else if (fallResult == RockFallResult.NotPossible)
                {
                    break;
                }
            }
        }

        Console.WriteLine(rockCount - 1);
    }

    public void Part2(string[] input)
    {
        _hasFloor = true;
        CreateInitialState(input);

        var fallenOff = false;
        var rockCount = 0;
        while (!fallenOff)
        {
            if (_state[_source.Y][_source.X] == PointState.Rock)
            {
                break;
            }

            rockCount++;
            AddRock();

            var rockLocation = _source;

            while (true)
            {
                var fallResult = TryRockFall(rockLocation!, out var newLocation);

                rockLocation = newLocation;

                if (fallResult == RockFallResult.FallenOff)
                {
                    fallenOff = true;
                    break;
                }
                else if (fallResult == RockFallResult.NotPossible)
                {
                    break;
                }
            }
        }

        Console.WriteLine(rockCount);
    }

    private RockFallResult TryRockFall(Point currentLocation, out Point? newLocation)
    {
        var possibleMoves = new[] {
            new Point
            {
                X = currentLocation.X,
                Y = currentLocation.Y + 1
            },
            new Point
            {
                X = currentLocation.X - 1,
                Y = currentLocation.Y + 1
            },
            new Point
            {
                X = currentLocation.X + 1,
                Y = currentLocation.Y + 1
            }
        };

        foreach (var move in possibleMoves)
        {
            var (fallResult, finalMove) = GetFallResult(currentLocation, move);

            if (fallResult == RockFallResult.FallenOff)
            {
                newLocation = null;
                return fallResult;
            }
            else if (fallResult == RockFallResult.NotPossible)
            {
                continue;
            }

            newLocation = finalMove;
            return fallResult;
        }

        newLocation = currentLocation;
        return RockFallResult.NotPossible;
    }

    private (RockFallResult, Point) GetFallResult(Point currentLocation, Point nextLocation)
    {
        if (nextLocation.Y < 0  || nextLocation.X < 0 ||
            nextLocation.Y >= _state.Length || nextLocation.X >= _state[nextLocation.Y].Length)
        {
            if (_hasFloor)
            {
                (nextLocation, currentLocation) = CreateBiggerState(currentLocation, nextLocation);
            }
            else
            {
                _state[currentLocation.Y][currentLocation.X] = PointState.Empty;
                return (RockFallResult.FallenOff, nextLocation);
            }
        }

        if (_state[nextLocation.Y][nextLocation.X] == PointState.Empty)
        {
            _state[currentLocation.Y][currentLocation.X] = PointState.Empty;
            _state[_source.Y][_source.X] = PointState.Source;

            _state[nextLocation.Y][nextLocation.X] = PointState.Rock;

            return (RockFallResult.Possible, nextLocation);
        }

        return (RockFallResult.NotPossible, nextLocation);
    }

    private void AddRock()
    {
        _state[_source.Y][_source.X] = PointState.Rock;
    }

    private void CreateInitialState(string[] input)
    {
        var points = new List<List<(int X, int Y)>>();
        _maxX = 0;
        _minX = int.MaxValue;
        var maxY = 0;
        var minY = 0;
        foreach (var row in input)
        {
            var rowPoints = new List<(int, int)>();
            var cordinatesString = row.Split(" -> ");
            foreach (var cordinateString in cordinatesString)
            {
                var cordinate = cordinateString.Split(',');
                var x = int.Parse(cordinate[0]);
                var y = int.Parse(cordinate[1]);

                if (x > _maxX)
                {
                    _maxX = x;
                }
                if (x < _minX)
                {
                    _minX = x;
                }
                if (y > maxY)
                {
                    maxY = y;
                }
                rowPoints.Add((x, y));
            }

            points.Add(rowPoints);
        }

        if (500 > _maxX)
        {
            _maxX = 500;
        }
        if (500 < _minX)
        {
            _minX = 500;
        }

        var xCount = _maxX - _minX + 1;
        int yCount;
        if (_hasFloor)
        {
            yCount = maxY - minY + 1 + 2;
        }
        else
        {
            yCount = maxY - minY + 1;
        }

        _state = new PointState[yCount][];
        for (var i = 0; i < _state.Length; i++)
        {
            _state[i] = new PointState[xCount];
        }

        if (_hasFloor)
        {
            for (var i = 0; i < _state[yCount - 1].Length; i++)
            {
                _state[yCount - 1][i] = PointState.Wall;
            }
        }

        _state[0][500 - _minX] = PointState.Source;

        foreach (var row in points)
        {
            for (int i = 0; i < row.Count - 1; i++)
            {
                var start = new Point
                {
                    X = row[i].X - _minX,
                    Y = row[i].Y - minY
                };
                var end = new Point
                {
                    X = row[i+1].X - _minX,
                    Y = row[i+1].Y - minY
                };
                var line = GetLineBetween(start, end);
                foreach (var point in line)
                {
                    _state[point.Y][point.X] = PointState.Wall;
                }
            }
        }

        _source = new Point
        {
            X = 500 - _minX,
            Y = 0
        };
    }

    private List<Point> GetLineBetween(Point first, Point second)
    {
        var result = new List<Point>();

        if (first.X == second.X)
        {
            var start = new Point
            {
                X = first.X,
                Y = Math.Min(first.Y, second.Y)
            };
            var end = new Point
            {
                X = first.X,
                Y = Math.Max(first.Y, second.Y)
            };

            for (var i = start.Y; i <= end.Y; i++)
            {
                result.Add(new Point
                {
                    X = first.X,
                    Y = i
                });
            }

            return result;
        }
        else if (first.Y == second.Y)
        {
            var start = new Point
            {
                X = Math.Min(first.X, second.X),
                Y = first.Y
            };
            var end = new Point
            {
                X = Math.Max(first.X, second.X),
                Y = first.Y
            };

            for (var i = start.X; i <= end.X; i++)
            {
                result.Add(new Point
                {
                    X = i,
                    Y = first.Y
                });
            }

            return result;
        }

        throw new Exception("Not a straight line!");
    }

    private (Point, Point) CreateBiggerState(Point currentLocation, Point nextLocation)
    {
        var newState = new PointState[_state.Length][];

        for (var i = 0; i < _state.Length; i++)
        {
            newState[i] = new PointState[_state[i].Length + 1];
        }

        var startIndex = nextLocation.X < 0 ? 1 : 0;

        for (var i = 0; i < _state.Length; i++)
        {
            for (var j = 0; j < _state[i].Length; j++)
            {
                newState[i][j + startIndex] = _state[i][j];
            }
        }

        newState[^1][0] = PointState.Wall;
        newState[^1][^1] = PointState.Wall;
        _state = newState;

        _source = new Point
        {
            X = _source.X + startIndex,
            Y = _source.Y
        };

        if (nextLocation.X < 0)
        {
            _minX--;
        }
        else
        {
            _maxX++;
        }

        return (new Point
        {
            X = nextLocation.X + startIndex,
            Y = nextLocation.Y
        },
        new Point
        {
            X = currentLocation.X + startIndex,
            Y = currentLocation.Y
        });
    }

    private void Display()
    {
        if (!_shouldDraw)
        {
            return;
        }

        for (var i = 0; i <= 2; i++)
        {
            File.AppendAllText("output.txt", "    ");
            for (int j = _minX; j <= _maxX; j++)
            {
                File.AppendAllText("output.txt", j.ToString()[i].ToString());
            }
            File.AppendAllText("output.txt", Environment.NewLine);
        }

        for (var i = 0; i < _state.Length; i++)
        {
            File.AppendAllText("output.txt", i.ToString().PadRight(4));
            for (var j = 0; j < _state[i].Length; j++)
            {
                Display(_state[i][j]);
            }
            File.AppendAllText("output.txt", Environment.NewLine);
        }

        File.AppendAllText("output.txt", Environment.NewLine);
        File.AppendAllText("output.txt", Environment.NewLine);
    }

    private static void Display(PointState pointState)
    {
        if (pointState == PointState.Empty)
        {
            File.AppendAllText("output.txt", ".");
        }
        else if (pointState == PointState.Rock)
        {
            File.AppendAllText("output.txt", "o");
        }
        else if (pointState == PointState.Wall)
        {
            File.AppendAllText("output.txt", "#");
        }
        else if (pointState == PointState.Source)
        {
            File.AppendAllText("output.txt", "+");
        }
    }

    private enum PointState
    {
        Empty,
        Wall,
        Rock,
        Source
    }

    private class Point
    {
        public required int X { get; init; }
        public required int Y { get; init; }
    }

    private enum RockFallResult
    {
        Possible,
        NotPossible,
        FallenOff
    }
}
