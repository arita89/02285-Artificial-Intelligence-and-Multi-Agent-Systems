using System.Collections.Generic;

namespace DeepMinds.Strategies
{
    abstract class Strategy
    {
        protected readonly LevelAnalyzed level;

        public Strategy(LevelAnalyzed level)
        {
            this.level = level;
        }

        public abstract List<List<Actions.Action>> Solve();
    }
}
