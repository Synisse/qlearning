using System.Drawing;
using Color = System.Windows.Media.Color;

namespace NeuralNetwork.Control
{
    public class CoordinateTag
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsValidationItem { get; set; }
        public Color Color { get; set; }

        protected bool Equals(CoordinateTag other)
        {
            return X == other.X && Y == other.Y && Color.Equals(other.Color);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((CoordinateTag)obj);
        }
    }
}
