using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.TransitionlessDemo {
    internal class KeyboardTransitionlessDemo {
        StateMachine<States, Signals> SM;

        public KeyboardTransitionlessDemo() {
            Initialize();
        }

        private void Initialize() {
            SM = new StateMachine<States, Signals>();

            SM.CreateState(States.Active);
            SM.CreateState(States.NumLockOff, States.Active);
            SM.CreateState(States.NumLockOn, States.Active);
            SM.CreateState(States.ScrollLockOff, States.Active, 1);
            SM.CreateState(States.ScrollLockOn, States.Active, 1);
            SM.CreateState(States.CapsLockOff, States.Active, 2);
            SM.CreateState(States.CapsLockOn, States.Active, 2);

            SM.ConnectSignal(Signals.NumLock, States.NumLockOff, States.NumLockOn);
            SM.ConnectSignal(Signals.NumLock, States.NumLockOn, States.NumLockOff);
            SM.ConnectSignal(Signals.ScrollLock, States.ScrollLockOff, States.ScrollLockOn);
            SM.ConnectSignal(Signals.ScrollLock, States.ScrollLockOn, States.ScrollLockOff);
            SM.ConnectSignal(Signals.CapsLock, States.CapsLockOff, States.CapsLockOn);
            SM.ConnectSignal(Signals.CapsLock, States.CapsLockOn, States.CapsLockOff);

            SM.SetInitialState(States.Active);
            SM.SetInitialState(States.NumLockOff, States.Active);
            SM.SetInitialState(States.ScrollLockOff, States.Active, 1);
            SM.SetInitialState(States.CapsLockOff, States.Active, 2);

            SM.OnStateChangedGeneric += SM_OnStateChanged;
        }

        private void SM_OnStateChanged(IState<States> priorState, IState<States> formerState) {
            Console.WriteLine(priorState.Name + " --> " + formerState.Name);
        }

        public void Start() {
            SM.Initialize();

            Console.WriteLine("Keyboard Transitionless Demo Started\r\n");
            Console.WriteLine("Active States: " + SM.GetAllActiveStateNamesAsString().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                switch (input) {
                    case "1":
                        SM.GetSignalByName(Signals.NumLock).Emit();
                        break;
                    case "2":
                        SM.GetSignalByName(Signals.ScrollLock).Emit();
                        break;
                    case "3":
                        SM.GetSignalByName(Signals.CapsLock).Emit();
                        break;
                    default:
                        continueDemo = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Active States: " + SM.GetAllActiveStateNamesAsString().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            SM.Terminate();

            Console.WriteLine("\r\nKeyboard Transitionless Demo finished");
        }

        public enum States {
            Active,
            NumLockOff,
            NumLockOn,
            ScrollLockOff,
            ScrollLockOn,
            CapsLockOn,
            CapsLockOff
        }

        public enum Signals {
            NumLock,
            ScrollLock,
            CapsLock
        }
    }
}
