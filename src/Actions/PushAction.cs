using DeepMinds.Strategies;

namespace DeepMinds.Actions
{
    class PushAction : Action
    {
        public readonly Direction AgentDirection;
        public readonly Direction BoxDirection;

        public PushAction(Agent agent, Direction agentDirection, Direction boxDirection) : base(agent)
        {
            AgentDirection = agentDirection;
            BoxDirection = boxDirection;
        }

        public bool AreDirectionsValid()
        {
            if (AgentDirection.Equals(Direction.N) && BoxDirection.Equals(Direction.S) ||
                AgentDirection.Equals(Direction.S) && BoxDirection.Equals(Direction.N) ||
                AgentDirection.Equals(Direction.E) && BoxDirection.Equals(Direction.W) ||
                AgentDirection.Equals(Direction.W) && BoxDirection.Equals(Direction.E))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override Action Clone()
        {
            var clone = new PushAction(Agent, AgentDirection, BoxDirection);
            clone.AgentNextPosition = AgentNextPosition;
            clone.BoxNextPosition = BoxNextPosition;
            clone.BoxPrevPosition = BoxPrevPosition;
            return clone;
        }

        /// <summary>
        /// Create a new state in which the new action is applied
        /// </summary>
        public override State CreateStateWithAppliedAction(State state, Position agentPos)
        {
            var freeCell = state.Map[BoxNextPosition.X, BoxNextPosition.Y] as FreeCell;
            var box = state.Map[BoxPrevPosition.X, BoxPrevPosition.Y] as Box;

            state.Map[BoxNextPosition.X, BoxNextPosition.Y] = box;
            state.Map[BoxPrevPosition.X, BoxPrevPosition.Y] = Agent;
            state.Map[agentPos.X, agentPos.Y] = freeCell;

            state.AgentsPositions[Agent] = AgentNextPosition;
            state.BoxesPositions[box] = BoxNextPosition;
            state.ActionsUsedToReachState.Add(this);
            return state;
        }

        /// <summary>
        /// Check whether:
        ///     - the future agent position is occupied by box
        ///     - the future box position is not occupied and within the bounds 
        /// </summary> 
        public override bool IsApplicable(Object[,] map, Position agentPos)
        {
            var agentCondition = CheckIfDestinationInBoundsAndHas<Box>(map, agentPos, AgentDirection);
            if (!agentCondition.isCorrect)
                return false;

            // Check if agent/box colors are the same
            var boxPosition = agentCondition.destination;
            var box = (map[boxPosition.X, boxPosition.Y] as Box);

            if (box.Color == Agent.Color)
            {
                AgentNextPosition = boxPosition;
                BoxPrevPosition = boxPosition;
                var boxCondition = CheckIfDestinationInBoundsAndHas<FreeCell>(map, BoxPrevPosition, BoxDirection);
                BoxNextPosition = boxCondition.destination;
                return boxCondition.isCorrect;
            }

            return false;
        }

        public override string ToString()
        {
            return $"Push({AgentDirection},{BoxDirection})";
        }
    }
}
