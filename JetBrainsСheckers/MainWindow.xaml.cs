using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrainsСheckers.BL;
using JetBrainsСheckers.BL.Maps.Checkers;
using JetBrainsСheckers.BL.Maps.Steps;
using Point = JetBrainsСheckers.BL.Maps.Checkers.Point;

namespace JetBrainsСheckers
{
	/// <summary>
	///     Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Dictionary<Point, Cell> _cells = new Dictionary<Point, Cell>(64);
		private readonly SolidColorBrush _color1 = new SolidColorBrush(Color.FromRgb(240, 240, 240));
		private readonly SolidColorBrush _color2 = new SolidColorBrush(Color.FromRgb(61, 49, 51));
		private readonly GameProcess _board;
		private Point _selectChecker;


		public MainWindow()
		{
			InitializeComponent();
			for (sbyte i = 0; i < 8; i++)
			for (sbyte j = 0; j < 8; j++)
			{
				var cell = new Cell(new Point(i, j))
				{
					Background = (i + j) % 2 == 0 ? _color1 : _color2
				};
				ChessBoard.Children.Add(cell);
				Grid.SetRow(cell, i);
				Grid.SetColumn(cell, j);
				cell.Click += Cell_Click;
				_cells.Add(new Point(i, j), cell);
			}

			_board = new GameProcess();
			foreach (var cell in _cells.Values) cell.SetChecker(null);
			_board.GetBoard()
				.ToList()
				.ForEach(c => _cells[c.Position].SetChecker(c.Checker));


			_board.UpdateBoardEvent += UpdateBoard;
			_board.RevertBoardEvent += RevertBoard;
			Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			_board.Dispose();
		}

		private void RevertBoard(List<StepResult> steps)
		{
			foreach (var step in steps)
			{
				_cells[step.NewPosition].SetChecker(null);
				step.KilledCheckers?
					.ForEach(dp => _cells[dp.Position].SetChecker(dp.Checker));
				_cells[step.OldPosition].SetChecker(step.OldChecker);
			}
		}

		private void UpdateBoard(List<StepResult> steps)
		{
			foreach (var step in steps)
			{
				_cells[step.OldPosition].SetChecker(null);
				step.KilledCheckers?
					.ForEach(dp => _cells[dp.Position].SetChecker(null));
				_cells[step.NewPosition].SetChecker(step.NewChecker);
			}
		}

		private void NewGame()
		{
			var isWhite = ForWhite.IsChecked == true;
			_board.NewGame(isWhite);
			foreach (var cell in _cells.Values) cell.SetChecker(null);
			_board.GetBoard()
				.ToList()
				.ForEach(c => _cells[c.Position].SetChecker(c.Checker));
		}

		private void Cell_Click(Point point, CheckerType? checker)
		{
			if (checker != null && checker.Value.IsPlayer())
				_selectChecker = point;
			else if (_selectChecker != null)
				_board.UserStep(_selectChecker, point);
			else
				_selectChecker = null;
		}


		private void UndoButton_Click(object sender, RoutedEventArgs e)
		{
			_board.Undo();
		}

		private void RedoButton_Click(object sender, RoutedEventArgs e)
		{
			_board.Redo();
		}

		private void NewGameButton_Click(object sender, RoutedEventArgs e)
		{
			NewGame();
		}

		private void SetButton_Click(object sender, RoutedEventArgs e)
		{
			var text = BotStepTime.Text;
			if (int.TryParse(text, out var value))
				_board.SetBotTimeout(value);
		}
	}
}