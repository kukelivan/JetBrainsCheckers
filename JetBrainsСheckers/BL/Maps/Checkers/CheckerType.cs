using System;

namespace JetBrainsСheckers.BL.Maps.Checkers
{
	[Flags]
	public enum CheckerType : byte
	{
		None = 0,
		IsBlack = 1 << 1,
		IsPlayer = 1 << 2,
		IsStepDown = 1 << 3,
		IsKing = 1 << 4
	}

	public static class CheckerTypeExtensions
	{
		public static bool IsBlack(this CheckerType type)
		{
			return type.HasFlag(CheckerType.IsBlack);
		}

		public static bool IsPlayer(this CheckerType type)
		{
			return type.HasFlag(CheckerType.IsPlayer);
		}

		public static bool IsStepDown(this CheckerType type)
		{
			return type.HasFlag(CheckerType.IsStepDown);
		}

		public static bool IsKing(this CheckerType type)
		{
			return type.HasFlag(CheckerType.IsKing);
		}

		public static CheckerType ToKing(this CheckerType type)
		{
			return type | CheckerType.IsKing;
		}

		public static CheckerType Checker(bool isBlack, bool isPlayer, bool isStepDown)
		{
			var type = CheckerType.None;
			if (isBlack)
				type |= CheckerType.IsBlack;
			if (isPlayer)
				type |= CheckerType.IsPlayer;
			if (isStepDown)
				type |= CheckerType.IsStepDown;
			return type;
		}
	}
}