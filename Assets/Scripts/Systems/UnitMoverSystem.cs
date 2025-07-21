using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Game
{
    public partial struct UnitMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (localTransform, moveSpeed,physicsVelocity) 
                     in SystemAPI.Query<RefRW<LocalTransform>,RefRO <MoveSpeed>, RefRW<PhysicsVelocity>>())
            {
                var targetPosition = (float3)MouseInput.LastClickPosition;
                // Debug.Log(targetPosition);
                var moveDirection = targetPosition - localTransform.ValueRO.Position;
                moveDirection.y = 0;
                moveDirection = math.normalize(moveDirection);
                // localTransform.ValueRW.Position += moveDirection* moveSpeed.ValueRO.Value* SystemAPI.Time.DeltaTime;
                localTransform.ValueRW.Rotation = quaternion.LookRotation(moveDirection, math.up());
                physicsVelocity.ValueRW.Linear = moveDirection * moveSpeed.ValueRO.Value;
                
            }
        }
    }
}
