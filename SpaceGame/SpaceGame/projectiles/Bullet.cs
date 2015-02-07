using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceGame
{
    public abstract class Bullet
    {
        public abstract void update(GameTime gameTime);

        public abstract bool deleteMe();

        public abstract Rectangle getHitBox();

        public abstract Vector2 getLocationVector();

        public abstract Rectangle getRectangle();

        public abstract float getRotation();

        public abstract Vector2 getBulletOrigin();
    }
}
