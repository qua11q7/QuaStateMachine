using QuaStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples.Demo {
    internal class ComplicatedDemo {
        StateMachine smComplicated;
        ISignal sig1;
        ISignal sig2;
        ISignal sig3;
        ISignal sig4;
        ISignal sig5;

        public ComplicatedDemo() {
            Initialize();
        }

        void Initialize() {
            smComplicated = new StateMachine();

            IState s1;
            IState s2;
            IState s3;
            IState s1_1;
            IState s1_2;
            IState s2_1;
            IState s2_2;
            IState s2_3;
            IState s3_1;
            IState s3_1_1;
            IState s3_1_2;
            IState s3_1_3;
            IState s3_1_4;
            IState s3_1_5;
            IState s3_1_6;
            IState s3_1_7;
            IState s3_1_5_1;
            IState s3_1_5_2;
            IState s3_1_7_1;
            IState s3_1_7_2;
            IState s3_1_7_3;
            IState s3_1_7_4;

            smComplicated.TryCreateState("s1", out s1);
            smComplicated.TryCreateState("s2", out s2);
            smComplicated.TryCreateState("s3", out s3);
            smComplicated.TryCreateState("s1_1", s1, out s1_1);
            smComplicated.TryCreateState("s1_2", s1, out s1_2);
            smComplicated.TryCreateState("s2_1", s2, out s2_1);
            smComplicated.TryCreateState("s2_2", s2, out s2_2);
            smComplicated.TryCreateState("s2_3", s2, out s2_3);
            smComplicated.TryCreateState("s3_1", s3, out s3_1);
            smComplicated.TryCreateState("s3_1_1", s3_1, out s3_1_1);
            smComplicated.TryCreateState("s3_1_2", s3_1, out s3_1_2);
            smComplicated.TryCreateState("s3_1_3", s3_1, out s3_1_3, 1);
            smComplicated.TryCreateState("s3_1_4", s3_1, out s3_1_4, 1);
            smComplicated.TryCreateState("s3_1_5", s3_1, out s3_1_5, 1);
            smComplicated.TryCreateState("s3_1_6", s3_1, out s3_1_6, 2);
            smComplicated.TryCreateState("s3_1_7", s3_1, out s3_1_7, 2);
            smComplicated.TryCreateState("s3_1_5_1", s3_1_5, out s3_1_5_1);
            smComplicated.TryCreateState("s3_1_5_2", s3_1_5, out s3_1_5_2);
            smComplicated.TryCreateState("s3_1_7_1", s3_1_7, out s3_1_7_1);
            smComplicated.TryCreateState("s3_1_7_2", s3_1_7, out s3_1_7_2);
            smComplicated.TryCreateState("s3_1_7_3", s3_1_7, out s3_1_7_3, 1);
            smComplicated.TryCreateState("s3_1_7_4", s3_1_7, out s3_1_7_4, 1);

            ITransition t1 = smComplicated.CreateTransition("s1 to s2", s1, s2);
            ITransition t2 = smComplicated.CreateTransition("s2 to s3", s2, s3);
            ITransition t3 = smComplicated.CreateTransition("s3 to s3", s3, s3);
            ITransition t4 = smComplicated.CreateTransition("s3 to s1", s3, s1);
            ITransition t5 = smComplicated.CreateTransition("s1_1 to s1_2", s1_1, s1_2);
            ITransition t6 = smComplicated.CreateTransition("s1_2 to s1_1", s1_2, s1_1);
            ITransition t7 = smComplicated.CreateTransition("s2_1 to s2_2", s2_1, s2_2);
            ITransition t8 = smComplicated.CreateTransition("s2_1 to s2_3", s2_1, s2_3);
            ITransition t9 = smComplicated.CreateTransition("s2_2 to s2_3", s2_2, s2_3);
            ITransition t10 = smComplicated.CreateTransition("s2_3 to s2_1", s2_3, s2_1);
            ITransition t11 = smComplicated.CreateTransition("s3_1_1 to s3_1_2", s3_1_1, s3_1_2);
            ITransition t12 = smComplicated.CreateTransition("s3_1_2 to s3_1_1", s3_1_2, s3_1_1);
            ITransition t13 = smComplicated.CreateTransition("s3_1_3 to s3_1_4", s3_1_3, s3_1_4);
            ITransition t14 = smComplicated.CreateTransition("s3_1_4 to s3_1_5", s3_1_4, s3_1_5);
            ITransition t15 = smComplicated.CreateTransition("s3_1_5 to s3_1_5", s3_1_5, s3_1_5);
            ITransition t16 = smComplicated.CreateTransition("s3_1_5 to s3_1_3", s3_1_5, s3_1_3);
            ITransition t17 = smComplicated.CreateTransition("s3_1_6 to s3_1_7", s3_1_6, s3_1_7);
            ITransition t18 = smComplicated.CreateTransition("s3_1_5_1 to s3_1_5_2", s3_1_5_1, s3_1_5_2);
            ITransition t19 = smComplicated.CreateTransition("s3_1_5_2 to s3_1_5_1", s3_1_5_2, s3_1_5_1);
            ITransition t20 = smComplicated.CreateTransition("s3_1_7_1 to s3_1_7_2", s3_1_7_1, s3_1_7_2);
            ITransition t21 = smComplicated.CreateTransition("s3_1_7_2 to s3_1_7_1", s3_1_7_2, s3_1_7_1);
            ITransition t22 = smComplicated.CreateTransition("s3_1_7_3 to s3_1_7_4", s3_1_7_3, s3_1_7_4);
            ITransition t23 = smComplicated.CreateTransition("s3_1_7_4 to s3_1_7_3", s3_1_7_4, s3_1_7_3);

            smComplicated.ConnectSignal("sig1", t5, out sig1);
            smComplicated.ConnectSignal("sig2", t7, out sig2);
            smComplicated.ConnectSignal("sig3", t1, out sig3);
            smComplicated.ConnectSignal("sig4", t3, out sig4);
            smComplicated.ConnectSignal("sig5", t18, out sig5);
            smComplicated.ConnectSignal(sig1, t6);
            smComplicated.ConnectSignal(sig1, t8);
            smComplicated.ConnectSignal(sig1, t11);
            smComplicated.ConnectSignal(sig1, t12);
            smComplicated.ConnectSignal(sig1, t15);
            smComplicated.ConnectSignal(sig1, t17);
            smComplicated.ConnectSignal(sig2, t9);
            smComplicated.ConnectSignal(sig2, t10);
            smComplicated.ConnectSignal(sig2, t13);
            smComplicated.ConnectSignal(sig2, t14);
            smComplicated.ConnectSignal(sig2, t16);
            smComplicated.ConnectSignal(sig2, t20);
            smComplicated.ConnectSignal(sig2, t23);
            smComplicated.ConnectSignal(sig3, t2);
            smComplicated.ConnectSignal(sig3, t4);
            smComplicated.ConnectSignal(sig5, t19);
            smComplicated.ConnectSignal(sig5, t21);
            smComplicated.ConnectSignal(sig5, t22);

            smComplicated.SetInitialState(s1);
            smComplicated.SetInitialState(s1_1, s1);
            smComplicated.SetInitialState(s2_1, s2);
            smComplicated.SetInitialState(s3_1, s3);
            smComplicated.SetInitialState(s3_1_1, s3_1);
            smComplicated.SetInitialState(s3_1_3, s3_1, 1);
            smComplicated.SetInitialState(s3_1_6, s3_1, 2);
            smComplicated.SetInitialState(s3_1_5_1, s3_1_5);
            smComplicated.SetInitialState(s3_1_7_1, s3_1_7);
            smComplicated.SetInitialState(s3_1_7_3, s3_1_7, 1);
        }

        public void Start() {
            smComplicated.Initialize();

            Console.WriteLine("Complicated Demo Started\r\n");
            Console.WriteLine(smComplicated.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
            Console.WriteLine();

            bool continueDemo = true;
            do {
                string input = Console.ReadLine().Trim();
                switch (input) {
                    case "1":
                        sig1.Emit();
                        break;
                    case "2":
                        sig2.Emit();
                        break;
                    case "3":
                        sig3.Emit();
                        break;
                    case "4":
                        sig4.Emit();
                        break;
                    case "5":
                        sig5.Emit();
                        break;
                    default:
                        continueDemo = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(smComplicated.GetAllActiveStateNames().Aggregate((a, b) => a + " - " + b));
                Console.WriteLine();

            } while (continueDemo);

            smComplicated.Terminate();

            Console.WriteLine("Complicated Demo Finished\r\n");
        }
    }
}
