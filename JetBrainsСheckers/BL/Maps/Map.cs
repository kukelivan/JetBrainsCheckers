using System.Collections.Generic;
using System.Linq;
using JetBrainsСheckers.BL.Maps.Checkers;
using JetBrainsСheckers.BL.Maps.Steps;

namespace JetBrainsСheckers.BL.Maps
{
	public class Map : MapLite
	{
		private readonly List<StepResult> _steps = new List<StepResult>();
		private bool _startUserIsBlack;
		private int _storyStep;


		public Map(bool isBlack = false)
		{
			NewMap(isBlack);
		}

		public bool IsPlayer => _storyStep % 2 == 0 ? !_startUserIsBlack : _startUserIsBlack;

		public static Map GetMap(SaveMapObject saveMap)
		{
			if (saveMap == null)
				return null;
			var result = new Map(saveMap.IsBlack);
			saveMap.Steps
				.ForEach(s => result._steps.Add(s));
			while (saveMap.StoryStep > result._storyStep)
				result.Redo();
			return result;
		}


		public SaveMapObject ToSaveObject()
		{
			return new SaveMapObject
			{
				IsBlack = _startUserIsBlack,
				Steps = _steps,
				StoryStep = _storyStep
			};
		}

		public void NewMap(bool isBlack)
		{
			_steps.Clear();
			_storyStep = 0;
			_startUserIsBlack = isBlack;
			Board = new Dictionary<Point, CheckerType>();
			for (var x = Config.MinX; x <= Config.MaxX; x++)
			for (var y = Config.MinY; y <= Config.MaxY; y++)
				if ((x + y) % 2 != 0 && (x < 3 || x > Config.MaxX - 3))
				{
					var point = new Point(x, y);
					Board.Add(point, CheckerTypeExtensions.Checker(
						x > 3 == isBlack,
						x > 3,
						x > 3
					));
				}
		}

		public List<StepResult> Undo()
		{
			var result = new List<StepResult>();
			if (_steps.Count == 0 || _storyStep == 0)
				return result;
			for (var i = _storyStep - 1; i >= 0; i--)
			{
				var step = _steps[i];
				result.Add(step);
				Previous(step);
				_storyStep--;
				if (step.OldChecker.IsPlayer()) return result;
			}

			return new List<StepResult>();
		}

		public List<StepResult> Redo()
		{
			var result = new List<StepResult>();
			if (_storyStep >= _steps.Count)
				return result;
			for (var i = _storyStep; i < _steps.Count; i++)
			{
				var step = _steps[i];
				result.Add(step);
				Next(step);
				_storyStep++;
				if (!step.NewChecker.IsPlayer()) return result;
			}

			return new List<StepResult>();
		}

		public void Add(StepResult data)
		{
			Next(data);
			while (_storyStep < _steps.Count)
				_steps.RemoveAt(_steps.Count - 1);
			_steps.Add(data);
			_storyStep++;
		}


		private void Previous(StepResult data)
		{
			Board.Remove(data.NewPosition);
			data.KilledCheckers?
				.ToList()
				.ForEach(ch => Board.Add(ch.Position, ch.Checker));
			Board.Add(data.OldPosition, data.OldChecker);
		}
	}
}