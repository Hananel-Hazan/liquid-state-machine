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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Liquid_Detector.Tests.LiquidHRF;
using System.Threading;


namespace Liquid_Detector.Tests.LiquidHRF
{
	/// <summary>
	/// Description of MainLiquidHRF.
	/// </summary>
	public class LiquidHRF
	{
		public static double THRESHOLD;
		public static Liquid.LSM lsmNet;
		public static globalParam parameters;

		double[][][] m_stimuliChunks;
		double[][][] m_voxelChunks;


		public static void StartJob(string[] args)
		{
			parameters = new globalParam();
			parameters.initialization();
			parameters.test = new Liquid_Detector.Tests.LiquidHRF.Parameters.TestParameters4LiquidHRF();
			parameters.input2liquid = new Liquid_Detector.Tests.LiquidHRF.Parameters.Input4LiquidHRF();
			parameters.neuronParam =  new Liquid_Detector.Tests.LiquidHRF.Parameters.Neuron4LiquidHRF();
			parameters.networkParam = new Liquid_Detector.Tests.LiquidHRF.Parameters.RecurrentNetwork4LiquidHRF( parameters.neuronParam.Neuron_Model, ref parameters );
			parameters.detector = new Liquid_Detector.Tests.LiquidHRF.Parameters.ReadOut4LiquidHRF(ref parameters.input2liquid, parameters.test.Input_Pattern_Size);

			
			
			InitLSM();

			int j=0;
			for (int i = 0; i < args.Length; i=i+2)
			{
				string[] w_args = new string[] { args[i], args[i+1] };
				LiquidHRF.MainLiquidHRF(w_args, i == 0);
				j++;
			}
		}

		public static void InitLSM()
		{
			// initialize system params
			THRESHOLD = -1;

			lsmNet = new Liquid.LSM(ref parameters);
			lsmNet.FullReset(ref parameters);
		}

		public static void MainLiquidHRF(string[] args, bool checkRes)
		{

			LiquidHRF experiment = new LiquidHRF();
			double res = 0;
			Console.WriteLine(args[0]);
			Console.WriteLine(args[1]);
			res = experiment.Run(args[0], args[1], 1121, false, checkRes);
			while (checkRes && res < THRESHOLD)
			{
				InitLSM();
				res = experiment.Run(args[0], args[1], 1121, false, checkRes);
				Console.WriteLine("---------------");
				Console.WriteLine("Result: " + res);
			}
		}

		public double Run(string stimuliPath, string voxelPath, int chunkSize, bool considerTR, bool checkRes)
		{
			// load simulation data
			if (!LoadData(stimuliPath, voxelPath, chunkSize))
				return 1.0;

			// prepare input data for LSM
			globalParam.Data[] stimuliData = InitInput(m_stimuliChunks, parameters.neuronParam.Neuron_Threshold, considerTR);
			bool[,,] inputNeurons = null;
			
			parameters.SeedInitialization();
			lsmNet = new Liquid.LSM(ref parameters);
			lsmNet.FullReset(ref parameters);
			
			// run LSM
			Console.WriteLine("Run LSM...");
			bool[][,] lsmOutput = RunLSM(lsmNet, parameters, stimuliData, ref inputNeurons);

			// run cross-validation
			int voxelCount = m_voxelChunks[0].Length;
			double[][] idealOutputData = null;

			double res = 0;
			for (int voxel = 0; voxel < voxelCount; voxel++)
			{
				if (voxel % 5 == 0)
				{
					idealOutputData = InitOutput(voxel);
				}
				
				{
					double[][] outputData = InitOutput(voxel);
					res = RunCrossValidation(lsmOutput, outputData, checkRes ? 2 : 2, parameters, voxel % 5 == 0 ? InitOutput(voxel + 1) : idealOutputData, voxel);
					if (checkRes)
						return res;
				}
			}

			return res;
		}

