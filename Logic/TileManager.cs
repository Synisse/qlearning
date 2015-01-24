using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MachineLearningQLearning.Logic
{
    public class TileManager
    {
        public static StateActionTable RewardTable;
        public static string GoalState = "";

        public static void BuildStateRewardTable(List<Tile> tiles, int dimension)
        {
            int xDim = dimension;
            int yDim = dimension;

            RewardTable = new StateActionTable();

            for (int i = 0; i < xDim; i++)
            {
                for (int j = 0; j < yDim; j++)
                {
                    int currentTilePos = (i * yDim + j);
                    Tile tile = (Tile)tiles[currentTilePos];
                    if (tile.GardenType != GardenUnitType.WALL)
                    {
                        foreach (var neighbor in GetNeighbors(currentTilePos, dimension))
                        {
                            if ((neighbor >= 0) && (neighbor < (dimension * dimension)))
                            {
                                Tile neighborTile = (Tile)tiles[neighbor];
                                if (neighborTile.GardenType == GardenUnitType.GRASS)
                                {
                                    RewardTable.Add(new StateActionPair(currentTilePos.ToString(), neighbor.ToString(),
                                                                        0));
                                }
                                else if (neighborTile.GardenType == GardenUnitType.FLOWER)
                                {
                                    GoalState = neighbor.ToString();
                                    RewardTable.Add(new StateActionPair(currentTilePos.ToString(), neighbor.ToString(),
                                                                        100));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static List<int> GetNeighbors(int currentState, int dimension)
        {
            List<int> neighborList = new List<int>();

            //top
            if ((currentState % dimension) != 0)
            {
                neighborList.Add(currentState - 1);
            }

            //left
            neighborList.Add(currentState - dimension);

            //right
            neighborList.Add(currentState + dimension);

            //bottom
            if ((currentState%dimension) != (dimension-1))
            {
                neighborList.Add(currentState + 1);
            }

            return neighborList;
        }

        public static ImageBrush CreateImageBrush(GardenUnitType gardenType)
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
    }
}
