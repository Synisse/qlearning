using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningQLearning.Logic.Singletons
{
    public class GammaSingleton
    {
        public double Gamma { get; set; }

        private static GammaSingleton _instance;

        private GammaSingleton()
        {
            //defaultvalue
            Gamma = 0.8;
        }

        public static GammaSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GammaSingleton();
                }
                return _instance;
            }
        }
    }
}
