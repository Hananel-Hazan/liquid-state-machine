 /*
 * This file is Part of a PROJECT NAME 
 * LGPLv3 Licence:
 *       Copyright (c) 2011 
 * 
 *	    Hananel Hazan [hananel.hazan@gmail.com]
 *		Larry Manevitz [manevitz@cs.haifa.ac.il]
 *      
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this list of conditions 
 *    and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice, this list of 
 *    conditions and the following disclaimer in the documentation and/or other materials provided
 *    with the distribution.
 * 3. Neither the name of the <ORGANIZATION> nor the names of its contributors may be used to endorse
 *    or promote products derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
 * DAMAGE.
 */
 
 using System;
using System.Collections.Generic;

using NN_Pr;

using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data.Basic;
using Encog.Neural.NeuralData;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Networks.Training;
using Encog.Neural.Data;
using Encog.Neural.Networks.Training.Simple;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Util.Banchmark;
using Encog.Util.Logging;
using Liquid_Detector.Tests.LiquidHRF;

namespace LiquidHRF
{
    class HRFReadOut
    {

        private int m_netSize;
        private int m_inputSize;
        private int m_outputSize;

        BasicNetwork m_network;

        public HRFReadOut(int inputSize, int hiddenSize, int outputSize, globalParam parameters)
        {
            //Logger.WriteLine("Initilaizing readout...");

            this.m_outputSize = outputSize;
            this.m_inputSize = inputSize;
            this.m_netSize = inputSize + hiddenSize + outputSize;
            this.m_network = new BasicNetwork();

            if (hiddenSize == 0) hiddenSize = 1;

            this.m_network.AddLayer(new BasicLayer(new ActivationTANH(), true, inputSize));
            this.m_network.AddLayer(new BasicLayer(new ActivationTANH(), true, hiddenSize));
            this.m_network.AddLayer(new BasicLayer(new ActivationLinear(), true, outputSize));
            this.m_network.Structure.FinalizeStructure();
            this.m_network.Reset();

            Encog.MathUtil.Randomize.NguyenWidrowRandomizer randomizer = new Encog.MathUtil.Randomize.NguyenWidrowRandomizer(-1, 1);
            randomizer.RandomGenerator = parameters.rnd;
            randomizer.Randomize(this.m_network);
        }

