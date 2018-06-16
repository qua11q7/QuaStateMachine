using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    internal class SignalCondition<S, T, G> : ISignalCondition<S> {
        internal List<State<S, T, G>> Conditions { get; private set; }
        List<IState<S>> ISignalCondition<S>.Conditions { get { return Conditions.ToList<IState<S>>(); } }
        List<IState> ISignalCondition.Conditions { get { return Conditions.ToList<IState>(); } }
        public bool IsValid { get { return Validate(); } }

        internal SignalCondition() {
            Conditions = new List<State<S, T, G>>();
        }

        internal void AddCondition(params IState[] conditions) {
            foreach (State<S, T, G> condition in conditions) {
                if (condition != null && !Conditions.Exists(c => c.Name.Equals(condition.Name))) {
                    Conditions.Add(condition);
                }
            }
        }

        private bool Validate() {
            return Conditions.All(c => c.IsCurrentState);
        }
    }
}
