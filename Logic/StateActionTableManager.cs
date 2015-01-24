using System;
using System.Collections.Generic;
using System.Threading;
using MachineLearningQLearning.Logic.Singletons;

namespace MachineLearningQLearning.Logic
{
    public delegate void CurrentStateChangedEventHandler(int state);

    public delegate void CycleFinishedEventHandler();

    public class StateActionTableManager
    {
        private double[,] _qMatrix;
        private double[,] _stateActionMatrix;
        private Boolean _qMatrixInitialised;
        private double _discountRate = 0.8;
        private List<string> _elementList;
        private string _goalState = "F";
        public string CurrentState = "";
        public string InitState = "";
        private bool _isInitialized;

        public string GoalState
        {
            get { return _goalState; }
            set { _goalState = value; }
        }

        public double DiscountRate
        {
            get { return _discountRate; }
            set { _discountRate = value; }
        }

        public StateActionTable RewardTable { get; set; }

        public StateActionTable CreateRoomExampleTable()
        {
            return RewardTable = new StateActionTable
                {
                    new StateActionPair("A", "E", 0),
                    new StateActionPair("A", "G", 0),
                    new StateActionPair("G", "A", 0),
                    new StateActionPair("E", "A", 0),
                    new StateActionPair("E", "F", 100),
                    new StateActionPair("F", "E", 0),
                    new StateActionPair("E", "D", 0),
                    new StateActionPair("D", "E", 0),
                    new StateActionPair("D", "C", 0),
                    new StateActionPair("C", "D", 0),
                    new StateActionPair("D", "B", 0),
                    new StateActionPair("B", "D", 0),
                    new StateActionPair("B", "F", 100),
                    new StateActionPair("F", "B", 0),
                    new StateActionPair("F", "F", 100)
                };
        }

        public double[,] CreateStateActionMatrix()
        {
            var elementList = EstimateElementCount();
            _stateActionMatrix = new double[elementList.Count, elementList.Count];
            for (int i = 0; i < elementList.Count; i++)
            {
                for (int j = 0; j < elementList.Count; j++)
                {
                    foreach (var stateRewardPair in RewardTable)
                    {
                        if ((stateRewardPair.StateOne.Equals(elementList[i])) &&
                            (stateRewardPair.Action.Equals(elementList[j])))
                        {
                            if (stateRewardPair.Reward >= 0)
                            {
                                _stateActionMatrix[i, j] = stateRewardPair.Reward;
                            }
                            break;
                        }
                        _stateActionMatrix[i, j] = -1;
                    }
                }
            }
            return _stateActionMatrix;
        }

        public List<string> EstimateElementCount()
        {
            if (!_isInitialized)
            {
                _elementList = new List<string>();

                foreach (var stateRewardPair in RewardTable)
                {
                    if (!_elementList.Contains(stateRewardPair.StateOne))
                        _elementList.Add(stateRewardPair.StateOne);
                }
                //_elementList.Sort();
                _isInitialized = true;
            }
            return _elementList;
        }

        public void InitQMatrix()
        {
            _qMatrix = new double[_elementList.Count, _elementList.Count];

            for (var i = 0; i < _elementList.Count; i++)
            {
                for (var j = 0; j < _elementList.Count; j++)
                {
                    _qMatrix[i, j] = 0;
                }
            }
        }

        public List<List<string>> PerformTrainStep(int cycles)
        {
            var lst = new List<List<string>>();
            for (int i = 0; i < cycles; i++)
            {
                lst = PerformTrainStep();
            }
            return lst;
        }

        public void AddActionPairs(string action, List<Tile> tiles, int dim)
        {
            _elementList.Add(action);
            TileManager.BuildStateRewardTable(tiles, dim);
            if (TileManager.RewardTable != null) RewardTable = TileManager.RewardTable;
            CreateStateActionMatrix();

            AddActionPairToQMatrix();
        }

        private void AddActionPairToQMatrix()
        {
            var newQMatrix = new double[_elementList.Count, _elementList.Count];

            for (var i = 0; i < _elementList.Count; i++)
            {
                for (var j = 0; j < _elementList.Count; j++)
                {
                    if ((i < _elementList.Count - 1))
                    {
                        if (j < _elementList.Count - 1)
                        {
                            newQMatrix[i, j] = _qMatrix[i, j];
                        }
                    }
                }
            }

            _qMatrix = newQMatrix;
        }

