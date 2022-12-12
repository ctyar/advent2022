namespace Advent2022;

internal class Day4
{
    public static void Run()
    {
        var sections = File.ReadAllLines("Day4Input.txt");

        var fullyContained = 0;
        foreach (var section in sections)
        {
            var pairs = section.Split(",");
            var first = pairs[0];
            var second = pairs[1];

            var firstStartAndFinish = first.Split("-");
            var secondStartAndFinish = second.Split("-");

            var firstStart = int.Parse(firstStartAndFinish[0]);
            var firstFinish = int.Parse(firstStartAndFinish[1]);

            var secondStart = int.Parse(secondStartAndFinish[0]);
            var secondFinish = int.Parse(secondStartAndFinish[1]);

            if (firstStart <= secondStart && secondFinish <= firstFinish)
            {
                fullyContained++;
            }
            else if (secondStart <= firstStart && firstFinish <= secondFinish)
            {
                fullyContained++;
            }
        }

        Console.WriteLine(fullyContained);

        Part2(sections);
    }

    public static void Part2(string[] sections)
    {
        var overlapped = 0;
        foreach (var section in sections)
        {
            var pairs = section.Split(",");
            var first = pairs[0];
            var second = pairs[1];

            var firstStartAndFinish = first.Split("-");
            var secondStartAndFinish = second.Split("-");

            var firstStart = int.Parse(firstStartAndFinish[0]);
            var firstFinish = int.Parse(firstStartAndFinish[1]);

            var secondStart = int.Parse(secondStartAndFinish[0]);
            var secondFinish = int.Parse(secondStartAndFinish[1]);

            if (firstStart <= secondStart && secondStart <= firstFinish)
            {
                overlapped++;
            }
            else if (firstStart <= secondFinish && secondFinish <= firstFinish)
            {
                overlapped++;
            }
            else if (secondStart <= firstStart && firstStart <= secondFinish)
            {
                overlapped++;
            }
            else if (secondStart <= firstFinish && firstFinish <= secondFinish)
            {
                overlapped++;
            }
        }

        Console.WriteLine(overlapped);
    }
}
