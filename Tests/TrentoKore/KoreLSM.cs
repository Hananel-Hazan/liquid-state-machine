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
using System.Runtime.InteropServices;
using System.Threading;
using Utils_Functions;
using Liquid;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Encog.Neural.Networks;
using System.Collections.Generic;

namespace Liquid_Detector.Tests.TrentoKore
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	[Serializable()]
	public class TrentoKore
	{
		
		public static void Liquid(string[] args)
		{
			globalParam Param = new globalParam();
			Param.initialization();
			
			Console.WriteLine("Loading Stimuly... ");
			// load simulation data
			globalParam.Data[] stimuliData;
			LoadStimuli(args[0], out stimuliData, ref Param);
			
			int time = stimuliData[0].Input.GetLength(1);
			
			
			// run LSM
			Console.WriteLine("Run LSM...");
			
			int MaxInputDuration = 0;
			for (int i = 0; i < stimuliData.Length ; i++){
				if (MaxInputDuration < stimuliData[i].Input.GetLength(1))
					MaxInputDuration = stimuliData[i].Input.GetLength(1);
			}
			MaxInputDuration = Param.input2liquid.ComputeInputTime(MaxInputDuration);
			
			LSM lsmNet = new LSM(ref Param);
			bool[][,] lsmOutputUnits = new bool[stimuliData.Length][,],
			inputSignal = new bool[stimuliData.Length][,];
			bool[][] inputNeurons = new bool[stimuliData.Length][];
			double[][,] outputFrequency = new double[stimuliData.Length][,];
			
			
			
			lsmNet.InputTuning(ref Param, ref stimuliData, 0,3);
			for (int i = 0; i < stimuliData.Length; i++){
				lsmNet.run_on_vector(ref stimuliData[i], out lsmOutputUnits[i], out inputNeurons[i], out outputFrequency[i], out inputSignal[i], 1, 0, 2, ref Param);
				lsmNet.reset(ref Param);
			}
			
			
//			for (int i = 0 ; i < stimuliData.Length ; i++){
//				plotxy p = new plotxy();
//				p.loadData(ref lsmOutputUnits,ref inputNeurons,ref inputSignal,i,ref Param );
//				Application.Run( p );
//				Application.Exit();
//			}
			
			Stream fileStream = new FileStream(args[1], FileMode.Create,FileAccess.ReadWrite, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			
			binaryFormater.Serialize(fileStream,lsmOutputUnits.Clone());
			fileStream.Flush();
			binaryFormater.Serialize(fileStream,stimuliData.Clone());
			fileStream.Flush();
			binaryFormater.Serialize(fileStream,Param.copy());
			fileStream.Flush();
			
			fileStream.Close();
			
			Console.WriteLine("Finish Run LSM.");
			
		}//-----------------------------------------------------------------------------------------------
		
		
		public static void MLP(string[] args)
		{
			globalParam.Data[] stimuliData = new globalParam.Data[0];
			bool[][,] ActivationPatters = new bool[0][,];
			globalParam Param = new globalParam();
			ReadOut_Detector ML;
			
			// Read Activity pattern
			Stream fileStream = new FileStream(args[0],FileMode.Open,FileAccess.Read, FileShare.Read);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			ActivationPatters = (bool[][,]) binaryFormater.Deserialize(fileStream);
			stimuliData = (globalParam.Data[]) binaryFormater.Deserialize(fileStream);
			Param = (globalParam) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			
			int inputSize = ActivationPatters.GetLength(1);
			
			ML = new ReadOut_Detector(inputSize,
			                          (int) Convert.ToInt32(Math.Round(Param.detector.ReadOut_Unit_HiddenLayerSize * inputSize)),
			                          1,0,ref Param);
			
			ML.initialization(ref Param,0);
			
			
			if (string.Compare(args[3],"test")==0){
				// Test run
				ML.load(ref args[1]);
				LSM tempLSM = new LSM(ref Param);
				double[][] function =  new double[ActivationPatters.Length][];
				
				bool[] temp = new bool[]{};
				for (int i = 0; i < function.Length; i++) {
					function[i] = ML.StartAproximation(ref ActivationPatters, ref temp,ref tempLSM, ref Param);
				}
				
				// create a writer and open the file
				TextWriter tw = new StreamWriter(args[2]);
				for (int  r = 0;  r < function[0].Length;  r++) {
					for (int line = 0; line < function.Length; line++) {
						// write a line of text to the file
						tw.Write(function[line][r].ToString()+" ");
					}
					tw.WriteLine("");
				}

				// close the stream
				tw.Close();
				
				
			}else if (string.Compare(args[3],"train")==0){
				// Learn run
				Console.WriteLine("Loading Voxel... ");
				// load Voxal data
				double[][] voxel = new double[0][];
				LoadVoxel(args[1], ref voxel , ref Param);
				LSM tempLSM = new LSM(ref Param);
				
				bool[] temp = new bool[]{};
				for (int i = 0; i < voxel.Length; i++) {
					int NumberOfVoxels = ActivationPatters.Length;
					ML.StartLearnAproximation(ref ActivationPatters[i],ref temp,ref voxel[i],ref tempLSM, ref Param);
				}
				
				// save Veriables to file
				ML.copy(ref args[2]);
			}
		}//-----------------------------------------------------------------------------------------------

		
		public static void Coralation(string[] args){
			LiquidHRF.Comparator c = new LiquidHRF.Comparator();
			c.CalculateMetrics(args[0],args[1]);
		}
		
		
		public static void  LoadStimuli(string dataPath, out globalParam.Data[] input, ref globalParam Param )
		{
			int rowCount = 0;
			int columnCount = 0;
			StreamReader sr = new StreamReader(dataPath);
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("#"))
				{
					continue;
				}

				if (columnCount == 0)
				{
					string[] words = line.Split( line.Contains("\t") ? '\t' : ' ');
					columnCount = words.Length;
				}

				rowCount++;
			}
			
//			input = new globalParam.Data[columnCount];
//			for (int row  = 0; row  < input.Length; row ++) {
//				input[row] = new globalParam.Data();
//				input[row].Input = new double[1,rowCount];
//			}
			
			input = new globalParam.Data[1];
			for (int row  = 0; row  < input.Length; row ++) {
				input[0] = new globalParam.Data();
				input[0].Input = new double[columnCount-1,rowCount];
			}

			int rowIdx = 0;
			sr = new StreamReader(dataPath);
			
			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("#"))
				{
					continue;
				}

				string[] cols = line.Split(line.Contains("\t") ? '\t' : ' ');
				int colIdx = 0,flag=0;
				foreach (string col in cols)
				{
					if (col.Length == 0) continue;
					if (flag==0){ flag++; continue;}
					input[0].Input[colIdx,rowIdx] = Convert.ToDouble(col) == 1 ? Param.neuronParam.Ext_Inp_Neuron_Spike : 0;
					colIdx++;
				}
				rowIdx++;
			}
			
		}//-----------------------------------------------------------------------------------------------
		
		
		public static void  LoadVoxel(string dataPath, ref double[][] input, ref globalParam Param )
		{
			int rowCount = 0;
			int columnCount = 0;
			StreamReader sr = new StreamReader(dataPath);
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("#"))
				{
					continue;
				}

				if (columnCount == 0)
				{
					string[] words = line.Split( line.Contains("\t") ? '\t' : ' ');
					columnCount = words.Length;
				}

				rowCount++;
			}
			
			input = new double[columnCount][];
			
			for (int row  = 0; row  < input.Length ; row ++) {
				input[row] = new double[rowCount];
			}

			int rowIdx = 0;
			sr = new StreamReader(dataPath);
			
			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("#"))
				{
					continue;
				}

				string[] cols = line.Split(line.Contains("\t") ? '\t' : ' ');
				int colIdx = 0;
				
				foreach (string col in cols)
				{
					if (col.Length == 0) continue;
					input[colIdx][rowIdx] = Convert.ToDouble(col);
					colIdx++;
				}
				rowIdx++;
			}
			
		}//-----------------------------------------------------------------------------------------------
	}
}