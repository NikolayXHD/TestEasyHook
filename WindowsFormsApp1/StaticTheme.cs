using System.Collections.Generic;
using System.Drawing;

namespace GitUI.Theming
{
	public class StaticTheme : Theme
	{
		private readonly IReadOnlyDictionary<KnownColor, Color> _sysColors;

		public StaticTheme(IReadOnlyDictionary<KnownColor, Color> sysColors)
		{
			_sysColors = sysColors;
		}

		protected override Color GetSysColor(KnownColor name) =>
			_sysColors.TryGetValue(name, out var result)
				? result
				: Color.Empty;
	}
}
