using System;
using System.Collections.Generic;

namespace DeepMinds
{
    abstract class Object : IEquatable<Object>
    {
        public char Symbol { get; set; }

        public override int GetHashCode()
        {
            return Symbol;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Object);
        }

        public virtual bool Equals(Object other)
        {
            return other != null && other.Symbol == Symbol;
        }

        public class EqualityComparer : IEqualityComparer<Object>
        {
            public int GetHashCode(Object obj)
            {
                return obj.Symbol;
            }

            public bool Equals(Object x, Object y)
            {
                return x.Symbol == y.Symbol;
            }
        }
    }

    abstract class ColoredObject : Object
    {
        public string Color { get; set; }
    }

    class Agent : ColoredObject { }

    class Box : ColoredObject
    {
        // As there may be multiple boxes of the same type, we introduce a unique identifier
        public int Id { get; set; }

        public override int GetHashCode()
        {
            return Convert.ToInt32(string.Format("{0}{1}", base.GetHashCode(), Id));
        }

        public override bool Equals(Object other)
        {
            var equal = base.Equals(other) &&
                (!(other is Box box) || this.Id == box.Id);
            return equal;
        }
    }

    class Goal : ColoredObject
    {
        // As there may be multiple goals of the same type, we introduce a unique identifier
        public int Id { get; set; }

        public Position Position { get; set; }
        public bool IsBoxGoal { get; set; }
        public Box BoxForGoal { get; set; }


        public override int GetHashCode()
        {
            return Convert.ToInt32(string.Format("{0}{1}", base.GetHashCode(), Id));
        }

        public override bool Equals(Object other)
        {
            var equal = base.Equals(other) &&
                (!(other is Goal goal) || this.Id == goal.Id);
            return equal;
        }

        public static Goal FromBox(int id, Box box)
        {
            return new Goal()
            {
                Id = id,
                Symbol = box.Symbol,
                Color = box.Color,
                BoxForGoal = box,
                IsBoxGoal = true
            };
        }
    }

    class Wall : Object
    {
        public Wall()
        {
            Symbol = (char)LevelSymbol.Wall;
        }
    }

    class FreeCell : Object
    {
        public FreeCell()
        {
            Symbol = (char)LevelSymbol.FreeCell;
        }
    }
}