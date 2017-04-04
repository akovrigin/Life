using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeHost
{
    public class Creature
    {
        public int X;
        public int Y;

        public bool IsAlive = false;

        public Player Owner;

        public Color Color;

        public Creature()
        {
        }

        public Creature(Player owner, int x, int y)
        {
            Owner = owner;

            if (owner != null)
                Color = owner.Color;

            X = x;

            Y = y;
        }
    }
}
