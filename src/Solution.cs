using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepMinds.Exceptions;

namespace DeepMinds
{
    class Solution
    {
        private readonly List<List<Actions.Action>> actions;
        private readonly bool isMultiAgent;

        public Solution(List<List<Actions.Action>> actions, bool isMultiAgent)
        {
            this.actions = actions;
            this.isMultiAgent = isMultiAgent;
        }

        public void PrintSolution()
        {
            if (isMultiAgent)
            {
                PrintMultiAgentSolution();
            }
            else
            {
                PrintSingleAgentSolution();
            }
        }

        private void PrintMultiAgentSolution()
        {
            // code for approach: actions[i] is the list of all moves of agent i

            var sb = new StringBuilder();
            var noMoves = actions.First().Count;
            var noAgents = actions.Count;
            for (int i = 0; i < noMoves; i++)
            {
                for (int j = 0; j < noAgents; j++)
                {
                    var move = actions[j][i].ToString();
                    sb.Append($"{move};");
                }
                sb.Remove(sb.Length - 1, 1);
                Console.WriteLine($"#{sb.ToString()}");
                Console.WriteLine(sb.ToString());

                var response = Console.ReadLine();
                if (response.Equals("False"))
                {
                    throw new InvalidJointActionException(actions[i]);
                }
                sb.Clear();
            }

            // code for approach: actions[i] is the i'th joint action

            // foreach (var jointAction in actions)
            // {
            //     foreach (var action in jointAction)
            //     {
            //         sb.Append($"{action.ToString()};");
            //     }
            //     sb.Remove(sb.Length - 1, 1);
            //     Console.WriteLine($"#{sb.ToString()}");
            //     Console.WriteLine(sb.ToString());

            //     var response = Console.ReadLine();
            //     if (response.Equals("False"))
            //     {
            //         //throw new InvalidJointActionException(jointAction);
            //     }
            //     sb.Clear();
            // }
        }

        private void PrintSingleAgentSolution()
        {
            foreach (var action in actions.First())
            {
                // display the sent message as a comment on the server side for debugging purpose
                Console.WriteLine($"#{action.ToString()}");
                Console.WriteLine(action.ToString());

                var response = Console.ReadLine();
                if (response.Equals("False"))
                {
                    throw new InvalidActionException(action);
                }
            }
        }
    }
}
