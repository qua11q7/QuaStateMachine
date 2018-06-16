using QuaStateMachine;
using QuaStateMachine.Creator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.ChainCreation {
    internal class StopwatchChainDemo {
        StateMachine<States, Transitions, Signals> smStopwatch;
        ChainCreator<States, Transitions, Signals> creator;

        Stopwatch stopwatch;

        ISignal sigReset, sigStartStop;

        public StopwatchChainDemo() {
            Initialize();
        }

        void Initialize() {
            smStopwatch = new StateMachine<States, Transitions, Signals>();
            creator = new ChainCreator<States, Transitions, Signals>(smStopwatch);

            creator
                .CreateState(States.Active)
                .OnStateEnter(SActive_OnStateEnter)
                .OnStateLeave(SActive_OnStateLeave)
                .CreateInnerState(States.Stopped, States.Active)
                .OnStateEnter(SStopped_OnStateEnter)
                .OnStateLeave(SStopped_OnStateLeave)
                .CreateInnerState(States.Running, States.Active)
                .OnStateEnter(SRunning_OnStateEnter)
                .OnStateLeave(SRunning_OnStateLeave)
                .CreateTransition(Transitions.ActiveLoop, States.Active, States.Active)
                .CreateTransition(Transitions.Stopped2Runnig, States.Stopped, States.Running)
                .CreateTransition(Transitions.Running2Stopped, States.Running, States.Stopped)
                .CreateSignal(Signals.Reset, Transitions.ActiveLoop, out sigReset)
                .CreateSignal(Signals.StartStop, Transitions.Stopped2Runnig, out sigStartStop)
                .ConnectSignal(Signals.StartStop, Transitions.Running2Stopped)
                .SetInitialState(States.Active)
                .SetInitialInnerState(States.Stopped, States.Active);
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
