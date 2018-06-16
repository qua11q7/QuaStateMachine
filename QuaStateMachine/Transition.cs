using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    internal sealed class Transition<S, T, G> : ITransition<T> {
        public T Name { get; private set; }
        internal List<Signal<S, T, G>> TransitionSignals { get; private set; }
        internal State<S, T, G> StartState { get; private set; }
        internal State<S, T, G> EndState { get; private set; }
        internal bool CanTransition { get; set; }

        public event TransitionStart OnTransitionStart;
        public event StateMachineDelegate OnTransitionFinish;

        internal Transition(T name) {
            Name = name;
            CanTransition = true;
            TransitionSignals = new List<Signal<S, T, G>>();
        }

        internal Transition(T name, State<S, T, G> s1, State<S, T, G> s2) {
            Name = name;
            CanTransition = true;
            TransitionSignals = new List<Signal<S, T, G>>();
            SetTransition(s1, s2);
        }

        internal bool SetTransition(State<S, T, G> s1, State<S, T, G> s2) {
            if (StartState == null && EndState == null) {
                StartState = s1;
                EndState = s2;
                return true;
            }

            return false;
        }

        internal bool AddSignal(Signal<S, T, G> signal) {
            if (TransitionSignals.Exists(ts => ts.Name.Equals(signal.Name))) {
                return false;
            }

            TransitionSignals.Add(signal);
            return true;
        }

        internal bool StartTransition() {
            if (OnTransitionStart != null) {
                TransitionEventArgs args = new TransitionEventArgs();
                OnTransitionStart.Invoke(this, args);

                if (args.CancelTransition) {
                    return false;
                }
            }

            return true;
        }

        internal void EndTransition() {
            if (OnTransitionFinish != null) {
                OnTransitionFinish.Invoke();
            }
        }

        public override string ToString() {
            return Name.ToString();
        }
    }
}
