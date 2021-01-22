using System;
using System.Collections.Generic;
using System.Linq;
using DeepMinds.Actions;

namespace DeepMinds
{
    interface IHeuristicValue
    {
        int h(Heuristic heuristic, State state);
    }

    abstract class Heuristic : IComparer<State>
    {
        private readonly IHeuristicValue heuristicValue;
        protected Func<Heuristic, State, int> h => heuristicValue.h;

        public LevelAnalyzed Level { get; private set; }
        public Dictionary<Goal, int[,]> DistancesToGoal { get; private set; }

        public Heuristic(LevelAnalyzed level, IHeuristicValue heuristicValue)
        {
            this.Level = level;
            this.heuristicValue = heuristicValue;

            // TODO: Should we always calculate distances here?
            CalculateDistancesToGoals();
        }

        /// <summary>
        /// Calculates distances from all positions to all goals
        /// </summary>
        private void CalculateDistancesToGoals()
        {
            DistancesToGoal = new Dictionary<Goal, int[,]>();

            foreach (var goal in Level.AllGoals)
            {
                // Omit if there aren't any boxes for the goal
                if (!Level.Boxes.Any(box => box.Symbol == goal.Symbol))
                    continue;

                var distances = new int[Level.Rows, Level.Columns];

                for (var i = 0; i < Level.Rows; i++)
                    for (var j = 0; j < Level.Columns; j++)
                        distances[i, j] = int.MaxValue;

                distances[goal.Position.X, goal.Position.Y] = 0;

                var Q = new Queue<Position>();
                Q.Enqueue(goal.Position);

                while (Q.Any())
                {
                    var position = Q.Dequeue();

                    foreach (var direction in ActionGenerator.AllDirections)
                    {
                        var boxPosition = ActionGenerator.GeneratePositionFromDirection(position, direction);

                        // check if in bounds
                        if (boxPosition.X < 0 || boxPosition.X >= Level.Rows || boxPosition.Y < 0 || boxPosition.Y >= Level.Columns)
                            continue;

                        if (distances[boxPosition.X, boxPosition.Y] == int.MaxValue)
                        {
                            if (!(Level.Map[boxPosition.X, boxPosition.Y] is Wall))
                            {
                                distances[boxPosition.X, boxPosition.Y] = distances[position.X, position.Y] + 1;
                                Q.Enqueue(boxPosition);
                            }
                        }
                    }
                }

                DistancesToGoal[goal] = distances;
            }
        }

        public abstract int f(State state);

        public int Compare(State s1, State s2)
        {
            return f(s1) + s1.Penalty - f(s2) - s2.Penalty;
        }
    }

    class AStar : Heuristic
    {
        public AStar(LevelAnalyzed level, IHeuristicValue heuristicValue) : base(level, heuristicValue) { }

        public override int f(State state)
        {
            var stateH = state.h ?? h(this, state);
            state.h = stateH;
            return state.g + stateH;
        }
    }
}
