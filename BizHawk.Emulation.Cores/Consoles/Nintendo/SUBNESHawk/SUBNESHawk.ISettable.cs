using System;
using System.ComponentModel;

using Newtonsoft.Json;

using BizHawk.Common;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Nintendo.NES;

namespace BizHawk.Emulation.Cores.Nintendo.SUBNESHawk
{
	public partial class SUBNESHawk : IEmulator, IStatable, ISettable<SUBNESHawk.SUBNESHawkSettings, SUBNESHawk.SUBNESHawkSyncSettings>
	{
		public SUBNESHawkSettings GetSettings()
		{
			return subnesSettings.Clone();
		}

		public SUBNESHawkSyncSettings GetSyncSettings()
		{
			return subnesSyncSettings.Clone();
		}

		public bool PutSettings(SUBNESHawkSettings o)
		{
			subnesSettings = o;
			return false;
		}

		public bool PutSyncSettings(SUBNESHawkSyncSettings o)
		{
			bool ret = SUBNESHawkSyncSettings.NeedsReboot(subnesSyncSettings, o);
			subnesSyncSettings = o;
			return ret;
		}

		private SUBNESHawkSettings subnesSettings = new SUBNESHawkSettings();
		public SUBNESHawkSyncSettings subnesSyncSettings = new SUBNESHawkSyncSettings();

		public class SUBNESHawkSettings
		{
			public SUBNESHawkSettings Clone()
			{
				return (SUBNESHawkSettings)MemberwiseClone();
			}
		}

		public class SUBNESHawkSyncSettings
		{
			public SUBNESHawkSyncSettings Clone()
			{
				return (SUBNESHawkSyncSettings)MemberwiseClone();
			}

			public static bool NeedsReboot(SUBNESHawkSyncSettings x, SUBNESHawkSyncSettings y)
			{
				return !DeepEquality.DeepEquals(x, y);
			}
		}
	}
}
