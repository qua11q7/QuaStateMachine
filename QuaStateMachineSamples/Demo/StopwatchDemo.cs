using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.Demo {
    internal class StopwatchDemo {
        StateMachine smStopwatch;
        ISignal sigReset;
        ISignal sigStartStop;

        Stopwatch stopwatch;

        public StopwatchDemo() {
            Initialize();
        }

        void Initialize() {
            smStopwatch = new StateMachine();

            IState sActive;
            IState sStopped;
            IState sRunning;

            smStopwatch.TryCreateState("sActive", out sActive);
            smStopwatch.TryCreateState("sStopped", sActive, out sStopped);
            smStopwatch.TryCreateState("sRunning", sActive, out sRunning);

            smStopwatch.CreateTransition("sActive loop", sActive, sActive);
            smStopwatch.CreateTransition("sStopped to sRunning", sStopped, sRunning);
            smStopwatch.CreateTransition("sRunning to sStopped", sRunning, sStopped);

            smStopwatch.ConnectSignal("sigReset", "sActive loop", out sigReset);
            smStopwatch.ConnectSignal("sigStartStop", "sStopped to sRunning", out sigStartStop);
            smStopwatch.ConnectSignal(sigStartStop, "sRunning to sStopped");

            smStopwatch.SetInitialState(sActive);
            smStopwatch.SetInitialState(sStopped, sActive);

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
            Console.WriteLine(smStopwatch.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
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
                Console.WriteLine(smStopwatch.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
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
    }
}
