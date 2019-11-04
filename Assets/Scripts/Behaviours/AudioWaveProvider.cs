using Unity.Collections;
using Unity.Entities;
using UnityEngine;


namespace CubesDOTS.Behaviours
{
    public interface IAudioWaveProvider
    {
        float CurrentWave { get; }
        float MaxHeight { get; }
        float WaveIntensity { get; }
    }


    public class AudioWaveProvider : MonoBehaviour, IAudioWaveProvider
    {
        #region Static Fields
        private static AudioWaveProvider m_instance = null;
        #endregion


        #region Inspector Fields
        [Header("Configuration")]
        [SerializeField]
        private float m_waveScale = 3.5f;
        [SerializeField]
        private float m_calmDuration = 20f;
        [SerializeField]
        private float m_maxHeight = 1f;
        [SerializeField]
        private float m_waveItensity = 20f;

        [Header("References")]
        [SerializeField]
        private AudioSource m_audioSource;
        #endregion


        #region Private Fields
        private float[] m_samples = null;
        private float m_waveValue = 0f;

        private const int SAMPLE_SIZE = 1024;
        #endregion


        #region Main Methods
        private void Awake()
        {
            m_instance = this;
            m_samples = new float[SAMPLE_SIZE];
        }

        private void Update()
        {
            UpdateWaveValue();
        }

        private void OnDestroy()
        {
            m_instance = null;
        }
        #endregion


        #region Private Methods
        private void UpdateWaveValue()
        {
            m_audioSource.GetOutputData(m_samples, 0);

            float _wave = 0f;
            for (int i = 0; i < SAMPLE_SIZE; i++)
                _wave += m_samples[i]*m_samples[i];

            float _newValue = Mathf.Sqrt(_wave/SAMPLE_SIZE)*m_waveScale;
            m_waveValue = Mathf.Lerp(m_waveValue, _newValue, Time.deltaTime*m_calmDuration);
        }
        #endregion


        #region Properties
        public static AudioWaveProvider Instance
        {
            get
            {
                if (m_instance == null)
                    Debug.LogWarning("AudioWaveProvider: access to singleton before initialization.");

                return m_instance;
            }
        }

        public float CurrentWave
        {
            get
            {
                if (m_instance == null)
                    return 0f;

                return m_waveValue;
            }
        }

        public float MaxHeight
        {
            get
            {
                if (m_instance == null)
                    return 0f;

                return m_maxHeight;
            }
        }

        public float WaveIntensity
        {
            get
            {
                if (m_instance == null)
                    return 0f;

                return m_waveItensity;
            }
        }
        #endregion
    }
}