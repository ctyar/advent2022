using System.Diagnostics;

namespace Advent2022;

internal class Day13
{
    public static void Run()
    {
        var input = File.ReadAllLines("Day13Input.txt");

        var rightOrderIndices = new List<int>();
        var pairIndex = 1;
        for (var i = 0; i < input.Length; i+=3)
        {
            var left = input[i];
            var right = input[i+1];

            var leftTreeRoot = BuildTree(left);
            var rightTreeRoot = BuildTree(right);

            var comparison = Compare(leftTreeRoot, rightTreeRoot);
            if (comparison == ComparisonResultType.RightOrder)
            {
                rightOrderIndices.Add(pairIndex);
            }

            pairIndex++;
        }

        Console.WriteLine(rightOrderIndices.Sum());

        Part2(input);
    }

    private static void Part2(string[] input)
    {
        var packets = new List<string>();
        for (var i = 0; i < input.Length; i+=3)
        {
            var left = input[i];
            var right = input[i+1];

            packets.Add(left);
            packets.Add(right);
        }

        packets.Add("[[2]]");
        packets.Add("[[6]]");

        var sortedPackets = new List<(string, Node)>();
        foreach (var packet in packets)
        {
            sortedPackets.Add((packet, BuildTree(packet)));
        }

        sortedPackets = sortedPackets.OrderBy(item => item.Item2).ToList();

        var firstPacket = sortedPackets
            .Select((value, index) => new { value, index = index + 1 })
            .First(item => item.value.Item1 == "[[2]]")
            .index;

        var secondPacket = sortedPackets
            .Select((value, index) => new { value, index = index + 1 })
            .First(item => item.value.Item1 == "[[6]]")
            .index;

        Console.WriteLine(firstPacket * secondPacket);
    }

    private static Node BuildTree(string input)
    {
        Node root = null!;
        Node currentNode = null!;

        for (var i = 0; i < input.Length; i++)
        {
            if (input[i] == '[')
            {
                var list = new Node
                {
                    Parent = currentNode,
                    Type = NodeType.List
                };

                if (currentNode is not null)
                {
                    currentNode!.Children.Add(list);
                }

                currentNode = list;
                if (root is null)
                {
                    root = currentNode;
                }
            }
            else if (input[i] == ']')
            {
                currentNode = currentNode.Parent!;
            }
            else if (input[i] == ',')
            {
                continue;
            }
            else
            {
                var nextIndex = input.IndexOfAny(new[] { '[', ']', ',' }, i);
                if (nextIndex != -1)
                {
                    var value = int.Parse(input[i..nextIndex]);
                    var valueNode = new Node
                    {
                        Parent = currentNode,
                        Type = NodeType.Value,
                        Value = value
                    };
                    currentNode!.Children.Add(valueNode);
                }
                i = nextIndex - 1;
            }
        }

        return root;
    }

    private static ComparisonResultType Compare(Node? left, Node? right)
    {
        if (left is null && right is null)
        {
            return ComparisonResultType.Equal;
        }
        else if (left is null && right is not null)
        {
            return ComparisonResultType.RightOrder;
        }
        else if (left is not null && right is null)
        {
            return ComparisonResultType.WrongOrder;
        }
        else if (left!.Type == NodeType.Value && right!.Type == NodeType.Value)
        {
            if (left.Value == right.Value)
            {
                return ComparisonResultType.Equal;
            }
            else if (left.Value < right.Value)
            {
                return ComparisonResultType.RightOrder;
            }

            return ComparisonResultType.WrongOrder;
        }
        else if (left.Type == NodeType.List && right!.Type == NodeType.Value)
        {
            var tempNode = new Node
            {
                Parent = right.Parent,
                Type = NodeType.List,
            };
            tempNode.Children.Add(right);
            return Compare(left, tempNode);
        }
        else if (left.Type == NodeType.Value && right!.Type == NodeType.List)
        {
            var tempNode = new Node
            {
                Parent = left.Parent,
                Type = NodeType.List,
            };
            tempNode.Children.Add(left);
            return Compare(tempNode, right);
        }
        else
        {
            for (var i = 0; i < left.Children.Count; i++)
            {
                if (i >= right!.Children.Count)
                {
                    return ComparisonResultType.WrongOrder;
                }

                var comparison = Compare(left.Children[i], right.Children[i]);

                if (comparison == ComparisonResultType.Equal)
                {
                    continue;
                }
                else
                {
                    return comparison;
                }
            }

            if (left.Children.Count < right!.Children.Count)
            {
                return ComparisonResultType.RightOrder;
            }

            return ComparisonResultType.Equal;
        }
    }

    [DebuggerDisplay("{Type}{Type == NodeType.Value ? \":\" + Value.ToString() : System.String.Empty,nq}")]
    private class Node : IComparable<Node>
    {
        public required Node? Parent { get; init; }

        public List<Node> Children { get; } = new();

        public required NodeType Type { get; set; }

        public int? Value { get; set; }

        public int CompareTo(Node? other)
        {
            if (other is null)
            {
                return 1;
            }

            var comparison = Compare(this, other);
            return comparison switch
            {
                ComparisonResultType.RightOrder => -1,
                ComparisonResultType.Equal => 0,
                ComparisonResultType => 1
            };
        }
    }

    private enum NodeType
    {
        List,
        Value
    }

    private enum ComparisonResultType
    {
        Equal,
        RightOrder,
        WrongOrder
    }
}