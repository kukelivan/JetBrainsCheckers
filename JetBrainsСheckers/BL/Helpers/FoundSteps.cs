using System.Collections.Generic;
using System.Linq;
using JetBrainsСheckers.BL.Maps;
using JetBrainsСheckers.BL.Maps.Checkers;
using JetBrainsСheckers.BL.Maps.Steps;

namespace JetBrainsСheckers.BL.Helpers
{
	public static class FoundSteps
	{
		private static readonly Point[] PossibleSteps =
		{
			new Point(1, 1),
			new Point(1, -1),
			new Point(-1, -1),
			new Point(-1, 1)
		};

		public static List<AttackVariationPosition> CanAttack(this IMap map, CheckerPosition checkerPosition)
		{
			return map.CanAttack(checkerPosition.Checker, new AttackVariationPosition
			{
				NewStep = checkerPosition.Position,
				Opponents = new List<Point>(),
				IsKing = checkerPosition.Checker.IsKing()
			});
		}

		private static List<AttackVariationPosition> CanAttack(this IMap map, CheckerType checker,
			AttackVariationPosition previous)
		{
			return previous.IsKing
				? map.CanKingAttack(checker, previous)
				: map.CanPawnAttack(checker, previous);
		}

		private static List<AttackVariationPosition> CanPawnAttack(this IMap map, CheckerType checker,
			AttackVariationPosition previous)
		{
			return PossibleSteps.Select(pas =>
				{
					var opponent = pas + previous.NewStep;
					return new
					{
						Opponent = opponent,
						Step = pas + opponent
					};
				})
				.Where(pas =>
					!previous.Opponents.Contains(pas.Opponent)
					&& map.TryGetValue(pas.Opponent, out var opp)
					&& opp.IsPlayer() != checker.IsPlayer()
					&& !map.ContainsKey(pas.Step)
					&& InBoard(pas.Step))
				.SelectMany(pas =>
				{
					var newPos = new AttackVariationPosition
					{
						NewStep = pas.Step,
						Opponents = new List<Point>(previous.Opponents)
						{
							pas.Opponent
						},
						IsKing = previous.IsKing || IsSetKing(pas.Step, checker.IsStepDown())
					};

					var result = map.CanAttack(checker, newPos);
					result.Add(newPos);
					return result;
				})
				.ToList();
		}

		private static bool IsSetKing(Point position, bool isStepDown)
		{
			return isStepDown
				? position.X == Config.MinX
				: position.X == Config.MaxX;
		}

		private static List<AttackVariationPosition> CanKingAttack(this IMap map, CheckerType checker,
			AttackVariationPosition previous)
		{
			return PossibleSteps.SelectMany(pas =>
			{
				var opponent = previous.NewStep;
				while (InBoard(opponent += pas))
					if (map.TryGetValue(opponent, out var opp) && opp.IsPlayer() != checker.IsPlayer() &&
					    !previous.Opponents.Contains(opponent))
					{
						var positions = new List<Point>();
						var nextPosition = opponent;
						while (InBoard(nextPosition += pas) && !map.ContainsKey(nextPosition))
							positions.Add(nextPosition);
						return positions.Select(pos => new AttackVariationPosition
						{
							NewStep = pos,
							Opponents = new List<Point>(previous.Opponents)
							{
								opponent
							},
							IsKing = previous.IsKing || IsSetKing(pos, checker.IsStepDown())
						}).SelectMany(step =>
						{
							var result = map.CanAttack(checker, step);
							result.Add(step);
							return result;
						});
					}

				return Enumerable.Empty<AttackVariationPosition>();
			}).ToList();
		}

		private static bool InBoard(Point position)
		{
			return Config.MinX <= position.X && Config.MaxX >= position.X && Config.MinY <= position.Y &&
			       Config.MaxY >= position.Y;
		}

		public static StepResult MovementResult(CheckerPosition checkerPosition, Point newPosition)
		{
			return new StepResult
			{
				OldChecker = checkerPosition.Checker,
				NewChecker = !checkerPosition.Checker.IsKing() &&
				             IsSetKing(newPosition, checkerPosition.Checker.IsStepDown())
					? checkerPosition.Checker.ToKing()
					: checkerPosition.Checker,
				OldPosition = checkerPosition.Position,
				NewPosition = newPosition
			};
		}

		public static StepResult AttackResult(this IMap map, CheckerPosition checkerPosition,
			AttackVariationPosition attack)
		{
			return new StepResult
			{
				OldChecker = checkerPosition.Checker,
				NewPosition = attack.NewStep,
				OldPosition = checkerPosition.Position,
				NewChecker = !checkerPosition.Checker.IsKing() && attack.IsKing
					? checkerPosition.Checker.ToKing()
					: checkerPosition.Checker,
				KilledCheckers = attack.Opponents
					.Select(opp => new CheckerPosition
					{
						Position = opp,
						Checker = map.GetValue(opp)
					})
					.ToList()
			};
		}

		private static List<Point> CanPawnStep(this IMap map, Point position, bool isStepDown)
		{
			return PossibleSteps
				.Where(pm => pm.X < 0 == isStepDown)
				.Select(pm => pm + position)
				.Where(pm => !map.ContainsKey(pm)
				             && InBoard(pm))
				.ToList();
		}

		public static List<Point> CanStep(this IMap map, CheckerPosition checkerPosition)
		{
			return checkerPosition.Checker.IsKing()
				? map.CanKingStep(checkerPosition.Position)
				: map.CanPawnStep(checkerPosition.Position, checkerPosition.Checker.IsStepDown());
		}

		private static List<Point> CanKingStep(this IMap map, Point position)
		{
			return PossibleSteps.SelectMany(ps =>
				{
					var result = new List<Point>();
					var nextPosition = position;
					while (InBoard(nextPosition += ps) && !map.ContainsKey(nextPosition))
						result.Add(nextPosition);
					return result;
				}
			).ToList();
		}
	}
}