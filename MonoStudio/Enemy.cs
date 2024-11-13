using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Enemy.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoStudio
{


    public class Enemy : GameObject
    {
        public Enemy(Texture2D texture, Vector2 startPosition)
            : base("Enemy", texture, startPosition) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.Red);  // Unique color or texture for enemy
        }

        public override void Update(GameTime gameTime) { /* Custom enemy logic */ }
    }

}
