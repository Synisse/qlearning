using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MachineLearningQLearning.Logic;
using MachineLearningQLearning.Logic.Singletons;
using MachineLearningQLearning.View;

namespace MachineLearningQLearning
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public ImageBrush CurrentBrush { get; set; }
        public MainViewModel MainViewModel { get; set; }

        public GameWindow()
        {
            InitializeComponent();

            MainViewModel = DataContext as MainViewModel;

            if (MainViewModel != null)
            {
                MainViewModel.CoordinateGrid = MainCoordinateGrid;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainViewModel.CurrentTile = GardenUnitType.BEE;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainViewModel.CurrentTile = GardenUnitType.FLOWER;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainViewModel.CurrentTile = GardenUnitType.WALL;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MainViewModel.CurrentTile = GardenUnitType.GRASS;
        }

        private void trainstepClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.InitQLearning();
            MainViewModel.InitEvents();
            MainViewModel.DoSteps(1);
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            TimeOutSingleton.Instance.TimeOutDuration = ((int)slider.Value*100)-75;
        }

        private void PerformLoopsClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.InitQLearning();
            MainViewModel.InitEvents();
            MainViewModel.DoSteps(Convert.ToInt32(cycleTB.Text));
        }

        private void learningRateChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            LearningRateSingleton.Instance.LearningRate = slider.Value / 100;
        }

        private void explorationRateChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            ExplorationRateSingleton.Instance.ExplorationRate = slider.Value / 100;
        }

        private void gammaChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            GammaSingleton.Instance.Gamma = slider.Value / 100;
        }
    }
}
