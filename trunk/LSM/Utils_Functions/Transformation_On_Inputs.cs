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

namespace Utils_Functions
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	[Serializable()]
	public class Manipulation_On_Inputs
	{
		
		public Manipulation_On_Inputs()
		{
		}//------------------------------------------------------------
		
		
		public void InputTOOnary(ref globalParam Param, ref globalParam.Data[] data)
		{
			globalParam.Data[] tempdata = new globalParam.Data[data.Length];
			
			for (int i = 0; i < data.Length; i++) {
				tempdata[i].Input = (double[,]) data[i].Input.Clone();
				tempdata[i].Tag = data[i].Tag;
				tempdata[i].Target = (double[]) data[i].Target.Clone();
			}
			
			for (int i = 0; i < data.Length; i++) {
				int time = tempdata[i].Input.GetLength(0), stimuli = tempdata[i].Input.GetLength(1);
//				data[i].Input = new double[stimuli*digit,time*9];
				data[i].Input = new double[stimuli,time*50];
				
				for (int t = 0; t < time ; t++) {
					for (int s = 0; s < stimuli ; s++) {
						
//						double[] number = double2digits(tempdata[i].Input[t,s],0,digit);
//
//						for (int n = t*9 ; n < (t*9)+9 ; n++){
//							for (int d = s*digit ; d < (s*digit)+digit ; d++) {  // convert to Unary
//								if (number[d%digit]>=(n%9)){
//									data[i].Input[d,n]=Param.Ext_Inp_Neuron_Spike;
//								}else{
//									data[i].Input[d,n]=0;
//								}
//							}
//						}
						
						double number = Math.Round(tempdata[i].Input[t,s]/2);
						
						for (int n = t*50 ; n < (t*50)+50 ; n++){
							for (int d = 0 ; d < stimuli ; d++) {  // convert to Unary
								if (number>=(n%50)){
									data[i].Input[d,n]=Param.neuronParam.Ext_Inp_Neuron_Spike;
								}else{
									data[i].Input[d,n]=0;
								}
							}
						}

						
						
						
					}
				}
			}
			
			
		}
		
		
		public void FrequencyFFTtoOnary(ref globalParam Param, ref globalParam.Data data, ref int[,] channle)
		{
			int MaxLevel = 50;
			data.Input = new double[channle.GetLength(1),channle.Length*MaxLevel];
			int inputCounter=0;
			for (int input = 0; input < channle.GetLength(1); input++) {
				
				int counter=0;
				for (int time  = 0; time  < channle.Length; time ++) {

					//convert to Unary representation
					string[] temp;
					if (channle[time,input]<=(MaxLevel/2))
						temp = ToUnary(channle[time,input],Param.neuronParam.Ext_Inp_Neuron_Spike);
					else
						temp = new System.String[]{"0"};
					
					for (int i = 0; i < temp.Length; i++) {
						data.Input[inputCounter,counter] = Convert.ToDouble(temp[i]);
						counter++;
					}
					for (int i = temp.Length; i < MaxLevel ; i++) {
						data.Input[inputCounter,counter] = 0;
						counter++;
					}
				}
				inputCounter++;
			}
		}
		//--------------------------------------------------------
		public void FrequencyFFTtoOnaryNormelize(ref globalParam Param, ref globalParam.Data data, ref int[][] channle)
		{
			int min = channle[0][0];
			for (int i = 0; i < channle.Length; i++) {
				for (int t = 0; t < channle[i].Length; t++) {
					if( (channle[i][t]>0)&& (channle[i][t]<min)) min = channle[i][t];
				}
			}
			
			if (min >0) min--;
			FrequencyFFTtoOnary(ref Param,ref data,ref channle,min);
		}
		
		public void FrequencyFFTtoOnary(ref globalParam Param, ref globalParam.Data data, ref int[][] channle)
		{
			FrequencyFFTtoOnary(ref Param,ref data,ref channle,0);
		}
		
		public void FrequencyFFTtoOnary(ref globalParam Param, ref globalParam.Data data, ref int[][] channle,int min)
		{
			int MaxLevel = 50;
			data.Input = new double[channle[0].Length,channle.Length*MaxLevel];
			int inputCounter=0;
			for (int input = 0; input < channle[0].Length; input++) {
				
//				data.Input[inputCounter] = new double[channle.Length*Param.MaxLevel];
				int counter=0;
				for (int time  = 0; time  < channle.Length; time ++) {

					//convert to Unary representation
					string[] temp;
					if ((channle[time][input]<=(MaxLevel/2))&&(channle[time][input]-min>0))
						temp = ToUnary(channle[time][input]-min,Param.neuronParam.Ext_Inp_Neuron_Spike);
					else
						temp = new System.String[]{"0"};
					
					for (int i = 0; i < temp.Length; i++) {
						data.Input[inputCounter,counter] = Convert.ToDouble(temp[i]);
						counter++;
					}
					for (int i = temp.Length; i < MaxLevel ; i++) {
						data.Input[inputCounter,counter] = 0;
						counter++;
					}
				}
				inputCounter++;
			}
		}
		//--------------------------------------------------------

		
		public void FrequencyFFTtoDirectInput(ref globalParam Param, ref globalParam.Data data, ref int[,] channle)
		{
			int MaxLevel = 50;
			int x = channle.GetLength(1),y = channle.GetLength(0);
			
			data.Input = new double[x,y];
			for (int input = 0; input < x; input++) {
				for (int time  = 0; time  < y ; time ++) {
					if (channle[time,input]<=(MaxLevel/4))
						data.Input[input,time] = channle[time,input];
					else
						data.Input[input,time] = 0;
				}
			}
		}
		//--------------------------------------------------------
		
		public void MRIinput_Method_1(ref globalParam Param, out double[,] data, ref double[,] inputData)
		{
			int numbersBeforeDot = 4;
			int numbersAfterDot = 10;
			double StreamFactor = 40;
			
			// input of 3.1234 , 4.8563 , 2.5864 wil be in chanelse
			//	time :    t1			,		t2				t3
			//		1: 3.1234 * factor	,	4.8563 * factor		,	2.5864  * factor
			//		2: 1 * factor		,	8 * factor			,	5  * factor
			//		3: 2 * factor		,	5 * factor			,	8  * factor
			//		4: 3 * factor		,	6 * factor			,	6  * factor
			//		5: 4 * factor		,	3 * factor			,	4  * factor
			
			int digits = numbersAfterDot+numbersBeforeDot;
			int time = inputData.GetLength(0);
			data = new double[1+digits,time];
			int VoxelNumber = Param.iteration;
			double factor = StreamFactor;
			
			// first channle
			for (int t = 0 ; t < time ; t++ ) {	data[0,t] = factor * inputData[t,VoxelNumber]; }
			// rest of the channles
			for (int t = 0 ; t < time ; t++ ){
				double[] temp = this.double2digits(inputData[t,VoxelNumber],numbersBeforeDot,numbersAfterDot);
				for (int x =0 ; x < digits ; x++){
					data[x+1,t] = factor * temp[x];
				}
			}
		}//------------------------------------------------------------
		
		
		public void MRIinput_Method_2(ref globalParam Param, out double[,] data, ref double[,] inputData)
		{
			double StreamFactor = 40;
			// input of 3.1234 , 4.8563 , 2.5864 wil be in chanelse
			//	time :    t1			,		t2				,		t3
			//		1: 3.1234 * factor	,	3.1234 * factor		,	3.1234 * factor
			//		2: 4.8563 * factor	,	4.8563 * factor		,	4.8563 * factor
			//		3: 2.5864 * factor	,	2.5864 * factor		, 	2.5864 * factor

			data = new double[inputData.GetLength(0),1];
			int VoxelNumber = Param.iteration;
			double factor = StreamFactor;
			// first channle
			for (int t = 0 ; t < data.Length ; t++ ) {	data[t,0] = factor * inputData[t,VoxelNumber]; }
		}//------------------------------------------------------------
		
		public void Normelize(ref globalParam.Data[] data,ref globalParam Param)
		{
			double max=0,min=0;
			for (int i = 0 ; i < data.Length ; i++ ) {
				int dimx = data[i].Input.GetLength(0), dimy = data[i].Input.GetLength(1);
				for (int j=0 ; j < dimx ; j++ ) {
					for (int x=0 ; x < dimy ; x++ ) {
						if (max < data[i].Input[j,x]) max = data[i].Input[j,x];
						if (min > data[i].Input[j,x]) min = data[i].Input[j,x];
					}
				}
			}
			
			for (int i = 0 ; i < data.Length ; i++ ) {
				int dimx = data[i].Input.GetLength(0), dimy = data[i].Input.GetLength(1);
				for (int j=0 ; j < dimx ; j++ ) {
					for (int x=0 ; x < dimy ; x++ ) {
						if (data[i].Input[j,x] >0)
							data[i].Input[j,x] = data[i].Input[j,x]/max;
						else
							data[i].Input[j,x] = data[i].Input[j,x]/min;
					}
				}
			}
		}//------------------------------------------------------------

		public void DuplicateInputToNeg(ref globalParam.Data[] data,ref globalParam Param)
		{
			for (int datapoint = 0; datapoint < data.Length; datapoint++) {
				int inputsize=data[datapoint].Input.GetLength(1);
				int inputlenght=data[datapoint].Input.GetLength(0);
				if(inputlenght>1) continue;
				else if(inputlenght==1){
					double[,] newInput = new double[2,inputsize];
					for (int i = 0; i < inputsize; i++) {
						if (data[datapoint].Input[0,i]==Param.neuronParam.Ext_Inp_Neuron_Spike){
							newInput[0,i] = Param.neuronParam.Ext_Inp_Neuron_Spike;
							newInput[1,i] = 0;
						}else{
							newInput[0,i] = 0;
							newInput[1,i] = Param.neuronParam.Ext_Inp_Neuron_Spike;
						}
					}
					data[datapoint].Input = newInput;
				}
			}
			
			
		}//------------------------------------------------------------

		
		public void Normelize(ref globalParam Param)
		{
			this.Normelize(ref Param.LearnData, ref Param);
			this.Normelize(ref Param.TestData, ref Param);
			
		}//------------------------------------------------------------
		
		
		
		
		public double[] double2digits(double inNumber,int numBeforeDot, int numAfterDot)
		{
			int counter = numAfterDot+numBeforeDot;
			double[] outnumber = new double[counter];
			double number = inNumber;
			
			for(int i = numBeforeDot ; i > 0 ; i--){
				counter--;
				outnumber[counter] = (int) (number/(Math.Pow(10,i)));
				number = (number%(Math.Pow(10,i)));
			}
			if (numBeforeDot==0)
				number = inNumber*Math.Pow(10,numAfterDot+1);
			else
				number = inNumber*Math.Pow(10,numAfterDot);
			
			for(int i = numAfterDot ; i > 0 ; i--){
				counter--;
				outnumber[counter] = (int) (number/(Math.Pow(10,i)));
				number = (number%(Math.Pow(10,i)));
			}
			return outnumber;
		}//------------------------------------------------------------
		
		public int ToDecimal(string bin)
		{
			return Convert.ToInt32(bin,2);
		}
		//--------------------------------------------------------
		
		
		public string[] ToUnary(int num,double spike)
		{
			string[]  returnNum = new System.String[num];
			for (int i = 0 ; i < num ; i++)
				returnNum[i] = spike.ToString();
			
			return returnNum;
		}
		//--------------------------------------------------------
		
		
		
		public string ToBinary(Int64 Decimal)
		{
			// Declare a few variables we're going to need
			Int64 BinaryHolder;
			char[] BinaryArray;
			string BinaryResult = "";
			
			if ( Decimal==0) {
				BinaryResult="0";
			}

			while (Decimal > 0)
			{
				BinaryHolder = Decimal % 2;
				BinaryResult += BinaryHolder;
				Decimal = Decimal / 2;
			}
			// The algoritm gives us the binary number in reverse order (mirrored)
			// We store it in an array so that we can reverse it back to normal
			BinaryArray = BinaryResult.ToCharArray();
			Array.Reverse(BinaryArray);
			BinaryResult = new string(BinaryArray);

			return BinaryResult;
		}
		//--------------------------------------------------------
		
	}

}

