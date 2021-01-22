using System.Collections.Generic;
using System.Linq;

namespace DeepMinds
{
    class PrioritizeHeuristic : IHeuristicValue
    {
        private readonly Goal goalToPrioritize;
        private Dictionary<Box, Goal> matching;

        public PrioritizeHeuristic(Goal goalToPrioritize)
        {
            this.goalToPrioritize = goalToPrioritize;
        }

        public int h(Heuristic heuristic, State state)
        {
            var value = 0;

            // How often should we recalculate matching? For now it's once.
            if (matching == null)
                matching = GreedyMinmatching.FindMatching(state, heuristic.DistancesToGoal);

            // How many goals are left?
            foreach (var (box, goal) in matching)
            {
                if (goal == goalToPrioritize) continue;

                if (state.BoxesPositions.ContainsKey(box))
                {
                    var boxPosition = state.BoxesPositions[box];
                    value += heuristic.DistancesToGoal[goal][boxPosition.X, boxPosition.Y];
                }
            }

            // Is box goal or normal goal?
            if (goalToPrioritize.IsBoxGoal)
            {
                var boxForGoal = goalToPrioritize.BoxForGoal;

                if (state.BoxesPositions.ContainsKey(boxForGoal))
                {
                    var boxPosition = state.BoxesPositions[boxForGoal];
                    value += state.AgentPosition.GetManhattanDistanceTo(boxPosition);
                }
            }
            else
            {
                var boxForGoal = goalToPrioritize.BoxForGoal;

                // If there is no box in the boxes positions, then
                // that means we already matched it to the goal and removed it.
                // So we don't want to increase the heuristic for that case.

                if (state.BoxesPositions.ContainsKey(boxForGoal))
                {
                    var boxPosition = state.BoxesPositions[boxForGoal];
                    var distanceToGoal = heuristic.DistancesToGoal[goalToPrioritize];
                    value += state.AgentPosition.GetManhattanDistanceTo(boxPosition);
                    value += distanceToGoal[boxPosition.X, boxPosition.Y];
                }
            }

            return value;
        }
    }
}