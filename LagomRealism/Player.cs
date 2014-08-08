using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    class Player
    {
        private Vector2 pos;
        private Vector2 velocity = Vector2.Zero;
        public float[] heightMap;
        private bool canJump = true;
        private Texture2D texture;
        public int ID;
        public bool NeedUpdate = false;
        private Vector2 prevPos = Vector2.Zero;
        public Vector2 Position
        {
            get { return pos; }
            set { pos = value; }
        }
        
        public Player(float[] hm, int id)
        {
            texture = TextureManager.TextureCache["Texture"];
            heightMap = hm;
            ID = id;
            
        }

        public override void Update()
        {
            //Input
            

            if (velocity.Y > 0.5f)
                velocity.Y -= velocity.Y / 6;
            if (velocity.Y < 0.2f)
                velocity.Y = 0f;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity.X = -0.5f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity.X = 0.5f;
            }
            else
                velocity.X = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && canJump)
            {
                velocity.Y += -10f;
                canJump = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                canJump = true;   
            }
            float locX = Position.X + texture.Width / 2;
            Vector2 vec = new Vector2(locX, pos.Y + texture.Height);
            if (vec.Y >= heightMap[(int)Math.Floor(pos.X)])
            {
                pos.Y = heightMap[(int)Math.Floor(pos.X)] - texture.Height;
            }
            else
                velocity.Y = 0.5f;

            pos += velocity;
            if (pos != prevPos)
                NeedUpdate = true;

            prevPos = pos;
            
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, pos, Color.White);
            
        }
        
        
    }
}
