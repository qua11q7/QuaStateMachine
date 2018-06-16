using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.Demo {
    internal class Exception1 {
        StateMachine smScenario1;
        ISignal sigA1;
        ISignal sigB1;
        ISignal sigC1;

        public Exception1() {
            Initialize();
        }

        void Initialize() {
            smScenario1 = new StateMachine();

            IState s1;
            IState s2;
            IState s3;
            IState s1_1;
            IState s1_2;
            IState s2_1;
            IState s2_2;

            ITransition t1s1s2;
            ITransition t2s2s1;
            ITransition t3s1_1s1_2;
            ITransition t4s1_2s1_1;
            ITransition t5s2_1s2_2;
            ITransition t6s2_2s2_1;
            ITransition t7s1_2s3;
            ITransition t8s3s2_2;

            smScenario1.TryCreateState("s1", out s1);
            smScenario1.TryCreateState("s2", out s2);
            smScenario1.TryCreateState("s3", out s3);
            smScenario1.TryCreateState("s1_1", s1, out s1_1);
            smScenario1.TryCreateState("s1_2", s1, out s1_2);
            smScenario1.TryCreateState("s2_1", s2, out s2_1);
            smScenario1.TryCreateState("s2_2", s2, out s2_2);

            smScenario1.TryCreateTransition("s1 to s2", s1, s2, out t1s1s2);
            smScenario1.TryCreateTransition("s2 to s1", s2, s1, out t2s2s1);
            smScenario1.TryCreateTransition("s1_1 to s1_2", s1_1, s1_2, out t3s1_1s1_2);
            smScenario1.TryCreateTransition("s1_2 to s1_1", s1_2, s1_1, out t4s1_2s1_1);
            smScenario1.TryCreateTransition("s2_1 to s2_2", s2_1, s2_2, out t5s2_1s2_2);
            smScenario1.TryCreateTransition("s2_2 to s2_1", s2_2, s2_1, out t6s2_2s2_1);

            // Invalid transitions
            smScenario1.TryCreateTransition("s1_2 to s3", s1_2, s3, out t7s1_2s3);
            smScenario1.TryCreateTransition("s3 to s2_2", s3, s2_2, out t8s3s2_2);

            smScenario1.ConnectSignal("sigA", t1s1s2, out sigA1);
            smScenario1.ConnectSignal("sigB", t3s1_1s1_2, out sigB1);
            smScenario1.ConnectSignal("sigC", t7s1_2s3, out sigC1);
            smScenario1.ConnectSignal(sigA1, t2s2s1);
            smScenario1.ConnectSignal(sigB1, t4s1_2s1_1);
            smScenario1.ConnectSignal(sigB1, t5s2_1s2_2);
            smScenario1.ConnectSignal(sigB1, t6s2_2s2_1);
            smScenario1.ConnectSignal(sigC1, t8s3s2_2);

            smScenario1.SetInitialState(s1);
            smScenario1.SetInitialState(s1_1, s1);
            smScenario1.SetInitialState(s2_1, s2);
        }

        public void Start() {
            smScenario1.Initialize();

            Console.WriteLine("Scenario1 Started\r\n");
            Console.WriteLine(smScenario1.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                switch (input) {
                    case "1":
                        sigA1.Emit();
                        break;
                    case "2":
                        sigB1.Emit();
                        break;
                    case "3":
                        sigC1.Emit();
                        break;
                    default:
                        continueDemo = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(smScenario1.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            smScenario1.Terminate();

            Console.WriteLine("Scenario1 Finished\r\n");
        }
    }
}
