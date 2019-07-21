using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using JetBrainsСheckers.BL.Helpers;
using JetBrainsСheckers.BL.Maps;
using JetBrainsСheckers.BL.Maps.Checkers;
using JetBrainsСheckers.BL.Maps.Steps;
using Timer = System.Timers.Timer;

namespace JetBrainsСheckers.BL
{
	public class Bot : IDisposable
	{
		//Можно доделать, чтобы после прохода, результат выбранного шага сохранялся 
		// и его выполнение продолжалось при следующем запуске
		// но так как уровень игры не важен, пусть так.

		private CancellationTokenSource _cancelToken;
		private Timer _timer;

		public void Dispose()
		{
			Reset();
		}

		private void StopThreads(object sender, ElapsedEventArgs e)
		{
			_cancelToken?.Cancel();
			_timer?.Stop();
		}

		public void ResetMap()
		{
		}

		public StepResult FoundNextStep(Map map, int time)
		{
			var root = new StepRating
			{
				Map = map.Clone(),
				IsPlayer = true
			};
			var queue = new ConcurrentQueue<StepRating>();
			var firstChildrens = new ConcurrentBag<StepRating>();

			queue.Enqueue(root);

			Reset();
			_cancelToken = new CancellationTokenSource();
			var token = _cancelToken.Token;
			_timer = new Timer(time);
			_timer.Elapsed += StopThreads;
			_timer.Start();

			void Action()
			{
				try
				{
					while (!token.IsCancellationRequested)
						if (queue.TryDequeue(out var step) && !token.IsCancellationRequested)
						{
							var checkers = step.Map.Where(s => s.Checker.IsPlayer() != step.IsPlayer).ToList();
							var steps = new List<StepResult>();
							checkers.ForEach(checker =>
								step.Map
									.CanAttack(checker)
									.ForEach(attackPoint =>
										steps.Add(step.Map.AttackResult(checker, attackPoint))));

							if (!steps.Any())
								checkers.ForEach(checker =>
									step.Map
										.CanStep(checker)
										.ForEach(move =>
											steps.Add(FoundSteps.MovementResult(checker, move))));


							foreach (var stepResult in steps)
							{
								var newMap = step.Map.Clone();
								newMap.Next(stepResult);
								var stepRating = new StepRating
								{
									Killed = stepResult.KilledCheckers?.Count ?? 0,
									Map = newMap,
									IsPlayer = !step.IsPlayer,
									Step = step.Parent == null //нужны только ходы первых детей
										? stepResult
										: null
								};
								step.AddChildren(stepRating);
								if (step.Parent == null)
									firstChildrens.Add(stepRating);
								queue.Enqueue(stepRating);
							}

							step.Map = null;
						}
				}
				catch (OutOfMemoryException)
				{
					_cancelToken?.Cancel();
					GC.Collect();
				}
			}

			var threads = Enumerable.Range(1, Environment.ProcessorCount * 2)
				.Select(a => new Thread(Action))
				.ToList();
			threads.ForEach(t => t.Start());
			threads.ForEach(t => t.Join());


			var result = firstChildrens
				.OrderByDescending(c => c.Killed / c.Dead)
				.ThenByDescending(c => c.Killed)
				.ThenBy(c => c.Dead)
				.FirstOrDefault();

			return result?.Step;
		}

		private void Reset()
		{
			_cancelToken?.Dispose();
			_timer?.Dispose();
		}
	}
}