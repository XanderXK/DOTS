using Game.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Game.Systems
{
    [BurstCompile]
    public partial struct UnitMoveJob: IJobEntity
    {
        public float DeltaTime;
        public void Execute(ref LocalTransform localTransform, in UnitMove unitMove, ref PhysicsVelocity physicsVelocity)
        {
            var targetPosition = unitMove.TargetPosition;
            targetPosition.y = localTransform.Position.y;
            physicsVelocity.Angular = float3.zero;
            if (math.distance(localTransform.Position, targetPosition) < 1f)
            {
                physicsVelocity.Linear = float3.zero;
                return;
            }
            
            var moveDirection = targetPosition - localTransform.Position;
            moveDirection.y = 0;
            moveDirection = math.normalize(moveDirection);
            var rotation = math.slerp(
                localTransform.Rotation,
                quaternion.LookRotation(moveDirection, math.up()),
                unitMove.RotationSpeed * DeltaTime);
            localTransform.Rotation = rotation;
            physicsVelocity.Linear = moveDirection * unitMove.MoveSpeed;
        }
    }
}