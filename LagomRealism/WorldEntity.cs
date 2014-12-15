using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    class WorldEntity
    {
        public int ID;
        public int X;
        public float Y;
        public int Type;
        public int state = 0;
        public bool NeedUpdate = false;
        public int State
        {
            get { return state; }
            set 
            {
                if (value != state)
                {
                    state = value;
                    NeedUpdate = true;
                }
            }
        }
        public WorldEntity(int Id, int x, float y, int type)
        {
            ID = Id;
            X = x;
            Y = y;
            Type = type;
        }
        public override string ToString()
        {
            
            return String.Format("ID: {0} | Type: {1} | State: {4}| Position: {2};{3}", new object[] { ID, Type, X, Y,State });
        }
    }
}
