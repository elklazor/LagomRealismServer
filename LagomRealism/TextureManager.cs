using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace LagomRealism
{
    static class TextureManager
    {
        public static Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        public static void Load(ContentManager content)
        {
            TextureCache = content.LoadListContent<Texture2D>("Textures");
            
        }

    }
    
}
