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
	public class LIFNeuron : NeuronArch.Unit
	{
		//----------------------------------------------------------------
		//                    init ver
		//----------------------------------------------------------------
		double refractoryV;
		public double Output;
		//----------------------------------------------------------------

		public LIFNeuron(ref globalParam Param)
		{
			this.initTherashold = Param.neuronParam.Neuron_Threshold;
			this.init_FiringDecayFactor = Param.neuronParam.FiringDecayFactor;
			this.init_RefractoryDecayFactor = Param.neuronParam.RefractoryDecayFactor;
			this.initV = Param.neuronParam.initV + (Param.rndA.NextDouble(ref Param,-20,+60));
			this.refractoryV = this.initV + (Param.rndA.NextDouble(ref Param,-20,0));
			this.reset(ref Param);
		}

		//----------------------------------------------------------------
		override public void reset(ref globalParam Param)
		{
			this.V = initV;
			this.FiringDecayFactor = this.init_FiringDecayFactor ;
			this.RefractoryDecayFactor = this.init_RefractoryDecayFactor;
			this.therashold = this.initTherashold;
			this.Output = 0;
			this.internal_Refactory = false;
		}

		//----------------------------------------------------------------
		override public bool step(ref double retuenV,double InternalInput, double ExternalIntput)
		{
			
			bool spike=false;
			
			if (internal_Refactory) {// Input + Decay
				// Refactoring
				this.V += 1 + (this.RefractoryDecayFactor * (this.initV - this.V )) ;
				if (this.V >= this.initV)
					internal_Refactory = false;
			}else{
				if (this.V >= this.therashold) {// Emit Spike
					spike=true;
					this.V = this.refractoryV;
					internal_Refactory = true;
				}else{
					this.V +=  (InternalInput+ExternalIntput) + (this.FiringDecayFactor * (this.initV - this.V));
				}
			}
			retuenV = this.V;
			return (spike);
		}
	}

}
