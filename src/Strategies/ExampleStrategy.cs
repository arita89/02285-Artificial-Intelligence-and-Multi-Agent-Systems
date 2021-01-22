using System.Collections.Generic;
using DeepMinds.Actions;

namespace DeepMinds.Strategies
{
    /// <summary>
    /// Naive strategy picking the next state which is the best according to the heuristic. 
    /// Moves one agent      
    /// </summary>
    class ExampleStrategy : Strategy
    {
        public ExampleStrategy(LevelAnalyzed level) : base(level) { }

        public override List<List<Action>> Solve()
        {
            // var actions = new List<List<Action>>();

            // var goalState = new State(level.GoalsMap);
            // var initialState = new State(level.Map, level.AgentsPositions, level.BoxesPositions, level.GoalsForBox);

            // // best state from the list of the achievable states
            // var bestState = initialState;
            // var currentState = initialState;

            // while (true)
            // {
            //     currentState.PopulateAchievableStates();

            //     foreach (var state in currentState.AchievableStates)
            //     {
            //         if (HeuristicCalculation(state, goalState) < HeuristicCalculation(bestState, goalState))
            //         {
            //             bestState = state;
            //         }
            //     }

            //     // TODO: fix this checking
            //     if (bestState.Map.Equals(goalState.Map) || bestState.Map.Equals(currentState.Map))
            //     {
            //         break;
            //     }
            //     else
            //     {
            //         actions.Add(bestState.ActionsUsedToReachState);
            //         currentState = bestState;
            //     }
            // }

            // return actions;

            return null;
        }

        private double HeuristicCalculation(State state, State goalState)
        {
            return new System.Random().Next(0, 100);
        }
    }
}
