using QuaStateMachineSamples.ChainCreation;
using QuaStateMachineSamples.Demo;
using QuaStateMachineSamples.GenericDemo;
using QuaStateMachineSamples.TransitionlessDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuaStateMachineSamples {
    class Program {
        // NORMAL SAMPLES
        static StopwatchDemo stopwatchDemo = new StopwatchDemo();
        static CameraDemo cameraDemo = new CameraDemo();
        static KeyboardDemo keyboardDemo = new KeyboardDemo();
        static ComplicatedDemo complicatedDemo = new ComplicatedDemo();
        static EmitConditionDemo emitConditionDemo = new EmitConditionDemo();
        static TransitionConditionDemo transitionConditionDemo = new TransitionConditionDemo();
        static Exception1 exception1Demo;

        // GENERIC SAMPLES
        static StopwatchGenericDemo stopwatchGenericDemo = new StopwatchGenericDemo();
        // TODO : Add more samples

        // CHAIN SAMPLES
        static StopwatchChainDemo stopwatchChainDemo = new StopwatchChainDemo();
        // TODO : Add more samples

        // TRANSITIONLESS SAMPLES
        static StopwatchTransitionlessDemo stopwatchTransitionlessDemo = new StopwatchTransitionlessDemo();
        static CameraTransitionlessDemo cameraTransitionlessDemo = new CameraTransitionlessDemo();
        static KeyboardTransitionlessDemo keyboardTransitionlessDemo = new KeyboardTransitionlessDemo();
        // TODO : Add more samples

        static void Main(string[] args) {
            Samples selectedSample = Samples.EndOfSamples;
            do {
                selectedSample = SampleSelection();
                switch (selectedSample) {
                    case Samples.Stopwatch:
                        stopwatchDemo.Start();
                        break;
                    case Samples.Camera:
                        cameraDemo.Start();
                        break;
                    case Samples.Keyboard:
                        keyboardDemo.Start();
                        break;
                    case Samples.Complicated:
                        complicatedDemo.Start();
                        break;
                    case Samples.EmitCondition:
                        emitConditionDemo.Start();
                        break;
                    case Samples.TransitionCondition:
                        transitionConditionDemo.Start();
                        break;
                    case Samples.Exception1:
                        exception1Demo = new Exception1();
                        exception1Demo.Start();
                        break;
                    case Samples.StopwatchGeneric:
                        stopwatchGenericDemo.Start();
                        break;
                    case Samples.StopwatchChain:
                        stopwatchChainDemo.Start();
                        break;
                    case Samples.StopwatchTransitionless:
                        stopwatchTransitionlessDemo.Start();
                        break;
                    case Samples.CameraTransitionless:
                        cameraTransitionlessDemo.Start();
                        break;
                    case Samples.KeyboardTransitionless:
                        keyboardTransitionlessDemo.Start();
                        break;
                    case Samples.EndOfSamples:
                        break;
                    default:
                        break;
                }
            } while (selectedSample != Samples.EndOfSamples);

            Console.ReadKey();
        }

        static Samples SampleSelection() {
            Console.WriteLine("\r\nSAMPLES");
            IEnumerable<string> sampleNames = Enum.GetNames(typeof(Samples));
            for (int i = 0; i < sampleNames.Count() - 1; i++) {
                Console.WriteLine((i + 1) + "-) " + sampleNames.ElementAt(i));
            }
            Console.Write("Enter the number of desired sample: ");
            string input = Console.ReadLine().Trim();
            Console.WriteLine();

            int number;
            if (Int32.TryParse(input, out number)) {
                if (number > (int)Samples.EndOfSamples)
                    return Samples.EndOfSamples;

                return (Samples)(number - 1);
            } else {
                return Samples.EndOfSamples;
            }
        }

        enum Samples {
            Stopwatch,
            Camera,
            Keyboard,
            Complicated,
            EmitCondition,
            TransitionCondition,
            Exception1,
            // GENERIC
            StopwatchGeneric,
            // CHAIN
            StopwatchChain,
            // TRANSITIONLESS
            StopwatchTransitionless,
            CameraTransitionless,
            KeyboardTransitionless,
            EndOfSamples
        }
    }
}
