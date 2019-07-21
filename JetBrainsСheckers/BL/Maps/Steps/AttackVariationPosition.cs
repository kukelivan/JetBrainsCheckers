using System.Collections.Generic;
using JetBrainsСheckers.BL.Maps.Checkers;

namespace JetBrainsСheckers.BL.Maps.Steps
{
	public class AttackVariationPosition
	{
		public Point NewStep { get; set; }
		public List<Point> Opponents { get; set; }
		public bool IsKing { get; set; }
	}
}