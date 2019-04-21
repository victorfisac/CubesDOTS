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
            private float m_topBound;
            private float m_bottomBound;
            private float m_deltaTime;
            private float m_waveAmount;
            private float m_waveIntensity;

            public void Execute(ref Translation position, ref Rotation rotation, ref NonUniformScale scale, [ReadOnly] ref MoveSpeed speed, [ReadOnly] ref WaveJump wave)
            {
                float3 _position = position.Value;
                _position += new float3(0f, 0f, 1f)*speed.Value*m_deltaTime;

                if (_position.z > m_bottomBound)
                    _position.z = m_topBound;

                if (wave.Enabled)
                {
                    _position.y = scale.Value.y/2 + m_waveAmount;
                    _position.y = Mathf.Lerp(position.Value.y, _position.y, m_deltaTime*m_waveIntensity);

                    float3 _scale = scale.Value;
                    _scale.y = wave.ScaleY + _position.y/2.5f*m_waveAmount;
                    scale.Value = _scale;
                }

                position.Value = _position;
            }

            public float topBound
            {
                get { return m_topBound; }
                set { m_topBound = value; }
            }

            public float bottomBound
            {
                get { return m_bottomBound; }
                set { m_bottomBound = value; }
            }

            public float deltaTime
            {
                get { return m_deltaTime; }
                set { m_deltaTime = value; }
            }

            public float waveAmount
            {
                get { return m_waveAmount; }
                set { m_waveAmount = value; }
            }

            public float waveIntensity
            {
                get { return m_waveIntensity; }
                set { m_waveIntensity = value; }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            MovementJob moveJob = new MovementJob {
                topBound = GameManagerPro.Instance.TopBound,
                bottomBound = GameManagerPro.Instance.BottomBound,
                deltaTime = Time.deltaTime,
                waveAmount = GameManagerPro.Instance.WaveAmount,
                waveIntensity = GameManagerPro.Instance.WaveIntensity
            };

            JobHandle moveHandle = moveJob.ScheduleSingle(this, inputDeps);

            return moveHandle;
        }
    }
}