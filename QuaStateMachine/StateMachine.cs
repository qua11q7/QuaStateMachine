using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    // Should we implement IDisposable? Some events are connected to States and Transitions.
    // User should know that we are disposing so that user can unsubscribe those events.
    public class StateMachine<S, T, G> {
        internal Dictionary<S, State<S, T, G>> states;
        internal Dictionary<T, Transition<S, T, G>> transitions;
        internal Dictionary<G, Signal<S, T, G>> signals;
        internal State<S, T, G> InitialState { get; private set; }
        internal State<S, T, G> CurrentState { get; private set; }
        public bool Initialized { get; private set; }

        // TODO YO : OnTerminate
        public event StateChanged OnStateChanged;

        public StateMachine() {
            states = new Dictionary<S, State<S, T, G>>();
            transitions = new Dictionary<T, Transition<S, T, G>>();
            signals = new Dictionary<G, Signal<S, T, G>>();
        }

        #region State Creation
        internal bool AddState(State<S, T, G> state) {
            if (state == null || states.ContainsKey(state.Name)) {
                return false;
            }

            state.SetStateMachine(this);
            states.Add(state.Name, state);
            return true;
        }

        public IState CreateState(S stateName) {
            State<S, T, G> state = new State<S, T, G>(stateName);
            AddState(state);
            return state;
        }

        public IState CreateState(S innerStateName, S stateName, int orthogonalIndex = 0) {
            State<S, T, G> state = states[stateName];
            return CreateState(innerStateName, state, orthogonalIndex);
        }

        public IState CreateState(S innerStateName, IState state, int orthogonalIndex = 0) {
            State<S, T, G> innerState = new State<S, T, G>(innerStateName);
            states.Add(innerStateName, innerState);

            (state as State<S, T, G>).AddInnerState(innerState, orthogonalIndex);

            return innerState;
        }

        public bool TryCreateState(S stateName, out IState state) {
            if (states.ContainsKey(stateName)) {
                state = null;
                return false;
            }

            state = CreateState(stateName);
            return true;
        }

        public bool TryCreateState(S innerStateName, S stateName, out IState innerState, int orthogonalIndex = 0) {
            if (!states.ContainsKey(stateName) || states.ContainsKey(innerStateName)) {
                innerState = null;
                return false;
            }

            State<S, T, G> state = states[stateName];
            return TryCreateState(innerStateName, state, out innerState, orthogonalIndex);
        }

        public bool TryCreateState(S innerStateName, IState state, out IState innerState, int orthogonalIndex = 0) {
            if (state == null) {
                innerState = null;
                return false;
            }

            innerState = CreateState(innerStateName, state, orthogonalIndex);
            return true;
        }
        #endregion

        #region Transition Creation
        public ITransition CreateTransition(T transitionName, S startStateName, S endStateName) {
            State<S, T, G> startState = states[startStateName];
            State<S, T, G> endState = states[endStateName];

            if (startState.SM != endState.SM) {
                throw new InvalidTransitionException<S, T>(startState.Name, endState.Name, transitionName);
            }

            return CreateTransition(transitionName, startState, endState);
        }

        public ITransition CreateTransition(T transitionName, IState startState, IState endState) {
            if (startState == null || endState == null) {
                return null;
            }

            State<S, T, G> sState = startState as State<S, T, G>;
            State<S, T, G> eState = endState as State<S, T, G>;

            if (sState.SM != eState.SM) {
                throw new InvalidTransitionException<S, T>(sState.Name, eState.Name, transitionName);
            }

            Transition<S, T, G> transition = new Transition<S, T, G>(transitionName, sState, eState);
            transitions.Add(transitionName, transition);
            sState.AddTransition(transition, eState);

            return transition;
        }

        public bool TryCreateTransition(T transitionName, S startStateName, S endStateName, out ITransition transition) {
            if (transitions.ContainsKey(transitionName) || !states.ContainsKey(startStateName) || !states.ContainsKey(endStateName)) {
                transition = null;
                return false;
            }

            State<S, T, G> startState = states[startStateName];
            State<S, T, G> endState = states[endStateName];
            return TryCreateTransition(transitionName, startState, endState, out transition);
        }

        public bool TryCreateTransition(T transitionName, IState startState, IState endState, out ITransition transition) {
            if (startState == null || endState == null || transitions.ContainsKey(transitionName)) {
                transition = null;
                return false;
            }

            State<S, T, G> sState = startState as State<S, T, G>;
            State<S, T, G> eState = endState as State<S, T, G>;

            if (sState.SM != eState.SM) {
                throw new InvalidTransitionException<S, T>(sState.Name, eState.Name, transitionName);
            }

            transition = CreateTransition(transitionName, startState, endState);
            return true;
        }
        #endregion

        #region Signal Creation And Connection
        public bool ConnectSignal(G signalName, T transitionName) {
            if (!transitions.ContainsKey(transitionName)) {
                return false;
            }

            Transition<S, T, G> transition = transitions[transitionName];
            return ConnectSignal(signalName, transition);
        }

        public bool ConnectSignal(G signalName, T transitionName, out ISignal signal) {
            if (!transitions.ContainsKey(transitionName)) {
                signal = null;
                return false;
            }

            Transition<S, T, G> transition = transitions[transitionName];
            return ConnectSignal(signalName, transition, out signal);
        }

        public bool ConnectSignal(G signalName, ITransition transition) {
            if (transition == null) {
                return false;
            }

            ISignal signal;
            if (!signals.ContainsKey(signalName)) {
                signal = CreateSignal(signalName, transition) as Signal<S, T, G>;
                return true;
            } else {
                signal = signals[signalName];
            }

            return ConnectSignal(signal, transition);
        }

        public bool ConnectSignal(G signalName, ITransition transition, out ISignal signal) {
            if (transition == null) {
                signal = null;
                return false;
            }

            if (!signals.ContainsKey(signalName)) {
                signal = CreateSignal(signalName, transition) as Signal<S, T, G>;
                return true;
            } else {
                signal = signals[signalName];
            }

            return ConnectSignal(signal, transition);
        }

        public bool ConnectSignal(ISignal signal, T transitionName) {
            if (signal == null || !transitions.ContainsKey(transitionName)) {
                return false;
            }

            Transition<S, T, G> transition = transitions[transitionName];
            return ConnectSignal(signal, transition);
        }

        public bool ConnectSignal(ISignal signal, ITransition transition) {
            if (signal == null || transition == null) {
                return false;
            }

            return (signal as Signal<S, T, G>).AddTransition(transition as Transition<S, T, G>) && (transition as Transition<S, T, G>).AddSignal(signal as Signal<S, T, G>);
        }

        public ISignal CreateSignal(G signalName, ITransition transition) {
            if (transition == null) {
                return null;
            }

            Signal<S, T, G> signal = new Signal<S, T, G>(this, signalName);
            signal.AddTransition(transition as Transition<S, T, G>);
            (transition as Transition<S, T, G>).AddSignal(signal);
            signals.Add(signal.Name, signal);

            return signal;
        }
        #endregion

        #region Set Initial State
        public bool SetInitialState(S stateName) {
            if (!states.ContainsKey(stateName)) {
                return false;
            }

            State<S, T, G> state = states[stateName];
            return SetInitialState(state);
        }

        public bool SetInitialState(IState state) {
            if (InitialState != null || CurrentState != null || Initialized || state == null) {
                return false;
            }

            InitialState = state as State<S, T, G>;
            return true;
        }

        public bool SetInitialState(S innerStateName, S stateName, int orthogonalIndex = 0) {
            if (!states.ContainsKey(stateName)) {
                return false;
            }

            State<S, T, G> state = states[stateName];
            return SetInitialState(innerStateName, state, orthogonalIndex);
        }

        public bool SetInitialState(S innerStateName, IState state, int orthogonalIndex = 0) {
            if (!states.ContainsKey(innerStateName)) {
                return false;
            }

            State<S, T, G> innerState = states[innerStateName];
            return SetInitialState(innerState, state, orthogonalIndex);
        }

        public bool SetInitialState(IState innerState, IState state, int orthogonalIndex = 0) {
            if (innerState == null || state == null) {
                return false;
            }

            // Set state's currentState to innerState
            (state as State<S, T, G>).SetInitialInnerState(innerState as State<S, T, G>, orthogonalIndex);
            return true;
        }
        #endregion

        #region Create Condition
        public bool CreateEmitCondition(G signalName, params S[] conditionalStateNames) {
            if (!signals.ContainsKey(signalName)) {
                return false;
            }

            Signal<S, T, G> signal = signals[signalName];
            return CreateEmitCondition(signal, conditionalStateNames);
        }

        public bool CreateEmitCondition(ISignal signal, params S[] conditionalStateNames) {
            if (signal == null || !signals.ContainsKey((signal as Signal<S, T, G>).Name)) {
                return false;
            }

            State<S, T, G>[] conditionalStates = new State<S, T, G>[conditionalStateNames.Length];
            for (int i = 0; i < conditionalStateNames.Length; i++) {
                if (!states.ContainsKey(conditionalStateNames[i])) {
                    return false;
                } else {
                    conditionalStates[i] = states[conditionalStateNames[i]];
                }
            }

            return CreateEmitCondition(signal, conditionalStates);
        }

        public bool CreateEmitCondition(ISignal signal, params IState[] conditionalStates) {
            if (signal == null || !signals.ContainsKey((signal as Signal<S, T, G>).Name)) {
                return false;
            }
            foreach (State<S, T, G> conditionalState in conditionalStates) {
                if (conditionalState == null || !states.ContainsKey(conditionalState.Name)) {
                    return false;
                }
            }

            SignalCondition<S, T, G> signalCondition = new SignalCondition<S, T, G>();
            signalCondition.AddCondition(conditionalStates);
            (signal as Signal<S, T, G>).AddEmitCondition(signalCondition);
            return true;
        }

        public bool CreateTransitionCondition(G signalName, T transitionName, params S[] conditionalStateNames) {
            if (!signals.ContainsKey(signalName) || !transitions.ContainsKey(transitionName)) {
                return false;
            }

            Signal<S, T, G> signal = signals[signalName];
            Transition<S, T, G> transition = transitions[transitionName];
            return CreateTransitionCondition(signal, transition, conditionalStateNames);
        }

        public bool CreateTransitionCondition(ISignal signal, T transitionName, params S[] conditionalStateNames) {
            if (signal == null || !signals.ContainsKey((signal as ISignal<G>).Name) || !transitions.ContainsKey(transitionName)) {
                return false;
            }

            Transition<S, T, G> transition = transitions[transitionName];
            return CreateTransitionCondition(signal, transition, conditionalStateNames);
        }

        public bool CreateTransitionCondition(ISignal signal, T transitionName, params IState[] conditionalStates) {
            if (signal == null || !signals.ContainsKey((signal as ISignal<G>).Name) || !transitions.ContainsKey(transitionName)) {
                return false;
            }

            Transition<S, T, G> transition = transitions[transitionName];
            return CreateTransitionCondition(signal, transition, conditionalStates);
        }

        public bool CreateTransitionCondition(ISignal signal, ITransition transition, params S[] conditionalStateNames) {
            if (signal == null || !signals.ContainsKey((signal as ISignal<G>).Name) || transition == null || !transitions.ContainsKey((transition as ITransition<T>).Name)) {
                return false;
            }

            State<S, T, G>[] conditionalStates = new State<S, T, G>[conditionalStateNames.Length];
            for (int i = 0; i < conditionalStateNames.Length; i++) {
                if (!states.ContainsKey(conditionalStateNames[i])) {
                    return false;
                } else {
                    conditionalStates[i] = states[conditionalStateNames[i]];
                }
            }

            return CreateTransitionCondition(signal, transition, conditionalStates);
        }

        public bool CreateTransitionCondition(ISignal signal, ITransition transition, params IState[] conditionalStates) {
            if (signal == null || !signals.ContainsKey((signal as ISignal<G>).Name) || transition == null || !transitions.ContainsKey((transition as ITransition<T>).Name)) {
                return false;
            }
            foreach (State<S, T, G> conditionalState in conditionalStates) {
                if (conditionalState == null || !states.ContainsKey(conditionalState.Name)) {
                    return false;
                }
            }

            SignalCondition<S, T, G> signalCondition = new SignalCondition<S, T, G>();
            signalCondition.AddCondition(conditionalStates);
            (signal as Signal<S, T, G>).AddTransitionCondition(signalCondition, transition as Transition<S, T, G>);
            return true;
        }
        #endregion

        public void Initialize() {
            if (!Initialized && CurrentState == null && InitialState != null) {
                Initialized = true;
                CurrentState = InitialState;
                CurrentState.EnterState();
            }
        }

        public void Terminate() {
            Initialized = false;
            CurrentState.Terminate();
            CurrentState = null;
        }

        public IState GetStateByName(S stateName) {
            return states[stateName];
        }

        public ITransition GetTransitionByName(T transitionName) {
            return transitions[transitionName];
        }

        public ISignal GetSignalByName(G signalName) {
            return signals[signalName];
        }

        public List<S> GetAllActiveStateNames() {
            return states.Values.Where(s => s.IsCurrentState).Select(s => s.Name).ToList();
        }

        public List<string> GetAllActiveStateNamesAsString() {
            return states.Values.Where(s => s.IsCurrentState).Select(s => s.Name.ToString()).ToList();
        }

        public List<IState> GetAllActiveStates() {
            return states.Values.Where(s => s.IsCurrentState).ToList<IState>();
        }

        internal void OnChildStateChanged(IState priorState, IState formerState) {
            if (OnStateChanged != null) {
                OnStateChanged.Invoke(priorState, formerState);
            }
        }

        internal void EmitSignal(G signalName) {
            if (!signals.ContainsKey(signalName)) {
                return;
            }

            Signal<S, T, G> signal = signals[signalName];
            ProcessSignal(signal);
        }

        internal bool ProcessSignal(Signal<S, T, G> signal) {
            // maybe check transition ambiguity... (If two or more transitions are available from the current state)
            foreach (Transition<S, T, G> transition in signal.SignalTo) {
                if (CurrentState.StateTransitions.ContainsKey(transition) && transition.CanTransition) {
                    CurrentState.LeaveState();
                    bool continueTransition = transition.StartTransition();
                    if (!continueTransition) {
                        CurrentState.EnterState();
                        return false;
                    }

                    transition.EndTransition();
                    CurrentState = transition.EndState;
                    CurrentState.EnterState();

                    if (OnStateChanged != null) {
                        OnStateChanged.Invoke(transition.StartState, transition.EndState);
                    }
                    return true;
                }
            }

            return CurrentState.PassSignal(signal);
        }
    }

    // This is just for easy usage if you want to use string as Name.
    public class StateMachine : StateMachine<string, string, string> { }
}
