using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MachineLearningQLearning.Logic;
using NeuralNetwork.Control;

namespace MachineLearningQLearning.View
{
    internal delegate void UpdateGridHandler();

    public class MainViewModel :INotifyPropertyChanged
    {
        private StateActionTableManager _manager;

        public MainViewModel()
        {
                
        }

        public void InitEvents()
        {
            CoordinateGrid.ActionRemoved += CoordinateGridOnActionRemoved;
            CoordinateGrid.ActionAdded +=CoordinateGridOnActionAdded;
            CoordinateGrid.isRunning = true;
        }

        private void CoordinateGridOnActionAdded(int x, int y)
        {
            RemoveWallDuringRuntime(x,y);
        }

        private void CoordinateGridOnActionRemoved(int x, int y)
        {
            PaintWallDuringRuntime(x,y);
        }


        public void InitQLearning()
        {
            
            TileManager.BuildStateRewardTable(CoordinateGrid.getMatrixTiles(), CoordinateGrid.XCounter);
            _manager = new StateActionTableManager();
            _manager.RewardTable = TileManager.RewardTable;
            _manager.CreateStateActionMatrix();
            _manager.InitQMatrix();
            _manager.GoalState = CoordinateGrid.getFlowerInt().ToString();
            
        }

        private void RemoveWallDuringRuntime(int x, int y)
        {
            int actionValue = y * CoordinateGrid.XCounter + x;
            _manager.AddActionPairs(actionValue.ToString(), CoordinateGrid.getMatrixTiles(), CoordinateGrid.XCounter);
        }

        private void PaintWallDuringRuntime(int x, int y)
        {
            RemoveActionOption(x,y);
        }

        private  void RemoveActionOption(int x, int y)
        {
            int actionValue = y * CoordinateGrid.XCounter + x;
            _manager.RemoveActionPairs(actionValue.ToString());
        }

        public void DoStep()
        {
            _manager.PerformFancyTrainStep(delegate(int state) { CurrentBeeX = state;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
            {
                CoordinateGrid.SetBee(CurrentBeeX);
            }));
            },"0");
        }

        public CoordinateGrid CoordinateGrid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private GardenUnitType _currentTile = GardenUnitType.WALL;

        public GardenUnitType CurrentTile
        {
            get { return _currentTile; }
            set 
            { 
                _currentTile = value;
                OnPropertyChanged();
            }
        }

        private int _currentBeeX = 0;

        public int CurrentBeeX
        {
            get { return _currentBeeX; }
            set 
            { 
                _currentBeeX = value;
                OnPropertyChanged();
            }
        }


        private int _currentBeeY = 0;

        public int CurrentBeeY
        {
            get { return _currentBeeY; }
            set
            {
                _currentBeeY = value;
                OnPropertyChanged();
            }
        }

        public void DoSteps(int i)
        {
            _manager.PerformFancyTrainSteps(delegate(int state)
            {
                CurrentBeeX = state;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
                {
                    CoordinateGrid.SetBee(CurrentBeeX);
                }));
            }, "0",i);
        }

        public void DoBestSteps(int i)
        {
            _manager.PerformBestRouteSteps(delegate(int state)
            {
                CurrentBeeX = state;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
                {
                    CoordinateGrid.SetBee(CurrentBeeX);
                }));
            }, "0",i);
        }
    }
}
