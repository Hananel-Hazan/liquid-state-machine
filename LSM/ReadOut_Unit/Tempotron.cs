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
using Neurons;

/// <summary>
/// Description of Class1.
/// </summary>
public class Tempotron
{
	LIF model;
	int  globalInputs;
	double[] wights;
	double changingRate;
	
	public struct DataSet{
		public bool[,]	input;
		public bool IdealOutput;
	}
	public DataSet[] TrainSet;
	
	public Tempotron(int numberOfInputs)
	{
		this.model = new LIF();
		globalInputs = numberOfInputs;
		wights = new double[numberOfInputs];
		
		changingRate = 0.1;
		
		this.Rest();
	}
	//===============
	
	
	public void Rest(){
		for (int i = 0; i < globalInputs; i++)
			wights[i] = changingRate ;
		this.model.reset();
		
		TrainSet = new DataSet[0]{};
	}
	//===============
	
	public void CollectData(ref bool[,] TrainingSet, ref double TargetSet){
		int lengh = TrainSet.Length;
		DataSet[] Temp = new DataSet[lengh+1];
		for (int i = 0; i < lengh ; i++) {
			Temp[i] = new DataSet();
			Temp[i].input = (bool[,]) TrainSet[i].input.Clone();
			Temp[i].IdealOutput = TrainSet[i].IdealOutput;
		}
		Temp[lengh].input = (bool[,]) TrainingSet.Clone();
		if (TargetSet>0)
			Temp[lengh].IdealOutput = true;
		else
			Temp[lengh].IdealOutput = false;
		
		TrainSet = Temp;
	}
	
	//===============
	
	public double Learn(){
		int epocs = 10000; //Param.detector.ReadoutU_epoch;
		if (TrainSet.Length ==0 ) return double.MaxValue;
		
		int lengh = TrainSet.Length,
		neurons = TrainSet[0].input.GetLength(1),
		Time = TrainSet[0].input.GetLength(0);
		
		Console.WriteLine("Start Trmpotron Learing");
		int loop=0, correction=0, minCorrection = lengh/2;
		double[] tempWights = new double[wights.Length];
		double fixRatio =  changingRate;
		while(loop<epocs){
			correction = 0;
			for (int sample = 0; sample < lengh ; sample++) {
				double[] voltOutput;
				int output = 0;
				int Iteration_of_MaxActivity = Run(ref TrainSet[sample].input,out voltOutput, ref output);
				if ( output == 0 && TrainSet[sample].IdealOutput == true) {
					correction++;
					if (output==0)
						fix(ref TrainSet[sample].input,ref TrainSet[sample].IdealOutput, +1 ,Time,fixRatio);
					else
						fix(ref TrainSet[sample].input,ref TrainSet[sample].IdealOutput, +1 ,Iteration_of_MaxActivity,fixRatio);
				} else if ( output > 0 && TrainSet[sample].IdealOutput == false) {
					correction++;
					fix(ref TrainSet[sample].input,ref TrainSet[sample].IdealOutput, -1, Iteration_of_MaxActivity,fixRatio);
				}
			}
			loop++;
			
			if (minCorrection>correction){
				minCorrection = correction;
				wights.CopyTo(tempWights,0);
			}
			if (correction==0) break;
			
			if (loop%200==0){
				fixRatio = fixRatio/2;
				if (minCorrection<correction){
					correction = minCorrection;
					tempWights.CopyTo(wights,0);
				}
			}
			
			
		}
		if (minCorrection<correction){
			correction = minCorrection;
			tempWights.CopyTo(wights,0);
		}
		
		Console.WriteLine("loop {0} correction {1}",loop,correction);
		
		return 1-((1.0*correction)/(lengh/2.0));
	}
	//===============
	public void fix(ref bool[,] sample, ref bool IdealOutput, int correction, int Iteration_of_MaxActivity,double fixRatio){
		
		int neurons = sample.GetLength(1),
		Time = sample.GetLength(0);
		
		Iteration_of_MaxActivity++;
		if (Iteration_of_MaxActivity>Time) Iteration_of_MaxActivity = Time;
		
		model.reset();
		LIF temp_model = new LIF();
		
		for (int n = 0; n < neurons; n++) {
			double change = 0;
			temp_model.reset();
			double[] outV  = new double[Iteration_of_MaxActivity];
			for (int t = 0; t < Iteration_of_MaxActivity ; t++){
				if (sample[t,n])
					temp_model.step(ref change, wights[n] ,0);
				else
					temp_model.step(ref change, 0 ,0);
				outV[t] =change;
			}
			if (change!=0)
				wights[n] += correction * (fixRatio * Math.Abs(model.therashold - change));
		}
		
	}
	
