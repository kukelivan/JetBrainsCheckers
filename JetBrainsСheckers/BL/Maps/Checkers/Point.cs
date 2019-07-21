namespace JetBrainsСheckers.BL.Maps.Checkers
{
	public class Point
	{
		public Point(sbyte x, sbyte y)
		{
			X = x;
			Y = y;
		}

		public sbyte X { get; }
		public sbyte Y { get; }

		public static Point operator +(Point p1, Point p2)
		{
			return new Point((sbyte) (p1.X + p2.X), (sbyte) (p1.Y + p2.Y));
		}

		public static Point operator -(Point p1, Point p2)
		{
			return new Point((sbyte) (p1.X - p2.X), (sbyte) (p1.Y - p2.Y));
		}

		private bool Equals(Point other)
		{
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Point point && Equals(point);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = (int) X;
				result = (result * 397) ^ Y;
				return result;
			}
		}

		public static bool operator ==(Point a, Point b)
		{
			if (a is null || b is null)
				return a is null && b is null;
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Point a, Point b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			return $"X:{X} Y:{Y}";
		}
	}
}