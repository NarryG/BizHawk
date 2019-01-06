using System;

using BizHawk.Common.BufferExtensions;
using BizHawk.Emulation.Common;

using BizHawk.Emulation.Cores.Nintendo.NES;
using System.Runtime.InteropServices;

namespace BizHawk.Emulation.Cores.Nintendo.SUBNESHawk
{
	[Core(
		"SUBNESHawk",
		"",
		isPorted: false,
		isReleased: false)]
	[ServiceNotApplicable(typeof(IDriveLight))]
	public partial class SUBNESHawk : IEmulator, ISaveRam, IDebuggable, IStatable, IInputPollable, IRegionable,
	ISettable<SUBNESHawk.SUBNESHawkSettings, SUBNESHawk.SUBNESHawkSyncSettings>
	{
		// we want to create two GBHawk instances that we will run concurrently
		// maybe up to 4 eventually?
		public NES.NES subnes;

		[CoreConstructor("NES")]
		public SUBNESHawk(CoreComm comm, GameInfo game, byte[] rom, /*string gameDbFn,*/ object settings, object syncSettings)
		{
			var ser = new BasicServiceProvider(this);

			subnesSettings = (SUBNESHawkSettings)settings ?? new SUBNESHawkSettings();
			subnesSyncSettings = (SUBNESHawkSyncSettings)syncSettings ?? new SUBNESHawkSyncSettings();
			_controllerDeck = new SUBNESHawkControllerDeck(SUBNESHawkControllerDeck.DefaultControllerName, SUBNESHawkControllerDeck.DefaultControllerName);

			CoreComm = comm;

			var temp_set = new NES.NES.NESSettings();

			var temp_sync = new NES.NES.NESSyncSettings();

			subnes = new NES.NES(new CoreComm(comm.ShowMessage, comm.Notify) { CoreFileProvider = comm.CoreFileProvider },
				game, rom, temp_set, temp_sync);

			ser.Register<IVideoProvider>(subnes.videoProvider);
			ser.Register<ISoundProvider>(subnes.magicSoundProvider); 

			_tracer = new TraceBuffer { Header = subnes.cpu.TraceHeader };
			ser.Register<ITraceable>(_tracer);

			ServiceProvider = ser;

			SetupMemoryDomains();

			HardReset();
		}

		public void HardReset()
		{
			subnes.HardReset();
		}

		public DisplayType Region => DisplayType.NTSC;

		public int _frame = 0;

		private readonly SUBNESHawkControllerDeck _controllerDeck;

		private readonly ITraceable _tracer;

		private void ExecFetch(ushort addr)
		{
			MemoryCallbacks.CallExecutes(addr, "System Bus");
		}
	}
}
