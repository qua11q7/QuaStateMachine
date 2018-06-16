using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.TransitionlessDemo {
    internal class CameraTransitionlessDemo {
        StateMachine<States, Signals> SM;

        ISignal sigConfig, sigHalfPressed, sigReleased;

        public CameraTransitionlessDemo() {
            Initialize();
        }

        private void Initialize() {
            SM = new StateMachine<States, Signals>();

            SM.CreateState(States.NotShooting);
            SM.CreateState(States.Shooting);
            SM.CreateState(States.Idle, States.NotShooting);
            SM.CreateState(States.Configuring, States.NotShooting);

            // We can still get the transition objects if we want.
            SM.ConnectSignal(Signals.Config, States.Idle, States.Configuring, out ITransition transition1);
            // We can also get both the signal and transition objects.
            SM.ConnectSignal(Signals.Config, States.Configuring, States.Idle, out sigConfig, out ITransition transition2);
            SM.ConnectSignal(Signals.HalfPressed, States.NotShooting, States.Shooting, out sigHalfPressed);
            SM.ConnectSignal(Signals.Released, States.Shooting, States.NotShooting, out sigReleased);

            SM.SetInitialState(States.NotShooting);
            SM.SetInitialState(States.Idle, States.NotShooting);
        }

        public void Start() {
            SM.Initialize();

            Console.WriteLine("Camera Transitionless Demo Started\r\n");
            Console.WriteLine(SM.GetAllActiveStateNamesAsString().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                switch (input) {
                    case "1":
                        sigConfig.Emit();
                        break;
                    case "2":
                        sigHalfPressed.Emit();
                        break;
                    case "3":
                        sigReleased.Emit();
                        break;
                    default:
                        continueDemo = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(SM.GetAllActiveStateNamesAsString().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            SM.Terminate();

            Console.WriteLine("\r\nCamera Transitionless Demo finished");
        }

        public enum States {
            NotShooting,
            Shooting,
            Idle,
            Configuring
        }

        public enum Signals {
            Config,
            HalfPressed,
            Released
        }
    }
}
