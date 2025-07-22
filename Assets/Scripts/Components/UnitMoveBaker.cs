using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class UnitMoveBaker : Baker<UnitMoveAuthoring>
    {
        public override void Bake(UnitMoveAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMove
            {
                MoveSpeed = authoring.MoveSpeed,
                RotationSpeed = authoring.RotationSpeed
            });
        }
    }
}
