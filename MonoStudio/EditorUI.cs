using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoStudio
{


    public class EditorUI
    {
        private GameWorld gameWorld;
        private Texture2D playerTexture, enemyTexture;
        private bool isDragging;
        private GameObject draggedObject;
        private GameObject selectedObjectTemplate;
        private List<GameObject> objectPalette;
        private Texture2D panelTexture;
        private SpriteFont font;


        private GameObject contextMenuTarget; // The object for which the context menu is shown

        // Context menu properties
        private bool isContextMenuVisible = false;
        private Vector2 contextMenuPosition;
        private List<string> contextMenuOptions = new List<string> { "Set Camera", "Remove Object" };
        private int contextMenuWidth = 120;
        private int contextMenuItemHeight = 30;

        // Dropdown-related fields
        private bool isDropdownExpanded = false;
        private int dropdownWidth = 150;
        private int dropdownHeight = 30;
        private int dropdownItemHeight = 30;
        private int dropdownX;
        private int dropdownY;

        // Camera properties
        private Vector2 cameraPosition = Vector2.Zero;
        private Vector2 defaultMovement = new Vector2(1.0f, 1.0f); // Default camera movement speed


        private string selectedObjectName = "Select Object";

        private int panelWidth;
        private int panelHeight;
        private int screenWidth;
        private int screenHeight;

        public EditorUI(GameWorld world, Texture2D playerTex, Texture2D enemyTex, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            gameWorld = world;
            playerTexture = playerTex;
            enemyTexture = enemyTex;
            this.font = font;  // Assign the font to the field

            // Define palette with available objects
            objectPalette = new List<GameObject>
        {
            new Player(playerTexture, Vector2.Zero), // Placeholder position
            new Enemy(enemyTexture, Vector2.Zero)
            // Add other object types here
        };

            selectedObjectTemplate = objectPalette[0];  // Default to first object (e.g., Player)

            // Set up panel dimensions (20% of screen width)
            screenWidth = graphicsDevice.Viewport.Width;
            screenHeight = graphicsDevice.Viewport.Height;
            panelWidth = screenWidth / 5;
            panelHeight = screenHeight;

            // Dropdown position within the right panel
            dropdownX = screenWidth - panelWidth + 10;
            dropdownY = 10;

            // Create a grey texture for the panel
            panelTexture = new Texture2D(graphicsDevice, 1, 1);
            panelTexture.SetData(new[] { Color.Gray });
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            var mousePosition = mouseState.Position.ToVector2();

            // Handle camera movement (e.g., using arrow keys)
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Right)) cameraPosition.X += defaultMovement.X;
            if (keyboardState.IsKeyDown(Keys.Left)) cameraPosition.X -= defaultMovement.X;
            if (keyboardState.IsKeyDown(Keys.Up)) cameraPosition.Y -= defaultMovement.Y;
            if (keyboardState.IsKeyDown(Keys.Down)) cameraPosition.Y += defaultMovement.Y;


            // Update the camera position dynamically in the code
            CodeGenerator.UpdateCameraCode(cameraPosition, defaultMovement);

            // Right-click to open context menu if over an object
            if (mouseState.RightButton == ButtonState.Pressed && !isContextMenuVisible)
            {
                foreach (var obj in gameWorld.GetObjects())
                {
                    if (obj.ContainsPoint(mousePosition))
                    {
                        contextMenuTarget = obj;
                        contextMenuPosition = mousePosition;
                        isContextMenuVisible = true;
                        break;
                    }
                }
            }

            // Toggle dropdown expansion on click
            if (mouseState.LeftButton == ButtonState.Pressed &&
                new Rectangle(dropdownX, dropdownY, dropdownWidth, dropdownHeight).Contains(mousePosition))
            {
                isDropdownExpanded = !isDropdownExpanded;
            }

            // Handle dropdown item selection if expanded
            if (isDropdownExpanded)
            {
                for (int i = 0; i < objectPalette.Count; i++)
                {
                    int itemY = dropdownY + dropdownHeight + i * dropdownItemHeight;
                    if (mouseState.LeftButton == ButtonState.Pressed &&
                        new Rectangle(dropdownX, itemY, dropdownWidth, dropdownItemHeight).Contains(mousePosition))
                    {
                        SelectObject(objectPalette[i]);
                        selectedObjectName = objectPalette[i].ObjectName;
                        isDropdownExpanded = false; // Collapse dropdown after selection
                    }
                }
            }

            // Hide context menu if clicking outside of it
            if (isContextMenuVisible && mouseState.LeftButton == ButtonState.Pressed && !new Rectangle((int)contextMenuPosition.X, (int)contextMenuPosition.Y, contextMenuWidth, contextMenuItemHeight * contextMenuOptions.Count).Contains(mousePosition))
            {
                isContextMenuVisible = false;
            }

            // Handle left-click on context menu options
            if (isContextMenuVisible && mouseState.LeftButton == ButtonState.Pressed)
            {
                for (int i = 0; i < contextMenuOptions.Count; i++)
                {
                    Rectangle optionRect = new Rectangle((int)contextMenuPosition.X, (int)contextMenuPosition.Y + i * contextMenuItemHeight, contextMenuWidth, contextMenuItemHeight);

                    if (optionRect.Contains(mousePosition))
                    {
                        HandleContextMenuOption(contextMenuOptions[i]);
                        isContextMenuVisible = false;
                        break;
                    }
                }
            }

            // Dragging logic with position adjustment for centering
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!isDragging)
                {
                    isDragging = true;
                    draggedObject = CloneSelectedObject(mousePosition);
                }
                else if (isDragging && draggedObject != null)
                {
                    // Center object on the cursor by offsetting its position
                    draggedObject.Position = mousePosition - new Vector2(draggedObject.Texture.Width / 2, draggedObject.Texture.Height / 2);
                }
            }
            else if (mouseState.LeftButton == ButtonState.Released && isDragging)
            {
                gameWorld.AddObject(draggedObject);
                CodeGenerator.GenerateObjectCode(draggedObject);
                isDragging = false;
                draggedObject = null;
            }

            // Optionally update all code at specific intervals
            CodeGenerator.UpdateAllCode(gameWorld.GetObjects(), cameraPosition, defaultMovement);

        }


        private GameObject CloneSelectedObject(Vector2 position)
        {
            return selectedObjectTemplate switch
            {
                Player => new Player(playerTexture, position),
                Enemy => new Enemy(enemyTexture, position),
                _ => null
            };
        }

        public void SelectObject(GameObject selectedObject)
        {
            selectedObjectTemplate = selectedObject;
        }

        private void HandleContextMenuOption(string option)
        {
            if (option == "Set Camera")
            {
                SetCameraToTarget(contextMenuTarget);
                // Code to set the camera position to this object's position
                System.Console.WriteLine($"Set camera to {contextMenuTarget.ObjectName} at {contextMenuTarget.Position}");
            }
            else if (option == "Remove Object")
            {
                gameWorld.RemoveObject(contextMenuTarget);
                contextMenuTarget = null;
            }
        }

        private void SetCameraToTarget(GameObject target)
        {
            if (target != null)
            {
                // Set the camera position to the target’s position and apply default movement
                cameraPosition = target.Position;
                System.Console.WriteLine($"Camera set to {target.ObjectName} at {cameraPosition} with default movement {defaultMovement}");
                CodeGenerator.UpdateCameraCode(cameraPosition, defaultMovement);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the grey panel on the right side
            DrawRightPanel(spriteBatch);

            // Draw the dropdown menu for object selection
            DrawDropdownMenu(spriteBatch);

            // Draw draggable object
            if (isDragging && draggedObject != null)
            {
                draggedObject.Draw(spriteBatch);
            }

            // Draw all placed objects
            gameWorld.Draw(spriteBatch);

            // Draw selected object at mouse position as a cursor
            DrawSelectedObjectAsCursor(spriteBatch);


            // Draw context menu if visible
            if (isContextMenuVisible)
            {
                DrawContextMenu(spriteBatch);
            }

        }

        private void DrawRightPanel(SpriteBatch spriteBatch)
        {
            // Draw the panel as a grey rectangle on the right side of the screen
            Rectangle panelRect = new Rectangle(screenWidth - panelWidth, 0, panelWidth, panelHeight);
            spriteBatch.Draw(panelTexture, panelRect, Color.White);
        }

        private void DrawDropdownMenu(SpriteBatch spriteBatch)
        {
            // Draw the dropdown header (collapsed view)
            spriteBatch.Draw(panelTexture, new Rectangle(dropdownX, dropdownY, dropdownWidth, dropdownHeight), Color.DarkGray);
            spriteBatch.DrawString(font, selectedObjectName, new Vector2(dropdownX + 5, dropdownY + 5), Color.White);

            if (isDropdownExpanded)
            {
                // Draw each item in the dropdown list when expanded
                for (int i = 0; i < objectPalette.Count; i++)
                {
                    int itemY = dropdownY + dropdownHeight + i * dropdownItemHeight;
                    spriteBatch.Draw(panelTexture, new Rectangle(dropdownX, itemY, dropdownWidth, dropdownItemHeight), Color.Gray);
                    spriteBatch.DrawString(font, objectPalette[i].ObjectName, new Vector2(dropdownX + 5, itemY + 5), Color.White);
                }
            }
        }

        private void DrawContextMenu(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < contextMenuOptions.Count; i++)
            {
                Rectangle optionRect = new Rectangle((int)contextMenuPosition.X, (int)contextMenuPosition.Y + i * contextMenuItemHeight, contextMenuWidth, contextMenuItemHeight);
                spriteBatch.Draw(panelTexture, optionRect, Color.DarkGray);
                spriteBatch.DrawString(font, contextMenuOptions[i], new Vector2(optionRect.X + 5, optionRect.Y + 5), Color.White);
            }
        }
        private void DrawSelectedObjectAsCursor(SpriteBatch spriteBatch)
        {
            if (selectedObjectTemplate != null && !isDragging)
            {
                var mousePosition = Mouse.GetState().Position.ToVector2();
                spriteBatch.Draw(selectedObjectTemplate.Texture, mousePosition, Color.White * 0.7f); // Slightly transparent
            }
        }
    }


}
