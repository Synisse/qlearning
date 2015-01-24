using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MachineLearningQLearning.Logic;

namespace NeuralNetwork.Control
{
    //testgrid
    //public delegate Color GridUpdateHandler(int x, int y, Color currentColor);
    public delegate GardenUnitType GridUpdateHandler(int x, int y, GardenUnitType gType);

    public delegate void ActionRemovedHandler(int x, int y);

    public delegate void ActionAddedHandler(int x, int y);

    /// <summary>
    /// Interaktionslogik für CoordinateGrid.xaml
    /// </summary>
    public partial class CoordinateGrid
    {
        /// <summary>
        /// Border = GridArray[x,y]
        /// </summary>
        private Border[,] GridArray { get; set; }

        public bool isRunning = false;

        public CoordinateGrid()
        {
            InitializeComponent();

            Loaded += delegate { SetupGrid(); };
        }

        #region Setup

        private void SetupGrid()
        {
            YCounter = (int)Height / (ItemSize - (IsBorderEnabled ? 1 : 0));
            XCounter = (int)Width / (ItemSize - (IsBorderEnabled ? 1 : 0));

            GridArray = new Border[XCounter, YCounter];

            for (var x = 0; x < XCounter; x++)
            {
                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

                for (var y = 0; y < YCounter; y++)
                {
                    var border = CreateTileBorder(x, y);

                    GridArray[x, y] = border;
                    stackPanel.Children.Add(border);
                }

                YStack.Children.Add(stackPanel);
            }

            //test if null
            Tile beeTile = GridArray[CurrentBeeX, CurrentBeeY].Tag as Tile;
            beeTile.GardenType = GardenUnitType.BEE;
            GridArray[CurrentBeeX, CurrentBeeY].Background = TileManager.CreateImageBrush(GardenUnitType.BEE);
            OldBeeX = CurrentBeeX;
            OldBeeY = CurrentBeeY;
            //test if null

            CurrentFlowerX = XCounter;
            CurrentFlowerY = YCounter;

            Tile flowerTile = GridArray[CurrentFlowerX-1, CurrentFlowerY-1].Tag as Tile;
            flowerTile.GardenType = GardenUnitType.FLOWER;
            GridArray[CurrentFlowerX - 1, CurrentFlowerY - 1].Background = TileManager.CreateImageBrush(GardenUnitType.FLOWER);
        }

        public void ResetGrid()
        {
            ItemSet.Clear();

            for (var y = YCounter - 1; y > -1; y--)
            {
                for (var x = 0; x < XCounter; x++)
                {
                    var border = GridArray[x, y];

                    if (border != null)
                    {
                        var tag = border.Tag as Tile;
                        tag.GardenType = GardenUnitType.GRASS;

                        border.Background = TileManager.CreateImageBrush(tag.GardenType);
                        border.Tag = tag;
                        GridArray[tag.X, tag.Y] = border;

                        SetBorderChild(border, tag);
                    }
                }
            }
        }

        private Border CreateTileBorder(int x, int y)
        {
            var borderValue = IsBorderEnabled ? 1 : 0;

            var border = new Border
            {
                Tag = new Tile(x,y),
                Height = ItemSize,
                Width = ItemSize,
                BorderBrush = new SolidColorBrush(ItemBorderColor),
                BorderThickness = new Thickness(borderValue, borderValue, borderValue, borderValue),
                Margin = new Thickness(-1, -1, 0, 0),
                Background = TileManager.CreateImageBrush(GardenUnitType.GRASS),
                Child = new Grid { Visibility = Visibility.Hidden, Background = new SolidColorBrush(Colors.Black), Width = 2, Height = 2, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center }
            };

            border.MouseLeftButtonDown += SetCurrentColorValue;
            border.MouseRightButtonDown += RemoveColorValue;
            border.MouseEnter += DrawCurrentColorValue;

            return border;
        }

        #endregion

        #region Events

        public event ActionAddedHandler ActionAdded;

        protected virtual void OnActionAdded(int x, int y)
        {
            ActionAddedHandler handler = ActionAdded;
            if (handler != null) handler(x, y);
        }

        private void RemoveColorValue(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var border = sender as Border;
            Tile tag = null;

            if (border != null)
            {
                tag = border.Tag as Tile;
                tag.GardenType = GardenUnitType.GRASS;

                border.Background = TileManager.CreateImageBrush(GardenUnitType.GRASS);
                border.Tag = tag;
                GridArray[tag.X, tag.Y] = border;

                SetBorderChild(border, tag);
            }

            if (isRunning)
            {
                OnActionAdded(tag.X,tag.Y);
            }
        }


        private void DeleteOldBee(int newBeeX,int newBeeY)
        {
            GridArray[OldBeeX, OldBeeY].Background = TileManager.CreateImageBrush(GardenUnitType.GRASS);
            OldBeeX = newBeeX;
            OldBeeY = newBeeY;
        }

        public int OldBeeX { get; set; }
        public int OldBeeY { get; set; }

        public void SetBee(int state)
        {
            int x = state%XCounter;
            int y = state/XCounter;
            GridArray[x, y].Background = TileManager.CreateImageBrush(GardenUnitType.BEE);
            DeleteOldBee(x,y);

            GridArray[XCounter-1, YCounter -1].Background = TileManager.CreateImageBrush(GardenUnitType.FLOWER);
        }

        private void DrawCurrentColorValue(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.LeftButton == MouseButtonState.Pressed)
            {
                var border = sender as Border;

                if (border != null)
                {
                    border.Background = TileManager.CreateImageBrush(CurrentTile);

                    var tag = border.Tag as Tile;
                    tag.GardenType = CurrentTile;

                    SetBorderChild(border, tag);
                }
            }
        }

