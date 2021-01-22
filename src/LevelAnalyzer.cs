using System.Collections.Generic;
using System.Linq;

namespace DeepMinds
{
    class LevelAnalyzed : Level
    {
        public bool IsMultiAgent { get; private set; }
        public int NumberOfObstacles { get; private set; } // The number of boxes and walls on the map
        public Dictionary<string, List<Object>> ObjectsOfColor; // The list of all objects of specified color
        public List<Agent> Agents { get; private set; }
        public List<Box> Boxes { get; private set; }
        public Dictionary<Agent, Position> AgentsPositions { get; private set; }
        public Dictionary<Box, Position> BoxesPositions { get; private set; }
        public Dictionary<Box, List<Goal>> GoalsForBox { get; private set; }
        public List<Goal> AllGoals { get; private set; }

        public LevelAnalyzed(
            Level level,
            bool isMultiAgent,
            int numberOfObstacles,
            Dictionary<string, List<Object>> objectsOfColor,
            List<Agent> agents,
            List<Box> boxes,
            Dictionary<Agent, Position> agentsPositions,
            Dictionary<Box, Position> boxesPositions,
            Dictionary<Box, List<Goal>> goalsForBox,
            List<Goal> allGoals)
            : base(level.Name, level.Map, level.GoalsMap, level.ColorOfObject)
        {
            IsMultiAgent = isMultiAgent;
            NumberOfObstacles = numberOfObstacles;
            Agents = agents;
            Boxes = boxes;
            ObjectsOfColor = objectsOfColor;
            AgentsPositions = agentsPositions;
            BoxesPositions = boxesPositions;
            GoalsForBox = goalsForBox;
            AllGoals = allGoals;
        }

        // public LevelAnalyzed WithFixedGoal(Goal goal)
        // {
        //     var goalsMap = (Goal[,])GoalsMap.Clone();
        //     var map = (Object[,])Map.Clone();
        //     var goalsForBox = new Dictionary<Box, List<Goal>>(GoalsForBox);
        //     var allGoals = new List<Goal>(AllGoals);

        //     var position = goal.Position;
        //     goalsMap[position.X, position.Y] = null;
        //     map[position.X, position.Y] = new Wall();

        //     foreach (var (box, goals) in goalsForBox)
        //         if (goals.Contains(goal))
        //             goals.Remove(goal);

        //     allGoals.Remove(goal);

        //     var level = new Level(Name, map, goalsMap, ColorOfObject);

        //     return new LevelAnalyzed(
        //         level,
        //         IsMultiAgent,
        //         NumberOfObstacles,
        //         ObjectsOfColor,
        //         Agents,
        //         Boxes,
        //         AgentsPositions,
        //         BoxesPositions,
        //         goalsForBox,
        //         allGoals
        //     );
        // }
    }


    class LevelAnalyzer
    {
        public static LevelAnalyzed Analyze(Level level)
        {
            var isMultiAgent = level.Name.StartsWith("MA");
            int numberOfObstacles = 0;
            var objectsOfColor = new Dictionary<string, List<Object>>();
            var agents = new List<Agent>();
            var boxes = new List<Box>();
            var agentsPositions = new Dictionary<Agent, Position>();
            var boxesPositions = new Dictionary<Box, Position>();

            // We introduce a custom comparer that compares boxes only by their symbol (without id).
            // This way given any box with the same symbol, we receive the same list of goal positions no matter what is the id of that box.
            var goalsForBox = new Dictionary<Box, List<Goal>>(new Object.EqualityComparer());
            var allGoals = new List<Goal>();

            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    var cell = level.Map[i, j];
                    var position = new Position() { X = i, Y = j };

                    if (cell is Wall)
                        numberOfObstacles++;
                    else if (cell is Agent agent)
                    {
                        agents.Add(agent);
                        agentsPositions.Add(agent, position);
                    }
                    else if (cell is Box box)
                    {
                        boxes.Add(box);
                        boxesPositions.Add(box, position);
                    }

                    if (cell is Agent || cell is Box)
                        AddColoredObject(cell as ColoredObject, objectsOfColor);
                }
            }

            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    var goal = level.GoalsMap[i, j];

                    if (goal is Goal)
                    {
                        // We can use any of the boxes with the goal symbol. See the definition of goalsPosition above.
                        var box = boxes.Where(box => box.Symbol == goal.Symbol).FirstOrDefault();

                        if (goalsForBox.ContainsKey(box))
                            goalsForBox[box].Add(goal);
                        else
                            goalsForBox[box] = new List<Goal>() { goal };

                        allGoals.Add(goal);

                        AddColoredObject(goal, objectsOfColor);
                    }
                }
            }

            return new LevelAnalyzed(
                level,
                isMultiAgent,
                numberOfObstacles,
                objectsOfColor,
                agents,
                boxes,
                agentsPositions,
                boxesPositions,
                goalsForBox,
                allGoals
            );
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