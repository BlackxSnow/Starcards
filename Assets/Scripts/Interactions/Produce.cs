//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityAsync;
//using UnityEngine;

//namespace Interactions
//{
//    public class Produce : Interaction
//    {
//        public override bool RunIfValid()
//        {
//            if (Data.Worker.CardName == "" || Data.Worker.CardName == AttachedCard.StackedChild.CardName)
//            {
//                if (!IsRunning)
//                {
//                    Run();
//                }
//                IsRunning = true;
//                return true;
//            }

//            IsRunning = false;
//            return false;
//        }
//    }
//}
