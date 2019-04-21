using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace CubesECS.Classic
{
    [BurstCompile]
    public struct MovementJob : IJobParallelForTransform
    {
        #region Serialized Fields
        private float m_moveSpeed;
        private float m_topBound;
        private float m_bottomBound;
        private float m_deltaTime;
        private float m_musicWave;
        private float m_musicWaveIntensity;
        #endregion


        #region Job Methods
        public void Execute(int index, TransformAccess transform)
        {
            Vector3 _position = transform.position;
            _position += Vector3.forward*m_moveSpeed*m_deltaTime;

            if (_position.z > m_bottomBound)
                _position.z = m_topBound;

            if ((index % 2) == 0)
                _position.y = transform.localScale.y/2 + m_musicWave;
            else
                _position.y = transform.localScale.y/2f;

            _position.y = Mathf.Lerp(transform.position.y, _position.y, m_deltaTime*m_musicWaveIntensity);

            transform.position = _position;
        }
        #endregion

        
        #region Properties
        public float moveSpeed
        {
            get { return m_moveSpeed; }
            set { m_moveSpeed = value; }
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

        public float musicWave
        {
            get { return m_musicWave; }
            set { m_musicWave = value; }
        }

        public float musicWaveIntensity
        {
            get { return m_musicWaveIntensity; }
            set { m_musicWaveIntensity = value; }
        }
        #endregion
    }
}