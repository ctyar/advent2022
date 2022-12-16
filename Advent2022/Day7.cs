namespace Advent2022;

internal class Day7
{
    public static void Run()
    {
        var input = File.ReadAllLines("Day7Input.txt");

        var root = new Node
        {
            Name = "/",
            Parrent = null
        };
        var current = root;

        foreach (var line in input)
        {
            if (line.StartsWith("$"))
            {
                var command = line.Substring(2, 2);
                if (command == "cd")
                {
                    var dirName = line[5..];
                    if (dirName == "/")
                    {
                        current = root;
                    }
                    else if (dirName == "..")
                    {
                        current = current!.Parrent;
                    }
                    else
                    {
                        current = current!.Children.First(item => item.Name == dirName);
                    }
                }
                else if (command == "ls")
                {
                    continue;
                }
            }
            else if (line.StartsWith("dir"))
            {
                var dirName = line[4..];
                current!.Children.Add(new()
                {
                    Name = dirName,
                    Parrent = current
                });
            }
            else
            {
                var infos = line.Split(" ");
                current!.Children.Add(new()
                {
                    Name = infos[1],
                    Size = int.Parse(infos[0]),
                    Parrent = current
                });
            }
        }

        var finalList = new List<Node>();
        var toBrowse = new Queue<Node>();
        toBrowse.Enqueue(root);

        while (toBrowse.Count > 0)
        {
            var node = toBrowse.Dequeue();

            if (node.Size == 0 && node.TotalSize <= 100000)
            {
                finalList.Add(node);
            }

            node.Children.ForEach(toBrowse.Enqueue);
        }

        Console.WriteLine(finalList.Sum(dir => dir.TotalSize));

        Part2(root);
    }

    public static void Part2(Node root)
    {
        var finalList = new List<Node>();
        var toBrowse = new Queue<Node>();
        toBrowse.Enqueue(root);

        while (toBrowse.Count > 0)
        {
            var node = toBrowse.Dequeue();

            if (node.Size == 0 && root.TotalSize - node.TotalSize <= 40000000)
            {
                finalList.Add(node);
            }

            node.Children.ForEach(toBrowse.Enqueue);
        }

        Console.WriteLine(finalList.OrderBy(item => item.TotalSize).First().TotalSize);
    }

    public class Node
    {
        public required string Name { get; init; }

        public int Size { get; init; }

        public required Node? Parrent { get; init; }

        public List<Node> Children { get; } = new();

        public int TotalSize => Children.Count == 0 ? Size : Children.Sum(item => item.TotalSize);
    }
}
