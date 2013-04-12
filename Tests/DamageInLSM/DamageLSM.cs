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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Liquid;
using Utils_Functions;

namespace Liquid_Detector.Tests.DamageLSM
{
	class Program
	{
		public static void MainProcess(string args)
		{
			globalParam Param = new globalParam();
			Param = Param.load(args);
			
			int iterationNumber = Param.iteration;
//			int iterationNumber=0;
//			for (int repitaition=0 ; repitaition<Param.test_Param.Repetition ; repitaition ++ ){
//				iterationNumber = repitaition;
//				Param.iteration =iterationNumber;
			
			Liquid.LSM Net = new LSM(ref Param);
			DamageLSM.Input Vector= new Input(ref Param,1),VectorII= new Input(ref Param,1);
			//open output files..
			string tempFileName, tempFileName2,
			tempFileName3 = @"Arch="+Param.networkParam.Liquid_Architecture.ToString();
			//open end
			for (int NoisePercent = Param.test_Param.NoisePercent ; NoisePercent >= -2 ; NoisePercent--) {
				
				if ((NoisePercent == 9) ||
				    (NoisePercent == 8) ||
				    (NoisePercent == 7) ||
				    (NoisePercent == 6) ||
				    (NoisePercent == 4) ||
				    (NoisePercent == 3) ||
				    (NoisePercent == 2)
				   ) continue;
				
				if (NoisePercent<0){
					if (NoisePercent==-1)
						Param.test_Param.LSM_Damage = 0.005;
					else if (NoisePercent==-2)
						Param.test_Param.LSM_Damage = 0.001;
				}else
					Param.test_Param.LSM_Damage = (double)NoisePercent/100;
				
				//open output files..
				tempFileName = iterationNumber.ToString();
				tempFileName2 = Param.test_Param.LSM_Damage.ToString();
				if (Directory.Exists (tempFileName3+@"/"+tempFileName2)==false)
					Directory.CreateDirectory(tempFileName3+@"/"+Param.test_Param.LSM_Damage.ToString());
				
				int counter = 0;
				for (int i = 0; i < Param.detector.ReadOut_Unit.Length; i++){
					string FileName = tempFileName3+@"/"+tempFileName2+@"/Result_on_Detector-"+i.ToString()+"-"+tempFileName+".txt";
					if (!File.Exists(FileName)) continue;
					FileInfo fInfo = new FileInfo(@FileName);
					long size = fInfo.Length;
					if (size>0) counter++;
				}
				if (counter==Param.detector.ReadOut_Unit.Length) continue;
				
				TextWriter[] reportFile = new TextWriter[Param.detector.ReadOut_Unit.Length];
				for (int i = 0; i < Param.detector.ReadOut_Unit.Length; i++) {
					reportFile[i] = new StreamWriter(tempFileName3+@"/"+tempFileName2+@"//Result_on_Detector-"+i.ToString()+"-"+tempFileName+".txt");
					reportFile[i].WriteLine("Neurons in the network :"+Param.networkParam.Number_Of_Neurons.ToString());
				}

				Console.WriteLine("!Start the program! {0} Degree of Damage {1}",iterationNumber,Param.test_Param.LSM_Damage);
				//-----------------------------------------------------------------------------
				
				if (NoisePercent == Param.test_Param.NoisePercent){
					//-----------------------------------------------------
					// Create Input
					Vector = new Input(ref Param,1);
					VectorII = new Input(ref Param,1);
					
					if (Param.test_Param.BinaryInput_OR_RealValuse==0){
						Vector.SetInputToBinaryRandom(ref Param);
						VectorII.SetInputToBinaryRandom(ref Param);
					}else if (Param.test_Param.BinaryInput_OR_RealValuse==1){
						Vector.SetInputToRandomValues(ref Param,Param.test_Param.RandomMaxMin_RealValues[1],Param.test_Param.RandomMaxMin_RealValues[0]);
						VectorII.SetInputToRandomValues(ref Param,Param.test_Param.RandomMaxMin_RealValues[1],Param.test_Param.RandomMaxMin_RealValues[0]);
					}
					
					//				Vector.SetInputToPattern(new int[]{471,599,71,400,430,371});
					// Finish Create Input
					
					//---------------------------------------------------------------
					// Init the LSM Network
					Net = new LSM(ref Param);
					//				Net.changeTheresholdForInputNeurons(ref Param,1);
					// finish Init and creating Network

					//--------------------------------------------------
					Console.WriteLine("Running LSM on Learning Vector");
					
					//				Manipulation_On_Inputs inputManimulation = new Manipulation_On_Inputs();
					//				inputManimulation.DuplicateInputToNeg(ref Vector.InputVec,ref Param);
					
					int print =  0 ;
					Net.FullReset(ref Param);
					
					Net.SilentTuneLiquid(ref Param,10 * Param.detector.LSM_1sec_interval,Param.test_Param.Silent_Tuning,print);
					Net.InputTuning(ref Param,ref Vector.InputVec,print,Param.test_Param.Input_Tuning);
					Net.Dump_Network_Weights(@tempFileName3);
					double[] LastReturnError;
					LastReturnError = Net.Learn(ref Param,ref Vector.InputVec, 0 ,1);
//					Net.save(@tempFileName3+@"/TempLSMnet"+"_Repitition="+Param.iteration.ToString()+".obj");
					//---------------------------------------------------------------
				}
				double[] targetLearnData;
				double middle = (1.0 + Param.detector.Readout_Negative)/2;
				for (int testDamage = 1 ; testDamage < 7; testDamage++) {
//					Net = Net.load(@tempFileName3+@"/TempLSMnet"+"_Repitition="+Param.iteration.ToString()+".obj");
					Net.reset(ref Param);
					
					double[][][] ReadOut;
					
					int Presult,Nresult,results;
					
					// NorGorD =-> 0) Generalization Test 1) Normal behaviral 2) Noise Damage 3) Dead Damage 4) Combine Damage
					if (testDamage==4) //random vector
						Net.Test(ref Param , ref VectorII.InputVec, out ReadOut, 0,0,Param.test_Param.LSM_Damage);
					else if (testDamage==5) // Generalization
						Net.Test(ref Param , ref Vector.InputVec, out ReadOut, 0,0,Param.test_Param.LSM_Damage);
					else if (testDamage==6) // Combine Damage
						Net.Test(ref Param , ref Vector.InputVec, out ReadOut, 0,4,Param.test_Param.LSM_Damage);
					else // 1,2,3
						Net.Test(ref Param , ref Vector.InputVec, out ReadOut, 0,testDamage,Param.test_Param.LSM_Damage);
					
					Vector.returnTargetData(out targetLearnData);
					int inputLengh = targetLearnData.Length;
					
					for (int report = 0; report < Net.Number_Of_Detectors ; report++) {
						
						Presult=0;Nresult=0;results=0;
						
						for (int Vec = 0; Vec < ReadOut.Length ; Vec++){
							double average = 0;
							int windows = ReadOut[Vec][report].Length;
							counter=0;
							for (int w = 0; w < windows ; w++){
								if ((ReadOut[Vec][report][w]==0)&&(Param.detector.ReadOut_Unit[report]==8))
									average+=Param.detector.Readout_Negative;
								else{
									if (ReadOut[Vec][report][w]==0){
										continue;
									}else{
										average+=ReadOut[Vec][report][w];
										counter++;
									}
								}
							}
							if (counter>0)
								average/=counter;
							
							if ((average>middle)&&(targetLearnData[Vec]>middle)) { results++;}
							else if ((average<=middle)&&(targetLearnData[Vec]<=middle)) { results++;}
							if (average>middle) {Presult++;} else {Nresult++;}
						}
						
						
						if (testDamage==1){
							Console.WriteLine("Start Normal Testing Vectors");
							reportFile[report].WriteLine("Results on learning Vectors : {0}/{1} ({2}+ , {3}-)",results,inputLengh,Presult,Nresult);
						}

						if (testDamage==2){
							Console.WriteLine("Start Noise Generator Testing Vectors");
							reportFile[report].WriteLine("Results on Noise Generator : {0}/{1} ({2}+ , {3}-)",results,inputLengh,Presult,Nresult);
						}
						if (testDamage==3){
							Console.WriteLine("Start Damage in LSM Testing Vectors");
							reportFile[report].WriteLine("Results on Damage in LSM : {0}/{1} ({2}+ , {3}-)",results,inputLengh,Presult,Nresult);
						}
						
						if (testDamage==4){
							Console.WriteLine("Start Testing Random Vectors");
							reportFile[report].WriteLine("Results on Random Vectors : {0}/{1} ({2}+ , {3}-)",results,inputLengh,Presult,Nresult);
						}
						
						if (testDamage==5){
							Console.WriteLine("Start Testing Generalization Vectors");
							reportFile[report].WriteLine("Results on Generalization Vectors : {0}/{1} ({2}+ , {3}-)",results,inputLengh,Presult,Nresult);
						}
						
						if (testDamage==6){
							Console.WriteLine("Start Testing Combine Damage Test");
							reportFile[report].WriteLine("Results on Combine Damage Test : {0}/{1} ({2}+ , {3}-)",results,inputLengh,Presult,Nresult);
						}
					}
				}
				// close the stream
				for (int i = 0; i < Param.detector.ReadOut_Unit.Length; i++) {
					reportFile[i].Flush();
					reportFile[i].Close();
				}
				//-----------------------------------------------------
			}
			File.Delete(@tempFileName3+@"/TempLSMnet"+"_Repitition="+Param.iteration.ToString()+".obj");
//			}
			File.Delete(args);
		}


		
		[STAThread]
		public static void DmageLSM_Main(string[] args)
		{
			if (args.Length>0){
				if (args.Length==2){
					Reports.ConectivityTest(args[0]);
				}
				else
					MainProcess(args[0]);
			}
			else{
				globalParam Param = new globalParam();
				Param.copyConfiguration("DamageLSM");
				Param.initialization();
				
				Process[] p;
				string[] pName,objFile;
				string cmd = Directory.GetCurrentDirectory();
				
				System.OperatingSystem osInfo = System.Environment.OSVersion;
				Console.WriteLine(osInfo.Platform);
				
				if ( osInfo.Platform== PlatformID.Win32NT) { Param.Linux_OR_Windows=2;}
				else {Param.Linux_OR_Windows=1;}
				
				p = new Process[Param.ThreadNum];
				pName = new string[Param.ThreadNum];
				objFile = new string[Param.ThreadNum];
				
				int architectors = Param.networkParam.Liquid_Architectures.Length;
				for (int arc = 0; arc < architectors ; arc++) {
					Param.initialization();
					Param.networkParam.Liquid_Architecture = Param.networkParam.Liquid_Architectures[arc];
					Param.networkParam.initArc(ref Param.rnd);
					for (int repitaition=0 ; repitaition<Param.test_Param.Repetition ; repitaition ++ ){
						Param.iteration =repitaition;
						
						int flag =0,counter=0;
						while (flag==0) {
							if (!File.Exists(objFile[counter])){
								Param.SeedInitialization();
								
								objFile[counter] = "obj"+arc.ToString()+"-"+repitaition.ToString()+".dat";
								
								Param.save(objFile[counter]);
								p[counter]= new Process();
								if (Param.Linux_OR_Windows==1){
									pName[counter]=@" --llvm --optimize=all 'Liquid Detector.exe' "+objFile[counter];
									p[counter].StartInfo.FileName=@"/opt/novell/mono/bin/mono";
								}else{
									pName[counter]=objFile[counter];
									p[counter].StartInfo.FileName=@"Liquid\ Detector.exe";
								}
//							if (arc==0)
//								p[counter].StartInfo.Arguments=pName[counter]+"  2";
//							else
								p[counter].StartInfo.Arguments=" "+pName[counter];
								System.Console.WriteLine("+++{0}+++",arc);

								if (Param.Linux_OR_Windows==1)
								{
									p[counter].Start();
								}else{
									string arg = objFile[counter];
									if (arc==Param.test_Param.NoisePercent){
										//Reports.ConectivityTest(arg); // for Dubuging
										File.Delete(arg);
										flag=1;
										continue;
									}
									if (Param.ThreadNum==1){
										MainProcess(arg); // for Dubuging
										File.Delete(arg);
									}else
										p[counter].Start();
								}
								flag=1;
								p[counter].Dispose();
							}
							if ((counter+1)>=Param.ThreadNum) {Thread.Sleep(10000); counter=0;}
							else counter++;
						}
					}
				}
				
				
				DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());
				FileInfo[] rgFiles = di.GetFiles("*.dat");
				while (rgFiles.Length>0) {
					Console.Write("Waiting....");
					Thread.Sleep(10000);
					rgFiles = di.GetFiles("*.dat");
				}
				Console.WriteLine("Finising....");
				Reports.Summing(cmd,ref Param);
				
			}
		}
	}
}