	//===============
	public int Run(ref bool[,] TestSet, out double[] outputVolt,ref int NumOfFiring){
		model.reset();
		int Iteration_of_MaxActivity = 0;
		double activity=0;
		NumOfFiring = 0;
		int neurons = TestSet.GetLength(1),
		Time = TestSet.GetLength(0);
		outputVolt = new double[Time];
		for (int t = 0; t < Time; t++) {
			double input = 0;
			for (int neuron = 0; neuron < neurons ; neuron++)
				if (TestSet[t,neuron] == true)
					input += wights[neuron];
			if(model.step(ref outputVolt[t],input,0))
				NumOfFiring++;
			if (outputVolt[t]>activity){
				activity = outputVolt[t];
				Iteration_of_MaxActivity = t;
			}
		}
		return Iteration_of_MaxActivity;
	}
	//===============

}

public class LIF
{
	//----------------------------------------------------------------
	//                    init ver
	//----------------------------------------------------------------
	double initDecay;
	double decay;
	double decayFactor;
	public double V,initV,therashold,initTherashold;
	public double Output;
	bool refactory_piriod;
	//----------------------------------------------------------------

	public LIF()
	{
		this.reset();
	}

	//----------------------------------------------------------------
	public void reset()
	{
		this.initV = 0;
		this.V = initV;
		this.initDecay = 1.4 ;
		this.decayFactor = 1.4 ;
		this.decay = initDecay;
		this.initTherashold = 1;
		this.therashold = this.initTherashold;
		this.Output = 0;
		refactory_piriod = false;
	}

	//----------------------------------------------------------------
	public bool stepORG(ref double retuenV,double InternalInput, double ExternalIntput)
	{
		this.Output = this.V;
		bool spike=false;

		if (refactory_piriod==false){
			if (this.V >= this.initV){
				if ((InternalInput > 0) || (ExternalIntput > 0)){
					this.V += InternalInput + ExternalIntput;
					this.decay = this.initDecay;
					if (this.V >= this.therashold){
						spike=true;
						refactory_piriod = true;
					}
				}else{
					this.V -= (this.V * this.decay) + 0.05;
					if (this.decay > 0.99) this.decay = 0.99;
					else this.decay *= this.decayFactor;
					// if there is no input the V is becoume minus BIG BIG number almost infinity
					if (this.V < this.initV) this.V = this.initV;
				}
			}
		}else{ // refactory piriod
			if (this.V < this.initV){
				this.V /= this.initDecay;
				if (this.V >= this.initV) {
					refactory_piriod = false;
					this.V = this.initV;
				}
			}else if (this.V >= this.therashold){
				this.V = -1 ;
				this.decay = this.initDecay;
			}
		}
		this.Output = this.V;
		retuenV = this.Output;
		return (spike);
	}
	
	public bool step(ref double retuenV,double InternalInput, double ExternalIntput)
		{
			this.Output = this.V;
			bool spike=false;

			if (this.V >= this.therashold){
				spike=true;
				this.Output = this.V;
				this.V = -0.5 ;
				this.decay = this.initDecay;
			}else if (this.V >= this.initV){
				if ((InternalInput > 0) || (ExternalIntput > 0)){
					this.V += InternalInput + ExternalIntput;
					this.decay = this.initDecay;
					this.Output = this.V;
				}else{
					this.V -= this.V * this.decay;
					if (this.decay > 0.99) this.decay = 0.99;
					else this.decay *= this.decayFactor;
					
					// if there is no input the V is becoume minus BIG BIG number almost infinity
					if (this.V < this.initV) this.V = this.initV;
					this.Output = this.V;
				}
				
			}else if (this.V < this.initV){
				this.V /= this.initDecay;
				this.decay = this.initDecay;
				this.Output = this.V;
			}
			retuenV = this.Output;
			return (spike);
		}

}
