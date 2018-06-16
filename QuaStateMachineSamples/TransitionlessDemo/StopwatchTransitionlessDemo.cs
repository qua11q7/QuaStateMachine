using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.TransitionlessDemo {
    internal class StopwatchTransitionlessDemo {
        StateMachine<States, Signals> SM;

        Stopwatch stopwatch;

        public StopwatchTransitionlessDemo() {
            Initialize();
        }

        private void Initialize() {
            SM = new StateMachine<States, Signals>();
            
            // Get the active state object to subscribe to OnStateEnter and OnStateLeave events.
            IState activeState = SM.CreateState(States.Active);
            // We want to only subscribe to OnStateEnter event of Stopped state. We can write an anonymous function such as:
            SM.CreateState(States.Stopped, States.Active).OnStateEnter += () => { stopwatch?.Stop(); Console.WriteLine("Elapsed time: " + stopwatch?.ElapsedMilliseconds.ToString()); };
            // We also only want to listen OnStateEnter event of Running state. 
            SM.CreateState(States.Running, States.Active).OnStateEnter += () => { stopwatch?.Start(); };
            
            // Subscribe to activeState's event with anonymous functions.
            activeState.OnStateEnter += () => { stopwatch = stopwatch ?? new Stopwatch(); };
            activeState.OnStateLeave += () => { stopwatch?.Reset(); };

            // We do not need to get the signal objects if we don't want to. We can emit the signal with the Signals enum.
            SM.ConnectSignal(Signals.Reset, States.Active, States.Active, out ISignal resetSignal);
            SM.ConnectSignal(Signals.StartStop, States.Stopped, States.Running, out ISignal startStopSignal);
            SM.ConnectSignal(Signals.StartStop, States.Running, States.Stopped);

            SM.SetInitialState(States.Active);
            SM.SetInitialState(States.Stopped, States.Active);

            SM.OnStateChanged += SM_OnStateChanged;
        }

        private void SM_OnStateChanged(IState priorState, IState formerState) {
            Console.WriteLine((priorState as IState<States>).Name + " --> " + (formerState as IState<States>).Name);

            // All logic can be implemented here as well. We just need to watch prior and former states.
        }

        public void EmitSignal(Signals signal) {
            SM.GetSignalByName(signal).Emit();
        }

        public void Start() {
            SM.Initialize();

            Console.WriteLine("Stopwatch Transitionless Demo Started\r\n");
            Console.WriteLine("Active States: " + SM.GetAllActiveStateNamesAsString().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                Signals signal = Signals.StartStop;
                int index = 0;
                if (int.TryParse(input, out index)) {
                    if (index >= Enum.GetNames(typeof(Signals)).Length) {
                        continueDemo = false;
                    } else {
                        signal = (Signals)index;
                        EmitSignal(signal);
                    }
                } else {
                    continueDemo = false;
                }

                Console.WriteLine();
                Console.WriteLine("Active States: " + SM.GetAllActiveStateNamesAsString().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            SM.Terminate();

            Console.WriteLine("\r\nStopwatch Transitionless Demo finished");
        }

        public enum States {
            Active,
            Stopped,
            Running
        }

        public enum Signals {
            Reset,
            StartStop
        }
    }
}
