using System;
using System.Collections.Generic;

namespace DeepMinds
{
    // TODO:
    // - dictionary <color, agents, boxes, goals>

    class LevelAnalyzed : Level
    {
        public bool IsMultiAgent { get; private set; }
        public int NumberOfObstacles { get; private set; }
        public List<Agent> Agents { get; private set; }
        public List<Box> Boxes { get; private set; }
        public List<Goal> Goals { get; private set; }
        public Dictionary<string, List<Object>> ObjectsOfColor;

        public LevelAnalyzed(
            Level level,
            bool isMultiAgent,
            int numberOfObstacles,
            List<Agent> agents,
            List<Box> boxes,
            List<Goal> goals,
            Dictionary<string, List<Object>> objectsOfColor)
            : base(level.Name, level.Map, level.Map_array, level.GoalsMap, level.ColorOfObject)
        {
            IsMultiAgent = isMultiAgent;
            NumberOfObstacles = numberOfObstacles;
            Agents = agents;
            Boxes = boxes;
            Goals = goals;
            ObjectsOfColor = objectsOfColor;

            Console.WriteLine();

            Console.WriteLine("Level Analyzer");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Is multi agent: {isMultiAgent}");
            Console.WriteLine("Number of agents: {0}", agents.Count);
            /*foreach (var a in agents)
            {
                Console.WriteLine(a.Symbol.ToString());
            }*/
            Console.WriteLine("Number of boxes: {0}", boxes.Count);
            /*foreach (var b in boxes)
            {
            Console.WriteLine(b.Symbol.ToString());
            }*/
            Console.WriteLine("Number of goals: {0}", goals.Count);
            /*foreach (var g in goals)
            {
                Console.WriteLine($"{g.Location.X} , {g.Location.Y}");
            }*/

            Console.WriteLine($"Number of obstacles: {numberOfObstacles}");
            Console.WriteLine();

            Console.WriteLine($"Number of colors: {objectsOfColor.Keys.Count }");
            foreach (var k in objectsOfColor.Keys)
            {Console.WriteLine($"{k}"); }
            Console.WriteLine();

            //number of rows and columns
            Console.WriteLine($"number of columns:{level.Columns};  number of rows: {level.Rows}"); 
            Console.WriteLine();

            // this returns the type of the cell
            //foreach(var cell in level.Map)
            //{ Console.WriteLine(cell);}

            level.PrintMarks();
            Console.WriteLine();
            Console.WriteLine("---------------------");
        }
    }


    class LevelAnalyzer
    {
        public static LevelAnalyzed Analyze(Level level)
        {
            var isMultiAgent = level.Name.StartsWith("MA");
            int numberOfObstacles = 0;
            var agents = new List<Agent>();
            var boxes = new List<Box>();
            var goals = new List<Goal>();
            var objectsOfColor = new Dictionary<string, List<Object>>();

            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    var cell = level.Map[i, j];
                    var goal = level.GoalsMap[i, j];

                    if (cell is Wall)
                        numberOfObstacles++;
                    else if (cell is Agent agent)
                        agents.Add(agent);
                    else if (cell is Box box)
                        boxes.Add(box);

                    if (goal is Goal)
                    {
                        goals.Add(goal);
                        AddColoredObject(goal, objectsOfColor);
                    }

                    if (cell is Agent || cell is Box)
                        AddColoredObject(cell as ColoredObject, objectsOfColor);
                }
            }

            return new LevelAnalyzed(level, isMultiAgent, numberOfObstacles, agents, boxes, goals, objectsOfColor);
        }

        private static void AddColoredObject(ColoredObject coloredObject, Dictionary<string, List<Object>> objectsOfColor)
        {
            var color = coloredObject.Color;

            if (objectsOfColor.ContainsKey(color))
                objectsOfColor[color].Add(coloredObject);
            else
                objectsOfColor[color] = new List<Object>() { coloredObject };
        }
    }
}