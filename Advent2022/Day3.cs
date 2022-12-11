namespace Advent2022;

internal class Day3
{
    public static void Run()
    {
        var rucksacks = File.ReadAllLines("Day3Input.txt");

        var prioritySum = 0;
        foreach (var rucksack in rucksacks)
        {
            var firstCompartment = rucksack.Substring(0, rucksack.Length / 2);
            var secondCompartment = rucksack.Substring(rucksack.Length / 2);

            foreach (var item in firstCompartment)
            {
                if (secondCompartment.Contains(item))
                {
                    if (char.IsAsciiLetterLower(item))
                    {
                        prioritySum += item - 'a' + 1;
                    }
                    else
                    {
                        prioritySum += item - 'A' + 27;
                    }
                    break;
                }
            }
        }

        Console.WriteLine(prioritySum);

        Part2(rucksacks);
    }

    public static void Part2(string[] rucksacks)
    {
        var prioritySum = 0;
        foreach (var group in rucksacks.Chunk(3))
        {
            foreach (var item in group[0])
            {
                if (group[1].Contains(item) && group[2].Contains(item))
                {
                    if (char.IsAsciiLetterLower(item))
                    {
                        prioritySum += item - 'a' + 1;
                    }
                    else
                    {
                        prioritySum += item - 'A' + 27;
                    }
                    break;
                }
            }
        }

        Console.WriteLine(prioritySum);
    }
}