		private bool[][] CreateFlatInput(bool[][,] readOutInput)
		{
            int RATIO = 1;
            bool[][] input = new bool[readOutInput.Length * (readOutInput[0].GetLength(1) / RATIO)][];
			int count = 0;
			for (int i = 0; i < readOutInput.Length; i++)
			{
				for (int k = 0; k < readOutInput[0].GetLength(1)/RATIO; k++)
				{
					input[count] = new bool[readOutInput[0].GetLength(0)*RATIO];
					int count1 = 0;
					for (int j = 0; j < readOutInput[0].GetLength(0); j++)
					{
						for (int m = 0; m < RATIO; m++)
						{
							input[count][count1++] = readOutInput[i][j, k * m + m];
						}
					}
					count++;
				}
			}

			return input;
		}

		private double[][] CreateFlatOutput(double[][] readOutOutput, bool normalize)
		{
			double[][] output = new double[readOutOutput.Length * readOutOutput[0].Length][];
			if (normalize)
			{
				double[] outArr = new double[readOutOutput.Length * readOutOutput[0].Length];
				int count = 0;
				for (int i = 0; i < readOutOutput.Length; i++)
				{
					for (int j = 0; j < readOutOutput[0].Length; j++)
					{
						outArr[count] = readOutOutput[i][j];
						count++;
					}
				}
//				outArr = HRFReadOut.ZScoreNormalize(outArr);
				for (int i = 0; i < outArr.Length; i++)
				{
					output[i] = new double[1];
					output[i][0] = outArr[i];
				}

			}
			else
			{
				int count = 0;
				for (int i = 0; i < readOutOutput.Length; i++)
				{
					for (int j = 0; j < readOutOutput[0].Length; j++)
					{
						output[count] = new double[1];
						output[count][0] = readOutOutput[i][j];
						count++;
					}
				}
			}
			return output;
		}

		private double RunCrossValidation(bool[][,] readOutInput, double[][] readOutOutput, int dCrossFoldCount, globalParam parameters, double[][] perfectVoxelOutput, int voxelId)
		{
			int crossFoldCount = Math.Abs(dCrossFoldCount);
			bool[][] input = CreateFlatInput(readOutInput);
			double[][] output = CreateFlatOutput(readOutOutput, true);
			double[][] idealOutput = CreateFlatOutput(perfectVoxelOutput, true);
			double[][] correlationVals = new double[dCrossFoldCount < 0 ? 1 : crossFoldCount][];

			for (int i = 0; i < crossFoldCount; i++)
			{
				ReadOut_Detector HRFdetector = new ReadOut_Detector(readOutInput[0].GetLength(0), 7, 1,0,ref parameters);
				correlationVals[i] = HRFdetector.RunCrossValidation(input,
				                                                    output,
				                                                    idealOutput,
				                                                    crossFoldCount,
				                                                    i,
				                                                    parameters);
				if (dCrossFoldCount < 0)
					break;

			}

			double[] avg = new double[8];
			double[] max = new double[8];
			double[] min = new double[8];
			for (int j = 0; j < 8; j++)
			{
				max[j] = 0;
				min[j] = 100;
			}

			for (int i = 0; i < correlationVals.Length; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					double val = Math.Abs(Norm(correlationVals[i][j], j < 4));
					if (val > max[j]) max[j] = val;
					if (val < min[j]) min[j] = val;
					avg[j] += val;
				}
			}

			for (int j = 0; j < 8; j++)
			{
                if (correlationVals.Length > 2)
                    avg[j] = (avg[j] - max[j] - min[j]) / (correlationVals.Length - 2);
                else
                    avg[j] = avg[j] / correlationVals.Length;
			}

			double[] variance = new double[8];
			for (int i = 0; i < correlationVals.Length; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					variance[j] += (Math.Abs(avg[j]) - Math.Abs(Norm(correlationVals[i][j], true))) * (Math.Abs(avg[j]) - Math.Abs(Norm(correlationVals[i][j],true)));
				}
			}

			for (int j = 0; j < 8; j++)
			{
				variance[j] = Math.Sqrt(variance[j] / correlationVals.Length);
			}
			
