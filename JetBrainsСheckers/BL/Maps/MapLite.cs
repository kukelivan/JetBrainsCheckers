using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrainsСheckers.BL.Maps.Checkers;
using JetBrainsСheckers.BL.Maps.Steps;

namespace JetBrainsСheckers.BL.Maps
{
	public class MapLite : IMap
	{
		protected Dictionary<Point, CheckerType> Board;

		public bool TryGetValue(Point key, out CheckerType value)
		{
			return Board.TryGetValue(key, out value);
		}

		public bool ContainsKey(Point key)
		{
			return Board.ContainsKey(key);
		}

		public CheckerType GetValue(Point key)
		{
			return Board[key];
		}

		public void Next(StepResult data)
		{
			Board.Remove(data.OldPosition);
			data.KilledCheckers?.ToList().ForEach(ch => Board.Remove(ch.Position));
			Board.Add(data.NewPosition, data.NewChecker);
		}

		public IEnumerator<CheckerPosition> GetEnumerator()
		{
			return Board.Select(pair => new CheckerPosition {Checker = pair.Value, Position = pair.Key})
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public MapLite Clone()
		{
			return new MapLite {Board = Board.ToDictionary(k => k.Key, v => v.Value)};
		}
	}
}