using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Systems
{
    public class DataManager
    {
        public static void LoadCards()
        {
            string cardPath = Application.streamingAssetsPath + "/cards/";
            Directory.CreateDirectory(cardPath);

            //using (StreamWriter writer = new StreamWriter(cardPath + "test.json"))
            //{
            //    writer.Write(JsonConvert.SerializeObject(
            //        new InteractionMetadata(typeof(uint), 
            //            new ProduceData(new CardRef("card1", 1), new CardRef("outputCard", 2), 5.0f)), 
            //        Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));
            //};

            CardData data = JsonConvert.DeserializeObject<CardData>(File.ReadAllText(cardPath + "Producer1.json"), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            CardManager.Cards.Add(data.Name, data);
            data = JsonConvert.DeserializeObject<CardData>(File.ReadAllText(cardPath + "Worker1.json"), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            CardManager.Cards.Add(data.Name, data);
            data = JsonConvert.DeserializeObject<CardData>(File.ReadAllText(cardPath + "Resource1.json"), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            CardManager.Cards.Add(data.Name, data);

            CardManager.SpawnCard("Producer 1");
            CardManager.SpawnCard("Worker 1");
            CardManager.SpawnCard("Resource 1");
        }
    } 
}
