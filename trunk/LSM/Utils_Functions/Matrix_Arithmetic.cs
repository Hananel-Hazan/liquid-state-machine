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
	/// Description of Matrix_Arithmetic.
	/// </summary>
	
	[Serializable()]
	public class Matrix_Arithmetic
	{
		public Matrix_Arithmetic()
		{}
		
		public double Max(double[,] Matrix)
		{
			double max=Matrix[0,0];
			for (int i = 0 ; i < Matrix.GetLength(0) ; i++) {
				for (int j = 0 ; j < Matrix.GetLength(1) ; j++ ) {
					if (max<Matrix[i,j]) max = Matrix[i,j];
				}
			}
			return max;
		}
		
		public double Min(double[,] Matrix)
		{
			double min=Matrix[0,0];
			for (int i = 0 ; i < Matrix.GetLength(0) ; i++) {
				for (int j = 0 ; j < Matrix.GetLength(1) ; j++ ) {
					if (min>Matrix[i,j]) min = Matrix[i,j];
				}
			}
			return min;
		}
		
//		public static int operator+(int mat1,int mat2)
//		{
//			int[] returnMat = new int[mat1.Length];
//
//			for (int x=0; x < mat1.Length ; x++)
//					returnMat[x] = mat1[x] + mat2[x];
//
//			return newMatrix;
//		}
		
		
//		public double Max(double[][] Matrix)
//		{
//			double max=Matrix[0][0];
//			for (int i = 0 ; i < Matrix.Length ; i++) {
//				for (int j = 0 ; j < Matrix[i].Length ; j++ ) {
//					if (max<Matrix[i][j]) max = Matrix[i][j];
//				}
//			}
//			return max;
//		}
//
//		public double Min(double[][] Matrix)
//		{
//			double min=Matrix[0][0];
//			for (int i = 0 ; i < Matrix.Length ; i++) {
//				for (int j = 0 ; j < Matrix[i].Length ; j++ ) {
//					if (min>Matrix[i][j]) min = Matrix[i][j];
//				}
//			}
//			return min;
//		}
		
		public double Max(double[] Matrix)
		{
			double max=Matrix[0];
			for (int j = 0 ; j < Matrix.Length ; j++ )
				if (max<Matrix[j]) max = Matrix[j];
			return max;
		}
		
		
		public double Min(double[] Matrix)
		{
			double min=Matrix[0];
			for (int j = 0 ; j < Matrix.Length ; j++ )
				if (min > Matrix[j]) min = Matrix[j];
			return min;
		}
		
		
//		public int Max(int[][] Matrix)
//		{
//			int max=Matrix[0][0];
//			for (int i = 0 ; i < Matrix.Length ; i++) {
//				for (int j = 0 ; j < Matrix[i].Length ; j++ ) {
//					if (max<Matrix[i][j]) max = Matrix[i][j];
//				}
//			}
//			return max;
//		}
//
//		public int Min(int[][] Matrix)
//		{
//			int min=Matrix[0][0];
//			for (int i = 0 ; i < Matrix.Length ; i++) {
//				for (int j = 0 ; j < Matrix[i].Length ; j++ ) {
//					if (min>Matrix[i][j]) min = Matrix[i][j];
//				}
//			}
//			return min;
//		}
		
		public int Max(int[] Matrix)
		{
			int max=Matrix[0];
			for (int j = 0 ; j < Matrix.Length ; j++ ) {
				if (max<Matrix[j]) max = Matrix[j];
			}
			return max;
		}
		
		public int Min(int[] Matrix)
		{
			int min=Matrix[0];
			for (int j = 0 ; j < Matrix.Length ; j++ ) {
				if (min>Matrix[j]) min = Matrix[j];
			}
			return min;
		}
		//-----------------------------------------------------
		public int Count(int[] Matrix,int number)
		{
			int r =0;
			for (int j = 0 ; j < Matrix.Length ; j++ ) {
				if (number == Matrix[j]) r++;
			}
			return r;
		}
		
		public int Count(double[] Matrix,double number)
		{
			int r =0;
			for (int j = 0 ; j < Matrix.Length ; j++ ) {
				if (number == Matrix[j]) r++;
			}
			return r;
		}
		
		public int Count(int[] Matrix,int number,ref int[] index)
		{
			int r =0;
			for (int j = 0 ; j < Matrix.Length ; j++ ) {
				if (number == Matrix[j]) {
					r++;
					AddCell(ref index,j);
				}
			}
			return r;
		}
		
		//-----------------------------------------------------
		
//		public double[] Vector_Average(double[][] Matrix)
//		{
//			double[] sum = new double[Matrix.Length];
//			for (int i = 0 ; i < Matrix.Length ; i++ ) {
//				int counter=0;
//				sum[i] = 0 ;
//				for (int t = 0 ;  t < Matrix[i].Length ; t++ ) {
//					sum[i] += Matrix[i][t];
//					counter++;
//				}
//				sum[i] = sum[i] / counter;
//			}
//
//			return sum;
//		}
		//-------------------------------------------------
		
		public double[] Vector_Average(double[,] Matrix)
		{
			double[] sum = new double[Matrix.GetLength(0)];
			for (int i = 0 ; i < Matrix.GetLength(0) ; i++ ) {
				int counter=0;
				sum[i] = 0 ;
				for (int t = 0 ;  t < Matrix.GetLength(1) ; t++ ) {
					sum[i] += Matrix[i,t];
					counter++;
				}
				sum[i] = sum[i] / counter;
			}
			
			return sum;
		}
		//-------------------------------------------------
		
		public double[] Vector_Average(double[,,] Matrix)
		{
			double[] sum = new double[Matrix.GetLength(0) * Matrix.GetLength(1)];
			int count = 0;
			for (int i = 0 ; i < Matrix.GetLength(1) ; i++ ) {
				for (int j = 0; j < Matrix.GetLength(0); j++) {
					int counter=0;
					sum[count] = 0 ;
					for (int t = 0 ;  t < Matrix.GetLength(2) ; t++ ) {
						sum[count] += Matrix[j,i,t];
						counter++;
					}
					sum[count] = sum[count] / counter;
					count++;
				}
			}
			
			return sum;
		}
		//-------------------------------------------------
		
		public double Vector_Average(double[] Matrix)
		{
			double sum = 0;
			for (int i = 0 ; i < Matrix.Length ; i++ ) {
				sum += Matrix[i];
			}
			sum = sum / Matrix.Length;
			return sum;
		}
		//-------------------------------------------------
		
		public void AddCell(ref int[] vector,int num)
		{
//			int size = vector.Length;
//			Array.Resize(ref vector,size+1);
//			vector[size] = num;
			
			int[] tempVec = new int[vector.Length+1];
			vector.CopyTo(tempVec,0);
			tempVec[vector.Length] = num;
			vector = tempVec;
		}
		//-------------------------------------------------
		
		public void AddCell(ref double[] vector,double num)
		{
//			int size = vector.Length;
//			Array.Resize(ref vector,size+1);
//			vector[size] = num;
			
			double[] tempVec = new double[vector.Length+1];
			vector.CopyTo(tempVec,0);
			tempVec[vector.Length] = num;
			vector = tempVec;
		}
		//-------------------------------------------------
		
		public void AddCell(ref Neurons.Neuron[] vector,Neurons.Neuron num)
		{
//			int size = vector.Length;
//			Array.Resize(ref vector,size+1);
//			vector[size] = num;
			
			Neurons.Neuron[] tempVec = new Neurons.Neuron[vector.Length+1];
			vector.CopyTo(tempVec,0);
			tempVec[vector.Length] = num;
			vector = tempVec;
		}
		//-------------------------------------------------
		
		public void AddCell(ref int[] vector1,int num1,
		                    ref double[] vector2,double num2)
		{
			int newsize = vector1.Length+1;
			int[] tempVec1 = new int[newsize];
			double[] tempVec2 = new double[newsize];
			for (int i = 0; i < vector1.Length; i++) {
				tempVec1[i] = vector1[i];
				tempVec2[i] = vector2[i];
			}
			tempVec1[vector1.Length] = num1;
			tempVec2[vector1.Length] = num2;
			vector1 = tempVec1;
			vector2 = tempVec2;
		}
		//-------------------------------------------------
		
		public void AddCell(ref Neurons.Neuron[] vector1,Neurons.Neuron num1,
		                    ref double[] vector2,double num2,
		                    ref int[] vector3,int num3)
		{
			int newsize = vector1.Length+1;
			Neurons.Neuron[] tempVec1 = new Neurons.Neuron[newsize];
			double[] tempVec2 = new double[newsize];
			int[] tempVec3 = new int[newsize];
			for (int i = 0; i < vector1.Length; i++) {
				tempVec1[i] = vector1[i];
				tempVec2[i] = vector2[i];
				tempVec3[i] = vector3[i];
			}
			tempVec1[vector1.Length] = num1;
			tempVec2[vector1.Length] = num2;
			tempVec3[vector1.Length] = num3;
			vector1 = tempVec1;
			vector2 = tempVec2;
			vector3 = tempVec3;
		}
		//-------------------------------------------------
		
		public void AddCell(ref Neurons.Neuron[] vector1,Neurons.Neuron num1,
		                    ref int[] vector2,int num2,
		                    ref int[] vector3,int num3)
		{
			int newsize = vector1.Length+1;
			Neurons.Neuron[] tempVec1 = new Neurons.Neuron[newsize];
			int[] tempVec2 = new int[newsize];
			int[] tempVec3 = new int[newsize];
			for (int i = 0; i < vector1.Length; i++) {
				tempVec1[i] = vector1[i];
				tempVec2[i] = vector2[i];
				tempVec3[i] = vector3[i];
			}
			tempVec1[vector1.Length] = num1;
			tempVec2[vector1.Length] = num2;
			tempVec3[vector1.Length] = num3;
			vector1 = tempVec1;
			vector2 = tempVec2;
			vector3 = tempVec3;
		}
		//-------------------------------------------------
		
		public void AddCells(ref int[] vector,ref int[] num)
		{
			int size = vector.Length;
//			int count=0;
//			Array.Resize(ref vector,vector.Length+num.Length);
//			for (int i = size ; i < vector.Length ; i++) {
//				vector[i] = num[count];
//				count++;
//			}

			int[] tempVec = new int[vector.Length+num.Length];
			vector.CopyTo(tempVec,0);
			for (int i = 0 ; i < num.Length ; i++) {
				tempVec[vector.Length+i] = num[i];
			}
			vector = tempVec;
		}
		//-------------------------------------------------
		
		public void AddCells(ref int[] vector,ref int[] num,ref int[] places)
		{
			Array.Sort(places);
			int[] tempVec = new int[vector.Length+places.Length];
			int counter=0,t=0,size=places.Length-1;
			for (int i = 0; i < tempVec.Length; i++) {
				if (i==places[t]){
					tempVec[i] = num[t];
					if(t<size)t++;
				}else{
					tempVec[i] = vector[counter];
					counter++;
				}
			}
			vector = tempVec;
		}
		//-------------------------------------------------
		
		public void AddCells(ref int[] vector,int num,ref int[] places)
		{
			Array.Sort(places);
			int[] tempVec = new int[vector.Length+places.Length];
			int counter=0,t=0,size=places.Length-1;
			for (int i = 0; i < tempVec.Length; i++) {
				if (i==places[t]){
					tempVec[i] = num;
					if(t<size)t++;
				}else{
					tempVec[i] = vector[counter];
					counter++;
				}
			}
			vector = tempVec;
		}
		//-------------------------------------------------
		
		public void DelCell(ref int[] vector,int place)
		{
			int[] tempVec = new int[vector.Length-1];
			int tempCount=0;
			for (int i = 0 ; i < vector.Length  ; i++ ) {
				if (i==place) continue;
				else
				{
					tempVec[tempCount]=vector[i];
					tempCount++;
				}
			}
			vector = new int[tempVec.Length];
			tempVec.CopyTo(vector,0);
		}
		//-------------------------------------------------
		
		public void DelCells(ref int[] vector,int[] places)
		{
			Array.Sort(places);
			int[] tempVec = new int[vector.Length-places.Length];
			int tempCount=0,countPlaces=0;
			for (int i = 0 ; i < tempVec.Length  ; i++ ) {
				if (countPlaces<places.Length && i==places[countPlaces]) {countPlaces++; continue;}
				else
				{
					tempVec[tempCount]=vector[i];
					tempCount++;
				}
			}
			vector = new int[tempVec.Length];
			tempVec.CopyTo(vector,0);
		}
		//-------------------------------------------------
		
		public void delNum(ref int[] vector,int num)
		{
			if (vector.Length==0) return;
			int sum=0,counter=0;
			for (int i = 0 ; i < vector.Length ; i++ )
				if (num==vector[i]) sum++;
			
			int[] tempVec = new int[vector.Length-sum];
			for (int i = 0 ; i < vector.Length ; i++ )
				if (num!=vector[i]) {
				tempVec[counter]=vector[i];
				counter++;
			}
			vector = new int[tempVec.Length];
			tempVec.CopyTo(vector,0);
		}
		//-------------------------------------------------
		
		public void delNums(ref int[] vector,int[] num)
		{
			if (vector.Length==0) return;
			int sum=0,counter=0;
			
			for (int i = 0 ; i < vector.Length ; i++ )
				for (int t = 0; t < num.Length; t++)
					if (num[t]==vector[i]) sum++;
			
			int[] tempVec = new int[vector.Length-sum];
			for (int i = 0 ; i < vector.Length ; i++ ){
				int flag = 0;
				for (int t = 0; t < num.Length; t++)
					if (num[t]==vector[i]) flag = 1;
				if (flag==0){
					tempVec[counter]=vector[i];
					counter++;
				}
			}
			vector = new int[tempVec.Length];
			tempVec.CopyTo(vector,0);
		}
		//-------------------------------------------------
		
		
		
		public void replicateVectorWithoutNum(ref int[] sourceVector,ref int[] targetgVector,int num)
		{
			int sum=0,counter=0;
			for (int i = 0 ; i < sourceVector.Length ; i++ )
				if (num==sourceVector[i]) sum++;
			
			targetgVector = new int[sourceVector.Length-sum];
			
			for (int i = 0 ; i < sourceVector.Length ; i++ )
				if (num!=sourceVector[i]) {
				targetgVector[counter]=sourceVector[i];
				counter++;
			}
		}
		//-------------------------------------------------
		
		public void replicateVectorWithoutNum(ref int[] sourceVector,ref int[] targetgVector,int num,int from,int until)
		{
			int sum=0,counter=0;
			for (int i = from ; i < until ; i++ )
				if (num==sourceVector[i]) sum++;
			
			targetgVector = new int[(until-from)-sum];
			
			for (int i = from ; i < until ; i++ )
				if (num!=sourceVector[i]) {
				targetgVector[counter]=sourceVector[i];
				counter++;
			}
		}
		//-------------------------------------------------
		
		public void replicateVector(ref int[] sourceVector,ref int[] targetgVector,int from,int until)
		{
			targetgVector = new int[until-from];
			for (int i = from ; i < until ; i++ )
				targetgVector[i] = sourceVector [i];
		}
		//-------------------------------------------------
		
		public bool find(ref int[] Matrix,int number)
		{
			for (int j = 0 ; j < Matrix.Length ; j++ ) {
				if (number == Matrix[j]) return true;
			}
			return false;
		}
		
		//-----------------------------------------------------
		public bool VecContainVec(ref int[] Matrix,ref int[] number)
		{
			if ((Matrix.Length==0)&&(number.Length==0)) return true;
			if (Matrix.Length==0) return false;
			if (number.Length==0) return false;
			for (int i = 0; i < number.Length ; i++) {
				int flag=0;
				for (int j = 0 ; j < Matrix.Length ; j++ ) {
					if (number[i] == Matrix[j])
					{ flag=1; break;}
				}
				if (flag==1) continue;
				else return false;
			}
			return true;
		}
		
		//-----------------------------------------------------
		
		public int[] FindIndex(ref int[] source, int num)
		{
			int[] arr = new int[0];
			for (int i = 0; i < source.Length; i++)
				if (source[i]==num) AddCell(ref arr,i);
			return arr;
		}
		
		//-------------------------------------------------
		
		public void Shift(ref int[] arr, int num)
		{
			int[] temp = new int[num];
			for (int i = 0; i < num ; i++) temp[i] = arr [i];
			int count = num, index = arr.Length-num;
			for (int i = 0; i < index ; i++) {arr[i] = arr[count]; count++;}
			count=0;
			for (int i = index ; i < arr.Length ; i++){ arr[i] = temp[count]; count++;}
		}
		
	}
	
}
