using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;
using Systems;
using System;

namespace Data
{
    public class DataManager
    {
        public class DirectoryNode : IEnumerable<DirectoryNode>
        {
            public string Path { get; private set; }
            public Dictionary<string, DirectoryNode> Children = new Dictionary<string, DirectoryNode>();


            public bool Contains(string child)
            {
                return Children.ContainsKey(child);
            }

            public DirectoryNode this[string path]
            {
                get => Children[path];
                set => Children[path] = value;
            }
            
            public IEnumerator<DirectoryNode> GetEnumerator()
            {
                return Children.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            
            public DirectoryNode(string path, params KeyValuePair<string, DirectoryNode>[] children)
            {
                Path = path;
            }
        }

        public static event Action FinishedLoad;

        public static void LoadModules()
        {
            string[] modules = Directory.GetDirectories(Application.streamingAssetsPath);

            foreach(string module in modules)
            {
                LoadSingle(module);
            }

            FinishedLoad?.Invoke();
        }

        private static void LoadSingle(string module)
        {
            DirectoryNode root = BuildDirectoryTree(module);

            uint cardCount = LoadCards(root);

            Debug.Log($"Loaded module {module}: Cards: {cardCount}");
        }

        private static DirectoryNode BuildDirectoryTree(string root)
        {
            Stack<DirectoryNode> unexplored = new Stack<DirectoryNode>();
            DirectoryNode rootNode = new DirectoryNode(root);
            unexplored.Push(rootNode);

            DirectoryNode current;
            while (unexplored.Count > 0)
            {
                current = unexplored.Pop();
                string[] children = Directory.GetDirectories(current.Path);
                foreach (string child in children)
                {
                    DirectoryNode childNode = new DirectoryNode(child);
                    current.Children.Add(child, childNode);
                    unexplored.Push(childNode);
                }
            }

            return rootNode;
        }

        private static uint LoadCards(DirectoryNode moduleRoot)
        {
            uint loaded = 0;
            string cardPath = Path.Combine(moduleRoot.Path, "cards");
            if (moduleRoot.Children.TryGetValue(cardPath, out DirectoryNode cardRoot))
            {
                Stack<DirectoryNode> unexplored = new Stack<DirectoryNode>();
                unexplored.Push(cardRoot);

                DirectoryNode current;
                while (unexplored.Count > 0)
                {
                    current = unexplored.Pop();
                    StreamingContext context = new StreamingContext(StreamingContextStates.File, new CardDataContext(cardPath));
                    foreach (string file in Directory.EnumerateFiles(current.Path, "*.json"))
                    {
                        string json = File.ReadAllText(file);
                        CardData[] data = JsonConvert.DeserializeObject<CardData[]>(json, new JsonSerializerSettings() { Context = context });
                        foreach (CardData card in data)
                        {
                            CardManager.LoadedCards.Add(card.Name, card);
                            loaded++;
                        }
                    }

                    foreach (DirectoryNode child in current)
                    {
                        unexplored.Push(child);
                    }
                }
            }
            return loaded;
        }
    } 
}
