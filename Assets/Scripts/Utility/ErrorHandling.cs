using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utility
{
    public class AssertFailedException : Exception
    {
        public AssertFailedException(string message) : base(message)
        {
        }
    }
    
    /// <summary>
    /// Exception throwing assertions.
    /// </summary>
    public static class Assert
    {
        
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new AssertFailedException(message);
            }
        }
    }
}
