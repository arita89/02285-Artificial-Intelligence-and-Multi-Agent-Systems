using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DeepMinds
{
    class A_STAR
    {
        public static void Run(LevelAnalyzed level)
        {
            Console.WriteLine();
            Console.WriteLine("---------------------");
            Console.WriteLine("Running A_STAR.cs");
            Console.WriteLine("---------------------");
            // local variables
            var Agents = level.Agents;
            var Boxes = level.Boxes;
            var Goals = level.Goals;

            /*
            //switch Target from Boxes and Goals//<------------------
            var @Switch = 0 ; 

            while (@Switch < 2)
            {
            if (@Switch == 0)
            {
            var Target = Boxes; 
            
            }
            else (@Switch == 1)
            {
            var Target = Boxes;
            }
            */

            //var Target = Boxes;
            var Target = Goals;
            // method to calculate H (eg manhattan distance)
            static int CalculateHeuristic(int x, int y, int targetX, int targetY)
            {
                return Math.Abs(targetX - x) + Math.Abs(targetY - y);
            }


            // method to get the passable cells 
            List<Location> GetPossibleDir(int x, int y)
            {
                //Console.WriteLine($"Directions Coordinates around position ({x},{y})");
                var Dir = new List<Location>();
                if (x == 0 && y>0)
                {var dir = new List<Location>()
                    {
                        new Location { X = x, Y = y - 1 }, // down
                        new Location { X = x, Y = y + 1 }, // up 
                        new Location { X = x + 1, Y = y }, // right
                        //new Location { X = x, Y = y },  // stay still
                    };
                    Dir=dir;
                }
                
                else if (y == 0 && x > 0)
                {var dir = new List<Location>()
                    {
                        new Location { X = x, Y = y + 1 }, // up 
                        new Location { X = x - 1, Y = y }, // left
                        new Location { X = x + 1, Y = y }, // right
                        //new Location { X = x, Y = y },  // stay still
                    };
                    Dir = dir;
                }

                else if ( x == 0 && y == 0)
                {var dir = new List<Location>()
                    {  
                    new Location { X = x, Y = y + 1 }, // up 
                    new Location { X = x + 1, Y = y }, // right
                    //new Location { X = x, Y = y },  // stay still
                    };
                    Dir = dir;
                }
                else 
                {var dir = new List<Location>()
                    {
                        new Location { X = x, Y = y - 1 }, // down
                        new Location { X = x, Y = y + 1 }, // up 
                        new Location { X = x - 1, Y = y }, // left
                        new Location { X = x + 1, Y = y }, // right
                        //new Location {X = x , Y = y },  // stay still
                    };
                    Dir = dir;
                }
                
                // print size of level
                //Console.WriteLine($"level size: {level.Rows}x{level.Columns}");

                List<Location> PassableDir = new List<Location>(); // free tiles
                //List<Location> MovableDir = new List<Location>(); // tiles with Box or Agent that are not free but can get free if they move
                //var PassableDir = dir.Where(l dir => is FreeCell || dir is Goal || dir is Box).ToList(); // 
                foreach (var d in Dir)
                { 
                    if ((d.Y < level.Columns) && (d.X < level.Rows ))//Console.WriteLine($"location ({d.X}, {d.Y}) is in the level");
                    {
                    var cell = level.Map[d.X, d.Y];
                    if (cell is FreeCell || cell is Goal)
                        {
                            // Console.WriteLine($"({d.X}, {d.Y}) is empty");
                            PassableDir.Add(d);
                        }
                    else if (cell is Box || cell is Agent)
                        {
                            //Console.WriteLine($"({d.X}, {d.Y}) is at the moment occupied by {cell.Symbol.ToString()}");
                            PassableDir.Add(d);
                        }
                    else
                        {
                            //Console.WriteLine($"({d.X}, {d.Y}) is an insormountable wall {cell.Symbol.ToString()}");
                        }
                    }
                }
                //Console.WriteLine(string.Join(";", PassableDir));
                return PassableDir;
            }


            // COLORS LIST-is this really needed? 
            List<string> OpenColors = new List<string>();
            //foreach (var item in level.ObjectsOfColor.Keys)
            //{ OpenColors.Add(item);}
            foreach (var a in Agents)
            { OpenColors.Add(a.Color.ToString());}
            Console.WriteLine();

            foreach (var a in Agents) 
            {
                foreach (var t in Target.Where (t => t.Color == a.Color))
                {
                Console.WriteLine($"Agent:{a.Symbol},Initial position: ({a.Location.X},{a.Location.Y}) ,Color: {a.Color}");
                Console.WriteLine($"Goal:{t.Symbol},Position: ({t.Location.X},{t.Location.Y}) ,Color: {t.Color}");
                
                dynamic current = new Node ();
                //var start = new Node (a.Location.X,a.Location.Y);
                    //start.Location.X = a.Location.X;
                var start = new Node {X = a.Location.X, Y= a.Location.Y };
                var target = new Node { X = t.Location.X, Y = t.Location.Y }; // ONE target position
                var openList = new List<Node>();
                var closedList = new List<Node>();
                int Cost = 0;

                Console.WriteLine();
                Console.WriteLine($"start Node: ({start.X},{start.Y})");
                Console.WriteLine($"target Node: ({target.X},{target.Y})");
                Console.WriteLine();

                // start by adding the original position to the open list
                openList.Add(start);

                while (openList.Count > 0 && (!closedList.Any(l => l.X == target.X && l.Y == target.Y))  )
                    {
                        //
                        //var closenode = closedList.FirstOrDefault();
                        //var topOpennode = openList.FirstOrDefault();

                        // get the node with the lowest F score
                        var lowest = openList.Min(l => l.F);
                        current = openList.First(l => l.F == lowest);
                        //Console.WriteLine();
                        //Console.WriteLine($"Current Node:({current.X},{current.Y})");

                        // add the current node to the closed list
                        closedList.Add(current);

                        // remove it from the open list
                        openList.Remove(current);

                        // if the current is the destination and we have added it to the closedList...
                        // we have found a path!
                        if (closedList.Any(l => l.X == target.X && l.Y == target.Y))
                        {
                            Console.WriteLine("SUCCESS, PATH FOUND!");
                            Console.WriteLine($"path of lenght: {closedList.Count()}");
                            var idx = 0;
                            foreach (var step in closedList)
                            {
                            Console.WriteLine($"{idx}: ({step.X},{step.Y})");  
                            idx++;
                            } 
                            break;
                        }
                        // else we continue moving...
                        // we get the possible directions where to move from the current 
                        
                        var Frontier = GetPossibleDir(current.X, current.Y);
                        //Console.WriteLine($"Frontier of {a.Symbol} in node {current}:");
                        //Console.WriteLine($"Frontier contains:");<---------------------------------
                        //foreach (var node in Frontier)
                        //{Console.WriteLine($"({node.X},{node.Y})"); }
                        //Console.WriteLine();
                        Cost++;

                        foreach (var node in Frontier)
                        {
                            // create Node.candidate from Location.node
                            var candidate = new Node { X = node.X, Y = node.Y };

                            // if this candidate is already in the closedList of Nodes, we ignore it
                            if (closedList.Any(l => l.X == candidate.X && l.Y == candidate.Y)) continue;
                            if (openList.Any(l => l.X == candidate.X && l.Y == candidate.Y)) continue;
                            
                            // if this candidate is not in the openList of Nodes, we add it
                            if (!openList.Any(l => l.X == candidate.X && l.Y == candidate.Y))

                            {
                                // and compute node cost, heuristic and overall score
                                candidate.G = Cost;
                                candidate.H = CalculateHeuristic(node.X, node.Y, t.Location.X, t.Location.Y);
                                candidate.F = candidate.G + candidate.H;
                                //node.Parent = current;

                                // and add it to the open list
                                openList.Add(candidate);

                                //Console.WriteLine($"({node.X},{node.Y}), F ={candidate.F}");
                            }
                        }
                        
                    }
                }
            }
            Console.WriteLine();
            //@Switch++;
            //}
            // SINGLE AGENT implementation

            // print element in frontier for the agent
            /*
                        foreach (var a in Agents)
                        {
                            var Frontier = GetPossibleDir(a.Location.X,a.Location.Y);
                            Console.WriteLine($"therefore number of elements in frontier for Agent{a.Symbol}: {Frontier.Count}");
                            foreach (var node in Frontier)
                                {Console.WriteLine($"({node.X},{node.Y})");}


                            // if in the frontier there is only one move possible, move there
                           // a.Location.X += 1;

                        Console.WriteLine();
                        }
            */

            // MULTIAGENT stuff
            /*
            foreach (var color in OpenColors)
            {
                // how many movement option does the agent have? 
                foreach (var a in Agents)
                {
                    if (a.Color == color)
                    {
                      Console.WriteLine($" agent {a.Symbol.ToString()} is in location : ({a.Location.X}, {a.Location.Y}) ");
                      var frontier = GetPossibleDir(a.Location.X,a.Location.Y);
                      { Console.WriteLine($"Frontier"); }
                      foreach (var node in frontier)
                      {Console.WriteLine($"{node}"); }
                      //Console.WriteLine($"Frontier :{frontier}");
                      Console.WriteLine();
                    }

                    //Console.WriteLine(color);
                    /* //calculate manhattan distance for each couple agent goal
                    foreach (var aa in Agents)
                    {
                        if (aa.Color == color) 
                        {
                            foreach (var g in Goals)
                            {
                                if (g.Color == color)
                                {var manhattan =Math.Abs(aa.Location.X - g.Location.X) + Math.Abs(aa.Location.Y - g.Location.Y);
                                    Console.WriteLine($" agent {aa.Symbol.ToString()} location , {aa.Location.X}, {aa.Location.Y} ");
                                    Console.WriteLine($" goal {g.Symbol.ToString()} location , {g.Location.X}, {g.Location.Y} ");
                                    Console.WriteLine($"between agent {aa.Symbol.ToString()} and goal {g.Symbol.ToString()} manhattan distance is {manhattan}");                             }
                                }                  
                            }//Console.WriteLine(a.Symbol.ToString(),a.Location);
                        }

                        //var matches = from val in level.ObjectsOfColor where val.Key == color select val.Value;
                        //Console.WriteLine(matches);
                        /*foreach (var match in matches)
                        {
                        Console.WriteLine(match);
                        foreach (Property prop in match)
                        {
                            // do stuff
                        }
                        }
                }

            }*/


        }

    }
}

