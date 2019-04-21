using UnityEngine;
using System;
using Unity.Entities;
using Unity.Transforms;

namespace CubesECS.Pro
{
    [Serializable]
    public struct WaveJump : IComponentData
    {
        public bool Enabled;
        public float ScaleY;
    }

    public class WaveJumpComponent : ComponentDataProxy<WaveJump>
    {}
}