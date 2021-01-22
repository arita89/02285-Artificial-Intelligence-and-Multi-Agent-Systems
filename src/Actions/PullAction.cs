using DeepMinds.Strategies;

namespace DeepMinds.Actions
{
    class PullAction : Action
    {
        public readonly Direction AgentDirection;
        public readonly Direction BoxDirection;

        public PullAction(Agent agent, Direction agentDirection, Direction boxDirection) : base(agent)
        {
            AgentDirection = agentDirection;
            BoxDirection = boxDirection;
        }

        public bool AreDirectionsValid()
        {
            if (AgentDirection.Equals(BoxDirection))
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
            var clone = new PullAction(Agent, AgentDirection, BoxDirection);
            clone.AgentNextPosition = AgentNextPosition;
            clone.BoxNextPosition = BoxNextPosition;
            clone.BoxPrevPosition = BoxPrevPosition;
            return clone;
        }

        public override State CreateStateWithAppliedAction(State state, Position agentPos)
        {
            var freeCell = state.Map[AgentNextPosition.X, AgentNextPosition.Y] as FreeCell;
            var box = state.Map[BoxPrevPosition.X, BoxPrevPosition.Y] as Box;

            state.Map[BoxPrevPosition.X, BoxPrevPosition.Y] = freeCell;
            state.Map[agentPos.X, agentPos.Y] = box;
            state.Map[AgentNextPosition.X, AgentNextPosition.Y] = Agent;

            state.AgentsPositions[Agent] = AgentNextPosition;
            state.BoxesPositions[box] = BoxNextPosition;
            state.ActionsUsedToReachState.Add(this);

            return state;
        }

        /// <summary>
        /// Check whether:
        ///     - the future agent position is not occupied and within the bounds
        ///     - the future box position is currently occupied by the agent
        /// </summary>   
        public override bool IsApplicable(Object[,] map, Position agentPos)
        {
            var boxCondition = CheckIfDestinationInBoundsAndHas<Box>(map, agentPos, BoxDirection);
            if (!boxCondition.isCorrect)
                return false;

            // Check if agent/box colors are the same
            var boxPosition = boxCondition.destination;
            var box = (map[boxPosition.X, boxPosition.Y] as Box);

            if (box.Color == Agent.Color)
            {
                BoxPrevPosition = boxPosition;
                BoxNextPosition = agentPos;
                var agentCondition = CheckIfDestinationInBoundsAndHas<FreeCell>(map, agentPos, AgentDirection);
                AgentNextPosition = agentCondition.destination;
                return agentCondition.isCorrect;
            }

            return false;
        }

        public override string ToString()
        {
            return $"Pull({AgentDirection},{BoxDirection})";
        }
    }
}
