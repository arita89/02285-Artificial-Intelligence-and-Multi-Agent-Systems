using System;
using System.Collections.Generic;

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
        public Goal[,] GoalsMap { get; private set; } // The map containing goals
        public Dictionary<char, string> ColorOfObject { get; private set; } // The colors of boxes and agents

        public int Rows => Map.GetLength(0);
        public int Columns => Map.GetLength(1);

        public Level(string name, Object[,] map, Goal[,] goalsMap, Dictionary<char, string> colorOfObject)
        {
            Name = name;
            Map = map;
            GoalsMap = goalsMap;
            ColorOfObject = colorOfObject;
        }

        public void Print()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Map[i, j] != null)
                        Console.Write(Map[i, j].Symbol);
                    else
                        Console.Write((char)LevelSymbol.FreeCell);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}