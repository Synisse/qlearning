﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningQLearning.View
{
    public class ViewModelProvider
    {
        public static MainViewModel MainViewModel
        {
            get { return new MainViewModel(); }
        }
    }
}
