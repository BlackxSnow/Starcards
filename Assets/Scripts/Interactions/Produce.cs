using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;

namespace Interactions
{
    public class Produce : Interaction
    {
        public ProduceData Data { get; protected set; }
        public override bool RunIfValid()
        {
            if (Data.Worker.CardName == "" || Data.Worker.CardName == AttachedCard.StackedChild.CardName)
            {
                if (!IsRunning)
                {
                    Run();
                }
                IsRunning = true;
                return true;
            }

            IsRunning = false;
            return false;
        }

        private async void Run()
        {
            UI.ProgressBar progressBar = AttachedCard.RequestProgressBar();
            float lastTick = Time.time;
            float elapsed;
            while (IsRunning)
            {
                elapsed = Time.time - lastTick;
                progressBar.UpdateValue(elapsed / Data.Duration);

                if (elapsed > Data.Duration)
                {
                    lastTick = Time.time;
                    for (int i = 0; i < Data.Output.Quantity; i++)
                    {
                        Systems.CardManager.SpawnCard(Data.Output.CardName); 
                    }
                }
                await Await.NextUpdate();
            }
        }
    }
}
