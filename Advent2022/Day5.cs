namespace Advent2022;

internal class Day5
{
    public static void Run()
    {
        var input = File.ReadAllLines("Day5Input.txt");

        var (stacks, orders) = ReadStacks(input);

        foreach (var order in orders)
        {
            var parts = order.Split(' ');

            var count = int.Parse(parts[1]);
            var source = int.Parse(parts[3]) - 1;
            var destination = int.Parse(parts[5]) - 1;

            foreach (var _ in Enumerable.Range(1, count))
            {
                var cargo = stacks[source].Pop();
                stacks[destination].Push(cargo);
            }
        }

        foreach (var stack in stacks)
        {
            Console.Write(stack.Pop());
        }
        Console.WriteLine();

        Part2(input);
    }

    public static void Part2(string[] input)
    {
        var (stacks, orders) = ReadStacks(input);

        foreach (var order in orders)
        {
            var parts = order.Split(' ');

            var count = int.Parse(parts[1]);
            var source = int.Parse(parts[3]) - 1;
            var destination = int.Parse(parts[5]) - 1;

            var toMove = new List<char>();
            foreach (var _ in Enumerable.Range(1, count))
            {
                toMove.Add(stacks[source].Pop());
            }
            toMove.Reverse();
            foreach (var cargo in toMove)
            {
                stacks[destination].Push(cargo);
            }
        }

        foreach (var stack in stacks)
        {
            Console.Write(stack.Pop());
        }
        Console.WriteLine();

        Console.WriteLine("");
    }

    private static (Stack<char>[], string[]) ReadStacks(string[] input)
    {
        var stackDiagram = new List<string>();

        foreach (var line in input)
        {
            if (string.IsNullOrEmpty(line))
            {
                break;
            }

            stackDiagram.Add(line);
        }

        var stackCount = stackDiagram.Last().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        var stacks = new Stack<char>[stackCount.Length];
        for (var i = 0; i < stacks.Length; i++)
        {
            stacks[i] = new Stack<char>();
        }

        stackDiagram.RemoveAt(stackDiagram.Count - 1);
        stackDiagram.Reverse();
        foreach (var stackLine in stackDiagram)
        {
            for (var i = 0; i < stacks.Length; i++)
            {
                var charIndex = (i * 4) + 1;
                if (stackLine[charIndex] != ' ')
                {
                    stacks[i].Push(stackLine[charIndex]);
                }
            }
        }

        var ordersIndex = stacks.Length + 1;
        var orders = input[ordersIndex..];

        return (stacks, orders);
    }
}
