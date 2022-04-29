using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;
using Systems;

namespace Data
{
    public class DataManager
    {
        public static void LoadCards()
        {
            string cardPath = Application.streamingAssetsPath + "/cards/";
            Directory.CreateDirectory(cardPath);

            StreamingContext context = new StreamingContext(StreamingContextStates.File, new CardDataContext(cardPath));

            CardData[] data = JsonConvert.DeserializeObject<CardData[]>(File.ReadAllText(cardPath + "Producer1.json"), new JsonSerializerSettings() { Context = context });
            CardManager.LoadedCards.Add(data[0].Name, data[0]);
            data = JsonConvert.DeserializeObject<CardData[]>(File.ReadAllText(cardPath + "Worker1.json"), new JsonSerializerSettings() { Context = context });
            CardManager.LoadedCards.Add(data[0].Name, data[0]);
            data = JsonConvert.DeserializeObject<CardData[]>(File.ReadAllText(cardPath + "Resource1.json"), new JsonSerializerSettings() { Context = context });
            CardManager.LoadedCards.Add(data[0].Name, data[0]);

            CardManager.SpawnCard("Producer 1");
            CardManager.SpawnCard("Worker 1");
            CardManager.SpawnCard("Resource 1");
        }
    } 
}
