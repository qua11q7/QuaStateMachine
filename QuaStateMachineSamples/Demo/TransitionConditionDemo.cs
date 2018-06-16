using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.Demo {
    internal class TransitionConditionDemo {
        StateMachine smTrans;
        ISignal signalA;
        ISignal signalB;

        public TransitionConditionDemo() {
            Initialize();
        }

        void Initialize() {
            smTrans = new StateMachine();

            IState s1;
            IState s2;
            IState s3;
            IState s1_1;
            IState s1_2;

            smTrans.TryCreateState("s1", out s1);
            smTrans.TryCreateState("s2", out s2);
            smTrans.TryCreateState("s3", out s3);
            smTrans.TryCreateState("s1_1", s1, out s1_1);
            smTrans.TryCreateState("s1_2", s1, out s1_2);

            smTrans.CreateTransition("s1 to s2", s1, s2);
            smTrans.CreateTransition("s1 to s3", s1, s3);
            smTrans.CreateTransition("s2 to s1", s2, s1);
            smTrans.CreateTransition("s3 to s1", s3, s1);
            smTrans.CreateTransition("s1_1 to s1_2", s1_1, s1_2);
            smTrans.CreateTransition("s1_2 to s1_1", s1_2, s1_1);

            smTrans.ConnectSignal("signalA", "s1 to s2", out signalA);
            smTrans.ConnectSignal("signalB", "s1_1 to s1_2", out signalB);
            smTrans.ConnectSignal(signalA, "s1 to s3");
            smTrans.ConnectSignal(signalB, "s1_2 to s1_1");
            smTrans.ConnectSignal(signalB, "s2 to s1");
            smTrans.ConnectSignal(signalB, "s3 to s1");

            smTrans.CreateTransitionCondition(signalA, "s1 to s2", s1_1);
            smTrans.CreateTransitionCondition(signalA, "s1 to s3", s1_2);

            smTrans.SetInitialState(s1);
            smTrans.SetInitialState(s1_1, s1);
        }

        public void Start() {
            smTrans.Initialize();

            Console.WriteLine("Transition Condition Demo Started\r\n");
            Console.WriteLine(smTrans.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                switch (input) {
                    case "1":
                        signalA.Emit();
                        break;
                    case "2":
                        signalB.Emit();
                        break;
                    default:
                        continueDemo = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(smTrans.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            smTrans.Terminate();

            Console.WriteLine("Transition Condition Finished\r\n");
        }
    }
}
