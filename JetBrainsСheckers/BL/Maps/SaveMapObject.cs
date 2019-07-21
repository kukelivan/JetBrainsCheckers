using System.Collections.Generic;
using JetBrainsСheckers.BL.Maps.Steps;

namespace JetBrainsСheckers.BL.Maps
{
	public class SaveMapObject
	{
		public List<StepResult> Steps { get; set; }
		public int StoryStep { get; set; }
		public bool IsBlack { get; set; }
	}
}