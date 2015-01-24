using System;
using System.Collections.Generic;
using System.Windows;
using MachineLearningQLearning.Logic;

namespace MachineLearningQLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StateActionTable _stateRewardTable;
        private List<List<string>> qList;
        private StateActionTableManager manager;

        public MainWindow()
        {
            InitializeComponent();
            
            manager = new StateActionTableManager();
            InitTestTable();

        }

        public StateActionTable StateRewardTable
        {
            get { return _stateRewardTable; }
            set { _stateRewardTable = value; }
        }

        private void InitTestTable()
        {
            StateGrid.ItemsSource = manager.CreateRoomExampleTable();
            manager.CreateStateActionMatrix();
            manager.InitQMatrix();
            qList = manager.ConvertQMatrixToList();
            lst.ItemsSource = qList;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            qList = manager.PerformTrainStep();
            //List<string> test = StateRewardTableManager.GetBestRouteOfState("C");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int cycles = Convert.ToInt32(cycleTB.Text);
            for (int i = 0; i < cycles; i++)
            {
                qList = manager.PerformTrainStep();
                lst.ItemsSource = qList;
                currentCycleLabel.Content = (i+1).ToString();
            }
        }

        private void initB_Click(object sender, RoutedEventArgs e)
        {
            manager.GoalState = goalTB.Text;
            manager.DiscountRate = Convert.ToDouble(gammaTB.Text) / 10;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string bestRoute =StartStateTB.Text;
            foreach (var state in manager.GetBestRouteOfState(StartStateTB.Text))
            {
                bestRoute += "-" + state;
            }
            RouteLabel.Content = bestRoute;
        }

    }
}
