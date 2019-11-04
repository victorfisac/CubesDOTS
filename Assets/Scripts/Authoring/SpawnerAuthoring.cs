using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using CubesDOTS.Components;


namespace CubesDOTS.Authoring
{
    public class SpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        #region Inspector Fields
        [Header("Spawn")]
        [SerializeField]
        private GameObject m_prefab;
        [SerializeField]
        private float m_frequency;
        [SerializeField]
        private float m_maxDistance;
        #endregion


        #region Authoring Methods
        public void DeclareReferencedPrefabs(List<GameObject> pReferencedPrefabs)
        {
            pReferencedPrefabs.Add(m_prefab);
        }

        public void Convert(Entity pEntity, EntityManager pManager, GameObjectConversionSystem pConversionSystem)
        {
            pManager.AddComponentData(pEntity, new SpawnData() {
                prefab = pConversionSystem.GetPrimaryEntity(m_prefab),
                distance = m_maxDistance,
                frequency = m_frequency,
                timeCounter = 0f
            });
        }
        #endregion
    }
}