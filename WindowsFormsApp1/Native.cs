namespace GitUI.Theming
{
	static class Native
	{
		public static class Edit
		{
			public static class Parts
			{
				public const int EP_EDITTEXT = 1;
				public const int EP_CARET = 2;
				public const int EP_BACKGROUND = 3;
				public const int EP_PASSWORD = 4;
				public const int EP_BACKGROUNDWITHBORDER = 5;
				public const int EP_EDITBORDER_NOSCROLL = 6;
				public const int EP_EDITBORDER_HSCROLL = 7;
				public const int EP_EDITBORDER_VSCROLL = 8;
				public const int EP_EDITBORDER_HVSCROLL = 9;
			}

			public static class EditStates
			{
				public const int ETS_NORMAL = 1;
				public const int ETS_HOT = 2;
				public const int ETS_SELECTED = 3;
				public const int ETS_DISABLED = 4;
				public const int ETS_FOCUSED = 5;
				public const int ETS_READONLY = 6;
				public const int ETS_ASSIST = 7;
				public const int ETS_CUEBANNER = 8;
			}

			public static class BackgroundStates
			{
				public const int EBS_NORMAL = 1;
				public const int EBS_HOT = 2;
				public const int EBS_DISABLED = 3;
				public const int EBS_FOCUSED = 4;
				public const int EBS_READONLY = 5;
				public const int EBS_ASSIST = 6;
			}

			public static class BackgroundWithBorderStates
			{
				public const int EBWBS_NORMAL = 1;
				public const int EBWBS_HOT = 2;
				public const int EBWBS_DISABLED = 3;
				public const int EBWBS_FOCUSED = 4;
			}

			public static class BorderNoScrollStates
			{
				public const int EPSN_NORMAL = 1;
				public const int EPSN_HOT = 2;
				public const int EPSN_FOCUSED = 3;
				public const int EPSN_DISABLED = 4;
			}

			public static class BorderHScrollStates
			{
				public const int EPSH_NORMAL = 1;
				public const int EPSH_HOT = 2;
				public const int EPSH_FOCUSED = 3;
				public const int EPSH_DISABLED = 4;
			}

			public static class BorderVScrollStates
			{
				public const int EPSV_NORMAL = 1;
				public const int EPSV_HOT = 2;
				public const int EPSV_FOCUSED = 3;
				public const int EPSV_DISABLED = 4;
			}

			public static class BorderHVScrollStates
			{
				public const int EPSHV_NORMAL = 1;
				public const int EPSHV_HOT = 2;
				public const int EPSHV_FOCUSED = 3;
				public const int EPSHV_DISABLED = 4;
			}
		}

		public static class ScrollBar
		{
			public static class Parts
			{
				public const int SBP_ARROWBTN = 1;
				public const int SBP_THUMBBTNHORZ = 2;
				public const int SBP_THUMBBTNVERT = 3;
				public const int SBP_LOWERTRACKHORZ = 4;
				public const int SBP_UPPERTRACKHORZ = 5;
				public const int SBP_LOWERTRACKVERT = 6;
				public const int SBP_UPPERTRACKVERT = 7;
				public const int SBP_GRIPPERHORZ = 8;
				public const int SBP_GRIPPERVERT = 9;
				public const int SBP_SIZEBOX = 10;
			}

			public static class States
			{
				public const int ABS_UPNORMAL = 1;
				public const int ABS_UPHOT = 2;
				public const int ABS_UPPRESSED = 3;
				public const int ABS_UPDISABLED = 4;
				public const int ABS_DOWNNORMAL = 5;
				public const int ABS_DOWNHOT = 6;
				public const int ABS_DOWNPRESSED = 7;
				public const int ABS_DOWNDISABLED = 8;
				public const int ABS_LEFTNORMAL = 9;
				public const int ABS_LEFTHOT = 10;
				public const int ABS_LEFTPRESSED = 11;
				public const int ABS_LEFTDISABLED = 12;
				public const int ABS_RIGHTNORMAL = 13;
				public const int ABS_RIGHTHOT = 14;
				public const int ABS_RIGHTPRESSED = 15;
				public const int ABS_RIGHTDISABLED = 16;
				public const int ABS_UPHOVER = 17;
				public const int ABS_DOWNHOVER = 18;
				public const int ABS_LEFTHOVER = 19;
				public const int ABS_RIGHTHOVER = 20;

				public const int SCRBS_NORMAL = 1;
				public const int SCRBS_HOT = 2;
				public const int SCRBS_PRESSED = 3;
				public const int SCRBS_DISABLED = 4;
				public const int SCRBS_HOVER = 5;

				public const int SZB_HALFBOTTOMLEFTALIGN = 6;
				public const int SZB_HALFBOTTOMRIGHTALIGN = 5;
				public const int SZB_HALFTOPLEFTALIGN = 8;
				public const int SZB_HALFTOPRIGHTALIGN = 7;
				public const int SZB_LEFTALIGN = 2;
				public const int SZB_RIGHTALIGN = 1;
				public const int SZB_TOPLEFTALIGN = 4;
				public const int SZB_TOPRIGHTALIGN = 3;
			}
		}
	}
}
