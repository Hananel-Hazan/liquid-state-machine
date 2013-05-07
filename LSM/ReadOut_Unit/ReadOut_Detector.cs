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
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

using NN_Pr;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Simple;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Engine.Network.Activation;
using SVM;

using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.SVM.Training;
using Encog.ML.SVM;

using Liquid;


//


[Serializable()]
public class ReadOut_Detector
{
	public int model,NetSize,Window_Size,LiquidSize,DetectorInputSize,RealInputSize,OutputSize,windowSize,HiddenSize;
	private int ReadOut_unit_Model_AP_or_Sequ;
	public int Detector_Run_Time,How_Many_Windows,MaxNumberOfWindows,ReadOut4EveryWindow;
	public double[] Window_Accuracy;
	private int[] Window_Start_Index;

	//
	Perceptron[] Per;
	List<Pattern>[] patterns;
	
	BasicNetwork[] network;
	IMLDataSet[] GtrainingSet;
	
	Model[] SVM;
	Problem[] dataToTrain;
	
	Tempotron[] TempotronDetector;
	//
	
	public ReadOut_Detector(int LiquidSize ,int hiddenSize, int outputSize,int place,ref globalParam Param)
	{
		//---------------------------------------------------------------------------------
		//   Init Backpropagation Network
		//---------------------------------------------------------------------------------
		Console.WriteLine("Inisilaize Readout Neuronal Network ");
		this.model = Param.detector.ReadOut_Unit[place];
		this.ReadOut_unit_Model_AP_or_Sequ  = Param.detector.ReadOut_unit_Model_AP_or_Sequ[place];
		this.ReadOut4EveryWindow = Param.detector.ReadOut4EveryWindow[place];
		
		this.Window_Accuracy = new double[0];
		this.Window_Start_Index = new int[0];
		MaxNumberOfWindows = 0;
		
		this.OutputSize = outputSize;
		this.LiquidSize = LiquidSize;
		this.Window_Size = 0;
		this.DetectorInputSize = 0;
		this.RealInputSize = 0;
		this.HiddenSize = hiddenSize;
		if (hiddenSize==0) hiddenSize=1;
	}//---------------------------------------------------------------------------
	
