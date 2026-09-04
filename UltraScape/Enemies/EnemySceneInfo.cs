using System.Collections.Generic;
using UnityEngine;

namespace UltraScape.Enemies
{
    public class EnemySceneInfo
    {
        public List<GameObject> activeInstances = [];

        internal int spawnBuffer = 0;
    }
}