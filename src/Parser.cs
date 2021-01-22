using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeepMinds
{
    class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }

    class Parser : IDisposable
    {
        private readonly StreamReader reader;

        private Dictionary<char, string> colors;
        private Object[,] map;
        private Goal[,] goals;
        private Dictionary<char, bool> agentOccured;
        private Dictionary<char, int> boxOccurences;

        public Parser(Stream stream)
        {
            this.reader = new StreamReader(stream, Encoding.UTF8);
        }

        public Level Parse()
        {
            var domain = ExtractSection(Section.Name.LevelName, Section.Name.Domain, Delimiter.LineFeed);
            if (!domain.matches || domain.values.FirstOrDefault() != Section.Value.Hospital)
                throw new ParseException("Failed to parse domain.");

            var levelName = ExtractSection(Section.Name.Colors);
            if (!levelName.matches || levelName.values.Count > 1)
                throw new ParseException("Failed to parse level name.");

            var colors = ExtractSection(Section.Name.InitialState);
            var (colorsParsed, colorsError) = ParseColors(colors.values);
            if (!colors.matches || !colorsParsed)
                throw new ParseException($"Failed to parse colors. {colorsError}");

            var initialState = ExtractSection(Section.Name.GoalState);
            var (initialStateParsed, initialStateError) = ParseInitialState(initialState.values);
            if (!initialState.matches || !initialStateParsed)
                throw new ParseException($"Failed to parse initial state. {initialStateError}");

            var goalState = ExtractSection(Section.Name.End);
            var (goalStateParsed, goalStateError) = ParseGoalState(initialState.values, goalState.values);
            if (!goalState.matches || !goalStateParsed)
                throw new ParseException($"Failed to parse goal state. {goalStateError}");

            return new Level(levelName.values.First(), this.map, this.goals, this.colors);
        }

        private (bool success, string error) ParseColors(List<string> values)
        {
            colors = new Dictionary<char, string>();

            foreach (var value in values)
            {
                var colorData = value.Split(":");

                if (colorData.Length != 2)
                    return (false, "Invalid color definition.");

                var color = colorData[0];
                var objects = colorData[1].Split(", ").Select(o => o.Trim()).ToList();

                if (!Colors.Allowed.Contains(color) || objects.Count == 0)
                    return (false, $"Empty list of objects or the color '{color}' does not exist.");

                foreach (var @object in objects)
                {
                    var (isObject, type) = IsObject(@object);
                    if (isObject)
                    {
                        if (colors.ContainsKey(type.Value))
                            return (false, $"The object '{type}' has already been assigned a color.");

                        colors[type.Value] = color;
                    }
                    else
                        return (false, "Tried to assign the color to a non-object");
                }
            }

            return (true, null);
        }

        private (bool success, string error) ParseInitialState(List<string> rows)
        {
            if (rows.Count <= 2)
                return (false, "Invalid state definition.");

            var levelWidth = rows.Max(r => r.Length);
            var levelHeight = rows.Count;
            var rowIndex = 0;

            map = new Object[levelHeight, levelWidth];
            agentOccured = new Dictionary<char, bool>();
            boxOccurences = new Dictionary<char, int>();

            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    var symbol = row[i];

                    if (!IsSymbol(symbol))
                        return (false, $"Unexpected symbol '{symbol}'.");

                    if ((IsBox(symbol) || IsAgent(symbol)) && !colors.ContainsKey(symbol))
                        return (false, $"The object {symbol} does not have a color specified.");

                    map[rowIndex, i] = ToObject(symbol);

                    if (IsBox(symbol))
                    {
                        boxOccurences.TryGetValue(symbol, out var occurences);
                        boxOccurences[symbol] = occurences + 1;
                    }

                    if (IsAgent(symbol))
                    {
                        if (agentOccured.ContainsKey(symbol))
                            return (false, $"The agent '{symbol}' already occured on the map.");

                        agentOccured[symbol] = true;
                    }
                }

                rowIndex++;
            }

            return (true, null);
        }

        private Object ToObject(char symbol)
        {
            if (IsAgent(symbol))
                return new Agent() { Symbol = symbol, Color = colors[symbol] };
            else if (IsBox(symbol))
            {
                boxOccurences.TryGetValue(symbol, out var occurences);
                return new Box() { Symbol = symbol, Color = colors[symbol], Id = occurences };
            }
            else if (IsWall(symbol))
                return new Wall();
            else
                return new FreeCell();
        }

        private (bool success, string error) ParseGoalState(List<string> initialStateRows, List<string> rows)
        {
            if (rows.Count <= 2)
                return (false, "Invalid state definition.");

            var levelWidth = map.GetLength(1);
            var levelHeight = map.GetLength(0);
            var rowIndex = 0;

            if (rows.Count != levelHeight)
                return (false, "Map height is different than for the initial state.");

            for (var i = 0; i < initialStateRows.Count; i++)
                if (initialStateRows[i].Length != rows[i].Length)
                    return (false, "Map width is different than for the initial state.");

            goals = new Goal[levelHeight, levelWidth];

            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    var symbol = row[i];

                    if (!IsSymbol(symbol))
                        return (false, $"Unexpected symbol '{symbol}'.");

                    if (IsBox(symbol))
                    {
                        goals[rowIndex, i] = new Goal()
                        {
                            Symbol = symbol,
                            Color = colors[symbol],
                            Position = new Position() { X = rowIndex, Y = i },
                            Id = boxOccurences[symbol]
                        };

                        if (!boxOccurences.ContainsKey(symbol) || --boxOccurences[symbol] < 0)
                            return (false, $"The box '{symbol}' occurs too many times.");
                    }

                    if (IsWall(symbol) ^ IsWall(map[rowIndex, i].Symbol))
                        return (false, "Walls should be the same as in the initial state.");

                    if (IsAgent(symbol))
                        return (false, "The agent should not occur in the goal state.");
                }

                rowIndex++;
            }

            return (true, null);
        }

        private (bool matches, List<string> values) ExtractSection(string nextSectionName, string sectionName = null, Delimiter delimiter = Delimiter.NewLine)
        {
            var matches = true;
            var values = new List<string>();
            var lines = 0;

            foreach (var line in reader.ReadLines(delimiter))
            {
                if (IsSectionHeader(line))
                {
                    matches &= (lines == 0 ? sectionName : nextSectionName) == line;
                    if (lines > 0)
                        break;
                }
                else
                    values.Add(line);

                lines++;
            }

            return (matches && lines > 0, values);
        }

        private bool IsSectionHeader(string line)
        {
            return line.StartsWith(Section.Delimiter);
        }

        private bool IsSymbol(char symbol)
        {
            return IsWall(symbol)
                || IsBox(symbol)
                || IsAgent(symbol)
                || IsFree(symbol);
        }

        private (bool isObject, char? @type) IsObject(string @object)
        {
            if (@object.Length != 1)
                return (false, null);

            var type = @object[0];
            var isObject = IsBox(type) || IsAgent(type);
            return (isObject, type);
        }

        private bool IsFree(char freeCell)
        {
            return freeCell == (char)LevelSymbol.FreeCell;
        }

        private bool IsWall(char wall)
        {
            return wall == (char)LevelSymbol.Wall;
        }

        private bool IsWall(string row)
        {
            return row.All(IsWall);
        }

        private bool IsBox(char box)
        {
            return box >= (char)LevelSymbol.BoxStart
                && box <= (char)LevelSymbol.BoxEnd;
        }

        private bool IsAgent(char agent)
        {
            return agent >= (char)LevelSymbol.AgentStart
                && agent <= (char)LevelSymbol.AgentEnd;
        }

        private bool IsGoal(char goal)
        {
            return goal >= (char)LevelSymbol.GoalStart
                && goal <= (char)LevelSymbol.GoalEnd;
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        static class Section
        {
            public const string Delimiter = "#";

            public static class Name
            {
                public const string Domain = "#domain";
                public const string LevelName = "#levelname";
                public const string Colors = "#colors";
                public const string InitialState = "#initial";
                public const string GoalState = "#goal";
                public const string End = "#end";
            }

            public static class Value
            {
                public const string Hospital = "hospital";
            }
        }
    }
}