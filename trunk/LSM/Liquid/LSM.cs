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

using NeuronNetwork;
using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;



namespace Liquid
{
	[Serializable()]
	public class LSM
	{

		public int Netsize;
		public int NetOutputSize;
		public int Number_Of_Detectors;
		public int[] ConnectionDistribution_Input,ConnectionDistribution_Output;
		NeuronNetwork.Network LSMnet;
		ReadOut_Detector[] NN;
		public int[] groupNames;
		
		public LSM(ref globalParam Param){
			this.Netsize=Param.networkParam.Number_Of_Neurons;
			ConnectionDistribution_Input = new int[this.Netsize];
			ConnectionDistribution_Output = new int[this.Netsize];
			this.LSMnet = new Network(ref this.NetOutputSize , ref Param, ref this.ConnectionDistribution_Input,ref this.ConnectionDistribution_Output);
			this.Number_Of_Detectors = Param.detector.ReadOut_Unit.Length;
			
			// finish the Detector
			this.FullReset(ref Param);
			
		}//--------------------------------------------------------------------
		
		public void changeTheresholdForInputNeurons(ref globalParam Param){
			this.LSMnet.changeTheresholdForInputNeurons(ref Param);
		}
		
		public LSM returnRef()
		{
			return (this);
		}//----------------------------------------------------------------
		
		public void Dump_Network_Weights(string path){
			LSMnet.Dump_Network_Weights(path);
		}
		
		public LSM copy(){
			LSM source = (LSM) this.MemberwiseClone();
			return(source);
		}//----------------------------------------------------------------------------------
		
