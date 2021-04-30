using System;
using System.Collections.Generic;
using System.IO;

namespace bytepusheremu
{
	public class CPU
	{
		const int INSTRUCTIONS_PER_FRAME = 65536;
		const int SCREEN_WIDTH = 256;
		const int SCREEN_HEIGHT = 256;
		const int NUMBER_OF_KEYS = 16;
		
		// CPU properties
		private byte[] memory;
		private byte[,] display;

		private bool[] keys;
		
		public CPU()
		{
			Reset();
		}

		private void Reset()
		{
			memory = new byte[16 * 1024 * 1024]; // 16 MiB
			display = new byte[256, 256];
			keys = new bool[NUMBER_OF_KEYS];
		}

		public void LoadGame(string name)
		{
			Reset();
		}
		
		public void RunOneFrame()
		{

		}

		private void InnerLoop()
		{

		}

		public byte[,] GetDisplay()
		{
			return display;
		}
	}


}