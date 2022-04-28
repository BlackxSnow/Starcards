using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactions
{
    public abstract class Interaction
    {
        /// <summary>
        /// Whether the interaction is currently running.
        /// </summary>
        public bool IsRunning { get; protected set; }

        public Card AttachedCard { get; protected set; }

        /// <summary>
        /// Determine whether the interaction can validly run.
        /// </summary>
        /// <returns></returns>
        public abstract bool RunIfValid();
    }
}
