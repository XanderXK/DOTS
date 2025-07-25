using Game.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Game.Systems
{
    public partial struct UnitMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitMoveJob = new UnitMoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            
            unitMoveJob.ScheduleParallel();
            /*
            foreach (var (localTransform, unitMove, physicsVelocity)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMove>, RefRW<PhysicsVelocity>>())
            {
                // var targetPosition = (float3)MouseInput.LastClickPosition;
                // Debug.Log(targetPosition);
                var targetPosition = unitMove.ValueRO.TargetPosition;
                targetPosition.y = localTransform.ValueRO.Position.y;
                if (math.distance(localTransform.ValueRO.Position, targetPosition) < 1f)
                {
                    physicsVelocity.ValueRW.Linear = float3.zero;
                    continue;
                }
                
                var moveDirection = targetPosition - localTransform.ValueRO.Position;
                moveDirection.y = 0;
                moveDirection = math.normalize(moveDirection);
                // localTransform.ValueRW.Position += moveDirection* moveSpeed.ValueRO.Value* SystemAPI.Time.DeltaTime;
                var rotation = math.slerp(
                    localTransform.ValueRO.Rotation,
                    quaternion.LookRotation(moveDirection, math.up()),
                    unitMove.ValueRO.RotationSpeed * SystemAPI.Time.DeltaTime);
                localTransform.ValueRW.Rotation = rotation;
                physicsVelocity.ValueRW.Linear = moveDirection * unitMove.ValueRO.MoveSpeed;
            }
            */
        }
    }
}