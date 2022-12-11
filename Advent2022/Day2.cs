namespace Advent2022;

internal class Day2
{
    public static void Run()
    {
        var rounds = File.ReadAllLines("Day2Input.txt");

        var states = new Dictionary<string, int>
        {
            { "A X", 4 },
            { "A Y", 8 },
            { "A Z", 3 },
            { "B X", 1 },
            { "B Y", 5 },
            { "B Z", 9 },
            { "C X", 7 },
            { "C Y", 2 },
            { "C Z", 6 },
        };

        var totalScore = rounds.Select(round => states[round]).Sum();

        Console.WriteLine(totalScore);

        Part2(rounds);
    }

    public static void Part2(string[] rounds)
    {
        var states = new Dictionary<string, int>
        {
            { "A X", 3 },
            { "A Y", 4 },
            { "A Z", 8 },
            { "B X", 1 },
            { "B Y", 5 },
            { "B Z", 9 },
            { "C X", 2 },
            { "C Y", 6 },
            { "C Z", 7 },
        };

        var totalScore = rounds.Select(round => states[round]).Sum();

        Console.WriteLine(totalScore);
    }
}
