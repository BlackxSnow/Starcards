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

        public static Card SpawnCard(string name, Vector3 position = default, Quaternion rotation = default)
        {
            if (LoadedCards.TryGetValue(name, out CardData data))
            {
                return SpawnCard(data, position, rotation);
            }

            throw new ArgumentOutOfRangeException($"Card '{name}' does not exist.");
        }

        public static Card SpawnCard(CardData data, Vector3 position = default, Quaternion rotation = default)
        {
            Card card = UnityEngine.Object.Instantiate(GameManager.instance.CardPrefab, position, rotation, GameManager.CardContainer).GetComponent<Card>();
            card.gameObject.name = data.Name;
            card.Initialise(data);

            if (!CardsByName.ContainsKey(data.Name))
            {
                CardsByName.Add(data.Name, new List<Card>());
            }
            // TODO: Add to dictionary by tags.            
            CardsByName[data.Name].Add(card);
            return card;
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

        private static void PrepareCardForDestroy(Card card)
        {
            CardsByName.Remove(card.CardName);
            card.PrepareForDestroy();

            if (card.StackedOn != null && card.StackedChild != null)
            {
                card.StackedOn.UnstackChild(false);
                card.StackedChild.StackOn(card.StackedOn);
            }
            else
            {
                card.StackedOn?.UnstackChild(true);
                card.StackedChild?.Unstack();
            }

            card.transform.SetParent(GameManager.CardContainer);
            card.ClearStackRefs();
        }


        public static void DestroyCards(params Card[] cards)
        {
            foreach(Card toDestroy in cards)
            {
                PrepareCardForDestroy(toDestroy);
                UnityEngine.Object.Destroy(toDestroy.gameObject);
            }
        }

        public static void DestroyCards(Vector3 moveTo, params Card[] cards)
        {
            foreach (Card toDestroy in cards)
            {
                PrepareCardForDestroy(toDestroy);

                toDestroy.MoveTo(moveTo, (v) => UnityEngine.Object.Destroy(v.gameObject));
            }
        }
    }
}
