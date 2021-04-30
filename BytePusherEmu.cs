using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bytepusheremu
{
	public class BytePusherEmu : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Texture2D _currentDisplay;
		private CPU _gameCPU;


		public BytePusherEmu()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			_gameCPU = new CPU();

			_graphics.PreferredBackBufferWidth = 1028;
			_graphics.PreferredBackBufferHeight = 1028;
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


			_currentDisplay = CreateTexture(_graphics.GraphicsDevice, _gameCPU.GetDisplay());
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

		
	}
}
