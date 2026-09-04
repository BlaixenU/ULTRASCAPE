using UnityEngine;

namespace UltraScape.Enemies
{
    public class Operator : Enemy
    {
        public OperatorState State { get; private set; }

        public enum OperatorState
        {
            green,
            yellow,
            red
        }
    }
}