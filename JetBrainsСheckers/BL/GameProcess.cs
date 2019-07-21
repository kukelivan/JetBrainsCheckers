using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using JetBrainsСheckers.BL.Helpers;
using JetBrainsСheckers.BL.Maps;
using JetBrainsСheckers.BL.Maps.Checkers;
using JetBrainsСheckers.BL.Maps.Steps;
using Newtonsoft.Json;
using Point = JetBrainsСheckers.BL.Maps.Checkers.Point;

namespace JetBrainsСheckers.BL
{
	public class GameProcess : IDisposable
	{
		public delegate void StepsHandler(List<StepResult> board);

		private readonly Bot _bot;
		private readonly Map _map;
		private readonly SaveStream _saveStream = new SaveStream();
		private int _botStep = 1000;

		public GameProcess()
		{
			_map = LoadMap() ?? new Map();
			_bot = new Bot();
			BotStep();
		}


		public void Dispose()
		{
			_saveStream?.Dispose();
			_bot?.Dispose();
		}


		private static Map LoadMap()
		{
			string obj;
			const string file = "save.json";
			if (!File.Exists(file))
				return null;

			using (var sr = new StreamReader(file, Encoding.Default))
			{
				obj = sr.ReadToEnd();
			}

			var deserializeObject = JsonConvert.DeserializeObject<SaveMapObject>(obj);
			return Map.GetMap(deserializeObject);
		}


		public void SetBotTimeout(int value)
		{
			_botStep = value;
		}

		public void NewGame(bool isWhite)
		{
			_map.NewMap(!isWhite);
			_bot.ResetMap();
			BotStep();
		}

		private void Save()
		{
			var saveObject = _map.ToSaveObject();
			var output = JsonConvert.SerializeObject(saveObject);
			_saveStream.Save(output);
		}

		public IEnumerable<CheckerPosition> GetBoard()
		{
			return _map;
		}


		public event StepsHandler UpdateBoardEvent;
		public event StepsHandler RevertBoardEvent;


		public void Undo()
		{
			var stepResults = _map.Undo();
			if (!stepResults.Any())
				return;
			UpdateBoardEvent?.Invoke(stepResults);
			Save();
		}

		public void Redo()
		{
			var stepResults = _map.Redo();
			if (!stepResults.Any())
				return;
			RevertBoardEvent?.Invoke(stepResults);
			Save();
		}

		private void BotStep()
		{
			if (!_map.IsPlayer)
				NewStep(_bot.FoundNextStep(_map, _botStep));
		}

		public void UserStep(Point oldPoint, Point newPosition)
		{
			if (_map.IsPlayer)
			{
				NewStep(CheckUserStep(oldPoint, newPosition));
				BotStep();
			}
		}

		private void NewStep(StepResult step)
		{
			if (step == null)
				return;
			_map.Add(step);
			Save();
			UpdateBoardEvent?.Invoke(new List<StepResult> { step });
		}


		private StepResult CheckUserStep(Point oldPoint, Point newPosition)
		{
			if (!_map.TryGetValue(oldPoint, out var checker))
				return null;

			var oldCheckerPosition = new CheckerPosition { Position = oldPoint, Checker = checker };
			var canAttack = _map.CanAttack(oldCheckerPosition);
			var attackPoint = canAttack.FirstOrDefault(ca => ca.NewStep == newPosition);
			if (attackPoint != null)
				return _map.AttackResult(oldCheckerPosition, attackPoint);

			if (_map.Where(c => c.Checker.IsPlayer() && c.Position != oldPoint)
				.Any(pair => _map.CanAttack(pair).Any()))
				return null;

			var canStep = _map.CanStep(oldCheckerPosition);
			return canStep.Contains(newPosition)
				? FoundSteps.MovementResult(oldCheckerPosition, newPosition)
				: null;
		}
	}
}