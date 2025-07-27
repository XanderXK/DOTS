using Unity.Entities;

namespace Game.Components
{
    public class UnitSelectBaker : Baker<UnitSelectAuthoring>
    {
        public override void Bake(UnitSelectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitSelect
            {
                VisualEntity = GetEntity(authoring.VisualElement, TransformUsageFlags.Dynamic)
            });
        }
    }
}