using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepMinds
{
    enum LevelSymbol
    {
        FreeCell = ' ',
        Wall = '+',
        BoxStart = 'A',
        BoxEnd = 'Z',
        GoalStart = 'a',
        GoalEnd = 'z',
        AgentStart = '0',
        AgentEnd = '9'
    }

    class Level
    {
        public string Name { get; private set; } // The name of a level
        public Object[,] Map { get; private set; } // The map containing walls, boxes, agents
        public string[] Map_array { get; private set; } // The map array containing walls, boxes, agents
        public Goal[,] GoalsMap { get; private set; } // The map containing goals
        public Dictionary<char, string> ColorOfObject { get; private set; } // The colors of boxes and agents

        public int Rows => Map.GetLength(0);
        public int Columns => Map.GetLength(1);

        public Level(string name, Object[,] map,string[] map_array, Goal[,] goalsMap, Dictionary<char, string> colorOfObject)
        {
            Name = name;
            Map = map;
            Map_array = map_array;
            GoalsMap = goalsMap;
            ColorOfObject = colorOfObject;
        }

        public void PrintMarks()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var mark = Map[i, j].Symbol.ToString();
                    Console.Write(mark);
                    //Console.Write("type of Map = {0}", Map.GetType());
                }
                Console.WriteLine();
            }
        }

        public void PrintTypes()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    //Console.Write(Map[i, j]);
                    //Console.Write("element {0} is type = {1}",Map[i, j], Map.GetType());
                    Console.Write("in position ({0},{1}) is element = {2}",i,j, Map[i, j]);
                    Console.WriteLine();
                }
                Console.WriteLine("------------------------------------------");
            }
        }

        public void ChartoStringtoArray()
        {
            List<string> Allstrings = new List<string>();            
            for (int i = 0; i < Rows; i++)
            {
                string singlestring = "";
                {
                    for (int j = 0; j < Columns; j++)
                    {
                    var mark = Map[i, j].Symbol.ToString();
                    singlestring += (mark);
                    }
                    Allstrings.Add(singlestring);    
                }
            }
            string[] map_array = Allstrings.ToArray();
            Test(map_array);

            static void Test(string[] map_array)
            {
            //Console.WriteLine();
            Console.WriteLine("Map array with number of rows: " + map_array.Length);
            foreach (var item in map_array)
                {
                Console.WriteLine(item);
                }
            }
        }


    }
}