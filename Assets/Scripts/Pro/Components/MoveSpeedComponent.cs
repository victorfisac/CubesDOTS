using UnityEngine;
using System;
using Unity.Entities;
using Unity.Transforms;

namespace CubesECS.Pro
{
    [Serializable]
    public struct MoveSpeed : IComponentData
    {
        public float Value;
    }

    public class MoveSpeedComponent : ComponentDataProxy<MoveSpeed>
    {}
}