        public double[] RunCrossValidation(bool[][] readOutInput, double[][] readOutOutput, double[][] perfectVoxelOutput, int count, int part, globalParam parameters)
        {
            int chunkSize = readOutOutput.Length / count;
            int f_start = 0;
            int f_length = chunkSize * part;
            int s_start = f_length + chunkSize;
            int s_length = readOutOutput.Length - s_start;
            int p_start = chunkSize * part;
            int p_length = chunkSize;

            //Console.WriteLine("Training output");

            double[][] trainingInput = new double[f_length + s_length][];
            double[][] trainingOutput = new double[f_length + s_length][];
            int j = 0;
            for (int i = f_start; i < f_start + f_length; i++)
            {
                trainingOutput[j] = new double[1];
                trainingOutput[j][0] = readOutOutput[i][0];
                //Console.WriteLine(trainingOutput[j][0]);
                trainingInput[j] = new double[readOutInput[0].Length];
                for (int k = 0; k < readOutInput[0].Length; k++)
                    trainingInput[j][k] = readOutInput[i][k] ? 1 : 0;
                j++;
            }

            for (int i = s_start; i < s_start + s_length; i++)
            {
                trainingOutput[j] = new double[1];
                trainingOutput[j][0] = readOutOutput[i][0];
                //Console.WriteLine(trainingOutput[j][0]);
                trainingInput[j] = new double[readOutInput[0].Length];
                for (int k = 0; k < readOutInput[0].Length; k++)
                    trainingInput[j][k] = readOutInput[i][k] ? 1 : 0;
                j++;
            }

            //Console.WriteLine("Testing output");
            double[][] testingInput = new double[p_length][];
            double[][] testingOutput = new double[p_length][];
            double[][] perfectTestingOutput = new double[p_length][];
            j = 0;
            for (int i = p_start; i < p_start + p_length; i++)
            {
                testingOutput[j] = new double[2];
                testingOutput[j][0] = readOutOutput[i][0];
                //Console.WriteLine(testingOutput[j][0]);
                testingOutput[j][1] = perfectVoxelOutput[i][0];

                testingInput[j] = new double[readOutInput[0].Length];
                for (int k = 0; k < readOutInput[0].Length; k++)
                    testingInput[j][k] = readOutInput[i][k] ? 1 : 0;
                j++;
            }


            BasicNeuralDataSet trainingSet = new BasicNeuralDataSet(trainingInput, trainingOutput);
            BasicNeuralDataSet testingSet = new BasicNeuralDataSet(testingInput, testingOutput);
            Train(trainingSet, parameters);

            trainingOutput = new double[f_length + s_length][];
            j = 0;
            for (int i = f_start; i < f_start + f_length; i++)
            {
                trainingOutput[j] = new double[2];
                trainingOutput[j][0] = readOutOutput[i][0];
                trainingOutput[j][1] = perfectVoxelOutput[i][0];
                trainingInput[j] = new double[readOutInput[0].Length];
                for (int k = 0; k < readOutInput[0].Length; k++)
                    trainingInput[j][k] = readOutInput[i][k] ? 1 : 0;
                j++;
            }

            for (int i = s_start; i < s_start + s_length; i++)
            {
                trainingOutput[j] = new double[2];
                trainingOutput[j][0] = readOutOutput[i][0];
                trainingOutput[j][1] = perfectVoxelOutput[i][0];
                trainingInput[j] = new double[readOutInput[0].Length];
                for (int k = 0; k < readOutInput[0].Length; k++)
                    trainingInput[j][k] = readOutInput[i][k] ? 1 : 0;
                j++;
            }

            // Training results
            trainingSet = new BasicNeuralDataSet(trainingInput, trainingOutput);
            double[][] results = Test(trainingSet);
            double[] correlation_vs_ed = new double[8];

            //Console.WriteLine("Cross Correlation" + GetCrossCorrelation(results[0], results[1]));

            //Console.WriteLine("Train");
            double[] output_norm = results[0]; // Normalize(results[0]);
            double[] signal_norm = results[1]; // Normalize(results[1]);
            double[] ideal_norm = results[2]; //  Normalize(results[2]);
            correlation_vs_ed[0] = GetRankedPearsonCorrelation(output_norm, signal_norm, false);
            correlation_vs_ed[1] = GetRankedPearsonCorrelation(output_norm, ideal_norm, false);
            correlation_vs_ed[4] = GetRMS(output_norm, signal_norm, false);
            correlation_vs_ed[5] = GetRMS(output_norm, ideal_norm, false);

            // Generalization results
            results = Test(testingSet);

            //Console.WriteLine("Test");
            output_norm = ZScoreNormalize(Normalize(results[0]));
            signal_norm = ZScoreNormalize(Normalize(results[1]));
            ideal_norm = ZScoreNormalize(Normalize(results[2]));
            correlation_vs_ed[2] = GetRankedPearsonCorrelation(output_norm, signal_norm, false);
            correlation_vs_ed[3] = GetRankedPearsonCorrelation(output_norm, ideal_norm, false);
            correlation_vs_ed[6] = GetRMS(output_norm, signal_norm, false);
            correlation_vs_ed[7] = GetRMS(output_norm, ideal_norm, false);
            //Console.WriteLine("CORRELATION");
            //Console.WriteLine("TRAINED\t" + correlation_vs_ed[0] + "\t" + correlation_vs_ed[1]); // + " TESTED\t" + correlation_vs_ed[4] + "\t" + correlation_vs_ed[5]);
            //Console.WriteLine("RMS");
            //Console.WriteLine("TESTED\t" + correlation_vs_ed[2] + "\t" + correlation_vs_ed[3]); // + " TESTED\t" + correlation_vs_ed[6] + "\t" + correlation_vs_ed[7]);

            return correlation_vs_ed;
        }


