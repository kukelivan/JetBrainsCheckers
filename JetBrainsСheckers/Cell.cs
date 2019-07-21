using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrainsСheckers.BL;
using JetBrainsСheckers.BL.Maps.Checkers;
using Point = JetBrainsСheckers.BL.Maps.Checkers.Point;

namespace JetBrainsСheckers
{
	public class Cell : Button
	{
		public delegate void MethodContainer(Point position, CheckerType? checker);

		public Cell(Point position)
		{
			Position = position;
			base.Click += Cell_Click1;
		}

		private Point Position { get; }

		private CheckerType? Checker { get; set; }

		private void Cell_Click1(object sender, RoutedEventArgs e)
		{
			Click?.Invoke(Position, Checker);
		}

		public new event MethodContainer Click;

		public void SetChecker(CheckerType? checker)
		{
			Checker = checker;
			if (checker != null)
				Content = new Image
				{
					Stretch = Stretch.Fill,
					Source = checker.Value.IsBlack()
						? checker.Value.IsKing()
							? Config.BlackKingImage
							: Config.BlackImage
						: checker.Value.IsKing()
							? Config.WhiteKingImage
							: Config.WhiteImage
				};
			else
				Content = null;
		}
	}
}