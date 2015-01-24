using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningQLearning.Logic
{
    public class LearningRateSingleton
    {

        public double LearningRate { get; set; }

        private static LearningRateSingleton _instance;

        private LearningRateSingleton()
        {
            //defaultvalue
            LearningRate = 0.5;
        }

        public static LearningRateSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LearningRateSingleton();
                }
                return _instance;
            }
        }
    }
}
