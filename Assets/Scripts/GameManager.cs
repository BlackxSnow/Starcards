using Data;
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

            DataManager.LoadModules();
            SpawnAllCardsInGrid();
        }

        private void SpawnAllCardsInGrid()
        {
            const float xExtent = 10;
            const float yExtent = 4;
            const float xSpacing = 1.5f;
            const float ySpacing = 2f;

            const int xCount = (int)(xExtent / xSpacing);

            int current = 0;
            foreach (CardData card in CardManager.LoadedCards.Values)
            {
                int x = current % xCount;
                int y = current / xCount;
                CardManager.SpawnCard(card, new Vector3(-xExtent + x * xSpacing, yExtent - y * ySpacing, 0));
                current++;
            }
        }
    }
}