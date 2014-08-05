using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LagomRealism
{
    class World:GameComponent
    {

        private float[] heightMap;
        private GraphicsDevice Gd;
        private Texture2D worldTexture;
        private int seed;

        public int Seed
        {
            get { return seed; }
            set { seed = value; }
        }


        public Texture2D WorldTexture
        {
            get { return worldTexture; }
            set { worldTexture = value; }
        }
        private Point[] pointArr;
        private Point imageSize;

        public float[] HeightMap
        {
            get { return heightMap; }
            set { heightMap = value; }
        }

        public World(int seed)
        {
            Seed = seed;
        }
        public World()
        {
            Random rnd = new Random();
            Seed = rnd.Next(100, 90000);
        }

        public void Load(Point imageSize,GraphicsDevice gd)
        {
            this.imageSize = imageSize;
            Gd = gd;
            Generate();
        }

        public void Load(int x, int y)
        {
            this.imageSize = new Point(x, y);
            ServerGenerate();
        }

        private void Generate()
        {
            // Array size: screen width / jump(50) + 3
            pointArr = new Point[(imageSize.X / 50) +3];
            heightMap = TerrainManager.GenerateTerrain(ref pointArr, imageSize.Y / 2,Seed,50);
            worldTexture = TerrainManager.PolygonToTexture(Gd, pointArr, imageSize);
        }

        private void ServerGenerate()
        {
            pointArr = new Point[imageSize.X];
            heightMap = TerrainManager.GenerateTerrain(ref pointArr, imageSize.Y / 2,Seed,50);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(worldTexture, Vector2.Zero, Color.White);
            base.Draw(spriteBatch);
        }
    }
}
