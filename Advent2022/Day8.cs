namespace Advent2022;

internal class Day8
{
    public static void Run()
    {
        var input = File.ReadAllLines("Day8Input.txt");

        var rowLength = 0;
        var trees = new int[input.Length][];
        for (var i = 0; i < trees.Length; i++)
        {
            var row = input[i];
            if (rowLength == 0)
            {
                rowLength = row.Length;
            }
            trees[i] = new int[rowLength];

            for (var j = 0; j < rowLength; j++)
            {
                trees[i][j] = int.Parse(input[i][j].ToString());
            }
        }

        // Surrounding trees
        var visibleTreeCount = (trees.Length * 2) + 2 * (rowLength - 2);

        for (var i = 1; i < trees.Length - 1; i++)
        {
            for (var j = 1; j < rowLength - 1; j++)
            {
                var tree = trees[i][j];

                // Check top
                var isVisible = true;
                for (var x = 0; x < i; x++)
                {
                    if (trees[x][j] >= tree)
                    {
                        isVisible = false;
                        break;
                    }
                }
                if (isVisible)
                {
                    visibleTreeCount++;
                    continue;
                }

                // Check left
                isVisible = true;
                for (var x = 0; x < j; x++)
                {
                    if (trees[i][x] >= tree)
                    {
                        isVisible = false;
                        break;
                    }
                }
                if (isVisible)
                {
                    visibleTreeCount++;
                    continue;
                }

                // Check bottom
                isVisible = true;
                for (var x = i + 1; x < trees.Length; x++)
                {
                    if (trees[x][j] >= tree)
                    {
                        isVisible = false;
                        break;
                    }
                }
                if (isVisible)
                {
                    visibleTreeCount++;
                    continue;
                }

                // Check right
                isVisible = true;
                for (var x = j + 1; x < rowLength; x++)
                {
                    if (trees[i][x] >= tree)
                    {
                        isVisible = false;
                        break;
                    }
                }
                if (isVisible)
                {
                    visibleTreeCount++;
                    continue;
                }
            }
        }

        Console.WriteLine(visibleTreeCount);

        Part2(trees, rowLength);
    }

    public static void Part2(int[][] trees, int rowLength)
    {
        var visibilities = new int[trees.Length][];

        for (var i = 0; i < trees.Length; i++)
        {
            for (int j = 0; j < rowLength; j++)
            {
                var tree = trees[i][j];

                var topVisibility = i;
                for (var x = i - 1; x >= 0; x--)
                {
                    if (trees[x][j] >= tree)
                    {
                        topVisibility = i - x;
                        break;
                    }
                }

                var leftVisibility = j;
                for (var x = j - 1; x >= 0; x--)
                {
                    if (trees[i][x] >= tree)
                    {
                        leftVisibility = j - x;
                        break;
                    }
                }

                var bottomVisibility = trees.Length - i - 1;
                for (var x = i + 1; x < trees.Length; x++)
                {
                    if (trees[x][j] >= tree)
                    {
                        bottomVisibility = x - i;
                        break;
                    }
                }

                var rightVisibility = rowLength - j - 1;
                for (var x = j + 1; x < rowLength; x++)
                {
                    if (trees[i][x] >= tree)
                    {
                        rightVisibility = x - j;
                        break;
                    }
                }

                if (visibilities[i] == null)
                {
                    visibilities[i] = new int[rowLength];
                }

                visibilities[i][j] = topVisibility * bottomVisibility * rightVisibility * leftVisibility;
            }
        }

        var maxVisibility = 0;

        for (var i = 0; i < visibilities.Length; i++)
        {
            for (int j = 0; j < rowLength; j++)
            {
                if (visibilities[i][j] > maxVisibility)
                {
                    maxVisibility = visibilities[i][j];
                }
            }
        }

        Console.WriteLine(maxVisibility);
    }
}
