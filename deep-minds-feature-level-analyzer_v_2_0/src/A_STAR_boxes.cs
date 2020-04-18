using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DeepMinds
{
    class A_STAR_BOXES
    {
        public static void Run(LevelAnalyzed level)
        {
            Console.WriteLine();
            Console.WriteLine("---------------------");
            Console.WriteLine("Running A_STAR_BOXES.cs");
            Console.WriteLine("---------------------");
            // local variables
            var Agents = level.Agents;
            var Boxes = level.Boxes;
            var Goals = level.Goals;

            /*
            //for each Agent-Box-Goal triplet
            // triplet = 1;
            // List<Location> InitialPositions = new List<Location>;
            // IntialPosition.Add (start):
                // Turn 1: Agent to Box
                // start = initial position.lastinthepile
                // Turn 2: Box to Goal 
                //need to save the parent
                // InitialPosition.Add (parent)
            // triplet ++;
            var Round = 0 ; 

            */

            //var Movers = Agents;
            //var Target = Boxes;
            //var Target = Goals;

            // method to move to previous position
            static Node MoveAgentBack(Agent agent,List<Node> fullpathagent )
            {
              return  fullpathagent[fullpathagent.Count] ;
            }

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
                if (x == 0 && y > 0)
                {
                    var dir = new List<Location>()
                    {
                        new Location { X = x, Y = y - 1 }, // down
                        new Location { X = x, Y = y + 1 }, // up 
                        new Location { X = x + 1, Y = y }, // right
                        //new Location { X = x, Y = y },  // stay still
                    };
                    Dir = dir;
                }

                else if (y == 0 && x > 0)
                {
                    var dir = new List<Location>()
                    {
                        new Location { X = x, Y = y + 1 }, // up 
                        new Location { X = x - 1, Y = y }, // left
                        new Location { X = x + 1, Y = y }, // right
                        //new Location { X = x, Y = y },  // stay still
                    };
                    Dir = dir;
                }

                else if (x == 0 && y == 0)
                {
                    var dir = new List<Location>()
                    {
                    new Location { X = x, Y = y + 1 }, // up 
                    new Location { X = x + 1, Y = y }, // right
                    //new Location { X = x, Y = y },  // stay still
                    };
                    Dir = dir;
                }
                else
                {
                    var dir = new List<Location>()
                    {
                        new Location { X = x, Y = y - 1 }, // down
                        new Location { X = x, Y = y + 1 }, // up 
                        new Location { X = x - 1, Y = y }, // left
                        new Location { X = x + 1, Y = y }, // right
                        //new Location {X = x , Y = y },  // stay still
                    };
                    Dir = dir;
                }

                List<Location> PassableDir = new List<Location>(); // free tiles
                //List<Location> MovableDir = new List<Location>(); // tiles with Box or Agent that are not free but can get free if they move
                //var PassableDir = dir.Where(l dir => is FreeCell || dir is Goal || dir is Box).ToList(); // 
                foreach (var d in Dir)
                {
                    if ((d.Y < level.Columns) && (d.X < level.Rows))//Console.WriteLine($"location ({d.X}, {d.Y}) is in the level");
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
                return PassableDir;
            }
                  
            foreach (var a in Agents)
            {
                // list with all the nodes for one triplet agent-->box1-->Goal1--->box2-->Goal2 etc
                // every time a turn is over, we want to pop up the last position on this list to start the other turn
                List<Node> fullpathagent = new List<Node>();
                //List<Goal> ThisAgentGoals = Goals.Where(g => g.Color == a.Color);

                // the initial position of the agent 
                var initial = new Node();
                initial.X = a.Location.X; 
                initial.Y = a.Location.Y;
                fullpathagent.Add(initial);

                foreach (var b in Boxes.Where(b => b.Color == a.Color))
                { //OPEN FOREACH BOX
                    // *this first* has to be made better
                    // it should be probably listed with the shortest distance from the box
                    var g = Goals.First(g => g.Color == a.Color); // we take the first goal with the right color as a goal 

                    // we have now a triplet of Agent, Box and Goal with the same color
                    // we need to remove the goal from the list once its used by a box

                    // start the first turn Agent a to Box
                    var Turn = 1;

                    while (Turn<3) // stop after second turn (and start with new triplet a-b-g)
                        {
                        Console.WriteLine($"TURN: {Turn}");

                        // we want to move ONE agent to the ONE box,
                        if (Turn == 1)// this will return the path agent-box
                        { // OPEN IF  
                            var start = fullpathagent.LastOrDefault(); // NODE start of first turn is the last position in the agent path
                            var Mover = a; // the agent a moves
                            var Target = b; // toward the box b
                            var openList = new List<Node>(); // this is the open list ONLY for the path this agent to this box
                            var closedList = new List<Node>(); // this is the list of closed nodes ONLY for the path of this agent to this box 
                            var exploredList = new List<Node>();
                            int Cost = 0;

                                    //foreach (var t in Targets.Where(t => t.Color == a.Color))
                            Console.WriteLine($"Calculating A* path for Turn {Turn}, between Agent:{a.Symbol} and Box:{b.Symbol}");
                            //Console.WriteLine($"Agent:{a.Symbol},Initial position: ({a.Location.X},{a.Location.Y}) ,Color: {a.Color}");
                            //Console.WriteLine($"Box:{b.Symbol},Position: ({b.Location.X},{b.Location.Y}) ,Color: {b.Color}");

                            dynamic current = new Node();
                            var target = new Node { X = Target.Location.X, Y = Target.Location.Y }; // ONE target position as NODE

                            Console.WriteLine();
                            Console.WriteLine($"start Node: ({start.X},{start.Y})"); //NODE start 
                            Console.WriteLine($"target Node: ({target.X},{target.Y})"); // NODE target
                            Console.WriteLine();

                            // start by adding the original position to the open list
                            //openList.Add(start);
                            openList.Insert(0, start);

                            while (openList.Count > 0)
                            {//OPEN WHILE of OpenList
                                
                                Console.WriteLine("---------------------------------------");
                                // get the node from the open list with the lowest F score (the F score should be striclty decreasing following Astar algorithm)
                                
                                var lowest = openList.Min(l => l.F);
                                current = openList.First(l => l.F == lowest);
                                int minimumValueIndex = openList.IndexOf(openList.First(l => l.F == lowest));
                                Console.WriteLine($"Current Node:({current.X},{current.Y})");
                                var Frontier = GetPossibleDir(current.X, current.Y); 
                                
                                // is the next current best movement available? 
                                // if yes, add it to the closed list etc
                                // if not... we need to go back!

                                IEnumerable<Node> MIN = openList.Where(l => l.F == lowest);
                                int OpenListMin = MIN.Count();

                                // how many min in frontier? 
                                //if multiple we might want to go back here !
                                int numMin = 1;
                                foreach (var itemMIN in MIN)
                                {
                                    foreach (var itemFRONTIER in Frontier)
                                    {

                                    if (itemMIN.X == itemFRONTIER.X && itemMIN.Y == itemFRONTIER.Y )
                                    {numMin++; }  
                                    } 
                                }
                                
                                // how many times have we been in this node? 
                                // count ExploredList!
                                int countExplored = exploredList.Count(item => item.X ==current.X && item.Y ==current.Y);

                                //PRINT
                                Console.WriteLine($" min in open list : {OpenListMin}"); 
                                Console.WriteLine($" min in frontier : {numMin}"); 
                                Console.WriteLine($"We have explored this node {countExplored} times"); 

                                if ((numMin != 1) && countExplored<=numMin) // if there are multiple min we want to be able to go back to the node
                                {
                                    Console.WriteLine($"THERE ARE MULTIPLE {openList.Where(l => l.F == lowest).Count()} MIN VIABLE NODES");
                                }
                                else if ((numMin == 1) || countExplored>numMin)
                                {
                                    // remove it from the open list cause we have explored it all
                                    Console.WriteLine($"We have explored this node {countExplored} times");
                                    Console.WriteLine($"Node ({current.X},{current.Y}) removed from *openList*");
                                    openList.Remove(current);

                                    // add the current node to the closed list for this path AGENT to BOX
                                    Console.WriteLine($"Node ({current.X},{current.Y}) added to *ClosedList*");
                                    closedList.Add(current); // this list is re-initialized at every turn
                                }
                                
                                // add to the explored nodes list a priori
                                Console.WriteLine($"Node ({current.X},{current.Y}) added to *ExploredList*");
                                exploredList.Add(current); // this list is re-initialized at every turn


                                // add the current node to the general path of the Agent, we are storing all the moves
                                Console.WriteLine($"Node ({current.X},{current.Y}) added to *fullpathagent*");
                                fullpathagent.Add(current); // this list is not re-initialized

                                // if the current is the destination and we have added it to the closedList...
                                // we have found a path!
                                if (exploredList.Any(l => l.X == target.X && l.Y == target.Y))
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("SUCCESS, path from Agent to Box FOUND!");
                                    Console.WriteLine($"path of lenght (closed): {closedList.Count()}");
                                    Console.WriteLine($"path of lenght (explored): {exploredList.Count()}");
                                    openList.Clear(); // if we have found the goal we dont want to go again the the open Nodes in the openList, so we clean the openList
                                    Turn++; // we move to the second turn! to move the Box to the Goal
                                    break;
                                }

                                else // if we have not the target in the closed list, we keep computing the possible direction to move from the current
                                {
                                    
                                    //var Frontier = GetPossibleDir(current.X, current.Y);
                                    Cost++; // at each step the cost increase by one

                                    // some PRINT statement here to check whats in the Frontier
                                    //Console.WriteLine($"Frontier of {a.Symbol} in node {current}:");
                                    //Console.WriteLine($"Agent in ({fullpathagent.LastOrDefault().X},{fullpathagent.LastOrDefault().Y})");
                                    Console.WriteLine($"Frontier contains:");//<---------------------------------
                                    foreach (var node in Frontier)
                                    {
                                        Console.WriteLine($"({node.X},{node.Y})");
                                    }
                                    Console.WriteLine();

                                    foreach (var node in Frontier)
                                    {
                                        // create Node.candidate from Location.node
                                        var candidate = new Node { X = node.X, Y = node.Y };

                                        // if this candidate is already in the closedList of Nodes, we ignore it
                                        if (closedList.Any(l => l.X == candidate.X && l.Y == candidate.Y)) 
                                        {
                                            Console.WriteLine($"Node ({candidate.X},{candidate.Y})is in the closed list already");  
                                            continue;
                                        }

                                        else if (openList.Any(l => l.X == candidate.X && l.Y == candidate.Y)) 
                                        {                                    
                                            Console.WriteLine($"node ({candidate.X},{candidate.Y})already in openlist, not added to openlist ");  
                                            continue;
                                        }  
                                    
                                        
                                        // if this candidate is not in the openList of Nodes, we add it
                                        else if(!openList.Any(l => l.X == candidate.X && l.Y == candidate.Y))
                                        {
                                            Console.WriteLine($"Node ({candidate.X},{candidate.Y}) is in  NOT the open list yet, so we add it"); 
                                            // and compute node cost, heuristic and overall score
                                            candidate.G = Cost;
                                            candidate.H = CalculateHeuristic(node.X, node.Y, Target.Location.X, Target.Location.Y);
                                            candidate.F = candidate.G + candidate.H;
                                            //node.Parent = current;

                                            // and add it to the open list
                                            //openList.Add(candidate);
                                            openList.Insert(0, candidate);

                                            //Console.WriteLine($"({node.X},{node.Y}), F ={candidate.F}")
                                            //Console.WriteLine();
                                        }
                                    }
                                    Console.WriteLine();
                                    Console.WriteLine("OpenList:");
                                    foreach (var item in openList)
                                    {
                                        Console.WriteLine($"({item.X},{item.Y}), F ={item.F}"); 
                                    }
                
                                    Frontier.Clear();
                                }
                                Console.WriteLine();
                                //openList[minimumValueIndex].Explored= 1+ openList[minimumValueIndex].Explored;
                                //Console.WriteLine("-------------------------------------");
                            }// CLOSED WHILE of OpenList
                        Console.WriteLine();

                        }// CLOSED IF of Turn ==1

                        else if (Turn == 2)
                        {// OPEN  ELSE IF  

                            //var start = fullpathagent.LastOrDefault(); 
                            // NODE start of first turn is the position of the box (and the last position in the agent path)
                            var start = new Node();
                            start.X = b.Location.X; 
                            start.Y = b.Location.Y;
                            // we want to take away the last position in the fullpathagent (thats the box location!))
                            if(fullpathagent.Any()) //prevent IndexOutOfRangeException for empty list
                            {
                                fullpathagent.RemoveAt(fullpathagent.Count - 1);
                            }
                            //Console.WriteLine($"START ({start.X},{start.Y})");
                            //Console.WriteLine($"fullpathagent.Count ({fullpathagent[fullpathagent.Count-1].X},{fullpathagent[fullpathagent.Count-1].Y})");  //previous position
                            //Console.WriteLine();


                            var Mover = b; // the box is moved 
                            var Follower = a; // The position of the agent is always -1 the one of the box
                            var Target = g; // toward the goal 
                            var openList = new List<Node>(); // this is the open list ONLY for the path this box to this goal
                            var closedList = new List<Node>(); // this is the list of closed nodes ONLY for the path of this box to this goal
                            var exploredList = new List<Node>();
                            //closedList.Add(fullpathagent[fullpathagent.Count]);
                            //Console.WriteLine($"fullpathagent.Count {fullpathagent.Count}");
                            closedList.Add(fullpathagent.LastOrDefault());   
                            int Cost = 0;

                            //foreach (var t in Targets.Where(t => t.Color == a.Color))
                            Console.WriteLine($"Calculating A* path for Turn {Turn}, between Box :{b.Symbol} and Goal:{g.Symbol}");
                            //Console.WriteLine($"Box :{b.Symbol},Initial position: ({b.Location.X},{b.Location.Y}) ,Color: {b.Color}");
                            //Console.WriteLine($"Goal:{g.Symbol},Position: ({g.Location.X},{g.Location.Y}) ,Color: {g.Color}");

                            dynamic current = new Node();
                            //var turn= Turn;
                            var target = new Node { X = Target.Location.X, Y = Target.Location.Y }; // ONE target position as NODE

                            Console.WriteLine();
                            Console.WriteLine($"start Node: ({start.X},{start.Y})"); //NODE start 
                            Console.WriteLine($"target Node: ({target.X},{target.Y})"); // NODE target
                            Console.WriteLine();

                            // start by adding the original position to the open list
                            //openList.Add(start);
                            openList.Insert(0, start);

                            while (openList.Count > 0)
                            {//OPEN WHILE of OpenList

                                // get the node from the open list with the lowest F score (the F score should be striclty decreasing following Astar algorithm)
                                var lowest = openList.Min(l => l.F);
                                Console.WriteLine(lowest);
                                current = openList.First(l => l.F == lowest);
                                Console.WriteLine();
                                Console.WriteLine($"Current Node:({current.X},{current.Y})");
                                var Frontier = GetPossibleDir(current.X, current.Y); 

                                // how many min in open list? 
                                IEnumerable<Node> MIN = openList.Where(l => l.F == lowest);
                                int OpenListMin = MIN.Count(); // how many min in the open list

                                 // how many min in frontier? 
                                 //if multiple we might want to go back here !
                                int numMin = 1;
                                foreach (var itemMIN in MIN)
                                {
                                    foreach (var itemFRONTIER in Frontier)
                                    {

                                    if (itemMIN.X == itemFRONTIER.X && itemMIN.Y == itemFRONTIER.Y )
                                        {
                                        Console.WriteLine("MIN found in frontier");
                                        numMin++;
                                        
                                        }   
                                    } 
                                }
                                
                                // how many times have we been in this node? 
                                // count ExploredList!

                                int countExplored = exploredList.Count(item => item.X ==current.X && item.Y ==current.Y);

                                //PRINT
                                Console.WriteLine($" min in open list : {OpenListMin}"); 
                                Console.WriteLine($" min in frontier : {numMin}"); 
                                Console.WriteLine($"We have explored this node {countExplored} times"); 


                                if ((numMin != 1) && countExplored<=numMin) // if there are multiple min we want to be able to go back to the node
                                {
                                    Console.WriteLine($"THERE ARE MULTIPLE {openList.Where(l => l.F == lowest).Count()} MIN VIABLE NODES");
                                }
                                else if ((numMin == 1) || countExplored>numMin)
                                {
                                    // remove it from the open list cause we have explored it all
                                    Console.WriteLine();
                                    Console.WriteLine($"Node ({current.X},{current.Y}) removed from *openList*");
                                    openList.Remove(current);
                                    
                                    // add the current node to the closed list for this path AGENT to BOX
                                    // the closed list is re-initialized at every turn
                                    Console.WriteLine($"Node ({current.X},{current.Y}) added to *ClosedList*");
                                    closedList.Add(current); // this list is re-initialized at every turn
                                }

                                // add to the explored nodes list a priori
                                Console.WriteLine($"Node ({current.X},{current.Y}) added to *ExploredList*");
                                exploredList.Add(current); // this list is re-initialized at every turn   
                                

                                int currentindex = closedList.IndexOf(current); 
                                int agentindex = currentindex-1;
                                //Console.WriteLine($"current box index: {currentindex}");
                                //Console.WriteLine($"current agent index: {agentindex})";

                                // add the node previous to the current of the box to the general path of the Agent
                                Console.WriteLine($"Node ({closedList[agentindex].X},{closedList[agentindex].Y}) added to *fullpathagent*");
                                fullpathagent.Add(closedList[agentindex]); // this list is not re-initialized

                                // if the current is the destination and we have added it to the closedList...
                                // we have found a path!
                                if (exploredList.Any(l => l.X == target.X && l.Y == target.Y))
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("SUCCESS, PATH from Box to Goal FOUND!");
                                    //Console.WriteLine($"path of lenght: {closedList.Count()-1}");
                                    Console.WriteLine($"path of lenght (closed): {closedList.Count()-1}");
                                    Console.WriteLine($"path of lenght (explored): {exploredList.Count()-1}");
                                    /*
                                    var idx = 0;
                                    foreach (var step in closedList)
                                    {
                                        Console.WriteLine($"{idx}: ({step.X},{step.Y})");
                                        idx++;
                                    }
                                    */
                                    openList.Clear(); // if we have found the goal we dont want to go again the the open Nodes in the openList, so we clean the openList
                                    Turn++; // we move out of the WHILE turn
                                    break;
                                }

                                else // if we have not the target in the closed list, we keep computing the possible direction to move from the current
                                {
                                    //var Frontier = GetPossibleDir(current.X, current.Y);
                                    Cost++; // at each step the cost increase by one

                                    // some PRINT statement here to check whats in the Frontier
                                    //Console.WriteLine($"Frontier of {a.Symbol} in node {current}:");
                                    Console.WriteLine($"Frontier contains:");//<---------------------------------
                                    foreach (var node in Frontier)
                                    {Console.WriteLine($"({node.X},{node.Y})"); }
                                    Console.WriteLine();

                                    foreach (var node in Frontier)
                                    {
                                        // create Node.candidate from Location.node
                                        var candidate = new Node { X = node.X, Y = node.Y };

                                        // if this candidate is already in the closedList of Nodes, we ignore it
                                        if (closedList.Any(l => l.X == candidate.X && l.Y == candidate.Y)) 
                                        {
                                            Console.WriteLine($"node ({candidate.X},{candidate.Y})already in closed list, not added to openlist ");  
                                            continue;
                                        }                                        
                                        
                                        //continue;
                                        else if (openList.Any(l => l.X == candidate.X && l.Y == candidate.Y))
                                        {                                    
                                            Console.WriteLine($"node ({candidate.X},{candidate.Y})already in openlist, not added to openlist ");  
                                            continue;
                                        }  

                                        // if this candidate is not in the openList of Nodes, we add it
                                        else if (!openList.Any(l => l.X == candidate.X && l.Y == candidate.Y))

                                        {
                                            // and compute node cost, heuristic and overall score
                                            candidate.G = Cost;
                                            candidate.H = CalculateHeuristic(node.X, node.Y, Target.Location.X, Target.Location.Y);
                                            candidate.F = candidate.G + candidate.H;
                                            //node.Parent = current;

                                            // and add it to the open list
                                            //openList.Add(candidate);
                                            openList.Insert(0,candidate);

                                            //Console.WriteLine($"({node.X},{node.Y}), F ={candidate.F}");
                                        }
                                    }

                                    Console.WriteLine();
                                    Console.WriteLine("OpenList:");
                                    foreach (var item in openList)
                                    {
                                        Console.WriteLine($"({item.X},{item.Y}), F ={item.F}"); 
                                    }

                                    Frontier.Clear();
                                }
                             //Console.WriteLine("----------------------------");
                             Console.WriteLine();
                            }// CLOSED WHILE of OpenList
                            
                         Console.WriteLine();
                         Goals.Remove(g);// we remove the goal from the list of options for next box!
                        } // CLOSED IF of Turn ==2

                        else {Console.WriteLine("error, there should be only two turns for each agent-box combination");}
                      Console.WriteLine("----------------------------"); 
                    } // CLOSED WHILE Turn < 3
                }    //return fullpathagent for combo agent -box-goal; WHY CANNOT RETURN?

                //Console.WriteLine("----------------------------");
                Console.WriteLine($"full path agent {a.Symbol} lenght {fullpathagent.Count}: ");
                foreach (var step in fullpathagent) {Console.WriteLine($"Node Location: ({step.X},{step.Y}), F:{step.F}");}
                Console.WriteLine("----------------------------");
            }
        }     
    }
}
