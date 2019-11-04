using CubesECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace CubesECS.Systems
{
    public class SpawnSystem : JobComponentSystem
    {
        #region Private Fields
        private EndSimulationEntityCommandBufferSystem m_bufferSystem;
        #endregion


        #region Structs
        public struct SpawnJob : IJobForEachWithEntity<SpawnData, LocalToWorld>
        {
            #region Private Fields
            private EntityCommandBuffer.Concurrent m_buffer;
            private Random m_random;
            private readonly float m_deltaTime;

            private const float MIN_SCALE = 0.005f;
            private const float MAX_SCALE = 0.25f;
            
            private const float MIN_SPEED = 0.75f;
            private const float MAX_SPEED = 1.25f;

            private const float MIN_FORCE = 0.25f;
            private const float MAX_FORCE = 1f;            
            #endregion


            #region Main Methods
            public SpawnJob(EntityCommandBuffer.Concurrent pCommandBuffer, Random pRandom, float pDeltaTime)
            {
                m_buffer = pCommandBuffer;
                m_random = pRandom;
                m_deltaTime = pDeltaTime;
            }

            public void Execute(Entity pEntity, int pIndex, ref SpawnData pSpawner, [ReadOnly] ref LocalToWorld pLocalWorld)
            {
                pSpawner.timeCounter -= m_deltaTime;
                
                if (pSpawner.timeCounter > 0f)
                    return;

                pSpawner.timeCounter = pSpawner.frequency;

                Entity _instance = m_buffer.Instantiate(pIndex, pSpawner.prefab);

                m_buffer.SetComponent(pIndex, _instance, new Translation {
                    Value = pLocalWorld.Position + new float3(m_random.NextFloat()*pSpawner.distance, 0f, 0f)
                });

                m_buffer.SetComponent(pIndex, _instance, new NonUniformScale {
                    Value = new float3(m_random.NextFloat(MIN_SCALE, MAX_SCALE),
                        m_random.NextFloat(MIN_SCALE, MAX_SCALE),
                        m_random.NextFloat(MIN_SCALE, MAX_SCALE))
                });

                m_buffer.AddComponent(pIndex, _instance, new Movement {
                    bouncing = m_random.NextInt(0, 3),
                    speed = m_random.NextFloat(MIN_SPEED, MAX_SPEED),
                    jumpForce = m_random.NextFloat(MIN_FORCE, MAX_FORCE)
                });
            }
            #endregion
        }
        #endregion


        #region Job Methods
        protected override void OnCreate()
        {
            m_bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle pInputDeps)
        {
            SpawnJob _job = new SpawnJob(
                m_bufferSystem.CreateCommandBuffer().ToConcurrent(),
                new Random((uint)UnityEngine.Random.Range(0, int.MaxValue)),
                UnityEngine.Time.deltaTime
            );

            JobHandle _jobHandle = _job.Schedule(this, pInputDeps);
            m_bufferSystem.AddJobHandleForProducer(_jobHandle);

            return _jobHandle;
        }
        #endregion 
    }
}