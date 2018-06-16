using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    internal class Orthogonal<S, T, G> {
        internal int Index { get; private set; }
        internal State<S, T, G> CurrentState { get { return SM.CurrentState; } }
        internal State<S, T, G> OrthogonalOf { get; private set; }
        internal StateMachine<S, T, G> SM { get; private set; }

        internal Orthogonal(State<S, T, G> orthogonalOf, int index) {
            Index = index;
            OrthogonalOf = orthogonalOf;
            SM = new StateMachine<S, T, G>();

            ConnectStateChangedEvent();
        }

        internal void ConnectStateChangedEvent() {
            if (OrthogonalOf.SM != null)
                SM.OnStateChanged += OrthogonalOf.SM.FireOnStateChanged;
        }
    }
}
