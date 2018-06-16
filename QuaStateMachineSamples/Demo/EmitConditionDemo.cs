using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.Demo {
    internal class EmitConditionDemo {
        StateMachine smEmit;
        ISignal sigA;
        ISignal sigB;

        public EmitConditionDemo() {
            Initialize();
        }

        void Initialize() {
            smEmit = new StateMachine();

            IState s1;
            IState s2;
            IState s1_1;
            IState s1_2;

            smEmit.TryCreateState("s1", out s1);
            smEmit.TryCreateState("s2", out s2);
            smEmit.TryCreateState("s1_1", s1, out s1_1);
            smEmit.TryCreateState("s1_2", s1, out s1_2);

            smEmit.CreateTransition("s1 to s2", s1, s2);
            smEmit.CreateTransition("s2 to s1", s2, s1);
            smEmit.CreateTransition("s1_1 to s1_2", s1_1, s1_2);
            smEmit.CreateTransition("s1_2 to s1_1", s1_2, s1_1);

            smEmit.ConnectSignal("sigA", "s1 to s2", out sigA);
            smEmit.ConnectSignal("sigB", "s1_1 to s1_2", out sigB);
            smEmit.ConnectSignal(sigB, "s2 to s1");
            smEmit.ConnectSignal(sigB, "s1_2 to s1_1");

            smEmit.CreateEmitCondition(sigA, s1_2);

            smEmit.SetInitialState(s1);
            smEmit.SetInitialState(s1_1, s1);
        }

        public void Start() {
            smEmit.Initialize();

            Console.WriteLine("Emit Condition Demo Started\r\n");
            Console.WriteLine(smEmit.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                switch (input) {
                    case "1":
                        sigA.Emit();
                        break;
                    case "2":
                        sigB.Emit();
                        break;
                    default:
                        continueDemo = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(smEmit.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            smEmit.Terminate();

            Console.WriteLine("Emit Condition Finished\r\n");
        }
    }
}
