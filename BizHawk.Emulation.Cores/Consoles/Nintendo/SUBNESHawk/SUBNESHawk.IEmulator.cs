﻿using BizHawk.Common.NumberExtensions;
using BizHawk.Emulation.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BizHawk.Emulation.Cores.Nintendo.NES;

namespace BizHawk.Emulation.Cores.Nintendo.SUBNESHawk
{
	public partial class SUBNESHawk : IEmulator
	{
		public IEmulatorServiceProvider ServiceProvider { get; }

		public ControllerDefinition ControllerDefinition => _controllerDeck.Definition;

		public bool FrameAdvance(IController controller, bool render, bool rendersound)
		{
			//Console.WriteLine("-----------------------FRAME-----------------------");
			if (_tracer.Enabled)
			{
				subnes.cpu.TraceCallback = s => _tracer.Put(s);
			}
			else
			{
				subnes.cpu.TraceCallback = null;
			}

			_frame++;

			if (controller.IsPressed("Power"))
			{
				HardReset();
			}

			_islag = true;

			GetControllerState(controller);

			do_frame();

			bool ret = pass_a_frame;

			if (pass_a_frame)
			{
				subnes.videoProvider.FillFrameBuffer();
				Console.Write("pass: ");
				Console.WriteLine(_frame);
			}
			
			_islag = subnes.islag;

			if (_islag)
			{
				_lagcount++;
			}

			return ret;
		}

		public bool stop_cur_frame;
		public bool pass_a_frame;

		public void do_frame()
		{
			stop_cur_frame = false;
			while (!stop_cur_frame)
			{
				subnes.do_single_step(out stop_cur_frame, out pass_a_frame);
				stop_cur_frame |= pass_a_frame;
			}
		}

		public void GetControllerState(IController controller)
		{
			InputCallbacks.Call();
		}

		public int Frame => _frame;

		public string SystemId => "NES";

		public bool DeterministicEmulation { get; set; }

		public void ResetCounters()
		{
			_frame = 0;
			_lagcount = 0;
			_islag = false;
		}

		public CoreComm CoreComm { get; }

		public void Dispose()
		{
			subnes.Dispose();
		}
		/*
		#region Video provider

		public int _frameHz = 60;

		public int[] _vidbuffer = new int[160 * 2 * 144];
		public int[] buff_L = new int[160 * 144];
		public int[] buff_R = new int[160 * 144];

		public int[] GetVideoBuffer()
		{
			// combine the 2 video buffers from the instances
			for (int i = 0; i < 144; i++)
			{
				for (int j = 0; j < 160; j++)
				{
					_vidbuffer[i * 320 + j] = buff_L[i * 160 + j];
					_vidbuffer[i * 320 + j + 160] = buff_R[i * 160 + j];
				}				
			}

			return _vidbuffer;		
		}

		public int VirtualWidth => 160 * 2;
		public int VirtualHeight => 144;
		public int BufferWidth => 160 * 2;
		public int BufferHeight => 144;
		public int BackgroundColor => unchecked((int)0xFF000000);
		public int VsyncNumerator => _frameHz;
		public int VsyncDenominator => 1;

		public static readonly uint[] color_palette_BW = { 0xFFFFFFFF , 0xFFAAAAAA, 0xFF555555, 0xFF000000 };
		public static readonly uint[] color_palette_Gr = { 0xFFA4C505, 0xFF88A905, 0xFF1D551D, 0xFF052505 };

		public uint[] color_palette = new uint[4];

		#endregion
		
		#region audio

		public bool CanProvideAsync => false;

		public void SetSyncMode(SyncSoundMode mode)
		{
			if (mode != SyncSoundMode.Sync)
			{
				throw new InvalidOperationException("Only Sync mode is supported_");
			}
		}

		public SyncSoundMode SyncMode => SyncSoundMode.Sync;

		public void GetSamplesSync(out short[] samples, out int nsamp)
		{
			short[] temp_samp_L;
			short[] temp_samp_R;

			int nsamp_L;
			int nsamp_R;

			L.audio.GetSamplesSync(out temp_samp_L, out nsamp_L);
			R.audio.GetSamplesSync(out temp_samp_R, out nsamp_R);

			if (linkSettings.AudioSet == GBLinkSettings.AudioSrc.Left)
			{
				samples = temp_samp_L;
				nsamp = nsamp_L;
			}
			else if (linkSettings.AudioSet == GBLinkSettings.AudioSrc.Right)
			{
				samples = temp_samp_R;
				nsamp = nsamp_R;
			}
			else
			{
				samples = new short[0];
				nsamp = 0;
			}
		}

		public void GetSamplesAsync(short[] samples)
		{
			throw new NotSupportedException("Async is not available");
		}

		public void DiscardSamples()
		{
			L.audio.DiscardSamples();
			R.audio.DiscardSamples();
		}

		private void GetSamples(short[] samples)
		{

		}

		public void DisposeSound()
		{
			L.audio.DisposeSound();
			R.audio.DisposeSound();
		}

		#endregion
		*/
	}
}
