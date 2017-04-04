using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeHost
{
    public class Player
    {
        public int Id;
        public Color Color;
        public int Power;

        private static readonly List<Player> Players = new List<Player>();

        readonly Random _random = new Random(DateTime.Now.Millisecond);

        public static Player Get(int id)
        {
            var player = Players.FirstOrDefault(p => p.Id == id);
            return player ?? new Player();
        }

        public static List<Player> Get()
        {
            return Players.ToList();
        }

        public Player()
        {
            Id = Players.Any() ? Players.Max(p => p.Id) + 1 : 0;
            Color = GetColor();
            Players.Add(this);
        }

        private Color GetColor()
        {
            var red = _random.Next(0, 255);
            var green = _random.Next(0, 255);
            var blue = _random.Next(0, 255);

            return Color.FromArgb(red, green, blue);
        }
    }
}
