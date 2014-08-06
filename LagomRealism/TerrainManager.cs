using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace LagomRealism
{
    class TerrainManager
    {
        /// <summary>
        /// Generates a texture from a point array representing a polygon
        /// </summary>
        /// <param name="gd">GraphicsDevice</param>
        /// <param name="pA">Array to use for the generation</param>
        /// <param name="imageSize">Size of the window</param>
        /// <returns></returns>
        public static Texture2D PolygonToTexture(GraphicsDevice gd, Microsoft.Xna.Framework.Point[] pA, Microsoft.Xna.Framework.Point imageSize)
        {
            Bitmap bmp = new Bitmap(imageSize.X, imageSize.Y);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            System.Drawing.Point[] b = new System.Drawing.Point[pA.Length];
            for (int i = 0; i < pA.Length; i++)
            {
                b[i].X = pA[i].X;
                b[i].Y = pA[i].Y;
            }
            g.FillPolygon(Brushes.ForestGreen, b);
            using (MemoryStream s = new MemoryStream())
            {
                bmp.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                s.Seek(0, SeekOrigin.Begin);
                return Texture2D.FromStream(gd, s);
            }
        }
        /// <summary>
        /// Generates terrain and returns the heightmap of the terrain
        /// </summary>
        /// <param name="pa">The array to modify</param>
        /// <param name="startY"> The starting Y coordinate</param>
        /// <returns>Heigthmap to the generated terrain</returns>
        public static float[] GenerateTerrain(ref Microsoft.Xna.Framework.Point[] pa, int startY, int seed,int Jump,int change)
        {
            int maxChangeY = change;
            int jump = Jump;
            Random rnd = new Random(seed);

            int prevY = startY;
            for (int i = 0; i < pa.Length; i++)
            {

                if (i < pa.Length - 2)
                {
                    pa[i] = new Microsoft.Xna.Framework.Point(jump * i, prevY + rnd.Next(-maxChangeY, maxChangeY));
                    prevY = pa[i].Y;
                }
                else
                {
                    pa[i] = new Microsoft.Xna.Framework.Point(pa[i - 1].X, pa[i - 1].Y + 400);
                    pa[i + 1] = new Microsoft.Xna.Framework.Point(pa[i].X - ((pa.Length - 2) * 50), pa[i].Y);
                    break;
                }

            }
            float[] heightMap = GenerateHeightmap(pa);
           
            //Add asset generation
            
            return heightMap;
        }

        private static float[] GenerateHeightmap(Microsoft.Xna.Framework.Point[] currentMap)
        {
            int c = 0;
            int jump = currentMap[1].X;
            float[] heightMap = new float[jump * (currentMap.Length - 3)];
            List<Microsoft.Xna.Framework.Point> pList = currentMap.ToList<Microsoft.Xna.Framework.Point>();
            pList.RemoveAt(pList.Count - 1);
            pList.RemoveAt(pList.Count - 1);

            for (int i = 1; i < pList.Count; i++)
            {
                float prevY = pList[i - 1].Y;
                float curY = pList[i].Y;
                float yPerPix = (curY - prevY) / jump;

                for (int g = 0; g < jump; g++)
                {
                    heightMap[c] = (prevY + (yPerPix * g));
                    c++;
                }

            }
            return heightMap;
        }
    }
}

