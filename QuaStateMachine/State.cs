using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    internal sealed class State<S, T, G> : IState<S> {
        public S Name { get; private set; }
        public bool HasInnerState { get; private set; }
        public bool IsCurrentState { get; private set; }
        public bool HasParentState { get; internal set; }
        public State<S, T, G> ParentState { get; internal set; }
        internal StateMachine<S, T, G> SM { get; private set; }

        internal Dictionary<Transition<S, T, G>, State<S, T, G>> StateTransitions { get; private set; }
        internal Dictionary<int, Orthogonal<S, T, G>> Orthogonals { get; private set; }

        public event StateMachineDelegate OnStateEnter;
        public event StateMachineDelegate OnStateLeave;
        public event StateMachineDelegate OnStateTerminated;

        internal State(S name) {
            Name = name;
            StateTransitions = new Dictionary<Transition<S, T, G>, State<S, T, G>>();
            Orthogonals = new Dictionary<int, Orthogonal<S, T, G>>();
            Orthogonals.Add(0, new Orthogonal<S, T, G>(this, 0));
        }

        internal bool AddTransition(Transition<S, T, G> transition, State<S, T, G> state) {
            if (StateTransitions.ContainsKey(transition)) {
                return false;
            }

            StateTransitions.Add(transition, state);
            return true;
        }

        internal bool AddOrthogonal(int index) {
            if (Orthogonals.Count == index) {
                Orthogonals.Add(index, new Orthogonal<S, T, G>(this, index));
                return true;
            }

            return false;
        }

        internal bool AddInnerState(State<S, T, G> innerState, int index = 0) {
            if (!Orthogonals.ContainsKey(index)) {
                if (!AddOrthogonal(index)) {
                    return false;
                }
            }

            HasInnerState = true;
            innerState.HasParentState = true;
            innerState.ParentState = this;
            return Orthogonals[index].SM.AddState(innerState);
        }

        internal bool SetInitialInnerState(State<S, T, G> innerState, int index = 0) {
            if (!HasInnerState) {
                return false;
            }

            if (!Orthogonals.ContainsKey(index)) {
                if (!AddOrthogonal(index)) {
                    return false;
                }
            }

            return Orthogonals[index].SM.SetInitialState(innerState);
        }

        internal bool SetStateMachine(StateMachine<S, T, G> sm) {
            if (SM != null || sm == null)
                return false;

            SM = sm;
            foreach (Orthogonal<S, T, G> ort in Orthogonals.Values) {
                ort.ConnectStateChangedEvent();
            }

            return true;
        }

        internal void EnterState() {
            IsCurrentState = true;
            if (OnStateEnter != null) {
                OnStateEnter.Invoke();
            }

            if (HasInnerState) {
                foreach (Orthogonal<S, T, G> ort in Orthogonals.Values) {
                    ort.SM.Initialize();
                }
            }
        }

        internal void LeaveState() {
            IsCurrentState = false;
            if (OnStateLeave != null) {
                OnStateLeave.Invoke();
            }

            if (HasInnerState) {
                foreach (Orthogonal<S, T, G> ort in Orthogonals.Values) {
                    ort.SM.Terminate();
                }
            }
        }

        internal void Terminate() {
            if (HasInnerState) {
                foreach (Orthogonal<S, T, G> ort in Orthogonals.Values) {
                    ort.SM.Terminate();
                }
            }

            IsCurrentState = false;
            if (OnStateTerminated != null) {
                OnStateTerminated.Invoke();
            }
        }

        internal bool PassSignal(Signal<S, T, G> signal) {
            bool signalProcessed = false;
            if (HasInnerState) {
                foreach (Orthogonal<S, T, G> ort in Orthogonals.Values) {
                    if (ort.SM.ProcessSignal(signal)) {
                        signalProcessed = true;
                    }
                }
            }

            return signalProcessed;
        }

        public override string ToString() {
            return Name.ToString();
        }
    }
}
