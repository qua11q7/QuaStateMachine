using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    public interface ITransition {
        event TransitionStart OnTransitionStart;
        event StateMachineDelegate OnTransitionFinish;
    }

    public interface ITransition<T> : ITransition {
        T Name { get; }
    }
}
