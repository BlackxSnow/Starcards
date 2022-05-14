using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Data
{
    [JsonConverter(typeof(InteractionConverter))]
    public class InteractionData
    {
        public static uint NextID = 0;
        public readonly uint ID;
        /// <summary>
        /// Required to be present but not consumed.
        /// </summary>
        public List<CardRef> Require = new List<CardRef>();
        /// <summary>
        /// Required to be present and consumed.
        /// </summary>
        public List<CardRef> Consume = new List<CardRef>();
        /// <summary>
        /// Created at the end of the interaction.
        /// </summary>
        public List<CardRef> Create = new List<CardRef>();
        /// <summary>
        /// Length of the interaction. 0 is instant.
        /// </summary>
        public float Duration = 0;
        /// <summary>
        /// Higher priority interactions will be executed before lower priority ones.
        /// </summary>
        public int Priority = 0;

        public bool TryFulfillCards(Card requester, List<CardRef> toFulfill, out List<Card> fulfilled)
        {
            Dictionary<string, int> requests = toFulfill.ToDictionary((r) => r.CardName, (r) => r.Quantity);
            fulfilled = new List<Card>();

            Card current = requester.GetNextMajor();
            while (requests.Count > 0 && current != null)
            {
                fulfilled.AddRange(current.RequestCards(ref requests));
                current = current.GetNextMajor();
            }

            return requests.Count == 0;
        }

        public bool IsValidForRun(Card card, out Card[] required, out Card[] consumed)
        {
            // TODO: Implement range in cardRefs
            required = null;
            consumed = null;

            if (TryFulfillCards(card, Require, out var req) && TryFulfillCards(card, Consume, out var con))
            {
                required = req.ToArray();
                consumed = con.ToArray();
                return true;
            }

            return false;
        }
        public InteractionData()
        {
            ID = NextID++;
        }
    }


    public class InteractionConverter : JsonConverter<InteractionData>
    {
        public override InteractionData ReadJson(JsonReader reader, Type objectType, InteractionData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            InteractionData result = new InteractionData();

            JToken require = obj["Require"];
            if (require != null)
            {
                HydrateCardRefs(require.Value<JArray>(), ref result.Require, "Require");
            }

            JToken consume = obj["Consume"];
            if (consume != null)
            {
                HydrateCardRefs(consume.Value<JArray>(), ref result.Consume, "Consume");
            }

            JToken create = obj["Create"];
            if (create != null)
            {
                HydrateCardRefs(create.Value<JArray>(), ref result.Create, "Create");
            }

            JToken duration = obj["Duration"];
            if (duration != null)
            {
                result.Duration = duration.Value<float>();
            }

            JToken priority = obj["Priority"];
            if (priority != null)
            {
                result.Priority = priority.Value<int>();
            }

            return result;
        }

        private void HydrateCardRefs(JArray array, ref List<CardRef> target, string arrayName)
        {
            foreach (JToken card in array)
            {
                if (card is JObject cardRefContainer)
                {
                    CardRef cardRef = cardRefContainer.Properties().First().Value.ToObject<CardRef>();
                    string cardName = cardRefContainer.Properties().First().Name;
                    cardRef.CardName = cardName;
                    target.Add(cardRef);
                }
                else if (card is JValue cardName)
                {
                    CardRef cardRef = new CardRef();
                    cardRef.Quantity = 1;
                    cardRef.CardName = cardName.Value<string>();
                    target.Add(cardRef);
                }
                else
                {
                    throw new AssertFailedException($"Invalid type in interaction array '{arrayName}'.");
                }
            }
        }

        public override void WriteJson(JsonWriter writer, InteractionData value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
