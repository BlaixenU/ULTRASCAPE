using UnityEngine;

namespace UltraScape.Enemies
{
    public enum SpawnType
    {
        None, // just dont use the attribute?
        RaisedCenter,
        RandomTile,
    }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class SpawnInfoAttribute : System.Attribute
    {
        public SpawnType type = SpawnType.None;

        public SpawnInfoAttribute(SpawnType spawnType)
        {
            type = spawnType;
        }
    }
}