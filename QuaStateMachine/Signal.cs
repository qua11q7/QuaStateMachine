using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    internal sealed class Signal<S, T, G> : ISignal<G> {
        public G Name { get; private set; }
        internal List<Transition<S, T, G>> SignalTo { get; private set; }
        internal List<SignalCondition<S, T, G>> SignalEmitConditions { get; private set; }
        internal Dictionary<SignalCondition<S, T, G>, Transition<S, T, G>> SignalTransitionConditions { get; private set; }
        private StateMachine<S, T, G> stateMachine;

        public event SignalNotProcessed OnSignalNotProcessed;

        internal Signal(StateMachine<S, T, G> sMachine, G signalName) {
            stateMachine = sMachine;
            Name = signalName;
            SignalTo = new List<Transition<S, T, G>>();
            SignalEmitConditions = new List<SignalCondition<S, T, G>>();
            SignalTransitionConditions = new Dictionary<SignalCondition<S, T, G>, Transition<S, T, G>>();
        }

        internal bool AddTransition(Transition<S, T, G> transition) {
            if (SignalTo.Exists(t => t.Name.Equals(transition.Name))) {
                return false;
            }

            SignalTo.Add(transition);
            return true;
        }

        internal void AddEmitCondition(SignalCondition<S, T, G> condition) {
            SignalEmitConditions.Add(condition);
        }

        internal void AddTransitionCondition(SignalCondition<S, T, G> condition, Transition<S, T, G> transition) {
            if (condition == null || transition == null || !SignalTo.Exists(t => t.Name.Equals(transition.Name))) {
                return;
            }

            SignalTransitionConditions.Add(condition, transition);
        }

        public bool Emit() {
            #region Emit Conditions Check
            // check emit conditions, it is enough to pass if one of the conditions is met. This allows to make OR logical comparisons between SignalEmitConditions.
            bool emitConditionMet = SignalEmitConditions.Count > 0 ? false : true;
            foreach (ISignalCondition signalCondition in SignalEmitConditions) {
                if (signalCondition.IsValid) {
                    emitConditionMet = true;
                }
            }
            if (!emitConditionMet) {
                SignalNotProcessedEventArgs eventArgs = new SignalNotProcessedEventArgs(SignalFailure.EmitConditionsNotMet, SignalEmitConditions.ToList<ISignalCondition>());
                if (OnSignalNotProcessed != null) {
                    OnSignalNotProcessed.Invoke(eventArgs);
                }
                return false;
            }
            #endregion

            #region Transition Conditions Check
            // check transition conditions, there must be only one valid transition. If more than one, stop emitting the signal, otherwise this might cause undefined behaviour.
            int conditionMetCount = SignalTransitionConditions.Count != 0 ? 0 : 1;
            foreach (KeyValuePair<SignalCondition<S, T, G>, Transition<S, T, G>> pair in SignalTransitionConditions) {
                if (pair.Key.IsValid) {
                    pair.Value.CanTransition = true;
                    conditionMetCount++;
                } else {
                    pair.Value.CanTransition = false;
                }
            }
            if (conditionMetCount == 0) {
                SignalNotProcessedEventArgs eventArgs = new SignalNotProcessedEventArgs(SignalFailure.TransitionConditionsNotMet, SignalTransitionConditions.Keys.ToList<ISignalCondition>());
                if (OnSignalNotProcessed != null) {
                    OnSignalNotProcessed.Invoke(eventArgs);
                }
                return false;
            } else if (conditionMetCount > 1) {
                SignalNotProcessedEventArgs eventArgs = new SignalNotProcessedEventArgs(SignalFailure.TransitionAmbiguity, SignalTransitionConditions.Keys.ToList<ISignalCondition>());
                if (OnSignalNotProcessed != null) {
                    OnSignalNotProcessed.Invoke(eventArgs);
                }
                return false;
            }
            #endregion

            #region Process Signal
            if (!stateMachine.ProcessSignal(this)) {
                SignalNotProcessedEventArgs eventArgs = new SignalNotProcessedEventArgs(SignalFailure.NoTransitionToState);
                if (OnSignalNotProcessed != null) {
                    OnSignalNotProcessed.Invoke(eventArgs);
                }
                return false;
            }
            return true;
            #endregion
        }

        public override string ToString() {
            return Name.ToString();
        }
    }
}
