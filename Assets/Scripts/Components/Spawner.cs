using Unity.Entities;
using Unity.Mathematics;

namespace Game.Components
{
    public struct Spawner : IComponentData
    {
        public Entity UnitPrefab;
        public int Count;
        public float3 SpawnPosition;
        public bool SpawnRequested;
    }
}