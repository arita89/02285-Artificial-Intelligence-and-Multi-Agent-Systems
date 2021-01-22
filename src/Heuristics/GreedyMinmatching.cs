using System.Collections.Generic;

namespace DeepMinds
{
    static class GreedyMinmatching
    {
        public static Dictionary<Box, Goal> FindMatching(State state, Dictionary<Goal, int[,]> distancesToGoal)
        {
            // A queue of edges, each edge is between a box and a goal
            var queue = new PriorityQueue<Edge>(new GreedyMinmatchingComparer(distancesToGoal));

            var matching = new Dictionary<Box, Goal>();
            var matchedBoxes = new HashSet<Box>();
            var matchedGoals = new HashSet<Goal>();

            // Populate queue with edges 
            foreach (var (box, boxPosition) in state.BoxesPositions)
            {
                // There may be a box without any goal
                if (state.GoalsForBox.ContainsKey(box))
                {
                    var allGoalsForBox = state.GoalsForBox[box];

                    foreach (var goal in allGoalsForBox)
                    {
                        queue.Enqueue(new Edge() { Box = box, BoxPosition = boxPosition, Goal = goal });
                    }
                }
            }

            while (!queue.IsEmpty())
            {
                var edge = queue.Dequeue();
                var box = edge.Box;
                var boxPosition = edge.BoxPosition;
                var goal = edge.Goal;

                if (!matchedBoxes.Contains(box) && !matchedGoals.Contains(goal))
                {
                    matching.Add(box, goal);
                    matchedBoxes.Add(box);
                    matchedGoals.Add(goal);
                }
            }

            return matching;
        }

        class GreedyMinmatchingComparer : IComparer<Edge>
        {
            private readonly Dictionary<Goal, int[,]> distancesToGoal;

            public GreedyMinmatchingComparer(Dictionary<Goal, int[,]> distancesToGoal)
            {
                this.distancesToGoal = distancesToGoal;
            }

            public int Compare(Edge e1, Edge e2)
            {
                var firstWeight = GetWeight(e1);
                var secondWeight = GetWeight(e2);

                if (firstWeight == secondWeight)
                    return 0;
                if (firstWeight > secondWeight)
                    return 1;
                return -1;
            }

            private int GetWeight(Edge edge)
            {
                var distances = distancesToGoal[edge.Goal];
                return distances[edge.BoxPosition.X, edge.BoxPosition.Y];
            }
        }

        class Edge
        {
            public Box Box { get; set; }
            public Position BoxPosition { get; set; }
            public Goal Goal { get; set; }
        }
    }
}