using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PrimitivesSample;

namespace comp4900_xtoon
{
    class Light
    {
        private const int LIGHT_SIZE = 80;

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

        public void Draw3D(PrimitiveBatch primitiveBatch, Matrix projection, Vector3 where)
        {
            // the sun is made from 4 lines in a circle.
            primitiveBatch.Begin(PrimitiveType.LineList, projection);

            // draw the vertical and horizontal lines
            primitiveBatch.AddVertex(where + new Vector3(0, LIGHT_SIZE, 0), Color.Black);
            primitiveBatch.AddVertex(where + new Vector3(0, -LIGHT_SIZE, 0), Color.White);

            primitiveBatch.AddVertex(where + new Vector3(LIGHT_SIZE, 0, 0), Color.White);
            primitiveBatch.AddVertex(where + new Vector3(-LIGHT_SIZE, 0, 0), Color.White);

            // to know where to draw the diagonal lines, we need to use trig.
            // cosine of pi / 4 tells us what the x coordinate of a circle's radius is
            // at 45 degrees. the y coordinate normally would come from sin, but sin and
            // cos 45 are the same, so we can reuse cos for both x and y.
            float sunSizeDiagonal = (float)Math.Cos(MathHelper.PiOver4);

            // since that trig tells us the x and y for a unit circle, which has a
            // radius of 1, we need scale that result by the sun's radius.
            sunSizeDiagonal *= LIGHT_SIZE;

            primitiveBatch.AddVertex(
                where + new Vector3(-sunSizeDiagonal, sunSizeDiagonal, 0), Color.Gray);
            primitiveBatch.AddVertex(
                where + new Vector3(sunSizeDiagonal, -sunSizeDiagonal, 0), Color.Gray);

            primitiveBatch.AddVertex(
                where + new Vector3(sunSizeDiagonal, sunSizeDiagonal, 0), Color.Gray);
            primitiveBatch.AddVertex(
                where + new Vector3(-sunSizeDiagonal, -sunSizeDiagonal, 0), Color.Gray);

            primitiveBatch.End();
        }
    }
}
