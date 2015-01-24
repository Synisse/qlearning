using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using MachineLearningQLearning.Annotations;

namespace MachineLearningQLearning.Logic
{
    public class Tile : Border, INotifyPropertyChanged
    {
        public int X { get; set; }
        public int Y { get; set; }
        public GardenUnitType GardenType { get; set; }

        public Tile(int x, int y, GardenUnitType gardenType)
        {
            X = x;
            Y = y;
            GardenType = gardenType;
        }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            GardenType = GardenUnitType.GRASS;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
