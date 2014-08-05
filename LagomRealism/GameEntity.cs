using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    abstract class GameEntity
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private EntityType type;
        private EntityState state;

        internal EntityState State
        {
            get { return state; }
            set { state = value; }
        }

    }
}
