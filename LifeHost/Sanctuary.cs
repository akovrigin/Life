using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeHost
{
    public static class Sanctuary
    {
        public struct Population
        {
            public int[,] Cells;
            public Player Player;
            public int OffsetX;
            public int OffsetY;
        }

        private static readonly List<Population> Populations = new List<Population>();

        private static object _lockObject = new object();

        public static List<Population> Get()
        {
            lock (_lockObject)
                return Populations.ToList();
        }

        public static void Clear()
        {
            lock (_lockObject)
                Populations.Clear();
        }

        public static void Populate(PresetType preset, Player player, int offsetX, int offsetY) //TODO: Вынести в отдельный класс, может быть в экстеншнсы
        {
            int[,] cells;

            switch (preset)
            {
                case PresetType.Tub:
                    cells = new[,] { { 1, 0 }, { 0, 1 }, { 2, 1 }, { 1, 2 } };
                    break;
                case PresetType.Blinker:
                    cells = new[,] { { 1, 0 }, { 1, 1 }, { 1, 2 } };
                    break;
                case PresetType.Beacon:
                    cells = new[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 }, { 2, 2 }, { 3, 2 }, { 3, 3 }, { 2, 3 } };
                    break;
                case PresetType.Glider:
                    cells = new[,] { { 1, 0 }, { 2, 1 }, { 2, 2 }, { 0, 2 }, { 1, 2 }, { 2, 2 } };
                    break;
                case PresetType.SuperTub:
                    cells = new[,] { { 1, 0 }, { 0, 1 }, { 2, 1 }, { 1, 2 }, { 3, 0 }, { 4, 1 }, { 3, 2 } };
                    break;
                case PresetType.Pentadecathlon:
                    cells = new[,]
                    {
                        { 1, 0 }, { 1, 1 }, { 0, 2 }, { 2, 2 }, { 1, 3 }, { 1, 4 },
                        { 1, 5 }, { 1, 6 }, { 0, 7 }, { 2, 7 }, { 1, 8 }, { 1, 9 }
                    };
                    break;
                default:
                    cells = new[,] { { 0, 0 } };
                    break;
            }

            lock (_lockObject)
                Populations.Add(new Population{Cells = cells, Player = player, OffsetX = offsetX, OffsetY = offsetY});
        }

        public static void Populate(PresetType preset, Player player)
        {
            Populate(preset, player, 0, 0);
        }
    }
}
