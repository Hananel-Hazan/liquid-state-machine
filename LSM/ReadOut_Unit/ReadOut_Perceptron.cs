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
using System.Collections.Generic;


namespace NN_Pr
{
	/// <summary>
	/// Description of Perceptron.
	/// </summary>
	
	public class Perceptron
	{
		double[] weights;
		public double learningRate,globalError;
		public int maxIteration;
		
		public Perceptron(int Inputsize,ref globalParam Param){
			System.Console.WriteLine("Perceptron Inizilaize.... ");
			this.weights = new double[Inputsize]; for(int i=0; i<Inputsize; i++){	this.weights[i]=Param.rnd.NextDouble();  }
			this.globalError=0;
			this.learningRate=0.05;
			this.maxIteration=250;
			System.Console.WriteLine("Done.");
		}
		
		public double Train(List<Pattern> patterns){
			System.Console.WriteLine("Perceptron Training.... ");
			double iterError = 0;
			int iteration = 0;
			do{
				iterError = 0;
				for (int p = 0; p < patterns.Count; p++)
				{
					double[] inputs = patterns[p].Inputs;
					double outputs = patterns[p].Outputs[0];
					// Calculate error.
					double localError = outputs - this.Compute(inputs);
					if (localError != 0)
					{
						// Update weights.
						for (int i = 0; i < this.weights.Length; i++)
						{
							this.weights[i] += this.learningRate * localError * inputs[i];
						}
					}
					// Convert error to absolute value.
					iterError += Math.Abs(localError);
				}

//				Console.WriteLine("Iteration {0}\tError {1}", iteration, iterError);
				iteration++;
				
			} while ((iterError != this.globalError)&&(iteration!=this.maxIteration));
			
			System.Console.WriteLine("Finish Training ");
			return iterError;
		}
		
		public double Sigmoid(double x)
		{
			return 1 / (1 + Math.Exp(-x));
		}
		
		public int Function(double x)
		{
			return (x >= 0) ? 1 : -1;
		}

		public double Compute(double[] input){
			
			double sum=0;
			for ( int i=0; i<input.Length ; i++) {
				sum += input[i]*this.weights[i];
			}
			//return this.Sigmoid(sum);
			return this.Function(sum);
		}
	}
	
	
	public class Pattern
	{
		private double[] inputs;
		private bool[] Binputs;
		private double[] outputs;
		
		public Pattern(double[] inputs,double[] outputs)
		{
			this.inputs=inputs;
			this.outputs=outputs;
		}
		
		public Pattern(bool[] inputs,double[] outputs)
		{
			this.Binputs=inputs;
			this.outputs=outputs;
		}
		
		public int MaxOutput
		{
			get
			{
				int item = -1;
				double max = double.MinValue;
				for (int i = 0; i < Outputs.Length; i++)
				{
					if (Outputs[i] > max)
					{
						max = Outputs[i];
						item = i;
					}
				}
				return item;
			}
		}
		
		public double[] Inputs
		{
			get { return inputs; }
		}
		
		public double[] Outputs
		{
			get { return outputs; }
		}
	}
}
