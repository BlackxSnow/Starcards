using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utility
{
    public static class Vector
    {
        public static bool IsNaN(Vector2 vec)
        {
            return float.IsNaN(vec.x) || float.IsNaN(vec.y);
        }
        public static bool IsNaN(Vector3 vec)
        {
            return float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z);
        }
    }
}
