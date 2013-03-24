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
using System.Windows.Forms;
using System.IO;
using System.Threading;
using SoundCatcher;
using Utils_Functions;
using Liquid;

namespace Liquid_Detector.Tests.Sound.IDsound
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class IDsounds
	{
		
		public static void MainProgram(string[] args)
		{
			globalParam Param = new globalParam();
			Param.initialization();
			Param.test = new Liquid_Detector.Tests.Sound.Parameters.TestParameters4Sound(0);
			Param.input2liquid = new Liquid_Detector.Tests.Sound.Parameters.Input4Sound();
			Param.neuronParam =  new Liquid_Detector.Tests.Sound.Parameters.Neuron4Sound();
			Param.networkParam = new Liquid_Detector.Tests.Sound.Parameters.RecurrentNetwork4Sound( Param.neuronParam.Neuron_Model, ref Param );

			
			//cheking howmeny files need to be leadnd
			string path=@Param.test.soundFiles_toLearn[0];
			DirectoryInfo di = new DirectoryInfo(path);
			DirectoryInfo[] ListDirec = di.GetDirectories();
			int Num_Files_Learn=0;
			foreach(DirectoryInfo Dir in ListDirec)
			{
				FileInfo[] rgFiles = Dir.GetFiles("*.wav");
				Num_Files_Learn+= rgFiles.Length;
			}
			//...
			
			// Init the LSM Network
			Console.WriteLine("Creating Liquid Unit for Right Channle");
			Liquid.LSM RNet = new Liquid.LSM(ref Param);
			Console.WriteLine("Creating Liquid Unit for Left Channle");
			Liquid.LSM LNet = new Liquid.LSM(ref Param);
			// finish Init and creating Network

			Utils_Functions.Manipulation_On_Inputs soundToSpikes = new Manipulation_On_Inputs();
			
			globalParam.Data[] RData = new globalParam.Data[Num_Files_Learn];
			globalParam.Data[] LData = new globalParam.Data[Num_Files_Learn];
			int fileCounter=0;
			for(int counter = 0 ; counter < ListDirec.Length ; counter++)
			{
				FileInfo[] rgFiles = ListDirec[counter].GetFiles("*.wav");
				for (int i = 0; i < rgFiles.Length ; i++) {
					
					int[][] RightChannle,LeftChannle;
					Console.WriteLine("Loading:"+rgFiles[i].FullName);
					loadWaveFile(rgFiles[i].FullName,out RightChannle,out LeftChannle,RNet.InputSize,Param.test.MaxLevel);
					
					soundToSpikes.FrequencyFFTtoOnaryNormelize(ref Param,ref RData[fileCounter], ref RightChannle);
					soundToSpikes.FrequencyFFTtoOnaryNormelize(ref Param,ref LData[fileCounter], ref LeftChannle);
//					soundToSpikes.FrequencyFFTtoDirectInput(ref Param,ref RData[fileCounter], ref RightChannle);
//					soundToSpikes.FrequencyFFTtoDirectInput(ref Param,ref LData[fileCounter], ref LeftChannle);
					RData[fileCounter].Target = new double[]{counter};
					LData[fileCounter].Target = new double[]{counter};
					fileCounter++;
				}
			}
			
			// Training //
			double[] LastReturnError;
			Console.Write("Trainning Readout Unit on Right Liquid ");
			LastReturnError = RNet.Learn_Multiple_Targets(ref Param, ref RData,0);
			Matrix_Arithmetic sum = new Matrix_Arithmetic();
			double temp = sum.Vector_Average(LastReturnError);
			Console.WriteLine(" Average Training Error:"+temp.ToString());
			RNet.CleanCollectedData();
			
			Console.Write("Trainning Readout Unit on Left Liquid");
			LastReturnError = LNet.Learn_Multiple_Targets(ref Param, ref LData,0);
			temp = sum.Vector_Average(LastReturnError);
			Console.WriteLine(" Average Training Error:"+temp.ToString());
			LNet.CleanCollectedData();
			
			
			// Testing //
			
			//cheking howmeny files need to be Test
			path=@Param.test.soundFiles_toTest[0];
			di = new DirectoryInfo(path);
			ListDirec = di.GetDirectories();
			int Num_Files_Test=0;
			foreach(DirectoryInfo Dir in ListDirec)
			{
				FileInfo[] rgFiles = Dir.GetFiles("*.wav");
				Num_Files_Test+= rgFiles.Length;
			}
			//...
			
			RData = new globalParam.Data[Num_Files_Test];
			LData = new globalParam.Data[Num_Files_Test];
			fileCounter=0;
			for(int counter = 0 ; counter < ListDirec.Length ; counter++)
			{
				FileInfo[] rgFiles = ListDirec[counter].GetFiles("*.wav");
				for (int i = 0; i < rgFiles.Length ; i++) {
					
					int[][] RightChannle,LeftChannle;
					Console.WriteLine("Loading:"+rgFiles[i].FullName);
					loadWaveFile(rgFiles[i].FullName,out RightChannle,out LeftChannle,RNet.InputSize,Param.test.MaxLevel);
					
					soundToSpikes.FrequencyFFTtoOnaryNormelize(ref Param,ref RData[fileCounter], ref RightChannle);
					soundToSpikes.FrequencyFFTtoOnaryNormelize(ref Param,ref LData[fileCounter], ref LeftChannle);
//					soundToSpikes.FrequencyFFTtoDirectInput(ref Param,ref RData[fileCounter], ref RightChannle);
//					soundToSpikes.FrequencyFFTtoDirectInput(ref Param,ref LData[fileCounter], ref LeftChannle);
					RData[fileCounter].Target = new double[]{counter};
					LData[fileCounter].Target = new double[]{counter};
					fileCounter++;
				}
			}
			
			Console.WriteLine("Running Liquid... ");
			Console.WriteLine("Running LSM on Learining Vector");
			double[,] Right_DetectorOutput = new double[RData.Length,RNet.Number_Of_Detectors],
			Left_DetectorOutput = new double[RData.Length,RNet.Number_Of_Detectors];
			
			for (int detector = 0; detector < RNet.Number_Of_Detectors; detector++) {
				double[,,] Right_Detector,Left_Detector;
				
				RNet.Test( ref Param, ref RData , out Right_Detector,0,1, 0.0,new int[]{detector});
				LNet.Test( ref Param, ref LData , out Left_Detector ,0,1, 0.0,new int[]{detector});
				for (int j = 0; j < RData.Length; j++) {
					double tempL=0,tempR=0;
					for (int i = 0; i < Param.detector.How_Many_Windows; i++) {
						tempR+=Right_Detector[j,0,i];
						tempL+=Left_Detector[j,0,i];
					}
					Right_DetectorOutput[j,detector]  = tempR/Param.detector.How_Many_Windows;
					Left_DetectorOutput[j,detector]  =tempL/Param.detector.How_Many_Windows;
				}
			}
			for(int sample=0 ; sample<LData.Length ; sample++){
				Console.WriteLine("Left Target\t= {0}, ",LData[sample].Target[0]);
				for (int detector = 0; detector < LNet.Number_Of_Detectors; detector++) {
					Console.WriteLine("Detector {0} is \t = {1}",detector,Left_DetectorOutput[sample,detector]);
				}
			}
			
			Console.WriteLine();
			for(int sample=0 ; sample<RData.Length ; sample++){
				Console.WriteLine("Right Target\t= {0}, ",RData[sample].Target[0]);
				for (int detector = 0; detector < RNet.Number_Of_Detectors; detector++) {
					Console.WriteLine("Detector {0} is \t = {1}",detector,Right_DetectorOutput[sample,detector]);
				}
			}
			Console.WriteLine();
			
			Console.WriteLine("Finish");
			Console.ReadKey();

		}//-----------------------------------------------------------------------------------------------
		
		public static void return_Targets(globalParam.Data[] input,out double[] TargetVoxels,int task,int Negative){
			int size = input.Length;
			TargetVoxels = new double[size];
			for( int i = 0 ; i < size ; i++){
				TargetVoxels[i] = (input[i].Target[0]==task)? 1 : Negative ;
			}
			
		}
		
		public static void loadWaveFile(string file,out int[][]Left,out int[][] Right,int numOfInputUnits,int inputRange)
		{
			WaveStream wavStream = new WaveStream(new FileStream(file,FileMode.Open));
			byte[] wave = new byte[wavStream.Length];
			int countRead = wavStream.Read(wave,0,wave.Length);
			wavStream.Close();
			
			AudioFrame AudioProcess = new AudioFrame(false);
			
			int count=0,size = 16384; //16bit
			int temp = (wave.Length/size) +1 ;
			Left = new int[temp][];
			Right = new int[temp][];
			
			for(int i = 0 ; i < Left.Length ; i++)
			{
				byte[] buf = new byte[size];
				int intCount=0;
				for(int t = i ; t < (i+size) ; t++){
					if (t<wave.Length) buf[intCount]= wave[t];
					else buf[intCount]=0;
					intCount++;
				}
				
				AudioProcess.Process(ref buf);
				
				AudioProcess.RenderFrequencyDomainLeft(wavStream.Format.nSamplesPerSec,out Left[count],numOfInputUnits,inputRange);
				AudioProcess.RenderFrequencyDomainRight(wavStream.Format.nSamplesPerSec,out Right[count],numOfInputUnits,inputRange);
				
				count++;
			}
			
		}//-----------------------------------------------------------------------------------------------
		
	}
}
