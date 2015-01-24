using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningQLearning.Logic
{
    public class TimeOutSingleton
    {
        public int TimeOutDuration { get; set; }

        private static TimeOutSingleton _instance;

        private TimeOutSingleton()
        {
            //defaultvalue
            TimeOutDuration = 100;
        }

        public static TimeOutSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TimeOutSingleton();
                }
                return _instance;
            }
        }



    }
}
