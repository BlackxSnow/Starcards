using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public static Transform CardContainer { get; private set; }
        [SerializeField]
        private Transform _CardContainer;

        [Header("Prefabs")]
        public GameObject CardPrefab;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning($"A second game manager was created while one was already registered on {gameObject.name}");
                Destroy(this);
            }

            if (_CardContainer == null) throw new System.NullReferenceException("_CardContainer is null. Set from inspector.");
            CardContainer = _CardContainer;

            DataManager.LoadCards();
        }


    }
}