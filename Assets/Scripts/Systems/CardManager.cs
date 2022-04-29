using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Systems
{
    public static class CardManager
    {
        /// <summary>
        /// Loaded cards in data form.
        /// </summary>
        public static Dictionary<string, CardData> LoadedCards = new Dictionary<string, CardData>();
        /// <summary>
        /// Existing card instances indexed by name.
        /// </summary>
        public static Dictionary<string, List<Card>> CardsByName = new Dictionary<string, List<Card>>();
        /// <summary>
        /// Existing card instances indexed by tag.
        /// </summary>
        public static Dictionary<string, List<Card>> CardsByTag = new Dictionary<string, List<Card>>();

        public static Card SpawnCard(string name)
        {
            if (LoadedCards.TryGetValue(name, out CardData data))
            {
                Card card = UnityEngine.Object.Instantiate(GameManager.instance.CardPrefab, GameManager.CardContainer).GetComponent<Card>();
                card.Initialise(data);

                if (!CardsByName.ContainsKey(name))
                {
                    CardsByName.Add(name, new List<Card>());
                }
                CardsByName[name].Add(card);

                // TODO: Add to dictionary by tags.
                return card;
            }

            throw new ArgumentOutOfRangeException($"Card '{name}' does not exist.");
        }

        public static void RemoveCard(Card card)
        {
            if (CardsByName.ContainsKey(card.CardName))
            {
                CardsByName[card.CardName].Remove(card);
            }
            else
            {
                Debug.LogError($"Card '{card.CardName}' does not exist.");
            }
        }
    }
}