        public double Train(INeuralDataSet trainingSet, globalParam parameters)
        {
            // train the network
            ResilientPropagation train = new ResilientPropagation(m_network, trainingSet);
            int epoch = 1;
            do
            {
                train.Iteration();
                epoch++;
            } while ((epoch < parameters.detector.ReadoutU_epoch) && train.Error > parameters.detector.Readout_Max_Error);

            return train.Error;
        }

        public double[][] Test(INeuralDataSet testingSet)
        {

            IEnumerator<INeuralDataPair> testingSetIt = testingSet.GetEnumerator();
            int count = 0;
            while (testingSetIt.MoveNext())
                count++;

            double[][] result;
            result = new double[3][];
            result[0] = new double[count];
            result[1] = new double[count];
            result[2] = new double[count];

            count = 0;
            testingSetIt = testingSet.GetEnumerator();
            INeuralDataPair dataPair = null;
            while (testingSetIt.MoveNext())
            {
                dataPair = testingSetIt.Current;
                INeuralData output = m_network.Compute(dataPair.Input);
                result[0][count] = output.Data[0];
                result[1][count] = dataPair.Ideal[0];
                result[2][count] = dataPair.Ideal[1];
                count++;
            }
            return result;
        }

        public static double[] Normalize(double[] x)
        {
            return MinMaxNormalize(x);
        }

