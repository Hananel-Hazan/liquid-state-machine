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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Globalization;

namespace Neurons
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	[Serializable()]
	public class Neuron
	{
		//----------------------------------------------------------------
		//                    init ver
		//----------------------------------------------------------------
		public NeuronArch.Unit Nunit;
		public double ExternalIntput;
		public int posiORneg;
		public int Input_OR_Output;
		public int Neuron_propotional_weight_Update,totalPass;
		double internalInput;
		
		public int Number_Fireing_in_Second;
		bool[] Fireing_in_Second;
		double minNegWeights,minPosWeights,	maxNegWeights,maxPosWeights;
		
		public Neuron[] OutputNeuronsList;
		int[] output_weightIndex;

		public double[] Input_weights;
		double[] iniInput_weights;
		int[] Input_delay,Output_delay;
		public Neuron[] InputNeuronsList;
		
		double[] InputQu;
		int numberInQ,Number_Of_Neuron_In_input_List;
		
		public int name;
		public int mode;

		public double RandomElemnt_double;
		public int Slideing_Threshold,STDP;
		
		
		//STDP v.1.  - Start
		double LTPchange,LTDchange,LTDchangeLittle;
		int LTPcounter,LTDcounter;
		int LTPwin,LTDwin;
		Int64 firedBeforeXciycles;
		double[] LTP,LTD,LTDwindows,LTPwindows;
		//STDP v.1.  - End
		
		//----------------------------------------------------------------

		public Neuron(int model, int name,ref globalParam Param)
		{
			// model: 1=LIF , 2=Izhikevich , 3=LIF model 2 , 4=McCulloch_Pitts
			switch (model)
			{
					case 1: Nunit = (NeuronArch.LIFNeuron) new NeuronArch.LIFNeuron(ref Param.neuronParam); 			break;
					case 2: Nunit = (NeuronArch.IzakNeuron) new NeuronArch.IzakNeuron(ref Param.neuronParam); 			break;
					case 3: Nunit = (NeuronArch.LIFmodle2) new NeuronArch.LIFmodle2(ref Param.neuronParam); 			break;
					case 4: Nunit = (NeuronArch.McCulloch_Pitts) new NeuronArch.McCulloch_Pitts(ref Param.neuronParam); break;
			}
			this.OutputNeuronsList = new Neuron[0];
			this.output_weightIndex = new int[0];
			this.iniInput_weights = new double[0];
			this.Input_weights = new Double[0];
			this.iniInput_weights = new Double[0];
			this.InputNeuronsList = new Neuron[0];
			this.name = name;
			this.RandomElemnt_double = 0;
			this.Slideing_Threshold = 0 ; // default
			this.STDP = 0 ;
			
			this.FullReset(ref Param);
			
		}//----------------------------------------------------------------
		
		public void Reset_Weight(int i,ref globalParam Param){
//			if (Param.networkParam.Neuron_propotional_weight_Initialize==0){
//				if (this.posiORneg==2)
//					this.Input_weights[i] =
//						-1*((Param.rndA.NextDouble(ref Param,Param.networkParam.LSM_Min_Init_Weight_NegN,Param.networkParam.LSM_Max_Init_Weight_NegN)));
//				else
//					this.Input_weights[i] =
//						(Param.rndA.NextDouble(ref Param,Param.networkParam.LSM_Min_Init_Weight_PosN,Param.networkParam.LSM_Max_Init_Weight_PosN));
//
//			}else if (Param.networkParam.Neuron_propotional_weight_Initialize==1){
//				double part = 1.0/(this.iniInput_weights.Length);
//				if (this.posiORneg==2)
//					this.Input_weights[i] = -1 * part;
//				else
//					this.Input_weights[i] = part;
//			}
			
			this.Input_weights[i]=this.iniInput_weights[i];
			
		}//----------------------------------------------------------------
		
		public void FullReset(ref globalParam Param)
		{
			for (int i = 0; i <  this.iniInput_weights.Length ; i++){
				if (Param.networkParam.Neuron_propotional_weight_Initialize==0){
					if (this.posiORneg==2)
						this.iniInput_weights[i] =
							-1*((Param.rndA.NextDouble(ref Param,Param.networkParam.LSM_Min_Init_Weight_NegN,Param.networkParam.LSM_Max_Init_Weight_NegN)));
					else
						this.iniInput_weights[i] =
							(Param.rndA.NextDouble(ref Param,Param.networkParam.LSM_Min_Init_Weight_PosN,Param.networkParam.LSM_Max_Init_Weight_PosN));
					
				}else if (Param.networkParam.Neuron_propotional_weight_Initialize==1){
					double part = 1.0/(this.iniInput_weights.Length);
					if (this.posiORneg==2)
						this.iniInput_weights[i] = -1 * part;
					else
						this.iniInput_weights[i] = part;
				}
			}
			
			
			this.Input_weights = new double[this.Input_weights.Length];
			for (int i = 0; i <  this.Input_weights.Length ; i++)
				Reset_Weight(i,ref Param);
			
			if (Param.networkParam.Neuron_propotional_weight_Initialize==0){
				minPosWeights = 0.000001;
				minNegWeights = -0.00001;
				maxPosWeights = 1;
				maxNegWeights = -1;
			}else if (Param.networkParam.Neuron_propotional_weight_Initialize==1){
				double temp = 1.0/(this.iniInput_weights.Length);
				minPosWeights = temp;
				minNegWeights = -1*temp;
				maxPosWeights = temp;
				maxNegWeights = -1*temp;
			}
			
			if (Param.neuronParam.Random_Threshold_On_FullRest==1)
//		23/03	Nunit.initTherashold = Param.rndA.NextDouble(ref Param,0, Param.neuronParam.Neuron_Threshold);
				Nunit.initTherashold = Param.rndA.NextDouble(ref Param,Nunit.initV+5, Param.neuronParam.Neuron_Threshold-5);
			else
				Nunit.initTherashold = Param.neuronParam.Neuron_Threshold;
			
			RandomElemnt_double = Param.rnd.NextDouble()*Param.neuronParam.Random_Factor_Sliding_Treshold;
			
			this.reset(ref Param);
		}//----------------------------------------------------------------

		public void reset(ref globalParam Param)
		{
			Number_Of_Neuron_In_input_List = this.InputNeuronsList.Length;
			this.InputQu = new double[this.InputNeuronsList.Length * Param.neuronParam.Neuron_Max_Delay];
			if (Number_Of_Neuron_In_input_List>0)
				totalPass =	this.InputQu.Length/this.Number_Of_Neuron_In_input_List;
			this.numberInQ = 0;
			
			this.internalInput = 0;
			this.ExternalIntput = 0;
			this.mode = 1;
			
			this.Input_weights = (double[]) this.iniInput_weights.Clone();
			
			this.Fireing_in_Second = new bool[Param.detector.LSM_1sec_interval];
			for (int i = 0; i < Param.detector.LSM_1sec_interval ; i++)
				this.Fireing_in_Second[i] = false;
			this.Number_Fireing_in_Second = 0;
			
			Nunit.reset(ref Param.neuronParam);
			if (Param.neuronParam.Randomize_initilaization==1)
				Nunit.V = Param.rndA.NextDouble(ref Param, Param.neuronParam.initV, Param.neuronParam.Int_Neuron_Spike);
			
			//STDP v.1.  - Start
			LTPchange = Param.neuronParam.STDPMaxChange[0];
			LTDchange = Param.neuronParam.STDPMaxChange[1];
			LTDchangeLittle = LTDchange/10;
			LTPwin = Param.neuronParam.STDPwindow[0];
			LTDwin = Param.neuronParam.STDPwindow[1];
			LTPcounter = 0;
			LTDcounter = 0;
			firedBeforeXciycles = 0;
			LTP = new double[this.Input_weights.Length];
			LTD = new double[this.Input_weights.Length];
			LTDwindows = new double[LTDwin+1];
			for (int i = LTDwin ; i > -1 ; i--) {
				LTDwindows[i]=1 - (i+0.0)/LTDwin;
			}
			LTPwindows = new double[LTPwin];
			for (int i = 0; i < LTPwin; i++) {
				LTPwindows[i]= (LTPwin-(i+0.0))/LTPwin;
			}
			
			//STDP v.1.  - End
			
		}//----------------------------------------------------------------

		public bool step(int runningTime,ref int[] flag, ref globalParam Param)
		{
			switch (this.mode)
			{
				case 1:  // Normal
					break;
					
				case 2: // Noise Generator
					if (Param.test_Param.BinaryInput_OR_RealValuse==0)
						this.ExternalIntput=Param.neuronParam.Ext_Inp_Neuron_Spike;
					else if (Param.test_Param.BinaryInput_OR_RealValuse==1)
						this.ExternalIntput= Param.rndA.NextDouble(ref Param,Param.test_Param.RandomMaxMin_RealValues[1],Param.test_Param.RandomMaxMin_RealValues[0]);
					// and continue Normaly to case 1
					goto case 1;

				case 3: // Dead unit
					this.ExternalIntput = 0;
					internalInput = 0;
					this.Nunit.reset(ref Param.neuronParam);
					break;
			}
			
			
//			Nunit.V+= Param.rnd.NextDouble()>0.5?-1*Param.rnd.NextDouble()*0.1:Param.rnd.NextDouble()*0.1;
			
			double Vout =0;
			this.decInputQ(Param.networkParam.Liquid_Weights_Resistend);
			bool returnV = Nunit.step(ref Vout,internalInput, this.ExternalIntput);
			int clockPlace = runningTime%this.Fireing_in_Second.Length;
			
			if (returnV){
				// Fire to whom neuron is conneced.
				this.OutputToAllConnectedUnits(Param.neuronParam.Int_Neuron_Spike);
				
				if (this.Fireing_in_Second[clockPlace] == false){
					this.Fireing_in_Second[clockPlace] = true;
					this.Number_Fireing_in_Second++;
				}
				firedBeforeXciycles = 0 ;
				
//				// STDP - V.2 - START
				if (STDP == 1 ){
					for (int i = 0; i < InputNeuronsList.Length; i++) {
						if ((InputNeuronsList[i].firedBeforeXciycles > 0) &&
						    (InputNeuronsList[i].firedBeforeXciycles<LTPwin)){
//							LTP[i] += LTPwindows[InputNeuronsList[i].firedBeforeXciycles]/LTPwin;
							increase_weight(i, ((LTPwindows[InputNeuronsList[i].firedBeforeXciycles]/LTPwin)* LTPchange) );
							flag[1]++;  // LTP
						}
					}
				}
				// STDP - V.2 - END
				
				// Sliding Threshold
				if (this.Slideing_Threshold==1){
					
//					// Sliding Threshold v1
//					if (this.Nunit.therashold<=this.Nunit.initV)
//						this.Nunit.therashold += (this.Nunit.initTherashold - this.Nunit.therashold)/2;
//					else
//						this.Nunit.therashold += RandomElemnt_double;
//					flag[2]++;
//					// Sliding Threshold v1 - end
					
					// Sliding Threshold v2
//					if (this.Nunit.therashold>this.Nunit.initTherashold)
					if (this.Number_Fireing_in_Second>Param.neuronParam.Neuron_Slideing_Threshold_Recommended_Firing_Rate_Max){
						this.Nunit.therashold += RandomElemnt_double;
//					else
//						this.Nunit.therashold = this.Nunit.initTherashold;
					flag[2]++;
					}
				}
			}else{
				// NO FIRE
				firedBeforeXciycles++;
				if (this.Fireing_in_Second[clockPlace]){
					this.Fireing_in_Second[clockPlace] = false;
					this.Number_Fireing_in_Second--;
				}
				
				// Sliding Threshold v2
				if (this.Slideing_Threshold==1){
					if (this.Number_Fireing_in_Second<Param.neuronParam.Neuron_Slideing_Threshold_Recommended_Firing_Rate_Min){
					this.Nunit.therashold -= RandomElemnt_double;
					flag[2]++;
					}
				}
				
				
//				// STDP - V.1 - START
//				if ((this.STDP==1)&&(this.Number_Fireing_in_Second==0)){
//					for (int i = 0; i < this.Input_weights.Length ; i++){
//						if (this.InputNeuronsList[i].Number_Fireing_in_Second>0) continue;
//						LTD[i] += LTDchangeLittle;
//					}
//				}
//				// STDP - V.1 - END
				
				
			}
			
			
			if (runningTime>0 && runningTime%Param.detector.LSM_1sec_interval==0){

//				// STDP - V.1 - START
//				if (STDP == 1 ){
//
//					for (int i = 0; i < LTP.Length; i++){
//						if (LTP[i] >0 ){
//							increase_weight(i, (LTP[i] * LTPchange) );
//							LTP[i] = 0;
//							flag[1]++;  // LTP
//						}
//						if (LTD[i] >0 ){
//							decrease_weight(i, (LTD[i] * LTDchange));
//							LTD[i] = 0;
//							flag[0]++;	// LTD
//						}
//						else if (this.InputNeuronsList[i].Number_Fireing_in_Second==0){
//							decrease_weight(i, LTDchangeLittle);
//							flag[0]++;	// LTD
//						}
//					}
//				}
//				// STDP - V.1 - END
				
//				// Sliding Threshold base on frequncy
//				if (this.Slideing_Threshold==1){
//
//					if (this.Number_Fireing_in_Second>Param.neuronParam.Neuron_Firing_Rate_Max){
//						this.Nunit.therashold += RandomElemnt_double;
//						flag[2]++;
//					}
//					else
//						if ((this.Number_Fireing_in_Second==0)&&(this.Nunit.therashold>this.Nunit.initV)){
//						this.Nunit.therashold -= RandomElemnt_double;
//						flag[2]++;
//					}
//				}
				
			}
			
			
			flag[1] += LTPcounter;  // LTP
			LTPcounter = 0;
			flag[0] +=LTDcounter;	// LTD
			LTDcounter = 0;
			
			this.mode = 1;
			this.ExternalIntput = 0;
			return returnV;
		}//----------------------------------------------------------------

		public void Save_State(ref globalParam Param)
		{
			
			this.iniInput_weights = (double[]) this.Input_weights.Clone();
			
//			this.Nunit.initTherashold = this.Nunit.therashold;
//			this.Nunit.init_decayRate = this.Nunit.decayRate;
		}//----------------------------------------------------------------

		public Neuron returnRef()
		{
			return (this);
		}//----------------------------------------------------------------

		public void EnterToInputQ(int place,double Volt)
		{
			// STDP - V.2 - START
			
			if (STDP==1){
				
				if ((this.firedBeforeXciycles>0)&&(this.firedBeforeXciycles<=LTDwin)){
					decrease_weight(place, (LTDwindows[this.firedBeforeXciycles] * LTDchange));
					LTDcounter++;
				}
//				else
					if (Nunit.internal_Refactory){
					decrease_weight(place, (LTDchange));
					LTDcounter++;
				}
				
				
			}
			
//			if ((STDP==1)&&((Nunit.internal_Refactory)||((this.firedBeforeXciycles>0)&&(this.firedBeforeXciycles<=LTDwin)))){
//			//				LTD[place] +=  LTDwindows[this.firedBeforeXciycles];
//				decrease_weight(place, (LTDwindows[this.firedBeforeXciycles] * LTDchange));
//				LTDcounter++;
//			}
			
//			if ((STDP==1)&&(Nunit.internal_Refactory)){
//				decrease_weight(place, (LTDchange));
//				LTDcounter++;
//			}
			
			// STDP - V.2 - END
			
			this.InputQu[place + (this.Input_delay[place] * Number_Of_Neuron_In_input_List)] = Input_weights[place] * Volt;
			numberInQ++;
		}
		
		
		public void OutputToAllConnectedUnits(double volts)
		{
			for (int outputN = 0; outputN < this.OutputNeuronsList.Length; outputN++)
				this.OutputNeuronsList[outputN].EnterToInputQ(this.output_weightIndex[outputN],volts);
			
		}//----------------------------------------------------------------
		

		public void decInputQ(double resistance){
			internalInput = 0;
			if (this.numberInQ>0){
				int tempNumInQ = numberInQ;
				
				int ptr = 0;
				int ptrPast = -1 * (this.Number_Of_Neuron_In_input_List);
				
				for (int pass = 0 ; pass < totalPass ; pass++) {
					for (int Neuron = 0; Neuron < this.Number_Of_Neuron_In_input_List ; Neuron++,ptr++,ptrPast++) {
						if (this.InputQu[ptr]!=0){
							if (pass==0){
								internalInput += this.InputQu[ptr];
								numberInQ--;
							}else
								this.InputQu[ptrPast] =  resistance * this.InputQu[ptr];
							
							tempNumInQ--;
							this.InputQu[ptr]=0;
						}
						if (tempNumInQ==0) break;
					}
					if (tempNumInQ==0) break;
				}
				
			}
			
		}//----------------------------------------------------------------

		public void increase_weight(int place,double by){
			if (by!=0){
				if  (this.Neuron_propotional_weight_Update==1){
					int length = this.Input_weights.Length;
					double reset = by/(length-1);
					
					if (this.posiORneg==1){
//						double temp = this.Input_weights[place] + by;
//						int flag = 0;
//						if(temp < 0.99){
//							this.Input_weights[place] = temp;
//							int w =0;
//							for (; w < length ; w++) {
//								if (w != place){
//									temp = this.Input_weights[w] - reset;
//									if (temp>0.01)
//										this.Input_weights[w] = temp;
//									else{
//										flag = 1;
//										break;
//									}
//								}
//							}
//							if (flag==1){ // full back
//								for (; w>0 ; w--)
//									this.Input_weights[w] =- reset;
//								this.Input_weights[place] =+ by;
//							}
//						}
						
						int w = 0,flag = 0;
						for (; w < length ; w++) {
							if (w==place){
								this.Input_weights[w] += by;
								if (this.Input_weights[w] >= 1){
									flag=1; break;}
							}else{
								this.Input_weights[w] -= reset;
								if (this.Input_weights[w] <= 0){
									flag=1; break; }
							}
						}
						if (flag==1)
							for (; w >= 0 ; w--) {
							if (w==place)
								this.Input_weights[w] -= by;
							else
								this.Input_weights[w] += reset;
						}
						
						
					}else{
//						double temp = this.Input_weights[place] - by;
//						int flag = 0;
//						if(temp > -0.99){
//							this.Input_weights[place] = temp;
//							int w = 0;
//							for (; w < length ; w++) {
//								if (w != place){
//									temp = this.Input_weights[w] + reset;
//									if (temp<-0.01)
//										this.Input_weights[w] = temp;
//								}
//							}
//							if (flag==1){ // full back
//								for (; w>0 ; w--)
//									this.Input_weights[w] -= reset;
//								this.Input_weights[place] += by;
//							}
//						}
						
						int w = 0,flag = 0;
						for (; w < length ; w++) {
							if (w==place){
								this.Input_weights[w] -= by;
								if (this.Input_weights[w] >= 0 ){
									flag=1; break;}
							}else{
								this.Input_weights[w] += reset;
								if (this.Input_weights[w] <= -1){
									flag=1; break;}
							}
						}
						if (flag==1)
							for (; w >= 0 ; w--) {
							if (w==place)
								this.Input_weights[w] += by;
							else
								this.Input_weights[w] -= reset;
						}
						
						
					}
				}else{
					
					if(this.posiORneg==1){
						this.Input_weights[place]+=by;
						if (this.Input_weights[place] > maxPosWeights)
							this.Input_weights[place]-=by;
					}else if(this.posiORneg==2){
						this.Input_weights[place]-=by;
						if (this.Input_weights[place] < minNegWeights)
							this.Input_weights[place]+=by;
					}
					
					
//					if((this.posiORneg==1)&&((this.Input_weights[place]+by)<maxPosWeights))
//						this.Input_weights[place]+=by;
//					else if ((this.posiORneg==2)&&((this.Input_weights[place]-by)>minNegWeights))
//						this.Input_weights[place]-=by;
				}
			}
		}//----------------------------------------------------------------

		public void decrease_weight(int place,double by){
			///////////
			/// need to use:
			/// Neuron_propotional_weight_Update
			/// 
			
			if  (this.Neuron_propotional_weight_Update==1){
				
				int length = this.Input_weights.Length;
				double reset = by/(length-1);
				
				if (this.posiORneg==1){
//					int flag = 0;
//					double temp = this.Input_weights[place] - by;
//					if(temp > 0.01){
//						this.Input_weights[place] = temp;
//						int w = 0;
//						for (; w < length ; w++) {
//							if (w != place){
//								temp = this.Input_weights[w] + reset;
//								if (temp < 0.99)
//									this.Input_weights[w] = temp;
//								else{
//									flag = 1;
//									break;
//								}
//							}
//						}
//						if (flag==1){ // full back
//							for (; w>0 ; w--)
//								this.Input_weights[w] -= reset;
//							this.Input_weights[place] += by;
//						}
//					}
					
					
					int w = 0,flag = 0;
					for (; w < length ; w++) {
						if (w==place){
							this.Input_weights[w] -= by;
							if (this.Input_weights[w] <= 0){
								flag=1; break;}
						}else{
							this.Input_weights[w] += reset;
							if (this.Input_weights[w] >= 1){
								flag=1; break; }
						}
					}
					if (flag==1)
						for (; w >= 0 ; w--) {
						if (w==place)
							this.Input_weights[w] += by;
						else
							this.Input_weights[w] -= reset;
					}
					
					
					
					
				}else{
//					double temp = this.Input_weights[place] + by;
//					int flag = 0;
//					if(temp < -0.01){
//						this.Input_weights[place] = temp;
//						int w = 0;
//						for (; w < length ; w++) {
//							if (w != place){
//								temp = this.Input_weights[w] - reset;
//								if (temp > -0.99)
//									this.Input_weights[w] = temp;
//								else{
//									flag = 1;
//									break;
//								}
//							}
//						}
//						if (flag==1){ // full back
//							for (; w>0 ; w--)
//								this.Input_weights[w] += reset;
//							this.Input_weights[place] -= by;
//						}
//					}
					
					
					int w = 0,flag = 0;
					for (; w < length ; w++) {
						if (w==place){
							this.Input_weights[w] += by;
							if (this.Input_weights[w] >= 0){
								flag=1; break;}
						}else{
							this.Input_weights[w] -= reset;
							if (this.Input_weights[w] <= -1){
								flag=1; break; }
						}
					}
					if (flag==1)
						for (; w >= 0 ; w--) {
						if (w==place)
							this.Input_weights[w] -= by;
						else
							this.Input_weights[w] += reset;
					}
					
					
					
				}
			}else{
				
				if(this.posiORneg==1){
					this.Input_weights[place]-=by;
					if (this.Input_weights[place] < minPosWeights)
						this.Input_weights[place]+=by;
				}else if(this.posiORneg==2){
					this.Input_weights[place]+=by;
					if (this.Input_weights[place] > maxNegWeights)
						this.Input_weights[place]-=by;
				}
				
				
//				if((this.posiORneg==1)&&((this.Input_weights[place]-by)>minPosWeights))
//					this.Input_weights[place]-=by;
//				else if ((this.posiORneg==2)&&((this.Input_weights[place]+by)<maxNegWeights))
//					this.Input_weights[place]+=by;
			}
		}//----------------------------------------------------------------

		
		public void addNueronToOutputList(ref Neuron source,int index,int delay,ref globalParam Param)
		{
			delay--;
			
			if (OutputNeuronsList.Length>0){
				int Number_Of_Neuron_In_output_List = output_weightIndex.Length +1;
				
				Neurons.Neuron[] tempVec1 = new Neurons.Neuron[Number_Of_Neuron_In_output_List];
				for (int i = 0; i < OutputNeuronsList.Length ; i++)
					tempVec1[i] = OutputNeuronsList[i];
				tempVec1[OutputNeuronsList.Length] = source;
				OutputNeuronsList = tempVec1;
				
				int[] temp_output_weightIndex = new int[Number_Of_Neuron_In_output_List];
				for (int i = 0; i < this.output_weightIndex.Length ; i++)
					temp_output_weightIndex[i] = output_weightIndex[i];
				temp_output_weightIndex[this.output_weightIndex.Length] = index;
				this.output_weightIndex = temp_output_weightIndex;
				
				int[] temp_Output_delay = new int[Number_Of_Neuron_In_output_List];
				for (int i = 0; i < this.Output_delay.Length ; i++)
					temp_Output_delay[i] = this.Output_delay[i];
				temp_Output_delay[this.Output_delay.Length] = delay;
				this.Output_delay = temp_Output_delay;
				
			}else{
				OutputNeuronsList = new Neuron[1];
				OutputNeuronsList[0] = source;
				
				this.output_weightIndex = new int[1];
				this.output_weightIndex[0] = index;
				
				this.Output_delay = new int[1];
				this.Output_delay[0] = delay;
				
			}
			
		}//----------------------------------------------------------------

		public int addNueronToInputList(ref Neuron source, double weight,int delay,ref globalParam Param)
		{
			Number_Of_Neuron_In_input_List = this.iniInput_weights.Length;
			Number_Of_Neuron_In_input_List++;
			delay--;
			
			if (iniInput_weights.Length>0){
				double[] temp_iniInput_weights = new double[Number_Of_Neuron_In_input_List];
				for (int i = 0; i < this.iniInput_weights.Length ; i++)
					temp_iniInput_weights[i] = iniInput_weights[i];
				temp_iniInput_weights[this.iniInput_weights.Length] = weight;
				this.iniInput_weights = temp_iniInput_weights;
				
				int[] temp_Input_delay = new int[Number_Of_Neuron_In_input_List];
				for (int i = 0; i < this.Input_delay.Length ; i++)
					temp_Input_delay[i] = this.Input_delay[i];
				temp_Input_delay[this.Input_delay.Length] = delay;
				this.Input_delay = temp_Input_delay;
				
				Neurons.Neuron[] tempVec1 = new Neurons.Neuron[Number_Of_Neuron_In_input_List];
				for (int i = 0; i < InputNeuronsList.Length ; i++)
					tempVec1[i] = InputNeuronsList[i];
				tempVec1[InputNeuronsList.Length] = source;
				InputNeuronsList = tempVec1;
				
			}else{
				// first time
				this.iniInput_weights = new double[1];
				iniInput_weights[0] = weight;
				
				this.Input_delay = new int[1];
				Input_delay[0] = delay;
				
				this.InputNeuronsList = new Neuron[1];
				InputNeuronsList[0] = source;
			}
			
			this.InputQu = new double[this.iniInput_weights.Length * Param.neuronParam.Neuron_Max_Delay];

			this.FullReset(ref Param);
			return ((this.iniInput_weights.Length-1));
			
		}//----------------------------------------------------------------

		
		
		
		
	} // End Class
}