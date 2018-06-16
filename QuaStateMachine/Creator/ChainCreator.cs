using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine.Creator {
    public class ChainCreator<S, T, G> : IStateCreator<S, T, G>, ITransitionCreator<S, T, G>, ISignalCreator<S, T, G> {
        public StateMachine<S, T, G> StateMachine { get; private set; }
        private State<S, T, G> LastCreatedState { get; set; }
        private Transition<S, T, G> LastCreatedTransition { get; set; }
        private Signal<S, T, G> LastCreatedSignal { get; set; }

        public ChainCreator(StateMachine<S, T, G> stateMachine) {
            StateMachine = stateMachine;
        }

        #region State
        public IStateCreator<S, T, G> CreateState(S stateName) {
            IState state;
            return CreateState(stateName, out state);
        }

        public IStateCreator<S, T, G> CreateState(S stateName, out IState state) {
            if (StateMachine.TryCreateState(stateName, out state)) {
                LastCreatedState = state as State<S, T, G>;
                return this;
            }

            return null;
        }

        public IStateCreator<S, T, G> CreateInnerState(S innerStateName, S parentStateName, int orthogonalIndex = 0) {
            IState state;
            return CreateInnerState(innerStateName, parentStateName, out state, orthogonalIndex);
        }

        public IStateCreator<S, T, G> CreateInnerState(S innerStateName, S parentStateName, out IState innerState, int orthogonalIndex = 0) {
            if (StateMachine.TryCreateState(innerStateName, parentStateName, out innerState, orthogonalIndex)) {
                LastCreatedState = innerState as State<S, T, G>;
                return this;
            }

            return null;
        }

        public IStateCreator<S, T, G> OnStateEnter(StateMachineDelegate function) {
            if (LastCreatedState == null)
                return null;

            LastCreatedState.OnStateEnter += function;
            return this;
        }

        public IStateCreator<S, T, G> OnStateLeave(StateMachineDelegate function) {
            if (LastCreatedState == null)
                return null;

            LastCreatedState.OnStateLeave += function;
            return this;
        }

        public IStateCreator<S, T, G> OnStateTerminated(StateMachineDelegate function) {
            if (LastCreatedState == null)
                return null;

            LastCreatedState.OnStateTerminated += function;
            return this;
        }

        public IBaseCreator<S, T, G> SetInitialState(S stateName) {
            if (StateMachine.SetInitialState(stateName))
                return this;
            return null;
        }

        public IBaseCreator<S, T, G> SetInitialInnerState(S stateName, S parentStateName, int orthogonalIndex = 0) {
            if (StateMachine.SetInitialState(stateName, parentStateName, orthogonalIndex))
                return this;
            return null;
        }
        #endregion

        #region Transition
        public ITransitionCreator<S, T, G> CreateTransition(T transitionName, S startStateName, S endStateName) {
            ITransition transition;
            return CreateTransition(transitionName, startStateName, endStateName, out transition);
        }

        public ITransitionCreator<S, T, G> CreateTransition(T transitionName, S startStateName, S endStateName, out ITransition transition) {
            if (StateMachine.TryCreateTransition(transitionName, startStateName, endStateName, out transition)) {
                LastCreatedTransition = transition as Transition<S, T, G>;
                return this;
            }

            return null;
        }

        public ITransitionCreator<S, T, G> OnTransitionStarted(TransitionStart function) {
            if (LastCreatedTransition == null)
                return null;

            LastCreatedTransition.OnTransitionStart += function;
            return this;
        }

        public ITransitionCreator<S, T, G> OnTransitionFinished(StateMachineDelegate function) {
            if (LastCreatedTransition == null)
                return null;

            LastCreatedTransition.OnTransitionFinish += function;
            return this;
        }
        #endregion

        #region Signal
        public ISignalCreator<S, T, G> CreateSignal(G signalName, T transitionName) {
            ISignal signal;
            return CreateSignal(signalName, transitionName, out signal);
        }

        public ISignalCreator<S, T, G> CreateSignal(G signalName, T transitionName, out ISignal signal) {
            if (StateMachine.ConnectSignal(signalName, transitionName, out signal)) {
                LastCreatedSignal = signal as Signal<S, T, G>;
                return this;
            }

            return null;
        }

        public IBaseCreator<S, T, G> ConnectSignal(G signalName, T transitionName) {
            if (StateMachine.ConnectSignal(signalName, transitionName)) {
                return this;
            }

            return null;
        }

        public ISignalCreator<S, T, G> OnSignalNotProcessed(SignalNotProcessed function) {
            if (LastCreatedSignal == null)
                return null;

            LastCreatedSignal.OnSignalNotProcessed += function;
            return this;
        }
        #endregion
    }

    public interface IBaseCreator<S, T, G> {
        StateMachine<S, T, G> StateMachine { get; }

        IStateCreator<S, T, G> CreateState(S stateName);
        IStateCreator<S, T, G> CreateState(S stateName, out IState state);
        IStateCreator<S, T, G> CreateInnerState(S innerStateName, S parentStateName, int orthogonalIndex = 0);
        IStateCreator<S, T, G> CreateInnerState(S innerStateName, S parentStateName, out IState innerState, int orthogonalIndex = 0);
        IBaseCreator<S, T, G> SetInitialState(S stateName);
        IBaseCreator<S, T, G> SetInitialInnerState(S stateName, S parentStateName, int orthogonalIndex = 0);

        ITransitionCreator<S, T, G> CreateTransition(T transitionName, S startStateName, S endStateName);
        ITransitionCreator<S, T, G> CreateTransition(T transitionName, S startStateName, S endStateName, out ITransition transition);

        ISignalCreator<S, T, G> CreateSignal(G signalName, T transitionName);
        ISignalCreator<S, T, G> CreateSignal(G signalName, T transitionName, out ISignal signal);
        IBaseCreator<S, T, G> ConnectSignal(G signalName, T transitionName);
    }

    public interface IStateCreator<S, T, G> : IBaseCreator<S, T, G> {
        IStateCreator<S, T, G> OnStateEnter(StateMachineDelegate function);
        IStateCreator<S, T, G> OnStateLeave(StateMachineDelegate function);
        IStateCreator<S, T, G> OnStateTerminated(StateMachineDelegate function);
    }

    public interface ITransitionCreator<S, T, G> : IBaseCreator<S, T, G> {
        ITransitionCreator<S, T, G> OnTransitionStarted(TransitionStart function);
        ITransitionCreator<S, T, G> OnTransitionFinished(StateMachineDelegate function);
    }

    public interface ISignalCreator<S, T, G> : IBaseCreator<S, T, G> {
        ISignalCreator<S, T, G> OnSignalNotProcessed(SignalNotProcessed function);
    }
}
