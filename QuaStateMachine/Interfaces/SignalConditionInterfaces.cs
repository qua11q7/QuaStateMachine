using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    public interface ISignalCondition {
        List<IState> Conditions { get; }
        bool IsValid { get; }
    }

    public interface ISignalCondition<S> : ISignalCondition {
        new List<IState<S>> Conditions { get; }
    }
}
