using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepMinds.Exceptions;

namespace DeepMinds.Actions
{
    static class ActionGenerator
    {
        public static List<Direction> AllDirections = new List<Direction>
        {
            Direction.N,
            Direction.S,
            Direction.E,
            Direction.W
        };


        public static Position GeneratePositionFromDirection(Position pos, Direction direction)
        {
            var destination = new Position
            {
                X = pos.X,
                Y = pos.Y
            };

            switch (direction)
            {
                case Direction.N:
                    destination.X--;
                    break;
                case Direction.S:
                    destination.X++;
                    break;
                case Direction.E:
                    destination.Y++;
                    break;
                case Direction.W:
                    destination.Y--;
                    break;

                default:
                    throw new InvalidDirectionException(direction);
            }

            return destination;
        }

        public static string GenerateJointAction(List<Action> actions)
        {
            var sb = new StringBuilder();

            foreach (var action in actions)
            {
                sb.Append($"<{GenerateSingleAction(action)}>;");
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static string GenerateSingleAction(Action action)
        {
            return action.ToString();
        }

        /// <summary>
        /// Creates dictionary, where key is the agent and value is every viable move (excluding collisions which are checked later)
        /// </summary>
        public static List<List<Action>> GenerateAllPossibleActions(List<Agent> agents)
        {
            var allPossibleActions = new List<List<Action>>();

            foreach (var agent in agents)
            {
                var agentPossibleActions = new List<Action>();

                // TODO: Uncomment when working on multi agent solution
                // agentPossibleActions.Add(new NoOpAction(agent));

                foreach (var agentDirection in AllDirections)
                {
                    agentPossibleActions.Add(new MoveAction(agent, agentDirection));

                    foreach (var boxDirection in AllDirections)
                    {
                        var pushAction = new PushAction(agent, agentDirection, boxDirection);
                        if (pushAction.AreDirectionsValid())
                        {
                            agentPossibleActions.Add(pushAction);
                        }

                        var pullAction = new PullAction(agent, agentDirection, boxDirection);
                        if (pullAction.AreDirectionsValid())
                        {
                            agentPossibleActions.Add(pullAction);
                        }
                    }
                }

                allPossibleActions.Add(agentPossibleActions);
            }

            return CartesianProduct<Action>(allPossibleActions);
        }

        private static List<List<T>> CartesianProduct<T>(
            this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                from accseq in accumulator from item in sequence select accseq.Concat(new[] { item })
            )
            .Select(x => x.ToList())
            .ToList();
        }
    }

}
