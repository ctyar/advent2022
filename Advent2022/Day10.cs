namespace Advent2022;

internal class Day10
{
    public static void Run()
    {
        var input = File.ReadAllLines("Day10Input.txt");

        var xRegister = 1;
        var cycle = 0;
        var totalSignalStrength = 0;

        for (var i = 0; i < input.Length; i++)
        {
            if (i != 0 && input[i - 1].StartsWith("addx"))
            {
                var number = int.Parse(input[i - 1].Split(" ")[1]);
                xRegister += number;
            }

            var cycleCount = 1;

            if (input[i].StartsWith("addx"))
            {
                cycleCount = 2;
            }

            foreach (var _ in Enumerable.Range(1, cycleCount))
            {
                cycle++;

                if ((cycle - 20) % 40 == 0)
                {
                    totalSignalStrength += cycle * xRegister;
                }
            }
        }

        Console.WriteLine(totalSignalStrength);

        Part2(input);
    }

    public static void Part2(string[] input)
    {
        var xRegister = 1;
        var cycle = 0;
        var output = new char[240];

        for (var i = 0; i < input.Length; i++)
        {
            if (i != 0 && input[i - 1].StartsWith("addx"))
            {
                var number = int.Parse(input[i - 1].Split(" ")[1]);
                xRegister += number;
            }

            var cycleCount = 1;

            if (input[i].StartsWith("addx"))
            {
                cycleCount = 2;
            }

            foreach (var _ in Enumerable.Range(1, cycleCount))
            {
                cycle++;

                if (Math.Abs(((cycle - 1) % 40) - xRegister) <= 1)
                {
                    output[cycle - 1] = '#';
                }
                else
                {
                    output[cycle - 1] = '.';
                }
            }
        }

        for (int i = 0; i < output.Length; i++)
        {
            Console.Write(output[i]);

            if ((i + 1) % 40 == 0)
            {
                Console.WriteLine();
            }
        }
    }
}
