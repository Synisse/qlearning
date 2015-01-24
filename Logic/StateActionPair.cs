using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningQLearning.Logic
{
    public class StateActionPair
    {
        public string StateOne { get; set; }
        public string Action { get; set; }
        public double Reward { get; set; }

        public StateActionPair(string state, string action, double reward)
        {
            StateOne = state;
            Action = action;
            Reward = reward;
        }
    }
}