		public void save(string filename){
			Stream fileStream = new FileStream(filename, FileMode.Create,FileAccess.ReadWrite, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			binaryFormater.Serialize(fileStream,this.copy());
			fileStream.Flush();
			fileStream.Close();
		}//----------------------------------------------------------------------------------
		
		public LSM load(string args){
			Stream fileStream = new FileStream(args, FileMode.Open,FileAccess.Read, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			LSM net = (LSM) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			return net;
		}//----------------------------------------------------------------------------------
		
		
		public void FullReset(ref globalParam Param ){
			this.Netsize=Param.networkParam.Number_Of_Neurons;
			this.Number_Of_Detectors = Param.detector.ReadOut_Unit.Length;
			
			this.LSMnet.FullReset(ref Param);
			
			// init the Detector
			NN = new ReadOut_Detector[Param.detector.ReadOut_Unit.Length];
			this.groupNames = new int[Param.detector.ReadOut_Unit.Length];
			for (int i = 0; i < NN.Length; i++) {
				this.NN[i] = new ReadOut_Detector(this.NetOutputSize,((int) Math.Round(this.NetOutputSize*Param.detector.ReadOut_Unit_HiddenLayerSize)),Param.detector.ReadOut_Unit_outputSize,i,ref Param);
				NN[i].initialization(ref Param,0);
				this.groupNames[i] = i;
			}
			
			// finish the Detector
		}//--------------------------------------------------------------------
		
		public int TimeInitialization(int Input_Length, ref globalParam Param)
		{
			int Window_Size = Param.detector.ReadoutU_Window_Size;
			if (Param.detector.ReadoutU_Window_Size<=0)
				Window_Size = Math.Max(1,Math.Abs(Param.detector.ReadoutU_Window_Size)) * Param.neuronParam.Steps_Between_Two_Spikes;
			
			int windowSize = Window_Size +
				Param.detector.ReadoutU_Disctance_Between_Windows +
				Param.detector.ReadoutU_Window_Shifting;
			int Real_Inpur_Length = Param.input2liquid.ComputeInputTime(Input_Length);
			
//			int backTime = Param.detector.LSM_1sec_interval;
			
//			// one read out window must be smaller then a 1 sec, if window is larger then 1sec increse 1sec
//			while(Param.detector.LSM_1sec_interval%Window_Size!=0){
//				Param.detector.LSM_1sec_interval++;
//			}
			
//			if (backTime!=Param.detector.LSM_1sec_interval)
//				Param.neuronParam.initialization(ref Param);
			
			int Liquid_Minimum_RunTime = (Param.detector.LSM_Runing_Interval * Param.detector.LSM_1sec_interval);
			int Liquid_Run_Time=0;
			
			if (Param.detector.Readout_Activity_during_Input_time==0){ // readout is NOT during input time
				if (Param.detector.Approximate_OR_Classification==1){ //Approximate
//					Liquid_Run_Time += (windowSize*Input_Length) + Real_Inpur_Length;
					Liquid_Run_Time = Math.Max(Real_Inpur_Length,(windowSize*Input_Length))+Liquid_Minimum_RunTime;
				}else if (Param.detector.Approximate_OR_Classification==2){ // Classification
					Liquid_Run_Time = Real_Inpur_Length + 1*windowSize;
				}
			}else{// readout is on during input time
				if (Param.detector.Approximate_OR_Classification==1){  //Approximate
					Liquid_Run_Time = windowSize * Real_Inpur_Length;
					while(Liquid_Run_Time % Param.detector.LSM_1sec_interval!=0){
						Liquid_Run_Time++;
					}
				}else if (Param.detector.Approximate_OR_Classification==2){ // Classification
					if (Liquid_Run_Time < Real_Inpur_Length){
						Liquid_Run_Time = Real_Inpur_Length;
						
					}else if (Liquid_Run_Time > Real_Inpur_Length){
					}
				}
			}
			
			if (Param.detector.LSM_Adjustment_Time_at_Beginning>0)
				Liquid_Run_Time +=Param.detector.LSM_Adjustment_Time_at_Beginning;
			if (Param.detector.LSM_Adjustment_Time_at_Ending>0)
				Liquid_Run_Time +=Param.detector.LSM_Adjustment_Time_at_Ending;
			
			while(Liquid_Run_Time % Param.detector.LSM_1sec_interval!=0){
				Liquid_Run_Time++;
			}
			
			return Liquid_Run_Time;
		}
		

		public Int64[] run_on_vector(ref globalParam.Data LearnVec , out bool[,] OutputVectot,out bool[] inputNueronsVec,
		                             out double[,] outputFrequency,out bool[,] inputSignal, int NorGorD, double percent,
		                             int LearnRun_Or_TestRun_Trace,ref globalParam Param)
		{
			int Liquid_Run_Time = TimeInitialization(LearnVec.Input.GetLength(1),ref Param);
			
			int tempFreq = (int) System.Math.Round((double)Liquid_Run_Time/Param.detector.LSM_1sec_interval,1,System.MidpointRounding.AwayFromZero);
			
			inputNueronsVec = new bool[this.Netsize];
			OutputVectot = new bool[this.Netsize, Liquid_Run_Time];

			outputFrequency = new double[NetOutputSize,tempFreq];
			inputSignal = new bool[this.Netsize-this.NetOutputSize,Liquid_Run_Time];
			
			int[] trackNeurons = new int[3];
			Param.rndA.select(0,Netsize,ref trackNeurons,ref Param);
			
			double[,] NeuronTrace = new double[trackNeurons.Length,Liquid_Run_Time];
			
			LSMnet.reset(ref Param);
			LSMnet.WhoIsInput(ref inputNueronsVec);
			Int64[] valu = LSMnet.run_on_vector(ref LearnVec.Input, ref OutputVectot,ref inputSignal,
			                                    ref outputFrequency,ref NeuronTrace, ref trackNeurons, Liquid_Run_Time , NorGorD, percent, ref Param);
			
			if (LearnRun_Or_TestRun_Trace==1)
				LSMnet.Save_Neurons_Stat(ref Param);
			else if(LearnRun_Or_TestRun_Trace==2){
				
			}else if(LearnRun_Or_TestRun_Trace==3){
				plotGraph p = new plotGraph();
				p.loadData(ref NeuronTrace);
				Application.Run( p );
				Application.Exit();
				plotxy pl = new plotxy();
				pl.loadData(ref OutputVectot,ref inputNueronsVec,ref inputSignal,ref Param );
				Application.Run( pl );
				Application.Exit();
				
				LSMnet.Save_Neurons_Stat(ref Param);
				
			}else if(LearnRun_Or_TestRun_Trace==4){
				plotGraph p = new plotGraph();
				p.loadData(ref NeuronTrace);
				Application.Run( p );
				Application.Exit();
				plotxy pl = new plotxy();
				pl.loadData(ref OutputVectot,ref inputNueronsVec,ref inputSignal,ref Param );
				Application.Run( pl );
				Application.Exit();
			}
			return(valu);
		}
		
		
		
		
//		public Int64[] run(ref globalParam.Data[] LearnVec , out bool[][,] OutputVectot,out bool[][] inputNueronsVec,
//		                   out double[][,] outputFrequency,out bool[][,] inputSignal, int NorGorD, double percent,
//		                   int LearnRun_Or_TestRun_Trace,ref globalParam Param)
//		{
//
//			OutputVectot = new bool[LearnVec.Length][,];
//			inputNueronsVec = new bool[LearnVec.Length][];
//
//			outputFrequency = new double[LearnVec.Length][,];
//			inputSignal = new bool[LearnVec.Length][,];
//
//
//			// Start run the LSM on the inputs
//			Int64[] ReturnValu = new Int64[]{0,0,0};
//			for (int i = 0; i <  LearnVec.Length ; i++){
//				Int64[] valu = run_on_vector(ref LearnVec[i], out OutputVectot[i], out inputNueronsVec[i],out outputFrequency[i],
//				                   out inputSignal[i],NorGorD,percent,LearnRun_Or_TestRun_Trace,ref Param);
//				ReturnValu[0]+=valu[0];
//				ReturnValu[1]+=valu[1];
//				ReturnValu[2]+=valu[2];
//			}
//
//			return ReturnValu;
//		}//--------------------------------------------------------------------
		
		public void StimulyTest(int times, out bool[,,] OutputVectot,out bool[,,] inputNueronsVec, out bool[,,] inputSignal, out double[,,] Frequency,ref globalParam Param)
		{
			OutputVectot = new bool[times, this.NetOutputSize, Param.test_Param.Activity_Test_Running_Time];
			inputNueronsVec = new bool[times, this.Netsize-this.NetOutputSize, Param.test_Param.Activity_Test_Running_Time];
			inputSignal = new bool[times, this.Netsize-this.NetOutputSize,Param.test_Param.Activity_Test_Running_Time];
			Frequency = new double[times, NetOutputSize, Param.test_Param.Activity_Test_Running_Time/Param.detector.LSM_1sec_interval];
			
			// Start run the LSM on the inputs
			for (int i = 0; i < times ; i++){
				LSMnet.reset(ref Param);
//				LSMnet.TimeStep(i, ref OutputVectot, ref inputNueronsVec, ref inputSignal,ref Frequency,2, ref Param);
			}

		}//--------------------------------------------------------------------
		

		public void reset(ref globalParam Param)
		{
			LSMnet.reset(ref Param);
		}//--------------------------------------------------------------------

		
		public void PrintNetwork()
		{
			LSMnet.PrintNetwork();
		}//--------------------------------------------------------------------
		
		public void DOTFile()
		{
			LSMnet.DOTFile();
		}//--------------------------------------------------------------------
		
		
		public void InputTuning(ref globalParam Param, ref globalParam.Data[] LearnData,int print,int repetition)
		{
			
//			int backup1 = Param.detector.LSM_Adjustment_Time_at_Beginning;
//			int backup2 = Param.detector.LSM_Adjustment_Time_at_Ending;
//
//			Param.detector.LSM_Adjustment_Time_at_Ending = 0;
//			Param.detector.LSM_Adjustment_Time_at_Beginning = 0;
			
			if ((Param.neuronParam.Active_STDP_rule==1)&&(repetition>0)){
				Int64[] last = new Int64[]{0,0,0};
//				double[] STDPChange = new double[]{Param.neuronParam.STDPMaxChange[0],Param.neuronParam.STDPMaxChange[1]};
				
				Console.WriteLine("Start Input tuning...");
				
				bool[] LiquidOutput_InputUnits_Learn = new bool[0];
				bool[,] LiquidOutput_OutputUnits_Learn = new bool[0,0];
				double[,] outputNeurons_Frequency_Learn = new double[0,0];
				bool[,] inputSignal = new bool[0,0];
				for (int  repet = 0; repet < repetition ; repet++) {
					Param.RandomaizeData(ref LearnData);
					
//					int vectors = 0;
					Int64[] tempChanges = new Int64[0];
					for (int vector = 0; vector < LearnData.Length ; vector++) {
						int ActivityStart = 0;
						for (int i = 0; i < inputSignal.GetLength(0); i++){
							for (int t = 0; t < inputSignal.GetLength(1); t++)
								if (inputSignal[i,t]){
								ActivityStart = t;
								break;
							}
							if (ActivityStart>0)
								break;
						}
						ActivityStart = ActivityStart / Param.detector.LSM_1sec_interval;
						
						this.reset(ref Param);
						tempChanges = this.run_on_vector( ref LearnData[vector], out LiquidOutput_OutputUnits_Learn,
						                                 out LiquidOutput_InputUnits_Learn,out outputNeurons_Frequency_Learn,
						                                 out inputSignal, 1, 0.0,1,ref Param);
						
//							// Activity Check  - START
//							int ActiveNeurons = 0;
//							int neuron;
//							for (neuron = 0; neuron < outputNeurons_Frequency_Learn.GetLength(0); neuron++) {
//								int i,NueronActivityCounter_1 = 0,NueronActivityCounter_2 = 0;
//								for (i = ActivityStart ; i < outputNeurons_Frequency_Learn.GetLength(1); i++) {
//									if (outputNeurons_Frequency_Learn[neuron,i] == 1  )
//										NueronActivityCounter_1++;
//									else if (outputNeurons_Frequency_Learn[neuron,i] > 1 ){
//										NueronActivityCounter_2++;NueronActivityCounter_1++;
//									}
//
//								}
//								if((NueronActivityCounter_1 >= 0.8 * (i - ActivityStart)) &&
//								   (NueronActivityCounter_2 >= 0.5 * (i - ActivityStart)))
//									ActiveNeurons++;
//							}
//							if (ActiveNeurons >= (neuron * 0.3))
//								vectors++;
					}
//						if (vectors==LearnData.Length)
//							break;
//						// Activity Check  - END
					
					

					Console.WriteLine("{3}) changes: LTD={0} , LTP={1} , threshold={2}", tempChanges[0] , tempChanges[1],tempChanges[2],repet);
					
					if (print==1){
						// ploting the activity of the liquid
						for (int i = 0 ; i < 1 ; i++){
							plotxy p = new plotxy();
							p.loadData(ref LiquidOutput_OutputUnits_Learn,ref LiquidOutput_InputUnits_Learn,ref inputSignal,ref Param );
							Application.Run( p );
							Application.Exit();
						}
					}
				}
				if (print==2){
					// ploting the activity of the liquid
					for (int i = 0 ; i < 1 ; i++){
						plotxy p = new plotxy();
						p.loadData(ref LiquidOutput_OutputUnits_Learn,ref LiquidOutput_InputUnits_Learn,ref inputSignal,ref Param );
						Application.Run( p );
						Application.Exit();
					}
				}
				LiquidOutput_InputUnits_Learn = null;
				LiquidOutput_OutputUnits_Learn = null;
				outputNeurons_Frequency_Learn = null;
				inputSignal = null;
				Console.WriteLine("Finish Input tuning...");
//				if (times==2){
//					Param.neuronParam.STDPMaxChange[0] = STDPChange[0] ;
//					Param.neuronParam.STDPMaxChange[1] = STDPChange[1] ;
//				}
				
			}
//			Param.detector.LSM_Adjustment_Time_at_Beginning = backup1;
//			Param.detector.LSM_Adjustment_Time_at_Ending = backup2;
		}//--------------------------------------------------------------------

		public void SilentTuneLiquid(ref globalParam Param,int inputLengh, int maxRepeteration, int print)
		{
			Int64[] changes = new Int64[]{0,0};
			Int64[] last = new Int64[]{0,0,0};
//			double[] STDPChange = new double[]{Param.neuronParam.STDPMaxChange[0],Param.neuronParam.STDPMaxChange[1]};
			
			globalParam.Data Vector = new globalParam.Data();
			Vector.Input = new double[1,inputLengh];
			Vector.Target = new double[1];
			Console.WriteLine("Start Silent tuning...");
			if ((Param.neuronParam.Slideing_Threshold>0)&&(Param.neuronParam.Active_STDP_rule==1)
			    &&(maxRepeteration>0)){
				int repet =0;
				bool[] LiquidOutput_InputUnits_Learn = new bool[0];
				bool[,] LiquidOutput_OutputUnits_Learn = new bool[0,0];
				double[,] outputNeurons_Frequency_Learn = new double[0,0];
				bool[,] inputSignal = new bool[0,0];
				while(true){
					this.reset(ref Param);
					changes = this.run_on_vector( ref Vector , out LiquidOutput_OutputUnits_Learn,
					                             out LiquidOutput_InputUnits_Learn,out outputNeurons_Frequency_Learn,
					                             out inputSignal, 1, 0.0,1,ref Param);
					Console.WriteLine("{3}) changes: LTD={0} , LTP={1} , threshold={2}", changes[0] , changes[1],changes[2],repet);
					if (repet==maxRepeteration) break;
					
					if (print==1){
						// ploting the activity of the liquid
						for (int i = 0 ; i < 1 ; i++){
							plotxy p = new plotxy();
							p.loadData(ref LiquidOutput_OutputUnits_Learn,ref LiquidOutput_InputUnits_Learn,ref inputSignal,ref Param );
							Application.Run( p );
							Application.Exit();
						}
					}
					repet++;
				}
				if (print==2){
					// ploting the activity of the liquid
					for (int i = 0 ; i < 1 ; i++){
						plotxy p = new plotxy();
						p.loadData(ref LiquidOutput_OutputUnits_Learn,ref LiquidOutput_InputUnits_Learn,ref inputSignal,ref Param );
						Application.Run( p );
						Application.Exit();
					}
				}
				LiquidOutput_InputUnits_Learn = null;
				LiquidOutput_OutputUnits_Learn = null;
				outputNeurons_Frequency_Learn = null;
				inputSignal = null;
				Console.WriteLine("Finish Silent tuning...");
//			Param.neuronParam.STDPMaxChange[0] = STDPChange[0] ;
//			Param.neuronParam.STDPMaxChange[1] = STDPChange[1] ;
			}
		}//--------------------------------------------------------------------
		
		
		public double[] LearnTwoGroup(ref globalParam Param, ref globalParam.Data[] LearnData)
		{
			globalParam.Data[] tempdata = new globalParam.Data[LearnData.Length];
			
			for (int i = 0; i < LearnData.Length; i++) {
				tempdata[i].Input = (double[,]) LearnData[i].Input.Clone();
				tempdata[i].Tag = LearnData[i].Tag;
				if (tempdata[i].Tag==groupNames[0])
					tempdata[i].Target = new double[]{1};
				else
					tempdata[i].Target = new double[]{Param.detector.Readout_Negative};
			}
			return(Learn(ref Param, ref tempdata,0,1, groupNames,groupNames.Length));
		}
		
		public double[] Learn(ref globalParam Param, ref globalParam.Data[] LearnData,int print,int repetition)
		{
			int[] detectors = new int[NN.Length];
			for (int i = 0; i < detectors.Length; i++) detectors[i] = this.groupNames[i];
			return Learn(ref Param, ref LearnData,print,repetition, detectors, groupNames.Length);
			
		}
		
		
		public double Learn(ref globalParam Param, ref globalParam.Data[] LearnData,int print,int repetition, int Detectors,int NumOfGroups)
		{
			double LastReturnError = 0 ;
			if (Param.networkParam.Liquid_Update_Sync_Or_ASync == 1)
				repetition = repetition * 10; // dont know why 4...
			if (Param.neuronParam.Randomize_initilaization == 1)
				repetition = repetition * 4; // dont know why 4...
			
			for (int  repet = 0; repet < repetition ; repet++) {
				
				Param.RandomaizeData(ref LearnData);
				
				for (int vector = 0; vector < LearnData.Length; vector++) {
					
					bool[] LiquidOutput_InputUnits_Learn = new bool[0];
					bool[,] LiquidOutput_OutputUnits_Learn = new bool[0,0];
					double[,] outputNeurons_Frequency_Learn = new double[0,0];
					bool[,] inputSignal = new bool[0,0];
					
					// The Detector need to learn not the Liquid!!!
					if (print>0)
						this.run_on_vector( ref LearnData[vector] , out LiquidOutput_OutputUnits_Learn, out LiquidOutput_InputUnits_Learn,
						                   out outputNeurons_Frequency_Learn,out inputSignal, 1 , 0.0 , 4 ,ref Param);
					else
						this.run_on_vector( ref LearnData[vector] , out LiquidOutput_OutputUnits_Learn, out LiquidOutput_InputUnits_Learn,
						                   out outputNeurons_Frequency_Learn,out inputSignal, 1 , 0.0 , 2 ,ref Param);
					
					this.reset(ref Param);
					
					LSM temp = this.returnRef();
					// Training Detector BY Sliceing Time
					NN[Detectors].CollectData(ref LiquidOutput_OutputUnits_Learn,ref LiquidOutput_InputUnits_Learn,
					                          ref LearnData[vector].Target,ref temp ,LearnData[vector].Input.GetLength(1), (vector==0),LearnData[vector].Tag,NumOfGroups ,ref Param);
//
//						if (print==1){
//							// ploting the activity of the liquid
//							plotxy p = new plotxy();
//							p.loadData(ref LiquidOutput_OutputUnits_Learn,ref LiquidOutput_InputUnits_Learn,ref inputSignal,ref Param );
//							Application.Run( p );
//							Application.Exit();
//						}
					LiquidOutput_InputUnits_Learn = null;
					LiquidOutput_OutputUnits_Learn = null;
					outputNeurons_Frequency_Learn = null;
					inputSignal = null;
				}
			}
			NN[Detectors].StratTrain(ref LastReturnError,ref Param);
			NN[Detectors].CleanCollectedData();
			
			return LastReturnError;
		}//--------------------------------------------------------------------
		
		
		public double[] Learn(ref globalParam Param, ref globalParam.Data[] LearnData,int print,int repetition,int[] Detectors,int NumOfGroups)
		{
			if (Param.networkParam.Liquid_Update_Sync_Or_ASync == 1)
				repetition = repetition * 10; // dont know why 4...
			if (Param.neuronParam.Randomize_initilaization == 1)
				repetition = repetition * 4; // dont know why 4...
			
			double[] LastReturnError = new double[Detectors.Length];
			
			
			for (int n = 0; n < Detectors.Length; n++){
				
				for (int  repet = 0; repet < repetition ; repet++) {
					
					Param.RandomaizeData(ref LearnData);
					
					for (int vector = 0; vector < LearnData.Length; vector++) {
						
						bool[] LiquidOutput_InputUnits_Learn = new bool[0];
						bool[,] LiquidOutput_OutputUnits_Learn = new bool[0,0];
						double[,] outputNeurons_Frequency_Learn = new double[0,0];
						bool[,] inputSignal = new bool[0,0];
						
						// The Detector need to learn not the Liquid!!!
						if (print>0)
							this.run_on_vector( ref LearnData[vector] , out LiquidOutput_OutputUnits_Learn, out LiquidOutput_InputUnits_Learn,
							                   out outputNeurons_Frequency_Learn,out inputSignal, 1 , 0.0 , 4 ,ref Param);
						else
							this.run_on_vector( ref LearnData[vector] , out LiquidOutput_OutputUnits_Learn, out LiquidOutput_InputUnits_Learn,
							                   out outputNeurons_Frequency_Learn,out inputSignal, 1 , 0.0 , 2 ,ref Param);
						
						this.reset(ref Param);
						
						LSM temp = this.returnRef();
						// Training Detector BY Sliceing Time
						NN[Detectors[n]].CollectData(ref LiquidOutput_OutputUnits_Learn,ref LiquidOutput_InputUnits_Learn,
						                             ref LearnData[vector].Target,ref temp ,LearnData[vector].Input.GetLength(1), (vector==0),LearnData[vector].Tag, NumOfGroups ,ref Param);

						LiquidOutput_InputUnits_Learn = null;
						LiquidOutput_OutputUnits_Learn = null;
						outputNeurons_Frequency_Learn = null;
						inputSignal = null;
					}
				}
				NN[Detectors[n]].StratTrain(ref LastReturnError[n],ref Param);
				NN[Detectors[n]].CleanCollectedData();
			}
			
			return LastReturnError;
		}//--------------------------------------------------------------------
		
		
		public void Test(ref globalParam Param, ref globalParam.Data[] testData , out double[][][] DetectorOutput,int print, int KindOfDamage,double degree)
		{
			int[] detectors = new int[NN.Length];
			for (int i = 0; i < detectors.Length; i++) detectors[i] =this.groupNames[i];
			Test(ref Param, ref testData , out DetectorOutput,print,KindOfDamage, degree, detectors);
		}
		
		public void Test(ref globalParam Param, ref globalParam.Data[] testData , out double[][] DetectorOutput,int print, int KindOfDamage,double degree,int Detector)
		{
			LSM temp = (Liquid.LSM) this.MemberwiseClone();
			
			DetectorOutput = new double[testData.Length][];
			
			for (int vec = 0; vec < testData.Length ; vec++) {
				bool[] LiquidOutput_InputUnits_Test;
				bool[,] LiquidOutput_OutputUnits_Test;
				double[,] outputNeurons_Frequency_Test;
				bool[,] inputSignal;
				
				this.run_on_vector( ref testData[vec] , out LiquidOutput_OutputUnits_Test, out LiquidOutput_InputUnits_Test,
				                   out outputNeurons_Frequency_Test,out inputSignal, KindOfDamage , degree ,2 ,ref Param);

				if (print==1){
					// ploting the activity of the liquid
					plotxy p = new plotxy();
					p.loadData(ref LiquidOutput_OutputUnits_Test,ref LiquidOutput_InputUnits_Test,ref inputSignal,ref Param );
					Application.Run( p );
					Application.Exit();
				}

				// testing Detector
				
				int inputsize = testData[vec].Input.GetLength(1);
				
				NN[Detector].StratTesting(ref LiquidOutput_OutputUnits_Test,ref LiquidOutput_InputUnits_Test
				                          ,ref temp,ref inputsize, out DetectorOutput[vec] ,ref Param);
				
				LiquidOutput_InputUnits_Test = null;
				LiquidOutput_OutputUnits_Test = null;
				outputNeurons_Frequency_Test = null;
				inputSignal = null;
			}
			
		}//--------------------------------------------------------------------
		
		
		
		public void Test(ref globalParam Param, ref globalParam.Data[] testData , out double[][][] DetectorOutput,int print, int KindOfDamage,double degree,int[] Detectors)
		{
			LSM temp = (Liquid.LSM) this.MemberwiseClone();
			
			DetectorOutput = new double[testData.Length][][];
			for (int v = 0; v < testData.Length ; v++)
				DetectorOutput[v] = new double[Detectors.Length][];
			
			for (int vec = 0; vec < testData.Length ; vec++) {
				bool[] LiquidOutput_InputUnits_Test;
				bool[,] LiquidOutput_OutputUnits_Test;
				double[,] outputNeurons_Frequency_Test;
				bool[,] inputSignal;
				
				this.run_on_vector( ref testData[vec] , out LiquidOutput_OutputUnits_Test, out LiquidOutput_InputUnits_Test,
				                   out outputNeurons_Frequency_Test,out inputSignal, KindOfDamage , degree ,2 ,ref Param);

				if (print==1){
					// ploting the activity of the liquid
					plotxy p = new plotxy();
					p.loadData(ref LiquidOutput_OutputUnits_Test,ref LiquidOutput_InputUnits_Test,ref inputSignal,ref Param );
					Application.Run( p );
					Application.Exit();
				}

				// testing Detector
				
				int inputsize = testData[vec].Input.GetLength(1);
				
				for (int d = 0; d < Detectors.Length ; d++)
					NN[Detectors[d]].StratTesting(ref LiquidOutput_OutputUnits_Test,ref LiquidOutput_InputUnits_Test
					                              ,ref temp,ref inputsize, out DetectorOutput[vec][Detectors[d]] ,ref Param);
				
				LiquidOutput_InputUnits_Test = null;
				LiquidOutput_OutputUnits_Test = null;
				outputNeurons_Frequency_Test = null;
				inputSignal = null;
			}
			
		}//--------------------------------------------------------------------
		
		
		public int MakeTargetinDB(ref globalParam Param, ref globalParam.Data[] LearnData, ref globalParam.Data[] TestData)
		{
			
			groupNames = new int[1]{0};
			for (int i = 0; i < LearnData.Length; i++) {
				int flag = 0;
				for (int l = 0; l < groupNames.Length; l++) {
					if (groupNames[l]==LearnData[i].Tag) { flag=1; break;}
				}
				if (flag==0){
					int size = groupNames.Length;
					Array.Resize(ref groupNames,size+1);
					groupNames[size] = size;
				}
			}
			
			if (groupNames.Length == 2){
				for (int i = 0; i < LearnData.Length ; i++){
					if (LearnData[i].Tag==groupNames[0])
						LearnData[i].Target = new double[]{1};
					else
						LearnData[i].Target = new double[]{-1};
				}
				for (int i = 0; i < TestData.Length ; i++){
					if (TestData[i].Tag==groupNames[0])
						TestData[i].Target = new double[]{1};
					else
						TestData[i].Target = new double[]{-1};
				}
				
			}
			
			
			return groupNames.Length;
			
			
			
		}//--------------------------------------------------------------------
		
		public double[] Learn_Multiple_Targets(ref globalParam Param, ref globalParam.Data[] LearnData,int print)
		{
			// init the Detector
			if (groupNames.Length==2){
				NN = new ReadOut_Detector[1];
				this.NN[0] = new ReadOut_Detector(this.NetOutputSize,((int) Math.Round(this.NetOutputSize*Param.detector.ReadOut_Unit_HiddenLayerSize)),Param.detector.ReadOut_Unit_outputSize,0,ref Param);
				NN[0].initialization(ref Param,groupNames.Length);
			}else{
				NN = new ReadOut_Detector[Param.detector.ReadOut_Unit.Length*groupNames.Length];
				int temp = 0;
				for (int t = 0; t < groupNames.Length; t++) {
					for (int i = 0; i < Param.detector.ReadOut_Unit.Length; i++) {
						this.NN[temp] = new ReadOut_Detector(this.NetOutputSize,((int) Math.Round(this.NetOutputSize*Param.detector.ReadOut_Unit_HiddenLayerSize)),Param.detector.ReadOut_Unit_outputSize,i,ref Param);
						NN[temp].initialization(ref Param,groupNames.Length);
					}
					temp++;
				}
			}
			// finish the Detector
			
			
			double[] output = new double[NN.Length];
			this.Number_Of_Detectors = NN.Length;
			
			int repetition = 1;
			if (Param.networkParam.Liquid_Update_Sync_Or_ASync == 1)
				repetition = repetition * 2; // dont know why 4...
			
			for (int n = 0; n < NN.Length; n++){
				output[n] = this.Learn(ref Param,ref LearnData,print,repetition, n,groupNames.Length);
				NN[n].CleanCollectedData();
			}
			return output;
		}//--------------------------------------------------------------------
		
		
		public double[,] Test_Multiple_Targets(ref globalParam Param, ref globalParam.Data[] TestData,int print,int damage,double degree)
		{
			// damage 0: gernealization, 1: Normal, 2: Noise, 3:Dead, 4:Combain Dead Noise
			double[,] output = new double[TestData.Length,this.NN.Length];
//			double[,] output = new double[TestData.Length,Param.detector.ReadOut_Unit.Length];
			
//			double[][][] Detector_Output ;
//			this.Test( ref Param, ref TestData , out Detector_Output,0,damage,degree);

			for (int detector = 0; detector < Number_Of_Detectors ; detector++) {
				double[][] Detector_Output ;
				this.Test( ref Param, ref TestData , out Detector_Output,print,damage,degree,detector);
				for (int vec = 0; vec < TestData.Length; vec++) {
					double temp=0;
					int count = 0;
					if (this.NN[detector].model==10){
						
					}else{
						for (int i = 0; i < Detector_Output[vec].Length ; i++) {
							if (Detector_Output[vec][i] == 0)
								continue; // IMPORTENT!!!! 0 is not an answare!!!
							temp+=Detector_Output[vec][i];
							count++;
						}
						if (count>0)
							output[vec,this.groupNames[detector]]  = temp/count;
					}
				}
			}
			return output;
		}//--------------------------------------------------------------------
		
		public void CleanCollectedData(){
			for (int i = 0; i < NN.Length; i++)
				NN[i].CleanCollectedData();
		}

	}

}
