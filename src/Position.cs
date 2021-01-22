using System;

namespace DeepMinds
{

    struct Position
    {
        public int X;
        public int Y;

        public int GetManhattanDistanceTo(Position p)
        {
            return Math.Abs(p.X - X) + Math.Abs(p.Y - Y);
        }

        public int GetPythagoreanDistanceTo(Position p)
        {
            return (int)Math.Sqrt(Math.Pow(p.X - X, 2) + Math.Pow(p.Y - Y, 2));
        }
    }
}