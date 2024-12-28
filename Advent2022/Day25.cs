namespace Advent2022;

internal class Day25
{
    public void Run()
    {
        var input = File.ReadAllLines("Day25Input.txt");
        /*input =
        [
            "1",
            "2",
            "1=",
            "1-",
            "10",
            "11",
            "12",
            "2=",
            "2-",
            "20",
            "1=0",
            "1-0",
            "1=11-2",
            "1-0---0",
            "1121-1110-1=0",
        ];
        input =
        [
            "1=-0-2",
            "12111",
            "2=0=",
            "21",
            "2=01",
            "111",
            "20012",
            "112",
            "1=-1=",
            "1-12",
            "12",
            "1=",
            "122",
        ];*/

        Part1(input);
    }

    private static void Part1(string[] input)
    {
        var total = 0L;

        foreach (var item in input)
        {
            var value = SnafuToInt(item);

            total += value;
        }

        Console.WriteLine(IntToSnafu(total));
    }

    private static string IntToSnafu(long value)
    {
        var snafu = string.Empty;

        var hasCarry = false;

        while (value > 0 || hasCarry)
        {
            var quotient = Math.DivRem(value, 5, out var remainder);

            if (hasCarry)
            {
                remainder++;
            }

            if (remainder <= 2)
            {
                snafu += remainder;
                hasCarry = false;
            }
            else
            {
                hasCarry = true;
                if (remainder == 3)
                {
                    snafu += "=";
                }
                else if (remainder == 4)
                {
                    snafu += "-";
                }
                else
                {
                    snafu += "0";
                }
            }

            value = quotient;
        }

        var charArray = snafu.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    private static long SnafuToInt(string snafu)
    {
        var digits = snafu.Reverse().ToArray();

        var result = 0L;
        for (var i = 0; i < digits.Length; i++)
        {
            var digit = digits[i];

            var baseValue = (long)Math.Pow(5, i);
            var digitValue = digit switch
            {
                '0' => 0,
                '1' => 1,
                '2' => 2,
                '-' => -1,
                '=' => -2,
                _ => throw new Exception(),
            };

            result += baseValue * digitValue;
        }

        return result;
    }
}
