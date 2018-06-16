using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.GenericDemo {
    internal class StopwatchGenericDemo {
        StateMachine<States, Transitions, Signals> smStopwatch;
        ISignal sigReset;
        ISignal sigStartStop;

        Stopwatch stopwatch;

        public StopwatchGenericDemo() {
            Initialize();
        }

        void Initialize() {
            smStopwatch = new StateMachine<States, Transitions, Signals>();

            IState sActive;
            IState sStopped;
            IState sRunning;

            smStopwatch.TryCreateState(States.Active, out sActive);
            smStopwatch.TryCreateState(States.Stopped, States.Active, out sStopped);
            smStopwatch.TryCreateState(States.Running, States.Active, out sRunning);

            smStopwatch.CreateTransition(Transitions.ActiveLoop, States.Active, States.Active);
            smStopwatch.CreateTransition(Transitions.Stopped2Runnig, States.Stopped, States.Running);
            smStopwatch.CreateTransition(Transitions.Running2Stopped, States.Running, States.Stopped);

            smStopwatch.ConnectSignal(Signals.Reset, Transitions.ActiveLoop, out sigReset);
            smStopwatch.ConnectSignal(Signals.StartStop, Transitions.Stopped2Runnig, out sigStartStop);
            smStopwatch.ConnectSignal(Signals.StartStop, Transitions.Running2Stopped);

            smStopwatch.SetInitialState(States.Active);
            smStopwatch.SetInitialState(States.Stopped, States.Active);

            sActive.OnStateEnter += SActive_OnStateEnter;
            sActive.OnStateLeave += SActive_OnStateLeave;
            sStopped.OnStateEnter += SStopped_OnStateEnter;
            sStopped.OnStateLeave += SStopped_OnStateLeave;
            sRunning.OnStateEnter += SRunning_OnStateEnter;
            sRunning.OnStateLeave += SRunning_OnStateLeave;
        }

        public void Start() {
            smStopwatch.Initialize();

            Console.WriteLine("Stopwatch Demo Started\r\n");
            Console.WriteLine(smStopwatch.GetAllActiveStateNamesAsString().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                switch (input) {
                    case "1":
                        sigReset.Emit();
                        break;
                    case "2":
                        sigStartStop.Emit();
                        break;
                    default:
                        continueDemo = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(smStopwatch.GetAllActiveStateNamesAsString().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            smStopwatch.Terminate();

            Console.WriteLine("\r\nStopwatch Demo finished");
        }

        private void SRunning_OnStateLeave() {
            Console.WriteLine("Leaving Running state...");
        }

        private void SRunning_OnStateEnter() {
            Console.WriteLine("Entering Running state...");
            stopwatch.Start();
        }

        private void SStopped_OnStateLeave() {
            Console.WriteLine("Leaving Stopped state...");
        }

        private void SStopped_OnStateEnter() {
            Console.WriteLine("Entering Stopped state...");
            stopwatch.Stop();
            Console.WriteLine("Elapsed time: " + stopwatch.ElapsedMilliseconds.ToString());
        }

        private void SActive_OnStateLeave() {
            Console.WriteLine("Leaving Active state...");
            stopwatch.Reset();
        }

        private void SActive_OnStateEnter() {
            Console.WriteLine("Entering Active state...");
            if (stopwatch == null)
                stopwatch = new Stopwatch();
        }

        enum States {
            Active,
            Stopped,
            Running
        }

        enum Transitions {
            ActiveLoop,
            Stopped2Runnig,
            Running2Stopped
        }

        enum Signals {
            Reset,
            StartStop
        }
    }
}
