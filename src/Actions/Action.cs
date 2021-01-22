using System;
using DeepMinds.Exceptions;
using DeepMinds.Strategies;

namespace DeepMinds.Actions
{
    abstract class Action
    {
        public Position AgentNextPosition { get; set; }
        public Agent Agent { get; set; }
        public Position BoxNextPosition { get; set; }
        public Position BoxPrevPosition { get; set; }

        public Action(Agent agent)
        {
            Agent = agent;
        }

        public abstract Action Clone();
        public abstract bool IsApplicable(Object[,] map, Position agentPos);
        public abstract State CreateStateWithAppliedAction(State state, Position agentPos);

        public (bool isCorrect, Position destination) CheckIfDestinationInBoundsAndHas<T>(
            Object[,] map,
            Position pos,
            Direction direction) where T : Object
        {
            var destination = ActionGenerator.GeneratePositionFromDirection(pos, direction);

            var mapWidth = map.GetLength(0);
            var mapHeight = map.GetLength(1);

            if (destination.X < 0 || destination.X >= mapWidth || destination.Y < 0 || destination.Y >= mapHeight)
            {
                return (false, destination);
            }

            return (map[destination.X, destination.Y] is T, destination);
        }
    }
}
