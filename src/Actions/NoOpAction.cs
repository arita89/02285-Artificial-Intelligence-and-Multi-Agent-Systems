using DeepMinds.Strategies;

namespace DeepMinds.Actions
{
    class NoOpAction : Action
    {
        public NoOpAction(Agent agent) : base(agent) { }

        public override Action Clone()
        {
            return new NoOpAction(Agent);
        }

        public override State CreateStateWithAppliedAction(State state, Position agentPos)
        {
            state.ActionsUsedToReachState.Add(this);
            return state;
        }

        public override bool IsApplicable(Object[,] map, Position agentPos)
        {
            return true;
        }

        public override string ToString()
        {
            return "NoOp";
        }
    }
}
