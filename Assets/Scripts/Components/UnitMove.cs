using Unity.Entities;
using Unity.Mathematics;

namespace Game.Components
{
    public struct UnitMove : IComponentData
    {
        public float MoveSpeed;
        public float RotationSpeed;
        public float3 TargetPosition;
    }
}