        public void RemoveActionPairs(string action)
        {
            var pairsToDelete = new List<StateActionPair>();
            foreach (var valuePair in RewardTable)
            {
                if (valuePair.Action.Equals(action))
                {
                    pairsToDelete.Add(valuePair);
                }
                if (valuePair.StateOne.Equals(action))
                {
                    pairsToDelete.Add(valuePair);
                }
            }

            foreach (var stateRewardPair in pairsToDelete)
            {
                RewardTable.Remove(stateRewardPair);
            }

            List<string> oldElementList = _elementList;
            _elementList.Remove(action);
            CreateStateActionMatrix();
            RemoveActionFromQMatrix(action, oldElementList);
        }

        private void RemoveActionFromQMatrix(string action, List<string> oldElementList)
        {
            //remove column
            var newQMatrix = new double[oldElementList.Count, oldElementList.Count];

            int elementColumnCounter = 0;
            int elementRowCounter = 0;
            int elementPosition = GetRowOfState(action);

            for (var i = 0; i < oldElementList.Count + 1; i++)
            {
                if ((i != elementPosition) && (i < oldElementList.Count))
                {
                    for (var j = 0; j < oldElementList.Count + 1; j++)
                    {

                        if ((j != elementPosition) && (j < oldElementList.Count))
                        {
                            newQMatrix[i, j] = _qMatrix[elementRowCounter, elementColumnCounter];
                            elementColumnCounter++;
                        }
                        else
                        {
                            //column ueberspringen
                            elementColumnCounter += 2;

                        }
                    }

                    elementRowCounter++;
                }
                else
                {
                    elementRowCounter += 2;
                }

                elementColumnCounter = 0;
            }

            _qMatrix = newQMatrix;
        }

        public List<List<string>> PerformTrainStep()
        {
            if (!_qMatrixInitialised)
            {
                InitQMatrix();
                _qMatrixInitialised = true;
            }
            var initialState = "";
            if (InitState.Equals(""))
            {
                initialState = _elementList[new Random().Next(0, _elementList.Count - 1)];
            }
            else
            {
                initialState = InitState;
            }

            CurrentState = initialState;

            while (!CurrentState.Equals(GoalState))
            {

                int rowOfCurrentState = GetRowOfState(CurrentState);
                int columnOfRandomAction = PickRandomActionForState(rowOfCurrentState);

                double newQValue = _qMatrix[rowOfCurrentState, columnOfRandomAction] +
                                   LearningRateSingleton.Instance.LearningRate*
                                   (_stateActionMatrix[rowOfCurrentState, columnOfRandomAction] +
                                    (GammaSingleton.Instance.Gamma*PickMaxActionForState(columnOfRandomAction)) -
                                    _qMatrix[rowOfCurrentState, columnOfRandomAction]);

                _qMatrix[rowOfCurrentState, columnOfRandomAction] = newQValue;

                CurrentState = GetStateOfRow(columnOfRandomAction);
            }

            return ConvertQMatrixToList();
        }

        public List<List<string>> PerformFancyTrainStep(CurrentStateChangedEventHandler handler, string initialState)
        {
            if (!_qMatrixInitialised)
            {
                InitQMatrix();
                _qMatrixInitialised = true;
            }
           
            CurrentState = initialState;
            while (!CurrentState.Equals(GoalState))
            {

                var rowOfCurrentState = GetRowOfState(CurrentState);

                int nextActionRow;

                var rnd = new Random();
                if (rnd.NextDouble() > ExplorationRateSingleton.Instance.ExplorationRate)
                {
                    nextActionRow = PickBestActionForState(rowOfCurrentState);
                }
                else
                {
                    nextActionRow = PickRandomActionForState(rowOfCurrentState);
                }

                double newQValue = _qMatrix[rowOfCurrentState, nextActionRow] +
                   LearningRateSingleton.Instance.LearningRate *
                   (_stateActionMatrix[rowOfCurrentState, nextActionRow] +
                    (GammaSingleton.Instance.Gamma * PickMaxActionForState(nextActionRow)) -
                    _qMatrix[rowOfCurrentState, nextActionRow]);

                _qMatrix[rowOfCurrentState, nextActionRow] = newQValue;

                CurrentState = GetStateOfRow(nextActionRow);
                if (handler != null)
                {
                    handler(Convert.ToInt32(CurrentState));
                }
                Thread.Sleep(TimeOutSingleton.Instance.TimeOutDuration);
            }
            return ConvertQMatrixToList();
        }

