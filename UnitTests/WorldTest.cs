using System;
using System.Collections.Generic;
using System.Linq;
using LifeHost;
using Xunit;

namespace UnitTests
{
    public class WorldTest
    {
        public static IEnumerable<object[]> GetCreatures()
        {
            yield return new object[] {new[,] {{1, 1}, {1, 1}}, 1, 0};

            yield return new object[] {new[,] {{1, 1}, {1, 2}, {2, 1}}, 3, 2};

            yield return new object[] {new[,] {{1, 1}, {1, 2}, {2, 1}, { 2, 2 }, { 0, 0 } }, 5, 4};
        }

        [Theory]
        [MemberData(nameof(GetCreatures))]
        public void BlankWorld_CreateCreatures_CheckCreatureCount(int[,] place, int count, int neighborsCount)
        {
            var world = World.Instance;

            world.Clear();

            for (int i = 0; i <= place.GetUpperBound(0); i++)
                world.AddCreature(place[i, 0], place[i, 1], new Player());

            Assert.True(world.CreatureCount == count, "Wrong count of creatures in the world");

            var creature = new Creature{X = place[0, 0], Y = place[0, 1] };

            Assert.True(world.NeighborsCount(creature) == neighborsCount, "Wrong count neighbors of the creature");
        }

        public static IEnumerable<object[]> GetPresetData()
        {
            yield return new object[] { PresetType.Blinker, new[,] { { 1, 0 }, { 1, 2 } }};
            yield return new object[] { PresetType.Beacon, new[,] { { 1, 1 }, { 2, 2 } }};
        }

        [Theory]
        [MemberData(nameof(GetPresetData))]
        public void CreateBlink_ProcessTwoSteps_CheckBlink(PresetType preset, int[,] data)
        {
            var world = World.Instance;

            world.Clear();

            Sanctuary.Populate(preset, new Player());

            for (int i = 0; i < 10; i++)
            {
                world.Step();

                var c1 = world.Get(data[0, 0], data[0, 1]);
                var c2 = world.Get(data[1, 0], data[1, 1]);

                Assert.True(i % 2 != 0 || c1 == null && c2 == null, $"Blinker doesn't blink after the step i = {i}");
                Assert.True(i % 2 == 0 || c1 != null && c2 != null, $"Blinker doesn't blink after the step i = {i}");
            }
        }
    }
}
