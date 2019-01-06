﻿using System.IO;
using Newtonsoft.Json;

using BizHawk.Common;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Nintendo.NES;

namespace BizHawk.Emulation.Cores.Nintendo.SubNESHawk
{
	public partial class SubNESHawk : IStatable
	{
		public bool BinarySaveStatesPreferred => true;

		public void SaveStateText(TextWriter writer)
		{
			subnes.SaveStateText(writer);
			SyncState(new Serializer(writer));
		}

		public void LoadStateText(TextReader reader)
		{
			subnes.LoadStateText(reader);
			SyncState(new Serializer(reader));
		}

		public void SaveStateBinary(BinaryWriter bw)
		{
			subnes.SaveStateBinary(bw);
			// other variables
			SyncState(new Serializer(bw));
		}

		public void LoadStateBinary(BinaryReader br)
		{
			subnes.LoadStateBinary(br);
			// other variables
			SyncState(new Serializer(br));
		}

		public byte[] SaveStateBinary()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			SaveStateBinary(bw);
			bw.Flush();
			return ms.ToArray();
		}

		//private JsonSerializer ser = new JsonSerializer { Formatting = Formatting.Indented };

		private void SyncState(Serializer ser)
		{
			ser.Sync("Lag", ref _lagcount);
			ser.Sync("Frame", ref _frame);
			ser.Sync("IsLag", ref _islag);
			ser.Sync("pass_a_frame", ref pass_a_frame);

			_controllerDeck.SyncState(ser);
		}
	}
}
