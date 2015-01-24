using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningQLearning.Logic.Singletons
{
    public class ExplorationRateSingleton
    {
        public double ExplorationRate { get; set; }

        private static ExplorationRateSingleton _instance;

        private ExplorationRateSingleton()
        {
            //defaultvalue
            ExplorationRate = 1;
        }

        public static ExplorationRateSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExplorationRateSingleton();
                }
                return _instance;
            }
        }
    }
}
