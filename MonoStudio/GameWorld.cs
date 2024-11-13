using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoStudio
{


    public class GameWorld
    {
        private List<GameObject> gameObjects;

        public GameWorld()
        {
            gameObjects = new List<GameObject>();
        }

        public void AddObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        public void RemoveObject(GameObject gameObject)
        {
            gameObjects.Remove(gameObject);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var obj in gameObjects)
            {
                obj.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var obj in gameObjects)
            {
                obj.Draw(spriteBatch);
            }
        }

        public List<GameObject> GetObjects() => gameObjects;
    }

}
