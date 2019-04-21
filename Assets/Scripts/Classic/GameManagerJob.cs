using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace CubesECS.Classic
{
    public class GameManagerJob : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Test")]
        [SerializeField]
        private int pawnCount;
        [SerializeField]
        private int pawnIncrement;

        [Header("Pawn")]
        [SerializeField]
        private GameObject[] pawnPrefabs;
        [SerializeField]
        private Vector2 pawnSpeed;
        [SerializeField]
        private Vector2 pawnScale;

        [Header("Limits")]
        [SerializeField]
        private float topBound;
        [SerializeField]
        private float bottomBound;
        [SerializeField]
        private float leftBound;
        [SerializeField]
        private float rightBound;

        [Header("Audio")]
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField, Range(0f, 5f)]
        private float beatThreshold;
        [SerializeField, Range(0f, 10f)]
        private float waveScale;
        [SerializeField, Range(0f, 50f)]
        private float m_musicWaveIntensity;
        [SerializeField, Range(0f, 10f)]
        private float calmDuration;
        #endregion


        #region Private Fields
        TransformAccessArray transforms;
        MovementJob moveJob;
        JobHandle moveHandle;

        private float m_waveValue;
        private float[] m_samples;

        private const int SAMPLE_SIZE = 1024;
        #endregion


        #region Main Methods
        private void Start()
        {
            m_samples = new float[SAMPLE_SIZE];   
            transforms = new TransformAccessArray(0, -1);

            AddPawns(pawnCount);
        }

        private void OnDisable()
        {
            moveHandle.Complete();
            transforms.Dispose();
        }

        private void Update()
        {
            moveHandle.Complete();

            if (Input.GetKeyDown(KeyCode.Space))
                AddPawns(pawnIncrement);

            UpdateAudioData();

            moveJob = new MovementJob() {
                moveSpeed = Random.Range(pawnSpeed.x, pawnSpeed.y),
                topBound = topBound,
                bottomBound = bottomBound,
                deltaTime = Time.deltaTime,
                musicWave = m_waveValue,
                musicWaveIntensity = m_musicWaveIntensity
            };

            moveHandle = moveJob.Schedule(transforms);
            JobHandle.ScheduleBatchedJobs();
        }
        #endregion


        #region Private Methods
        private void UpdateAudioData()
        {
            audioSource.GetOutputData(m_samples, 0);

            float _wave = 0f;
            for (int i = 0; i < SAMPLE_SIZE; i++)
                _wave += m_samples[i]*m_samples[i];

            m_waveValue = Mathf.Sqrt(_wave/SAMPLE_SIZE)*waveScale;

            if (m_waveValue < beatThreshold)
                m_waveValue = Mathf.Lerp(m_waveValue, 0f, Time.deltaTime*calmDuration);
        }

        private void AddPawns(int count)
        {
            moveHandle.Complete();
            transforms.capacity = transforms.length + count;

            for (int i = 0; i < count; i++)
            {
                float xVal = Random.Range(leftBound, rightBound);
                float zVal = Random.Range(topBound, bottomBound);

                Vector3 scale = new Vector3(Random.Range(0.25f, 1f), Random.Range(0.25f, 1f), Random.Range(0.25f, 1f))*Random.Range(pawnScale.x, pawnScale.y);
                Vector3 pos = new Vector3(xVal, scale.y/2f, zVal + topBound);
                Quaternion rot = Quaternion.identity;

                GameObject obj = Instantiate(pawnPrefabs[Random.Range(0, pawnPrefabs.Length)], pos, rot) as GameObject;
                obj.transform.localScale = scale;

                transforms.Add(obj.transform);
            }

            pawnCount += count;
        }
        #endregion
    }
}