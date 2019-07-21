using System;
using System.Windows.Media.Imaging;

namespace JetBrainsСheckers
{
	public static class Config
	{
		public const sbyte MinX = 0;
		public const sbyte MinY = 0;
		public const sbyte MaxX = 7;
		public const sbyte MaxY = 7;
		public static readonly BitmapImage BlackImage;
		public static readonly BitmapImage WhiteImage;
		public static readonly BitmapImage BlackKingImage;
		public static readonly BitmapImage WhiteKingImage;


		static Config()
		{
			BlackImage = new BitmapImage();
			BlackImage.BeginInit();
			BlackImage.UriSource = new Uri("black.png", UriKind.Relative);
			BlackImage.EndInit();
			WhiteImage = new BitmapImage();
			WhiteImage.BeginInit();
			WhiteImage.UriSource = new Uri("white.png", UriKind.Relative);
			WhiteImage.EndInit();

			BlackKingImage = new BitmapImage();
			BlackKingImage.BeginInit();
			BlackKingImage.UriSource = new Uri("blackKing.png", UriKind.Relative);
			BlackKingImage.EndInit();
			WhiteKingImage = new BitmapImage();
			WhiteKingImage.BeginInit();
			WhiteKingImage.UriSource = new Uri("whiteKing.png", UriKind.Relative);
			WhiteKingImage.EndInit();
		}
	}
}