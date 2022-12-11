namespace Advent2022;

internal class Day1
{
    public static void Run()
    {
        var input = File.ReadAllText("Day1Input.txt");

        var max = input.Split("\n\n")
            .Select(group => group
                .Split("\n")
                .Where(num => !string.IsNullOrEmpty(num))
                .Select(int.Parse)
                .Sum())
            .Max();

        Console.WriteLine(max);

        var top3 = input.Split("\n\n")
            .Select(group => group
                .Split("\n")
                .Where(num => !string.IsNullOrEmpty(num))
                .Select(int.Parse)
                .Sum())
            .OrderDescending()
            .Take(3)
            .Sum();

        Console.WriteLine(top3);
    }
}
