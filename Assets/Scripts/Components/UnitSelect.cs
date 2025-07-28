using Unity.Entities;
using Unity.Transforms;

namespace Game.Components
{
    public struct UnitSelect : IComponentData, IEnableableComponent
    {
        public bool IsSelected;
        public Entity VisualEntity;
    }
 
}