using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using CubesDOTS.Components;
using CubesDOTS.Behaviours;
using Unity.Mathematics;

namespace CubesDOTS.Systems
{
    public class MovementSystem : JobComponentSystem
    {   
        #region Structs
        [BurstCompile]
        public struct MovementJob : IJobForEach<Translation, Movement>
        {
            public float deltaTime;
            public float heightFactor;
            public float maxHeight;
            public float waveIntensity;

            private const float MAX_LENGTH = 25f;

            public void Execute(ref Translation pPosition, ref Movement pMovement)
            {
                pPosition.Value.z += pMovement.speed*deltaTime;
                
                if (pPosition.Value.z > MAX_LENGTH)
                    pPosition.Value.z = 1f;
                
                if (pMovement.bouncing == 1)
                    pPosition.Value.y = math.lerp(pPosition.Value.y, heightFactor*maxHeight*pMovement.jumpForce, deltaTime*waveIntensity);
            }
        }
        #endregion


        #region Job Methods
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new MovementJob {
                deltaTime = Time.deltaTime,
                heightFactor = AudioWaveProvider.Instance.CurrentWave,
                maxHeight = AudioWaveProvider.Instance.MaxHeight,
                waveIntensity = AudioWaveProvider.Instance.WaveIntensity
            };
            
            return job.Schedule(this, inputDeps);
        }
        #endregion
    }
}