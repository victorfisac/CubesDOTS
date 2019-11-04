using Unity.Entities;


namespace CubesDOTS.Components
{
    public struct SpawnData : IComponentData
    {
        public Entity prefab;
        public float distance;
        public float frequency;
        public float timeCounter;
    }
}