using UnityEngine;

namespace Game.Components
{
    public class UnitMoveAuthoring : MonoBehaviour
    {
        [field: SerializeField] public float MoveSpeed { get; set; }
        [field: SerializeField] public float RotationSpeed { get; set; }
    }
}