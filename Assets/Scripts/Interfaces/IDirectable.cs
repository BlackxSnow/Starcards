using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Interfaces
{
    /// <summary>
    /// Implementor can be directed to a position for internal handling.
    /// </summary>
    public interface IDirectable
    {
        Vector3 TargetPosition { get; set; }
    }
}
