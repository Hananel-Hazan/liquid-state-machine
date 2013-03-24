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
	/// Description of randomIntArr.
	/// </summary>
	[Serializable()]
	public class randomIntArr
	{
		
		public randomIntArr ()
		{
			
		}
		
		public double NextDouble(ref globalParam Param, double min, double max )
		{
			return min + Param.rnd.NextDouble()*(max-min);
		}

		public void select(int min, int max, ref int[] arr, ref Random rnd)
		{
			int[] temp = new int[max-min];
			for (int count=0, i = min ; i < max  ; count++ , i++ ) temp[count]=i;
			this.shuffle(ref temp, ref rnd);
			for (int i=0 ; i < arr.Length ; i++ )
				arr[i] = temp[i];
		}
		public void select(int min, int max, ref int[] arr, ref globalParam Param)
		{
			select(min, max, ref arr, ref Param.rnd);
		}
		
		public void select(int min, int max, ref long[] arr, ref globalParam Param)
		{
			long[] temp = new long[max-min];
			for (long count=0, i = min ; i < max  ; count++,i++ ) { temp[count]=i;}
			this.shuffle(ref temp, ref Param);
			for (long i=0 ; i < arr.Length ; i++ )
				arr[i] = temp[i];
		}
		
		
		public void dselect(int min, int max, int dontMin, int dontMax, ref int[] arr, ref globalParam Param)
		{
			int[] temp 	= new int[(max-min)-(dontMax-dontMin)];
			for (int i=min,count=0 ; i < max ; i++ ) {
				if ((i>=dontMin)&&(i<=dontMax)) continue;
				temp[count] = i;
				count++;
			}
			
			do{
				this.shuffle(ref temp, ref Param);
			}while (Param.rnd.NextDouble()<0.5);
			
			for (int i=0 ; i < arr.Length ; i++ ) {
				arr[i] = temp[i];
			}
		}
		
		public void dselect(int min, int max, int[] dont, ref int[] arr, ref globalParam Param)
		{
			int[] temp 	= new int[((max-min)-dont.Length)+1];
			for (int i=min,count=0 ; i < max ; i++ ) {
				int flag=0;
				for (int t=0 ; t < dont.Length ; t++ )
					if (i==dont[t]) flag=1;
				if (flag==1) continue;
				temp[count] = i;
				count++;
			}
			
			do{
				this.shuffle(ref temp, ref Param);
			}while (Param.rnd.NextDouble()<0.5);
			
			for (int i=0 ; i < arr.Length ; i++ ) {
				arr[i] = temp[i];
			}
		}
		
		public void dselect(int min, int max, int dont, ref int[] arr, ref globalParam Param)
		{
			int[] temp 	= new int[arr.Length];
			for (int i=min,count=0 ; i <= max ; i++ ) {
				if (i==dont) continue;
				if (count==temp.Length) break;
				temp[count] = i;
				count++;
			}
			
			do{
				this.shuffle(ref temp, ref Param);
			}while (Param.rnd.NextDouble()<0.5);
			
			for (int i=0 ; i < arr.Length ; i++ ) {
				arr[i] = temp[i];
			}
		}
		
		//--------------------------------------------------------------------------------------
		public void shuffle(ref int[] arr, ref globalParam Param)
		{
			shuffle(ref arr, ref Param.rnd);
		}
		
		public void shuffle(ref int[] arr, ref Random rnd)
		{
			int rand_ind,temp;
			for(int n=1 ; n<arr.Length ; n++)
			{
				rand_ind=rnd.Next(0,n-1); // function returns element in [0,n-1], it may be rand()%n.
				temp = arr[rand_ind];
				arr[rand_ind] = arr[n-1];
				arr[n-1] = temp;
			}
		}
		//--------------------------------------------------------------------------------------
		
		public void shuffle(ref int[] arr,int from,int until, ref globalParam Param)
		{
			int rand_ind,temp;
			for(int n=1 ; n<arr.Length ; n++)
			{
				rand_ind=Param.rnd.Next(from,n-1); // function returns element in [0,n-1], it may be rand()%n.
				temp = arr[rand_ind];
				arr[rand_ind] = arr[n-1];
				arr[n-1] = temp;
			}
		}
		//--------------------------------------------------------------------------------------
		
		public void shuffle(ref long[] arr, ref globalParam Param)
		{
			int rand_ind;
			long temp;
			for(int n=1 ; n<arr.Length ; n++)
			{
				rand_ind=Param.rnd.Next(0,n-1); // function returns element in [0,n-1], it may be rand()%n.
				temp = arr[rand_ind];
				arr[rand_ind] = arr[n-1];
				arr[n-1] = temp;
			}
		}
		//--------------------------------------------------------------------------------------
		
		public void random_power_low_sellect_Mathod1(int minimum_connection,int[] nodesList,int howMuchToSellect,int extend, out int[] list,out int[] countingList, ref globalParam Param)
		{
			// oreginal one
			list = new int[howMuchToSellect];
			countingList = new int[nodesList.Length];
			int additionalSize=(howMuchToSellect*extend);
			int nodesListSize=nodesList.Length;
			for ( int i=0 ; i<countingList.Length ;  i++) {countingList[i]=0;}
			
			Array.Resize(ref nodesList,nodesListSize+additionalSize);
			for ( int mone=0; mone < howMuchToSellect ; mone++ ) {
				int candidant = nodesList[Param.rnd.Next(0,nodesListSize)];
				for ( int i = nodesListSize ; i < (nodesListSize+extend) ;i++ ) { nodesList[i] = candidant;	}
				list[mone] = candidant;
				nodesListSize+= extend;
			}
			//------------------------------------
			
			for ( int i=0 ; i<countingList.Length ;  i++) {countingList[i]=0;}
			long sellectCounter = 0;
			long[] temp = new long[list.Length];
			list.CopyTo(temp,0);
			Array.Sort(temp);
			long lastNumber = temp[0];
			for ( int i=0 ; i<temp.Length ;  i++) {
				countingList[sellectCounter]++;
				if (lastNumber!=temp[i]) { lastNumber=temp[i]; sellectCounter++;}
			}
			
			nodesList = new int[1];
			nodesList = null;
		}
		
	}

	public class RandomPowerLaw
	{
		Matrix_Arithmetic VectorMethods;
		int[] nodesListCounter,LeftOverList;
		int candidate,listSize,listSize_org,extend,extend_org;
		public int counter,nodes;

		
		public RandomPowerLaw(int NumOfnodesList, int extend,ref globalParam Param)
		{
			this.listSize_org = NumOfnodesList;
			this.extend_org = extend;
			this.nodes = NumOfnodesList;
			this.Rest(ref Param);
		}
		
		public RandomPowerLaw(int NumOfnodesList, int extend,int[] connections,ref globalParam Param)
		{
			this.listSize_org = NumOfnodesList;
			this.extend_org = extend;
			this.nodes = NumOfnodesList;
			this.Rest(ref Param);
		}
		
		public void Rest(int[] connections,ref globalParam Param){
			this.listSize = this.listSize_org;
			this.extend = this.extend_org;
			
			VectorMethods = new Matrix_Arithmetic();
			
			nodesListCounter = new int[listSize];
			LeftOverList = new int[listSize];
			for (int i=0 ; i < listSize ; i++ ){
				nodesListCounter[i] = connections[listSize-i-1];
			}
			this.NewNode(ref Param);
		}
		
		public void Rest(ref globalParam Param){
			this.listSize = this.listSize_org;
			this.extend = this.extend_org;

			VectorMethods = new Matrix_Arithmetic();
			nodesListCounter = new int[listSize];
			LeftOverList = new int[listSize];
			for (int i=0 ; i < listSize ; i++ ){
				nodesListCounter[i] = 1;
				LeftOverList[i] = i;
			}
			Param.rndA.shuffle(ref LeftOverList, ref Param);
		}
		
		public void NewNode(ref globalParam Param){
			int lengh=0;
			for (int i = 0; i < nodesListCounter.Length ; i++ ) { lengh+=(nodesListCounter[i]-1)*this.extend_org; }
			LeftOverList = new int[lengh+nodesListCounter.Length];
			int count=0;
			for (int i=0 ; i < nodesListCounter.Length ; i++ )
				for (int t = 0 ; t < ((nodesListCounter[i]-1)*this.extend_org)+1 ; t++ ) {
				LeftOverList[count] = i;
				count++;
			}
			Param.rndA.shuffle(ref LeftOverList, ref Param);
			this.counter=0;
		}
		
		public bool Next(ref int candid,ref globalParam Param){
			if (LeftOverList.Length==0) return false;
			this.candidate = Param.rnd.Next(0,LeftOverList.Length);
			this.candidate = LeftOverList[this.candidate];
			candid = this.candidate;
			return true;
		}
		
		public bool Next(ref int candid,int min,int max,ref globalParam Param){
			if (LeftOverList.Length==0) return false;
			do{
				this.candidate = LeftOverList[Param.rnd.Next(0,LeftOverList.Length)];
			}while(!(this.candidate>min)&&(this.candidate<max));
			candid = this.candidate;
			return true;
		}
		
		public void NotGood()
		{
			VectorMethods.delNum(ref LeftOverList,this.candidate);
		}
		
		public void Good()
		{
			this.nodesListCounter[this.candidate]++;
			VectorMethods.delNum(ref LeftOverList,this.candidate);
			counter++;
		}
		
	}


	public class PowerLaw
	{
		int[] In_LeftOverList,Out_LeftOverList;
		int In_candidate,Out_candidate,listSize,extend,connections;
		
		public PowerLaw(int extend,int size,int connections,ref globalParam Param)
		{
			this.extend = extend;
			this.listSize = size;
			this.connections = connections;
			this.Rest(ref Param);
		}
		
		public bool Rest(ref globalParam Param)
		{
			int counter=0,temp = this.listSize*(this.listSize-1);
			this.In_LeftOverList = new int[temp];
			this.Out_LeftOverList = new int[this.listSize];
			for (int i = 0 ; i < this.listSize ; i++ ){
				this.Out_LeftOverList[i] = i;
				for (int t = 0 ; t < (this.listSize-1) ; t++ ){
					this.In_LeftOverList[counter] = i;
					counter++;
				}
			}
			Param.rndA.shuffle(ref this.Out_LeftOverList, ref  Param);
			Param.rndA.shuffle(ref this.In_LeftOverList, ref  Param);
			
			this.In_candidate = 0 ; this.Out_candidate = 0 ;
			return true;
		}
		
		public bool Next(ref int output,ref int input)
		{
			int maxSize = (this.extend * this.connections) + this.listSize;
			if ((this.Out_LeftOverList.Length == maxSize)||(this.In_LeftOverList.Length == 0)) return false;
			
			input = In_LeftOverList[this.In_candidate];
			output = Out_LeftOverList[this.Out_candidate];
			
			return true;
		}
		
		public void Good(ref globalParam Param)
		{
			Matrix_Arithmetic mat = new Matrix_Arithmetic();
			
			int[] IndexForDel = mat.FindIndex(ref In_LeftOverList,In_LeftOverList[this.In_candidate]);
			int[] index;
			if (this.extend > IndexForDel.Length) 	index = new int[IndexForDel.Length];
			else index = new int[this.extend];
			Param.rndA.select(0,IndexForDel.Length,ref index, ref  Param);
			for (int i = 0; i < index.Length; i++) {
				index[i] = IndexForDel[index[i]];
			}
			mat.AddCell(ref index , 0);
			mat.DelCells(ref In_LeftOverList,index);
			
			index = new int[this.extend];
			Param.rndA.select(0,this.Out_LeftOverList.Length+extend-1,ref index, ref  Param);
			mat.AddCells(ref this.Out_LeftOverList,this.Out_LeftOverList[Out_candidate],ref index);
			mat.Shift(ref Out_LeftOverList,1);
			
			this.In_candidate = 0 ; this.Out_candidate = 0 ;
		}
		
		public bool NotGood()
		{
			if (this.In_candidate < this.In_LeftOverList.Length-1) { this.In_candidate++; return true;}
			else if ((this.In_candidate == this.In_LeftOverList.Length-1)&& (this.Out_candidate < this.Out_LeftOverList.Length-1 )) {
				this.In_candidate=0;
				this.Out_candidate++;
				return true;
			}
			else
				return false; //ERROR~!!!
		}
		
		
	}
}