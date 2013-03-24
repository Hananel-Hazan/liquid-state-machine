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

using System.IO;
using System;
using Liquid;

namespace Liquid_Detector.Tests.DamageLSM
{

	class Reports
	{
		public static void Summing(string cmd, ref globalParam Param)
		{
			for (int arc = 0; arc < Param.networkParam.Liquid_Architectures.Length ; arc++) {
				
				string path = @cmd+@"/Arch="+Param.networkParam.Liquid_Architectures[arc].ToString()+@"/";
				
				for (int reports = 0; reports < Param.detector.ReadOut_Unit.Length ; reports++) {
					//int NoisePercent =  Param.NoisePercent-1;
//					string path=Directory.GetCurrentDirectory()+@"/";
					int[][] ResultGen,ResultDead,ResultRnd,ResultGeneral,ResultCombine;
					double Number_of_input = Param.test_Param.Numbers_of_Inputs;
					int Number_of_input_For_Activity_Check = Param.test_Param.Numbers_of_Inputs_For_Activity_Check;
					cumulateor[] NetworkPresentability,DelayResponseToInpot,DistanceBetweenActivityes;
					
					TextWriter fo ;
					DirectoryInfo[] dd = new DirectoryInfo(path).GetDirectories("*");
					
					NetworkPresentability = new cumulateor[Number_of_input_For_Activity_Check];
					DelayResponseToInpot = new cumulateor[Number_of_input_For_Activity_Check];
					DistanceBetweenActivityes = new cumulateor[Number_of_input_For_Activity_Check];
					for (int i = 0 ; i < Number_of_input_For_Activity_Check ;  i++) {
						NetworkPresentability[i] = new cumulateor();
						DelayResponseToInpot[i] = new cumulateor();
						DistanceBetweenActivityes[i] = new cumulateor();
					}
					
					ResultGen = new int[dd.Length][];
					ResultDead = new int[dd.Length][];
					ResultRnd = new int[dd.Length][];
					ResultGeneral = new int[dd.Length][];
					ResultCombine = new int[dd.Length][];
					for (int t=0 ; t < dd.Length ; t++){
						ResultGen[t] = new int[(int)Number_of_input];
						ResultDead[t] = new int[(int)Number_of_input];
						ResultRnd[t] = new int[(int)Number_of_input];
						ResultGeneral[t]  = new int[(int)Number_of_input];
						ResultCombine[t]  = new int[(int)Number_of_input];
						for ( int i=0 ;i < Number_of_input ;  i++) {
							ResultGen[t][i]=0;
							ResultDead[t][i]=0;
							ResultRnd[t][i]=0;
							ResultGeneral[t][i]=0;
							ResultCombine[t][i]=0;
						}
					}
					
					int DirNum = 0;
					int NoisePercent=0;
					foreach(DirectoryInfo dname in dd){
						
						string[] DirName = dname.Name.Split('.');
						if ( DirName.Length==1) {
							NoisePercent=0 ;
						}else if (NoisePercent<int.Parse(DirName[1])) NoisePercent=int.Parse(DirName[1]) ;
						
						fo = new StreamWriter(path+dname.Name+@"/SumActivityResultDetector"+reports.ToString()+".txt");
						
						Console.WriteLine(dname.Name);
						
						DirectoryInfo di = new DirectoryInfo(path+dname.Name+@"/");
						FileInfo[] rgFiles;
						rgFiles = di.GetFiles("Result_on_Detector-"+reports.ToString()+"-*.txt");
						
						foreach(FileInfo fi in rgFiles)
						{
							if (fi.Name=="SumActivityResultDetector*.txt") continue;
							TextReader tr = new StreamReader(fi.FullName);
							string input = null;
							fo.Write("file:{0,18:c}",fi.Name);
							int LineNumber=0,SeccRecog = 0;;
							int[] NetActivity = new int[0],InputActivity = new int[0], DistanceActivity = new int[2];
							while ((input = tr.ReadLine()) != null)
							{
								string[] words = input.Split(':');
								if (LineNumber==0) { fo.Write("	Neuron#:{0,15:c}",words[1]);}
								else if (LineNumber==1){ fo.Write("	Nor:{0,16:c}",words[1]); words = words[1].Split('/'); SeccRecog=int.Parse(words[0]);}
								else if (LineNumber==2){ fo.Write("	Gen:{0,16:c}",words[1]);
//								if ( SeccRecog==Number_of_input) {
									words = words[1].Split('/');
									int temp = int.Parse(words[0])-1;
									ResultGen[DirNum][temp]++;
//								}
								}
								else if (LineNumber==3){ fo.Write("	Dam:{0,16:c}",words[1]);
//								if ( SeccRecog==Number_of_input) {
									words = words[1].Split('/');
									int temp = int.Parse(words[0])-1;
									ResultDead[DirNum][temp]++;
//								}
								}
								else if (LineNumber==4){ fo.Write("	Rnd:{0,16:c}",words[1]);
//								if ( SeccRecog==Number_of_input) {
									words = words[1].Split('/');
									int temp = int.Parse(words[0])-1;
									ResultRnd[DirNum][temp]++;
//								}
								}
								else if (LineNumber==5){ fo.Write("	General:{0,16:c}",words[1]);
//								if ( SeccRecog==Number_of_input) {
									words = words[1].Split('/');
									int temp = int.Parse(words[0])-1;
									ResultGeneral[DirNum][temp]++;
//								}
								}
								else if (LineNumber==6){ fo.Write("	Combine Test:{0,16:c}",words[1]);
//								if ( SeccRecog==Number_of_input) {
									words = words[1].Split('/');
									int temp = int.Parse(words[0])-1;
									ResultCombine[DirNum][temp]++;
//								}
								}
								else if (LineNumber==9){ NetActivity = Param.ConvertStringToInt(words[0].Split('	'));}
								else if (LineNumber==10){ InputActivity = Param.ConvertStringToInt(words[0].Split('	'));
									int GraceGup=5;
									int silenceGup=GraceGup+1;
									int ActivityCycle=0,InputDelay=0,EndActivity=0;
									for ( int i = 0 ; i < InputActivity.Length ; i++ ) {
										
										if ((NetActivity[i]==0)&&(silenceGup<GraceGup)){silenceGup++;}
										if ((NetActivity[i]==0)&&(silenceGup==GraceGup)){silenceGup=GraceGup+1;}
										else if ( NetActivity[i]!=0) {silenceGup=0;}
										
										// Delayed Activity in the Network = how much time take the network to react to input?
										if ((InputActivity[i]!=0)&&(NetActivity[i]==0)) InputDelay++;
										if ((InputActivity[i]==0)&&(InputDelay>0)){DelayResponseToInpot[0].Add(InputDelay); InputDelay=0;}
										
										//Network Presentability = how much cycles have activity in the network without Input?
										if ( (NetActivity[i]!=0) && (InputActivity[i]==0) ) ActivityCycle++;
										if ( (NetActivity[i]==0) && (ActivityCycle>0) && (silenceGup>GraceGup) ) {
											if (ActivityCycle>GraceGup) NetworkPresentability[0].Add(ActivityCycle-GraceGup);
											else  NetworkPresentability[0].Add(ActivityCycle);
											ActivityCycle=0;
										}
										
										//Distance Between Inputs --> first line is the last cycle that the network had activity after input
										if ( NetActivity[i]!=0 ) EndActivity=i;
									}
									DistanceBetweenActivityes[0].Add(EndActivity);
									DistanceActivity[0] = EndActivity;
								}
								else if (LineNumber==12){ NetActivity = Param.ConvertStringToInt(words[0].Split('	'));}
								else if (LineNumber==13){ InputActivity = Param.ConvertStringToInt(words[0].Split('	'));
									int GraceGup=5;
									int silenceGup=GraceGup+1;
									int ActivityCycle=0,InputDelay=0,EndActivity=0;
									for ( int i = 0 ; i < InputActivity.Length ; i++ ) {
										
										if ((NetActivity[i]==0)&&(silenceGup<GraceGup)){silenceGup++;}
										if ((NetActivity[i]==0)&&(silenceGup==GraceGup)){silenceGup=GraceGup+1;}
										else if ( NetActivity[i]!=0) { silenceGup=0;}
										
										// Delayed Activity in the Network = how much time take the network to react to input?
										if ((NetActivity[i]==0)&&(InputActivity[i]!=0)&&(InputDelay==0)) DistanceActivity[1] = i;
										if ((NetActivity[i]==0)&&(InputActivity[i]!=0)) InputDelay++;
										if ((InputActivity[i]==0)&&(InputDelay>0)){
											DelayResponseToInpot[1].Add(InputDelay);
											DistanceActivity[1]+=InputDelay;  // save the start of the Network Avtivation
											InputDelay=0;
										}
										
										//Network Presentability = how much cycles have activity in the network without Input?
										if ( (NetActivity[i]!=0) && (InputActivity[i]==0) ) ActivityCycle++;
										if ( (NetActivity[i]==0) && (ActivityCycle>0) && (silenceGup>GraceGup) ) {
											if (ActivityCycle>GraceGup) NetworkPresentability[1].Add(ActivityCycle-GraceGup);
											else  NetworkPresentability[1].Add(ActivityCycle);
											ActivityCycle=0;
										}
										
										//Distance Between Inputs --> Secend line is the last cycle that the network had activity after input
										if ( NetActivity[i]!=0 ) EndActivity=i;
									}
									DistanceBetweenActivityes[1].Add(EndActivity);
								}
								else if (LineNumber==15){ NetActivity = Param.ConvertStringToInt(words[0].Split('	'));}
								else if (LineNumber==16){ InputActivity = Param.ConvertStringToInt(words[0].Split('	'));
									int GraceGup=5;
									int silenceGup=GraceGup+1;
									int ActivityCycle=0,InputDelay=0,EndActivity=0;
									for ( int i = 0 ; i < InputActivity.Length ; i++ ) {
										
										if ((NetActivity[i]==0)&&(silenceGup<GraceGup)){silenceGup++;}
										if ((NetActivity[i]==0)&&(silenceGup==GraceGup)){silenceGup=GraceGup+1;}
										else if ( NetActivity[i]!=0) { silenceGup=0;}
										
										// Delayed Activity in the Network = how much time take the network to react to input?
										if ((InputActivity[i]!=0)&&(NetActivity[i]==0)) InputDelay++;
										if ((InputActivity[i]==0)&&(InputDelay>0)){DelayResponseToInpot[2].Add(InputDelay); InputDelay=0;}
										
										//Network Presentability = how much cycles have activity in the network without Input?
										if ( (NetActivity[i]!=0) && (InputActivity[i]==0) ) ActivityCycle++;
										if ( (NetActivity[i]==0) && (ActivityCycle>0) && (silenceGup>GraceGup) ) {
											if (ActivityCycle>GraceGup) NetworkPresentability[2].Add(ActivityCycle-GraceGup);
											else  NetworkPresentability[2].Add(ActivityCycle);
											ActivityCycle=0;
										}
										
										//Distance Between Inputs --> third line is the diffrence between the first last activity to the sec first last activity to the input
										if ( NetActivity[i]!=0 ) EndActivity=i;
									}
									if ( DistanceActivity[1]>DistanceActivity[0]) DistanceBetweenActivityes[2].Add(DistanceActivity[1] - DistanceActivity[0]);
									else DistanceBetweenActivityes[2].Add(0);
									
								}
								LineNumber++;
							}
							fo.WriteLine("");
							tr.Close();
						}
						fo.Close();
						DirNum++;
					}
					
					fo = new StreamWriter(cmd+@"/Sum_Arc-"+Param.networkParam.Liquid_Architectures[arc].ToString()
						                      +"Detector-"+reports.ToString()+".txt");
					
					fo.WriteLine("Noise Generator");
					fo.Write("	");
					for ( int i=0 ; i < (dd.Length) ; i++ ) {
//						fo.Write(((double)(i)/100).ToString() + "	");
						fo.Write( dd[i].Name + "	");
					}
					fo.WriteLine("");
					double[][] sum = new double[3][];
					sum[0] = new double[dd.Length];
					sum[1] = new double[dd.Length];
					sum[2] = new double[dd.Length];
					for (int i = 0 ; i <sum[0].Length ; i++ ) { sum[0][i]=0; sum[1][i]=0; sum[2][i]=0;}
					double fifty=-1;
					for ( int t = 0 ; t < Number_of_input ; t++ ) {
						fo.Write((1+t).ToString() + " :	");
						fifty += 1/(Number_of_input/2);
						double hundred=((1+t)/Number_of_input);
						for ( int i=0 ; i < (dd.Length) ; i++ ) {
							fo.Write(ResultGen[i][t].ToString()+"	");
							sum[0][i]+=ResultGen[i][t];
							sum[1][i]+=(ResultGen[i][t]*hundred);
							sum[2][i]+=(ResultGen[i][t]*fifty);
						}
						fo.WriteLine("");
					}
					fo.WriteLine(""); fo.Write("sum 	");
					for (int i = 0 ; i <sum[0].Length ; i++ ) { fo.Write(sum[0][i].ToString()+"	");}
					fo.WriteLine(""); fo.Write("100% 	");
					for (int i = 0 ; i <sum[1].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[1][i]/sum[0][i]));}
					fo.WriteLine(""); fo.Write("50%	");
					for (int i = 0 ; i <sum[2].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[2][i]/sum[0][i]));}
					fo.WriteLine(""); fo.WriteLine("");
					//--------------------------------------------
					
