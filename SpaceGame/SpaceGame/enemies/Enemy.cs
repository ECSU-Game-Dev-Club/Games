using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceGame
{
    class Enemy
    {
        //SWARM
        Texture2D swarmTexture;
        Texture2D swarmTexture_WHITE;



        public Enemy(IServiceProvider serviceProvider)
        {

            this.Initialize();
            this.LoadContent();
        }

        public void Initialize()
        {

        }

        public void LoadContent()
        {

        }
    }
}
