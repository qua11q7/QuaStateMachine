using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    public delegate void StateMachineDelegate();
    public delegate void StateChanged(IState priorState, IState formerState);
    public delegate void StateChanged<S>(IState<S> priorState, IState<S> formerState);

    public delegate void TransitionStart(ITransition transition, TransitionEventArgs args);
    public class TransitionEventArgs : EventArgs {
        public bool CancelTransition { get; set; }
    }

    // TODO YO : make an event from StateMachine itself to see which signals didn't emit and why.
    // This implementation works only on Signal basis.
    public delegate void SignalNotProcessed(SignalNotProcessedEventArgs eventArgs);
    public class SignalNotProcessedEventArgs : EventArgs {
        public SignalFailure FailureCause { get; private set; }
        public List<ISignalCondition> FailedConditions { get; private set; }

        internal SignalNotProcessedEventArgs(SignalFailure failureCause, List<ISignalCondition> failedConditions = null) {
            FailureCause = failureCause;
            FailedConditions = failedConditions;
        }
    }

    public enum SignalFailure {
        NoTransitionToState,
        EmitConditionsNotMet,
        TransitionConditionsNotMet,
        TransitionAmbiguity
    }
}
