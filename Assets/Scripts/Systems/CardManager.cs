using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Systems
{
    public static class CardManager
    {
        public static Dictionary<string, CardData> Cards = new Dictionary<string, CardData>();

        public static Card SpawnCard(string name)
        {
            if (Cards.TryGetValue(name, out CardData data))
            {
                Card card = UnityEngine.Object.Instantiate(GameManager.instance.CardPrefab, GameManager.CardContainer).GetComponent<Card>();
                card.Initialise(data);
                return card;
            }

            throw new ArgumentOutOfRangeException($"Card '{name}' does not exist.");
        }
    }
}
