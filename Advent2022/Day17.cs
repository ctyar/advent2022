namespace Advent2022;

internal class Day17
{
    public void Run()
    {
        var input = File.ReadAllLines("Day17Input.txt");
        /*input = new[]
        {
            ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>",
        };*/
        var jetPattern = input.First();

        //Part1(jetPattern);

        Part2(jetPattern);
    }

    private void Part1(string input)
    {
        var chamber = new Chamber();
        chamber.Display();

        var jetPattern = input.ToCharArray();
        var jetIndex = 0;

        for (var i = 1; i <= 2022; i++)
        {
            chamber.AddRock();
            //chamber.Display();
            //Console.WriteLine();

            do
            {
                jetIndex = chamber.ApplyJet(jetPattern, jetIndex);
            }
            while (chamber.Drop());
        }

        Console.WriteLine(chamber.Count);
    }

    public void Part2(string input)
    {
        const long RockCount = 1_000_000_000_000;
        var chamber = new Chamber();
        chamber.Display();

        var jetPattern = input.ToCharArray();
        var jetIndex = 0;
        var values = new HashSet<(int, int)>();
        var periodKey = (0, 0);
        var isFirstPeriod = true;
        var firstPeriodIndex = 0L;
        var firstPeriodChamberCount = 0;
        var isSkipped = false;
        var periodLength = 0L;
        var chamberDepthPerPeriod = 0;

        for (long i = 1; i <= RockCount; i++)
        {
            if (values.Contains((jetIndex, (int)chamber.RockType)))
            {
                if (isFirstPeriod)
                {
                    // Start of first period
                    periodKey = (jetIndex, (int)chamber.RockType);
                    firstPeriodIndex = i;
                    firstPeriodChamberCount = chamber.Count;
                    isFirstPeriod = false;
                }
                else if (!isSkipped && periodKey == (jetIndex, (int)chamber.RockType))
                {
                    // Start of second period
                    periodLength = i - firstPeriodIndex;
                    chamberDepthPerPeriod = chamber.Count - firstPeriodChamberCount;

                    i += (((RockCount / periodLength) - 2) * periodLength) - 1;
                    jetIndex = periodKey.Item1;
                    isSkipped = true;
                    continue;
                }
            }
            else
            {
                values.Add((jetIndex, (int)chamber.RockType));
            }

            chamber.AddRock();

            do
            {
                jetIndex = chamber.ApplyJet(jetPattern, jetIndex);
            }
            while (chamber.Drop());
        }

        Console.WriteLine(chamber.Count + ((RockCount / periodLength - 2) * chamberDepthPerPeriod));
    }

    private class Chamber
    {
        private const int Width = 7;
        private readonly List<string> _state = [];
        private RockType _rockType = RockType.Dash;
        private bool _isFirstRock = true;
        private int _highestRock = 0;
        private List<int> _rockRows = [];

        public int Count => _highestRock;
        public RockType RockType => _rockType;

        public void Empty()
        {
            _highestRock = 0;
            _state.Clear();
        }

        public void AddRock()
        {
            if (_isFirstRock)
            {
                _isFirstRock = false;
            }
            else
            {
                _rockType++;
                _rockType = (RockType)((int)_rockType % 5);
            }

            var rowIndex = _highestRock + 3;
            var rock = GetRock(_rockType);

            _rockRows = SetRows(rock.ToList(), rowIndex);
        }

        public void SetRow(string rowValue, int rowIndex)
        {
            EnsureStateSize(rowIndex);

            _state[rowIndex] = rowValue;
        }

        public List<int> SetRows(List<string> rowValues, int firstRowIndex)
        {
            EnsureStateSize(firstRowIndex + rowValues.Count);

            var result = new List<int>();

            for (var i = 0; i < rowValues.Count; i++)
            {
                SetRow(rowValues[rowValues.Count - 1 - i], firstRowIndex + i);
                result.Add(firstRowIndex + i);
            }

            result.Reverse();

            return result;
        }

        private void EnsureStateSize(int topRowIndex)
        {
            for (var rowIndex = _state.Count; rowIndex < topRowIndex; rowIndex++)
            {
                _state.Add(".......");
            }
        }

        private static IEnumerable<string> GetRock(RockType type)
        {
            if (type == RockType.Dash)
            {
                yield return "..@@@@.";
            }
            else if (type == RockType.Plus)
            {
                yield return "...@...";
                yield return "..@@@..";
                yield return "...@...";
            }
            else if (type == RockType.L)
            {
                yield return "....@..";
                yield return "....@..";
                yield return "..@@@..";
            }
            else if (type == RockType.I)
            {
                yield return "..@....";
                yield return "..@....";
                yield return "..@....";
                yield return "..@....";
            }
            else if (type == RockType.Square)
            {
                yield return "..@@...";
                yield return "..@@...";
            }
        }

