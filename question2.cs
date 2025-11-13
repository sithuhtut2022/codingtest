// See https://aka.ms/new-console-template for more information
// Assumption: M = rows, N = base width. Triangle increases stars evenly per row.
using System;

class question2
{
    static void Main()
    {
        DisplayTriangle3x4(); // Display a triangle of 3 rows and 4 base width
        Console.WriteLine();
        DisplayCustomTriangle(5, 7); // Display a triangle with 5 rows and 7 base width (bonus example)
    }

    static void DisplayTriangle3x4() =>
        // Loop through each row (1 to 3) and calculate stars: (i * 4 + 2) / 3 ensures stars are distributed evenly.
        Enumerable.Range(1, 3).ToList().ForEach(i => Console.WriteLine(new string('*', (i * 4 + 2) / 3)));

    static void DisplayCustomTriangle(int m, int n) =>
        // Loop through each row (1 to m) and calculate stars: (i * n + m - 1) / m ensures proper distribution.
        Enumerable.Range(1, m).ToList().ForEach(i => Console.WriteLine(new string('*', (i * n + m - 1) / m)));
}
