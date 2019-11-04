using Unity.Entities;


namespace CubesDOTS.Components
{
    public struct Movement : IComponentData
    {
        public float speed;
        public int bouncing;
        public float jumpForce;
    }
}