using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heeeeeexx
{
    class Ball
    {

        public Texture2D texture;
        public Rectangle location;
        public Color tint;
        public bool alive;
        public int indice1;
        public int indice2;

        public Rectangle Location
        {
            get { return location; }
        }

        public Ball(int Indice1, int Indice2, Texture2D texture, Rectangle location, Color tint)
        {
            this.texture = texture;
            this.location = location;
            this.tint = tint;
            this.alive = false;
            this.indice1 = Indice1;
            this.indice2 = Indice2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, location, tint);
        }

    }
}
