namespace Advent2022;

internal class Day6
{
    public static void Run()
    {
        var input = File.ReadAllText("Day6Input.txt");

        for (var i = 3; i < input.Length; i++)
        {
            if (input.Substring(i - 3, 4).Distinct().Count() == 4)
            {
                Console.WriteLine(i + 1);
                break;
            }
        }

        Part2(input);
    }

    public static void Part2(string input)
    {
        for (var i = 13; i < input.Length; i++)
        {
            if (input.Substring(i - 13, 14).Distinct().Count() == 14)
            {
                Console.WriteLine(i + 1);
                break;
            }
        }
    }
}
