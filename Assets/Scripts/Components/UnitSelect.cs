using Unity.Entities;

namespace Game.Components
{
    public struct UnitSelect : IComponentData, IEnableableComponent
    {
        public Entity VisualEntity;
    }
 
}