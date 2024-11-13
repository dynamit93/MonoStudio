using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// GameObject.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoStudio
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public string ObjectName { get; set; }

        public GameObject(string objectName, Texture2D texture, Vector2 startPosition)
        {
            ObjectName = objectName;
            Texture = texture;
            Position = startPosition;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);

        public bool ContainsPoint(Vector2 point)
        {
            Rectangle bounds = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            return bounds.Contains(point);
        }
    }

}
