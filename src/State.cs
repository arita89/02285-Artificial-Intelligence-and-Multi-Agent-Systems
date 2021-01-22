using System.Collections.Generic;
using System.Linq;
using DeepMinds.Actions;

namespace DeepMinds
{
    class State : System.ICloneable
    {
        public int? UniqueHash { get; set; }
        public int? h { get; set; }
        public int Penalty {get; set;}

        public Object[,] Map { get; set; }

        public Dictionary<Agent, Position> AgentsPositions { get; set; }
        public Dictionary<Box, Position> BoxesPositions { get; private set; }
        public Dictionary<Box, List<Goal>> GoalsForBox { get; private set; }
        public List<Goal> AllGoals { get; private set; }
        public int NumberOfAgents => AgentsPositions.Count;

        public Position AgentPosition => AgentsPositions.First().Value;

        public List<State> AchievableStates { get; set; }
        public List<Action> ActionsUsedToReachState { get; set; }

        // The cost of reaching this state from the initial state in the search
        // Step-cost assumed to be 1
        public int g { get; private set; } = 0;

        public State(
            Object[,] map,
            Dictionary<Agent, Position> agentsPositions = null,
            Dictionary<Box, Position> boxesPositions = null,
            Dictionary<Box, List<Goal>> goalsForBox = null,
            List<Goal> allGoals = null,
            List<Action> actionsUsedToReachState = null)
        {
            Map = map;
            AgentsPositions = agentsPositions;
            BoxesPositions = boxesPositions;
            GoalsForBox = goalsForBox;
            AllGoals = allGoals;
            ActionsUsedToReachState = actionsUsedToReachState ?? new List<Action>();
            AchievableStates = new List<State>();
        }

        public bool IsGoalState()
        {
            return GoalsForBox.All(
                goals => goals.Value.All(goal => Map[goal.Position.X, goal.Position.Y].Symbol == goal.Symbol));
        }

        public object Clone()
        {
            var map = (Object[,])Map.Clone();
            var agentsPositions = new Dictionary<Agent, Position>(AgentsPositions);
            var boxesPositions = new Dictionary<Box, Position>(BoxesPositions);
            var actionsUsedToReachState = new List<Action>(ActionsUsedToReachState);
            var clone = new State(map, agentsPositions, boxesPositions, GoalsForBox, AllGoals, actionsUsedToReachState)
            {
                g = g + 1
            };
            return clone;
        }

        public State WithFixedGoal(Goal goal)
        {
            var map = (Object[,])Map.Clone();
            var agentsPositions = new Dictionary<Agent, Position>(AgentsPositions);
            var boxesPositions = new Dictionary<Box, Position>(BoxesPositions);
            var actionsUsedToReachState = new List<Action>(ActionsUsedToReachState);
            var goalsForBox = new Dictionary<Box, List<Goal>>(GoalsForBox);
            var allGoals = new List<Goal>(AllGoals);
            var position = goal.Position;

            // Set the goal position to wall
            map[position.X, position.Y] = new Wall();

            // Remove the goal from the list of goals
            foreach (var (box, goals) in goalsForBox)
                if (goals.Contains(goal))
                    goals.Remove(goal);
            allGoals.Remove(goal);

            // Remove the box that is placed on the goal
            boxesPositions.Remove(goal.BoxForGoal);
            goalsForBox.Remove(goal.BoxForGoal);

            return new State(map, agentsPositions, boxesPositions, goalsForBox, allGoals, actionsUsedToReachState);
        }

        public void PopulateAchievableStates()
        {
            // Set of moves based on actions' rules
            var jointActions = ActionGenerator.GenerateAllPossibleActions(AgentsPositions.Keys.ToList());

            foreach (var jointAction in jointActions)
            {
                var state = (State)Clone();
                var successfulActionsAppliedCounter = 0;

                foreach (var action in jointAction)
                {
                    // Is the action applicable on the map?
                    if (action.IsApplicable(state.Map, AgentsPositions[action.Agent]))
                    {
                        state = action.CreateStateWithAppliedAction(state, AgentsPositions[action.Agent]);
                        successfulActionsAppliedCounter++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (successfulActionsAppliedCounter.Equals(jointAction.Count))
                {
                    AchievableStates.Add(state);
                }
            }
        }

        public void Print()
        {
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (Map[i, j] != null)
                        System.Console.Write(Map[i, j].Symbol);
                    else
                        System.Console.Write((char)LevelSymbol.FreeCell);
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
        }
    }

    class StateEqualityComparer : IEqualityComparer<State>
    {
        public bool Equals(State s1, State s2)
        {
            if (s1 == s2)
                return true;

            var agentPositionsEqual = s1.AgentsPositions
                .All(agent => s2.AgentsPositions[agent.Key].Equals(agent.Value));

            var boxesPositionsEqual = s1.BoxesPositions
                .All(box => s2.BoxesPositions[box.Key].Equals(box.Value));

            return agentPositionsEqual && boxesPositionsEqual;
        }

        // Java implementation of hash code
        // @Override
        // public int hashCode() {
        //     if (this._hash == 0) {
        //         final int prime = 31;
        //         int result = 1;
        //         result = prime * result + this.agentCol;
        //         result = prime * result + this.agentRow;
        //         result = prime * result + Arrays.deepHashCode(this.boxes);
        //         result = prime * result + Arrays.deepHashCode(this.goals);
        //         result = prime * result + Arrays.deepHashCode(this.walls);
        //         this._hash = result;
        //     }
        //     return this._hash;
        // }

        public int GetHashCode(State s)
        {
            if (s.UniqueHash == null)
            {
                var prime = 31;
                var agentResult = 1;
                var boxResult = 1;

                foreach (var (agent, agentPosition) in s.AgentsPositions)
                {
                    agentResult = prime * agentResult + agentPosition.Y;
                    agentResult = prime * agentResult + agentPosition.X;
                    agentResult = prime * agentResult + agent.Symbol.GetHashCode();
                }

                foreach (var (box, boxPosition) in s.BoxesPositions)
                {
                    boxResult = prime * boxResult + boxPosition.Y;
                    boxResult = prime * boxResult + boxPosition.X;
                    boxResult = prime * boxResult + box.Symbol.GetHashCode();
                }

                s.UniqueHash = agentResult * 17 + boxResult * 23;
            }

            return s.UniqueHash.Value;
        }
    }


    class StateHeuristicComparer : IComparer<State>
    {
        private readonly Heuristic heuristic;

        public StateHeuristicComparer(Heuristic heuristic)
        {
            this.heuristic = heuristic;
        }

        public int Compare(State s1, State s2)
        {
            return heuristic.Compare(s1, s2);
        }
    }
}
