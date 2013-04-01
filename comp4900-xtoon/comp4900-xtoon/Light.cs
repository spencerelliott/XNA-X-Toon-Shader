using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace comp4900_xtoon
{
    class Light
    {

        public Vector3 Position { get; set; }
        public float Intensity { get; set; }

        public Light() { }
        public Light(Vector3 position) { Position = position; }
        public Light(Vector3 position, float intensity) { Position = position; Intensity = intensity; }

        public void Draw(Texture2D texture, SpriteBatch device)
        {
            Rectangle dstRect = new Rectangle((int)(Position.X), (int)(Position.Y), 20, 20);
            device.Draw(texture, dstRect, Color.Black);
        }
    }
}
