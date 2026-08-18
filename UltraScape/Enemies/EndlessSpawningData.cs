using UnityEngine;

namespace UltraScape.Enemies
{
    /// <summary>
    /// param1 = startingWave. param2 = maxStack.
    /// </summary>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    public readonly struct EndlessSpawningData((int, int, int) param1, (int, int, int) param2)
    {
        public readonly (int casual, int standard, int extreme) startingWave = param1;

        public readonly (int casual, int standard, int extreme) maxStack = param2;
    }
}