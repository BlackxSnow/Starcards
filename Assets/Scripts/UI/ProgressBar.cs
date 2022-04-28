using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private RectTransform Background;
        [SerializeField]
        private RectTransform Fill;

        public void UpdateValue(float percentage)
        {
            Fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Background.rect.width * percentage);
        }
    }
}
