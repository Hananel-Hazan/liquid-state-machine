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
using System.Threading;
using Liquid;


namespace Liquid_Detector.Tests.Sound.TIMIT
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class TIMIT
	{
		public static void MainProgram(string[] args)
		{
			globalParam Param = new globalParam();
			Param.loadTestName("TIMIT");
			Param.copyConfiguration("TIMIT");
			Param.initialization();
			
			
			globalParam.Data[] SoundData_to_Learn,SoundData_to_Test;
			
			//cheking howmeny files need to be leadnd
			string filepath;
			System.OperatingSystem osInfo = System.Environment.OSVersion;
			if ( osInfo.Platform == PlatformID.Unix)
				filepath=@"../Audio/";
			else
//				filepath= @"D:\Rimon.Docs\Data To Process\Sound\Alex\Hananel-Liquid-Net-Projects";
//				filepath= @"D:\Local User Directory\Dropbox\SharePoints\Alex & Hananel\Phoneme tests\2013.03.04\";
				filepath= @"e:\SharePoints\Alex & Hananel\Phoneme tests\2013.03.04\";
			//...
			int numFiles = 0;
			
			int raion=Param.input2liquid.LSM_Ratio_Of_Input_Interval[1], MaxRatio = raion;
			if (raion==-1){
				MaxRatio = 150;
				raion=1;
			}
			for(; raion <= MaxRatio ; raion++){
				
				Param.input2liquid.LSM_Ratio_Of_Input_Interval[1] = raion;
				
				for (int tests = 0; tests < Param.test_Param.DirNumber.Length ; tests++) {
					
					Param.test_Param.DirName = Param.test_Param.DirNumber[tests].ToString();
					string[] FileNames;
					string DirName = loadFile(filepath,out SoundData_to_Learn,out SoundData_to_Test, ref numFiles, out FileNames, ref Param);

					Param.SeedInitialization();
					
					
					int architectors = Param.networkParam.Liquid_Architectures.Length;
					for (int arc = 0; arc < architectors ; arc++) {
						Param.initialization();
						Param.networkParam.Liquid_Architecture = Param.networkParam.Liquid_Architectures[arc];
						Param.networkParam.initArc(ref Param.rnd);

						int currentTest=0, results=numFiles*Param.detector.ReadOut_Unit.Length;
						if (numFiles==2) results=1;
						cumulateor[][] Accuraqcy = new cumulateor[2][];
						cumulateor[][] TP = new cumulateor[2][];
						cumulateor[][] TN = new cumulateor[2][];
						for (int test = 0; test < 2 ; test++) {
							Accuraqcy[test] = new cumulateor[results];
							TP[test] = new cumulateor[results];
							TN[test] = new cumulateor[results];
							for (int i = 0; i < results ; i++) {
								Accuraqcy[test][i] = new cumulateor();
								TP[test][i] = new cumulateor();
								TN[test][i] = new cumulateor();
							}
						}
						currentTest = 0;
						
						File.Delete("Weights_arc_"+Param.networkParam.Liquid_Architectures[arc].ToString()+" Directory "+DirName);
						for (int loop = 0; loop < Param.test_Param.Repetition ; loop ++) {
							currentTest = 0;
							double[,] positiveID = new double[2,results],
							FalsePositiveID = new double[2,results],
							NegativeID = new double[2,results],
							FalseNegativeID = new double[2,results];
							double[] TotalSize = new double[2];
							currentTest = 0;
							for (int test = 0; test < 2 ; test++) {
								TotalSize[test] = 0;
								for (int i = 0; i < results ; i++) {
									positiveID[test,i] = 0;
									NegativeID[test,i] = 0;
									FalsePositiveID[test,i] = 0;
									FalseNegativeID[test,i] = 0;
									currentTest++;
								}
							}
							currentTest = 0;
							Console.WriteLine(" Test {0} from {1}",loop,Param.test_Param.Repetition);
							
//					TextWriter txtWriter = new StreamWriter("result-"+loop.ToString());
							// Init the LSM Network
							Console.WriteLine("Creating Liquid Units");
							Liquid.LSM LSM_Net = new Liquid.LSM(ref Param);
							// finish Init and creating Network
							
							// change the Input Nuerons to react for the same input in difrent way.
							// by changein all input neurons to diffrent thereshold
							if (Param.test_Param.Change_Threshold_Of_Input_Neurons==1)
								LSM_Net.changeTheresholdForInputNeurons(ref Param);
							
							LSM_Net.FullReset(ref Param);
							LSM_Net.Dump_Network_Weights("Weights_arc_"+Param.networkParam.Liquid_Architectures[arc].ToString()+" Directory "+DirName);
							LSM_Net.SilentTuneLiquid(ref Param,Param.detector.LSM_1sec_interval*1000,Param.test_Param.Silent_Tuning,0);
							LSM_Net.Dump_Network_Weights("Weights_arc_"+Param.networkParam.Liquid_Architectures[arc].ToString()+" Directory "+DirName);
							LSM_Net.InputTuning(ref Param,ref SoundData_to_Learn,0,Param.test_Param.Input_Tuning);
							LSM_Net.Dump_Network_Weights("Weights_arc_"+Param.networkParam.Liquid_Architectures[arc].ToString()+" Directory "+DirName);
							LSM_Net.SilentTuneLiquid(ref Param,Param.detector.LSM_1sec_interval*1000,Param.test_Param.Silent_Tuning,0);
							LSM_Net.Dump_Network_Weights("Weights_arc_"+Param.networkParam.Liquid_Architectures[arc].ToString()+" Directory "+DirName);
							

							// Training //
							double[] LastReturnError;
							int NumOfGroups=0;
							Console.WriteLine("Trainning Readout Units");
							NumOfGroups = LSM_Net.MakeTargetinDB(ref Param, ref SoundData_to_Learn, ref SoundData_to_Test);
							globalParam.Data[] T_Data = SoundData_to_Learn;
//							Param.AddNoiseToData_Y_axis(ref Param,ref T_Data,3,0.1);
//							LastReturnError = LSM_Net.Learn_Multiple_Targets(ref Param, ref SoundData_to_Learn,0);
							LastReturnError = LSM_Net.Learn_Multiple_Targets(ref Param, ref T_Data,0);
							
							
							// Testing //
							Console.WriteLine("Running Liquid... ");
							Console.WriteLine("Running LSM on Learining Vector");
							
							for (int test = 0; test < 2 ; test++) {
								LSM_Net.reset(ref Param);
								
								globalParam.Data[] Test_Data;
								
								double[,] DetectorOutput;
								if(test==0){
									Console.WriteLine("Testing Learned Data... ");
									Test_Data = SoundData_to_Learn;
									DetectorOutput = LSM_Net.Test_Multiple_Targets(ref Param,ref Test_Data,0,1,0.0);
									currentTest = 0;
								}else{
									Console.WriteLine("Testing Test Data... ");
									Test_Data = SoundData_to_Test;
//									Test_Data = SoundData_to_Learn;
									Param.AddNoiseToData_Y_axis(ref Param,ref Test_Data,2,0.1);
									DetectorOutput = LSM_Net.Test_Multiple_Targets(ref Param,ref Test_Data,0,1,0.0);
									currentTest = 1;
								}
								TotalSize[test] = Test_Data.Length;
								
								double hulf = (Param.detector.Readout_Negative + 1) /2.0;
								int samples = DetectorOutput.GetLength(0), detectors = DetectorOutput.GetLength(1);
								for (int s = 0; s < samples; s++) {
									for (int d = 0; d < detectors; d++){
										if (Param.detector.ReadOut_Unit[d]==10){
											// One Class Clasfication //
											if (Test_Data[s].Tag==1) {
												if (DetectorOutput[s,d]==1)
													positiveID[test,d]++;
												else
													FalseNegativeID[test,d]++;
											}else if(Test_Data[s].Tag==0) {
												if (DetectorOutput[s,d]==0)
													NegativeID[test,d]++;
												else
													FalsePositiveID[test,d]++;
											}
										}else{
											// Binary Calssfication//
											if (Test_Data[s].Target[0]>=hulf) {
												if ((DetectorOutput[s,d]>hulf)&&(Test_Data[s].Target[0]>=hulf))
													positiveID[test,d]++;
												else
													FalseNegativeID[test,d]++;
											}else{
												if ((DetectorOutput[s,d]<hulf)&&(Test_Data[s].Target[0]<hulf))
													NegativeID[test,d]++;
												else
													FalsePositiveID[test,d]++;
											}
										}
									}
								}
								
								
							}
							for (int d = 0; d < results; d++){
								for (int i = 0; i < 2 ; i++) {
									Accuraqcy[i][d].Add((NegativeID[i,d]+positiveID[i,d])/TotalSize[i]);
									TP[i][d].Add(positiveID[i,d]/(positiveID[i,d]+FalseNegativeID[i,d]));
									TN[i][d].Add(NegativeID[i,d]/(NegativeID[i,d]+FalsePositiveID[i,d]));
								}
							}
							
							TextWriter finalReport = new StreamWriter("Final_Report_arc_"+Param.networkParam.Liquid_Architectures[arc].ToString()+" Directory "+DirName+" Ratio "+raion.ToString()+".txt");
							for (int d = 0; d < results; d++){
								finalReport.Flush();
								finalReport.WriteLine();
								finalReport.WriteLine();
								finalReport.WriteLine();
								finalReport.WriteLine("----------------------------------------------------------------------------");
								finalReport.WriteLine("File Names:");
								for (int i = 0; i < FileNames.Length; i++)
									finalReport.WriteLine("1) "+FileNames[i]);
								
								for (int i = 0; i < 2 ; i++) {
									
									if (i==0)
										finalReport.WriteLine("____________________Train_________________________");
									else
										finalReport.WriteLine("____________________Test_________________________");
									finalReport.WriteLine("Results");
									finalReport.WriteLine("(TruePositive/(TruePositive+FalseNegative))	=>	Average="+TP[i][d].Return_Average().ToString()+" Standard deviation="+TP[i][d].Return_Standard_Deviation().ToString());
									finalReport.WriteLine("(TrueNegative/(TrueNegative+FalsePositive))	=>	Average="+TN[i][d].Return_Average().ToString()+" Standard deviation="+TN[i][d].Return_Standard_Deviation().ToString());
									finalReport.WriteLine("Accuraqcy	=>	Average=" + (Accuraqcy[i][d].Return_Average()).ToString()+" Standard deviation="+(Accuraqcy[i][d].Return_Standard_Deviation()).ToString());
									finalReport.WriteLine("----------------------------------------------------------------------------");
									finalReport.Write("Accuraqcy:");
									for (int j = 0; j < Accuraqcy[i][d].members.Length; j++)
										finalReport.Write("	"+Accuraqcy[i][d].members[j].ToString());
									finalReport.WriteLine("");
									finalReport.Write("TruePositive:");
									for (int j = 0; j < Accuraqcy[i][d].members.Length; j++)
										finalReport.Write("	"+TP[i][d].members[j].ToString());
									finalReport.WriteLine("");
									finalReport.Write("TrueNegative:");
									for (int j = 0; j < Accuraqcy[i][d].members.Length; j++)
										finalReport.Write("	"+TN[i][d].members[j].ToString());
									finalReport.WriteLine("");
								}
								finalReport.WriteLine("----------------------------------------------------------------------------");
								finalReport.WriteLine(" Namber of tests: "+Accuraqcy[0][d].ReturnNamberOfMembers().ToString());
								finalReport.Flush();
							}
							finalReport.Close();
						}
					}
				}
				Console.WriteLine("Finish");
			}

		}//-----------------------------------------------------------------------------------------------
		
		public static string loadFile(string files,out globalParam.Data[] SoundData2Learn, out globalParam.Data[] SoundData2Test,ref int numFiles, out string[] FileName ,ref globalParam Param)
		{
			
			FileName = new string[0];
			
			DirectoryInfo di = new DirectoryInfo(files);
			DirectoryInfo[] drFiles = di.GetDirectories();
			
			string DirName = drFiles[Convert.ToInt32(Param.test_Param.DirName)].Name;
			
			SoundData2Learn = new globalParam.Data[0];
			SoundData2Test = new globalParam.Data[0];
			
			double factor = Param.neuronParam.Ext_Inp_Neuron_Spike ;
			
			double LearnRaio = Param.test_Param.Learn_Raio;
			int lerningExamples = 1 ;
			
			int LearnSum=0,TestSum=0;
			TextReader tr;
			string input;
			
			di = new DirectoryInfo(@files+@"/"+DirName);
			FileInfo[] rgFiles = di.GetFiles("*.txt");
			int[] lines = new int[rgFiles.Length], rows= new int[rgFiles.Length],
			learnSize = new int[rgFiles.Length],testSize = new int[rgFiles.Length];
			numFiles = 0;
			int Target=0;
			
			int persentagesORtwoFiles = 0 ; // 0 = presentages , 1 = two files.
			for (int f = 0; f < rgFiles.Length; f++)
				if (rgFiles[f].Name.Contains("Train")) {
				persentagesORtwoFiles = 1 ;
				break;
			}
			
			
			
			
			for (int f = 0; f < rgFiles.Length; f++) {
				numFiles++;
				int place = FileName.Length;
				Array.Resize(ref FileName,place+1);
				FileName[place] = rgFiles[f].Name;
				
				Console.WriteLine(rgFiles[f].FullName);
				// create a writer and open the file
				tr = rgFiles[f].OpenText();
				
				

				// write a line of text to the file
				while ((input = tr.ReadLine()) != null){
					string[] row = input.Split("	".ToCharArray());
					lines[f] = Convert.ToInt32(row[0]);
					rows[f] = Convert.ToInt32(row[1]);
					Param.test_Param.dataQunta = Convert.ToInt32(row[2]);
					break;
				}tr.Close();
				
				if (persentagesORtwoFiles==0){
					if (LearnRaio==0)
						learnSize[f] = lerningExamples;
					else
						learnSize[f] = (int) Math.Round((lines[f]-1)*LearnRaio);
					
					testSize[f] = lines[f] - learnSize[f];
					LearnSum += learnSize[f];
					TestSum += testSize[f];
				}else if (persentagesORtwoFiles==1){
					if (rgFiles[f].Name.Contains("Train"))
						LearnSum += lines[f];
					else
						TestSum += lines[f];
				}
				
			}
			
			SoundData2Learn = new globalParam.Data[LearnSum];
			SoundData2Test = new globalParam.Data[TestSum];

			int TrainI=0,TestI=0;
			for (int f = 0; f < rgFiles.Length; f++) {
				
				if (persentagesORtwoFiles==0){
					int[] trainIndex;
					trainIndex = new int[learnSize[f]];
					for (int i = 0; i < trainIndex.Length; i++)	trainIndex[i] = i ;
					// create a writer and open the file
					tr = rgFiles[f].OpenText();
					//ignore the first line
					input = tr.ReadLine();
					int TIndex = 0;
					for (int i = 0; i < lines[f] ; i++) { // phonema ID
						input = tr.ReadLine();
						string[] row = input.Split("	".ToCharArray());
						int leng = 0;
						for (int l = 0; l < row.Length; l++) {
							if (row[l]=="")
								continue;
							else
								leng++;
						}
						if (((TIndex<trainIndex.Length)&&(i==trainIndex[TIndex])) ||
						    (rgFiles[f].Name.Contains("Train")))
						{  // treain group
							TIndex++;
							SoundData2Learn[TrainI].Input = new double[rows[f],leng];
							for (int r = 0; r < rows[f]; r++) {
								for (int t = 0; t < row.Length ; t++) { // Search phnema
									if (row[t]=="") continue;
									SoundData2Learn[TrainI].Input[r,t] = factor * Convert.ToDouble(row[t]);
								}
								if ((r+1)<rows[f]){
									input = tr.ReadLine();
									row = input.Split("	".ToCharArray());
								}
							}
							SoundData2Learn[TrainI].Target = new double[]{Target};
							SoundData2Learn[TrainI].Tag = f;
							TrainI++;
						}else{  // Test group
							SoundData2Test[TestI].Input = new double[rows[f],leng];
							for (int r = 0; r < rows[f]; r++) {
								for (int t = 0; t < row.Length ; t++) { // Search phnema
									if (row[t]=="") continue;
									SoundData2Test[TestI].Input[r,t] = factor * Convert.ToDouble(row[t]);
								}
								if ((r+1)<rows[f]){
									input = tr.ReadLine();
									row = input.Split("	".ToCharArray());
								}
							}
							SoundData2Test[TestI].Target = new double[]{Target};
							SoundData2Test[TestI].Tag = f;
							TestI++;
						}
					}
					tr.Close();
				}
			}
			Console.WriteLine("finish loadin");
			
			return DirName;
			
		}//-----------------------------------------------------------------------------------------------
		
		
	}
}
