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
	public class LIFmodle2 : NeuronArch.Unit
	{
		//----------------------------------------------------------------
		//                    init ver
		//----------------------------------------------------------------
		double refactoryV;
		double iniV_forRefactory;

//		double RandomA;
//		double RandomB;
		
		Random rand;
		double energy;
		
//		double inputRate,previousInput;
//		double energy_fact;
//		double inputLeft;
		//----------------------------------------------------------------

		public LIFmodle2(ref globalParam Param)
		{
			this.initTherashold = Param.neuronParam.Neuron_Threshold; // init value the Nueron shold reset this!
			rand =  new Random();
//			RandomA = rand.NextDouble();
//			RandomB = 0;//rand.NextDouble();
			this.init_decayRate = Param.neuronParam.decayFactor;
			this.refactoryV = this.initV - 30;
			
			this.reset(ref Param);
		}

		//----------------------------------------------------------------
		override public void reset(ref globalParam Param)
		{
			this.initV = Param.neuronParam.initV;
			this.V = initV;
			this.iniV_forRefactory = Param.neuronParam.initV+ Math.Abs(Param.neuronParam.initV*0.1);
			this.therashold = this.initTherashold;
			this.decayRate = this.init_decayRate;
			
			this.energy = 0.9; // start on 1=100% energy
			this.internal_Refactory = false;
			
//			this.inputRate = 0.5;
//			this.inputLeft = 0;
//			this.previousInput = 0;
//			this.energy_fact = 1;
			
		}
		
		
		//----------------------------------------------------------------
		override public bool step(ref double retuenV,double InternalInput, double ExternalIntput)
		{
			//this.Output = 0;
			bool spike=false;
			
			if (internal_Refactory) {// Input + Decay
				// Refactoring
				this.V += 1 + (this.energy * this.decayRate * (this.initV - this.V )) ;
				
				if (this.V >= this.initV)
					internal_Refactory = false;
//				if (this.energy<1)
//					this.energy += (1-this.energy)*this.refactoryRate;
//				if (this.energy_fact>0) this.energy_fact/=1.15;
			}else{
				if (this.V >= this.therashold) {// Emit Spike
					spike=true;
					this.V = this.refactoryV;
					internal_Refactory = true;
//				this.energy-=this.energy_fact/100;
//				if (this.energy_fact<100) this.energy_fact*=1.15;
//				if ( this.energy<=0) this.energy=0;
					if (this.energy>0)
						this.energy -= (1-this.energy) *this.decayRate;
				}else{
//					double tempInput=InternalInput+ExternalIntput;
//					if (tempInput!=0){
//						this.V += tempInput;
//					if (this.V < this.refactoryV) this.V = this.refactoryV;
//					}else{ // No input at all -> Decay!
					
//					int temp = 5;
//					if ((this.initV - this.V)<0)
//						temp *=-1;
//					this.V +=  temp + (this.decayRate * (this.initV - this.V));
//					this.V +=  temp + ((1-this.energy)*(this.decayRate * (this.initV - this.V)));
//						this.V +=  (this.decayRate * (this.initV - this.V));
					this.V += (InternalInput+ExternalIntput) + (this.decayRate * (this.initV - this.V));
					
//					this.energy+= this.energy*this.refactoryRate;
//					if (this.energy_fact>0) this.energy_fact/=2.15;
					if (this.energy<1)
						this.energy += (1-this.energy)*this.decayRate;
//					}
				}
			}
			retuenV = this.V;
			return (spike);
		}


		
		//--------------------------------------------------------------------------------------
		/// Importent Note!!!
		///		in the old version of the neuron, there is an issue with neuron that recive NEGATIVE input
		///		it cause the neuron to get in a Refactory piriad and this is WRONG!!
		///		ONLY when fire! the neuron go in Refactory piriod
		///		before using the old code, please correct it!
		//--------------------------------------------------------------------------------------


//		//----------------------------------------------------------------
//		override public bool step(ref double retuenV,double InternalInput, double ExternalIntput)
//		{
//			bool spike=false;
//
//			if (this.V >= this.therashold) {// Emit Spike
//				spike=true;
//
//				this.V = this.refactoryV;
//				if (this.energy > 0.01) this.energy -= 0.1;
//				this.therashold+=RandomA;
//
//			}else if (this.V >= this.initV) { // Input + Decay
//				double tempInput=InternalInput+ExternalIntput;
//				if (tempInput != 0)
//					this.V += tempInput;
//
//				else{ // No input at all -> Decay!
//
//					if (this.V > this.initV)
//						this.V -= Math.Round(this.decayRate * (this.V - iniV_forRefactory ),1);
//
//					if (this.energy < 0.99) this.energy += 0.01;
//					this.therashold-=RandomB;
//				}
//
//			}else{ // Refactoring
//				this.V -= Math.Round(this.energy * (this.V - this.iniV_forRefactory),1);
//				this.energy += 0.01;
//			}
//			retuenV = this.V;
//			return (spike);
//		}
		
		
//		//----------------------------------------------------------------
//		override public bool step(ref double retuenV,double InternalInput, double ExternalIntput)
//		{
//			//this.Output = 0;
//			bool spike=false;
//
//			if (this.V >= this.therashold) {// Emit Spike
//				spike=true;
//				//this.Output = this.V+this.inputLeft;
//				this.V = this.refactoryV;
//
//				this.energy-=this.energy_fact/100;
//				if (this.energy_fact<100) this.energy_fact*=1.15;
//				if ( this.energy<=0) this.energy=0;
//
//			}else if (this.V >= this.initV) {// Input + Decay
//				double tempInput=InternalInput+ExternalIntput;
//
//				if ((tempInput+this.V)>this.therashold) {
//					this.V = this.V + tempInput;
//				}else if ((tempInput+this.inputLeft>=0.1)||(tempInput+this.inputLeft<=-0.1)) {
//					//if the New input is bigger then the previous, then increase the volt by the DELTA
//					//if the New input is smaller then the previous, then decrease the volt by the DELTA
//					//if the New input == the previous then there is no DELTA
//
//					this.inputLeft+=(tempInput-this.previousInput);
		////					this.previousInput=0.55*(tempInput+this.previousInput);
//
//					double tempV = this.V + this.inputLeft;
//					this.V += (this.inputRate==0)? this.inputLeft:this.inputRate * this.inputLeft;
//					this.inputLeft = tempV - this.V;
//
//				}else{ // No input at all -> Decay!
//					this.V += this.inputLeft + (this.decayRate * ((this.initV) - this.V));
//					this.inputLeft=0;
//					this.previousInput -=  this.decayRate * (this.previousInput);
//
//					this.energy+=(1-this.energy)*this.refactoryRate;
//					if (this.energy_fact>0) this.energy_fact/=2.15;
//				}
//
//			}else if (this.V < this.initV){ // Refactoring
//				this.V -= Math.Round(this.energy * this.refactoryRate * (this.V - (this.initV+1)),1);
//				this.inputLeft = 0 ;
//				this.previousInput = 0;
//				if ( this.V>=this.initV ) this.V=this.initV;
//
//			}else{
//				// Not supose to come here, If it does, there is a problem
//				System.Console.WriteLine(" OOOOOPPPPPPPSSSSSSSS");
//			}
//
//			retuenV = this.V;
//			return (spike);
//		}


	}
}
