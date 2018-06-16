using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachine {
    internal class InvalidTransitionException<S, T> : Exception {
        private string message;
        public override string Message {
            get {
                return message;
            }
        }

        internal InvalidTransitionException(S sourceStateName, S targetStateName, T transitionName) {
            message = "Transition between different state levels are not allowed.\r\nSource State: " + sourceStateName + "\r\nTarget State: " + targetStateName + "\r\nTransition Name: " + transitionName;
        }

        internal InvalidTransitionException() { }

        internal InvalidTransitionException(string message)
            : base(message) { }
    }
}
