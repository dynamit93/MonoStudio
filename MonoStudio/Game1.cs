using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoStudio
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private GameWorld gameWorld;
        private EditorUI editorUI;
        private Texture2D playerTexture;
        private Texture2D enemyTexture;
        private SpriteFont font; // Define the font
        private bool isInEditMode = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Hide the system mouse pointer
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures for different objects
            playerTexture = Content.Load<Texture2D>("player");
            enemyTexture = Content.Load<Texture2D>("enemy");

            // Load the font
            font = Content.Load<SpriteFont>("Font"); // Ensure you have a Font.spritefont in your Content folder

            // Initialize GameWorld and EditorUI with multiple textures
            gameWorld = new GameWorld();
            editorUI = new EditorUI(gameWorld, playerTexture, enemyTexture, GraphicsDevice, font); // Pass the font to EditorUI
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                isInEditMode = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                isInEditMode = false;
            }

            if (isInEditMode)
            {
                editorUI.Update(gameTime);
            }
            else
            {
                gameWorld.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (isInEditMode)
            {
                editorUI.Draw(spriteBatch);  // Draw editor UI and objects in edit mode
            }
            else
            {
                gameWorld.Draw(spriteBatch);  // Draw only game world in play mode
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
