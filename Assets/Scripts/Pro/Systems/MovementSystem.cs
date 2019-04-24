using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

namespace CubesECS.Pro
{
    public class MovementSystem : JobComponentSystem
    {
        [BurstCompile]
        struct MovementJob : IJobForEach<Translation, Rotation, NonUniformScale, MoveSpeed, WaveJump>
        {
            public readonly float deltaTime;
            public readonly float2 bounds;
            public readonly float2 jump;


            public MovementJob(float pTop, float pBottom, float pDelta, float pAmount, float pIntensity)
            {
                bounds.x = pTop;
                bounds.y = pBottom;
                deltaTime = pDelta;
                jump.x = pAmount;
                jump.y = pIntensity;
            }

            public void Execute(ref Translation position, [ReadOnly] ref Rotation rotation, ref NonUniformScale scale, [ReadOnly] ref MoveSpeed speed, [ReadOnly] ref WaveJump wave)
            {
                float3 _position = position.Value;
                _position += new float3(0f, 0f, 1f)*speed.Value*deltaTime;

                if (_position.z > bounds.y)
                    _position.z = bounds.x;

                if (wave.Enabled)
                {
                    _position.y = scale.Value.y/2 + jump.x*wave.Scale;
                    _position.y = Mathf.Lerp(position.Value.y, _position.y, deltaTime*jump.y);

                    float3 _scale = scale.Value;
                    _scale.y = wave.ScaleY + _position.y/2.5f*jump.x*wave.Scale;
                    scale.Value = _scale;
                }

                position.Value = _position;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            MovementJob moveJob = new MovementJob(
                GameManagerPro.Instance.TopBound,
                GameManagerPro.Instance.BottomBound,
                Time.deltaTime,
                GameManagerPro.Instance.WaveAmount,
                GameManagerPro.Instance.WaveIntensity
            );

            JobHandle moveHandle = moveJob.Schedule(this, inputDeps);

            return moveHandle;
        }
    }
}