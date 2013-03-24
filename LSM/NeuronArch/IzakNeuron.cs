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
	// Iimplementation of Izhikevich Model
	public class IzakNeuron : NeuronArch.Unit
	{
		//----------------------------------------------------------------
		//					init ver
		//----------------------------------------------------------------
		//        public double V;
		double a,b,c,d,U;
		public double Output;
		
		//----------------------------------------------------------------

		public IzakNeuron(ref NeuronParametes Param)
		{
			this.initTherashold = Param.Neuron_Threshold; // init value the Nueron shold reset this!
			this.reset(ref Param);
		}

		//----------------------------------------------------------------
		/*  http://senselab.med.yale.edu/modeldb/ShowModel.asp?model=39948&file=\izh\izh.mod
        a        b       c      d       I
================================================================================
      0.02      0.2     -65     6      14       % tonic spiking
      0.02      0.25    -65     6       0.5     % phasic spiking
      0.02      0.2     -50     2      15       % tonic bursting
      0.02      0.25    -55     0.05    0.6     % phasic bursting
      0.02      0.2     -55     4      10       % mixed mode
      0.01      0.2     -65     8      30       % spike frequency adaptation
      0.02     -0.1     -55     6       0       % Class 1
      0.2       0.26    -65     0       0       % Class 2
      0.02      0.2     -65     6       7       % spike latency
      0.05      0.26    -60     0       0       % subthreshold oscillations
      0.1       0.26    -60    -1       0       % resonator
      0.02     -0.1     -55     6       0       % integrator
      0.03      0.25    -60     4       0       % rebound spike
      0.03      0.25    -52     0       0       % rebound burst
      0.03      0.25    -60     4       0       % threshold variability
      1         1.5     -60     0     -65       % bistability
      1         0.2     -60   -21       0       % DAP
      0.02      1       -55     4       0       % accomodation
     -0.02     -1       -60     8      80       % inhibition-induced spiking
     -0.026    -1       -45     0      80       % inhibition-induced bursting
		 */
		override public void reset(ref NeuronParametes Param)
		{
			this.V = -65;
			this.a = 0.01;
			this.b = 0.2;
			this.c = -55;
			this.d = 4 ;
//			this.I = 10;
			this.U = this.b * V;
			this.Output = 0;
			this.therashold = this.initTherashold;

		}

		//----------------------------------------------------------------
		override public bool step(ref double retuenV, double InternalInput, double ExternalIntput)
		{

			bool spike=false;
			this.Output = 0;
			
			
			if (this.V >= this.therashold){
				internal_Refactory = true;
				spike=true;
				this.Output = this.V;
				this.V = this.c;
				this.U += this.d;
			}else if (this.V > this.initV)
				internal_Refactory = false;
			
			this.V += ((this.V * (0.04 * this.V + 5)) + 140) - this.U + InternalInput + ExternalIntput;
			
			this.U += this.a * (this.b * this.V - this.U);
			

			retuenV = this.Output;
			return (spike);
		}
	}
}

