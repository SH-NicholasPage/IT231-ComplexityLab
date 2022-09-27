using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ComplexityLab
{
    public static class Program
    {
        private const byte CORRECTNESS_SCORE = 15;
        private const byte TIME_SCORE = 25;
        private const byte GRADEING_RUNS = 5;


        private static IDictionary<uint, ulong> validation = new Dictionary<uint, ulong>();

#if DEBUG
        private static readonly sbyte MAX_SORT_COUNTER = 12;
#endif
#if !DEBUG
        private static readonly sbyte MAX_SORT_COUNTER = 1;
#endif
        private static sbyte sortCounter = MAX_SORT_COUNTER;

        public static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (Directory.Exists(args[0]) == true)
                {
                    Console.WriteLine("File " + GetFileName(args[0]) + (RunFile(args[0], out _, out _) == true ? " passed." : " failed."));
                }
            }
            else
            {
                String[] files = Directory.GetFiles("traces", "*.rep");
                byte filesPassed = 0;
                List<float> perfScores = new List<float>();
#if DEBUG
                for (int i = 0; i < files.Length; i++)
                {
                    uint? weight = 0;
                    float perfScore;
                    if (RunFile(files[i], out weight, out perfScore) == true)
                    {
                        while (weight-- > 0)
                        {
                            perfScores.Add(perfScore);
                        }
                        filesPassed++;
                    }
                }
#endif

#if !DEBUG
                for (int i = 0; i < GRADEING_RUNS; i++)
                {
                    filesPassed = 0;

                    for (int j = 0; j < files.Length; j++)
                    {
                        uint? weight = 0;
                        float perfScore;
                        if (RunFile(files[j], out weight, out perfScore) == true)
                        {
                            while (weight-- > 0)
                            {
                                perfScores.Add(perfScore);
                            }
                            filesPassed++;
                        }
                    }
                }
#endif
                float cScore = (float)filesPassed / files.Length * CORRECTNESS_SCORE;
                float pScore = ((perfScores.Count > 0) ? perfScores.Average() : 0) * TIME_SCORE;

                Console.WriteLine("\n***Correctness index: {0:0.0}/" + CORRECTNESS_SCORE, cScore);
                Console.WriteLine("***Performance index: {0:0.0}/" + TIME_SCORE + (filesPassed < files.Length ? "*" : ""), pScore);
                Console.Write("\n--- Total score: {0:0.0}/" + (CORRECTNESS_SCORE + TIME_SCORE) + "\n", cScore + pScore);
            }
        }

        private static bool PerfMalloc(Source arrContainer, uint id, ulong size, out double elapsedMS)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            if (arrContainer.Insert(size) == false)
            {
                timer.Stop();
                elapsedMS = timer.Elapsed.TotalMilliseconds;
                return false;
            }

            timer.Stop();
            elapsedMS = timer.Elapsed.TotalMilliseconds;
            validation.Add(id, size);

            return StandardCheck(arrContainer);
        }

        private static bool PerfFree(Source arrContainer, uint id, out double elapsedMS)
        {
            ulong size = validation[id];

            Stopwatch timer = new Stopwatch();
            timer.Start();

            if (arrContainer.Remove(size) == false)
            {
                timer.Stop();
                elapsedMS = timer.Elapsed.TotalMilliseconds;
                return false;
            }

            timer.Stop();
            elapsedMS = timer.Elapsed.TotalMilliseconds;
            validation.Remove(id);

            return StandardCheck(arrContainer);
        }

        private static bool PerfRealloc(Source arrContainer, uint id, ulong size, out double elapsedMS)
        {
            if (PerfFree(arrContainer, id, out elapsedMS) == false)
            {
                return false;
            }

            double individualMS;

            if (PerfMalloc(arrContainer, id, size, out individualMS) == false)
            {
                return false;
            }

            elapsedMS += individualMS;

            return true;
        }

        private static bool RunFile(String file, out uint? weight, out float perfScore)
        {
            Console.WriteLine("Testing " + GetFileName(file) + "...");

            weight = null;
            uint? numIds = null;
            ulong? numOps = null;
            ulong aOps = 0;
            ulong? maxAlloc = null;
            double? maxTimeExpected = null;
            bool passed = true;
            Source arrContainer = new Source();
            double timeTaken = 0;

            if (arrContainer.Init() == false)//Let the user initialize their MM
            {
                passed = false;
                goto loopEnd;
            }

            String[] lines = File.ReadLines(file).ToArray();
            foreach (String line in lines)
            {
                double tempTimeTaken = 0;
                String[] l = line.Split(" ");
                switch (line[0])
                {
                    case 'a':
                        aOps++;
                        if (PerfMalloc(arrContainer, Convert.ToUInt32(l[1]), Convert.ToUInt64(l[2]), out tempTimeTaken) == false)
                        {
                            passed = false;
                            goto loopEnd;
                        }
                        break;
                    case 'f':
                        if (PerfFree(arrContainer, Convert.ToUInt32(l[1]), out tempTimeTaken) == false)
                        {
                            passed = false;
                            goto loopEnd;
                        }
                        break;
                    case 'r':
                        aOps++;
                        numOps++;
                        if (PerfRealloc(arrContainer, Convert.ToUInt32(l[1]), Convert.ToUInt64(l[2]), out tempTimeTaken) == false)
                        {
                            passed = false;
                            goto loopEnd;
                        }
                        break;
                    default:
                        if (weight == null) weight = Convert.ToUInt32(line);
                        else if (numIds == null) numIds = Convert.ToUInt32(line);
                        else if (numOps == null) numOps = Convert.ToUInt64(line);
                        else if (maxAlloc == null) maxAlloc = Convert.ToUInt64(line);
                        else if (maxTimeExpected == null) maxTimeExpected = Convert.ToDouble(line);
                        break;
                }

                timeTaken += tempTimeTaken;
            }

        loopEnd:

#region CALCULATE TIME
            if (maxTimeExpected == null)
            {
                maxTimeExpected = aOps * Math.Log2(aOps) * (0.01d * (double)aOps / 58000);
            }

            //timeTaken = maxTimeExpected * 0.8d; //For testing

            double maxAcceptedTime = maxTimeExpected!.Value * 2;
            double pscore = (maxAcceptedTime - timeTaken) / (maxAcceptedTime - maxTimeExpected!.Value);

            perfScore = Math.Min(1f, (pscore >= 0) ? (float)pscore : 0);
#endregion

            validation.Clear();

            return passed;
        }

        private static String GetFileName(String file)
        {
            try
            {
                return file.Substring(file.IndexOf('\\') + 1);
            }
            catch
            {
                return file;
            }
        }

        private static bool StandardCheck(Source arrContainer)
        {
            bool returnStatus = true;

            if (arrContainer.Collection == null)
            {
                Console.Error.WriteLine("[ERROR]: Collection is null.");
                returnStatus = false;
            }
            else if (arrContainer.Collection.Count != validation.Count)
            {
                Console.Error.WriteLine("[ERROR]: Collection does not have the correct amount of elements.");
                returnStatus = false;
            }
            else if (--sortCounter <= 0 && IsSorted(arrContainer.Collection) == false)
            {
                Console.Error.WriteLine("[ERROR]: Collection is not sorted!");
                returnStatus = false;
            }

            if (sortCounter <= 0)
            {
                sortCounter = MAX_SORT_COUNTER;
            }

            return returnStatus;
        }

        private static bool IsSorted(ICollection<ulong> input)
        {
            if (input is IOrderedEnumerable<ulong>)
            {
                return true;
            }

            return Enumerable.SequenceEqual<ulong>(validation.Values.OrderBy(x => x), input);
        }
    }
}