        public void PerformFancyTrainSteps(CurrentStateChangedEventHandler handler, string initStateTest, int cycles)
        {
            var thread = new Thread(() =>
                {
                    for (int i = 0; i < cycles; i++)
                    {
                        PerformFancyTrainStep(handler, initStateTest);
                    }
                });
            thread.Start();
        }


        public int PickRandomActionForState(int stateRow)
        {
            var actionOptions = new List<int>();

            for (var i = 0; i < _elementList.Count; i++)
            {
                if (_stateActionMatrix[stateRow, i] >= 0)
                {
                    actionOptions.Add(i);
                }
            }

            actionOptions.Sort();

            return actionOptions[new Random().Next(0, actionOptions.Count)];
        }

        /// <summary>
        /// Estimates the max reward for a given state.
        /// </summary>
        /// <param name="stateRow">Row of the current state</param>
        /// <returns>max reward</returns>
        public double PickMaxActionForState(int stateRow)
        {
            var actionOptions = new List<double>();

            for (var i = 0; i < _elementList.Count; i++)
            {
                if (_stateActionMatrix[stateRow, i] >= 0)
                {
                    actionOptions.Add(_qMatrix[stateRow, i]);
                }
            }

            actionOptions.Sort();

            return actionOptions[actionOptions.Count - 1];
        }

        public int GetRowOfState(string initialState)
        {
            int rowOfInitState = -1;
            for (int i = 0; i < _elementList.Count; i++)
            {
                if (_elementList[i].Equals(initialState))
                {
                    rowOfInitState = i;
                }
            }
            return rowOfInitState;
        }

        public string GetStateOfRow(int row)
        {
            return _elementList[row];
        }

        public List<List<string>> ConvertQMatrixToList()
        {
            var qList = new List<List<string>>();
            qList.Add(new List<string>());
            qList[0].Add("");
            for (int i = 0; i < _elementList.Count; i++)
            {
                qList[0].Add(_elementList[i]);
            }
            for (int i = 0; i < _elementList.Count; i++)
            {
                qList.Add(new List<string>());
                qList[i + 1].Add(_elementList[i]);
            }

            for (int i = 0; i < _elementList.Count; i++)
            {
                for (int j = 0; j < _elementList.Count; j++)
                {
                    qList[i + 1].Add(_qMatrix[i, j].ToString());
                }
            }

            return qList;
        }

        #region Use of trained data

        public List<string> GetBestRouteOfState(string state)
        {
            var states = new List<string>();
            var stateRowRepresentation = GetRowOfState(state);

            var curState = stateRowRepresentation;

            while (curState != GetRowOfState(GoalState))
            {
                var bestRewardAction = PickBestActionForState(curState);

                curState = bestRewardAction;


                states.Add(GetStateOfRow(bestRewardAction));
            }

            return states;
        }

        public List<string> GetBestFancyRouteOfState(CurrentStateChangedEventHandler handler, string state)
        {
            var states = new List<string>();
            var stateRowRepresentation = GetRowOfState(state);
            var curState = stateRowRepresentation;

            while (!GetRowOfState(curState.ToString()).Equals(GetRowOfState(GoalState)))
            {
                var bestRewardAction = PickBestActionForState(GetRowOfState(curState.ToString()));

                var testState = GetStateOfRow(bestRewardAction);

                curState = Convert.ToInt32(testState);
                if (handler != null)
                {
                    handler(curState);
                }
                Thread.Sleep(TimeOutSingleton.Instance.TimeOutDuration);
                states.Add(testState);
            }

            return states;
        }

        public void PerformBestRouteSteps(CurrentStateChangedEventHandler handler, string initStateTest, int cycles)
        {
            var thread = new Thread(() =>
            {
                for (int i = 0; i < cycles; i++)
                {
                    GetBestFancyRouteOfState(handler, initStateTest);
                }
            });
            thread.Start();
        }

        public int PickBestActionForState(int stateRow)
        {
            var bestReward = -1;
            var bestRewardRow = -1;

            for (var i = 0; i < _elementList.Count; i++)
            {
                if ((_stateActionMatrix[stateRow, i] >= 0) && (_qMatrix[stateRow, i] > bestReward))
                {
                    bestRewardRow = i;
                }
            }
            return bestRewardRow;
        }
        #endregion
    }
}
