using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    public class StateMachine<S, G> {
        private StateMachine<S, int, G> SM;
        private int lastCreatedTransitionIndex;
        internal Dictionary<S, Dictionary<S, Transition<S, int, G>>> internalTransitions;

        public event StateChanged OnStateChanged;
        public event StateChanged<S> OnStateChangedGeneric;

        public StateMachine() {
            SM = new StateMachine<S, int, G>();
            SM.OnStateChanged += SM_OnStateChanged;

            internalTransitions = new Dictionary<S, Dictionary<S, Transition<S, int, G>>>();
        }

        private void SM_OnStateChanged(IState priorState, IState formerState) {
            OnStateChanged?.Invoke(priorState, formerState);
            OnStateChangedGeneric?.Invoke(priorState as IState<S>, formerState as IState<S>);
        }

        #region State Creation Adapter
        public IState CreateState(S stateName) => SM.CreateState(stateName);
        public bool TryCreateState(S stateName, out IState state) => SM.TryCreateState(stateName, out state);
        public IState CreateState(S innerStateName, S stateName, int orthogonalIndex = 0) => SM.CreateState(innerStateName, stateName, orthogonalIndex);
        public IState CreateState(S innerStateName, IState state, int orthogonalIndex = 0) => SM.CreateState(innerStateName, state, orthogonalIndex);
        public bool TryCreateState(S innerStateName, S stateName, out IState innerState, int orthogonalIndex = 0) => SM.TryCreateState(innerStateName, stateName, out innerState, orthogonalIndex);
        public bool TryCreateState(S innerStateName, IState state, out IState innerState, int orthogonalIndex = 0) => SM.TryCreateState(innerStateName, state, out innerState, orthogonalIndex);
        #endregion

        #region Signal Creation And Connection
        internal bool TryCreateSignal(G signalName, S sourceState, S destinationState, out ISignal signal, out ITransition transition) {
            Transition<S, int, G> _transition = GetOrCreateTransition(sourceState, destinationState);
            transition = _transition;
            return SM.ConnectSignal(signalName, _transition, out signal);
        }

        /// <summary>
        /// Creates and returns the signal as out parameter.
        /// </summary>
        public bool ConnectSignal(G signalName, S sourceState, S destinationState, out ISignal signal) {
            ITransition transition;
            bool success = TryCreateSignal(signalName, sourceState, destinationState, out signal, out transition);
            if (!success)
                return false;

            return ConnectSignal(signal, transition);
        }

        /// <summary>
        /// Creates the signal and returns the created transition as out parameter.
        /// </summary>
        public bool ConnectSignal(G signalName, S sourceState, S destinationState, out ITransition transition) {
            ISignal signal;
            bool success = TryCreateSignal(signalName, sourceState, destinationState, out signal, out transition);
            if (!success) {
                return false;
            }

            return ConnectSignal(signal, transition);
        }

        /// <summary>
        /// Creates and returns the signal as out parameter. Also returns created transitions index as out parameter.
        /// </summary>
        public bool ConnectSignal(G signalName, S sourceState, S destinationState, out ISignal signal, out int transitionIndex) {
            ITransition transition;
            bool success = TryCreateSignal(signalName, sourceState, destinationState, out signal, out transition);
            if (!success) {
                transitionIndex = -1;
                return false;
            }
            transitionIndex = (transition as Transition<S, int, G>).Name;

            return ConnectSignal(signal, transition);
        }

        /// <summary>
        /// Creates and returns the signal as out parameter. Also returns created transition as out parameter.
        /// </summary>
        public bool ConnectSignal(G signalName, S sourceState, S destinationState, out ISignal signal, out ITransition transition) {
            bool success = TryCreateSignal(signalName, sourceState, destinationState, out signal, out transition);
            if (!success) {
                return false;
            }

            return ConnectSignal(signal, transition);
        }

        /// <summary>
        /// Creates the signal if it hasn't created already.
        /// </summary>
        public bool ConnectSignal(G signalName, S sourceState, S destinationState) {
            if (!SM.signals.ContainsKey(signalName)) {
                ISignal signal;
                ITransition transition;
                return TryCreateSignal(signalName, sourceState, destinationState, out signal, out transition);
            }

            return ConnectSignal(signalName, GetOrCreateTransition(sourceState, destinationState));
        }

        /// <summary>
        /// Connects signal to the transition with the specified index.
        /// </summary>
        public bool ConnectSignal(G signalName, int transitionIndex) {
            return ConnectSignal(signalName, GetTransition(transitionIndex));
        }

        /// <summary>
        /// Connects signal to the specified transition.
        /// </summary>
        public bool ConnectSignal(G signalName, ITransition transition) {
            return SM.ConnectSignal(signalName, transition);
        }

        /// <summary>
        /// Connects signal to a transition. If the transition doesn't exist, creates it.
        /// </summary>
        public bool ConnectSignal(ISignal signal, S sourceState, S destinationState) {
            return ConnectSignal(signal, GetOrCreateTransition(sourceState, destinationState));
        }

        /// <summary>
        /// Connects signal to the transition with the specified index.
        /// </summary>
        public bool ConnectSignal(ISignal signal, int transitionIndex) {
            return ConnectSignal(signal, GetTransition(transitionIndex));
        }

        /// <summary>
        /// Connects signal to the specified transition.
        /// </summary>
        public bool ConnectSignal(ISignal signal, ITransition transition) {
            return SM.ConnectSignal(signal, transition);
        }
        #endregion

        #region Set Initial State Adapter
        public bool SetInitialState(S stateName) => SM.SetInitialState(stateName);
        public bool SetInitialState(IState state) => SM.SetInitialState(state);
        public bool SetInitialState(S innerStateName, S stateName, int orthogonalIndex = 0) => SM.SetInitialState(innerStateName, stateName, orthogonalIndex);
        public bool SetInitialState(S innerStateName, IState state, int orthogonalIndex = 0) => SM.SetInitialState(innerStateName, state, orthogonalIndex);
        public bool SetInitialState(IState innerState, IState state, int orthogonalIndex = 0) => SM.SetInitialState(innerState, state, orthogonalIndex);
        #endregion

        #region Emit Condition Creation
        public bool CreateEmitCondition(G signalName, params S[] conditionalStateNames) => SM.CreateEmitCondition(signalName, conditionalStateNames);
        public bool CreateEmitCondition(ISignal signal, params S[] conditionalStateNames) => SM.CreateEmitCondition(signal, conditionalStateNames);
        public bool CreateEmitCondition(ISignal signal, params IState[] conditionalStates) => SM.CreateEmitCondition(signal, conditionalStates);
        #endregion

        // TODO : Transition Condition Creation

        public void Initialize() => SM.Initialize();
        public void Terminate() => SM.Terminate();

        public IState GetStateByName(S stateName) => SM.GetStateByName(stateName);
        public ISignal GetSignalByName(G signalName) => SM.GetSignalByName(signalName);
        public List<S> GetAllActiveStateNames() => SM.GetAllActiveStateNames();
        public List<string> GetAllActiveStateNamesAsString() => SM.GetAllActiveStateNamesAsString();
        public List<IState> GetAllActiveState() => SM.GetAllActiveStates();

        private Transition<S, int, G> GetOrCreateTransition(S sourceState, S destinationState) {
            if (internalTransitions.ContainsKey(sourceState)) {
                if (internalTransitions[sourceState].ContainsKey(destinationState)) {
                    return internalTransitions[sourceState][destinationState];
                } else {
                    ITransition createdTransition;
                    bool success = SM.TryCreateTransition(lastCreatedTransitionIndex++, sourceState, destinationState, out createdTransition);
                    if (!success)
                        throw new Exception("Couldn't create transition. Please check your state machine creation.");

                    internalTransitions[sourceState].Add(destinationState, createdTransition as Transition<S, int, G>);
                    return createdTransition as Transition<S, int, G>;
                }
            } else {
                ITransition createdTransition;
                bool success = SM.TryCreateTransition(lastCreatedTransitionIndex++, sourceState, destinationState, out createdTransition);
                if (!success)
                    throw new Exception("Couldn't create transition. Please check your state machine creation.");

                internalTransitions.Add(sourceState, new Dictionary<S, Transition<S, int, G>>());
                internalTransitions[sourceState].Add(destinationState, createdTransition as Transition<S, int, G>);
                return createdTransition as Transition<S, int, G>;
            }
        }

        public ITransition GetTransition(int index) {
            return SM.GetTransitionByName(index);
        }

        public ITransition GetTransition(S sourceState, S destinationState) {
            if (internalTransitions.ContainsKey(sourceState)) {
                if (internalTransitions[sourceState].ContainsKey(destinationState)) {
                    return internalTransitions[sourceState][destinationState];
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }
    }

}
