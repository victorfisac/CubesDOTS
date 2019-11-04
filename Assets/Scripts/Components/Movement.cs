using Unity.Entities;


namespace CubesECS.Components
{
    public struct Movement : IComponentData
    {
        public float speed;
        public int bouncing;
        public float jumpForce;
    }
}