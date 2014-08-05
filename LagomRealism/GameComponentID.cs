using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    /// <summary>
    /// Contains ENUMS used by LagomRealism
    /// </summary>
    enum GCID
    {
        World,Player
    }

    enum MessageType
    {
        WorldSeed = 0,EntityUpdate,ClientPosition,ClientDisconnecting,ServerClosing,ClientConnected
    }

    enum EntityType
    {
        Tree = 1, Rock  
    }

    enum EntityState
    {
        Fine = 1,Broken
    }
    public static class TextureContent
    {
        public static Dictionary<string, T> LoadListContent<T>(this ContentManager contentManager, string contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            Dictionary<String, T> result = new Dictionary<String, T>();

            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);


                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }
            return result;
        }

    }
}
