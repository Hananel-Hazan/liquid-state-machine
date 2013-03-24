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
using System.Runtime.Hosting;
using NN_Pr;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data.Basic;
using Encog.Neural.NeuralData;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Networks.Training;
using Encog.Neural.Data;
using Encog.Neural.Networks.Training.Simple;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.SVM;
using Encog.Util.Banchmark;
using Encog.Util.Logging;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks.SVM;

using SVM;

//

public class ReadOut_Detector
{
	
	public int NetSize,InputSize,OutputSize,How_Many_Windows;
	private int model,mode,numOfReadOutUnits;
	private int[] Windows_Index;
	public double[] Window_Accuracy;
	public int kind;

	//
	Perceptron[] Per;
	List<Pattern>[] patterns;
	
	BasicNetwork[] network;
	INeuralDataSet[] GtrainingSet;
	ITrain train;
	
	Model[] SVM;
	Problem[] dataToTrain;
	
	Tempotron[] Tempotron_like;
	//

	
	public ReadOut_Detector(int inputSize, int hiddenSize, int outputSize,int place,ref globalParam Param)
	{
		//---------------------------------------------------------------------------------
		//   Init Backpropagation Network
		//---------------------------------------------------------------------------------
		Console.WriteLine("Inisilaize Readout Neuronal Network ");
		this.model = Param.detector.ReadOut_Unit[place];
		this.mode = Param.detector.Readout_Unit_Mode;
		this.How_Many_Windows = Param.detector.How_Many_Windows;
		this.kind  = Param.detector.ReadOut_unit_Detector_Model[place];
		
		if (mode==1){
			this.InputSize=inputSize;
		}else if (mode==2){
			this.InputSize=inputSize*Param.detector.ReadoutU_Window_Size;
		}
		//--------------
		if (this.kind==1){
			
		}else if (this.kind==2){
			this.OutputSize=outputSize;
			this.NetSize=InputSize+hiddenSize+outputSize;
			this.InputSize=inputSize;
		}
		//--------------
		this.OutputSize=outputSize;
		this.NetSize=InputSize+hiddenSize+outputSize;
		//--------------
		if (Param.detector.numOfReadOutUnits==1)
			this.numOfReadOutUnits=1;
		else
			this.numOfReadOutUnits = Param.detector.How_Many_Windows;
		//--------------
		if (model==1) {
			patterns = new List<Pattern>[this.numOfReadOutUnits];
			this.Per = new Perceptron[this.numOfReadOutUnits];
		}else if ((model>=2)&&(model<7)){
			this.GtrainingSet = new BasicNeuralDataSet[this.numOfReadOutUnits];
			this.network = new BasicNetwork[this.numOfReadOutUnits];
		}else if (model==7){
			this.SVM = new Model[this.numOfReadOutUnits];
			dataToTrain = new Problem[this.numOfReadOutUnits];
		}else if (model==8){
			this.InputSize=inputSize;
			this.OutputSize=outputSize;
			this.Tempotron_like = new Tempotron[1];
		}
		
		//--------------
		if (hiddenSize==0) hiddenSize=1;
		
		//--------------
		for (int i = 0 ; i <this.numOfReadOutUnits  ; i++)
		{
			if (model==1) {
				this.Per[i] = new Perceptron(this.InputSize,ref Param);
				patterns[i]= new List<Pattern>();
			}
			
			if (model==2) {
				this.network[i] = new BasicNetwork();
				this.network[i].AddLayer(new BasicLayer(new ActivationSoftMax(), true, this.InputSize));
				this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), true, this.OutputSize));
				
				this.network[i].Structure.FinalizeStructure();
				this.network[i].Reset();
			}
			
			if ((model>2)&&(model<7)){
				this.GtrainingSet[i] = new BasicNeuralDataSet();
				this.network[i] = new BasicNetwork();
				if (kind==1){
					this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), true, this.InputSize));
					this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), true, hiddenSize));
					this.network[i].AddLayer(new BasicLayer(new ActivationLinear(), true, this.OutputSize));
				}else if (kind==2){
					this.network[i].AddLayer(new BasicLayer(new ActivationLOG(), true, this.InputSize));;
					this.network[i].AddLayer(new BasicLayer(new ActivationLOG(), true, hiddenSize));
					this.network[i].AddLayer(new BasicLayer(new ActivationTANH(), true, this.OutputSize));
				}
