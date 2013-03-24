/*
 * Created by SharpDevelop.
 * User: hhazan01
 * Date: 05/05/2009
 * Time: 10:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Liquid_Detector.Tests.TestInput
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class Input
	{
		globalParam.Data[][] data;
		
		long[] numbers;
		
		public Input(){
			this.numbers= new long[]{2154623452485,574328521581,2463518721842,9874863015411,123428625684,6874681313534,4681326,78768443543,3543534211,85432134,215452485,5742518,246358242,9605411,123425684,6874134,468741816,87634376,3532112158,8543499721};
		}
		//--------------------------------------------------------
		
		public void initInput(int time_of_input,int samples,ref globalParam Param){
			
			string pattern = ToBinary(this.numbers[1]);
			int pattern_iteration = time_of_input/pattern.Length;
			Param.NumberOfSession = new int[samples];
			this.data = new globalParam.Data[samples][];
			for(int i = 0 ; i < samples ; i++) {
				Param.NumberOfSession[i] = i ;
				this.data[i] = new globalParam.Data[samples];
//				int flag=0,counter=0;
				for (int t = 0 ;  t < samples ; t++ ) {
					
					this.data[i][t].Input = new double[Param.NumberOfTimeInEachTest,Param.test.NumOfVoxels];
					this.data[i][t].Tag = new int();
					this.data[i][t].Tag = t;
					
//					for ( int time=0 ; time < time_of_input ; time++ ) {
//						
//						int TempInput;
//						if (flag == 0) {
//							if (Rnd.NextDouble()>0.5) TempInput = 1;
//							else TempInput = 0;
//							
//						}else{
//							if (pattern[counter]=='0') TempInput = 0 ;
//							else TempInput = 1 ;
//							
//							if ((counter+1) == pattern.Length) {flag = 0; counter=0;}
//							else counter++;
//						}
//						//this.data[i][t].Input = TempInput;
//					}
				}
			}
			
		}
		//--------------------------------------------------------
		
		
		public string returnInput(int num){
			
			string ret = "TESTING";
			
			return ret;
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
	}
}
