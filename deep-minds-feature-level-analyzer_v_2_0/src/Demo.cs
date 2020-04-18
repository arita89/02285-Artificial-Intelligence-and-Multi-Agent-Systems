using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace DeepMinds
{
    // Created this as a reference to use for future algorithms
    class Demo
    {
        public static void Run(LevelAnalyzed level)
        {



            // Version 1: create a List of ints.
            // ... Add 4 ints to it.
            var numbers = new List<int>();
            numbers.Add(2);
            numbers.Add(3);
            numbers.Add(4);
            numbers.Add(7);
            Console.WriteLine("LIST 1: " + numbers.Count);
            Console.WriteLine(numbers.Min());

            // Version 2: create a List with an initializer.
            var numbers2 = new List<int>() { 2, 2, 3, 5, 7 };
            Console.WriteLine("LIST 2: " + numbers2.Count);
            Console.WriteLine(numbers2.Min());
            /*
            //Console.WriteLine();
            //Console.WriteLine("Running DEMO.cs");
            // PRINT ALL THE AGENTS AND GOALS, WITH SYMBOLS AND LOCATIONS
            for (var r = 0; r < level.Rows; r++)
            {
                for (var c = 0; c < level.Columns; c++)
                {
                    var cell = level.Map[r, c];
                    var goalcell = level.GoalsMap[r,c];
                    //var mark = level.Map[r,c].Symbol;

                    if (cell is Agent)
                    {
                        //Console.WriteLine($"Agent {0} on position: ({r}, {c})", cell.Symbol);
                        Console.WriteLine($"Agent Symbol: {cell.Symbol}");
                        Console.WriteLine($"{cell.Location.X} , {cell.Location.Y}");
                    }
                    else if (goalcell is Goal)
                    {
                        //Console.WriteLine($"Goal {0} on position: ({r}, {c})", goalcell.Symbol);
                        Console.WriteLine($"Goal Symbol: {goalcell.Symbol}");
                        Console.WriteLine($"{goalcell.Location.X} , {goalcell.Location.Y}");
                    }

                }
            }

            // PRINT MAP ARRAY 
            /*
            for (var r = 0; r < level.Rows; r++)
            {
                for (var c = 0; c < level.Columns; c++)
                {
                    foreach (var line in level.GoalsMap)
                    {
                        Console.WriteLine($"{line}");
                    }
                }
            }

            // PRINT ALL THE CELLS IN THE MAP AND THEIR
            for (var r = 0; r < level.Rows; r++)
            {
                for (var c = 0; c < level.Columns; c++)
                {
                    foreach (var cell in level.Map)
                    {
                        Console.WriteLine($"{cell} ({cell.Location.X} , {cell.Location.Y})");
                    }
                }
            }*/
        }
    }
}