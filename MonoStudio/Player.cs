using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoStudio
{


    public class Player : GameObject
    {
        public Player(Texture2D texture, Vector2 startPosition)
            : base("Player", texture, startPosition) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        public override void Update(GameTime gameTime) { /* Custom player logic */ }
    }

}
