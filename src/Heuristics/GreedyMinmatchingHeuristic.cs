using System.Collections.Generic;

namespace DeepMinds
{
    class GreedyMinmatchingHeuristic : IHeuristicValue
    {
        private Dictionary<Box, Goal> matching;

        public int h(Heuristic heuristic, State state)
        {
            var distancesToGoal = heuristic.DistancesToGoal;
            var value = 0;

            // How often should we recalculate matching? For now it's once.
            if (matching == null)
                matching = GreedyMinmatching.FindMatching(state, distancesToGoal);

            // Distance from the agent to not matched boxes
            foreach (var (agent, agentPosition) in state.AgentsPositions)
            {
                foreach (var (box, boxPosition) in state.BoxesPositions)
                {
                    // If the agent can't move the box
                    if (agent.Color != box.Color)
                        continue;

                    // If the box doesn't have a goal or is already on the goal position
                    if (!matching.ContainsKey(box) || matching[box].Position.Equals(boxPosition))
                        continue;

                    value += agentPosition.GetManhattanDistanceTo(boxPosition);
                }
            }

            // Make agent-boxes distance heuristic less important than boxes-goals distance heuristic
            // value /= 2;

            // Distances from boxes to goals from the matching
            foreach (var (box, goal) in matching)
            {
                var boxPosition = state.BoxesPositions[box];
                value += distancesToGoal[goal][boxPosition.X, boxPosition.Y];
            }

            return value;
        }
    }
}