	public void BuildDetector(int HiddenLayer_for_OneClass, ref globalParam Param)
	{
		if (Window_Size==0) return;
		
		int numOfReadOutUnits = 1;
		
		if (ReadOut4EveryWindow>1) numOfReadOutUnits = MaxNumberOfWindows;
		
		if (model==1) {
			this.Per = new Perceptron[numOfReadOutUnits];
		}else if (((this.model>1)&&(this.model<7))||(this.model==9)){
			this.network = new BasicNetwork[numOfReadOutUnits];
			for (int i = 0 ; i < numOfReadOutUnits; i++)
				this.network [i] = new BasicNetwork();
		}else if (model==7){
			this.SVM = new Model[numOfReadOutUnits];
		}else if (model==8){
			this.TempotronDetector = new Tempotron[numOfReadOutUnits];
		}else if (model==10){
			this.network = new BasicNetwork[this.GtrainingSet.Length];
			for (int i = 0 ; i < this.network.Length ; i++)
				this.network [i] = new BasicNetwork();
		}
		//--------------
		numOfReadOutUnits = 1 ;
		if (ReadOut4EveryWindow>1) numOfReadOutUnits = this.MaxNumberOfWindows;
		if (model==10) numOfReadOutUnits = this.network.Length;
		
		if (this.Window_Accuracy.Length < numOfReadOutUnits)
			Array.Resize(ref this.Window_Accuracy,numOfReadOutUnits);
		
		for (int i = 0 ; i < numOfReadOutUnits  ; i++){
			if (model==1) {
				this.Per[i] = new Perceptron(this.DetectorInputSize,ref Param);
			}else if (model==2) {
				this.network[i] = new BasicNetwork();
				this.network[i].AddLayer(new BasicLayer(new ActivationSigmoid(), true, this.DetectorInputSize));
				this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), true, this.OutputSize));
				this.network[i].Structure.FinalizeStructure();
				this.network[i].Structure.FinalizeLimit();
				this.network[i].Reset();
			}else if ((model>2)&&(model<7)){
				this.network[i] = new BasicNetwork();
				if (ReadOut_unit_Model_AP_or_Sequ==1){
					this.network[i].AddLayer(new BasicLayer(new ActivationTANH() , true, this.DetectorInputSize));
					this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), true, this.HiddenSize));
					this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), false, this.OutputSize));
				}else if (ReadOut_unit_Model_AP_or_Sequ==2){
					this.network[i].AddLayer(new BasicLayer(new ActivationLOG(), true, this.DetectorInputSize));;
					this.network[i].AddLayer(new BasicLayer(new ActivationLOG(), true, this.HiddenSize));
					this.network[i].AddLayer(new BasicLayer(new ActivationBiPolar(), true, this.OutputSize));
				}
				this.network[i].Structure.FinalizeStructure();
				this.network[i].Structure.FinalizeLimit();
				this.network[i].Reset(Convert.ToInt32(Param.rnd.NextDouble()*100));
			}else if (model==7) {
				this.SVM[i] = new Model();
			}else if (model==8) {
				TempotronDetector[i] = new Tempotron(this.RealInputSize);
			}else if (model==9) {

			}else if (model==10) {
				Console.WriteLine(" Hidden Layer = "+HiddenLayer_for_OneClass.ToString());
				this.network[i] = new BasicNetwork();
				this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), true, this.DetectorInputSize));
				this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), true, HiddenLayer_for_OneClass));//Convert.ToInt32(Math.Round(this.DetectorInputSize * 0.001))));
				this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), false, this.DetectorInputSize));
				this.network[i].Structure.FinalizeStructure();
				this.network[i].Structure.FinalizeLimit();
				this.network[i].Reset(Convert.ToInt32(Param.rnd.NextDouble()*100));
			}
			
		}
		//--------------
		this.NetSize=DetectorInputSize+HiddenSize+OutputSize;
		//--------------
	}//---------------------------------------------------------------------------

	public void initialization(ref globalParam Param,int NumOfGroups)
	{
		int WinORDetec = 1;
		if (this.ReadOut4EveryWindow==2) WinORDetec = MaxNumberOfWindows;
		
		//--------------
		if (model==1) {
			patterns = new List<Pattern>[WinORDetec];
		}else if (((this.model>1)&&(this.model<7))||(this.model==9)){
			if (this.GtrainingSet==null){
				this.GtrainingSet = new IMLDataSet[WinORDetec];
				for (int i = 0 ; i < GtrainingSet.Length; i++)
					GtrainingSet[i] = new BasicMLDataSet();
			}else{
				if ((this.GtrainingSet!=null)&&(this.GtrainingSet.Length<WinORDetec)){
					IMLDataSet[] temp = new IMLDataSet[WinORDetec];
					for (int i = 0 ; i < GtrainingSet.Length; i++)
						temp[i]=GtrainingSet[i];
					for (int i = GtrainingSet.Length; i < WinORDetec; i++)
						temp[i] = new BasicMLDataSet();
					this.GtrainingSet = temp;
				}
				
			}
		}else if (model==7){
			if (dataToTrain==null){
				dataToTrain = new Problem[WinORDetec];
				for (int i = 0; i < WinORDetec; i++) {
					dataToTrain[i] =  new Problem();
				}
			}else if (dataToTrain.Length<WinORDetec){
				Problem[] temp = new Problem[WinORDetec];
				for (int i = 0 ; i < dataToTrain.Length; i++)
					temp[i]=dataToTrain[i];
				for (int i = dataToTrain.Length; i < WinORDetec; i++)
					temp[i] = new Problem();
				this.dataToTrain = temp;
			}
			
		}else if (model==8){
		}else if (model==10){
			if (this.GtrainingSet==null){
				this.GtrainingSet = new IMLDataSet[NumOfGroups];
				for (int i = 0 ; i < NumOfGroups ; i++)
					GtrainingSet[i] = new BasicMLDataSet();
			}else{
				if (NumOfGroups>this.GtrainingSet.Length){
					IMLDataSet[] temp = new IMLDataSet[NumOfGroups];
					for (int i = 0 ; i < GtrainingSet.Length; i++)
						temp[i]=GtrainingSet[i];
					for (int i = GtrainingSet.Length; i < NumOfGroups ; i++)
						temp[i] = new BasicMLDataSet();
					this.GtrainingSet = temp;
				}
			}
		}
	}//--------------------------------------------------------------------


	public int ConfigWindowParameters(ref Liquid.LSM LSM, int Input_Length, ref globalParam Param){
		
		int Real_Inpur_Length = Param.input2liquid.ComputeInputTime(Input_Length);
		int HowManyWindows = 0;
		if ( Window_Size > Real_Inpur_Length)
			Window_Size = Real_Inpur_Length;
		
		
		int tempDetectorInputSize = Window_Size * LiquidSize;
		if (tempDetectorInputSize > DetectorInputSize)
			DetectorInputSize = tempDetectorInputSize;
		
		int Liquid_Run_Time = LSM.TimeInitialization(Input_Length,out Window_Size,ref Param);
		int windowSize = Window_Size +
			Param.detector.ReadoutU_Disctance_Between_Windows +
			Param.detector.ReadoutU_Window_Shifting;
		
		
		
		if (Param.detector.Readout_Activity_during_Input_time==0){
			int temp = Param.detector.LSM_Adjustment_Time_at_Beginning +
				Param.detector.LSM_Adjustment_Time_at_Ending +
				Real_Inpur_Length;
			HowManyWindows +=  (Liquid_Run_Time - temp) / windowSize;
		}else{
			// readout is on during input time
			if (Param.detector.Approximate_OR_Classification==1){  //Approximate
				HowManyWindows += Input_Length;
			}else if (Param.detector.Approximate_OR_Classification==2){ // Classification
				if (Param.detector.ReadoutU_How_Many_Windows==0)
					HowManyWindows +=  Real_Inpur_Length / windowSize;
				else{
					int temp =  Real_Inpur_Length / windowSize;
					if (temp < Param.detector.ReadoutU_How_Many_Windows)
						HowManyWindows += temp;
					else{
						HowManyWindows = Param.detector.ReadoutU_How_Many_Windows;
					}
				}
			}
		}
//		//----------------------------------------
		
		if (HowManyWindows> MaxNumberOfWindows)
			MaxNumberOfWindows = HowManyWindows;
		
		return HowManyWindows;
	}


	public void DidWindowChange(ref Liquid.LSM LSM,int Input_Length,int NumOfGroups, ref globalParam Param){
		
		int Liquid_Run_Time = LSM.TimeInitialization(Input_Length,out Window_Size,ref Param);
		
		int HowManyWindows = Param.detector.ReadoutU_How_Many_Windows;
		if (Param.detector.ReadoutU_How_Many_Windows==0)
			HowManyWindows = ConfigWindowParameters(ref LSM,Input_Length,ref Param);
		
		if (HowManyWindows> this.How_Many_Windows){
//			this.How_Many_Windows = HowManyWindows;
			Array.Resize(ref this.Window_Start_Index, HowManyWindows);
		}
		this.How_Many_Windows = HowManyWindows;
		
		if (this.Window_Accuracy.Length < HowManyWindows)
			Array.Resize(ref this.Window_Accuracy, HowManyWindows);
		
		// Create the windows indexes in jumps of Disctance and make jitter on indies
		int part = 0;
		for (int i = 0; i < HowManyWindows ; i++) {
			if (i==0){
				if (Param.detector.Readout_Activity_during_Input_time==0){
					this.Window_Start_Index[i] = Param.detector.LSM_Adjustment_Time_at_Beginning +
						Param.detector.LSM_Adjustment_Time_at_Ending +
						Param.rnd.Next(0,Param.detector.ReadoutU_Window_Shifting);
				}else{
					this.Window_Start_Index[i] = Param.detector.LSM_Adjustment_Time_at_Beginning +
						Param.rnd.Next(0,Param.detector.ReadoutU_Window_Shifting);
				}
				part = (Liquid_Run_Time - this.Window_Start_Index[i])/HowManyWindows;
			}else if (i==(How_Many_Windows-1)){
				int temp  = 0;
				if (Param.detector.ReadoutU_How_Many_Windows==0)
					temp = this.Window_Start_Index[i-1] + Param.detector.ReadoutU_Disctance_Between_Windows+Window_Size;
				else
					temp = this.Window_Start_Index[i-1] + part;
				
				if (temp > Liquid_Run_Time)
					this.Window_Start_Index[i] = Liquid_Run_Time;
				else
					this.Window_Start_Index[i] = temp;
			}else{
				int current = this.Window_Start_Index[i-1];
				if (current>0)
					current += Param.rnd.Next(-Param.detector.ReadoutU_Window_Shifting,Param.detector.ReadoutU_Window_Shifting);
				if (Param.detector.ReadoutU_How_Many_Windows==0)
					current += Param.detector.ReadoutU_Disctance_Between_Windows + Window_Size;
				else
					current += part;
				this.Window_Start_Index[i] = current;
			}
		}
		this.initialization(ref Param,NumOfGroups);
	}

	public void copy(ref string file){
		
		FileStream fileStream = new FileStream(file, FileMode.Create,FileAccess.ReadWrite, FileShare.None);
		BinaryFormatter binaryFormater = new BinaryFormatter();
		
		binaryFormater.Serialize(fileStream, NetSize);
//		binaryFormater.Serialize(fileStream, NumberOfLiquidOutput);
		binaryFormater.Serialize(fileStream, OutputSize);
		binaryFormater.Serialize(fileStream, model);
		binaryFormater.Serialize(fileStream, Window_Accuracy);
//		binaryFormater.Serialize(fileStream, Per.Clone());
//		binaryFormater.Serialize(fileStream, patterns.Clone());
		binaryFormater.Serialize(fileStream, network.Clone());
//		binaryFormater.Serialize(fileStream, GtrainingSet.Clone());
//		binaryFormater.Serialize(fileStream, SVM.Clone());
//		binaryFormater.Serialize(fileStream, dataToTrain.Clone());
		
		fileStream.Close();
	}//----------------------------------------------------------------------------------

	public void load(ref string file){
		
		FileStream fileStream = new FileStream(file,FileMode.Open,FileAccess.Read, FileShare.Read);
		BinaryFormatter binaryFormater = new BinaryFormatter();
		
		NetSize = (int) binaryFormater.Deserialize(fileStream);
//		NumberOfLiquidOutput = (int) binaryFormater.Deserialize(fileStream);
		OutputSize = (int) binaryFormater.Deserialize(fileStream);
		model = (int) binaryFormater.Deserialize(fileStream);
		Window_Accuracy = (double[]) binaryFormater.Deserialize(fileStream);
//		Per = (Perceptron) binaryFormater.Deserialize(fileStream);
//		patterns = (List[]) binaryFormater.Deserialize(fileStream);
		network = (BasicNetwork[]) binaryFormater.Deserialize(fileStream);
//		GtrainingSet = (INeuralDataSet[]) binaryFormater.Deserialize(fileStream);
//		SVM = (Model[]) binaryFormater.Deserialize(fileStream);
//		dataToTrain = (Problem[]) binaryFormater.Deserialize(fileStream);
		
		fileStream.Close();
	}//----------------------------------------------------------------------------------


	public void CleanCollectedData()
	{
		if (this.model==1) {
			patterns = new List<Pattern>[0];
		}else if ((this.model>=2)&&(this.model<7)){
			for (int i = 0; i < this.GtrainingSet.Length; i++)
				this.GtrainingSet[i].Close();
			this.GtrainingSet = new IMLDataSet[0];
		}else if (this.model==7){
			dataToTrain = new Problem[0];
		}
	}//--------------------------------------------------------------------

	public int windowFinder(ref bool[,] LearnVec,ref bool[] input,out double[] trainingSet,
	                        int inputStart, int window,int timeInWin,int neg,int neurons)
	{
		int Activity=0;
		
		
		trainingSet = new double[DetectorInputSize];
		
		window += inputStart;
		timeInWin += window;
		
		if (timeInWin>LearnVec.GetLength(1))
			return Activity;
		
		int counter = 0;
		for (int n = 0; n < neurons; n++) {
			if (input[n]) continue;
			for (int t = window; t < timeInWin ; t++) {
				if (LearnVec[n,t] ){
					trainingSet[counter] = 1;
					Activity++;
				}else
					trainingSet[counter] = neg;
				counter++;
			}
		}
		
		return Activity;
	}


	public void SequenceFinder(ref double[] trainingSet, out double[] trainingResult,int neurons)
	{
		/// Shimon Maron Idea of sequance of firing in window
		trainingResult = new double[neurons];
		int[] ignoringTime = new int[this.Window_Size];
		for (int i = 0; i < this.Window_Size; i++) { ignoringTime[i] = 0;	}
		
		int backupTime=0,neu = 0,ncounter=0 , time =trainingSet.GetLength(0);
		for (int t = 0; t < time ; t++) {
			if (ncounter==neurons) ncounter++;
			if (trainingSet[t]==1)
			{
				int neuronNumber = t%this.Window_Size, temp = t/this.Window_Size;
				if (ignoringTime[neuronNumber]>0) {ignoringTime[neuronNumber]--; continue;}
				if (trainingSet[t]==1) continue;
				if (backupTime!=(temp)){
					backupTime =temp;
					neu++;
				}
				trainingResult[ncounter] = neu;
			}
			if (neu==this.Window_Size) break;
			
		}

	}

