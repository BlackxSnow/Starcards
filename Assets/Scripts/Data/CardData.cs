using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utility;

namespace Data
{
    public struct CardRef
    {
        public string CardName;
        public int Quantity;

    }


    /// <summary>
    /// Contains cards in their serializable form.
    /// </summary>
    [JsonConverter(typeof(CardDataConverter))]
    public class CardData
    {
        /// <summary>
        /// Display name of the card.
        /// </summary>
        public string Name;
        public Texture2D Image;

        //public Dictionary<string, List<InteractionData>> OnStack = new Dictionary<string, List<InteractionData>>();
        //public Dictionary<string, List<InteractionData>> OnTick = new Dictionary<string, List<InteractionData>>();
        public List<InteractionData> OnStack = new List<InteractionData>();
        public List<InteractionData> OnTick = new List<InteractionData>();
    }

    public struct CardDataContext
    {
        public string FilePath;

        public CardDataContext(string filePath)
        {
            FilePath = filePath;
        }
    }

    public class CardRefConverter : JsonConverter<CardRef>
    {
        public override CardRef ReadJson(JsonReader reader, Type objectType, CardRef existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            CardRef result = new CardRef();

            JValue quantity = obj["Quantity"] as JValue;
            result.Quantity = quantity != null ? quantity.Value<int>() : 1;

            return result;
        }

        public override void WriteJson(JsonWriter writer, CardRef value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }



    public class CardDataConverter : JsonConverter<CardData>
    {
        public override CardData ReadJson(JsonReader reader, Type objectType, CardData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (serializer.Context.Context is CardDataContext context)
            {
                JObject data = JObject.Load(reader);
                CardData result = new CardData();

                try
                {
                    Hydrate(ref result, data.Properties().First(), context);
                }
                catch (AssertFailedException e)
                {
                    Debug.LogError($"Error while loading {data.Properties().First().Name}: {e.Message}");
                    throw;
                }
                return result;
            }

            throw new Exception("CardDataConverter requires a CardDataContext to be passed to the JsonSerializer.");
        }

        private void Hydrate(ref CardData data, JProperty property, CardDataContext context)
        {
            JObject obj = property.Value as JObject;
            data.Name = property.Name;

            string relativePath = obj["Image"].Value<string>();
            Assert.IsTrue(relativePath != null, $"Card image path is null.");
            string fullImagePath = Path.Combine(context.FilePath, relativePath);
            data.Image = new Texture2D(0, 0);
            data.Image.LoadImage(File.ReadAllBytes(fullImagePath));

            JToken ost = obj["OnStack"];
            JArray onStack = ost as JArray;
            Assert.IsTrue((ost != null) == (onStack != null), "OnStack must be an array or null.");
            if (ost != null)
            {
                foreach (JToken token in onStack.Children())
                {
                    JObject interaction = token as JObject;
                    Assert.IsTrue(interaction != null, "");
                    data.OnStack.Add(interaction.ToObject<InteractionData>());
                }
            }
        }

        public override void WriteJson(JsonWriter writer, CardData value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}