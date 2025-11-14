// See https://aka.ms/new-console-template for more information
// Assumption: M = rows, N = base width. Triangle increases stars evenly per row.
using System;
using System.Linq;

public class TriangleWithDimension
{
    public static void DisplayTriangle3x4() =>
        // Loop through each row (1 to 3) and calculate stars: (i * 4 + 2) / 3 ensures stars are distributed evenly.
        Enumerable.Range(1, 3).ToList().ForEach(i => Console.WriteLine(new string('*', (i * 4 + 2) / 3)));

    public static void DisplayCustomTriangle(int m, int n) =>
        // Loop through each row (1 to m) and calculate stars: (i * n + m - 1) / m ensures proper distribution.
        Enumerable.Range(1, m).ToList().ForEach(i => Console.WriteLine(new string('*', (i * n + m - 1) / m)));
}