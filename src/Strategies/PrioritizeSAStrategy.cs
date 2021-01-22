using System.Collections.Generic;
using System.Linq;
using DeepMinds.Actions;

namespace DeepMinds.Strategies
{
    class PrioritizeSAStrategy : Strategy
    {
        private PriorityQueue<State> frontier;
        private HashSet<State> frontierSet;

        public PrioritizeSAStrategy(LevelAnalyzed level) : base(level) { }

        public override List<List<Action>> Solve()
        {
            // Thoughts: 
            // Goal pull distance doesn't work well when there are a lot of boxes.

            var actions = new List<Action>();
            var (found, goalsPrioritized, goalsUnprioritized) = CalculatePrioritizedGoals(level.AllGoals);
            if (!found)
            {
                var (lockingGoals, otherGoalsPrioritized) = FindLockingGoals(goalsUnprioritized);
                // TODO: What to do with locking goals?
                // Put the locking box as far as possible from both prioritized goals and their boxes?
                // Look at SARiddle.lvl and SALocking2.lvl
                throw new System.NotImplementedException("Whoops, look at those locking goals 🤭");
            }

            var currentState = new State(level.Map, level.AgentsPositions, level.BoxesPositions, level.GoalsForBox, level.AllGoals);
            State lastSolutionState = null;
            frontierSet = new HashSet<State>(new StateEqualityComparer());

            foreach (var goal in goalsPrioritized)
            {
                // 1. Get to the target box.
                // 2. Get to the target goal.
                // 3. Continue.

                var heuristic = new AStar(level, new PrioritizeHeuristic(goal));
                var exploredStates = new HashSet<State>(new StateEqualityComparer());

                if (frontier == null)
                {
                    frontier = new PriorityQueue<State>(new StateHeuristicComparer(heuristic));
                    AddToFrontier(currentState);
                }
                else
                    frontier.SetComparer(heuristic);

                var solutionFound = false;

                while (!solutionFound)
                {
                    // If frontier is empty, then no solution was found
                    if (frontier.IsEmpty())
                        break;

                    // Get the next state
                    currentState = RemoveFromFrontier();

                    // Check if we reached the box
                    if (goal.IsBoxGoal)
                    {
                        var boxFound = false;

                        // Is the box within any direction from the agent?
                        foreach (var direction in ActionGenerator.AllDirections)
                        {
                            var possibleBoxPosition = ActionGenerator.GeneratePositionFromDirection(currentState.AgentPosition, direction);
                            var goalPosition = currentState.BoxesPositions[goal.BoxForGoal];
                            if (goalPosition.Equals(possibleBoxPosition))
                            {
                                boxFound = true;
                                break;
                            }
                        }

                        if (boxFound)
                        {
                            solutionFound = true;
                            lastSolutionState = currentState;
                        }
                    }
                    // Check if we reached the goal
                    else if (currentState.Map[goal.Position.X, goal.Position.Y] is Box box && box.Symbol == goal.Symbol)
                    {
                        solutionFound = true;
                        lastSolutionState = currentState;
                    }

                    // Mark state as visited
                    exploredStates.Add(currentState);

                    // If solution for the current goal found
                    if (solutionFound)
                    {
                        frontier.Clear();
                        frontierSet.Clear();
                        // lastSolutionState.Print();

                        if (!goal.IsBoxGoal)
                        {
                            var stateWithFixedGoal = lastSolutionState.WithFixedGoal(goal);
                            // stateWithFixedGoal.Print();
                            AddToFrontier(stateWithFixedGoal);
                        }
                    }

                    // Add achievable and unvisited nodes to frontier
                    currentState.PopulateAchievableStates();
                    foreach (var state in currentState.AchievableStates)
                    {
                        if (LeadsToBlock(state))
                            state.Penalty += 100000;

                        if (!exploredStates.Contains(state) && !frontierSet.Contains(state))
                            AddToFrontier(state);
                    }
                }
            }

            return new List<List<Action>>() { lastSolutionState.ActionsUsedToReachState };
        }

        private (bool found, IEnumerable<Goal> goalsPrioritized, IEnumerable<Goal> goalsUnprioritized) CalculatePrioritizedGoals(IEnumerable<Goal> goals)
        {
            var goalsPrioritized = new List<Goal>();
            var goalsUnprioritized = new Queue<Goal>(goals);
            var goalsUnprioritizedSet = new HashSet<Goal>(goals);
            var matchedBoxesSet = new HashSet<Box>();
            var nextBoxGoal = goals.Count() + 1;

            var counter = 0;
            var initialLength = goalsUnprioritized.Count;
            while (goalsUnprioritized.Any())
            {
                if (counter == initialLength)
                {
                    // counter = 0;
                    // initialLength = goalsUnprioritized.Count;
                    return (false, goalsPrioritized, goalsUnprioritized);
                }

                var goal = goalsUnprioritized.Dequeue();
                goalsUnprioritizedSet.Remove(goal);

                var (isGoalReachable, box) = CheckIfGoalReachable(goal, goalsUnprioritizedSet, matchedBoxesSet);

                // If the goal is not reachable, leave it for later
                if (!isGoalReachable)
                {
                    counter++;
                    goalsUnprioritized.Enqueue(goal);
                    goalsUnprioritizedSet.Add(goal);
                    continue;
                }

                counter = 0;
                initialLength = goalsUnprioritized.Count;

                // The agent should go the the box before moving it to the goal
                // Therefore, we create a box goal
                var boxGoal = Goal.FromBox(nextBoxGoal++, box);
                goal.BoxForGoal = box;

                goalsPrioritized.Insert(0, goal);
                goalsPrioritized.Insert(0, boxGoal);

                matchedBoxesSet.Add(box);
            }

            return (true, goalsPrioritized, goalsUnprioritized);
        }

