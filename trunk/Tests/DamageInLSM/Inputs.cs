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

namespace Liquid_Detector.Tests.DamageLSM
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class Input
	{
		public globalParam.Data[] InputVec;
		double[][] InputVecTimeTest;
		int[] numbers;
		public int HowManyNumbers;
		
//		public Input(int HowManyNumbers){
//			this.HowManyNumbers=HowManyNumbers;
//			this.InputVec = new globalParam.Data[HowManyNumbers];
//			this.InputVecTimeTest = new double[HowManyNumbers][];
//			this.numbers= new long[]{268435456,2154623452485,574328521581,2463518721842,9874863015411,123428625684,6874681313534,4681326,78768443543,3543534211,85432134,215452485,5742518,246358242,9605411,123425684,6874134,468741816,87634376,3532112158,8543499721};
//			this.TimeInput = new long[]{5,11,22,25,44,64};
//		}
//		//--------------------------------------------------------
		
		public Input(ref globalParam Param,int mode){
			this.HowManyNumbers = Param.test_Param.Numbers_of_Inputs;
			this.InputVec = new globalParam.Data[HowManyNumbers];
			this.InputVecTimeTest = new double[HowManyNumbers][];
			
			for (int i = 0; i < HowManyNumbers; i++) 
				this.InputVec[i].MaxInputLength = Param.test_Param.Input_Pattern_Size;
			
			if (mode==0) {
				this.numbers= new int[]{268435456,154234525,574328581,24635182,98745411,12345684,687468134,4681326,78763543,43534211,85432134,215452485,5742518,246358242,9605411,123425684,6874134,468741816,87634376,332112158,85499721};
			}else if (mode==1){
				this.numbers = new int[HowManyNumbers];
				this.SetInputToBinaryRandom(ref Param);
			}
		}
		//--------------------------------------------------------
		
		public void SetInputToPattern(int[] numbers){
			this.HowManyNumbers  = 2;
			this.InputVec = new globalParam.Data[HowManyNumbers];
			int ii=0,iii=0;
			for (int i = 0; i < HowManyNumbers; i++) {
				int sum = 0;
				for (int j = 0; j < numbers.Length/2 ; j++) {
					string s = ToBinary(numbers[j]);
					sum += s.Length;
				}
				this.InputVec[i].Input = new double[numbers.Length/2,sum];
				this.InputVec[i].Target = new double[1];
			}
			
			
			for (int i=0 ; i<numbers.Length ; i++ ) {
				if (i==numbers.Length/2) {ii++; iii = 0;}
				string s = ToBinary(numbers[i]);
				for (int t = 0 ; t < s.Length ; t++ ) {
					if (s[t].ToString()=="1") this.InputVec[ii].Input[iii,t] = 1 ;
					else this.InputVec[ii].Input[iii,t] = 0 ;
				}
				iii++;
				this.InputVec[ii].Tag = 0;
				if (ii==1) this.InputVec[ii].Target[0] = 0; 
				else this.InputVec[ii].Target[0] = 1;
			}
		}
		//--------------------------------------------------------
		
		public void returnTargetData(out double[] targetData){
			targetData = new double[this.InputVec.Length];
			for (int i=0 ; i<this.InputVec.Length ; i++ ) {
				targetData[i] = this.InputVec[i].Target[0];
			}
		}
		//--------------------------------------------------------
		
		public void initTimePatterns(double fire){
			for (int i=0 ; i<this.HowManyNumbers ; i++ ) {
				char[] bits = ToBinary(this.numbers[i]).ToCharArray();
				this.InputVecTimeTest[i] = new double[bits.Length];
				for (int t = 0 ; t < bits.Length ; t++ ) {
					if (bits[t].ToString()=="1") this.InputVecTimeTest[i][t] = fire ;
					else this.InputVecTimeTest[i][t] = 0 ;
//					this.InputVecTimeTest[i][t] = Convert.ToDouble(bits[t].ToString());
				}
			}
		}
		//--------------------------------------------------------
		
		public void SetInputToBinaryRandom(ref globalParam Param){
			double fire = Param.neuronParam.Ext_Inp_Neuron_Spike;
			int negativ = Param.detector.Readout_Negative;
			for (int i=0 ; i<this.InputVec.Length ; i++ ) {
				this.InputVec[i].Input = new double[1,this.InputVec[i].MaxInputLength];
				for (int t = 0 ; t < this.InputVec[i].MaxInputLength ; t++ ) {
					if (Param.rnd.NextDouble()>0.5) this.InputVec[i].Input[0,t] = fire ;
					else this.InputVec[i].Input[0,t] = 0 ;
				}
				this.InputVec[i].Tag = 0;
				this.InputVec[i].Target = new double[1];
				if (i < (this.InputVec.Length/2)) this.InputVec[i].Target[0] = negativ;
				else this.InputVec[i].Target[0] = 1;
			}
		}
		//--------------------------------------------------------
		
		public void SetInputToRandomValues(ref globalParam Param){
			int size = Param.detector.LSM_Runing_Interval;
			for (int i=0 ; i<this.InputVec.Length ; i++ ) {
				this.InputVec[i].Input = new double[1,this.InputVec[i].MaxInputLength];
				for (int t = 0 ; t < this.InputVec[i].MaxInputLength ; t++ ) 
					this.InputVec[i].Input[0,t] = Param.rnd.NextDouble();
				this.InputVec[i].Tag = 0;
				this.InputVec[i].Target = new double[1];
				if (i < (this.InputVec.Length/2)) this.InputVec[i].Target[0] = Param.detector.Readout_Negative;
				else this.InputVec[i].Target[0] = 1;
			}
		}
		//--------------------------------------------------------
		
		public void SetInputToRandomValues(ref globalParam Param,double min,double max){
			int size = Param.detector.LSM_Runing_Interval;
			for (int i=0 ; i<this.InputVec.Length ; i++ ) {
				this.InputVec[i].Input = new double[1,this.InputVec[i].MaxInputLength];
				for (int t = 0 ; t < this.InputVec[i].MaxInputLength ; t++ ) 
					this.InputVec[i].Input[0,t] = Param.rndA.NextDouble(ref Param,min,max);
				this.InputVec[i].Tag = 0;
				this.InputVec[i].Target = new double[1];
				if (i < (this.InputVec.Length/2)) this.InputVec[i].Target[0] = Param.detector.Readout_Negative;
				else this.InputVec[i].Target[0] = 1;
			}
		}
		//--------------------------------------------------------

		public int returnSizeOfVector(int number){
			return this.InputVec[number].Input.Length;
		}
		//--------------------------------------------------------
		
		public double[] returnInputAtPlace(int number,int place){
			double[] temp = new double[this.InputVec[number].Input.GetLength(1)];
			for (int i = 0; i < temp.Length; i++) 
				temp[i] = this.InputVec[number].Input[place,i];
			return (temp);
		}
		//--------------------------------------------------------

		public double[] returnInput(int number){
			return this.InputVecTimeTest[number];
		}
		//--------------------------------------------------------
		
		static private int ToDecimal(string bin)
		{
			return Convert.ToInt32(bin,2);
		}
		//--------------------------------------------------------
		
		static private string ToBinary(Int64 Decimal)
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
		
		
	}//---------------------
}
