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

namespace NeuronArch
{
	
	[Serializable()]
	public class McCulloch_Pitts : NeuronArch.Unit
	{
		//----------------------------------------------------------------
		//					init ver
		//----------------------------------------------------------------
		public double Output,Decay;
		double spike;
		
		public McCulloch_Pitts(ref globalParam Param)
		{
			this.initTherashold = Param.neuronParam.Neuron_Threshold; // init value the Nueron shold reset this!
			this.reset(ref Param);
		}
		//----------------------------------------------------------------
		
		override public void reset(ref globalParam Param)
		{
			this.initV =  Param.neuronParam.initV;
			this.V = this.initV;
			this.therashold = this.initTherashold;
			this.Output  = 0 ;
			this.spike = Param.neuronParam.Int_Neuron_Spike;
			this.Decay = Param.neuronParam.FiringDecayFactor;
//			this.flag = 0;
//			this.flag2 =0;

		}
		//----------------------------------------------------------------
		
		override public bool step(ref double retuenV,double InternalInput, double ExternalIntput)
		{
			this.Output = 0 ;
			bool spike=false;
			
//			if (flag>0){
//				flag--;
//			}else{
//				if ((InternalInput>=this.therashold)||(ExternalIntput)>0)
//				{ this.Output = this.spike; flag2++; flag = flag2;}
//				else flag2 = (flag2>0)?flag2-1 : flag2;
//			}
			
			if  (InternalInput+ExternalIntput>=this.therashold){
				this.Output = this.spike;
				spike = true;
			}
			
			
			this.V = this.Output;
			retuenV = this.Output;
			return (spike);
		}
		//----------------------------------------------------------------

	}
}

