using UnityEngine;
using System.Collections.Generic;

namespace UltraScape.Enemies
{
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        private bool inEndless;

        public Modifiers modifiers = new();

        public readonly Dictionary<USEnemyType, EndlessSpawningData> endlessSpawningData = new()
        {
            [USEnemyType.baby] = new EndlessSpawningData((1, 1, 1), (4, 4, 8))
        };

        public Dictionary<USEnemyType, List<GameObject>> activeEnemies = new()
        {
            [USEnemyType.baby] = []
        };

        private void Awake()
        {
            if (SceneHelper.CurrentScene == "Endless")
            {
                inEndless = true;
            }
        }

        internal void WaveChange(int newWave)
        {

        }

        public void AddEnemy(GameObject enemy, USEnemyType type)
        {
            activeEnemies[type].Add(enemy);
        }

        public void RemoveEnemy(GameObject enemy, USEnemyType type)
        {
            for (int i = 0; i < activeEnemies[type].Count; i++)
            {
                if (activeEnemies[type][i] == enemy)
                {
                    activeEnemies[type].RemoveAt(i);
                    break;
                }
            }
        }
    }
}