        private (IEnumerable<Goal> lockingGoals, IEnumerable<Goal> otherGoalsPrioritized) FindLockingGoals(IEnumerable<Goal> goalsUnprioritized)
        {
            var maxLockingGoals = 10; // Up to 10 locking goals = 1024 sets
            var sets = Subsets(goalsUnprioritized, maxLockingGoals);

            for (var i = 1; i <= maxLockingGoals; i++)
            {
                var currentSets = sets.Where(s => s.Count == i); // Sets of length i

                foreach (var set in currentSets)
                {
                    var goals = goalsUnprioritized.Except(set);
                    var (found, goalsPrioritized, _) = CalculatePrioritizedGoals(goals);
                    if (found)
                    {
                        System.Console.WriteLine("Found solution for locking goals: " + string.Join(", ", set.Select(g => g.Symbol)));
                        System.Console.WriteLine(string.Join(", ", goalsPrioritized.Where(g => !g.IsBoxGoal).Select(g => g.Symbol)));
                        return (set, goalsPrioritized);
                    }
                    else
                    {
                        System.Console.WriteLine("No solution for locking goals: " + string.Join(", ", set.Select(g => g.Symbol)));
                    }
                }
            }

            return (null, null);
        }

        public static List<List<T>> Subsets<T>(IEnumerable<T> sequence, int maxSubsetSize)
        {
            // Generate list of sequences containing only 1 element
            var oneElemSequences = sequence.Select(x => new[] { x }).ToList();

            // Generate list of int sequences
            var result = new List<List<T>>();

            // Add initial empty set
            result.Add(new List<T>());

            // generate powerset, but skip sequences that are too long
            foreach (var oneElemSequence in oneElemSequences)
            {
                int length = result.Count;

                for (int i = 0; i < length; i++)
                {
                    if (result[i].Count >= maxSubsetSize)
                        continue;

                    result.Add(result[i].Concat(oneElemSequence).ToList());
                }
            }

            return result;
        }

        private (bool isReachable, Box closestBox) CheckIfGoalReachable(Goal goal, HashSet<Goal> otherUnprioritizedGoals, HashSet<Box> matchedBoxesSet)
        {
            var visited = new bool[level.Rows, level.Columns];
            visited[goal.Position.X, goal.Position.Y] = true;

            var Q = new Queue<Position>();
            Q.Enqueue(goal.Position);

            while (Q.Any())
            {
                var position = Q.Dequeue();

                foreach (var direction in ActionGenerator.AllDirections)
                {
                    var boxPosition = ActionGenerator.GeneratePositionFromDirection(position, direction);

                    // Check if in bounds
                    if (boxPosition.X < 0 || boxPosition.X >= level.Rows || boxPosition.Y < 0 || boxPosition.Y >= level.Columns)
                        continue;

                    if (!visited[boxPosition.X, boxPosition.Y])
                    {
                        var @object = level.Map[boxPosition.X, boxPosition.Y];
                        var goalObject = level.GoalsMap[boxPosition.X, boxPosition.Y];

                        // Return if the goal is reachable
                        if (@object is Box box && box.Symbol == goal.Symbol && !matchedBoxesSet.Contains(box))
                            return (true, box);

                        // what if @object is an Agent?
                        if (@object is Wall)
                            continue;

                        if (goalObject is Goal potentialUnprioritizedGoal &&
                            otherUnprioritizedGoals.Contains(potentialUnprioritizedGoal))
                            continue;

                        visited[boxPosition.X, boxPosition.Y] = true;
                        Q.Enqueue(boxPosition);
                    }
                }
            }

            return (false, null);
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

        private bool LeadsToBlock(State state)
        {
            var action = state.ActionsUsedToReachState.Last();
            var x = state.AgentPosition.X;
            var y = state.AgentPosition.Y;

            var wallCounter = 0;
            var boxCounter = 0;
            if (action is PullAction)
            {
                if (state.Map[x + 1, y] is Wall)
                    wallCounter++;
                if (state.Map[x, y + 1] is Wall)
                    wallCounter++;
                if (state.Map[x, y - 1] is Wall)
                    wallCounter++;
                if (state.Map[x - 1, y] is Wall)
                    wallCounter++;

                if (state.Map[x + 1, y] is Box)
                    boxCounter++;
                if (state.Map[x, y + 1] is Box)
                    boxCounter++;
                if (state.Map[x, y - 1] is Box)
                    boxCounter++;
                if (state.Map[x - 1, y] is Box)
                    boxCounter++;

                return boxCounter + wallCounter == 4 && wallCounter > 1;
            }
            return false;
        }
    }
}
