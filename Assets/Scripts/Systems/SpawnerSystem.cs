using Game.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Systems
{
    public partial struct SpawnerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var spawner in SystemAPI.Query<RefRW<Spawner>>())
            {
                if (!spawner.ValueRO.SpawnRequested) continue;
                for (var i = 0; i < spawner.ValueRO.Count; i++)
                {
                    var entity = ecb.Instantiate(spawner.ValueRO.UnitPrefab);
                    var position = spawner.ValueRO.SpawnPosition
                                   + new float3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
                    ecb.SetComponent(entity, LocalTransform.FromPosition(position));
                    if (SystemAPI.HasComponent<UnitMove>(spawner.ValueRO.UnitPrefab))
                    {
                        ecb.SetComponent(entity, new UnitMove
                        {
                            TargetPosition = position,
                            MoveSpeed = SystemAPI.GetComponent<UnitMove>(spawner.ValueRO.UnitPrefab).MoveSpeed,
                            RotationSpeed = SystemAPI.GetComponent<UnitMove>(spawner.ValueRO.UnitPrefab).RotationSpeed
                        });
                    }
                }

                spawner.ValueRW.SpawnRequested = false;
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}