        public event ActionRemovedHandler ActionRemoved;

        protected virtual void OnActionRemoved(int x, int y)
        {
            ActionRemovedHandler handler = ActionRemoved;
            if (handler != null) handler(x,y);
        }

        private void SetCurrentColorValue(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var border = sender as Border;
            Tile tag = null;
            if (border != null)
            {
                border.Background = TileManager.CreateImageBrush(CurrentTile);

                tag = border.Tag as Tile;
                tag.GardenType = CurrentTile;
                SetBorderChild(border, tag);
            }

            if (isRunning)
            {
                OnActionRemoved(tag.X,tag.Y);
            }
        }

        private void SetBorderChild(Border border, Tile tag)
        {
            var grid = border.Child;
        }

        #endregion Methodes

        //zum updaten uebriger felder im neuronal network . evtl verwerfen
        public void GridRunner(GridUpdateHandler handler)
        {
            for (var x = 0; x < XCounter; x++)
            {
                for (var y = 0; y < YCounter; y++)
                {
                    var tag = GridArray[x, y].Tag as Tile; 

                    if (tag != null)
                    {
                        tag.GardenType = handler(x, y, tag.GardenType);

                        GridArray[x, y].Background = TileManager.CreateImageBrush(tag.GardenType);

                    }
                }
            }
        }

        public List<Tile> getMatrixTiles()
        { 
            List<Tile> tiles = new List<Tile>();
            for (var y = 0; y< YCounter; y++)
            {
                for (var x = 0; x< XCounter; x++)
                {
                    tiles.Add(GridArray[x, y].Tag as Tile);

                    //var tag = GridArray[x, y].Tag as Tile;

                    //if (tag != null)
                    //{
                    //    tag.GardenType = handler(x, y, tag.GardenType);

                    //    GridArray[x, y].Background = TileManager.CreateImageBrush(tag.GardenType);

                    //}
                }
            }

            return tiles;
        }

        public int getFlowerInt()
        {
            return (CurrentFlowerX-1)*(YCounter) + (CurrentFlowerY-1);
            //return int currentTilePos = (i * yDim + j);
        }

        #region Getter / Setter

        public int XCounter { get; set; }
        public int YCounter { get; set; }

        #endregion

        #region Dependency Propertys

        public static readonly DependencyProperty CurrentTileProperty =
        DependencyProperty.Register("CurrentTile", typeof(GardenUnitType), typeof(CoordinateGrid), new PropertyMetadata(GardenUnitType.WALL));

        public GardenUnitType CurrentTile
        {
            get { return (GardenUnitType)GetValue(CurrentTileProperty); }
            set { SetValue(CurrentTileProperty, value); }
        }

        public static readonly DependencyProperty CurrentFlowerXProperty =
            DependencyProperty.Register("CurrentFlowerX", typeof (int), typeof (CoordinateGrid), new PropertyMetadata(default(int)));

        public int CurrentFlowerX
        {
            get { return (int) GetValue(CurrentFlowerXProperty); }
            set { SetValue(CurrentFlowerXProperty, value); }
        }

        public static readonly DependencyProperty CurrentFlowerYProperty =
            DependencyProperty.Register("CurrentFlowerY", typeof (int), typeof (CoordinateGrid), new PropertyMetadata(default(int)));

        public int CurrentFlowerY
        {
            get { return (int) GetValue(CurrentFlowerYProperty); }
            set { SetValue(CurrentFlowerYProperty, value); }
        }

        public static readonly DependencyProperty CurrentBeeXProperty =
            DependencyProperty.Register("CurrentBeeX", typeof (int), typeof (CoordinateGrid), new PropertyMetadata(0));

        public int CurrentBeeX
        {
            get { return (int) GetValue(CurrentBeeXProperty); }
            set { SetValue(CurrentBeeXProperty, value); }
        }

        public static readonly DependencyProperty currentBeeYProperty =
            DependencyProperty.Register("currentBeeY", typeof (int), typeof (CoordinateGrid), new PropertyMetadata(0));

        public int CurrentBeeY
        {
            get { return (int) GetValue(currentBeeYProperty); }
            set { SetValue(currentBeeYProperty, value); }
        }

        public static readonly DependencyProperty ItemBoaderColorProperty =
            DependencyProperty.Register("ItemBorderColor", typeof(Color), typeof(CoordinateGrid), new PropertyMetadata(Colors.Black));

        public Color ItemBorderColor
        {
            get { return (Color)GetValue(ItemBoaderColorProperty); }
            set { SetValue(ItemBoaderColorProperty, value); }
        }

        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(int), typeof(CoordinateGrid), new PropertyMetadata(10));

        public int ItemSize
        {
            get { return (int)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        public static readonly DependencyProperty IsBorderEnabledProperty =
            DependencyProperty.Register("IsBorderEnabled", typeof(bool), typeof(CoordinateGrid), new PropertyMetadata(true));

        public bool IsBorderEnabled
        {
            get { return (bool)GetValue(IsBorderEnabledProperty); }
            set { SetValue(IsBorderEnabledProperty, value); }
        }

        public static readonly DependencyProperty ItemSetProperty =
            DependencyProperty.Register("ItemSet", typeof(List<CoordinateTag>), typeof(CoordinateGrid), new PropertyMetadata(new List<CoordinateTag>()));

        public List<CoordinateTag> ItemSet
        {
            get { return (List<CoordinateTag>)GetValue(ItemSetProperty); }
            private set { SetValue(ItemSetProperty, value); }
        }

        #endregion
    }
}
