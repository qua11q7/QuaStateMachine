using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.Demo {
    internal class CameraDemo {
        StateMachine smCamera;
        ISignal sigConfig;
        ISignal sigHalfPressed;
        ISignal sigReleased;

        public CameraDemo() {
            Initialize();
        }

        void Initialize() {
            smCamera = new StateMachine();

            IState sNotShooting;
            IState sShooting;
            IState sIdle;
            IState sConfiguring;

            smCamera.TryCreateState("sNotShooting", out sNotShooting);
            smCamera.TryCreateState("sShooting", out sShooting);
            smCamera.TryCreateState("sIdle", sNotShooting, out sIdle);
            smCamera.TryCreateState("sConfiguring", sNotShooting, out sConfiguring);

            ITransition t1;
            ITransition t2;
            ITransition t3;
            ITransition t4;

            smCamera.TryCreateTransition("sNotShooting to sShooting", sNotShooting, sShooting, out t1);
            smCamera.TryCreateTransition("sShooting to sNotShooting", sShooting, sNotShooting, out t2);
            smCamera.TryCreateTransition("sIdle to sConfiguring", sIdle, sConfiguring, out t3);
            smCamera.TryCreateTransition("sConfiguring to sIdle", sConfiguring, sIdle, out t4);

            smCamera.ConnectSignal("sigHalfPressed", t1, out sigHalfPressed);
            smCamera.ConnectSignal("sigReleased", t2, out sigReleased);
            smCamera.ConnectSignal("sigConfig", t3, out sigConfig);
            smCamera.ConnectSignal(sigConfig, t4);

            smCamera.SetInitialState(sNotShooting);
            smCamera.SetInitialState(sIdle, sNotShooting);

            sNotShooting.OnStateEnter += SNotShooting_OnStateEnter;
            sNotShooting.OnStateLeave += SNotShooting_OnStateLeave;
            sShooting.OnStateEnter += SShooting_OnStateEnter;
            sShooting.OnStateLeave += SShooting_OnStateLeave;
            sIdle.OnStateEnter += SIdle_OnStateEnter;
            sIdle.OnStateLeave += SIdle_OnStateLeave;
            sConfiguring.OnStateEnter += SConfiguring_OnStateEnter;
            sConfiguring.OnStateLeave += SConfiguring_OnStateLeave;

            t1.OnTransitionStart += T1_OnTransitionStart;
            t1.OnTransitionFinish += T1_OnTransitionFinish;
            t2.OnTransitionStart += T2_OnTransitionStart;
            t2.OnTransitionFinish += T2_OnTransitionFinish;
            t3.OnTransitionStart += T3_OnTransitionStart;
            t3.OnTransitionFinish += T3_OnTransitionFinish;
            t4.OnTransitionStart += T4_OnTransitionStart;
            t4.OnTransitionFinish += T4_OnTransitionFinish;
        }

        public void Start() {
            smCamera.Initialize();

            Console.WriteLine("Camera Demo Started\r\n");
            Console.WriteLine(smCamera.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
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
                Console.WriteLine(smCamera.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            smCamera.Terminate();

            Console.WriteLine("\r\nCamera Demo finished");
        }

        private void T4_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sCONFIGURING to sIDLE");
        }

        private void T4_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sCONFIGURING to sIDLE");
        }

        private void T3_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sIDLE to sCONFIGURING");
        }

        private void T3_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sIDLE to sCONFIGURING");
        }

        private void T2_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sSHOOTING to sNOTSHOOTING");
        }

        private void T2_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sSHOOTING to sNOTSHOOTING");
        }

        private void T1_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sNOTSHOOTING to sSHOOTING");
        }

        private void T1_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sNOTSHOOTING to sSHOOTING");
        }

        private void SConfiguring_OnStateLeave() {
            Console.WriteLine("Leaving sCONFIGURING...");
        }

        private void SConfiguring_OnStateEnter() {
            Console.WriteLine("Entering sCONFIGURING...");
        }

        private void SIdle_OnStateLeave() {
            Console.WriteLine("Leaving sIDLE...");
        }

        private void SIdle_OnStateEnter() {
            Console.WriteLine("Entering sIDLE...");
        }

        private void SShooting_OnStateLeave() {
            Console.WriteLine("Leaving sSHOOTING...");
        }

        private void SShooting_OnStateEnter() {
            Console.WriteLine("Entering sSHOOTING...");
        }

        private void SNotShooting_OnStateLeave() {
            Console.WriteLine("Leaving sNOTSHOOTING...");
        }

        private void SNotShooting_OnStateEnter() {
            Console.WriteLine("Entering sNOTSHOOTING...");
        }
    }
}
