using Unity.Entities;

namespace Game.Components
{
    public class SpawnBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Spawner
            {
                UnitPrefab = GetEntity(authoring.UnitPrefab, TransformUsageFlags.Dynamic),
                Count = authoring.Count,
                SpawnPosition = authoring.transform.position,
                SpawnRequested = false
            });
        }
    }
}