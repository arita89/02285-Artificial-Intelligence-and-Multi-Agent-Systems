using System.Collections.Generic;
using DeepMinds.Actions;

namespace DeepMinds.Exceptions
{
    class InvalidJointActionException : System.Exception
    {
        public InvalidJointActionException(List<Action> jointAction) : base($"Invalid action: {jointAction.ToString()}") { }
    }
}
