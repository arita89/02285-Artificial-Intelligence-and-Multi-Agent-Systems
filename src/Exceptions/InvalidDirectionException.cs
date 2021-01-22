using System;

namespace DeepMinds.Exceptions
{
    public class InvalidDirectionException : Exception
    {
        public InvalidDirectionException(Direction direction) : base($"Invalid direction: {direction.ToString()}") { }
    }
}
