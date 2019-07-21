using System.Collections.Generic;
using JetBrainsСheckers.BL.Maps.Checkers;

namespace JetBrainsСheckers.BL.Maps.Steps
{
	public class StepResult
	{
		public CheckerType OldChecker { get; set; }
		public Point OldPosition { get; set; }
		public Point NewPosition { get; set; }
		public CheckerType NewChecker { get; set; }
		public List<CheckerPosition> KilledCheckers { get; set; }
	}
}