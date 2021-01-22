using System.Collections.Generic;
using DeepMinds.Actions;

namespace DeepMinds.Strategies
{
    class AStarSAStrategy : Strategy
    {
        private readonly IHeuristicValue heuristicValue;
        private readonly AStar heuristic;

        private PriorityQueue<State> frontier;
        private HashSet<State> frontierSet;

        public AStarSAStrategy(LevelAnalyzed level, IHeuristicValue heuristicValue) : base(level)
        {
            this.heuristicValue = heuristicValue;
            this.heuristic = new AStar(level, heuristicValue);
        }

        public override List<List<Action>> Solve()
        {
            frontier = new PriorityQueue<State>(new StateHeuristicComparer(heuristic));
            frontierSet = new HashSet<State>(new StateEqualityComparer());

            var actions = new List<List<Action>>();
            var initialState = new State(level.Map, level.AgentsPositions, level.BoxesPositions, level.GoalsForBox, level.AllGoals);
            var exploredStates = new HashSet<State>(new StateEqualityComparer());

            // Start from the initial state
            AddToFrontier(initialState);

            while (true)
            {
                // If frontier is empty, then no solution was found
                if (frontier.IsEmpty())
                    break;

                // Get the next state
                var currentState = RemoveFromFrontier();

                // Check if we reached the goal state
                if (currentState.IsGoalState())
                {
                    actions.Add(currentState.ActionsUsedToReachState);
                    break;
                }

                // Mark state as visited
                exploredStates.Add(currentState);

                // Add achievable and unvisited nodes to frontier
                currentState.PopulateAchievableStates();
                foreach (var state in currentState.AchievableStates)
                {
                    if (!exploredStates.Contains(state) && !frontierSet.Contains(state))
                        AddToFrontier(state);
                }
            }

            return actions;
        }

        private void AddToFrontier(State state)
        {
            frontier.Enqueue(state);
            frontierSet.Add(state);
        }

        private State RemoveFromFrontier()
        {
            var state = frontier.Dequeue();
            frontierSet.Remove(state);
            return state;
        }
    }
}
