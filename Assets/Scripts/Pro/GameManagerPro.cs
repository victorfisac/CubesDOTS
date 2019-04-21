using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

namespace CubesECS.Pro
{
    public class GameManagerPro : MonoBehaviour
    {
        #region Static Fields
        private static GameManagerPro m_instance = null;
        #endregion


        #region Inspector Fields
        [Header("Test")]
        [SerializeField]
        private int m_pawnCount;
        [SerializeField]
        private int m_pawnIncrement;
        [SerializeField]
        private float m_autoIncrementSeconds;

        [Header("Pawn")]
        [SerializeField]
        private GameObject m_pawnPrefab;
        [SerializeField]
        private Material[] m_materials;
        [SerializeField]
        private Vector2 m_pawnSpeed;
        [SerializeField]
        private Vector2 m_pawnScale;

        [Header("Limits")]
        [SerializeField]
        private float m_topBound;
        [SerializeField]
        private float m_bottomBound;
        [SerializeField]
        private float m_leftBound;
        [SerializeField]
        private float m_rightBound;

        [Header("Audio")]
        [SerializeField]
        private AudioSource m_audioSource;
        [SerializeField]
        private AudioClip[] m_audioClips;
        [SerializeField]
        private float m_waveScale;
        [SerializeField]
        private float m_musicWaveIntensity;
        [SerializeField]
        private float m_calmDuration;

        [Header("UI")]
        [SerializeField]
        private Text m_objectsCountTxt;

        [Header("FX")]
        [SerializeField]
        private ReflectionProbe m_reflections;
        [SerializeField]
        private PostProcessingBehaviour m_effects;
        #endregion

        
        #region Private Fields
        private EntityManager m_manager;
        private float m_waveValue;
        private float[] m_samples;
        private float m_timeCounter;
        private int m_instancesCount;
        private int m_currentClip;

        private const int SAMPLE_SIZE = 1024;
        private const string COUNT_FORMAT = "ENTITIES: {0}";
        #endregion


        #region Main Methods
        private void Awake()
        {
            m_instance = this;
        }

        private void Start()
        {
            m_samples = new float[SAMPLE_SIZE];
            m_manager = World.Active.EntityManager;
        }

        private void Update()
        {
            CheckInputs();
            AutoIncrementEntities();
            UpdateAudioData();
        }
        #endregion


        #region Private Fields
        private void UpdateAudioData()
        {
            m_audioSource.GetOutputData(m_samples, 0);

            float _wave = 0f;
            for (int i = 0; i < SAMPLE_SIZE; i++)
                _wave += m_samples[i]*m_samples[i];

            float _newValue = Mathf.Sqrt(_wave/SAMPLE_SIZE)*m_waveScale;
            m_waveValue = Mathf.Lerp(m_waveValue, _newValue, Time.deltaTime*m_calmDuration);
        }

        private void AddPawns(int count)
        {
            NativeArray<Entity> entities = new NativeArray<Entity>(count, Allocator.Temp);
            m_manager.Instantiate(m_pawnPrefab, entities);

            for (int i = 0; i < count; i++)
                SetUpEntity(i, entities[i]);

            entities.Dispose();
            m_pawnCount -= count;
            m_instancesCount += count;

            m_objectsCountTxt.text = string.Format(COUNT_FORMAT, m_instancesCount);
        }

        private void NextSong()
        {
            m_currentClip++;

            if (m_currentClip >= m_audioClips.Length)
                m_currentClip = 0;

            m_audioSource.Stop();
            m_audioSource.clip = m_audioClips[m_currentClip];
            m_audioSource.Play();
        }

        private void AutoIncrementEntities()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                AddPawns(m_pawnIncrement);

            if (m_autoIncrementSeconds > 0f && (m_pawnCount >= 0))
            {
                m_timeCounter += Time.deltaTime;

                if (m_timeCounter >= m_autoIncrementSeconds)
                {
                    m_timeCounter = 0f;
                    AddPawns(m_pawnIncrement);
                }
            }
        }

        #if UNITY_ANDROID || UNITY_IOS
        private void CheckInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!m_effects.enabled)
                {
                    QualitySettings.SetQualityLevel(5);
                    m_effects.enabled = true;
                    m_reflections.enabled = true;
                }
                else
                {
                    QualitySettings.SetQualityLevel(0);
                    m_effects.enabled = false;
                    m_reflections.enabled = false;
                }
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                NextSong();
        }
        #else
        private void CheckInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();

            if (Input.GetKeyDown(KeyCode.Space))
                NextSong();

            if (Input.GetKeyDown(KeyCode.F1))
                QualitySettings.DecreaseLevel();
            else if (Input.GetKeyDown(KeyCode.F2))
                QualitySettings.IncreaseLevel();
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                m_effects.enabled = !m_effects.enabled;
                m_reflections.enabled = !m_reflections.enabled;
            }
        }
        #endif
        #endregion


        #region Helper Methods
        private void SetUpEntity(int pIndex, Entity pEntity)
        {
            float xVal = UnityEngine.Random.Range(m_leftBound, m_rightBound);
            float zVal = UnityEngine.Random.Range(m_topBound, m_bottomBound);

            Vector3 scale = new Vector3(UnityEngine.Random.Range(0.25f, 1f), UnityEngine.Random.Range(0.25f, 1f), UnityEngine.Random.Range(0.25f, 1f))*UnityEngine.Random.Range(m_pawnScale.x, m_pawnScale.y);
            Vector3 pos = new Vector3(xVal, scale.y/2f, zVal + m_topBound);
            Quaternion rot = Quaternion.identity;

            m_manager.SetComponentData(pEntity, new Translation { Value = new float3(pos.x, pos.y, pos.z) });
            m_manager.SetComponentData(pEntity, new Rotation { Value = new quaternion(rot.x, rot.y, rot.z, rot.w) });
            m_manager.SetComponentData(pEntity, new NonUniformScale { Value = new float3(scale.x, scale.y, scale.z) });
            m_manager.SetComponentData(pEntity, new MoveSpeed { Value = UnityEngine.Random.Range(m_pawnSpeed.x, m_pawnSpeed.y) });
            m_manager.SetComponentData(pEntity, new WaveJump { Enabled = ((pIndex % 2) == 0), ScaleY = scale.y });
            
            RenderMesh _renderMesh = m_manager.GetSharedComponentData<RenderMesh>(pEntity);
            _renderMesh.material = m_materials[UnityEngine.Random.Range(0, m_materials.Length)];
            m_manager.SetSharedComponentData(pEntity, _renderMesh);
        }
        #endregion


        #region Properties
        public static GameManagerPro Instance
        {
            get { return m_instance; }
            set { m_instance = value; }
        }

        public float TopBound
        {
            get { return m_topBound; }
            set { m_topBound = value; }
        }

        public float BottomBound
        {
            get { return m_bottomBound; }
            set { m_bottomBound = value; }
        }

        public float WaveAmount
        {
            get { return m_waveValue; }
            set { m_waveValue = value; }
        }

        public float WaveIntensity
        {
            get { return m_musicWaveIntensity; }
            set { m_musicWaveIntensity = value; }
        }
        #endregion
    }
}