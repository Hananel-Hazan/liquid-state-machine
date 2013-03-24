
using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Liquid;


namespace Liquid_Detector.Tests.MRI
{

	class MRIProgram
	{
		
		public static void MainProcess(string args)
		{
			globalParam Param = new globalParam();
			Param = Param.load(args);
			
			//Input MRI data
			Inputs.MRI_Input input = new Inputs.MRI_Input();
			input.estimateInput(ref Param);
			Console.WriteLine("Write the Voxels to file....");
			input.ReadAllData(ref Param);
			Console.WriteLine("Normelaize the Voxels...");
			input.NormelizeInput(ref Param);
			Console.WriteLine("Finish Creating Data");
			
			Param.input2liquid = new Liquid_Detector.Tests.MRI.Parameters.Input4MRI();
			Param.neuronParam =  new Liquid_Detector.Tests.MRI.Parameters.Neuron4MRI();
			Param.networkParam = new Liquid_Detector.Tests.MRI.Parameters.RecurrentNetwork4MRI( Param.neuronParam.Neuron_Model, ref Param );
			Param.detector = new Liquid_Detector.Tests.MRI.Parameters.ReadOut4MRI();


			
			for (int iterationNumber=Param.iteration ; iterationNumber<Param.test.NumOfVoxels ; iterationNumber++) {
				// Return the relevent Voxel from the data
				Param.iteration = iterationNumber;
				Param.LearnData = null;
				Param.TestData = null;
				input.ReturnEpicData(ref Param);
				input.ReturnEpicData(ref Param);
				input.CollectVoxlsFromSomeSession(ref Param.LearnData);
				input.CollectVoxlsFromSomeSession(ref Param.TestData);
				
				Console.WriteLine("!Start the program! {0}",iterationNumber);
				
				System.Console.WriteLine("Finish loading.");
				// Finish Create Input

				// Init the LSM Network
				int MaxInputDuration = Param.LearnData.Length;
				for (int i = 0; i < Param.LearnData.Length ; i++){
					if (MaxInputDuration < Param.LearnData[i].Input.GetLength(1))
						MaxInputDuration = Param.LearnData[i].Input.GetLength(1);
				}
				MaxInputDuration = Param.input2liquid.ComputeInputTime(MaxInputDuration);
				
				Liquid.LSM Net = new Liquid.LSM(ref Param,MaxInputDuration);
				// finish Init and creating Network

				//open output files..
				string tempFileName = iterationNumber.ToString();
				string dirResuls = Param.test.OutputDir.ToString()+Param.CurrentGroup.ToString()+"//";
				if (Directory.Exists (dirResuls)==false){ Directory.CreateDirectory(dirResuls); }
				TextWriter tw = new StreamWriter(dirResuls+@"//Voxel#-"+tempFileName+".txt");

				// Run the LSM on Input
//				double LastReturnError = Net.Learn(ref Param , ref Param.LearnData,0);
				
//				double[,] DetectorOutput;
//				Net.Test(ref Param , ref Param.TestData, out DetectorOutput,0 );
				
//				tw.WriteLine("Detector Last Treaining error = {0}",LastReturnError);
//				for(int i=0 ; i<DetectorOutput.Length ; i++){
//					tw.WriteLine("Vector#\t{0}, Target\t= {1}, Detector degree of correction\t = {2}",Param.CurrentGroup,Param.TestData[i].Target,DetectorOutput[i,0]);
//				}
				
				// close the stream
				tw.Flush();
				tw.Close();


				Console.WriteLine("Finish Voxle {0}",iterationNumber);
				//-----------------------------------------------------
				
			}// all thr Voxel finish to run
			Reports summing = new Reports();
			summing.SmallSummery(ref Param);
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();

			File.Delete(args);
			return;
			
			
			
		}
		
		public static void return_Targets(globalParam.Data[] input,out double[] TargetVoxels){
			int size = input.Length;
			TargetVoxels = new double[size];
			for( int i = 0 ; i < size ; i++){
				TargetVoxels[i] = input[i].Target[0];
			}
			
		}

		[STAThread]
		public static void MainMRI(string[] args)
		{
			if (args.Length>0){
				MainProcess(args[0]);
			}
			else{

				globalParam Param = new globalParam();
				Param.initialization();
				Param.test = new Liquid_Detector.Tests.MRI.Parameters.TestParameters4MRI();
				
				Process[] p = new Process[Param.ThreadNum];
				string[] pName = new string[Param.ThreadNum];
				
				Reports summing = new Reports();
				
				
				//Input MRI data
				Inputs.MRI_Input input = new Inputs.MRI_Input();
				input.estimateInput(ref Param);
				
				Utils_Functions.Combinator combina = new Utils_Functions.Combinator(Param.NumberOfSession);
				Param.TestGrpup = combina.Select(1);
				Param.LearnGroup = combina.fill(Param.TestGrpup);
				
				// finish MRI data
				int[] NumberOfSession = new int[Param.NumberOfSession.Length];
				Param.NumberOfSession.CopyTo(NumberOfSession,0);
				int counter=0,flag=0;
				for (Param.CurrentGroup=0 ; Param.CurrentGroup< Param.NumberOfSession.Length ; Param.CurrentGroup++){
					while (flag==0) {
						if (!File.Exists(pName[counter])){
							Param.save(Param.CurrentGroup.ToString());
							pName[counter]="obj"+Param.CurrentGroup.ToString()+".dat";
							p[counter]= new Process();
							p[counter].StartInfo.FileName="mono Liquid\\ Detector.exe";
							p[counter].StartInfo.Arguments=pName[counter];
							System.Console.WriteLine("+++{0}+++",Param.CurrentGroup);
							if (Param.Linux_OR_Windows==2){
								MainProcess("obj"+Param.CurrentGroup.ToString()+".dat"); // for Dubuging
							}else{
								p[counter].Start();  // For Runing
							}
							flag=1;
							p[counter].Dispose();
						}
						if ((counter+1)==Param.ThreadNum) {Thread.Sleep(1000); counter=0;}
						else counter++;
					}
					flag=0;
				}//finish all the combination of testing/learning
				counter=0;
				while (flag<pName.Length) {
					if (!File.Exists(pName[counter])) flag++;
					else flag=0;
					if ((counter+1)==Param.ThreadNum) {Thread.Sleep(1000); counter=0;}
					else counter++;
				}
				
				summing.SummerizeAll(ref Param);
//				Console.ReadKey();
			}
		}
	}
}