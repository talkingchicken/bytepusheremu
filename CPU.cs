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

		const uint KEYBOARD_STATE_ADDRESS = 0x0;
		const uint PROGRAM_COUNTER_ADDRESS = 0x2;
		const uint DISPLAY_POINTER = 0x5;
		const uint AUDIO_POINTER = 0x6;
		const uint AUDIO_SAMPLE_SIZE = 256;
		
		// CPU properties
		private byte[] memory;

		public CPU()
		{
			Reset();
		}

		private void Reset()
		{
			memory = new byte[16 * 1024 * 1024 + 8]; // 16 MiB
		}

		public void LoadGame(string name)
		{
			Reset();

			var reader = File.OpenRead(name);

			int currentIndex = 0;

			int currentByte;
			while((currentByte = reader.ReadByte()) != -1)
			{
				if (currentIndex >= (16 * 1024 * 1024))
				{
					throw new Exception("file too big");
				}

				memory[currentIndex++] = (byte)currentByte;
			}
		}
		
		public void RunOneFrame()
		{
			uint currentAddress = GetProgramCounterAddress();
			for (int i = 0; i < INSTRUCTIONS_PER_FRAME; i++)
			{
				currentAddress = InnerLoop(currentAddress);
			}
		}

		private uint InnerLoop(uint address)
		{
			uint[] instruction = GetInstructionAtAddress(address);

			ExecuteInstruction(instruction);
			return instruction[2];
		}

		private uint GetProgramCounterAddress()
		{
			return Get24BitValueAtAddress(PROGRAM_COUNTER_ADDRESS);
		}

		private void SetProgramCounter(uint value)
		{
			Store24BitValueAtAddress(PROGRAM_COUNTER_ADDRESS, value);
		}

		private uint Get24BitValueAtAddress(uint address)
		{
			return (uint)((memory[address] << 16) | (memory[address + 1] << 8) | memory[address + 2]);
		}
		private uint[] GetInstructionAtAddress(uint address)
		{
			uint[] returnValue = new uint[3];

			returnValue[0] = Get24BitValueAtAddress(address);
			returnValue[1] = Get24BitValueAtAddress(address + 3);
			returnValue[2] = Get24BitValueAtAddress(address + 6);

			return returnValue;
		}

		private void ExecuteInstruction(uint[] instruction)
		{
			memory[instruction[1]] = memory[instruction[0]];
		}
		
		private void Store24BitValueAtAddress(uint address, uint value)
		{
			memory[address + 2] = (byte)(value & 0xFF);
			memory[address + 1] = (byte)((value >> 2) & 0xFF);
			memory[address] = (byte)((value >> 4) & 0xFF);
		}
		public byte[,] GetDisplay()
		{
			byte[,] returnValue = new byte[SCREEN_HEIGHT, SCREEN_WIDTH];

			for (int x = 0; x < SCREEN_HEIGHT; x++)
			{
				for(int y = 0; y < SCREEN_WIDTH; y++)
				{
					int pixelAddress = (memory[DISPLAY_POINTER] << 16) + (y << 8) + x;
					byte pixelValue = memory[pixelAddress];
					returnValue[x, y] = pixelValue;
				}
			}

			return returnValue;
		}

		public void SetKeyboardState(bool[] keyboardState)
		{
			byte keysValue = 0;
			for (int i = 0; i < 8; i++)
			{
				keysValue = (byte)(keysValue << 1);
				keysValue |= (byte)(keyboardState[15 - i] ? 0x1 : 0x0);
			}

			memory[KEYBOARD_STATE_ADDRESS] = keysValue;
			
			keysValue = 0;
			for (int i = 0; i < 8; i++)
			{
				keysValue = (byte)(keysValue << 1);
				keysValue |= (byte)(keyboardState[15 - (i + 8)] ? 0x1 : 0x0);
			}

			memory[KEYBOARD_STATE_ADDRESS + 1] = keysValue;
		}

		public byte[] GetAudio()
		{
			byte[] returnValue = new byte[AUDIO_SAMPLE_SIZE];

			uint audioAddress = (uint)((memory[AUDIO_POINTER] << 16) | (memory[AUDIO_POINTER + 1] << 8));

			for (int i = 0; i < AUDIO_SAMPLE_SIZE; i++)
			{
				returnValue[i] = memory[audioAddress + i];
			}

			return returnValue;
		}
	}


}