using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent2022;

internal class Day21
{
    public void Run()
    {
        var input = File.ReadAllLines("Day21Input.txt");
        /*input =
        [
            "root: pppw + sjmn",
            "dbpl: 5",
            "cczh: sllz + lgvd",
            "zczc: 2",
            "ptdq: humn - dvpt",
            "dvpt: 3",
            "lfqf: 4",
            "humn: 5",
            "ljgn: 2",
            "sjmn: drzm * dbpl",
            "sllz: 4",
            "pppw: cczh / lfqf",
            "lgvd: ljgn * ptdq",
            "drzm: hmdt - zczc",
            "hmdt: 32",
        ];*/

        //Part1(input);

        Part2(input);
    }

    private static void Part1(string[] input)
    {
        var tree = GetTree(input);
        var root = tree.First(n => n.Id == "root");

        var result = Solve(root);

        Console.WriteLine(result);
    }

    private static void Part2(string[] input)
    {
        var tree = GetTree(input);
        var root = tree.First(n => n.Id == "root");
        var humn = tree.First(n => n.Id == "humn");
        humn.Value = null;

        var res = Solve(root);
        var solution = root.LeftValue.HasValue ? root.LeftValue.Value : root.RightValue!.Value;
        var unsolved = root.Left != null ? root.Left : root.Right;

        SolveBackwards(unsolved!, solution);
    }

    private static long? Solve(Node node)
    {
        if (node.Left is not null)
        {
            node.LeftValue = Solve(node.Left);
            if (node.LeftValue is not null)
            {
                node.Left = null;
            }
        }
        if (node.Right is not null)
        {
            node.RightValue = Solve(node.Right);
            if (node.RightValue is not null)
            {
                node.Right = null;
            }
        }

        node.Solve();

        return node.Value;
    }

    private static void SolveBackwards(Node node, long value)
    {
        if (node.Left is null && node.Right is null)
        {
            Console.WriteLine(value);
            return;
        }

        var leftOrRightValue = node.SolveBackwards(value);

        if (node.Left is not null)
        {
            SolveBackwards(node.Left, leftOrRightValue);
        }
        else if (node.Right is not null)
        {
            SolveBackwards(node.Right, leftOrRightValue);
        }
    }

    private static List<Node> GetTree(string[] input)
    {
        var tree = new List<Node>();

        var numberRegex = new Regex("^\\d+$");
        foreach (var line in input)
        {
            var parts = line.Split(": ");
            var id = parts[0];

            var node = tree.FirstOrDefault(n => n.Id == id);
            if (node is null)
            {
                node = new Node
                {
                    Id = id,
                };
                tree.Add(node);
            }

            var match = numberRegex.Match(parts[1]);
            if (match.Success)
            {
                node.Value = long.Parse(match.Value);
            }
            else
            {
                var leftAndRight = Regex.Split(parts[1], " (\\+|\\-|\\*|\\/) ");
                var leftId = leftAndRight[0];
                node.Operand = leftAndRight[1].First();
                var rightId = leftAndRight[2];

                var left = tree.FirstOrDefault(n => n.Id == leftId);
                if (left is null)
                {
                    left = new Node
                    {
                        Id = leftId
                    };
                    tree.Add(left);
                }
                node.Left = left;

                var right = tree.FirstOrDefault(n => n.Id == rightId);
                if (right is null)
                {
                    right = new Node
                    {
                        Id = rightId
                    };
                    tree.Add(right);
                }
                node.Right = right;
            }
        }

        return tree;
    }

    [DebuggerDisplay("{Id}")]
    private class Node
    {
        public string Id { get; set; } = null!;
        public long? Value { get; set; }

        public long? LeftValue { get; set; }
        public long? RightValue { get; set; }
        public char? Operand { get; set; }

        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public void Solve()
        {
            if (Value is not null)
            {
                return;
            }
            else if (LeftValue is null || RightValue is null)
            {
                Value = null;
            }
            else
            {
                Value = Operand!.Value switch
                {
                    '+' => LeftValue!.Value + RightValue!.Value,
                    '-' => LeftValue!.Value - RightValue!.Value,
                    '*' => LeftValue!.Value * RightValue!.Value,
                    '/' => LeftValue!.Value / RightValue!.Value,
                    _ => throw new Exception()
                };
            }
        }

        public long SolveBackwards(long value)
        {
            if (LeftValue.HasValue)
            {
                return Operand!.Value switch
                {
                    '+' => value - LeftValue.Value,
                    '-' => LeftValue.Value - value,
                    '*' => value / LeftValue.Value,
                    '/' => value * LeftValue.Value,
                    _ => throw new Exception()
                };
            }

            return Operand!.Value switch
            {
                '+' => value - RightValue!.Value,
                '-' => value + RightValue!.Value,
                '*' => value / RightValue!.Value,
                '/' => value * RightValue!.Value,
                _ => throw new Exception()
            };
        }
    }
}