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
	public class Combinator
	{
		int[] numOFgroups,selectGroup;
		int max;
		public Combinator(int[] Group)
		{
			this.numOFgroups = Group;
			this.max = Group[0];
			for(int i=0 ; i<Group.Length  ; i++){ max=Math.Max(Group[i],max); }
		}
		
		public static Int64 factorial(int i)
		{
			return((i <= 1) ? 1 : (i * factorial(i-1)));
		}
		
		void next(int begin)
		{
			if (begin==0){
				for (int i=0 ; i<this.selectGroup.Length ; i++ )
					this.selectGroup[i]=i; // "pointer" to the cell in the array
			}else{
				for (int i=selectGroup.Length-1 ; i > -1 ; i--) {
					if (this.numOFgroups.Length-1>this.selectGroup[i]) {
						int flag=0;
						this.selectGroup[i]++;
						for(int t=i+1; t<selectGroup.Length ; t++)
							if (this.selectGroup[t-1]+1<=this.max) {	this.selectGroup[t] = this.selectGroup[t-1]+1;
						}else{
							flag =1 ; continue;}
						if (flag==0)	break;
					}
					
				}
			}
			
		}
		
		public int[,] Select(int NumOFgroupsTOSelect)
		{
			/// return the all combination of chosen n from m group
			/// 
			int[,] output = new int[0,0];
			// if select number is bigger then the group number EXIT
			if (NumOFgroupsTOSelect>this.numOFgroups.Length) return output;
			
			this.selectGroup = new int[NumOFgroupsTOSelect];
			
			output = new int[factorial(this.numOFgroups.Length)/(factorial(NumOFgroupsTOSelect)*factorial(this.numOFgroups.Length-NumOFgroupsTOSelect)),NumOFgroupsTOSelect];
			
			int counter=0;
			
			for (int frac=0 ; frac < output.Length ; frac++){
				int mone=1;
				while (mone>0) {
					mone=0;
					this.next(frac);
					for(int i = 0 ; i<NumOFgroupsTOSelect ; i++){output[counter,i]=this.numOFgroups[this.selectGroup[i]];}
				}
				counter++;
			}
			return output;
		}
		
		public int[,] fill(int[,] one)
		{
			int x = one.GetLength(0), y = one.GetLength(1);
			/// The opposite of sellect..
			/// retun the other groups that "sellect" didnt choose
			int[,] output = new int[x,this.numOFgroups.Length-y];
			
			for(int i = 0; i<x ; i++)
			{
				int mone=0,moneOut=0;
				for (int t=0 ; t<this.numOFgroups.Length ; t++) {
					if (this.numOFgroups[t]==one[i,mone]) {if (mone<x-1)mone++;}
					else {output[i,moneOut] = this.numOFgroups[t]; moneOut++;}
				}
			}
			
			
			return output;
		}

	}

}