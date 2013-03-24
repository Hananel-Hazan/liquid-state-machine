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
using Neurons;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Utils_Functions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace NeuronNetwork
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	[Serializable()]
	public class Network
	{
		//----------------------------------------------------------------
		//                    init ver
		//----------------------------------------------------------------
		Neuron[] InputNeurons;
		Neuron[] OutputNeurons;
		public Neuron[] ListOfAllNeurons;
		bool[,] ConnectivityMatrix;
		//----------------------------------------------------------------

		
		public Network(ref int NetOutputSize,ref globalParam Param,ref int[] ConnectionDistribution_Input,ref int[] ConnectionDistribution_Output)
		{
			int NetSize=Param.networkParam.Number_Of_Neurons;
			ConnectivityMatrix = new bool[NetSize, NetSize];
			bool[] InputConnectivityMatrix = new bool[NetSize];
			double[,] weight = new double[NetSize, NetSize];
			int[,] delay = new int[NetSize, NetSize];
			int[] posiORneg = new int[NetSize];

			// Neuron can be Only Negative(2) OR Positive(1) in its weights!!
			int[] temp = new int[(int)Math.Round(NetSize*Param.networkParam.LSM_Percent_Of_Negative_Weights)];
			int NumberOfNegNeurons = temp.Length;
			Param.rndA.select(0,NetSize,ref temp, ref Param);
			for (int i = 0 ;i<NumberOfNegNeurons ; i++ ) {	posiORneg[temp[i]]=1; }
			for (int i = 0; i < NetSize; i++){	posiORneg[i]++;}
			temp = null;

			//----------------------------------------------------------------------------------------------------------------------
			
			switch(Param.networkParam.Liquid_Architecture){
				case 0:
					this.RandomConnection(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 1:
					this.PowerLawConnection_Method1(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 2:
					this.FeedForwardWithHubs(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 3:
					this.Maass(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 4:
					this.GroupsPowerLaw(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 5:
					this.UncorrelatedScale_Free(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 6:
					this.UncorrelatedScale_Free_Powerlaw(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 7:
					this.PowerLawSelections(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 8:
					this.TwoWayPowerLaw(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 9:
					this.PowerLawX2Y2X(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 10:
					this.TwoWayLinearDescent(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 11:
					this.DoubleReversePowerLawMethodII(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 12:
					this.CombibeFFwithDoubleReversePowerLaw(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 13:
					this.CombibeFFwithTwoWayPowerLaw(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 14:
					this.CombibeFFwithUncorrelatedScale_Free(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 15:
					this.CombibeFFwithRandom(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 16:
					this.Mesh(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
				case 17:
					this.Click(ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
					break;
					
			}
			this.closingLoosened(ref ConnectivityMatrix,posiORneg,ref Param);
			this.finishNetwork(ref weight,ref delay,ref ConnectivityMatrix,NetSize,posiORneg,ref Param);
			Console.WriteLine("finish building connections");
			//----------------------------------------------------------------------------------------------------------------------
			// create Neuron List
			this.ListOfAllNeurons = new Neuron[NetSize];
			for (int i=0 ; i < NetSize ; i++ ) {
				this.ListOfAllNeurons[i] = new Neuron(Param.neuronParam.Neuron_Model, i,ref Param);
				this.ListOfAllNeurons[i].posiORneg=posiORneg[i];
			}
			
			// make/creat the connection between neurons
			for (int i = 0; i < NetSize; i++){
				Neuron  Source = this.ListOfAllNeurons[i].returnRef();
				
				Source.STDP = Param.neuronParam.Active_STDP_rule;
				Source.Neuron_propotional_weight_Update = Param.networkParam.Neuron_propotional_weight_Update;
				if (Param.neuronParam.Slideing_Threshold==2){
					if (Param.rnd.NextDouble()<=Param.networkParam.Precentege_Of_Slideing_Threshold)
						Source.Slideing_Threshold = 1;
					else
						Source.Slideing_Threshold = 0;
				}else
					Source.Slideing_Threshold = Param.neuronParam.Slideing_Threshold;
				
				for (int j = 0; j < NetSize; j++)
				{
					if (ConnectivityMatrix[i, j] == true)
					{
						Neuron Target = this.ListOfAllNeurons[j].returnRef();
						int wightIndex=Target.addNueronToInputList(ref Source, weight[i, j],delay[i, j],ref Param);
						Source.addNueronToOutputList(ref Target,wightIndex,delay[i, j],ref Param);
					}
				}
			}
			
			
			int inputNsize=Math.Max((int)Math.Round(NetSize*Param.networkParam.LSM_Input_Percent_Connectivity),1);
			int TotalConnections = 0;
			// count the input / output neurons
			for (int j = 0; j < NetSize; j++){
				InputConnectivityMatrix[j] = false;
				for (int i = 0; i < NetSize; i++){
					if (ConnectivityMatrix[j, i] == true){
						ConnectionDistribution_Output[j]++;
						ConnectionDistribution_Input[i]++;
						TotalConnections++;
					}
				}
			}
			
			Console.WriteLine("Total number of Connection : {0} from {1}",TotalConnections,Param.networkParam.Connections);
			
			int inputCounter;
			if (Param.networkParam.Methods_Of_Liquid_Input_Units==0){
				// Random chosen input neurons
				inputCounter = 0;
				while (inputCounter<inputNsize) {
					int inputCandidat = Param.rnd.Next(0,NetSize);
					if (InputConnectivityMatrix[inputCandidat]==true) continue;
					if (posiORneg[inputCandidat]==2) continue;
					InputConnectivityMatrix[inputCandidat] = true;
					inputCounter++;
				}
			}
			if (Param.networkParam.Methods_Of_Liquid_Input_Units==1){
				//The Input nueron must have Minimum input to him and more then zero output from him.
				inputCounter=0;
				double maxOutput,miniInput;
				Matrix_Arithmetic matrix = new Matrix_Arithmetic();
				miniInput = matrix.Min(ConnectionDistribution_Input);
				maxOutput = 1;
				while((inputCounter<inputNsize)&&(miniInput<=NetSize)){
					if (maxOutput>=NetSize){
						maxOutput=1;
						miniInput++;
					}
					for (int i = 0 ; i < ConnectionDistribution_Input.Length ; i++ ) {
						if ((InputConnectivityMatrix[i] == false)&&(inputCounter<inputNsize)&&
						    (ConnectionDistribution_Output[i]>0)&&
						    (ConnectionDistribution_Input[i]<=miniInput)) {
							InputConnectivityMatrix[i] = true;
							inputCounter++;
						}
					}
					maxOutput++;
				}
				if (inputNsize>inputCounter){
					// Random chosen input neurons
					Console.WriteLine("Error!! didn't find low Input Connectivity, Switching to random chosen input");
					inputCounter = 0;
					while (inputCounter<inputNsize) {
						int inputCandidat = Param.rnd.Next(0,NetSize);
						if (InputConnectivityMatrix[inputCandidat]==true) continue;
						InputConnectivityMatrix[inputCandidat] = true;
						inputCounter++;
					}
				}
			}
			if (Param.networkParam.Methods_Of_Liquid_Input_Units==2){
				//The Input nueron must have Minimum input to him and maximum output from him.
				inputCounter=0;
				int maxOutput,miniInput;
				Matrix_Arithmetic matrix = new Matrix_Arithmetic();
				miniInput = 0;
				maxOutput = matrix.Max(ConnectionDistribution_Output);
				while((inputCounter<inputNsize)&&(maxOutput>0)){
					if (miniInput>=NetSize){
						maxOutput--;
						miniInput=0;
					}
					for (int i = 0 ; i < ConnectionDistribution_Input.Length ; i++ ) {
						if ((InputConnectivityMatrix[i] == false)&&(inputCounter<inputNsize)&&
						    (ConnectionDistribution_Output[i]>=maxOutput)&&(ConnectionDistribution_Input[i]==miniInput)) {
							InputConnectivityMatrix[i] = true;
							inputCounter++;
						}
					}
					miniInput++;
				}
				if (inputCounter<inputNsize){
					// Random chosen input neurons
					Console.WriteLine("Error!! didn't find low Input Connectivity, Switching to random chosen input");
					while (inputCounter<inputNsize) {
						int inputCandidat = Param.rnd.Next(0,NetSize);
						if (InputConnectivityMatrix[inputCandidat]==true) continue;
						InputConnectivityMatrix[inputCandidat] = true;
						inputCounter++;
					}
				}
			}
			if (Param.networkParam.Methods_Of_Liquid_Input_Units==3){
				//   MIDDLE.
				Matrix_Arithmetic matrix = new Matrix_Arithmetic();
				inputCounter=0;
				int maxOutput,miniInput,maxConnection=matrix.Max(ConnectionDistribution_Output);
				miniInput = 0;
				maxOutput = Param.networkParam.Min_Group_Connections;
				while((inputCounter<inputNsize)&&(maxOutput<maxConnection)){
					if (miniInput*1.5>=maxOutput){
						maxOutput++;
						miniInput=0;
					}
					for (int i = 0 ; i < ConnectionDistribution_Input.Length ; i++ ) {
						if ((InputConnectivityMatrix[i] == false)&&(inputCounter<inputNsize)&&
						    (ConnectionDistribution_Output[i]==maxOutput)&&(ConnectionDistribution_Input[i]==miniInput)) {
							InputConnectivityMatrix[i] = true;
							inputCounter++;
						}
					}
					miniInput++;
				}
				if (inputCounter<inputNsize){
					// Random chosen input neurons
					Console.WriteLine("Error!! not enoght inputs choosen swiching to random chosen input");
					while (inputCounter<inputNsize) {
						int inputCandidat = Param.rnd.Next(0,NetSize);
						if (InputConnectivityMatrix[inputCandidat]==true) continue;
						InputConnectivityMatrix[inputCandidat] = true;
						inputCounter++;
					}
				}
			}

			// Creat External Input and Output List for Readout Neuron List
			int InputConnection = 0;
			for (int i = 0; i < NetSize; i++) if (InputConnectivityMatrix[i]) InputConnection++;
			NetOutputSize = NetSize - InputConnection;
			this.InputNeurons = new Neuron[InputConnection];
			this.OutputNeurons = new Neuron[NetOutputSize];
			inputCounter = 0;
			int OutputCounter = 0;
			for (int i = 0; i < NetSize; i++)
			{
				if (InputConnectivityMatrix[i]) {   // creat the Input list of the network
					this.InputNeurons[inputCounter] = this.ListOfAllNeurons[i].returnRef();
					this.ListOfAllNeurons[i].Input_OR_Output = 1;
					inputCounter++;
				}else{ // creat the Output list of the network
					this.OutputNeurons[OutputCounter] = this.ListOfAllNeurons[i].returnRef();
					this.ListOfAllNeurons[i].Input_OR_Output = 2;
					OutputCounter++;
				}
			}
//			this.changeTheresholdForInputNeurons(ref Param,inputCounter/2);
			System.Console.WriteLine("Number of Inputs Neurons : {0}",inputCounter);
			System.Console.WriteLine("Number of Neurons - Inputs : {0}",OutputCounter);
			System.Console.WriteLine("Number of Negative Neurons :{0}",NumberOfNegNeurons);
			this.FullReset(ref Param);
		} // End constractor
		//----------------------------------------------------------------
		
		public void FullReset(ref globalParam Param)
		{
			for (int i = 0; i < this.ListOfAllNeurons.Length; i++){
				this.ListOfAllNeurons[i].FullReset(ref Param);
			}
			
			if (Param.test_Param.BinaryInput_OR_RealValuse==1)
				this.changeTheresholdForInputNeurons(ref Param);
			
			// set the Threshold of neuron depending on Modle and Input Size
			double groupA=0,groupB=0;
			for (int i = 0 ; i < this.ListOfAllNeurons.Length ; i++ ){
				int inputNumber = this.ListOfAllNeurons[i].InputNeuronsList.Length;
				if (Param.neuronParam.Proportional_Threshold==0){
					if (Param.neuronParam.Neuron_Model==4){
						groupA=Param.neuronParam.Neuron_Threshold;
						groupB=Param.neuronParam.Neuron_Threshold;
					}else{
						groupA=Param.neuronParam.Neuron_Threshold;
						groupB=Param.neuronParam.Neuron_Threshold;
					}
				}else if (Param.neuronParam.Proportional_Threshold==1){
					if (Param.neuronParam.Neuron_Model==4){
						groupA=Param.neuronParam.Neuron_Threshold;
						groupB=(Param.neuronParam.Neuron_Threshold*(inputNumber*Param.neuronParam.Neuron_Threshold_Proportion));
						if (groupB<Param.neuronParam.Neuron_Threshold) groupB = Param.neuronParam.Neuron_Threshold;
					}else{
						groupA=Param.neuronParam.Neuron_Threshold;
						groupB=(Param.neuronParam.Neuron_Threshold*(inputNumber*Param.neuronParam.Neuron_Threshold_Proportion));
						if (groupB<Param.neuronParam.Neuron_Threshold) groupB = Param.neuronParam.Neuron_Threshold;
					}
				}
				if (inputNumber<=Param.networkParam.Group_size_Min)
					this.ListOfAllNeurons[i].Nunit.initTherashold = groupA;
				else
					this.ListOfAllNeurons[i].Nunit.initTherashold = groupB;
			}
			
		}//----------------------------------------------------------------
		
		public void reset(ref globalParam Param)
		{
			for (int i = 0; i < this.ListOfAllNeurons.Length; i++){
				this.ListOfAllNeurons[i].reset(ref Param);
			}
		}//----------------------------------------------------------------

		
		public void WhoIsInput(ref bool[] inputSignal){
			for (int i = 0; i < this.ListOfAllNeurons.Length; i++)
				if (this.ListOfAllNeurons[i].Input_OR_Output==1) inputSignal[i] = true;
			
		}
		
		public Int64[] run_on_vector(ref double[,] input, ref bool[,] output,
		                             ref bool[,] inputSignal,ref double[,] outputFrequency,ref double[,] NeuronTrace,ref int[] trackNeurons, int Time,
		                             int NorGorD, double percent,ref globalParam Param)
		{
			// create interference array
			int times_of_Interference=0,Interference_Counter = 0;
			int[] Input_Interference= new int[1]{Time+1};
			if (NorGorD == 0){ //Generalization Test
				int tempTime =Time - Param.detector.LSM_Adjustment_Time_at_Beginning -  Param.detector.LSM_Adjustment_Time_at_Ending;
				if (Param.input2liquid.Looping_Input>-1)
					tempTime = Param.test_Param.Input_Pattern_Size * Param.input2liquid.Looping_Input * Param.input2liquid.LSM_Ratio_Of_Input_Interval[1];
				if (percent>0)
					times_of_Interference = 1 + (int) Math.Round(tempTime * percent);
				Input_Interference  = new int[times_of_Interference];
				Param.rndA.select(0,tempTime, ref Input_Interference, ref Param);
				Array.Sort(Input_Interference);
				if (percent==0){
					Input_Interference = new int[1];
					Input_Interference[0] = Time+1;
				}
			}else{
				Input_Interference = new int[1];
				Input_Interference[0] = Time+1;
			}
			
			int freq_count=0,repeat_input=Param.input2liquid.Looping_Input,input_counter=0,flag=0,
			silence_or_repeted=Param.input2liquid.silence_or_repeted_Input_between_ratio,input_streem_lengh = input.GetLength(0),
			inputLengh = input.GetLength(1);
//			WinAndDistance=(Param.detector.LSM_1sec_interval+Param.detector.ReadoutU_Disctance_Between_Windows);
			double[] input_streem = new double[input.GetLength(0)];
			
			int[] neuronOrder = new int[this.ListOfAllNeurons.Length];
			for (int i = 0; i < this.ListOfAllNeurons.Length; i++)	neuronOrder[i]=i;
			
			int numberOfNeuronsForliveSignal = 0;
			if (Param.input2liquid.is_a_live_signal[0]==1) numberOfNeuronsForliveSignal=(int) Math.Max(1,Math.Round(InputNeurons.Length*Param.input2liquid.is_a_live_signal[1]));
			int numberOfNeuronsFor = 0;
			if (Param.input2liquid.inverse_Signal[0]==1) numberOfNeuronsFor=(int) Math.Max(1,Math.Round(InputNeurons.Length*Param.input2liquid.inverse_Signal[1]));
			
			Int64[] changes = new Int64[]{0,0,0};
			
			int outputCounter=0, ending = Time - Param.detector.LSM_Adjustment_Time_at_Ending;
			
			for (int runningTime = 0; runningTime < Time; runningTime++){
				if ((runningTime >= Param.detector.LSM_Adjustment_Time_at_Beginning) &&
				    (runningTime < ending)){
					// ----  dealing with the input--------
					input_streem = new double[input_streem_lengh];
					if (runningTime == Param.detector.LSM_Adjustment_Time_at_Beginning || (runningTime % Param.input2liquid.LSM_Ratio_Of_Input_Interval[1]) == 0)
					{
						if ( Param.input2liquid.LSM_Ratio_Of_Input_Interval[0]==1)
							input_counter++;
						else if  (Param.input2liquid.LSM_Ratio_Of_Input_Interval[0]>1)
							if (input_counter+Param.input2liquid.LSM_Ratio_Of_Input_Interval[0]<= inputLengh)
								input_counter+=Param.input2liquid.LSM_Ratio_Of_Input_Interval[0];
							else
								input_counter = input_counter+Param.input2liquid.LSM_Ratio_Of_Input_Interval[0] - inputLengh;
						flag=1;
					}
					if ((repeat_input>0)&&(input_counter%inputLengh==0)){repeat_input--;input_counter=1;}
					if (input_counter==inputLengh+1) input_streem = new double[input_streem_lengh];
					if ((repeat_input<0)||(repeat_input>0)) {
						if (flag==1){
							for (int size = 0 ; size < input_streem_lengh ; size++ )
								input_streem[size] = input[size,(input_counter-1) % inputLengh];
							flag = silence_or_repeted;
						}else{
							for (int size = 0 ; size < input_streem_lengh ; size++ )
								input_streem[size] = 0;
							flag=0;
						}
					}
					// ----  finish with the input--------
					
					// is a live signal
					if ((numberOfNeuronsForliveSignal>0)&(runningTime%Param.detector.LSM_1sec_interval==0)){
						if (input_streem_lengh==InputNeurons.Length)
							for (int i = input_streem_lengh-numberOfNeuronsForliveSignal ; i < input_streem_lengh; i++)
								input_streem[i] = Param.neuronParam.Ext_Inp_Neuron_Spike;
						else{
							Array.Resize(ref input_streem,InputNeurons.Length);
							for (int i = InputNeurons.Length-numberOfNeuronsForliveSignal ; i < InputNeurons.Length; i++)
								input_streem[i] = Param.neuronParam.Ext_Inp_Neuron_Spike;
						}
					}
					
					// ----  finish with the input--------
					
				}
				
				switch (NorGorD) {
					case 0:		//Generalization Test
						if (Input_Interference[Interference_Counter]==(input_counter-1)) {
							for (int size = 0 ; size < input_streem.Length ; size++ ) {
								if (Param.test_Param.BinaryInput_OR_RealValuse==0){
									if (input_streem[size] != 0)
										input_streem[size] = 0;
									else
										input_streem[size] = Param.neuronParam.Ext_Inp_Neuron_Spike;
								}else if (Param.test_Param.BinaryInput_OR_RealValuse==1){
//									input_streem[size] = 0;
//									input_streem[size] = Param.rndA.NextDouble(ref Param,Param.test_Param.RandomMaxMin_RealValues[0],Param.test_Param.RandomMaxMin_RealValues[1]);
									
									double temp = input_streem[size] * Param.rndA.NextDouble(ref Param,0,0.5);
									
									if(Param.rndA.NextDouble(ref Param,0,1)>0.5){
										if (input_streem[size] + temp > 150)
											input_streem[size] -= temp;
										else
											input_streem[size] += temp;
									}else{
										if (input_streem[size] - temp < -150 )
											input_streem[size] += temp;
										else
											input_streem[size] -= temp;
									}
								}
							}
						}
						this.InputToTheNet(input_streem,runningTime,ref inputSignal);
						break;
						
					case 1: // Normal Mode
						this.InputToTheNet(input_streem,runningTime,ref inputSignal);
						break;
						
					case 2: // Noise Generator
						this.InputToTheNet(input_streem,runningTime,ref inputSignal);
						spikeGenerator(percent,ref Param);
						break;
						
					case 3: // Dead Neurons
						this.InputToTheNet(input_streem,runningTime,ref inputSignal);
						Damage(percent,ref Param);
						break;
						
					case 4: // Combain Damage
						this.InputToTheNet(input_streem,runningTime,ref inputSignal);
						if (Param.rnd.NextDouble() <= 0.5)
							Damage(percent,ref Param);
						else
							spikeGenerator(percent,ref Param);
						break;
				}
				
				int OutputCounterHz=0;
				
				if (Param.networkParam.Liquid_Update_Sync_Or_ASync == 1 ) // Neurons update in Async ways
					Param.rndA.select(0,this.ListOfAllNeurons.Length,ref neuronOrder, ref Param);
				
				int traceCounter = 0;
				
				for (int i = 0; i < neuronOrder.Length; i++){
					int[] change = new int[]{0,0,0};
					
					output[this.ListOfAllNeurons[neuronOrder[i]].name, outputCounter] =
						this.ListOfAllNeurons[neuronOrder[i]].step(runningTime,ref change,ref Param);
					
					changes[0]+=change[0];  // LTD
					changes[1]+=change[1];	// LTP
					changes[2]+=change[2];	// Thereshold
					
					if ((trackNeurons.Length>traceCounter)&&(trackNeurons[traceCounter]==i)){
						NeuronTrace[traceCounter,outputCounter] = this.ListOfAllNeurons[neuronOrder[i]].Nunit.V;
						traceCounter++;
					}
					
					if (runningTime%Param.detector.LSM_1sec_interval==0){
						if (this.ListOfAllNeurons[neuronOrder[i]].Input_OR_Output==2){
							outputFrequency[OutputCounterHz,freq_count] = this.ListOfAllNeurons[neuronOrder[i]].Number_Fireing_in_Second;
							OutputCounterHz++;
						}
					}
				}
				
				outputCounter++;
				if (runningTime%Param.detector.LSM_1sec_interval==0){
					freq_count++;
				}

			}
			return changes;
		}//----------------------------------------------------------------
		
		public void Save_Neurons_Stat(ref globalParam Param){
			for (int i = 0; i < this.ListOfAllNeurons.Length; i++) {
				this.ListOfAllNeurons[i].Save_State(ref Param);
			}
		}
		
		
		public void Dump_Network_Weights(string path){
			path = path+@".txt";
			StreamWriter reportFile = new StreamWriter(@path,true);

			for (int i = 0; i < this.ListOfAllNeurons.Length; i++) {
				for (int n = 0; n < this.ListOfAllNeurons[i].Input_weights.Length; n++) {
					reportFile.Write(this.ListOfAllNeurons[i].Input_weights[n].ToString()+"	");
				}
			}
			reportFile.WriteLine();
			reportFile.Flush();
			reportFile.Close();
		}
		
		
		public void InputToTheNet(double[] input,int time,ref bool[,] inputSignal)
		{
			int input_lenght = inputSignal.GetLength(0)*inputSignal.GetLength(1);
			
			if (input.Length>this.InputNeurons.Length)
			{
				Console.WriteLine("ERROR : INPUT IS BIGGER THEN THE NUMBER OF INPUT UNITS IN THE LIQUID!!!!");
			}else{
				int input_group=this.InputNeurons.Length/input.Length;
				for (int i = 0; i < input.Length; i++)
				{
					if (input[i]==0) continue;
					for (int j = input_group * i; j < input_group * (i+1) ; j++)
					{
						this.InputNeurons[j].ExternalIntput = input[i];
						if (((input[i]>0)||(input[i]<0))&&(input_lenght>0)) inputSignal[j,time] = true;
					}
				}
				int lastIdx = input.Length-1;
				if (input[lastIdx]>0){
					for (int i = input_group*input.Length ; i < this.InputNeurons.Length ; i++){
						this.InputNeurons[i].ExternalIntput = input[lastIdx];
						if (((input[lastIdx]>0)||(input[lastIdx]<0))&&(input_lenght>0)) inputSignal[i,time] = true;
					}
				}
			}
		}//----------------------------------------------------------------

		public void spikeGenerator(double percent,ref globalParam Param)
		{
			for (int i = 0; i < this.ListOfAllNeurons.Length; i++){
				if (Param.rnd.NextDouble() <= percent){
					this.ListOfAllNeurons[i].mode = 2;
				}
			}
		}//----------------------------------------------------------------

		public void Damage(double percent,ref globalParam Param)
		{
			for (int i = 0; i < this.ListOfAllNeurons.Length; i++){
				if (Param.rnd.NextDouble() <= percent){
					this.ListOfAllNeurons[i].mode = 3;
				}
			}
		}//----------------------------------------------------------------

		public void DOTFile()
		{
			TextWriter tw1 = new StreamWriter("DOT network.txt");
			tw1.WriteLine("digraph \"Network Connectivity\"{");
			for(int y = 0 ; y < ConnectivityMatrix.GetLength(0) ; y++){
				int flag = 0;
				for ( int x = 0 ; x < ConnectivityMatrix.GetLength(1) ; x++ ) {
					if ( ConnectivityMatrix[x,y]==true ) {
						if ( flag ==0) { tw1.Write("	"+(1+y).ToString()+" -> "+(1+x).ToString()); flag++; }
						else tw1.Write(" -> "+(1+x).ToString());
					}
				}
				tw1.WriteLine(";");
			}
			tw1.WriteLine("}");
			tw1.Close();
		}
		//----------------------------------------------------------------
		public void PrintNetwork()
		{
			this.PrintNetwork(ref ConnectivityMatrix);
		}
		//----------------------------------------------------------------

		public void PrintNetwork(ref Boolean[,] connectionMatrix)
		{
			// Print the network
			int NetSizeX = connectionMatrix.GetLength(0);
			for (int i = 0; i < NetSizeX; i++){
				int NetSizeY = connectionMatrix.GetLength(1);
				for (int j = 0; j < NetSizeY; j++){
					if (connectionMatrix[i, j] == true){System.Console.Write("1");}
					else {System.Console.Write("0");}
				}
				System.Console.WriteLine("  ");
			}
		}
		//----------------------------------------------------------------

		
		public void closingLoosened(ref bool[,] ConnectivityMatrix,int[] posiORneg, ref globalParam Param)
		{
			int netsize = posiORneg.Length;
			Matrix_Arithmetic mat = new Matrix_Arithmetic();
			int[] noOutputNodes = new int[0];
			int[] noInputNodes = new int[0];
			int[] sum_nodes_Inputs = new int[netsize];
			int[] sum_nodes_Outputs = new int[netsize];
			
			// counting the nodes
			for (int x = 0 ; x < netsize ; x++ ) {
				for (int y = 0 ; y < netsize ; y++ ) {
					if (ConnectivityMatrix[x,y]==true) {
						sum_nodes_Outputs[x]++;
						sum_nodes_Inputs[y]++;
					}
				}
			}
			int Zero_Input_nodes = mat.Count(sum_nodes_Inputs, 0,ref noInputNodes);
			int Zero_Output_nodes = mat.Count(sum_nodes_Outputs, 0,ref noOutputNodes);
			//--
			
			// find if there is any node that is both in noInput and noOutput - spacial case!
			int[] spacialCase = new int[0];
			for (int i = 0; i < noInputNodes.Length; i++) {
				for (int t = 0; t < noOutputNodes.Length; t++) {
					if (noInputNodes[i]==noOutputNodes[t])
						mat.AddCell(ref spacialCase,noInputNodes[i]);
				}
			}
			if (spacialCase.Length>0){
				for (int i = 0; i < spacialCase.Length; i++) {
					int[] candidate = new int[2];
					Param.rndA.dselect(0,netsize,spacialCase,ref candidate,ref Param);
					ConnectivityMatrix[candidate[0],candidate[1]]=true;
					sum_nodes_Inputs[candidate[1]]++;
					sum_nodes_Outputs[candidate[0]]++;
				}
			}
			//--
			
			noInputNodes = new int[0];
			Zero_Input_nodes = mat.Count(sum_nodes_Inputs, 0,ref noInputNodes);
			noOutputNodes = new int[0];
			Zero_Output_nodes = mat.Count(sum_nodes_Outputs, 0,ref noOutputNodes);
			if (Zero_Input_nodes>0){
				int[] candidates = new int[0];
				int temp = 0;
				for (int i = 1; i < netsize ; i++) {
					int[] tempcandidates = new int[0];
					temp+=mat.Count(sum_nodes_Outputs, i ,ref tempcandidates);
					mat.AddCells(ref candidates,ref tempcandidates);
					if (temp>=Zero_Input_nodes) break;
				}
				if (candidates.Length == 0) {
					candidates = new int[Zero_Input_nodes];
					Param.rndA.select(0,netsize,ref candidates,ref Param);
				}
				for (int i = 0; i < Zero_Input_nodes ; i++) {
					int candidate = Param.rnd.Next(0,candidates.Length);
					ConnectivityMatrix[candidates[candidate],noInputNodes[i]]=true;
					sum_nodes_Inputs[noInputNodes[i]]++;
					sum_nodes_Outputs[candidates[candidate]]++;
				}
			}
			//--
			
			noInputNodes = new int[0];
			Zero_Input_nodes = mat.Count(sum_nodes_Inputs, 0,ref noInputNodes);
			noOutputNodes = new int[0];
			Zero_Output_nodes = mat.Count(sum_nodes_Outputs, 0,ref noOutputNodes);
			if (Zero_Output_nodes>0){
				int[] candidates = new int[0];
				int temp = 0;
				for (int i = 1; i < netsize ; i++) {
					int[] tempcandidates = new int[0];
					temp+=mat.Count(sum_nodes_Inputs, i ,ref tempcandidates);
					mat.AddCells(ref candidates,ref tempcandidates);
					if (temp>=Zero_Output_nodes) break;
				}
				if (candidates.Length == 0) {
					candidates = new int[Zero_Output_nodes];
					Param.rndA.select(0,netsize,ref candidates,ref Param);
				}
				
				for (int i = 0; i < Zero_Output_nodes ; i++) {
					int candidate = Param.rnd.Next(0,candidates.Length);
					ConnectivityMatrix[noOutputNodes[i],candidates[candidate]]=true;
					sum_nodes_Inputs[candidates[candidate]]++;
					sum_nodes_Outputs[noOutputNodes[i]]++;
				}
			}
			//--
			// check if there is an Input unit that dont have output
			for (int i = 1; i < netsize ; i++) {
				int output=0;
				for (int o = 0; o < netsize; o++)
					if (ConnectivityMatrix[i,o]==true) output++;
				if (output==0){
					int[] candidate = new int[Param.networkParam.Connections*netsize];
					Param.rndA.dselect(0,netsize,i,ref candidate,ref Param);
					for (int o = 0; o < candidate.Length; o++)
						ConnectivityMatrix[i,o]=true;
				}
			}
			//--
			
		}//----------------------------------------------------------------------------------------------------------------------
		
		public void finishNetwork(ref double[,] weight,ref int[,] delay,ref bool[,] ConnectivityMatrix,
		                          int NetSize,int[] posiORneg, ref globalParam Param)
		{
			//------------------------- finish part ----------------------------------
			// create : Connection, weight and delay between the neurons
			for (int x = 0 ; x < NetSize ; x++ ) {
				for (int y = 0 ; y < NetSize ; y++ ) {
					if (ConnectivityMatrix[x,y]==true){
						if (posiORneg[x]==1) {
							weight[x,y]=(Param.rndA.NextDouble(ref Param,Param.networkParam.LSM_Min_Init_Weight_PosN,Param.networkParam.LSM_Max_Init_Weight_PosN));
							delay[x,y]=Param.rnd.Next(Param.neuronParam.Neuron_Min_Delay,Param.neuronParam.Neuron_Max_Delay);
						}else{
							weight[x,y]=-1*((Param.rndA.NextDouble(ref Param,Param.networkParam.LSM_Min_Init_Weight_NegN,Param.networkParam.LSM_Max_Init_Weight_NegN)));
							delay[x,y]=Param.rnd.Next(Param.neuronParam.Neuron_Min_Delay,Param.neuronParam.Neuron_Max_Delay);
						}
					}
				}
			}
		}//----------------------------------------------------------------------------------------------------------------------
		
		public void changeTheresholdForInputNeurons(ref globalParam Param)
		{
//			double current = Param.neuronParam.Neuron_Threshold;
			int mone = this.InputNeurons.Length;
//			double temp=current,jump = (current-(-1*(current/2))) / mone;
			
			double  step = Math.Abs((Param.neuronParam.initV - Param.neuronParam.Neuron_Threshold)/mone);

//			double backupDecay = Param.neuronParam.decayFactor;
//			Param.neuronParam.decayFactor = 0.8 ;
			
			for (int i = 0; i < mone; i++) {
//				if (i%mone==0){
//					jump = (2*current) / mone;
//					temp=current;
//				}
				this.InputNeurons[i].FullReset(ref Param);
//				this.InputNeurons[i].STDP = 0;
//				this.InputNeurons[i].Slideing_Threshold = 1;
//				this.InputNeurons[i].Nunit.initTherashold = temp;	temp-=jump;
				
				if (i>0)
					this.InputNeurons[i].Nunit.initTherashold = this.InputNeurons[i-1].Nunit.initTherashold + step;
				else 
					this.InputNeurons[i].Nunit.initTherashold = Param.neuronParam.initV+5;
				
//				this.InputNeurons[i].RandomElemnt_double = Param.rnd.NextDouble() *10 ;
//				this.InputNeurons[i].reset(ref Param);
//				this.InputNeurons[i].Nunit = (NeuronArch.IzakNeuron) new NeuronArch.IzakNeuron(ref Param.neuronParam);
				this.InputNeurons[i].reset(ref Param);
			}
			
//			Param.neuronParam.decayFactor = backupDecay;
		}//----------------------------------------------------------------------------------------------------------------------





		//----------------------------------------------------------------------------------------------------------------------
		/// T O P O L O G Y!!!
		//----------------------------------------------------------------------------------------------------------------------
		
		public void RandomConnection(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			// Random Connections
			// create : Connection, weight and delay between the neurons
			int TotalConnection =0;
			while (TotalConnection < Param.networkParam.Connections) {
				for (int x = 0 ; x< NetSize ; x++ ) {
					for (int y = 0 ; y < NetSize ; y++ ) {
						if (TotalConnection >= Param.networkParam.Connections) break;
						if (x==y) continue;
						if ((Param.rnd.NextDouble()<=Param.networkParam.GeneralConnectivity)&&(ConnectivityMatrix[x, y] == false)) {
							ConnectivityMatrix[x, y] = true;
							TotalConnection++;
						}
					}
				}
			}
		}//----------------------------------------------------------------------------------------------------------------------

		public void PowerLawConnection_Method1(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			// stat the minimul Group connections
			int Min_Connection = Param.networkParam.Min_Neuron_Connection;
			int Group_size = Param.networkParam.Group_size;
			int Netsize = ConnectivityMatrix.GetLength(0)-1;
			for (int x = 0 ; x< Netsize ; x++ ) {
				int group_start = (x/Group_size) * Group_size , group_end = group_start + Group_size ;
				if (group_end>Netsize) group_end = Netsize;
				int[] candidates = new int[Min_Connection];
				Param.rndA.dselect(group_start,group_end,x,ref candidates, ref Param);
				for (int i = 0 ; i < Min_Connection ; i++) { ConnectivityMatrix[x,candidates[i]]=true; }
			}
			
			// stat the random sellection acording to powerlow
			int[] connectionProbability = new int[ConnectivityMatrix.GetLength(0)];
			for ( int i = 0 ; i<connectionProbability.Length ; i++ ) { connectionProbability[i]=i;	}
			int[] outputNodes,inputNodes,listOfoutputNodes;
			Param.rndA.random_power_low_sellect_Mathod1(Min_Connection,connectionProbability,Param.networkParam.Connections*10,Param.networkParam.IncreaseSelectedChanceBy,out outputNodes,out inputNodes, ref Param);
			int counter = 0;
			listOfoutputNodes = new int[outputNodes.LongLength];
			Param.rndA.select(0,outputNodes.Length, ref listOfoutputNodes, ref Param);
			Param.rndA.shuffle(ref listOfoutputNodes, ref Param);

			for (int candidant = 0 ; candidant < inputNodes.Length ; candidant++){
				for ( int times = 0 ; times < inputNodes[candidant] ; times++ ) {
					int node = outputNodes[listOfoutputNodes[counter]];
					/*
					 *   I know that here there is a mistake, if there is already connection between
					 * 	 candidant ---> node  (candidant to node) the cennection will be "mark" as true
					 * 	 again, if this mistake will be corrected the results will be worse
					 */
					//if (node==candidant) counter--;
					while (node==candidant) { node= outputNodes[listOfoutputNodes[Param.rnd.Next(0,outputNodes.Length)]];}
					counter++;
					ConnectivityMatrix[node,candidant]=true;
				}
			}
			
			// check if there is a group that dont have ANY output to the reset of the net
			int[] island_group_Out_Connection = new int[0];
			int flag=0;
			for (int x = 0 ; x< ConnectivityMatrix.GetLength(0) ; x++ ) {
				int group_start = (x/Group_size) * Group_size , group_end = group_start + Group_size ;
				if ( x%Group_size==0){
					if ((flag<Param.networkParam.Min_Group_Connections)&&(x>0)){
						Array.Resize(ref island_group_Out_Connection,island_group_Out_Connection.Length+1);
						island_group_Out_Connection[island_group_Out_Connection.Length-1]=(x-1)/Group_size;
					}else	flag=0;
					if (x==ConnectivityMatrix.GetLength(0)) continue;
				}
				for (int y = 0 ; y < ConnectivityMatrix.GetLength(1) ; y++ ) {
					if ((y >= group_start)&&(y < group_end)) { continue;}
					if (ConnectivityMatrix[x,y]==true) { flag++; }
				}
			}
			for(int i = 0 ; i < island_group_Out_Connection.Length ; i++){
				int group_start = island_group_Out_Connection[i] * Group_size , group_end = group_start + Group_size ;
				int x=0,y=0;
				while (x==y) {
					x=Param.rnd.Next(group_start,group_end);
					y=(int)outputNodes[listOfoutputNodes[Param.rnd.Next(0,outputNodes.Length)]];
				}
				ConnectivityMatrix[x,y]=true;
			}
			
			// check if there is a group that dont have ANY input from the reset of the net
			int[] island_group_In_Connection = new int[0];
			flag=0;
			for (int x = 0 ; x< ConnectivityMatrix.GetLength(1) ; x++ ) {
				int group_start = (x/Group_size) * Group_size , group_end = group_start + Group_size ;
				if ( x%Group_size==0){
					if ((flag<Param.networkParam.Min_Group_Connections)&&(x>0)){
						Array.Resize(ref island_group_In_Connection,island_group_In_Connection.Length+1);
						island_group_In_Connection[island_group_In_Connection.Length-1]=(x-1)/Group_size;
					}else	flag=0;
					if (x==ConnectivityMatrix.GetLength(1)) continue;
				}
				for (int y = 0 ; y < ConnectivityMatrix.GetLength(0) ; y++ ) {
					if ((y >= group_start)&&(y < group_end)) { continue;}
					if (ConnectivityMatrix[y,x]==true) { flag++; }
				}
			}
			for(int i = 0 ; i < island_group_In_Connection.Length ; i++){
				int group_start = island_group_In_Connection[i] * Group_size , group_end = group_start + Group_size ;
				int x=0,y=0;
				while (x==y) {
					x=Param.rnd.Next(group_start,group_end);
					y=(int)outputNodes[listOfoutputNodes[Param.rnd.Next(0,outputNodes.Length)]];
				}
				ConnectivityMatrix[y,x]=true;
			}
			
			outputNodes = new int[1];
			inputNodes = new int[1];
			outputNodes = null;
			inputNodes = null;
		}//----------------------------------------------------------------------------------------------------------------------

		public void FeedForwardWithHubs(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int TotalConnections=0;
			// stat the minimul Group connections
			int[] Group_size = new int[0]{};
			int Group_size_Max=Param.networkParam.Group_size_Max+1,
			Group_size_Min=Param.networkParam.Group_size_Min,
			Group_sum=0,
			Min_InterConnections = Param.networkParam.Group_interconnections[0],
			Max_InterConnections = 0,
			endGroup,startGroup,counter;
			
			if (Param.networkParam.GroupSize==null) {
				Matrix_Arithmetic Utils = new Matrix_Arithmetic();
				do{
					int candidateGroup = Param.rnd.Next(Group_size_Min,Group_size_Max);
					if ((Group_sum+Group_size_Max+1)>=ConnectivityMatrix.GetLength(0))
						candidateGroup = (ConnectivityMatrix.GetLength(0) - Group_sum);
					Group_sum += candidateGroup;
					Utils.AddCell(ref Group_size,candidateGroup);
				}while (Group_sum<ConnectivityMatrix.GetLength(0));
				
				endGroup = Group_size[0]-1; startGroup=0 ; counter = 0 ;
				for (int x = 0 ; x < ConnectivityMatrix.GetLength(0) ; x++ ) {
					if ((x == endGroup) && (counter+1<Group_size.Length)) {
						counter++;
						startGroup = endGroup;
						endGroup += Group_size[counter];
					}
					int[] candidates;
					if (Param.networkParam.Group_interconnections.Length==2){
						if (Param.networkParam.Group_interconnections[1]>=Group_size[counter])
							Max_InterConnections = Group_size[counter]-1;
						else
							Max_InterConnections = Param.networkParam.Group_interconnections[1];
					}else
						Max_InterConnections = Group_size[counter]-1;
					if (Max_InterConnections < Min_InterConnections)
						candidates = new int[Param.rnd.Next(Max_InterConnections,Min_InterConnections+1)];
					else
						candidates = new int[Param.rnd.Next(Min_InterConnections,Max_InterConnections+1)];
					Param.rndA.dselect(startGroup,endGroup,x,ref candidates, ref Param);
					for (int i = 0 ; i < candidates.Length ; i++ ) {
						ConnectivityMatrix[x,candidates[i]]=true;
						TotalConnections++;
					}
				}
			}else{
				for (int i = 0 ;  i < ConnectivityMatrix.GetLength(0) ; i++ )
					for (int t = 0 ;  t < ConnectivityMatrix.GetLength(1) ; t++ )
						if (ConnectivityMatrix[i,t]==true) TotalConnections++;
				
				Group_size = new int[Param.networkParam.GroupSize.Length];
				for (int i = 0 ; i < Param.networkParam.GroupSize.Length ; i++ ) {
					Group_size [i] = Param.networkParam.GroupSize[i];
				}
			}
			
			//-----------------------------------------------------------------------------------------
			// creating Group Connection Matix with number of connection from each group
			int[] hubs = new int[Math.Max(Group_size.Length/5+1,1)];
			Param.rndA.select(0,Group_size.Length,ref hubs, ref Param);
			// Creat the reset of the net group
			int[] resetOfTheNet = new int[Group_size.Length-hubs.Length];
			counter=0;
			for (int i= 0 ; i < Group_size.Length ; i++ ){
				int flag =0;
				for (int t = 0 ; t < hubs.Length ; t++ )
					if (i==hubs[t]) flag=1;
				if (flag==0) {resetOfTheNet[counter] = i; counter++;}
			}
			
			int hubsSize=0;
			for (int i = 0 ; i < hubs.Length ; i++ ) hubsSize+=Group_size[hubs[i]];
			
			int resetOfTheNetSize=0;
			for (int i = 0 ; i < resetOfTheNet.Length ; i++ ) resetOfTheNetSize+=Group_size[resetOfTheNet[i]];
			
			int[] hubNeuronNumbers = new int[hubsSize];
			int[] resetOfTheNetNeuronNumbers = new int[resetOfTheNetSize];
			int groupCounter = 0,counterII=0;
			endGroup = Group_size[0]; startGroup=0; counter = 0 ;
			
			for (int x = 0 ; x < NetSize ; x++ ) {
				if ((x == endGroup) && (groupCounter+1<Group_size.Length)) {
					groupCounter++;
					startGroup = endGroup;
					endGroup += Group_size[groupCounter];
				}
				int flag=0;
				for (int i = 0 ; i < hubs.Length ; i++ )
					if (hubs[i]==groupCounter) {flag=1;break;}
				if (flag ==1 ) {hubNeuronNumbers[counter] = x; counter++;}
				else {resetOfTheNetNeuronNumbers[counterII] = x; counterII++;}
			}
			
			//choose random representative from group resetOftheNet to be conect to representative in group hubGroup
			groupCounter = 0; counter = hubNeuronNumbers.Length; counterII=-1;
			int[] input = new int[hubNeuronNumbers.Length], output = new int[resetOfTheNetNeuronNumbers.Length];
			while(( Param.networkParam.Connections/2 > TotalConnections)&&(groupCounter<resetOfTheNetNeuronNumbers.Length)){
				if(counter==hubNeuronNumbers.Length)
				{
					Param.rndA.select(0,hubNeuronNumbers.Length,ref input, ref Param);
					counter=0;
					counterII++;
				}
				if(counterII==resetOfTheNetNeuronNumbers.Length)
				{
					Param.rndA.select(0,resetOfTheNetNeuronNumbers.Length,ref output, ref Param);
					counterII=0;
					groupCounter++;
				}
				
				
				if (ConnectivityMatrix[resetOfTheNetNeuronNumbers[output[counterII]],hubNeuronNumbers[input[counter]]]==true) {
					counter++;
					continue;
				}
				ConnectivityMatrix[resetOfTheNetNeuronNumbers[output[counterII]],hubNeuronNumbers[input[counter]]]=true;
				TotalConnections++;
				counter++;
			}
			
			
			// counting the connection of the nods
			// find the minimum output in the hubNet
			Matrix_Arithmetic ArrayManimulation = new Matrix_Arithmetic();
			int[] miniInputNeuros = new int[0];
			int[] counting = new int[hubNeuronNumbers.Length];
			int minimumOutput = NetSize;
			for (int neu = 0 ; neu < hubNeuronNumbers.Length ; neu++ ) {
				int neuSumOut = 0;
				for (int i = 0 ; i < NetSize ; i++)
					if (ConnectivityMatrix[hubNeuronNumbers[neu],i]==true) neuSumOut++;
				counting[neu] = neuSumOut;
				if (minimumOutput>counting[neu]) minimumOutput = counting[neu];
			}
			
			//search for the minimon input connection in the all the resetofthenetwork and connect some of the
			//hubNetwork with the minimum out put connection
			for (int min = 0 ; min < Min_InterConnections ; min++) {
				for (int neu = 0 ; neu < NetSize ; neu++) {
					int neuSumIn=0;
					for (int i = 0 ; i < resetOfTheNetNeuronNumbers.Length ; i++ )
						if (ConnectivityMatrix[neu,resetOfTheNetNeuronNumbers[i]]==true) neuSumIn++;
					if (neuSumIn==min) ArrayManimulation.AddCell(ref miniInputNeuros,neu);
				}
			}
			
			
			int max = ArrayManimulation.Max(counting);
			int[] hubNeuronNumbersMinimumOutput = new int[0];
			for( ;  hubNeuronNumbersMinimumOutput.Length < (miniInputNeuros.Length)/2 ; minimumOutput++){
				if ((hubNeuronNumbersMinimumOutput.Length==counting.Length)&&(minimumOutput > max)) break;
				for (int neu = 0 ; neu < hubNeuronNumbers.Length ; neu++ ) {
					if (minimumOutput==counting[neu]) ArrayManimulation.AddCell(ref hubNeuronNumbersMinimumOutput ,neu);
				}
			}
			
			// connect the hub group to thous neurons
			groupCounter = 0; counter = hubNeuronNumbersMinimumOutput.Length; counterII=-1;
			output = new int[hubNeuronNumbersMinimumOutput.Length]; input = new int[miniInputNeuros.Length];
			while((Param.networkParam.Connections > TotalConnections)&&(groupCounter<miniInputNeuros.Length)){
				if(counter==hubNeuronNumbersMinimumOutput.Length)
				{
					Param.rndA.select(0,hubNeuronNumbersMinimumOutput.Length,ref output, ref Param);
					counter=0;
					counterII++;
				}
				if(counterII==miniInputNeuros.Length)
				{
					Param.rndA.select(0,miniInputNeuros.Length,ref input, ref Param);
					counterII=0;
					groupCounter++;
				}
				
				
				if (ConnectivityMatrix[hubNeuronNumbersMinimumOutput[output[counter]],miniInputNeuros[input[counterII]]]==true) {
					counter++;
					continue;
				}
				ConnectivityMatrix[hubNeuronNumbersMinimumOutput[output[counter]],miniInputNeuros[input[counterII]]]=true;
				TotalConnections++;
				counter++;
			}
			
			// Create Random Connection between the Hubs Groups
			counter = 0 ;
			counterII = hubsSize*hubsSize*hubsSize;
			for (int i = 0 ; i <hubs.Length ; i++ ) {
				int shift=0;
				for (int t = 0 ; t < hubs[i] ; t++ ) { shift+=Group_size[t];	}
				int candidateGroup=i;
				do{
					if(hubs.Length<=1) break;
					candidateGroup = Param.rnd.Next(0,hubs.Length);
				}while(candidateGroup==i);
				int targrtShift=0;
				for (int t = 0 ; t < candidateGroup ; t++ ) { targrtShift+=Group_size[t];	}
				int source=0,target=0;
				for (counter = 0; counter <counterII; counter++) {
					source = Param.rnd.Next(0,Group_size[resetOfTheNet[i]]) + shift;
					target = Param.rnd.Next(0,Group_size[resetOfTheNet[candidateGroup]]) + targrtShift;
					if (ConnectivityMatrix[source,target]==false) break;
				}
				ConnectivityMatrix[source,target]=true;
				TotalConnections++;
			}
			// Create Random Connection between the Group of reset of the net
			counter =0;
			counterII = resetOfTheNetSize * resetOfTheNetSize* resetOfTheNetSize;
			for (int i = 0 ; i <resetOfTheNet.Length ; i++ ) {
				int shiftSource=0;
				for (int t = 0 ; t < resetOfTheNet[i] ; t++ ) { shiftSource+=Group_size[t];	}
				int candidateGroup;
				do{	candidateGroup = Param.rnd.Next(0,resetOfTheNet.Length);
				}while(candidateGroup==i);
				int targrtShift=0;
				for (int t = 0 ; t < candidateGroup ; t++ ) { targrtShift+=Group_size[t];	}
				int source=0,target=0;
				for (counter = 0; counter <counterII; counter++) {
					source = Param.rnd.Next(0,Group_size[resetOfTheNet[i]]) + shiftSource;
					target = Param.rnd.Next(0,Group_size[resetOfTheNet[candidateGroup]]) + targrtShift;
					if (ConnectivityMatrix[source,target]==false) break;
				}
				ConnectivityMatrix[source,target]=true;
				TotalConnections++;
			}
			
			
			
		}//----------------------------------------------------------------------------------------------------------------------


		public void Maass(ref bool[,] ConnectivityMatrix,int NetSize,int[] posiORneg, ref globalParam Param)
		{
			//----------------- Maass et al Connection Method-------------------------
			int TotalConnection =0;
			while (TotalConnection < Param.networkParam.Connections) {
				
				for (int xSource = 0 ; xSource < Param.networkParam.Maass_column[0] ; xSource++) {
					for (int ySource = 0 ; ySource < Param.networkParam.Maass_column[1] ; ySource++ ) {
						for (int zSource = 0 ; zSource < Param.networkParam.Maass_column[2] ; zSource++ ) {
							
							int source = (xSource + (ySource*Param.networkParam.Maass_column[0]) + (zSource*Param.networkParam.Maass_column[0]*Param.networkParam.Maass_column[1]) );
							int[] SourcePlace = new int[]{xSource,ySource,zSource};
							
							//------------Destination
							for (int xDest = 0 ; xDest < Param.networkParam.Maass_column[0] ; xDest++ ) {
								for (int yDest = 0 ; yDest <Param.networkParam.Maass_column[1] ; yDest++ ) {
									for (int zDest = 0 ; zDest < Param.networkParam.Maass_column[2] ; zDest++ ) {
										
										int dest = (xDest + (yDest*Param.networkParam.Maass_column[0]) + (zDest*Param.networkParam.Maass_column[0]*Param.networkParam.Maass_column[1]) );
										double C = 0;
										//Negative(2) OR Positive(1)
										if ((posiORneg[source]==1)&&(posiORneg[dest]==1)) C=0.3;
										if ((posiORneg[source]==1)&&(posiORneg[dest]==2)) C=0.2;
										if ((posiORneg[source]==2)&&(posiORneg[dest]==1)) C=0.4;
										if ((posiORneg[source]==2)&&(posiORneg[dest]==2)) C=0.1;
										
										double probability = this.EuclideanDistance(SourcePlace,new int[]{xDest,yDest,zDest});
										probability = C * Math.Exp(-Math.Pow((probability/Param.networkParam.Maass_Lambda),2));
										if (probability>=1)
											Console.WriteLine("Error: wird");
										if  (TotalConnection >= Param.networkParam.Connections) break;
										if (Param.rnd.NextDouble()<=probability)
										{
											if (ConnectivityMatrix[source,dest]==true) continue;
											ConnectivityMatrix[source,dest] = true;
											TotalConnection++;
										}
									}
								}
							}//------------Destination
							
							
						}
					}
				}
			}
		}//----------------------------------------------------------------------------------------------------------------------

		public double EuclideanDistance(int[] source,int[] destination){
			
			if (source.Length!=destination.Length) return 0;
			double distancce=0;
			for (int dimension = 0 ; dimension < source.Length ; dimension ++ ) {
				distancce+=Math.Pow((source[dimension]-destination[dimension]),2);
			}
			
			return Math.Sqrt(distancce);

		}//----------------------------------------------------------------------------------------------------------------------

		public void GroupsPowerLaw(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			
			// stat creating Groups
			int[] Group_size = new int[0]{};
			int Group_size_Max=Param.networkParam.Group_size_Max+1,
			Group_size_Min=Param.networkParam.Group_size_Min,
			Group_sum=0;
			
			Matrix_Arithmetic Utils = new Matrix_Arithmetic();
			do{
				int candidateGroup = Param.rnd.Next(Group_size_Min,Group_size_Max);
				if ((Group_sum+Group_size_Max+1)>=ConnectivityMatrix.GetLength(0))
					candidateGroup = (ConnectivityMatrix.GetLength(0) - Group_sum);
				Group_sum += candidateGroup;
				Utils.AddCell(ref Group_size,candidateGroup);
			}while (Group_sum<ConnectivityMatrix.GetLength(0));
			//-----------------------------------------------------------------------------------------
			Group_sum=Group_size.Length;
			int Group_counter=0;
			while(true){
				if (Group_sum/2==0) break;
				Group_sum=Group_sum/2;
				Group_counter++;
			}
			RandomPowerLaw[][] pwrlaw = new RandomPowerLaw[Group_counter][];
			int[][] shift = new int[Group_counter][];
			int[][] counter = new int[Group_counter][];
			
			for (int i = 0 ; i < Group_counter ; i++) {
				if (i==0) {
					pwrlaw[0] = new RandomPowerLaw[1];
					pwrlaw[0][0] = new RandomPowerLaw(NetSize , Param.networkParam.IncreaseSelectedChanceBy,ref Param);
					shift[0] = new int[1];
					shift[0][0] = 0;
					counter[0] = new int[1];
					counter[0][0] = 0;
				}else{
					pwrlaw[i] = new RandomPowerLaw[(int) Math.Pow(2,i)];
					shift[i] = new int[pwrlaw[i].Length];
					counter[i] = new int[pwrlaw[i].Length];
					
					int group_Start=0,group_end=0,groups=Group_size.Length/pwrlaw[i].Length,groupCounter=0;
					for (int t = 0 ; t < pwrlaw[i].Length ; t++ ) {
						for ( int g = groupCounter ; g < groups+groupCounter ; g++ ) { group_end += Group_size[g];	}
						groupCounter+=groups;
						pwrlaw[i][t] = new RandomPowerLaw( (group_end-group_Start), Param.networkParam.IncreaseSelectedChanceBy,ref Param);
						shift[i][t] = group_Start;
						group_Start=group_end;
					}
					if (groupCounter<Group_size.Length) {
						int sum=0;
						for (int t = 0 ; t < pwrlaw[i].Length-1 ; t++ ) {  sum += pwrlaw[i][t].nodes; }
						pwrlaw[i][pwrlaw[i].Length-1] = new RandomPowerLaw( NetSize - sum , Param.networkParam.IncreaseSelectedChanceBy,ref Param);
					}
				}
			}

			int TotalConnections=0;
			while( TotalConnections < Param.networkParam.Connections){
				for (int level = 0 ; level < pwrlaw.Length ; level++ ) {
					for (int group = 0 ; group < pwrlaw[level].Length ; group++ ) {
						for (int nu = 0 ; nu < (int)  Math.Round(pwrlaw[level][group].nodes * Param.networkParam.GeneralConnectivity) ; nu++ ) {
							int flag=0,tempCounter=0,candidate=0;
							pwrlaw[level][group].NewNode(ref Param);
							int maxConection =(int)  Math.Round(pwrlaw[level][group].nodes * Param.networkParam.GeneralConnectivity);
							int x = Param.rnd.Next(0,pwrlaw[level][group].nodes);
							int LorR = (Param.rnd.NextDouble()>0.5)? 1:0;
							while ((flag==0)&&(tempCounter<maxConection)) {
								flag=1;
								if (pwrlaw[level][group].Next(ref candidate,ref Param)==false) break;
								tempCounter++;
								if (candidate==x) {
									pwrlaw[level][group].NotGood();
									flag=0;
								}else if ((ConnectivityMatrix[(x+shift[level][group]),(candidate+shift[level][group])]==true)&&(LorR==0)) {
									pwrlaw[level][group].NotGood();
									flag=0;
								}else if ((ConnectivityMatrix[(candidate+shift[level][group]),(x+shift[level][group])]==true)&&(LorR==1)) {
									pwrlaw[level][group].NotGood();
									flag=0;
								}
							}
							if (tempCounter<maxConection){
								if (LorR==0)
									ConnectivityMatrix[(x+shift[level][group]),(candidate+shift[level][group])]=true;
								else
									ConnectivityMatrix[(candidate+shift[level][group]),(x+shift[level][group])]=true;
								TotalConnections++;
								pwrlaw[level][group].Good();
							}
						}
					}
				}
			}
		}//----------------------------------------------------------------------------------------------------------------------


		public void UncorrelatedScale_Free(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			// this function copied from Reservoir Computing Toolbox from RCToolbox-2.1.zip at http://snn.elis.ugent.be/rctoolbox
			// eric.antonelo [AT] UGent.be
			int TotalConnections =0;
			if (Param.Groups!=null)
				if (Param.Groups.Length>0){
				for (int i = 0 ;  i < ConnectivityMatrix.GetLength(0) ; i++ )
					for (int t = 0 ;  t < ConnectivityMatrix.GetLength(1) ; t++ )
						if (ConnectivityMatrix[i,t]==true) TotalConnections++;
			}
			int[] ki2 = new int[NetSize];
			ConnectivityMatrix[0,1] = true; TotalConnections++; ki2[0]++;ki2[1]++;
			
			
			
			while (TotalConnections < Param.networkParam.Connections) {
				int ski = 1;
				for (int i = 2 ; i < NetSize ; i++ ) {
					int tski = 0;
					for (int j = 0 ; j < i ;  j++ ) {
						int ki = ki2[j];
//						for (int count = 0 ; count < ConnectivityMatrix.GetLength(0) ; count++ )
//							if ((ConnectivityMatrix[j,count]==true)||(ConnectivityMatrix[count,j]==true))
//								ki++;
						
						if (Param.rnd.NextDouble() < (ki / ski) ){
							if (Param.rnd.NextDouble() > 0.5 ){
								if (ConnectivityMatrix[j,i] == true) continue;
								ConnectivityMatrix[j,i] = true ;
								ki2[j]++;
								ki2[i]++;
							}
							else{
								if (ConnectivityMatrix[i,j] == true) continue;
								ConnectivityMatrix[i,j] = true ;
								ki2[j]++;
								ki2[i]++;
							}
							TotalConnections++;
							tski++;
						}
					}
					ski += tski;
				}
			}
		}//----------------------------------------------------------------------------------------------------------------------

		public void UncorrelatedScale_Free_Powerlaw(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			// this function copied from Reservoir Computing Toolbox from RCToolbox-2.1.zip at http://snn.elis.ugent.be/rctoolbox
			// eric.antonelo [AT] UGent.be
		}//----------------------------------------------------------------------------------------------------------------------


		public void PowerLawSelections(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int TotalConnections=0;
			double decay = 0.03, diff=1;
			int[] connections = new int[NetSize];
			connections[0]= NetSize - 1;
			for (int i = 1 ; i < NetSize ; i++ ) {
				diff += (diff*decay);
				connections[i] =connections[i-1] - (int) Math.Round(diff);
				if (connections[i]<=2) connections[i]=2;
//				Console.WriteLine(connections);
			}
			RandomPowerLaw pwrlaw = new RandomPowerLaw(NetSize , Param.networkParam.IncreaseSelectedChanceBy,connections,ref Param);
			
			for ( int x=0 ; x < NetSize ; x++ ) {
				pwrlaw.NewNode(ref Param);
				for (int times=0 ; times < connections[NetSize-1-x] ; times++) {
					int candidate = 0;
					pwrlaw.Next(ref candidate,ref Param);
					while ((ConnectivityMatrix[x,candidate]==true) ||
					       (x==candidate)){
						pwrlaw.NotGood();
						pwrlaw.Next(ref candidate,ref Param);
					}
					pwrlaw.Good();
					ConnectivityMatrix[x,candidate]=true;
					TotalConnections++;
				}
			}
		}//----------------------------------------------------------------------------------------------------------------------

		public void TwoWayPowerLaw(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			
			int TotalConnections=0;
			// stat the minimul Group connections
			int[] Group_size = new int[0]{};
			int Group_size_Max=Param.networkParam.Group_size_Max+1,
			Group_size_Min=Param.networkParam.Group_size_Min,
			Group_sum=0,
			Min_InterConnections = Param.networkParam.Group_interconnections[0],
			Max_InterConnections = 0,
			endGroup,startGroup,counter;
			
			if (Param.networkParam.GroupSize==null) {
				Matrix_Arithmetic Utils = new Matrix_Arithmetic();
				do{
					int candidateGroup = Param.rnd.Next(Group_size_Min,Group_size_Max);
					if ((Group_sum+Group_size_Max+1)>=ConnectivityMatrix.GetLength(0))
						candidateGroup = (ConnectivityMatrix.GetLength(0) - Group_sum);
					Group_sum += candidateGroup;
					Utils.AddCell(ref Group_size,candidateGroup);
				}while (Group_sum<ConnectivityMatrix.GetLength(0));
				
				endGroup = Group_size[0]-1; startGroup=0 ; counter = 0 ;
				for (int x = 0 ; x < ConnectivityMatrix.GetLength(0) ; x++ ) {
					if ((x == endGroup) && (counter+1<Group_size.Length)) {
						counter++;
						startGroup = endGroup;
						endGroup += Group_size[counter];
					}
					int[] candidates;
					if (Param.networkParam.Group_interconnections.Length==2){
						if (Param.networkParam.Group_interconnections[1]>=Group_size[counter])
							Max_InterConnections = Group_size[counter]-1;
						else
							Max_InterConnections = Param.networkParam.Group_interconnections[1];
					}else
						Max_InterConnections = Group_size[counter]-1;
					if (Max_InterConnections < Min_InterConnections)
						candidates = new int[Param.rnd.Next(Max_InterConnections,Min_InterConnections+1)];
					else
						candidates = new int[Param.rnd.Next(Min_InterConnections,Max_InterConnections+1)];
					Param.rndA.dselect(startGroup,endGroup,x,ref candidates, ref Param);
					for (int i = 0 ; i < candidates.Length ; i++ ) {
						ConnectivityMatrix[x,candidates[i]]=true;
						TotalConnections++;
					}
				}
			}else{
				for (int i = 0 ;  i < ConnectivityMatrix.GetLength(0) ; i++ )
					for (int t = 0 ;  t < ConnectivityMatrix.GetLength(1) ; t++ )
						if (ConnectivityMatrix[i,t]==true) TotalConnections++;
				
				Group_size = new int[Param.networkParam.GroupSize.Length];
				for (int i = 0 ; i < Param.networkParam.GroupSize.Length ; i++ ) {
					Group_size [i] = Param.networkParam.GroupSize[i];
				}
			}
			
			//-----------------------------------------------------------------------------------------
			int[] connections = new int[NetSize];
			connections[NetSize-1]  = NetSize-1;
			double decay = 0.03, diff=1;
			int tempConnection = TotalConnections+connections[NetSize-1],
			minimumConnection = 3;
			for (int i = NetSize-2 ; i >= 0 ; i-- ) {
				diff += (diff*decay);
				connections[i] = connections[i+1] - (int) Math.Round(diff);
				tempConnection+=connections[i];
				if (connections[i]<=minimumConnection)
					if (tempConnection>=Param.networkParam.Connections)
						connections[i]=1;
					else
						connections[i]=minimumConnection;
			}
			
			for ( int x=0 ; x < NetSize ; x++ ) {
				int y = NetSize-1;
				for (int times=0 ; times < connections[NetSize-x-1] ; times++) {
					if ((x!=y)&&(ConnectivityMatrix[y,x]==false)&&(Param.rnd.NextDouble()>=0)){
						ConnectivityMatrix[y,x]=true;
						TotalConnections++;
					}
					y--;
				}
			}
			
			//-----------------------------------------------------------------------------------------
//			PrintNetwork(ref ConnectivityMatrix);
		}//----------------------------------------------------------------------------------------------------------------------

		public void PowerLawX2Y2X(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int TotalConnections=0;
			double decay = 0.03, diff=1,sum=0;
			int[] connections = new int[NetSize];
			connections[0]= NetSize - 1;
			sum+=connections[0];
			for (int i = 1 ; i < NetSize ; i++ ) {
				diff += (diff*decay);
				connections[i] =connections[i-1] - (int) Math.Round(diff);
				if (connections[i]<=2) connections[i]=2;
				sum+=connections[i];
			}
			RandomPowerLaw pwrlaw = new RandomPowerLaw(NetSize , Param.networkParam.IncreaseSelectedChanceBy,connections,ref Param);
			
			for ( int x=0 ; x < NetSize ; x++ ) {
				pwrlaw.NewNode(ref Param);
				for (int times=0 ; times < connections[x] ; times++) {
					int candidate = 0,count=0;
					pwrlaw.Next(ref candidate,ref Param);
					while ((ConnectivityMatrix[x,candidate]==true) ||
					       (x==candidate)){
						count++;
						if (count>=NetSize)
							Console.WriteLine("Error!!!!");
						pwrlaw.NotGood();
						pwrlaw.Next(ref candidate,ref Param);
					}
					pwrlaw.Good();
					ConnectivityMatrix[x,candidate]=true;
					TotalConnections++;
				}
			}
		}//----------------------------------------------------------------------------------------------------------------------

		public void TwoWayLinearDescent(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int TotalConnections = 0;
			int parts = 1 ;
			int numOfNodes = (NetSize/parts);
			// round the parts to avoid half groups
			parts = NetSize/numOfNodes;
			
			// conect groups of neurons internally
			for (int part = 0 ; part < parts ; part++ ) {
				for (int x= part*numOfNodes ; x < (part+1)*numOfNodes ; x++ ) {
					int tempX = x;
					if (TotalConnections>Param.networkParam.Connections) tempX = Param.rnd.Next(Param.networkParam.Group_size_Min,Param.networkParam.Group_size_Max);
					for (int i=tempX; i > part*numOfNodes ; i--) {
						if (TotalConnections<Param.networkParam.Connections) ConnectivityMatrix[x,i]=true;
						else ConnectivityMatrix[Param.rnd.Next(x,(part+1)*numOfNodes),Param.rnd.Next(x,(part+1)*numOfNodes)]=true;
						TotalConnections++;
					}
					
				}
			}
			// conect the Zero neurons internally to his group
			for (int part = 0 ; part < parts ; part++ ) {
				int x= part*numOfNodes;
				ConnectivityMatrix[x,Param.rnd.Next(x+1,((part+1)*numOfNodes))]=true;
				TotalConnections++;
			}
			
		}//----------------------------------------------------------------------------------------------------------------------

		public void DoubleReversePowerLawMethodII(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int TotalConnections = 0;
			PowerLaw pwrlaw = new PowerLaw(Param.networkParam.IncreaseSelectedChanceBy,NetSize,Param.networkParam.Connections,ref Param);
			while (TotalConnections < Param.networkParam.Connections) {
				int input=0,output=0;
				pwrlaw.Next(ref input,ref output);
				while ((ConnectivityMatrix[output,input]==true) || (input==output))
				{
					input=0;output=0;
					if (pwrlaw.NotGood()==false) Console.WriteLine("Error!");
					if (pwrlaw.Next(ref output,ref input)==false) Console.WriteLine("Error!!");
				}
				pwrlaw.Good(ref Param);
				ConnectivityMatrix[output,input]=true;
				TotalConnections++;
			}
			
		}//----------------------------------------------------------------------------------------------------------------------

		public void CombibeFFwithDoubleReversePowerLaw(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int GroupsCountingSize=0,groupSize=0;
			Param.networkParam.GroupSize = new int[0];
			Matrix_Arithmetic mat = new Matrix_Arithmetic();
			
			do{
				
				if ((GroupsCountingSize + Param.networkParam.Group_size_Max) >= Param.networkParam.Number_Of_Neurons )
					groupSize =  Param.networkParam.Number_Of_Neurons - GroupsCountingSize;
				else
					groupSize = Param.rnd.Next(Param.networkParam.Group_size_Min,Param.networkParam.Group_size_Max);
				
				
				mat.AddCell(ref Param.networkParam.GroupSize,groupSize);
				
				bool[,] tempConectivityMatrix = new bool[groupSize,groupSize];
				int[] tempPosiORneg = new int[groupSize];
				globalParam cloneParam = Param.copy();
				cloneParam.networkParam.Connections = Convert.ToInt32(Math.Round((groupSize * groupSize) * Param.networkParam.GeneralConnectivity));
				
				for (int i = 0; i < groupSize; i++) tempPosiORneg[i] = posiORneg[GroupsCountingSize+i];
				
				DoubleReversePowerLawMethodII(ref tempConectivityMatrix,groupSize,tempPosiORneg,ref cloneParam);
				
				for (int i = 0; i < groupSize; i++) {
					posiORneg[GroupsCountingSize+i] = tempPosiORneg[i] ;
					for (int t = 0; t < groupSize; t++) {
						ConnectivityMatrix[GroupsCountingSize+i,GroupsCountingSize+t] = tempConectivityMatrix[i,t];
					}
				}
				GroupsCountingSize += groupSize;
			}while(GroupsCountingSize < Param.networkParam.Number_Of_Neurons );
			
			FeedForwardWithHubs(ref ConnectivityMatrix,NetSize, posiORneg, ref Param);
		}//----------------------------------------------------------------------------------------------------------------------


		public void CombibeFFwithTwoWayPowerLaw(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int GroupsCountingSize=0,groupSize=0;
			Param.networkParam.GroupSize = new int[0];
			Matrix_Arithmetic mat = new Matrix_Arithmetic();
			
			do{
				
				if ((GroupsCountingSize + Param.networkParam.Group_size_Max) >= Param.networkParam.Number_Of_Neurons )
					groupSize =  Param.networkParam.Number_Of_Neurons - GroupsCountingSize;
				else
					groupSize = Param.rnd.Next(Param.networkParam.Group_size_Min,Param.networkParam.Group_size_Max);
				
				
				mat.AddCell(ref Param.networkParam.GroupSize,groupSize);
				
				bool[,] tempConectivityMatrix = new bool[groupSize,groupSize];
				int[] tempPosiORneg = new int[groupSize];
				globalParam cloneParam = Param.copy();
				cloneParam.networkParam.Connections = Convert.ToInt32(Math.Round((groupSize * groupSize) * Param.networkParam.GeneralConnectivity));
				
				for (int i = 0; i < groupSize; i++) tempPosiORneg[i] = posiORneg[GroupsCountingSize+i];
				
				TwoWayPowerLaw(ref tempConectivityMatrix,groupSize,tempPosiORneg,ref cloneParam);
				
				for (int i = 0; i < groupSize; i++) {
					posiORneg[GroupsCountingSize+i] = tempPosiORneg[i] ;
					for (int t = 0; t < groupSize; t++) {
						ConnectivityMatrix[GroupsCountingSize+i,GroupsCountingSize+t] = tempConectivityMatrix[i,t];
					}
				}
				GroupsCountingSize += groupSize;
			}while(GroupsCountingSize < Param.networkParam.Number_Of_Neurons );
			
			FeedForwardWithHubs(ref ConnectivityMatrix,NetSize, posiORneg, ref Param);
		}//----------------------------------------------------------------------------------------------------------------------

		public void CombibeFFwithUncorrelatedScale_Free(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int GroupsCountingSize=0,groupSize=0;
			Param.networkParam.GroupSize = new int[0];
			Matrix_Arithmetic mat = new Matrix_Arithmetic();
			
			do{
				
				if ((GroupsCountingSize + Param.networkParam.Group_size_Max) >= Param.networkParam.Number_Of_Neurons )
					groupSize =  Param.networkParam.Number_Of_Neurons - GroupsCountingSize;
				else
					groupSize = Param.rnd.Next(Param.networkParam.Group_size_Min,Param.networkParam.Group_size_Max);
				
				
				mat.AddCell(ref Param.networkParam.GroupSize,groupSize);
				
				bool[,] tempConectivityMatrix = new bool[groupSize,groupSize];
				int[] tempPosiORneg = new int[groupSize];
				globalParam cloneParam = Param.copy();
				cloneParam.networkParam.Connections = Convert.ToInt32(Math.Round((groupSize * groupSize) * Param.networkParam.GeneralConnectivity));
				
				for (int i = 0; i < groupSize; i++) tempPosiORneg[i] = posiORneg[GroupsCountingSize+i];
				
				UncorrelatedScale_Free(ref tempConectivityMatrix,groupSize,tempPosiORneg,ref cloneParam);
				
				for (int i = 0; i < groupSize; i++) {
					posiORneg[GroupsCountingSize+i] = tempPosiORneg[i] ;
					for (int t = 0; t < groupSize; t++) {
						ConnectivityMatrix[GroupsCountingSize+i,GroupsCountingSize+t] = tempConectivityMatrix[i,t];
					}
				}
				GroupsCountingSize += groupSize;
			}while(GroupsCountingSize < Param.networkParam.Number_Of_Neurons );
			
			FeedForwardWithHubs(ref ConnectivityMatrix,NetSize, posiORneg, ref Param);
		}//----------------------------------------------------------------------------------------------------------------------

		
		public void CombibeFFwithRandom(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int GroupsCountingSize=0,groupSize=0;
			Param.networkParam.GroupSize = new int[0];
			Matrix_Arithmetic mat = new Matrix_Arithmetic();
			
			do{
				
				if ((GroupsCountingSize + Param.networkParam.Group_size_Max) >= Param.networkParam.Number_Of_Neurons )
					groupSize =  Param.networkParam.Number_Of_Neurons - GroupsCountingSize;
				else
					groupSize = Param.rnd.Next(Param.networkParam.Group_size_Min,Param.networkParam.Group_size_Max);
				
				
				mat.AddCell(ref Param.networkParam.GroupSize,groupSize);
				
				bool[,] tempConectivityMatrix = new bool[groupSize,groupSize];
				int[] tempPosiORneg = new int[groupSize];
				globalParam cloneParam = Param.copy();
				cloneParam.networkParam.Connections = Convert.ToInt32(Math.Round((groupSize * groupSize) * Param.networkParam.GeneralConnectivity));
				
				for (int i = 0; i < groupSize; i++) tempPosiORneg[i] = posiORneg[GroupsCountingSize+i];
				
				RandomConnection(ref tempConectivityMatrix,groupSize,tempPosiORneg,ref cloneParam);
				
				for (int i = 0; i < groupSize; i++) {
					posiORneg[GroupsCountingSize+i] = tempPosiORneg[i] ;
					for (int t = 0; t < groupSize; t++) {
						ConnectivityMatrix[GroupsCountingSize+i,GroupsCountingSize+t] = tempConectivityMatrix[i,t];
					}
				}
				GroupsCountingSize += groupSize;
			}while(GroupsCountingSize < Param.networkParam.Number_Of_Neurons );
			
			FeedForwardWithHubs(ref ConnectivityMatrix,NetSize, posiORneg, ref Param);
		}//----------------------------------------------------------------------------------------------------------------------


		public void Mesh(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int size,TotalConnections=0;
			size = Convert.ToInt32(Math.Sqrt(NetSize));
			
			for (int round = 0 , end = size; round < NetSize; round+=size, end+=size) {
				for (int x = round ; x < end ; x++) {
					int temp=NetSize-1;
					if (x<temp) {
						this.ConnectivityMatrix[x,x+1] = true;
						this.ConnectivityMatrix[x+1,x] = true;
						TotalConnections+=2;
					}
					if(round+size<temp){
						this.ConnectivityMatrix[x,x+size] = true;
						this.ConnectivityMatrix[x+size,x] = true;
						TotalConnections+=2;
					}
				}
			}
		}//----------------------------------------------------------------------------------------------------------------------
		
		public void Click(ref bool[,] ConnectivityMatrix,int NetSize, int[] posiORneg, ref globalParam Param)
		{
			int TotalConnections=0;
			for (int x = 0; x < NetSize; x++) {
				for (int y = 0; y < NetSize; y++) {
					if (x==y)continue;
					this.ConnectivityMatrix[x,y] = true;
					TotalConnections++;
				}
			}
			
		}//----------------------------------------------------------------------------------------------------------------------

		

	} // End Class
}
