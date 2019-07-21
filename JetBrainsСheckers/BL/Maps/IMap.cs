using System.Collections.Generic;
using JetBrainsСheckers.BL.Maps.Checkers;
using JetBrainsСheckers.BL.Maps.Steps;

namespace JetBrainsСheckers.BL.Maps
{
	public interface IMap : IEnumerable<CheckerPosition>
	{
		bool ContainsKey(Point key);
		bool TryGetValue(Point key, out CheckerType value);
		CheckerType GetValue(Point key);
		void Next(StepResult data);
	}
}