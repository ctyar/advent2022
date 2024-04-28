
using System.Diagnostics;

namespace Advent2022;

internal class Day20
{
    public void Run()
    {
        var input = File.ReadAllLines("Day20Input.txt");
        /*input =
        [
            "1",
            "2",
            "-3",
            "3",
            "-2",
            "0",
            "4",
        ];*/

        //Part1(input);

        Part2(input);
    }

    private static void Part1(string[] input)
    {
        var values = GetInput(input);

        Mix(input, values);

        PrintCordinates(input, values);
    }

    private static void Part2(string[] input)
    {
        var values = GetInput(input);

        for (var i = 0; i < values.Count; i++)
        {
            values[i].Number *= 811589153;
        }

        for (var i = 1; i <= 10; i++)
        {
            Mix(input, values);
        }

        PrintCordinates(input, values);
    }

    private static void PrintCordinates(string[] input, List<Node> values)
    {
        var res = string.Join("\r\n", values.Select(i => i.Number));
        File.WriteAllText("output.txt", res);

        var sum = 0L;
        var zero = values.FindIndex(i => i.Number == 0);

        var index = (zero + 1000) % input.Length;
        Console.WriteLine(values[index].Number);
        sum += values[index].Number;

        index = (zero + 2000) % input.Length;
        Console.WriteLine(values[index].Number);
        sum += values[index].Number;

        index = (zero + 3000) % input.Length;
        Console.WriteLine(values[index].Number);
        sum += values[index].Number;

        Console.WriteLine(sum);
    }

    private static void Mix(string[] input, List<Node> values)
    {
        for (var InitialArrangementIndex = 0; InitialArrangementIndex < input.Length; InitialArrangementIndex++)
        {
            var currentIndex = GetByIndex(values, InitialArrangementIndex);
            var currentValue = values[currentIndex];

            if (currentValue.Number == 0)
            {
                continue;
            }

            var newIndex = GetNewIndex(currentIndex, currentValue.Number, values.Count);

            values.RemoveAt(currentIndex);
            values.Insert(newIndex, currentValue);
        }
    }

    private static int GetNewIndex(int currentIndex, long number, int valuesCount)
    {
        var newIndex = currentIndex + number;

        newIndex %= (valuesCount - 1);
        var result = newIndex < 0 ? newIndex + (valuesCount - 1) : newIndex;

        return (int)result;
    }

    private static int GetByIndex(List<Node> list, int index)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].InitialIndex == index)
            {
                return i;
            }
        }

        throw new Exception();
    }

    private static List<Node> GetInput(string[] input)
    {
        var list = new List<Node>();

        for (var i = 0; i < input.Length; i++)
        {
            list.Add(new Node
            {
                Number = int.Parse(input[i]),
                InitialIndex = i,
            });
        }

        return list;
    }

    [DebuggerDisplay("{Value}")]
    private class Node
    {
        public long Number { get; set; }

        public int InitialIndex { get; set; }
    }
}