using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
	public static class ClipboardHelper
	{
		public static string ClipBoard
		{
			get
			{
				return GUIUtility.systemCopyBuffer;
			}

			set
			{

				GUIUtility.systemCopyBuffer = value;
			}
		}
	}
}
