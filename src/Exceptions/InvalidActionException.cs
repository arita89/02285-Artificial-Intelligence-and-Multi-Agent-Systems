using DeepMinds.Actions;

namespace DeepMinds.Exceptions
{
    class InvalidActionException : System.Exception
    {
        public InvalidActionException(Action action) : base($"Invalid action: {action.ToString()}") { }
    }
}
