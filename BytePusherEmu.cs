using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace bytepusheremu
{
	public class BytePusherEmu : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Texture2D _currentDisplay;
		private CPU _gameCPU;
		private DynamicSoundEffectInstance _soundEffect;


		public BytePusherEmu()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			_soundEffect = new DynamicSoundEffectInstance(15360, AudioChannels.Mono);
			_soundEffect.Play();
			_gameCPU = new CPU();
			_gameCPU.LoadGame("testroms/Sprites.BytePusher");
			_graphics.PreferredBackBufferWidth = 512;
			_graphics.PreferredBackBufferHeight = 512;
			_graphics.ApplyChanges();
			TargetElapsedTime = TimeSpan.FromSeconds(1.0f/60);
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			_gameCPU.SetKeyboardState(GetKeyboardState());

			_gameCPU.RunOneFrame();

			_currentDisplay = CreateTexture(_graphics.GraphicsDevice, _gameCPU.GetDisplay());

			byte[] rawAudio = _gameCPU.GetAudio();
			byte[] sixteenBitAudio = new byte[rawAudio.Length * 2];

			for(int i = 0; i < rawAudio.Length; i++)
			{
				sixteenBitAudio[i*2] = 0;
				sixteenBitAudio[i*2 + 1] = rawAudio[i];
			}

			_soundEffect.SubmitBuffer(sixteenBitAudio);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			if (_currentDisplay == null)
				return;

			byte[,] screen = _gameCPU.GetDisplay();

			float minScale = Math.Min((float)_graphics.PreferredBackBufferWidth / screen.GetLength(0), (float)_graphics.PreferredBackBufferHeight / screen.GetLength(1));
			_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
			_spriteBatch.Draw(_currentDisplay, Vector2.Zero, new Rectangle(0, 0, screen.GetLength(0), screen.GetLength(1)), Color.White, 0f, Vector2.Zero, minScale, SpriteEffects.None, 0f);
			_spriteBatch.End();

			base.Draw(gameTime);
		}

		private static Texture2D CreateTexture(GraphicsDevice device, byte[,] pixelData)
		{
			int width = pixelData.GetLength(0);
			int height = pixelData.GetLength(1);
			Texture2D texture  = new Texture2D(device, width, height);

			Color[] data = new Color[width * height];
			
			int counter = 0;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					byte currentColor = pixelData[x,y];
					int blueValue = (currentColor % 6) * 0x33;
					int greenValue = ((currentColor / 6) % 6) * 0x33;
					int redValue = ((currentColor / 36) % 6) * 0x33;
					
					data[counter++] = new Color(redValue, greenValue, blueValue);
				}
			}
			
			texture.SetData(data);

			return texture;
		}

		private bool[] GetKeyboardState()
		{
			bool[] keys = new bool[16];
			KeyboardState state = Keyboard.GetState();

			keys[0x1] = state.IsKeyDown(Keys.D1);
			keys[0x2] = state.IsKeyDown(Keys.D2);
			keys[0x3] = state.IsKeyDown(Keys.D3);
			keys[0xC] = state.IsKeyDown(Keys.D4);
			keys[0x4] = state.IsKeyDown(Keys.Q);
			keys[0x5] = state.IsKeyDown(Keys.W);
			keys[0x6] = state.IsKeyDown(Keys.E);
			keys[0xD] = state.IsKeyDown(Keys.R);
			keys[0x7] = state.IsKeyDown(Keys.A);
			keys[0x8] = state.IsKeyDown(Keys.S);
			keys[0x9] = state.IsKeyDown(Keys.D);
			keys[0xE] = state.IsKeyDown(Keys.F);
			keys[0xA] = state.IsKeyDown(Keys.Z);
			keys[0x0] = state.IsKeyDown(Keys.X);
			keys[0xB] = state.IsKeyDown(Keys.C);
			keys[0xF] = state.IsKeyDown(Keys.V);

			return keys;
		}
	}
}
