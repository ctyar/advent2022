using System.Linq.Expressions;

namespace Advent2022;

internal class Day11
{
    public static void Run()
    {
        var input = File.ReadAllLines("Day11Input.txt");

        var monkeyBusiness = CalculateMonkeyBusiness(20, 3, input);

        Console.WriteLine(monkeyBusiness);

        Part2(input);
    }

    private static void Part2(string[] input)
    {
        var monkeyBusiness = CalculateMonkeyBusiness(10000, 1, input);

        Console.WriteLine(monkeyBusiness);
    }

    private static long CalculateMonkeyBusiness(int rounds, int relief, string[] input)
    {
        var monkeys = InitializeMonkeys(input);

        var commonMultiple = monkeys.Select(item => item.Test).Aggregate((a, b) => a * b);

        foreach (var _ in Enumerable.Range(1, rounds))
        {
            foreach (var monkey in monkeys)
            {
                foreach (var item in monkey.Items.ToList())
                {
                    var newWorryLevel = monkey.Operation(item) % commonMultiple;
                    newWorryLevel /= relief;

                    int nextMonkeyId;
                    if (newWorryLevel % monkey.Test == 0)
                    {
                        nextMonkeyId = monkey.IfTrue;
                    }
                    else
                    {
                        nextMonkeyId = monkey.IfFalse;
                    }

                    var nextMonkey = monkeys.First(item => item.Id == nextMonkeyId);
                    nextMonkey.Items.Add(newWorryLevel);

                    monkey.TotalInspectedItems++;
                    monkey.Items.RemoveAt(0);
                }
            }
        }

        monkeys = monkeys.OrderByDescending(item => item.TotalInspectedItems).ToList();

        return monkeys[0].TotalInspectedItems * monkeys[1].TotalInspectedItems;
    }

    private static List<Monkey> InitializeMonkeys(string[] input)
    {
        var result = new List<Monkey>();

        for (var i = 0; i < input.Length; i+=7)
        {
            var id = int.Parse(input[i].Split(' ')[1].Replace(":", ""));
            var items = input[i + 1].Split(':')[1].Replace(" ", "").Split(',').Select(long.Parse).ToList();
            var operation = GetOperation(input[i + 2].Split('=')[1].TrimStart());
            var test = int.Parse(input[i + 3].Split(' ').Last());
            var ifTrue = int.Parse(input[i + 4].Split(' ').Last());
            var ifFalse = int.Parse(input[i + 5].Split(' ').Last());

            result.Add(new Monkey
            {
                Id = id,
                Items = items,
                Operation = operation,
                Test = test,
                IfTrue = ifTrue,
                IfFalse = ifFalse
            });
        }

        return result;
    }

    private static Func<long, long> GetOperation(string input)
    {
        var tokens = input.Split(' ');

        ParameterExpression left = default!;
        if (tokens[0] == "old")
        {
            left = Expression.Parameter(typeof(long), "old");
        }

        Expression right;
        if (tokens[2] == "old")
        {
            right = left;
        }
        else
        {
            right = Expression.Constant(long.Parse(tokens[2]), typeof(long));
        }

        BinaryExpression finalExpression = default!;
        if (tokens[1] == "+")
        {
            finalExpression = Expression.Add(left, right);
        }
        else if (tokens[1] == "-")
        {
            finalExpression= Expression.Subtract(left, right);
        }
        else if (tokens[1] == "*")
        {
            finalExpression = Expression.Multiply(left, right);
        }
        else if (tokens[1] == "/")
        {
            finalExpression = Expression.Divide(left, right);
        }

        return Expression.Lambda<Func<long, long>>(finalExpression, left).Compile();
    }

    private class Monkey
    {
        public required int Id { get; init; }

        public required List<long> Items { get; init; }

        public required Func<long, long> Operation { get; init; }

        public required int Test { get; init; }

        public required int IfTrue { get; init; }

        public required int IfFalse { get; init; }

        public long TotalInspectedItems { get; set; }
    }
}
