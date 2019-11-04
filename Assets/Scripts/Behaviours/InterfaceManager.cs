using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace CubesECS.Behaviours
{
    public class InterfaceManager : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Configuration")]
        [SerializeField]
        private float m_refreshFreq;

        [Header("References")]
        [SerializeField]
        private Text m_fpsTxt;
        [SerializeField]
        private Text m_objectsTxt;
        #endregion


        #region Private Fields
        private EntityManager m_manager;
        private float m_timeCounter = 0f;
        private float m_framesPerSecond = 0f;
        #endregion


        #region Main Methods
        private void Start()
        {
            m_manager = World.Active.EntityManager;
        }

        private void Update()
        {
            m_timeCounter += Time.deltaTime;

            if (m_timeCounter >= m_refreshFreq)
            {
                m_timeCounter = 0f;
                UpdateTexts();
            }
        }
        #endregion


        #region Private Methods
        private void UpdateTexts()
        {
            var _entities = m_manager.GetAllEntities(Allocator.Temp);
            m_objectsTxt.text = string.Format("{0} entities", _entities.Length);

            m_framesPerSecond = 1f/Time.deltaTime;

            m_fpsTxt.text = string.Format("{0} FPS", m_framesPerSecond.ToString("00"));
        }
        #endregion
    }
}