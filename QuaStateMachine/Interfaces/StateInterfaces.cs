using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    public interface IState {
        bool HasInnerState { get; }
        bool IsCurrentState { get; }
        bool HasParentState { get; }

        event StateMachineDelegate OnStateEnter;
        event StateMachineDelegate OnStateLeave;
        event StateMachineDelegate OnStateTerminated;
    }

    public interface IState<S> : IState {
        S Name { get; }
    }
}
