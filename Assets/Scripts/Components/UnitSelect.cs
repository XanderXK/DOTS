using Unity.Entities;

namespace Game.Components
{
    public struct UnitSelect : IComponentData, IEnableableComponent
    {
        public bool IsSelected;
        public Entity VisualEntity;
    }
}