					fo.WriteLine(""); fo.WriteLine("Dead Neurons"); fo.Write("	");
					for ( int i=0 ; i < (dd.Length) ; i++ ) {
//						fo.Write(((double)(i)/100).ToString() + "	");
						fo.Write( dd[i].Name + "	");
					}
					fo.WriteLine("");
					sum = new double[3][]; sum[0] = new double[dd.Length];
					sum[1] = new double[dd.Length];sum[2] = new double[dd.Length];
					for (int i = 0 ; i <sum[0].Length ; i++ ) { sum[0][i]=0; sum[1][i]=0; sum[2][i]=0;}
					fifty=-1;
					for ( int t = 0 ; t < Number_of_input ; t++ ) {
						fo.Write((1+t).ToString() + " :	");
						fifty += 1/(Number_of_input/2);
						double hundred=((1+t)/Number_of_input);
						for ( int i=0 ; i < (dd.Length) ; i++ ) {
							fo.Write(ResultDead[i][t].ToString()+"	");
							sum[0][i]+=ResultDead[i][t];
							sum[1][i]+=(ResultDead[i][t]*hundred);
							sum[2][i]+=(ResultDead[i][t]*fifty);
						}
						fo.WriteLine("");
					}
					fo.WriteLine(""); fo.Write("sum 	");
					for (int i = 0 ; i <sum[0].Length ; i++ ) { fo.Write(sum[0][i].ToString()+"	");}
					fo.WriteLine(""); fo.Write("100% 	");
					for (int i = 0 ; i <sum[1].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[1][i]/sum[0][i]));}
					fo.WriteLine("");fo.Write("50%	");
					for (int i = 0 ; i <sum[2].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[2][i]/sum[0][i]));}
					fo.WriteLine("");fo.WriteLine("");
					//--------------------------------------------
					
					
					fo.WriteLine(""); fo.WriteLine("Combine Test"); fo.Write("	");
					for ( int i=0 ; i < (dd.Length) ; i++ ) {
//						fo.Write(((double)(i)/100).ToString() + "	");
						fo.Write( dd[i].Name + "	");

					}
					fo.WriteLine("");
					sum = new double[3][]; sum[0] = new double[dd.Length];
					sum[1] = new double[dd.Length];sum[2] = new double[dd.Length];
					for (int i = 0 ; i <sum[0].Length ; i++ ) { sum[0][i]=0; sum[1][i]=0; sum[2][i]=0;}
					fifty=-1;
					for ( int t = 0 ; t < Number_of_input ; t++ ) {
						fo.Write((1+t).ToString() + " :	");
						fifty += 1/(Number_of_input/2);
						double hundred=((1+t)/Number_of_input);
						for ( int i=0 ; i < (dd.Length) ; i++ ) {
							fo.Write(ResultCombine[i][t].ToString()+"	");
							sum[0][i]+=ResultCombine[i][t];
							sum[1][i]+=(ResultCombine[i][t]*hundred);
							sum[2][i]+=(ResultCombine[i][t]*fifty);
						}
						fo.WriteLine("");
					}
					fo.WriteLine(""); fo.Write("sum 	");
					for (int i = 0 ; i <sum[0].Length ; i++ ) { fo.Write(sum[0][i].ToString()+"	");}
					fo.WriteLine(""); fo.Write("100% 	");
					for (int i = 0 ; i <sum[1].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[1][i]/sum[0][i]));}
					fo.WriteLine("");fo.Write("50%	");
					for (int i = 0 ; i <sum[2].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[2][i]/sum[0][i]));}
					fo.WriteLine("");fo.WriteLine("");
					//--------------------------------------------
			

					fo.WriteLine(""); fo.WriteLine("Generalization Test"); fo.Write("	");
					for ( int i=0 ; i < (dd.Length) ; i++ ) {
//						fo.Write(((double)(i)/100).ToString() + "	");
						fo.Write( dd[i].Name + "	");

					}
					fo.WriteLine("");
					sum = new double[3][]; sum[0] = new double[dd.Length];
					sum[1] = new double[dd.Length];sum[2] = new double[dd.Length];
					for (int i = 0 ; i <sum[0].Length ; i++ ) { sum[0][i]=0; sum[1][i]=0; sum[2][i]=0;}
					fifty=-1;
					for ( int t = 0 ; t < Number_of_input ; t++ ) {
						fo.Write((1+t).ToString() + " :	");
						fifty += 1/(Number_of_input/2);
						double hundred=((1+t)/Number_of_input);
						for ( int i=0 ; i < (dd.Length) ; i++ ) {
							fo.Write(ResultGeneral[i][t].ToString()+"	");
							sum[0][i]+=ResultGeneral[i][t];
							sum[1][i]+=(ResultGeneral[i][t]*hundred);
							sum[2][i]+=(ResultGeneral[i][t]*fifty);
						}
						fo.WriteLine("");
					}
					fo.WriteLine(""); fo.Write("sum 	");
					for (int i = 0 ; i <sum[0].Length ; i++ ) { fo.Write(sum[0][i].ToString()+"	");}
					fo.WriteLine(""); fo.Write("100% 	");
					for (int i = 0 ; i <sum[1].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[1][i]/sum[0][i]));}
					fo.WriteLine("");fo.Write("50%	");
					for (int i = 0 ; i <sum[2].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[2][i]/sum[0][i]));}
					fo.WriteLine("");fo.WriteLine("");
					//--------------------------------------------
					fo.WriteLine("");fo.WriteLine("Random");fo.Write("	");
					for ( int i=0 ; i < (dd.Length) ; i++ ) {
//						fo.Write(((double)(i)/100).ToString() + "	");
						fo.Write( dd[i].Name + "	");
					}
					fo.WriteLine("");
					sum = new double[3][]; sum[0] = new double[dd.Length];
					sum[1] = new double[dd.Length];sum[2] = new double[dd.Length];
					for (int i = 0 ; i <sum[0].Length ; i++ ) { sum[0][i]=0; sum[1][i]=0; sum[2][i]=0;}
					fifty=-1;
					for ( int t = 0 ; t < Number_of_input ; t++ ) {
						fo.Write((1+t).ToString() + " :	");
						fifty += 1/(Number_of_input/2);
						double hundred=((1+t)/Number_of_input);
						for ( int i=0 ; i < (dd.Length) ; i++ ) {
							fo.Write(ResultRnd[i][t].ToString()+"	");
							sum[0][i]+=ResultRnd[i][t];
							sum[1][i]+=(ResultRnd[i][t]*hundred);
							sum[2][i]+=(ResultRnd[i][t]*fifty);
						}
						fo.WriteLine("");
					}
					fo.WriteLine(""); fo.Write("sum 	");
					for (int i = 0 ; i <sum[0].Length ; i++ ) { fo.Write(sum[0][i].ToString()+"	");}
					fo.WriteLine(""); fo.Write("100% 	");
					for (int i = 0 ; i <sum[1].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[1][i]/sum[0][i]));}
					fo.WriteLine(""); fo.Write("50%	");
					for (int i = 0 ; i <sum[2].Length ; i++ ) { fo.Write("{0:0.00}	",(sum[2][i]/sum[0][i]));}
					fo.WriteLine(""); fo.WriteLine("");
					//----------------------------------------------------------------------------------------------------------

					// Activation Report
					fo.WriteLine("");
					
					fo.WriteLine("");
					
					for ( int line = 0 ; line < Number_of_input_For_Activity_Check ; line++) {
						//NetworkPresentability,DistanceBetweenActivityes;
						fo.WriteLine("Report for Test Vector {0}:",1+line);
						fo.WriteLine("	Delayed response to input is in average {0:0.00}  ({1:0.00})",
						             DelayResponseToInpot[line].Return_Average(),DelayResponseToInpot[line].Return_Standard_Deviation());
						
						fo.WriteLine("	The Network was active after the input {0:0.00} ({1:0.00}) iterations. min=	{2}	max=	{3}",
						             NetworkPresentability[line].Return_Average(),NetworkPresentability[line].Return_Standard_Deviation(),
						             NetworkPresentability[line].Return_min(),NetworkPresentability[line].Return_max());
						
						if ( line<Number_of_input_For_Activity_Check-1) {
							fo.WriteLine("	The iteration that activity stop {0:0.00} ({1:0.00}) . min=	{2}	max=	{3}",
							             DistanceBetweenActivityes[line].Return_Average(),DistanceBetweenActivityes[line].Return_Standard_Deviation(),
							             DistanceBetweenActivityes[line].Return_min(),DistanceBetweenActivityes[line].Return_max());
						}else{
							fo.WriteLine("	The gap between two inputs was in average	{0:0.00} ({1:0.00}) . min=	{2}	max=	{3}",
							             DistanceBetweenActivityes[line].Return_Average(),DistanceBetweenActivityes[line].Return_Standard_Deviation(),
							             DistanceBetweenActivityes[line].Return_min(),DistanceBetweenActivityes[line].Return_max());
						}
					}
					fo.Close();
				}
			}
		}
		
		
		public static void ConectivityTest(string args){
			
			globalParam Param = new globalParam();
			Param = Param.load(args);
//			int iterationNumber=Param.iteration;
//			//  Testing connectivity
//			TextWriter tw1 = new StreamWriter("ConnectionDistribution - Sort - Input.txt");
//			TextWriter tw2 = new StreamWriter("ConnectionDistribution - Sort - Output.txt");
//			TextWriter tw3 = new StreamWriter("ConnectionDistribution - Input.txt");
//			TextWriter tw4 = new StreamWriter("ConnectionDistribution - Output.txt");
//			for (int i = 0; i<20 ; i++){
//				// Init the LSM Network
//				Liquid.LSM Net = new LSM(ref Param);
//				// finish Init and creating Network
//
//				if ( i==0 ) Net.DOTFile();
//
//				string temp="";
//
//				double[] tempDistro = new double[Net.ConnectionDistribution_Input.Length];
//				Net.ConnectionDistribution_Input.CopyTo(tempDistro,0);
//				Array.Sort(tempDistro);
//
//				for (int j=0; j<tempDistro.Length ;j++ ) {
//					temp=temp+tempDistro[j].ToString()+"\t";
//				}
//				tw1.WriteLine(temp);
//				temp="";
//
//				tempDistro = new double[Net.ConnectionDistribution_Output.Length];
//				Net.ConnectionDistribution_Output.CopyTo(tempDistro,0);
//				Array.Sort(tempDistro);
//				for (int j=0; j<tempDistro.Length ;j++ ) {
//					temp=temp+tempDistro[j].ToString()+"\t";
//				}
//				tw2.WriteLine(temp);
//				Console.WriteLine(i);
//				//--------------------------------------
//				temp="";
//				for (int j=0; j<Net.ConnectionDistribution_Input.Length ;j++ ) {
//					temp=temp+Net.ConnectionDistribution_Input[j].ToString()+"\t";
//				}
//				tw3.WriteLine(temp);
//				temp="";
//				for (int j=0; j<Net.ConnectionDistribution_Output.Length ;j++ ) {
//					temp=temp+Net.ConnectionDistribution_Output[j].ToString()+"\t";
//				}
//				tw4.WriteLine(temp);
//				Console.WriteLine(i);
//				//--------------------------------------
//				Net = null;
//			}
//			tw1.Close();
//			tw2.Close();
//			tw3.Close();
//			tw4.Close();
//
			// End of testing connectivity
			
			File.Delete(args);
		}
		
		
	}
}