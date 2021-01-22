using System.Linq;

namespace DeepMinds
{
    class ManhattanDistanceHeuristic : IHeuristicValue
    {
        public int h(Heuristic heuristic, State state)
        {
            var value = 0;

            foreach (var (agent, agentPosition) in state.AgentsPositions)
            {
                foreach (var (box, boxPosition) in state.BoxesPositions)
                {
                    if (box.Color != agent.Color || !state.GoalsForBox.ContainsKey(box))
                        continue;

                    value += agentPosition.GetManhattanDistanceTo(boxPosition);

                    var allGoalsForBox = state.GoalsForBox[box];
                    // TODO: Find a goal for the box in a more appropriate way
                    var firstGoal = allGoalsForBox.First();

                    value += boxPosition.GetManhattanDistanceTo(firstGoal.Position);
                }
            }

            return value;
        }
    }
}