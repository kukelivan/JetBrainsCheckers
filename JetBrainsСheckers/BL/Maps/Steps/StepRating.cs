namespace JetBrainsСheckers.BL.Maps.Steps
{
	public class StepRating
	{
		public StepResult Step { get; set; }
		public MapLite Map { get; set; }

		public bool IsPlayer { get; set; }
		public float Killed { get; set; }
		public float Dead { get; set; }

		public int Level { get; set; }
		public StepRating Parent { get; set; }

		private void AddValue(StepRating value)
		{
			var item = this;
			while (item != null)
			{
				if (value.IsPlayer)
				{
					item.Killed += value.Dead / value.Level;
					item.Dead += value.Killed / value.Level;
				}
				else
				{
					item.Killed += value.Killed / value.Level;
					item.Dead += value.Dead / value.Level;
				}

				item = item.Parent;
			}
		}

		public void AddChildren(StepRating value)
		{
			value.Parent = Parent?.Parent == null ? this : Parent;
			value.Level = Level + (value.IsPlayer ? 0 : 1);

			AddValue(value);
		}
	}
}