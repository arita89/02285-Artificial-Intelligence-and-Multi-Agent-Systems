using System;
using System.Collections.Generic;

namespace DeepMinds
{
    class ClosestMatchHeuristic : IHeuristicValue
    {
        public int h(Heuristic heuristic, State state)
        //Heuristic that starts by finding the closest match between a box and a goal of the same type.
        //Then ignoring this box and goal, it finds the new closest match between box and goal.
        //It continues like this until no box/goal is left and it returns the sum of the distances.
        {
            int value = 0;
            int numBoxes = state.BoxesPositions.Count;
            int numGoals = state.AllGoals.Count;

            //Matrix for all distances between boxes and goals
            int[,] distances = new int[numBoxes, numGoals];

            int i = 0;
            int j = 0;

            foreach (var (box, boxPosition) in state.BoxesPositions)
            {
                j = 0;
                foreach (var goal in state.AllGoals)
                {
                    if (box.Color != goal.Color)
                    {
                        //If the colors are different, we set the distance to int.MaxValue
                        //to avoid they get paired
                        distances[i, j] = int.MaxValue;
                    }
                    else
                    {
                        distances[i, j] = boxPosition.GetManhattanDistanceTo(goal.Position);
                    }
                    j += 1;
                }
                i += 1;
            }

            int minDistance;
            int iMin = 0;
            int jMin = 0;
            //Box-indices for boxes already paired
            var iSkip = new HashSet<int>();
            //Goal-indices for goals already paired
            var jSkip = new HashSet<int>();

            var iterations = Math.Min(numBoxes, numGoals);

            for (int n = 0; n < iterations; n++)
            {
                minDistance = int.MaxValue;

                for (i = 0; i < numBoxes; i++)
                {
                    if (iSkip.Contains(i)) { continue; }

                    for (j = 0; j < numGoals; j++)
                    {
                        if (jSkip.Contains(i)) { continue; }

                        if (distances[i, j] < minDistance)
                        {
                            minDistance = distances[i, j];
                            iMin = i;
                            jMin = j;
                        }
                    }
                }

                //Add the distance and save the indices of the pair
                value += minDistance;
                iSkip.Add(iMin);
                jSkip.Add(jMin);
            }

            return value;
        }
    }
}