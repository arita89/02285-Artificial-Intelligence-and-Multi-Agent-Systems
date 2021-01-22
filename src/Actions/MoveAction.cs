using System.Linq;
using DeepMinds.Exceptions;
using DeepMinds.Strategies;

namespace DeepMinds.Actions
{
    class MoveAction : Action
    {
        public readonly Direction MoveDirection;

        public MoveAction(Agent agent, Direction direction) : base(agent)
        {
            MoveDirection = direction;
        }

        public override Action Clone()
        {
            return new MoveAction(Agent, MoveDirection);
        }

        public override State CreateStateWithAppliedAction(State state, Position agentPos)
        {
            var newAgentPosition = ActionGenerator.GeneratePositionFromDirection(agentPos, MoveDirection);

            var freeCell = state.Map[newAgentPosition.X, newAgentPosition.Y] as FreeCell;

            state.Map[agentPos.X, agentPos.Y] = freeCell;
            state.Map[newAgentPosition.X, newAgentPosition.Y] = Agent;

            state.AgentsPositions[Agent] = newAgentPosition;
            state.ActionsUsedToReachState.Add(this);

            return state;
        }

        /// <summary>
        /// Check whether the future position is not occupied and is withing the board bounds
        /// </summary>        
        public override bool IsApplicable(Object[,] map, Position agentPos)
        {
            return CheckIfDestinationInBoundsAndHas<FreeCell>(map, agentPos, MoveDirection).isCorrect;
        }

        public override string ToString()
        {
            return $"Move({MoveDirection})";
        }
    }
}