        public static double[] MinMaxNormalize(double[] x)
        {
            double min = -10;
            double max = 10;    
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < min) min = x[i];
                if (x[i] > max) max = x[i];
            }
            double[] output = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                output[i] = (x[i] - min) * 2 / (max - min) - 1; 
            }

            return output;
        }

        public static double[] ZScoreNormalize(double[] x)
        {
            double mean = 0;
            for (int i = 0; i < x.Length; i++)
            {
                mean += x[i];
            }

            mean = mean / x.Length;

            double sd_x = 0;
            double local_var = 0;
            for (int i = 0; i < x.Length; i++)
            {                
                local_var = Math.Abs(x[i] - mean);
                sd_x += local_var * local_var;
            }

            sd_x = Math.Sqrt(sd_x / x.Length);

            double[] x_norm = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                x_norm[i] = (x[i] - mean) / sd_x;
            }

            return x_norm;
        }

        public double GetRMS(double[] x, double[] y, bool print)
        {
            double[] x_norm = x; // Normalize(x);
            double[] y_norm = y; // Normalize(y);
            double diff_xy_sqrt = 0;
            double diff_xy = 0;
            for (int i = 0; i < x.Length; i++)
            {
                if (print) Logger.WriteLine(x[i] + " " + y[i]);
                diff_xy = Math.Abs(x_norm[i] - y_norm[i]);
                diff_xy_sqrt += diff_xy * diff_xy;
            }

            return Math.Sqrt(diff_xy_sqrt / x.Length);
        }

        public double GetRankedPearsonCorrelation(double[] x, double[] y, bool print)
        {
            double mu_x = 0;
            double mu_y = 0;
            double mu_dev_x_y = 0;
            double mu_dev_x = 0;
            double mu_dev_y = 0;

            int startIdx = 0;
            int count = x.Length;

            for (int i = startIdx; i < startIdx + count; i++)
            {
                mu_x += x[i];
                mu_y += y[i];
                if (print)
                    Logger.WriteLine(x[i]+" "+y[i]);
            }

            mu_x = mu_x / count;
            mu_y = mu_y / count;

            for (int i = startIdx; i < startIdx + count; i++)
            {
                mu_dev_x += (x[i] - mu_x) * (x[i] - mu_x);
                mu_dev_y += (y[i] - mu_y) * (y[i] - mu_y);
                mu_dev_x_y += (x[i] - mu_x) * (y[i] - mu_y);
            }

            mu_dev_x = Math.Sqrt(mu_dev_x / (count - 1));
            mu_dev_y = Math.Sqrt(mu_dev_y / (count - 1));
            mu_dev_x_y = mu_dev_x_y / (count - 1);

            return mu_dev_x_y / (mu_dev_x * mu_dev_y);
        }

        public double GetSpearmanCorrelation(double[] x, double[] y)
        {
            double[] x_sort = new double[x.Length];
            double[] y_sort = new double[y.Length];
            double[] x_rank = new double[x.Length];
            double[] y_rank = new double[y.Length];

            for (int i = 0; i < x.Length; i++)
            {
                x_sort[i] = x[i];
                y_sort[i] = y[i];
                Logger.WriteLine(x[i] + " " + y[i]);
                x_rank[i] = -1;
                y_rank[i] = -1;
            }

            Array.Sort(x_sort);
            Array.Sort(y_sort);

            for (int i = 0; i < x_sort.Length; i++)
            {
                for (int j = 0; j < x.Length; j++)
                {
                    if (x_sort[i] == x[j] && x_rank[j] == -1)
                    {
                        x_rank[j] = i;
                        x_sort[i] = -10000;
                    }

                    if (y_sort[i] == y[j] && y_rank[j] == -1)
                    {
                        y_rank[j] = i;
                        y_sort[i] = -10000;
                    }
                }
            }


            return GetRankedPearsonCorrelation(x_rank, y_rank,false);
        }


        public double GetCorrelation(double[] x, double[] y, bool print)
        {
            int startIdx = 0;
            double sum_x = 0;
            double sum_x_sqrt = 0;
            for (int i = startIdx; i < x.Length; i++)
            {
                sum_x += x[i];
                sum_x_sqrt += x[i] * x[i];
            }

            double sum_y = 0;
            double sum_y_sqrt = 0;
            for (int i = startIdx; i < x.Length; i++)
            {
                sum_y += y[i];
                sum_y_sqrt += y[i] * y[i];
            }

            double sum_xy = 0;
            for (int i = startIdx; i < x.Length; i++)
            {
                sum_xy += x[i] * y[i];
                if (print)
                {
                    Logger.WriteLine(x[i]+" "+y[i]);
                }
            }

            double upper = (x.Length - startIdx) * sum_xy - sum_x * sum_y;
            double lower = Math.Sqrt(Math.Abs((x.Length - startIdx) * sum_x_sqrt - sum_x * sum_x)) * Math.Sqrt(Math.Abs((x.Length - startIdx) * sum_y_sqrt - sum_y * sum_y));

            return upper / lower;
        }

        double GetCrossCorrelation(double[] x, double[] y)
        {
            double mx = 0;
            double my = 0;
            for (int i = 0; i < x.Length; i++)
            {
                mx += x[i];
                my += y[i];
            }
            mx /= x.Length;
            my /= x.Length;

            /* Calculate the denominator */
            double sx = 0;
            double sy = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sx += (x[i] - mx) * (x[i] - mx);
                sy += (y[i] - my) * (y[i] - my);
            }
            double denom = Math.Sqrt(sx * sy);

            /* Calculate the correlation series */
            double r_final = 0;
            for (int delay = -5; delay < 5; delay++)
            {
                double sxy = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    int j = i + delay;
                    while (j < 0)
                        j += x.Length;
                    j %= x.Length;
                    sxy += (x[i] - mx) * (y[j] - my);
                }
                double r = sxy / denom;
                if (Math.Abs(r) > r_final)
                    r_final = Math.Abs(r);
            }

            return r_final;
        }
    }
}
