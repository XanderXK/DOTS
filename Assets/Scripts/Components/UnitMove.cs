
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public struct UnitMove : IComponentData
    {
        public float MoveSpeed;
        public float RotationSpeed;
        public float3 TargetPosition;
    }
}
