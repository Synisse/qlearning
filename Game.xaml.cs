using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MachineLearningQLearning.Logic;

namespace MachineLearningQLearning
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        private Grid gamePane = new Grid();
        public ImageBrush CurrentBrush { get; set; }
        public GardenUnitType CurrentType { get; set; }
        private List<Tile> tiles = new List<Tile>();
        private BackgroundWorker worker;
        private StateActionTableManager manager;


        public Game()
        {
            InitializeComponent();
            //testFrame.Content = GenerateGrid(3, 3, 32, 32);
            CurrentBrush = CreateImageBrush(GardenUnitType.WALL);
            CurrentType = GardenUnitType.WALL;
            worker = new BackgroundWorker();
            initBGWorker();
        }

        private void initBGWorker()
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.WorkerSupportsCancellation = true;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            while (!worker.CancellationPending)
            {

                manager.PerformTrainStep(Convert.ToInt32(e.Argument));

            }
        }

        private void UpdateGrid()
        {
            string cur = manager.CurrentState;
            if (!cur.Equals(""))
            {
                //testFrame.InvalidateVisual();
                int test = Convert.ToInt32(cur);
                tiles[test].GardenType = GardenUnitType.BEE;
                tiles[test].Background =
                    CreateImageBrush(GardenUnitType.BEE);
            }
            //testFrame.Parent.GetType().
        }

        private Grid GenerateGrid(int rows, int columns, int cellWidth, int cellHeight)
        {
            var grid = new Grid();
            grid.Width = columns * cellWidth;
            grid.Height = rows * cellHeight;

            for (int i = 0; i < rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var cell = new Tile(i, j);
                    cell.MouseDown += cell_MouseDown;
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    cell.Background = CreateImageBrush(GardenUnitType.GRASS);

                    cell.BorderBrush = Brushes.Gray;
                    cell.BorderThickness = new Thickness(0.1);
                    grid.Children.Add(cell);
                    tiles.Add(cell);
                }
            }

            gamePane = grid;

            return grid;
        }

        //Clickmethode aus maingui
        public void CreateTable()
        {
            TileManager.BuildStateRewardTable(tiles, 3);
            manager = new StateActionTableManager();
            manager.RewardTable = TileManager.RewardTable;
            manager.CreateStateActionMatrix();
            manager.InitQMatrix();
            manager.GoalState = TileManager.GoalState;
            //qList = StateRewardTableManager.ConvertQMatrixToList();
            //bool isRunning = false;
            worker.RunWorkerAsync(10);
            //manager.PerformTrainStep(10);
            
            //if (!isRunning)
            //{
            //    worker.RunWorkerAsync();
            //    isRunning = true;
            //}

        }


        private ImageBrush CreateImageBrush(GardenUnitType gardenType)
        {
            var gType = gardenType;
            ImageBrush br = new ImageBrush();
            switch (gType)
            {
                case GardenUnitType.GRASS:
                    br.ImageSource = new BitmapImage(new Uri(@"Images\grass.png", UriKind.Relative));
                    break;
                case GardenUnitType.FLOWER:
                    br.ImageSource = new BitmapImage(new Uri(@"Images\flower.png", UriKind.Relative));
                    break;
                case GardenUnitType.BEE:
                    br.ImageSource = new BitmapImage(new Uri(@"Images\bee.png", UriKind.Relative));
                    break;
                case GardenUnitType.WALL:
                    br.ImageSource = new BitmapImage(new Uri(@"Images\monster.png", UriKind.Relative));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return br;
        }

        public void ChangeCurrentBrush(GardenUnitType gType)
        {
            CurrentBrush = CreateImageBrush(gType);
            CurrentType = gType;
        }

        void cell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (tiles.Contains((Tile)sender))
                {
                    tiles[tiles.IndexOf((Tile)sender)].Background = CurrentBrush;
                    tiles[tiles.IndexOf((Tile)sender)].GardenType = CurrentType;
                }

                //tile.Background = CreateImageBrush(GardenUnitType.WALL);
            }
        }
    }
}
