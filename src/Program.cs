using System;
using System.IO;
using System.Linq;
using DeepMinds.Strategies;

namespace DeepMinds
{
    class Program
    {
        /// <summary>
        /// Command for executing with the server (for working dir: (...)/deep-minds:
        /// java -jar server.jar -c "dotnet run ../levels/<level_name>"
        /// </summary>
        /// <param name="args">1.Level 2.TODO: strategy</param>
        static void Main(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args.First()))
            {
                Console.WriteLine("#Arguments: <file> - a path to a file containing a level");
                return;
            }

            try
            {
                // Choose client name
                Console.WriteLine("DeepMinds");

                var path = args[0];
                var levelAnalyzed = ParseLevel(path);

                // TODO: Pick the strategy based on levelAnalyzed and other info
                // Strategy strategy = new ExampleStrategy(levelAnalyzed);
                // Strategy strategy = new AStarSAStrategy(levelAnalyzed, new GreedyMinmatchingHeuristic());
                Strategy strategy = new PrioritizeSAStrategy(levelAnalyzed);

                var solution = new Solution(strategy.Solve(), levelAnalyzed.IsMultiAgent);
                solution.PrintSolution();
            }
            catch (Exception ex)
            {
                // Write it as a comment so the server produces this output
                Console.WriteLine($"#{ex}");
            }
        }

        static LevelAnalyzed ParseLevel(string path)
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var parser = new Parser(fileStream);
            var level = parser.Parse();
            var levelAnalyzed = LevelAnalyzer.Analyze(level);
            return levelAnalyzed;
        }
    }
}