        public void Display()
        {
            for (var i = 0; i < _state.Count; i++)
            {
                Console.WriteLine(_state[_state.Count - 1 - i]);
            }
        }

        public void PushRight()
        {
            var rowsToModify = new List<string>();

            foreach (var rowIndex in _rockRows)
            {
                var row = _state[rowIndex];
                var lastAtSignIndex = row.LastIndexOf('@');
                if (lastAtSignIndex >= row.Length - 1)
                {
                    // At the edge so no pushing
                    return;
                }

                if (row[lastAtSignIndex + 1] == '#')
                {
                    // Blocked so no pushing
                    return;
                }

                var newRow = row.ToCharArray();
                var firstAtSignIndex = row.IndexOf('@');
                newRow[firstAtSignIndex] = '.';
                newRow[lastAtSignIndex + 1] = '@';

                rowsToModify.Add(new string(newRow));
            }

            SetRows(rowsToModify, _rockRows.Last());
        }

        public void PushLeft()
        {
            var rowsToModify = new List<string>();

            foreach (var rowIndex in _rockRows)
            {
                var row = _state[rowIndex];
                var firstAtSignIndex = row.IndexOf('@');
                if (firstAtSignIndex <= 0)
                {
                    // At the edge so no pushing
                    return;
                }

                if (row[firstAtSignIndex - 1] == '#')
                {
                    // Blocked so no pushing
                    return;
                }

                var newRow = row.ToCharArray();
                var lastAtSignIndex = row.LastIndexOf('@');
                newRow[firstAtSignIndex - 1] = '@';
                newRow[lastAtSignIndex] = '.';

                rowsToModify.Add(new string(newRow));
            }

            SetRows(rowsToModify, _rockRows.Last());
        }

        public bool Drop()
        {
            List<int>? indexesOfAtSignInPreviousRow = null;

            var isFloor = !TrySetDropRows(out var rowsToModify);

            if (isFloor)
            {
                RestRock();
                return false;
            }

            var newRowValues = new List<string>();

            foreach (var row in rowsToModify)
            {
                var indexesOfAtSignInCurrentRow = AllIndexesOf(row.Value, '@');

                var newValue = row.Value.Replace('@', '.');

                if (indexesOfAtSignInPreviousRow is not null)
                {
                    var newValueArray = newValue.ToCharArray();
                    foreach (var atSignIndex in indexesOfAtSignInPreviousRow)
                    {
                        if (newValueArray[atSignIndex] == '#')
                        {
                            // We hit a rock
                            RestRock();
                            return false;
                        }
                        newValueArray[atSignIndex] = '@';
                    }
                    newValue = new string(newValueArray);
                }

                newRowValues.Add(newValue);

                indexesOfAtSignInPreviousRow = indexesOfAtSignInCurrentRow;
            }

            _rockRows = rowsToModify.Skip(1).Select(i => i.Index).ToList();
            SetRows(newRowValues, rowsToModify.Last().Index);

            return true;
        }

        private void RestRock()
        {
            foreach (var rockIndex in _rockRows)
            {
                _state[rockIndex] = _state[rockIndex].Replace('@', '#');
            }

            _highestRock = Math.Max(_highestRock, _rockRows.First() + 1);
        }

        private bool TrySetDropRows(out List<(int Index, string Value)> dropRows)
        {
            dropRows = [];

            foreach (var rockRow in _rockRows)
            {
                dropRows.Add((rockRow, _state[rockRow]));
            }

            var nextIndex = dropRows.Last().Index - 1;

            if (nextIndex < 0)
            {
                // Floor
                return false;
            }
            dropRows.Add((nextIndex, _state[nextIndex]));

            return true;
        }

        private static List<int> AllIndexesOf(string str, char search)
        {
            var minIndex = str.IndexOf(search);

            var result = new List<int>();

            while (minIndex != -1)
            {
                result.Add(minIndex);
                minIndex = str.IndexOf(search, minIndex + 1);
            }

            return result;
        }

        public int ApplyJet(char[] jetPattern, int jetIndex)
        {
            jetIndex %= jetPattern.Length;

            if (jetPattern[jetIndex] == '>')
            {
                PushRight();
            }
            else
            {
                PushLeft();
            }

            return jetIndex + 1;
        }
    }

    private enum RockType : byte
    {
        Dash = 0,
        Plus = 1,
        L = 2,
        I = 3,
        Square = 4
    }
}
