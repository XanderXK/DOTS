using UnityEngine;

namespace Game
{
    public class UnitMoveAuthoring : MonoBehaviour
    {
        [field: SerializeField] public float MoveSpeed { get; set; }
        [field: SerializeField] public float RotationSpeed { get; set; }
    }
}