//	public void CollectData(ref bool[,] LearnVec,ref bool[] InputNeurons,ref globalParam.Data TargetVec,ref Liquid.LSM LSM,bool iniz,ref globalParam Param)
//	{
//		CollectData(ref LearnVec,ref InputNeurons, ref TargetVec.Target,ref LSM, TargetVec.Input.GetLength(1),iniz , ref Param);
//	}

	public void CollectData(ref bool[][,] LearnVec,ref bool[] InputNeurons,ref globalParam.Data[] TargetVec,ref Liquid.LSM LSM, ref int[] inputSize,int NumOfGroup,int NumOfGroups,ref globalParam Param)
	{
		int samples = LearnVec.Length;
		
		for (int sample = 0; sample < samples; sample++)
			CollectData(ref LearnVec[sample],ref InputNeurons,ref TargetVec[sample].Target,ref LSM, TargetVec[sample].Input.GetLength(1),(sample==0),NumOfGroup,NumOfGroups ,ref Param);
		
	}


	public int CollectData(ref bool[,] LearnVec,ref bool[] InputNeurons,ref double[] TargetVec,ref Liquid.LSM LSM, int inputSize,bool iniz,int NumOfGroup,int NumOfGroups, ref globalParam Param)
	{
		int neurons = LearnVec.GetLength(0) , times = LearnVec.GetLength(1);
		int Real_Inpur_Length = Param.input2liquid.ComputeInputTime(inputSize);
		int neg = 0;//Param.detector.Readout_Negative;
		if (model==10)
			neg = 0;//Param.detector.Readout_Negative;
		
		int MaxActivity = 0;
		
		DidWindowChange(ref LSM, inputSize,NumOfGroups,ref Param);
		
		for (int win = 0; win < How_Many_Windows; win++) {
			double[] dataset = new double[0];
			double[] datasetSeqence = new double[0];
			double trainingResult = TargetVec[0];
			if (Param.detector.Approximate_OR_Classification==1)
				trainingResult = TargetVec[win];
			
			int readoutNumber=0;
			if (this.ReadOut4EveryWindow == 2)
				readoutNumber=win;
			
			int Activity = 0;
			
			if (Param.detector.Readout_Activity_during_Input_time==1)
				Activity = windowFinder(ref LearnVec,ref InputNeurons,out dataset,0 , this.Window_Start_Index[win], Window_Size,neg,neurons);
			else if (Param.detector.Readout_Activity_during_Input_time==0)
				Activity = windowFinder(ref LearnVec,ref InputNeurons,out dataset,Real_Inpur_Length , this.Window_Start_Index[win], Window_Size,neg,neurons);
			
			if ((Activity==0)||(Activity==DetectorInputSize)) continue;
			if (Activity>MaxActivity) MaxActivity = Activity;
			
			
			if (this.model==1){
				
				if ((this.ReadOut4EveryWindow == 1 && win==0&iniz)||(this.ReadOut4EveryWindow == 2 &iniz))
					patterns[readoutNumber] = new List<Pattern>();
				if (this.ReadOut_unit_Model_AP_or_Sequ==1){
					patterns[readoutNumber].Add(new Pattern(dataset,new double[]{trainingResult}));
				}else if (this.ReadOut_unit_Model_AP_or_Sequ==2){
					SequenceFinder(ref dataset,out datasetSeqence,neurons);
					patterns[readoutNumber].Add(new Pattern(datasetSeqence,new double[]{trainingResult}));
				}
				
				
			}else if (((this.model>1)&&(this.model<7))||(this.model==9)){
				
				if (this.ReadOut_unit_Model_AP_or_Sequ==1){
					GtrainingSet[readoutNumber].Add(new BasicMLData(dataset),
					                                new BasicMLData(new double[]{trainingResult}));
				}else if (this.ReadOut_unit_Model_AP_or_Sequ==2){
					SequenceFinder(ref dataset,out datasetSeqence, neurons);
					GtrainingSet[readoutNumber].Add(new BasicMLData(datasetSeqence),
					                                new BasicMLData(new double[]{trainingResult}));
				}
				
			}else if (model==7) {
				
				if ((this.ReadOut4EveryWindow == 1 && win==0&iniz)||(this.ReadOut4EveryWindow == 2 &iniz)){
					dataToTrain[readoutNumber].Y = new double[How_Many_Windows];
					dataToTrain[readoutNumber].X = new Node[How_Many_Windows][];
				}
				
//				if (How_Many_Windows > dataToTrain[readoutNumber].Y.Length)
				
				
				if (this.ReadOut_unit_Model_AP_or_Sequ==1){
					dataToTrain[readoutNumber].Y[win] = trainingResult; // class lable
					dataToTrain[readoutNumber].X[0] = new Node[Window_Size];
					for (int num = 0; num < Window_Size ; num++) {
						dataToTrain[readoutNumber].X[0][num] = new Node(win,dataset[num]);  //i->index of the data point, the sec is data point
						dataToTrain[readoutNumber].MaxIndex++;
					}
				}else if (this.ReadOut_unit_Model_AP_or_Sequ==2){
					SequenceFinder(ref dataset,out datasetSeqence, neurons);
					dataToTrain[readoutNumber].Y[win] = trainingResult; // class lable
					dataToTrain[readoutNumber].X[0] = new Node[neurons];
					for (int num = 0; num < neurons ; num++) {
						dataToTrain[readoutNumber].X[0][num] = new Node(win,datasetSeqence[num]);  //i->index of the data point, the sec is data point
						dataToTrain[readoutNumber].MaxIndex++;
					}
				}
				
			}else if (model==8) {
				TempotronDetector[readoutNumber].CollectData(ref dataset, ref trainingResult);
				
			}else if (this.model==10){
				GtrainingSet[NumOfGroup].Add(new BasicMLData(dataset),
				                             new BasicMLData(dataset));
			}
			
		}
		
		return MaxActivity;
	}

	public void StratTrain(int MaxActivity, ref double LastReturnError,ref globalParam Param)
	{
		int HidenLayr=2;
		double globalError=0, error=0;
		int actualWindow = 0;
		int flag =0;
		do{
			BuildDetector(HidenLayr,ref Param);
			
			if (this.model==1)
				Console.WriteLine("Perceptron Training..");
			else if (((this.model>1)&&(this.model<7))||(model==10))
				Console.WriteLine("Start Encog Training..");
			else if (model==7)
				Console.WriteLine("Start SVM Training..");
			
			
			string modelName="";
			
			int temp = 1;
			if (this.ReadOut4EveryWindow==2) temp = How_Many_Windows;
			if (model==10)
				temp = this.network.Length;
			
			this.initialization(ref Param,0);
			
			for (int windowindex = 0 ; windowindex < temp ; windowindex++ ) {
				int readoutNumber=0;
				if ((model==10)||(this.ReadOut4EveryWindow == 2)) readoutNumber=windowindex;
				
				if (this.model==1){
					
					error= Per[readoutNumber].Train(patterns[windowindex]);
					if (this.ReadOut4EveryWindow == 2)
						this.Window_Accuracy[windowindex] = (error>1)? 0: 1-error;
					globalError += error/(patterns.Length);
					
				}else if (((this.model>1)&&(this.model<7))||(this.model==9)||(this.model==10)){
					IMLTrain train;
					switch (this.model) {
							
						case 2:
							train = new TrainAdaline(network[readoutNumber], this.GtrainingSet[windowindex], 0.001);
							modelName="Adaline";
							break;
							
						case 3:
							train = new Backpropagation(network[readoutNumber], this.GtrainingSet[windowindex]);
							modelName="Backpropagation";
							break;
							
						case 4:
							train = new ManhattanPropagation(network[readoutNumber], this.GtrainingSet[windowindex],0.1);
							train.AddStrategy(new SmartLearningRate());
							modelName="Manhattan Backpropagation";
							break;
							
						case 5:
							train = new ResilientPropagation(network[readoutNumber], this.GtrainingSet[windowindex]);
							modelName="Resilient Backpropagation";
							break;
							
						case 6:
							train = new ScaledConjugateGradient(network[readoutNumber], this.GtrainingSet[windowindex]);
							modelName="Scaled Conjugate Gradient";
							break;
							
						case 9:
							SupportVectorMachine SVM2 = new SupportVectorMachine();
							train = new SVMTrain(SVM2 , this.GtrainingSet[windowindex]);
							modelName="Encog SVM";
							break;
							
						case 10:
							train = new ResilientPropagation(network[readoutNumber], this.GtrainingSet[windowindex]);
							modelName="Resilient Backpropagation";
							Param.detector.ReadoutU_epoch = 250;
							break;
							
						default:
							train = new TrainAdaline(network[readoutNumber], this.GtrainingSet[windowindex], 0.01);
							modelName="Adaline";
							break;
					}
					int epoch = 1;
					
					do{
						train.Iteration();
						epoch++;
						if (epoch%51==0)
							Console.WriteLine("window "+windowindex.ToString()+" " +modelName+" Epoch #" + epoch.ToString() + " Error:" + train.Error.ToString());
					} while ((epoch < Param.detector.ReadoutU_epoch ) && (train.Error > Param.detector.Readout_Max_Error));
					
//					if ((model==10)&&((train.Error>0.49)||(train.Error==0))){
//						flag=0;
//						break;
//					}else
//						flag=1;
					
					Console.WriteLine("window "+windowindex.ToString()+" " +modelName+" Epoch #" + epoch.ToString() + " Error:" + train.Error.ToString());
					if (this.ReadOut4EveryWindow == 2){
						if ((train.Error>0)&&(train.Error<1)){
							this.Window_Accuracy[windowindex] = Math.Round(1-train.Error,2);
							globalError+=train.Error;
							actualWindow++;
						}else{
							this.Window_Accuracy[windowindex] = 0;
						}
					}
					flag=1;
					if (model==10){ // Find Threshold
						double hulf = (Param.detector.Readout_Negative + 1) /2.0;
						Window_Accuracy[readoutNumber] = 0;
						double[] Non_Relevent_Windows = new double[Window_Accuracy.Length];
						int[] count = new int[]{0,0};
						for (int Win = 0; Win < temp; Win++) {
							for (int c = 0; c < this.GtrainingSet[Win].Count; c++) {
								IMLDataPair input = new BasicMLDataPair(new BasicMLData(new double[]{0.0}),new BasicMLData(new double[]{0.0}));
								this.GtrainingSet[Win].GetRecord(c,input);
								double[] Netoutput = new double[input.InputArray.Length];
								network[readoutNumber].Compute(input.InputArray,Netoutput);
								double result = 0;
								for (int i = 0; i < input.InputArray.Length ; i++) {
									if ((input.InputArray[i] >= hulf && Netoutput[i] >= hulf) ||
									    (input.InputArray[i] < hulf && Netoutput[i] < hulf))
										result++;
								}
								if (Win == windowindex)
									Non_Relevent_Windows[readoutNumber] += result / input.InputArray.Length;
								else
									Window_Accuracy[readoutNumber] += result / input.InputArray.Length;
							}
							if (Win == windowindex)
								count[0] += this.GtrainingSet[Win].Count;
							else
								count[1] += this.GtrainingSet[Win].Count;
						}
						Window_Accuracy[readoutNumber] /= count[1];
						Non_Relevent_Windows[readoutNumber] /= count[0];
						if (Non_Relevent_Windows[readoutNumber]>Window_Accuracy[readoutNumber]){
							flag=0;
							break;
						}
					}
					train.FinishTraining();
					GtrainingSet[windowindex].Close();
					
				}else if (model==7) {
					
					//For this example (and indeed, many scenarios), the default
					//parameters will suffice.
					Parameter parameters = new Parameter();
					double C;
					double Gamma;
					for (int t = 0; t < How_Many_Windows ; t++) {
						//This will do a grid optimization to find the best parameters and store them in C and Gamma,
						//outputting the entire search to params.txt.
						ParameterSelection.Grid(dataToTrain[t], parameters, "params"+Param.test_Param.LSM_Damage.ToString()+".txt" , out C, out Gamma);
						parameters.C = C;
						parameters.Gamma = Gamma;
						
						//Train the model using the optimal parameters.
						this.SVM[readoutNumber] = Training.Train(dataToTrain[windowindex], parameters);
						
					}
				}else if (model==8) {
					this.Window_Accuracy[windowindex] = TempotronDetector[readoutNumber].Learn();
					globalError+=(1.0*globalError)/How_Many_Windows;
				}
			}
			HidenLayr++;
		}while(flag==0);
		if (actualWindow>0)
			globalError/=actualWindow;
		Console.WriteLine("Finish... Error = {0}",globalError);
		
		CleanCollectedData();
	}//--------------------------------------------------------------------

	public void StratTesting(ref bool[][,] TestVec,ref bool[] InputNeurons,ref Liquid.LSM LSM ,ref int[] inputSize, out  double[][] output,ref globalParam Param)
	{
		int numOfSamples = TestVec.Length;
		
		output = new double[numOfSamples][];
		
		if (this.model==1)
			Console.WriteLine("Testing Perceptron.....");
		else if ((this.model>1)&&(this.model<7))
			Console.WriteLine("Testing Encog.....");
		else if (model==7)
			Console.WriteLine("Testing SVM.....");
		else if (model==8)
			Console.WriteLine("Testing Tempotron.....");
		
		
		for (int sample = 0; sample < numOfSamples; sample++) {
			StratTesting(ref TestVec[sample],ref InputNeurons,ref LSM, ref inputSize[sample],out output[sample],ref Param);
		}
		CleanCollectedData();
	}//---------------------------------------------------------------------------------


	public void StratTesting(ref bool[,] TestVec,ref bool[] InputNeurons,ref Liquid.LSM LSM,ref int inputSize, out  double[] output,ref globalParam Param)
	{
		
		int neurons =TestVec.GetLength(0) ,	times = TestVec.GetLength(1);
		int Real_Inpur_Length = Param.input2liquid.ComputeInputTime(inputSize);
		
		//this.How_Many_Windows = ManyWindows(ref LSM,inputSize,ref Param);
		DidWindowChange(ref LSM, inputSize,0,ref Param);
		
		if ((this.ReadOut4EveryWindow == 2)||(Param.detector.Approximate_OR_Classification==1))
			output = new double[How_Many_Windows];
		else
			output = new double[1];
		
		if (this.model==10){
			output = new double[this.network.Length];
		}

		int neg = 0;//Param.detector.Readout_Negative;
		if (model==10)
			neg = 0;//Param.detector.Readout_Negative;;
		double hulf = (neg + 1) /2.0;
		
		int count=0;
		for (int win = 0; win < this.How_Many_Windows; win++) {
			
			double[] dataset = new double[0];
			double[] datasetSeqence = new double[0];
			
			int Activity = 0;
			
			if (Param.detector.Readout_Activity_during_Input_time==1)
				Activity = windowFinder(ref TestVec,ref InputNeurons,out dataset,0 , this.Window_Start_Index[win], Window_Size,neg,neurons);
			else if (Param.detector.Readout_Activity_during_Input_time==0)
				Activity = windowFinder(ref TestVec,ref InputNeurons,out dataset,Real_Inpur_Length , this.Window_Start_Index[win], Window_Size,neg,neurons);
			
			if ((Activity==0)||(Activity==Window_Size*TestVec.GetLength(0)))
				continue;
			
			int readoutNumber=0;
			if (this.ReadOut4EveryWindow == 2) readoutNumber=win;
			
			if (this.model==1){
				double temp=0;
				
				if (this.ReadOut_unit_Model_AP_or_Sequ==1){
					temp = Per[readoutNumber].Compute(dataset);
				}else if (this.ReadOut_unit_Model_AP_or_Sequ==2){
					SequenceFinder(ref dataset,out datasetSeqence,neurons);
					temp = Per[readoutNumber].Compute(datasetSeqence);
				}
				if (this.ReadOut4EveryWindow == 1)
					output[0] +=  temp;
				else{
					if (Param.detector.ReadOut_Ignore_Window_Acuracy==1)
						output[win] +=  temp ;
					else
						output[win] +=  temp * this.Window_Accuracy[win];
				}
			}else if (((this.model>1)&&(this.model<7))||(this.model==9)){
				if (readoutNumber>=network.Length) continue;
				double[] Netoutput = new double[OutputSize];
				if (this.ReadOut_unit_Model_AP_or_Sequ==1){
					network[readoutNumber].Compute(dataset,Netoutput);
				}else if (this.ReadOut_unit_Model_AP_or_Sequ==2){
					SequenceFinder(ref dataset,out datasetSeqence,neurons);
					network[readoutNumber].Compute(datasetSeqence,Netoutput);
				}
				if (Param.detector.Approximate_OR_Classification==1){
					output[win] =  Netoutput[0];
				}else{
					if (this.ReadOut4EveryWindow == 1){
						output[0] +=  Netoutput[0];
						count++;
					}else if (this.Window_Accuracy[readoutNumber]>0){
						if (Param.detector.ReadOut_Ignore_Window_Acuracy==1)
							output[win] +=  Netoutput[0];
						else
							output[win] +=  Netoutput[0] * this.Window_Accuracy[readoutNumber];
						count++;
					}
				}
			}else if (this.model==7){
				
				if (this.ReadOut_unit_Model_AP_or_Sequ==1){
					Node[] dataToTest = new Node[Window_Size];
					for (int neuron = 0; neuron < Window_Size ; neuron++) {
						dataToTest[neuron] = new Node(win,dataset[neuron]);// index and data point
					}
					output[0]= Prediction.Predict(this.SVM[readoutNumber], dataToTest);
				}else if (this.ReadOut_unit_Model_AP_or_Sequ==2){
					SequenceFinder(ref dataset,out datasetSeqence,neurons);
					Node[] dataToTest = new Node[TestVec.GetLength(1)];
					for (int neuron = 0; neuron < TestVec.GetLength(1) ; neuron++) {
						dataToTest[neuron] = new Node(win,datasetSeqence[neuron]);// index and data point
					}
					output[win]= Prediction.Predict(this.SVM[readoutNumber], dataToTest);
				}
			}else if (this.model==8){
				bool[,] temp = new bool[Window_Size,neurons];
				
				if (Param.detector.Readout_Activity_during_Input_time==1)
					windowFinder(ref TestVec,ref InputNeurons,out dataset,0 , this.Window_Start_Index[win], Window_Size,Param.detector.Readout_Negative,neurons);
				else if (Param.detector.Readout_Activity_during_Input_time==0)
					windowFinder(ref TestVec,ref InputNeurons,out dataset,Real_Inpur_Length , this.Window_Start_Index[win], Window_Size,Param.detector.Readout_Negative,neurons);

				double[] voltOutput;
				int Firing=0;
				TempotronDetector[readoutNumber].Run(ref temp ,out voltOutput,ref Firing);
				if (this.ReadOut4EveryWindow == 1)
					output[0] = Firing>0?1:Param.detector.Readout_Negative;
				else
					output[win] = this.Window_Accuracy[readoutNumber] * (Firing>0?1:Param.detector.Readout_Negative);
			}else if (this.model==10){
				for (int n = 0; n < this.network.Length ; n++) {
					double[] Netoutput = new double[dataset.Length];
					network[n].Compute(dataset,Netoutput);
					double result = 0;
					for (int i = 0; i < dataset.Length ; i++) {
						if ((dataset[i] >= hulf && Netoutput[i] >= hulf) ||
						    (dataset[i] < hulf && Netoutput[i] < hulf))
							result++;
					}
					result = result / dataset.Length;
					count++;
					if (result >= this.Window_Accuracy[n])
						output[n]++;
				}
			}
		}
		
		if (((this.model>1)&&(this.model<7))||(this.model==9)){
			if ((count>0)&&(output[0]>0)&&(this.ReadOut4EveryWindow == 1))
				output[0] /= count;
		}else if (this.model==10){
			for (int n = 0; n < this.network.Length ; n++)
				output[n] = output[n] / count;
		}
		
		
	}//---------------------------------------------------------------------------------



	public double StartLearnAproximation(ref bool[,] ActivityPatern,ref bool[] inputNueron,ref double[] Voxel,ref Liquid.LSM LSM,ref globalParam Param){
		double errorRate=0;
		CollectData(ref ActivityPatern,ref inputNueron,ref Voxel,ref LSM, Voxel.Length,true,1,1,ref Param);
		StratTrain(0, ref errorRate,ref Param);
		CleanCollectedData();
		
		return errorRate;
	}//---------------------------------------------------------------------------------

	public double[] StartAproximation(ref bool[][,] testData,ref bool[] inputNeuron,ref Liquid.LSM LSM, ref globalParam Param){
		double[] function = new double[0];
		double[][] temp;
		
		int[] inputsize = new int[testData.Length];
		for (int i = 0; i < inputsize.Length; i++) {
			inputsize[i] = testData[i].GetLength(1);
		}
		StratTesting(ref testData,ref inputNeuron,ref LSM,ref inputsize,out temp,ref Param);
		
		function = new double[temp.GetLength(1)];
		for (int i = 0; i < function.Length ; i++) {
			function[i] = temp[0][i];
		}
		
		return function;
	}//---------------------------------------------------------------------------------
}