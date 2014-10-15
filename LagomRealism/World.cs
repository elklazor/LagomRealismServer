using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Diagnostics;
namespace LagomRealism
{
    class World
    {

        private float[] heightMap;
        private GraphicsDevice Gd;
        private Texture2D worldTexture;
        private int seed;
        private int jump;
        private int maxChange;
        private int Density;
        public List<WorldEntity> entities = new List<WorldEntity>();
        public int MaxChange
        {
            get { return maxChange; }
            set { maxChange = value; }
        }
        

        public int Jump
        {
            get { return jump; }
            set { jump = value; }
        }
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
        /// <summary>
        /// Loads world with config file
        /// </summary>
        /// <param name="configName">Name of config file without extension</param>
        public void Load(string configName)
        {
            Console.WriteLine("Loading config...");
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("./WorldConfig/" + configName + ".xml");
            string[] size = (xDoc.SelectSingleNode("CONFIG//SIZE").InnerText).Split(':');
            imageSize = new Point(Convert.ToInt32(size[0]),Convert.ToInt32(size[1]));
            Seed = Convert.ToInt32(xDoc.SelectSingleNode("CONFIG//SEED").InnerText);
            maxChange = Convert.ToInt32(xDoc.SelectSingleNode("CONFIG//MAXCHANGE").InnerText);
            Jump = Convert.ToInt32(xDoc.SelectSingleNode("CONFIG//JUMP").InnerText);
            Density = Convert.ToInt32(xDoc.SelectSingleNode("CONFIG//DENSITY").InnerText);
            Console.WriteLine(@"/***** Config *****\");
            Console.WriteLine(WorldConfigToString());
            Console.WriteLine(@"\******************/");
            ServerGenerate();
        }
        public void StringLoad(string config,GraphicsDevice gd)
        {
            Gd = gd;
            string[] conf = config.Split('|');
            string[] sizArr = conf[0].Split(':');
            imageSize = new Point(Convert.ToInt32(sizArr[0]),Convert.ToInt32(sizArr[1]));
            seed = Convert.ToInt32(conf[1]);
            Jump = Convert.ToInt32(conf[2]);
            MaxChange = Convert.ToInt32(conf[3]);
            Debug.WriteLine(WorldConfigToString());
            Generate();

        }
        public void Load(int x, int y)
        {
            this.imageSize = new Point(x, y);
            ServerGenerate();
        }

        private void Generate()
        {
            // Array size: (screen width / jump) + 3
            pointArr = new Point[(imageSize.X / jump) +3];
            heightMap = TerrainManager.GenerateTerrain(ref pointArr, imageSize.Y / 2,Seed,jump,maxChange);
            worldTexture = TerrainManager.PolygonToTexture(Gd, pointArr, imageSize);
        }

        private void ServerGenerate()
        {
            pointArr = new Point[imageSize.X];
            heightMap = TerrainManager.GenerateTerrain(ref pointArr, imageSize.Y / 2,Seed,jump,maxChange);
            Random rnd = new Random(Seed);
            //Generate trees
            bool prevGen = false;
            int ID = 0;
            for (int i = 0; i < imageSize.X; i++)
            {
                if (rnd.Next(0, Density) > Density - 10 && !prevGen)
                {
                    //Add logic for different entities
                    entities.Add(new WorldEntity(ID++, i, heightMap[i], rnd.Next(1,3)));
                    
                    prevGen = true;
                }
                else
                    prevGen = false;

                if (i + 50 <= imageSize.X)
                    i += 50;
                else
                    i += imageSize.X - i; 
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(worldTexture, Vector2.Zero, Color.White);
            
        }

        public string WorldConfigToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(imageSize.X + ":" + imageSize.Y);
            sb.Append("|");
            sb.Append(seed.ToString());
            sb.Append("|");
            sb.Append(jump.ToString());
            sb.Append("|");
            sb.Append(maxChange.ToString());
            return sb.ToString();
        }
    }
}