//				Encog.MathUtil.Randomize.NguyenWidrowRandomizer randomizer = new Encog.MathUtil.Randomize.NguyenWidrowRandomizer(-1, 1);
//				randomizer.RandomGenerator = Param.rnd;
//				randomizer.Randomize(this.network[i]);
				this.network[i].Structure.FinalizeStructure();
				this.network[i].Reset();
			}
			
			if (model==7) {
				this.SVM[i] = new Model();
				dataToTrain[i] = new Problem();
			}
			
			if (model==8) {
				Tempotron_like[0] = new Tempotron(this.InputSize, ref Param);
			}
			
			if (model==9) {

			}
			
		}
		
		//
		
		
	}//--------------------------------------------------------------------
	
	public void CleanCollectedData()
	{
		if (this.model==1) {
			patterns = null;
		}else if ((this.model>=2)&&(this.model<7)){
			this.GtrainingSet = null;
		}else if (this.model==7){
			dataToTrain = null;
		}
		
	}//--------------------------------------------------------------------
	
	public void window(ref bool[,,] LearnVec,ref double[] TargetVec,out bool[,,] trainingSet, out double[] trainingResult,ref globalParam Param)
	{
		int tests = LearnVec.GetLength(0), neurons =  LearnVec.GetLength(1), time = LearnVec.GetLength(2);
		
		trainingSet = new bool[tests,Param.detector.How_Many_Windows,neurons*Param.detector.ReadoutU_Window_Size];
		
		if (TargetVec == null)
			trainingResult = new double[1];
		else
			trainingResult = new double[tests];
		
		for (int test = 0; test < tests; test++) {
			for (int Index_Window_Counter = 0; Index_Window_Counter < this.Windows_Index.Length ; Index_Window_Counter++){ // Times
				int place = 0;
				for (int windowSize=0 ; windowSize < Param.detector.ReadoutU_Window_Size ; windowSize++ ) { // window size
					for (int neuron = 0; neuron < neurons; neuron++) { // every neuron in the window
						trainingSet[test,Index_Window_Counter,place] = LearnVec[test,neuron, windowSize+this.Windows_Index[Index_Window_Counter]];
						place++;
					}
				}
			}
			if (TargetVec != null)
				if (Param.networkParam.Dynamic_OR_Static_synapses==1){
				trainingResult[test]=TargetVec[test%TargetVec.Length];
			}else{
				trainingResult[test]=TargetVec[test];
			}
		}
	}


	public void Sequence(ref bool[,,] trainingSet, ref int[,,] trainingResult ,ref globalParam Param)
	{
		int tests = trainingSet.GetLength(0), windows =  trainingSet.GetLength(1),
		neurons = this.InputSize , time = trainingSet.GetLength(2);

		for (int test = 0; test < tests; test++) {
			int[] ignoringTime = new int[this.InputSize];
			for (int i = 0; i < this.InputSize; i++) { ignoringTime[i] = 0;	}
			
			for (int windowIndex = 0; windowIndex < windows ; windowIndex++) {
				int neu = 0, backupTime =0;
				for (int t = 0; t < time ; t++) {
					if (trainingSet[test,windowIndex,t]==true)
					{
						int neuronNumber = t%this.InputSize, temp = t/this.InputSize;
						if (ignoringTime[neuronNumber]>0) {ignoringTime[neuronNumber]--; continue;}
						if (trainingSet[test,windowIndex,neuronNumber]==true) continue;
						if (backupTime!=(temp)){
							backupTime =temp;
							neu++;
						}
						trainingResult[test,windowIndex,neuronNumber] = neu;
					}
					if (neu==this.InputSize) break;
				}
			}
		}
	}

	public void CollectData(ref bool[,,] LearnVec,ref globalParam.Data[] TargetVec,ref globalParam Param)
	{
		double[] target = new double[TargetVec.Length];
		for (int i = 0; i < target.Length; i++) {
			target[i] = TargetVec[i].Target[0];
		}
		
		CollectData(ref LearnVec,ref target,ref Param);
	}
	
	public void CollectData(ref bool[,,] LearnVec,ref double[] TargetVec,ref globalParam Param)
	{
		// Create the windows indexes in jumps of Disctance and make jitter on indies
		int counter=0;
		if (Param.detector.Readout_Activity_during_Input_time==1)
			counter = Param.detector.LSM_AdjustmentTime;
		else if (Param.detector.Readout_Activity_during_Input_time==0){
			if (Param.input2liquid.Looping_Input==-1)
				counter = Param.detector.LSM_Runing_Interval;
			else if (Param.input2liquid.Looping_Input>0)
				counter = Param.detector.LSM_AdjustmentTime+(Param.test.Input_Pattern_Size*Param.input2liquid.LSM_Ratio_Of_Input_Interval[1]*Param.input2liquid.Looping_Input);
		}
		
		this.Windows_Index = new int[this.How_Many_Windows];
		for (int i = 0; i < this.Windows_Index.Length; i++) {
			this.Windows_Index[i] = ((i==0)||(i==this.Windows_Index.Length-1))? counter : counter+Param.rnd.Next(-5,5);
			counter+=Param.detector.ReadoutU_Disctance_Between_Windows+Param.detector.ReadoutU_Window_Size;
		}
		
		int samples = LearnVec.GetLength(0), neuron =LearnVec.GetLength(1) ,times = LearnVec.GetLength(2);
		
		bool[,,] trainingSet = new bool[samples,1,1];
		double[] trainingResult = new double[samples];
		int[,,] trainSequnce = new int[0,0,0];
		
		if (this.model==1){
			if (patterns==null){
				patterns = new List<Pattern>[this.numOfReadOutUnits];
				for (int i = 0 ; i <this.numOfReadOutUnits  ; i++)
					patterns[i]= new List<Pattern>();
			}
			
			if (this.kind==1) window(ref LearnVec,ref TargetVec,out trainingSet,out trainingResult, ref Param);
			if (this.kind==2) {
				window(ref LearnVec,ref TargetVec,out trainingSet,out trainingResult, ref Param);
				Sequence(ref trainingSet,ref trainSequnce, ref Param);
			}
			Console.WriteLine("Perceptron Collecting Data.....");
			int windows = trainingSet.GetLength(1),neurons =  trainingSet.GetLength(2);
			for (int windowindex = 0 ; windowindex < windows ; windowindex++ ) {
				for (int sample = 0; sample < samples; sample++) {
					double[] trainingSet_temp= new double[neurons];
					for (int neuon = 0; neuon < neurons; neuon++)

						if (trainingSet[sample,windowindex,neuon])
							trainingSet_temp[neuon] = 1;
						else
							trainingSet_temp[neuon] = 0;

					patterns[windowindex].Add(new Pattern(trainingSet_temp,new double[]{trainingResult[sample]}));
				}
			}
		}
		
		else if ((this.model>=2)&&(this.model<7)){
			if (this.kind==1) window(ref LearnVec,ref TargetVec,out trainingSet,out trainingResult, ref Param);
			if (this.kind==2) {
				window(ref LearnVec,ref TargetVec,out trainingSet,out trainingResult, ref Param);
				Sequence(ref trainingSet,ref trainSequnce, ref Param);
			}
			Console.WriteLine("Start Collecting Data ");
			int windows  = trainingSet.GetLength(1);
			int time = trainingSet.GetLength(2);
			
			if((GtrainingSet==null)||(GtrainingSet[0]==null)){
				this.GtrainingSet = new BasicNeuralDataSet[this.numOfReadOutUnits];
				for (int i = 0; i < this.numOfReadOutUnits; i++) {
					this.GtrainingSet[i] = new BasicNeuralDataSet();
				}
			}
			
			// train the neural network
			for (int i = 0 ; i < windows ; i++)
			{
				int readOutNumber=i;
				if (this.numOfReadOutUnits==1) readOutNumber=0;
				
				for (int sample = 0; sample < samples; sample++) {
					double[] trainingSet_temp = new double[time];
					for (int t = 0; t < time ; t++) {
						if(trainingSet[sample,i,t])
							trainingSet_temp[t] = 1;
						else
							trainingSet_temp[t] = 0;
					}
					GtrainingSet[readOutNumber].Add(new BasicNeuralData(trainingSet_temp),
					                                new BasicNeuralData(new double[]{trainingResult[sample]}));
				}
			}
		}
		
		else if (model==7) {
			if (this.kind==1) window(ref LearnVec,ref TargetVec,out trainingSet,out trainingResult, ref Param);
			if (this.kind==2) {
				window(ref LearnVec,ref TargetVec,out trainingSet,out trainingResult, ref Param);
				Sequence(ref trainingSet,ref trainSequnce, ref Param);
			}
			Console.WriteLine("Start Collecting Data ");
			//First, read in the training data.
			int neu = trainingSet.GetLength(2);
			int win = trainingSet.GetLength(1);
			int sample = trainingSet.GetLength(0);
			for (int t = 0; t < win; t++) {
				dataToTrain[t].Count =sample;
				dataToTrain[t].MaxIndex = 0;
				dataToTrain[t].Y = new double[sample];
				dataToTrain[t].X = new Node[sample][];
				for (int i = 0 ; i < sample ; i++){
					dataToTrain[t].Y[i] = trainingResult[i]; // class lable
					dataToTrain[t].X[i] = new Node[neu];
					double temp;
					for (int num = 0; num < neu ; num++) {
						if (trainingSet[i,t,num]) temp = 1;
						else temp = 0;
						dataToTrain[t].X[i][num] = new Node(t,temp);  //i->index of the data point, the sec is data point
						dataToTrain[t].MaxIndex++;
					}
					
				}
			}
		}else if (model==8) {
			
			Tempotron_like[0].CollectData(ref LearnVec, ref TargetVec, ref Param);
			
		}
	}
	
	
	public void StratTrain(ref double LastReturnError,ref globalParam Param)
	{
		
		if (this.model==1){
			Console.WriteLine("Perceptron Training.....");
			double error=0;
			for (int windowindex = 0 ; windowindex < this.How_Many_Windows ; windowindex++ ) {
				error+= Per[windowindex].Train(patterns[windowindex]);
			}
			Console.WriteLine("Finish... Error = {0}",(error/(this.How_Many_Windows)));
		}
		
		if ((this.model>=2)&&(this.model<7)){
			//07-10-2009
			Console.WriteLine("Start Training ");
			
			int backup = this.How_Many_Windows;
			if (this.numOfReadOutUnits==1) this.How_Many_Windows = 1;
			this.Window_Accuracy = new double[this.How_Many_Windows];
			
			// train the neural network
			for (int i = 0 ; i < this.How_Many_Windows ; i++)
			{
				string modelName;
				int readOutNumber=i;
				if (this.numOfReadOutUnits==1) readOutNumber=0;
				
				switch (this.model) {
						
					case 2:
						this.train = new TrainAdaline(network[readOutNumber], this.GtrainingSet[i], 0.001);
						modelName="Adaline";
						break;
						
					case 3:
						this.train = new Backpropagation(network[readOutNumber], this.GtrainingSet[i]);
						modelName="Backpropagation";
						break;
						
					case 4:
						this.train = new ManhattanPropagation(network[readOutNumber], this.GtrainingSet[i],0.1);
						this.train.AddStrategy(new SmartLearningRate());
						modelName="Manhattan Backpropagation";
						break;
						
					case 5:
						this.train = new ResilientPropagation(network[readOutNumber], this.GtrainingSet[i]);
						modelName="Resilient Backpropagation";
						break;
						
					case 6:
						this.train = new ScaledConjugateGradient(network[readOutNumber], this.GtrainingSet[i]);
						modelName="Scaled Conjugate Gradient";
						break;
						
					case 9:
						this.train = new SVMTrain(network[readOutNumber], this.GtrainingSet[i]);
						modelName="Encog SVM";
						break;
						
					default:
						this.train = new TrainAdaline(network[readOutNumber], this.GtrainingSet[i], 0.01);
						modelName="Adaline";
						break;
				}
				
				int epoch = 1;
				do
				{
					this.train.Iteration();
					epoch++;
				} while ((epoch < Param.detector.ReadoutU_epoch) && (train.Error > Param.detector.Readout_Max_Error));
				Console.WriteLine("window "+i.ToString()+" " +modelName+" Epoch #" + epoch.ToString() + " Error:" + train.Error.ToString());
				this.Window_Accuracy[i] = (train.Error>1)? 0: 1-train.Error;
				LastReturnError += train.Error;
			}
			LastReturnError = LastReturnError /How_Many_Windows;
			if (this.numOfReadOutUnits==1) this.How_Many_Windows = backup;
		}
		
		else if (model==7) {
			
			//For this example (and indeed, many scenarios), the default
			//parameters will suffice.
			Parameter parameters = new Parameter();
			double C;
			double Gamma;
			for (int t = 0; t < this.How_Many_Windows ; t++) {
				//This will do a grid optimization to find the best parameters and store them in C and Gamma,
				//outputting the entire search to params.txt.
				ParameterSelection.Grid(dataToTrain[t], parameters, "params"+Param.test.LSM_Damage.ToString()+".txt" , out C, out Gamma);
				parameters.C = C;
				parameters.Gamma = Gamma;
				
				//Train the model using the optimal parameters.
				this.SVM[t] = Training.Train(dataToTrain[t], parameters);
			}
		}
		
		else if (model==8) {
			for (int t = 0; t < this.Tempotron_like.Length ; t++) {
				Console.WriteLine("Detector {0}",t);
				LastReturnError = Tempotron_like[t].Learn(ref Param);
			}
		}
		
	}//--------------------------------------------------------------------


	public void StratTesting(ref bool[,,] TestVec, out  double[,] output,ref globalParam Param)
	{
		int negative =  Param.detector.Readout_Negative;

		bool[,,] tempOutput = new bool[1,1,1];
		double[] trainingResult = new double[1];
		int[,,] trainSequnce = new int[0,0,0];
		output = new double[1,1];
		
		if (this.model==1){
			if (this.kind==1) window(ref TestVec,ref trainingResult,out tempOutput,out trainingResult, ref Param);
			if (this.kind==2) {
				window(ref TestVec,ref trainingResult,out tempOutput,out trainingResult, ref Param);
				Sequence(ref tempOutput,ref trainSequnce, ref Param);
			}
			int samples = tempOutput.GetLength(0), windows = tempOutput.GetLength(1) , neurons = tempOutput.GetLength(2) ;
			output = new double[samples,windows];

			Console.WriteLine("Testing Perceptron.....");
			for (int sample = 0; sample < samples; sample++) {
				for(int win=0 ; win < windows ; win++){
					double[] temp = new double[neurons];
					for(int neu = 0 ; neu < neurons ; neu++){
						if (tempOutput[sample,win,neu])
							temp[neu] = 1;
						else
							temp[neu] = 0;
					}
					output[sample,win] = Per[win].Compute(temp);
				}
			}
		}else if ((this.model>1)&&(this.model<7)){
			if (this.kind==1) window(ref TestVec,ref trainingResult,out tempOutput,out trainingResult, ref Param);
			if (this.kind==2) {
				window(ref TestVec,ref trainingResult,out tempOutput,out trainingResult, ref Param);
				Sequence(ref tempOutput,ref trainSequnce, ref Param);
			}
			int samples = tempOutput.GetLength(0), windows = tempOutput.GetLength(1) , neurons = tempOutput.GetLength(2) ;
			output = new double[samples,windows];

			Console.WriteLine("Testing.....");
			if (this.numOfReadOutUnits==1)
				output = new double[samples,Param.detector.How_Many_Windows];
			else
				output = new double[samples,this.numOfReadOutUnits];
			for (int i = 0; i < samples; i++) {
				for(int win=0 ; win < windows ; win++){
					double[] temp = new double[neurons];
					for(int neu = 0 ; neu < neurons ; neu++){
						if (tempOutput[i,win,neu])
							temp[neu] = 1;
						else
							temp[neu] = 0;
					}
					INeuralData Netoutput;
					if (this.numOfReadOutUnits==1)
						Netoutput = this.network[0].Compute(new BasicNeuralData(temp));
					else
						Netoutput = this.network[win].Compute(new BasicNeuralData(temp));
					for (int t = 0; t < this.OutputSize; t++) {
						if (this.numOfReadOutUnits==1)
							output[i,win] =  Netoutput.Data[t];
						else
							output[i,win] =  Netoutput.Data[t] * this.Window_Accuracy[win];
					}
				}
			}
		}else if (this.model==7){
			if (this.kind==1) window(ref TestVec,ref trainingResult,out tempOutput,out trainingResult, ref Param);
			if (this.kind==2) {
				window(ref TestVec,ref trainingResult,out tempOutput,out trainingResult, ref Param);
				Sequence(ref tempOutput,ref trainSequnce, ref Param);
			}
			int samples = tempOutput.GetLength(0), windows = tempOutput.GetLength(1) , neurons = tempOutput.GetLength(2) ;
			output = new double[samples,windows];
			
			samples = tempOutput.GetLength(0); windows = tempOutput.GetLength(1); neurons = tempOutput.GetLength(2) ;
			output = new double[samples,windows];
			for (int i = 0; i < samples; i++) {
				for (int t = 0 ; t < windows ; t++){
					Node[] dataToTest = new Node[neurons];
					for (int neuron = 0; neuron < neurons ; neuron++) {
						double temp;
						if (tempOutput[i,t,neuron]) temp = 1;
						else temp = 0;
						dataToTest[neuron] = new Node(t,temp);// index and data point
					}
					output[i,t]= Prediction.Predict(this.SVM[t], dataToTest);
				}
			}
		}else if (this.model==8){

			output = new double[TestVec.GetLength(0),TestVec.GetLength(1)];

			for (int i = 0; i < Tempotron_like.Length; i++) {
				Tempotron_like[i].Run(ref TestVec,ref output,ref Param);
			}
		}
	}//---------------------------------------------------------------------------------

	//---------------------------------------------------------------------------------
	// Added from Esti Readout Progam
	// IMPORTENT NOTE : the Param.detector.numOfReadOutUnits and this.numOfReadOutUnits MUST BE == 1
	//---------------------------------------------------------------------------------
	
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
	}//---------------------------------------------------------------------------------
	
	public double Train(INeuralDataSet trainingSet, globalParam parameters)
	{
		// train the network
		ResilientPropagation train = new ResilientPropagation(network[0], trainingSet);
		int epoch = 1;
		do
		{
			train.Iteration();
			epoch++;
		} while ((epoch < parameters.detector.ReadoutU_epoch) && train.Error > parameters.detector.Readout_Max_Error);

		return train.Error;
	}//---------------------------------------------------------------------------------

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
			INeuralData output = network[0].Compute(dataPair.Input);
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
	}//---------------------------------------------------------------------------------

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
	}//---------------------------------------------------------------------------------

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
	}//---------------------------------------------------------------------------------

	public double GetRMS(double[] x, double[] y, bool print)
	{
		double[] x_norm = x; // Normalize(x);
		double[] y_norm = y; // Normalize(y);
		double diff_xy_sqrt = 0;
		double diff_xy = 0;
		for (int i = 0; i < x.Length; i++)
		{
//			if (print) Logger.WriteLine(x[i] + " " + y[i]);
			diff_xy = Math.Abs(x_norm[i] - y_norm[i]);
			diff_xy_sqrt += diff_xy * diff_xy;
		}

		return Math.Sqrt(diff_xy_sqrt / x.Length);
	}//---------------------------------------------------------------------------------

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
//			if (print)
//				Logger.WriteLine(x[i]+" "+y[i]);
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
	}//---------------------------------------------------------------------------------

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
//			Logger.WriteLine(x[i] + " " + y[i]);
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
	}//---------------------------------------------------------------------------------


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
//			if (print)
//			{
//				Logger.WriteLine(x[i]+" "+y[i]);
//			}
		}

		double upper = (x.Length - startIdx) * sum_xy - sum_x * sum_y;
		double lower = Math.Sqrt(Math.Abs((x.Length - startIdx) * sum_x_sqrt - sum_x * sum_x)) * Math.Sqrt(Math.Abs((x.Length - startIdx) * sum_y_sqrt - sum_y * sum_y));

		return upper / lower;
	}//---------------------------------------------------------------------------------

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
	}//---------------------------------------------------------------------------------
}