			Console.Write("" + voxelId + "\t");
			for (int i = 0; i < 8; i++)
				Console.Write(avg[i] + "\t");
			

			Console.Write("" + voxelId + "\t");
			for (int i = 0; i < 8; i++)
				Console.Write(variance[i] + "\t");

			Console.WriteLine();
			
			return avg[1];
		}

		private bool[][,] RunLSM(Liquid.LSM lsmNet, globalParam parameters, globalParam.Data[] stimuliData, ref bool[,,] inputNeurons)
		{
			bool[,,] lsmOutput;
			double[, ,] outputFrequency;
			bool[,,] inputSignal;

			lsmNet.run(ref stimuliData,
			           out lsmOutput,
			           out inputNeurons,
			           out outputFrequency,
			           out inputSignal,
			           1,
			           0,
			           2,
			           ref parameters);

			bool[][,] finalOutput = new bool[lsmOutput.GetLength(0)][,];
			for (int i = 0; i < lsmOutput.GetLength(0); i++)
			{
				finalOutput[i] = new bool[lsmOutput.GetLength(1) + inputNeurons.GetLength(1), lsmOutput.GetLength(2)];
				for (int j = 0; j < lsmOutput.GetLength(1); j++)
				{
					for (int k = 0; k < lsmOutput.GetLength(2); k++)
					{
						finalOutput[i][j, k] = lsmOutput[i, j, k];
					}
				}
				for (int j = 0; j < inputNeurons.GetLength(1); j++)
				{
					for (int k = 0; k < inputNeurons.GetLength(2); k++)
					{
						finalOutput[i][j + lsmOutput.GetLength(1), k] = inputNeurons[i, j, k];
					}
				}
			}
			return finalOutput;
		}

		private bool LoadData(string stimuliPath, string voxelPath, int chunkSize)
		{
			// load input sequences for 2 stimuli
			DataSet stimuli = new DataSet();
			if (!stimuli.Load(stimuliPath))
			{
				return false;
			}
			// load signals for X voxels
			DataSet voxels = new DataSet();
			if (!voxels.Load(voxelPath))
			{
				return false;
			}
			// divide stimuli input into chunks
			m_stimuliChunks = stimuli.GetChunks(chunkSize);
			// divide voxels input into chunks
			m_voxelChunks = voxels.GetChunks(chunkSize);

			return true;
		}

		private double[][] InitOutput(int voxel)
		{
			double[][] voxelData = new double[m_voxelChunks.Length][];
			for (int i = 0; i < m_voxelChunks.Length; i++)
			{
				voxelData[i] = new double[m_voxelChunks[i][voxel].Length];
				for (int j = 0; j < voxelData[i].Length; j++)
				{
					voxelData[i][j] = m_voxelChunks[i][voxel][j];
				}
			}

			return voxelData;
		}

		private globalParam.Data[] InitInput(double[][][] stimuliChunks, double neuronSpike, bool considerTR)
		{
			globalParam.Data[] input = new globalParam.Data[stimuliChunks.Length];
			for (int chunk = 0; chunk < stimuliChunks.Length; chunk++)
			{
				//Console.WriteLine("Chunk: " + chunk);
				input[chunk] = new globalParam.Data();
				input[chunk].Input = new double[stimuliChunks[0].Length, stimuliChunks[0][0].Length];

				for (int i = 0; i < stimuliChunks[0][0].Length; i++)
				{
					for (int j = 0; j < stimuliChunks[0].Length; j++)
					{
						if (j == 2 && considerTR)
							continue;
						input[chunk].Input[j, i] = stimuliChunks[chunk][considerTR ? j + 1 : j][i] == 1 ? neuronSpike * 4 : 0;
					}
				}
			}

			return input;
		}

        double Norm(double x, bool complete)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                x = 0;

            if (complete)
            {
                if (x >= 1)
                    x = x - 1;
                if (x <= -1)
                    x = x + 1;
            }
            return Math.Abs(x);
        }
    }

}
