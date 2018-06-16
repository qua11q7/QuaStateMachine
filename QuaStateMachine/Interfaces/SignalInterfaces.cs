using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    public interface ISignal {
        bool Emit();
        event SignalNotProcessed OnSignalNotProcessed;
    }

    public interface ISignal<G> : ISignal {
        G Name { get; }
    }
}
