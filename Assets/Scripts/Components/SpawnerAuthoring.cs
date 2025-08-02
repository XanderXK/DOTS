using UnityEngine;

namespace Game.Components
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        [field: SerializeField] public GameObject UnitPrefab { get; set; }
        [field: SerializeField] public int Count { get; set; } = 10;
    }
}