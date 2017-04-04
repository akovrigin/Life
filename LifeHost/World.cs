using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LifeHost
{
    public enum PresetType
    {
        Cell = 0,
        Tub = 1,
        Blinker = 2,
        Beacon = 3,
        Glider = 4,
        SuperTub = 5,
        Pentadecathlon = 6
    }

    public class World
    {
        private static World _world;

        private World()
        {
        }

        public static readonly World Instance = _world ?? (_world = new World());

        private int _stepCount;

        public int StepCount { get { return _stepCount; } }

        private readonly object _lockObject = new object();

        private bool _isInIteration;

        private const int Revival = 3;
        private const int Enough = 2;

        readonly Random _random = new Random(DateTime.Now.Millisecond);

        private readonly List<Creature> _creatures = new List<Creature>();

        public List<Creature> GetCreatures()
        {
             return _creatures.ToList();
        }

        public int CreatureCount => _creatures.Count;

        public void AddCreature(int x, int y, Player owner)
        {
            lock (_lockObject)
            {
                if (_creatures.Exists(c => c.X == x && c.Y == y))
                    return;

                _creatures.Add(new Creature(owner, x, y));
            }
        }

        public Creature Get(int x, int y)
        {
            lock (_lockObject)
                return _creatures.FirstOrDefault(c => c.X == x && c.Y == y);
        }

        public void Clear()
        {
            lock (_lockObject)
                _creatures.Clear();
        }

        public int NeighborsCount(Creature creature)
        {
            lock (_lockObject)
                return _creatures.Count(c => Math.Abs(c.X - creature.X) <= 1 && Math.Abs(c.Y - creature.Y) <= 1) - 1;
        }

        public List<Creature> GetNeighbors(int x, int y)
        {
            lock (_lockObject)
                return _creatures.Where(c => Math.Abs(c.X - x) <= 1 && Math.Abs(c.Y - y) <= 1).ToList();
        }

        public void Step()
        {
            _isInIteration = true;

            try
            {
                lock (_lockObject)
                {
                    //TryToPopulate();

                    var nextGeneration = new List<Creature>();

                    // Searching for adjacent cells 

                    foreach (var creature in _creatures)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                var x = creature.X + i;
                                var y = creature.Y + j;

                                if (x == -1 || y == -1)
                                    continue;

                                if (!_creatures.Exists(c => c.X == x && c.Y == y))
                                {
                                    if (!nextGeneration.Exists(c => c.X == x && c.Y == y))
                                        nextGeneration.Add(new Creature(null, x, y));
                                }
                            }
                        }
                    }

                    // Check if somebody will revive

                    nextGeneration.ForEach(c =>
                    {
                        var neighbors = GetNeighbors(c.X, c.Y);

                        if (neighbors.Count == Revival)
                        {
                            c.IsAlive = true;

                            // Calc among players who is the winner

                            var groups = neighbors.GroupBy(n => n.Owner.Id, (key, grp) => new {Id = key, Count = grp.Count()}).ToList();

                            if (groups.Count == Revival)
                            {
                                c.Owner = neighbors[_random.Next(Revival)].Owner;
                            }
                            else
                            {
                                var max = groups.Max(grp => grp.Count);
                                c.Owner = Player.Get(groups.First(grp => grp.Count == max).Id);
                            }

                            // Calc color of revival cell

                            int r = 0, g = 0, b = 0;

                            foreach (var n in neighbors)
                            {
                                r += n.Color.R;
                                g += n.Color.G;
                                b += n.Color.B;
                            }

                            c.Color = Color.FromArgb(r / Revival, g / Revival, b / Revival);
                        }
                    });

                    // Remove unavailing cells

                    nextGeneration.RemoveAll(c => !c.IsAlive);

                    // Make them dead

                    _creatures.ForEach(c =>
                    {
                        var count = NeighborsCount(c);
                        c.IsAlive = count == Enough || count == Revival;
                    });

                    _creatures.RemoveAll(c => !c.IsAlive);

                    // Merge with revival cells

                    _creatures.AddRange(nextGeneration);

                    TryToPopulate();

                    // Who is the most powerful player? We have to know!

                    Player.Get().ForEach(p => p.Power = 0);

                    _creatures.GroupBy(c => c.Owner).ToList().ForEach(grp => grp.Key.Power = grp.Count());

                    _stepCount++;
                }
            }
            finally
            {
                _isInIteration = false;
            }
        }

        public void TryToPopulate()
        {
            if (_isInIteration)
                return;

            lock (_lockObject)
            {
                foreach (var pattern in Sanctuary.Get())
                {
                    for (int i = 0; i <= pattern.Cells.GetUpperBound(0); i++)
                        AddCreature(
                            //TODO: Maybe I have to allow negative offset too
                            pattern.Cells[i, 0] + Math.Abs(pattern.OffsetX),
                            pattern.Cells[i, 1] + Math.Abs(pattern.OffsetY),
                            pattern.Player);
                }
            }

            Sanctuary.Clear();
        }
    }
}