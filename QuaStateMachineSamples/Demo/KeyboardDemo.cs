using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.Demo {
    internal class KeyboardDemo {
        StateMachine smKeyboard;
        ISignal sigNumLock;
        ISignal sigCapsLock;
        ISignal sigScrollLock;

        public KeyboardDemo() {
            Initialize();
        }

        void Initialize() {
            smKeyboard = new StateMachine();

            IState sActive;
            IState sNumLockOff;
            IState sNumLockOn;
            IState sCapsLockOff;
            IState sCapsLockOn;
            IState sScrollLockOff;
            IState sScrollLockOn;

            smKeyboard.TryCreateState("sActive", out sActive);
            smKeyboard.TryCreateState("sNumLockOff", sActive, out sNumLockOff);
            smKeyboard.TryCreateState("sNumLockOn", sActive, out sNumLockOn);
            smKeyboard.TryCreateState("sCapsLockOff", sActive, out sCapsLockOff, 1);
            smKeyboard.TryCreateState("sCapsLockOn", sActive, out sCapsLockOn, 1);
            smKeyboard.TryCreateState("sScrollLockOff", sActive, out sScrollLockOff, 2);
            smKeyboard.TryCreateState("sScrollLockOn", sActive, out sScrollLockOn, 2);

            ITransition t1NLOff2On;
            ITransition t2NLOn2Off;
            ITransition t3CLOff2On;
            ITransition t4CLOn2Off;
            ITransition t5SLOff2On;
            ITransition t6SLOn2Off;

            smKeyboard.TryCreateTransition("sNumLock Off to On", sNumLockOff, sNumLockOn, out t1NLOff2On);
            smKeyboard.TryCreateTransition("sNumLock On to Off", sNumLockOn, sNumLockOff, out t2NLOn2Off);
            smKeyboard.TryCreateTransition("sCapsLock Off to On", sCapsLockOff, sCapsLockOn, out t3CLOff2On);
            smKeyboard.TryCreateTransition("sCapsLock On to Off", sCapsLockOn, sCapsLockOff, out t4CLOn2Off);
            smKeyboard.TryCreateTransition("sScrollLock Off to On", sScrollLockOff, sScrollLockOn, out t5SLOff2On);
            smKeyboard.TryCreateTransition("sScrollLock On to Off", sScrollLockOn, sScrollLockOff, out t6SLOn2Off);

            smKeyboard.ConnectSignal("sigNumLock", t1NLOff2On, out sigNumLock);
            smKeyboard.ConnectSignal("sigCapsLock", t3CLOff2On, out sigCapsLock);
            smKeyboard.ConnectSignal("sigScrollLock", t5SLOff2On, out sigScrollLock);
            smKeyboard.ConnectSignal(sigNumLock, t2NLOn2Off);
            smKeyboard.ConnectSignal(sigCapsLock, t4CLOn2Off);
            smKeyboard.ConnectSignal(sigScrollLock, t6SLOn2Off);

            smKeyboard.SetInitialState(sActive);
            smKeyboard.SetInitialState(sNumLockOff, sActive);
            smKeyboard.SetInitialState(sCapsLockOff, sActive, 1);
            smKeyboard.SetInitialState(sScrollLockOff, sActive, 2);

            sActive.OnStateEnter += SActive_OnStateEnter1;
            sActive.OnStateLeave += SActive_OnStateLeave1;
            sNumLockOff.OnStateEnter += SNumLockOff_OnStateEnter;
            sNumLockOff.OnStateLeave += SNumLockOff_OnStateLeave;
            sNumLockOn.OnStateEnter += SNumLockOn_OnStateEnter;
            sNumLockOn.OnStateLeave += SNumLockOn_OnStateLeave;
            sCapsLockOff.OnStateEnter += SCapsLockOff_OnStateEnter;
            sCapsLockOff.OnStateLeave += SCapsLockOff_OnStateLeave;
            sCapsLockOn.OnStateEnter += SCapsLockOn_OnStateEnter;
            sCapsLockOn.OnStateLeave += SCapsLockOn_OnStateLeave;
            sScrollLockOff.OnStateEnter += SScrollLockOff_OnStateEnter;
            sScrollLockOff.OnStateLeave += SScrollLockOff_OnStateLeave;
            sScrollLockOn.OnStateEnter += SScrollLockOn_OnStateEnter;
            sScrollLockOn.OnStateLeave += SScrollLockOn_OnStateLeave;

            t1NLOff2On.OnTransitionStart += T1NLOff2On_OnTransitionStart;
            t1NLOff2On.OnTransitionFinish += T1NLOff2On_OnTransitionFinish;
            t2NLOn2Off.OnTransitionStart += T2NLOn2Off_OnTransitionStart;
            t2NLOn2Off.OnTransitionFinish += T2NLOn2Off_OnTransitionFinish;
            t3CLOff2On.OnTransitionStart += T3CLOff2On_OnTransitionStart;
            t3CLOff2On.OnTransitionFinish += T3CLOff2On_OnTransitionFinish;
            t4CLOn2Off.OnTransitionStart += T4CLOn2Off_OnTransitionStart;
            t4CLOn2Off.OnTransitionFinish += T4CLOn2Off_OnTransitionFinish;
            t5SLOff2On.OnTransitionStart += T5SLOff2On_OnTransitionStart;
            t5SLOff2On.OnTransitionFinish += T5SLOff2On_OnTransitionFinish;
            t6SLOn2Off.OnTransitionStart += T6SLOn2Off_OnTransitionStart;
            t6SLOn2Off.OnTransitionFinish += T6SLOn2Off_OnTransitionFinish;
        }

        public void Start() {
            smKeyboard.Initialize();

            Console.WriteLine("Keyboard Demo Started\r\n");
            Console.WriteLine(smKeyboard.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                switch (input) {
                    case "1":
                        sigNumLock.Emit();
                        break;
                    case "2":
                        sigCapsLock.Emit();
                        break;
                    case "3":
                        sigScrollLock.Emit();
                        break;
                    default:
                        continueDemo = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(smKeyboard.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            smKeyboard.Terminate();

            Console.WriteLine("\r\nKeyboard Demo finished");
        }

        void KeyboardDemoSample() {
            Console.WriteLine("Keyboard demo started\r\n");

            smKeyboard.Initialize();

            Console.WriteLine("1-) Pressing NumLock");
            sigNumLock.Emit();
            Console.WriteLine();

            Console.WriteLine("2-) Pressing CapsLock");
            sigCapsLock.Emit();
            Console.WriteLine();

            Console.WriteLine("3-) Pressing ScrollLock");
            sigScrollLock.Emit();
            Console.WriteLine();

            Console.WriteLine("4-) Pressing NumLock");
            sigNumLock.Emit();
            Console.WriteLine();

            Console.WriteLine("5-) Pressing NumLock");
            sigNumLock.Emit();
            Console.WriteLine();

            Console.WriteLine("6-) Pressing CapsLock");
            sigCapsLock.Emit();
            Console.WriteLine();

            Console.WriteLine("7-) Pressing CapsLock");
            sigCapsLock.Emit();
            Console.WriteLine();

            Console.WriteLine("8-) Pressing ScrollLock");
            sigScrollLock.Emit();
            Console.WriteLine();

            Console.WriteLine("9-) Pressing ScrollLock");
            sigScrollLock.Emit();
            Console.WriteLine();

            smKeyboard.Terminate();

            Console.WriteLine("\r\nKeyboard demo finished");
        }

        private void T6SLOn2Off_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sSLON to sSLOFF");
        }

        private void T6SLOn2Off_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sSLON to sSLOFF");
        }

        private void T5SLOff2On_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sSLOFF to sSLON");
        }

        private void T5SLOff2On_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sSLOFF to sSLON");
        }

        private void T4CLOn2Off_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sCLON to sCLOFF");
        }

        private void T4CLOn2Off_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sCLON to sCLOFF");
        }

        private void T3CLOff2On_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sCLOFF to sCLON");
        }

        private void T3CLOff2On_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sCLOFF to sCLON");
        }

        private void T2NLOn2Off_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sNLON to sNLOFF");
        }

        private void T2NLOn2Off_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sNLON to sNLOFF");
        }

        private void T1NLOff2On_OnTransitionFinish() {
            Console.WriteLine("Finishing transition from sNLOFF to sNLON");
        }

        private void T1NLOff2On_OnTransitionStart(ITransition t, TransitionEventArgs args) {
            Console.WriteLine("Starting transition from sNLOFF to sNLON");
        }

        private void SScrollLockOn_OnStateLeave() {
            Console.WriteLine("Leaving sSLON state");
        }

        private void SScrollLockOn_OnStateEnter() {
            Console.WriteLine("Entering sSLON state");
        }

        private void SScrollLockOff_OnStateLeave() {
            Console.WriteLine("Leaving sSLOFF state");
        }

        private void SScrollLockOff_OnStateEnter() {
            Console.WriteLine("Entering sSLOFF state");
        }

        private void SCapsLockOn_OnStateLeave() {
            Console.WriteLine("Leaving sCLON state");
        }

        private void SCapsLockOn_OnStateEnter() {
            Console.WriteLine("Entering sCLON state");
        }

        private void SCapsLockOff_OnStateLeave() {
            Console.WriteLine("Leaving sCLOFF state");
        }

        private void SCapsLockOff_OnStateEnter() {
            Console.WriteLine("Entering sCLOFF state");
        }

        private void SNumLockOn_OnStateLeave() {
            Console.WriteLine("Leaving sNLON state");
        }

        private void SNumLockOn_OnStateEnter() {
            Console.WriteLine("Entering sNLON state");
        }

        private void SNumLockOff_OnStateLeave() {
            Console.WriteLine("Leaving sNLOFF state");
        }

        private void SNumLockOff_OnStateEnter() {
            Console.WriteLine("Entering sNLOFF state");
        }

        private void SActive_OnStateLeave1() {
            Console.WriteLine("Leaving Active state");
        }

        private void SActive_OnStateEnter1() {
            Console.WriteLine("Entering Active state");
        }
    }
}
