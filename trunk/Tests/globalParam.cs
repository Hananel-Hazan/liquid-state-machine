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
using Utils_Functions;
using System.Collections.Generic;
using Neurons;


[Serializable()]
public class Input2liquid
{
	virtual public int ComputeInputTime(int Input_time){
		
		return (Looping_Input * LSM_Ratio_Of_Input_Interval[1] * Input_time);
	}
	public int[] LSM_Ratio_Of_Input_Interval;
	public int silence_or_repeted_Input_between_ratio;
	public int Looping_Input;
	public double[] is_a_live_signal;
	public double[] inverse_Signal;
	
	public void parsing(ref Dictionary<string, string> dictionary){
		string temp;
		string[] words;
		
		dictionary.TryGetValue("silence_or_repeted_Input_between_ratio",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-silence_or_repeted_Input_between_ratio");
		silence_or_repeted_Input_between_ratio = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Looping_Input",out temp);
		Looping_Input = Convert.ToInt32(temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Looping_Input");
		
		dictionary.TryGetValue("LSM_Ratio_Of_Input_Interval",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-LSM_Ratio_Of_Input_Interval");
		words = temp.Split(',');
		LSM_Ratio_Of_Input_Interval = new int[words.Length];
		LSM_Ratio_Of_Input_Interval[0] = Convert.ToInt32(words[0]);
		LSM_Ratio_Of_Input_Interval[1] = Convert.ToInt32(words[1]);
		
		dictionary.TryGetValue("is_a_live_signal",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-is_a_live_signal");
		words = temp.Split(',');
		is_a_live_signal = new double[words.Length];
		is_a_live_signal[0] = Convert.ToDouble(words[0]);
		is_a_live_signal[1] = Convert.ToDouble(words[1]);
		
		dictionary.TryGetValue("inverse_Signal",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-inverse_Signal");
		words = temp.Split(',');
		inverse_Signal = new double[words.Length];
		inverse_Signal[0] = Convert.ToDouble(words[0]);
		inverse_Signal[1] = Convert.ToDouble(words[1]);
	}
}
//-----------------------------------------------------------------------------------

[Serializable()]
public class NeuronParametes
{
	virtual public void initNueronMaxParameters(int a){}
	
	public int Neuron_Model;
	public double Int_Neuron_Spike;
	public double Ext_Inp_Neuron_Spike;
	public double initV;
	public double decayFactor;
	public double Neuron_Firing_Rate_Max,
	Neuron_Slideing_Threshold_Recommended_Firing_Rate_Max,Neuron_Slideing_Threshold_Recommended_Firing_Rate_Min,
	Neuron_STDP_Recommended_Firing_Rate_Max,Neuron_STDP_Recommended_Firing_Rate_Min;
	public int Steps_Between_Two_Spikes;
	public int Steps_Between_Two_Spikes_In_Silence;
	
	
	public int Neuron_Min_Delay;
	public int Neuron_Max_Delay;
	
	public int[] STDPwindow;
	public double[] STDPMaxChange;
	public int Active_STDP_rule;
	
	public int Randomize_initilaization;
	public int Proportional_Threshold;
	public double Neuron_Threshold_Proportion;
	public double Neuron_Threshold;
	public int Slideing_Threshold;
	public int Random_Threshold_On_FullRest;
	public double Random_Factor_Sliding_Treshold;
	
	public void parsing(ref Dictionary<string, string> dictionary){
		string temp;
		string[] words;
		dictionary.TryGetValue("Neuron_Model",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Neuron_Model");
		Neuron_Model = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Neuron_Min_Delay",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Neuron_Min_Delay");
		Neuron_Min_Delay = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Neuron_Max_Delay",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Neuron_Max_Delay");
		Neuron_Max_Delay = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Active_STDP_rule",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Active_STDP_rule");
		Active_STDP_rule = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Randomize_initilaization",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Randomize_initilaization");
		Randomize_initilaization = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Random_Factor_Sliding_Treshold",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Random_Factor_Sliding_Treshold");
		Random_Factor_Sliding_Treshold = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("Random_Threshold_On_FullRest",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Random_Threshold_On_FullRest");
		Random_Threshold_On_FullRest = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("STDPwindow",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-STDPwindow");
		words = temp.Split(',');
		int time = words.Length;
		STDPwindow = new int[time];
		for (int i = 0; i < time; i++) {
			STDPwindow[i] = Convert.ToInt32(words[i]);
		}
		
		dictionary.TryGetValue("STDPMaxChange",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-STDPMaxChange");
		words = temp.Split(',');
		time = words.Length;
		STDPMaxChange = new double[time];
		for (int i = 0; i < time; i++) {
			STDPMaxChange[i] = Convert.ToDouble(words[i]);
		}
		
		if (Neuron_Model==1){
			
			dictionary.TryGetValue("LIF.decayFactor",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-decayFactor");
			decayFactor = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF.Proportional_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.Proportional_Threshold");
			Proportional_Threshold = Convert.ToInt32(temp);

			dictionary.TryGetValue("LIF.Neuron_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.Neuron_Threshold");
			Neuron_Threshold = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF.Neuron_Threshold_Proportion",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.Neuron_Threshold_Proportion");
			Neuron_Threshold_Proportion = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF.Slideing_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.Slideing_Threshold");
			Slideing_Threshold = Convert.ToInt32(temp);
			
			dictionary.TryGetValue("LIF.initV",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.initV");
			initV = Convert.ToDouble(temp);
			
		}else if (Neuron_Model==2) {
			
			dictionary.TryGetValue("Izhikevich.decayFactor",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.decayFactor");
			decayFactor = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("Izhikevich.Proportional_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration");
			Proportional_Threshold = Convert.ToInt32(temp);

			dictionary.TryGetValue("Izhikevich.Neuron_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.Neuron_Threshold");
			Neuron_Threshold = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("Izhikevich.Neuron_Threshold_Proportion",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.Neuron_Threshold_Proportion");
			Neuron_Threshold_Proportion = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("Izhikevich.Slideing_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.Slideing_Threshold");
			Slideing_Threshold = Convert.ToInt32(temp);
			
			dictionary.TryGetValue("Izhikevich.initV",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.initV");
			initV = Convert.ToDouble(temp);
			
		}else if (Neuron_Model==3) {
			
			dictionary.TryGetValue("LIF-H.decayFactor",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.decayFactor");
			decayFactor = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF-H.Proportional_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.Proportional_Threshold");
			Proportional_Threshold = Convert.ToInt32(temp);

			dictionary.TryGetValue("LIF-H.Neuron_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.Neuron_Threshold");
			Neuron_Threshold = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF-H.Neuron_Threshold_Proportion",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.Neuron_Threshold_Proportion");
			Neuron_Threshold_Proportion = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF-H.Slideing_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.Slideing_Threshold");
			Slideing_Threshold = Convert.ToInt32(temp);
			
			dictionary.TryGetValue("LIF-H.initV",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.initV");
			initV = Convert.ToDouble(temp);
			
		}else if (Neuron_Model==4) {
			
			dictionary.TryGetValue("McCulloch-Pitts.decayFactor",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.decayFactor");
			decayFactor = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("McCulloch-Pitts.Proportional_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.Proportional_Threshold");
			Proportional_Threshold = Convert.ToInt32(temp);

			dictionary.TryGetValue("McCulloch-Pitts.Neuron_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.Neuron_Threshold");
			Neuron_Threshold = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("McCulloch-Pitts.Neuron_Threshold_Proportion",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.Neuron_Threshold_Proportion");
			Neuron_Threshold_Proportion = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("McCulloch-Pitts.Slideing_Threshold",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.Slideing_Threshold");
			Slideing_Threshold = Convert.ToInt32(temp);
			
			dictionary.TryGetValue("McCulloch-Pitts.initV",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.initV");
			initV = Convert.ToDouble(temp);
		}
		
		// Input from External MUST be bigger then INT_SPIKE
		this.Int_Neuron_Spike = (Neuron_Threshold-initV) ;
		this.Ext_Inp_Neuron_Spike = this.Int_Neuron_Spike  ;
		//-----------------------------------------------------
		
	}
	
	public void initialization(ref globalParam Param){
		
		int one_Second = Param.detector.LSM_1sec_interval;
		
		//-------------------Prefurm Testing on single neuron------------------------------------
		
		Neuron neu = new Neuron(Neuron_Model,1,ref Param);
		neu.posiORneg=1;
		neu.Input_OR_Output=2;
		neu.reset(ref Param);
		neu.Slideing_Threshold=this.Slideing_Threshold;
		int factor = 1000;
		
		one_Second *=factor;
		int counterA=0;
		Steps_Between_Two_Spikes=0;
		
		bool flip = false;
		Neuron_Firing_Rate_Max = 0 ;
		double[] plot = new double[one_Second];
		for (int i = 0; i < one_Second ; i++) {
			int[] dontcare = new int[3];
			neu.ExternalIntput = Param.neuronParam.Ext_Inp_Neuron_Spike;
			bool spike = neu.step(i,ref dontcare,ref Param);
			plot[i] = neu.Nunit.V;
			if (spike){
				Neuron_Firing_Rate_Max++;
				flip=true;
			}else
				flip=false;
			
			if (flip){
				Steps_Between_Two_Spikes+=counterA;
				counterA=0;
			}else{
				counterA++;
			}
		}
		Steps_Between_Two_Spikes =  2 + (int) Math.Round(((double)Steps_Between_Two_Spikes/Neuron_Firing_Rate_Max),0);
		Neuron_Firing_Rate_Max /= Param.detector.LSM_1sec_interval;
		if (Neuron_Firing_Rate_Max==0) Console.WriteLine("!!! Neuron Initilization = zero !!!");
		Neuron_Slideing_Threshold_Recommended_Firing_Rate_Max = 0.5 * Neuron_Firing_Rate_Max;
		Neuron_Slideing_Threshold_Recommended_Firing_Rate_Min = Math.Min(1,0.01 * Neuron_Firing_Rate_Max);
		Neuron_STDP_Recommended_Firing_Rate_Max = 0.6 * Neuron_Firing_Rate_Max;
		Neuron_STDP_Recommended_Firing_Rate_Min = 0.01 * Neuron_Firing_Rate_Max;
		
		
		counterA=0;
		int counterB = 0;
		if (Param.neuronParam.Slideing_Threshold>0){
			neu.FullReset(ref Param);
			neu.Slideing_Threshold = Param.neuronParam.Slideing_Threshold;
			plot = new double[one_Second];
			for (int i = 0; i < one_Second ; i++) {
				int[] dontcare = new int[3];
				bool spike = neu.step(i,ref dontcare,ref Param);
				plot[i] = neu.Nunit.V;
				if (spike){
					counterB++;
					flip=true;
				}else
					flip=false;
				
				if (flip){
					Steps_Between_Two_Spikes_In_Silence+=counterA;
					counterA=0;
				}else{
					counterA++;
				}
			}
			
			Steps_Between_Two_Spikes_In_Silence = 2 + (int) Math.Round(((double)Steps_Between_Two_Spikes_In_Silence/counterB),0);
		}
		
	}
	
	
	
}
//-----------------------------------------------------------------------------------

[Serializable()]
public class NetworkParm
{
	public int IncreaseSelectedChanceBy;
	public int Number_Of_Neurons;
	public double GeneralConnectivity;
	public int Connections;
	public double Precentege_Of_Slideing_Threshold;

	public int[] Liquid_Architectures;
	public int Liquid_Architecture;
	public int Methods_Of_Liquid_Input_Units;
	public int Liquid_Update_Sync_Or_ASync;
	public double[] synapseStrength_template;
	
	public int Min_Neuron_Connection;
	public int Min_Group_Connections;
	public int Group_size;

	public int Group_size_Min;
	public int Group_size_Max;
	public int[] Group_interconnections;
	
	public int[] Maass_column;
	public int Maass_Lambda;
	
	public int[] GroupSize;
	
	public double LSM_Percent_Of_Negative_Weights;
	public double LSM_Input_Percent_Connectivity;
	public double Liquid_Weights_Resistend;
	public int Neuron_propotional_weight_Update;
	public int Neuron_propotional_weight_Initialize;
	public double LSM_Max_Init_Weight_NegN;
	public double LSM_Min_Init_Weight_NegN;
	public double LSM_Max_Init_Weight_PosN;
	public double LSM_Min_Init_Weight_PosN;
	
	
	public void parsing(ref Dictionary<string, string> dictionary){
		string temp;
		string[] words;
		
		dictionary.TryGetValue("IncreaseSelectedChanceBy",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-IncreaseSelectedChanceBy");
		IncreaseSelectedChanceBy = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Number_Of_Neurons",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Number_Of_Neurons");
		Number_Of_Neurons = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("GeneralConnectivity",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-GeneralConnectivity");
		GeneralConnectivity = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("Precentege_Of_Slideing_Threshold",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Precentege_Of_Slideing_Threshold");
		Precentege_Of_Slideing_Threshold = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("Liquid_Architectures",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Liquid_Architectures");
		words = temp.Split(',');
		int time = words.Length;
		Liquid_Architectures = new int[time];
		for (int i = 0; i < time; i++) {
			Liquid_Architectures[i] = Convert.ToInt32(words[i]);
		}
		
		dictionary.TryGetValue("Methods_Of_Liquid_Input_Units",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Methods_Of_Liquid_Input_Units");
		Methods_Of_Liquid_Input_Units = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Liquid_Update_Sync_Or_ASync",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Liquid_Update_Sync_Or_ASync");
		Liquid_Update_Sync_Or_ASync = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("LSM_Input_Percent_Connectivity",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-LSM_Input_Percent_Connectivity");
		LSM_Input_Percent_Connectivity = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("LSM_Percent_Of_Negative_Weights",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-LSM_Percent_Of_Negative_Weights");
		LSM_Percent_Of_Negative_Weights = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("Liquid_Weights_Resistend",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Liquid_Weights_Resistend");
		Liquid_Weights_Resistend = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("Neuron_propotional_weight_Update",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Neuron_propotional_weight_Update");
		Neuron_propotional_weight_Update = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Neuron_propotional_weight_Initialize",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Neuron_propotional_weight_Initialize");
		Neuron_propotional_weight_Initialize = Convert.ToInt32(temp);
		
		
		//--------------------------------------------------------------
	}
	
	public void initialization (ref Dictionary<string, string> dictionary, ref Random rnd,int model){
		
		if (model==1){
			string temp;
			dictionary.TryGetValue("LIF.LSM_Max_Init_Weight_NegN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.LSM_Max_Init_Weight_NegN");
			LSM_Max_Init_Weight_NegN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF.LSM_Min_Init_Weight_NegN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.LSM_Min_Init_Weight_NegN");
			LSM_Min_Init_Weight_NegN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF.LSM_Max_Init_Weight_PosN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.LSM_Max_Init_Weight_PosN");
			LSM_Max_Init_Weight_PosN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF.LSM_Min_Init_Weight_PosN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF.LSM_Min_Init_Weight_PosN");
			LSM_Min_Init_Weight_PosN = Convert.ToDouble(temp);
			
		}else if (model==2) {
			string temp;
			dictionary.TryGetValue("Izhikevich.LSM_Max_Init_Weight_NegN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.LSM_Max_Init_Weight_NegN");
			LSM_Max_Init_Weight_NegN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("Izhikevich.LSM_Min_Init_Weight_NegN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.LSM_Min_Init_Weight_NegN");
			LSM_Min_Init_Weight_NegN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("Izhikevich.LSM_Max_Init_Weight_PosN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.LSM_Max_Init_Weight_PosN");
			LSM_Max_Init_Weight_PosN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("Izhikevich.LSM_Min_Init_Weight_PosN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-Izhikevich.LSM_Min_Init_Weight_PosN");
			LSM_Min_Init_Weight_PosN = Convert.ToDouble(temp);
			
		}else if (model==3) {
			string temp;
			dictionary.TryGetValue("LIF-H.LSM_Max_Init_Weight_NegN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.LSM_Max_Init_Weight_NegN");
			LSM_Max_Init_Weight_NegN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF-H.LSM_Min_Init_Weight_NegN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.LSM_Min_Init_Weight_NegN");
			LSM_Min_Init_Weight_NegN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF-H.LSM_Max_Init_Weight_PosN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.LSM_Max_Init_Weight_PosN");
			LSM_Max_Init_Weight_PosN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("LIF-H.LSM_Min_Init_Weight_PosN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-LIF-H.LSM_Min_Init_Weight_PosN");
			LSM_Min_Init_Weight_PosN = Convert.ToDouble(temp);
			
		}else if (model==4) {
			string temp;
			dictionary.TryGetValue("McCulloch-Pitts.LSM_Max_Init_Weight_NegN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.LSM_Max_Init_Weight_NegN");
			LSM_Max_Init_Weight_NegN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("McCulloch-Pitts.LSM_Min_Init_Weight_NegN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.LSM_Min_Init_Weight_NegN");
			LSM_Min_Init_Weight_NegN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("McCulloch-Pitts.LSM_Max_Init_Weight_PosN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.LSM_Max_Init_Weight_PosN");
			LSM_Max_Init_Weight_PosN = Convert.ToDouble(temp);
			
			dictionary.TryGetValue("McCulloch-Pitts.LSM_Min_Init_Weight_PosN",out temp);
			if (string.IsNullOrEmpty(temp))
				Console.WriteLine("configuration-McCulloch-Pitts.LSM_Min_Init_Weight_PosN");
			LSM_Min_Init_Weight_PosN = Convert.ToDouble(temp);
		}
	}
	
	public void initArc(ref Random rnd){
		Connections = (int) Math.Floor(Number_Of_Neurons * (Number_Of_Neurons-1) * GeneralConnectivity);
		
		if (Liquid_Architecture==1){ //Old Mathod
			Min_Neuron_Connection = 2 ; // random in or out
			Min_Group_Connections = 1 ; // in and out
			Group_size = 3;  // number of nurons need to be divided by Group Size
		}
		
		if (Liquid_Architecture==2){ // FeadForward Net
			int minimumGroupSize = 20;
			int totalgroups = Math.Max(5,Number_Of_Neurons/minimumGroupSize);
			while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
				Number_Of_Neurons++;
			
			Group_size_Min = Number_Of_Neurons/totalgroups;
			Group_size_Max = Group_size_Min+Group_size_Min/2;
			Group_interconnections = new int[2]{Group_size_Min/2,
				Group_size_Min-3};
			
			Connections = (int) Math.Floor(Number_Of_Neurons *
			                               (Number_Of_Neurons-1) * 
			                               GeneralConnectivity);
		}//-------------------------------------------------------------------------
		
		else if (Liquid_Architecture==3) { // Maass et al.
			
			Maass_Lambda = 2; // Maass parameters are 0,2,4,8

			int x=1,y=4;
			while(x*x*y<Number_Of_Neurons){
				if (x*x<y) {x++;}
				else {y++; x=1;}
			}
			Number_Of_Neurons = x*x*y;
			
			Maass_column = new int[3]{x,x,y};
			
			Connections = (int) Math.Floor(Number_Of_Neurons *
			                               (Number_Of_Neurons-1) * 
			                               GeneralConnectivity);
		}//-------------------------------------------------------------------------
		
		else if (Liquid_Architecture==4) { // Power Law on groups
			int minimumGroupSize = 10;
			int totalgroups = Math.Max(2,Number_Of_Neurons/minimumGroupSize);
			while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
				Number_Of_Neurons++;
			
			Group_size_Min = Number_Of_Neurons/3;
			Group_size_Max = Group_size_Min+rnd.Next(0,Number_Of_Neurons/10);
			Group_interconnections = new int[2]{Group_size_Min/3,Group_size_Min/2};
			
			Connections = (int) Math.Floor(Number_Of_Neurons * 
			                               (Number_Of_Neurons-1) *
			                               GeneralConnectivity);
		}//-------------------------------------------------------------------------
		
		else if (Liquid_Architecture==5) { // Uncorrelated Scale Free
			
			
			Connections = (int) Math.Floor(Number_Of_Neurons * 
			                               (Number_Of_Neurons-1) *
			                               GeneralConnectivity);
		}//-------------------------------------------------------------------------
		
		else if (Liquid_Architecture==6) { // Uncorrelated Scale Free Powerlaw ,
			
			Connections = (int) Math.Floor(Number_Of_Neurons *
			                               (Number_Of_Neurons-1) *
			                               GeneralConnectivity);
		}//-------------------------------------------------------------------------
		
		else if (Liquid_Architecture==7) { // Power Law Selections (one:PowerLaw sec:Gaussian)
			
			Connections = (int) Math.Floor(Number_Of_Neurons *
			                               (Number_Of_Neurons-1) *
			                               GeneralConnectivity);
		}//-------------------------------------------------------------------------
		
		else if (Liquid_Architecture==8) { // Two Ways Power Law
			int minimumGroupSize = 8; // was 12 20/03/2013
			int totalgroups = Math.Max(1,Number_Of_Neurons/minimumGroupSize);
			while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
				Number_Of_Neurons++;
			
			Group_size_Min = Number_Of_Neurons/totalgroups;
			Group_size_Max = Group_size_Min+rnd.Next(0,Group_size_Min/4);
			Group_interconnections = new int[2]{Group_size_Min/3,Group_size_Min/2};
			
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)* GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==9) { // Power law x<-->y
			
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)* GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==10) { // Two Ways Linear Descent
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==11) { //Power Law Method 2
			
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==12) { // Combine Methods (11 + 2 without internal groups)
			int minimumGroupSize = 50;
			int totalgroups = Math.Max(3,Number_Of_Neurons/minimumGroupSize);
			while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
				Number_Of_Neurons++;
			
			Group_size_Min = (Number_Of_Neurons/totalgroups)-1;
			Group_size_Max = Group_size_Min+rnd.Next(0,Group_size_Min/10);
			Group_interconnections = new int[2]{Group_size_Min/2,Group_size_Min};
			
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==13) { // combine Methods (8 + 2 without internal groups)
			int minimumGroupSize = 80;
			int totalgroups = Math.Max(3,Number_Of_Neurons/minimumGroupSize);
			while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
				Number_Of_Neurons++;
			
			Group_size_Min = Number_Of_Neurons/totalgroups;
			Group_size_Max = Group_size_Min+1;
			Group_interconnections = new int[2]{Group_size_Min/2, Group_size_Min/1};
			
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==14) { // combine Methods (5 + 2 without internal groups)
			int minimumGroupSize = 80;
			int totalgroups = Math.Max(3,Number_Of_Neurons/minimumGroupSize);
			while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
				Number_Of_Neurons++;
			
			Group_size_Min = (Number_Of_Neurons/totalgroups)-1;
			Group_size_Max = Group_size_Min+1;
			Group_interconnections = new int[2]{Group_size_Min/2,Group_size_Min};
			
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==15) { //combine Methods (0 + 2 without internal groups)
			int minimumGroupSize = 20;
			int totalgroups = Math.Max(25,Number_Of_Neurons/minimumGroupSize);
			while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
				Number_Of_Neurons++;
			
			Group_size_Min = (Number_Of_Neurons/totalgroups)-1;
			Group_size_Max = Group_size_Min+1;
			Group_interconnections = new int[2]{Group_size_Min/8,Group_size_Min/6};
			
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==16) { //Arrange the neuron in a Mesh
			while(Math.Sqrt(Number_Of_Neurons)%10==0){
				Number_Of_Neurons++;
			}
			Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*GeneralConnectivity);
		}//-------------------------------------------------------------------------
		else if (Liquid_Architecture==17) { //Click --> full connectivity
			Connections = Number_Of_Neurons * (Number_Of_Neurons-1);
		}
	}
}
//-----------------------------------------------------------------------------------

[Serializable()]
public class Readout
{
	public int[] ReadOut_Unit;
	public int[] ReadOut_unit_Model_AP_or_Sequ;
	public int[] ReadOut4EveryWindow;

	public int LSM_Runing_Interval;
	public int LSM_1sec_interval;
	public int LSM_Adjustment_Time_at_Ending ,LSM_Adjustment_Time_at_Beginning ;
	public int ReadoutU_Window_Size;
	public int ReadoutU_How_Many_Windows;
	public int ReadoutU_Window_Shifting;
	public int ReadoutU_Disctance_Between_Windows;
	public int ReadoutU_epoch;
	public double Readout_Max_Error;
	public int Readout_Negative;
	public double ReadOut_Unit_HiddenLayerSize;
	public int ReadOut_Unit_outputSize;
	public int Readout_Activity_during_Input_time;
	public int Approximate_OR_Classification;

	public void parsing(ref Dictionary<string, string> dictionary){
		string temp;
		string[] words;
		
		dictionary.TryGetValue("LSM_Runing_Interval",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-LSM_Runing_Interval");
		LSM_Runing_Interval = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("LSM_1sec_interval",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-LSM_1sec_interval");
		LSM_1sec_interval = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("LSM_Adjustment_Time_at_Beginning",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-LSM_Adjustment_Time_at_Beginning");
		LSM_Adjustment_Time_at_Beginning = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("LSM_Adjustment_Time_at_Ending",out temp);
		if (string.IsNullOrEmpty(temp)){
			Console.WriteLine("configuration-LSM_Adjustment_Time_at_Ending load with a defult value");
			temp = "0";
		}
		LSM_Adjustment_Time_at_Ending = Convert.ToInt32(temp);
		
		
		dictionary.TryGetValue("ReadoutU_How_Many_Windows",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadoutU_How_Many_Windows");
		ReadoutU_How_Many_Windows = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("ReadoutU_Window_Size",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadoutU_Window_Size");
		ReadoutU_Window_Size = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("ReadoutU_Window_Shifting",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadoutU_Window_Shifting");
		ReadoutU_Window_Shifting = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("ReadoutU_Disctance_Between_Windows",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadoutU_Disctance_Between_Windows");
		ReadoutU_Disctance_Between_Windows = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("ReadoutU_epoch",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadoutU_epoch");
		ReadoutU_epoch = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("ReadOut_Unit_HiddenLayerSize",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadOut_Unit_HiddenLayerSize");
		ReadOut_Unit_HiddenLayerSize = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("Readout_Max_Error",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Readout_Max_Error");
		Readout_Max_Error = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("Readout_Negative",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Readout_Negative");
		Readout_Negative = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("ReadOut_Unit_outputSize",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadOut_Unit_outputSize");
		ReadOut_Unit_outputSize = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Readout_Activity_during_Input_time",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Readout_Activity_during_Input_time");
		Readout_Activity_during_Input_time = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Approximate_OR_Classification",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-Approximate_OR_Classification");
		Approximate_OR_Classification = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("ReadOut_Unit",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadOut_Unit");
		words = temp.Split(',');
		int time = words.Length;
		ReadOut_Unit = new int[time];
		for (int i = 0; i < time; i++) {
			ReadOut_Unit[i] = Convert.ToInt32(words[i]);
		}
		
		dictionary.TryGetValue("ReadOut_unit_Model_AP_or_Sequ",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadOut_unit_Model_AP_or_Sequ");
		words = temp.Split(',');
		time = words.Length;
		ReadOut_unit_Model_AP_or_Sequ = new int[time];
		for (int i = 0; i < time; i++) {
			ReadOut_unit_Model_AP_or_Sequ[i] = Convert.ToInt32(words[i]);
		}
		
		dictionary.TryGetValue("ReadOut4EveryWindow",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("configuration-ReadOut4EveryWindow");
		words = temp.Split(',');
		time = words.Length;
		ReadOut4EveryWindow = new int[time];
		for (int i = 0; i < time; i++) {
			ReadOut4EveryWindow[i] = Convert.ToInt32(words[i]);
		}
		
	}
	


}
//-----------------------------------------------------------------------------------

[Serializable()]
public class testParam
{
	// Global
	public int Repetition;// = 20 ;
	public int BinaryInput_OR_RealValuse;// = 0 ; // 0 = Binary , 1 = Real Values
	public string DirName;
	public int NoisePercent;//=11;
	public int Silent_Tuning;
	public int Input_Tuning;
	public int Change_Threshold_Of_Input_Neurons; // 0 = No , 1 = Yes
	
	//Damage in the LSM
	public int Numbers_of_Inputs;// = 20;
	public int Input_Pattern_Size;// = 400;
	public double[] RandomMaxMin_RealValues;//=  new double[]{-1,1};
	public double LSM_Damage;
	
	// Activity Test in Liquid
	public int Activity_Test_Running_Time;
	public int Activity_Test_Time_Between_Inputs;
	public int Activity_Test_Greace_Time;
	public int Activity_Test_MinSignalLength;
	public int Activity_Test_MaxSignalLength;
	public int Numbers_of_Inputs_For_Activity_Check;
	
	// Sound Files
	public string[] soundFiles_toLearn;
	public string[] soundFiles_toTest;
	public int MaxLevel;
	public int dataQunta;
	
	// TIMIT
	public int[] DirNumber;
	public double Learn_Raio;
	
	
	public void parsing(ref Dictionary<string, string> dictionary){
		string temp;
		string[] words;
		
		// Global
		dictionary.TryGetValue("Repetition",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Repetition");
		Repetition = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("BinaryInput_OR_RealValuse",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - BinaryInput_OR_RealValuse");
		BinaryInput_OR_RealValuse = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("DirName",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - DirName");
		DirName = temp;
		
		dictionary.TryGetValue("Silent_Tuning",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Silent_Tuning");
		Silent_Tuning = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Input_Tuning",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Input_Tuning");
		Input_Tuning = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Change_Threshold_Of_Input_Neurons",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Change_Threshold_Of_Input_Neurons");
		Change_Threshold_Of_Input_Neurons = Convert.ToInt32(temp);
		
		
		//Damage in the LSM
		dictionary.TryGetValue("NoisePercent",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - NoisePercent");
		NoisePercent = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Numbers_of_Inputs",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Numbers_of_Inputs");
		Numbers_of_Inputs = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Input_Pattern_Size",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Input_Pattern_Size");
		Input_Pattern_Size = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("RandomMaxMin_RealValues",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - RandomMaxMin_RealValues");
		words = temp.Split(',');
		int time = words.Length;
		RandomMaxMin_RealValues = new double[time];
		for (int i = 0; i < time; i++) {
			RandomMaxMin_RealValues[i] = Convert.ToDouble(words[i]);
		}
		
		dictionary.TryGetValue("DirName",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - DirName");
		DirName = temp;
		
		dictionary.TryGetValue("LSM_Damage",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - LSM_Damage");
		LSM_Damage = Convert.ToDouble(temp);
		
		// Activity Test in Liquid
		dictionary.TryGetValue("Activity_Test_Running_Time",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Activity_Test_Running_Time");
		Activity_Test_Running_Time = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Activity_Test_Time_Between_Inputs",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Activity_Test_Time_Between_Inputs");
		Activity_Test_Time_Between_Inputs = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Activity_Test_Greace_Time",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Activity_Test_Greace_Time");
		Activity_Test_Greace_Time = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Activity_Test_MinSignalLength",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Activity_Test_MinSignalLength");
		Activity_Test_MinSignalLength = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Activity_Test_MaxSignalLength",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Activity_Test_MaxSignalLength");
		Activity_Test_MaxSignalLength = Convert.ToInt32(temp);
		
		dictionary.TryGetValue("Numbers_of_Inputs_For_Activity_Check",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Numbers_of_Inputs_For_Activity_Check");
		Numbers_of_Inputs_For_Activity_Check = Convert.ToInt32(temp);
		
		
		
		// TIMIT\
		
		dictionary.TryGetValue("Learn_Raio",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - Learn_Raio");
		Learn_Raio = Convert.ToDouble(temp);
		
		dictionary.TryGetValue("DirNumber",out temp);
		if (string.IsNullOrEmpty(temp))
			Console.WriteLine("testParam - DirNumber");
		words = temp.Split(',');
		time = words.Length;
		DirNumber = new int[time];
		for (int i = 0; i < time; i++) {
			DirNumber[i] = Convert.ToInt32(words[i]);
		}
		
		
	}
	
	
}
//-----------------------------------------------------------------------------------

[Serializable()]
public class globalParam
{
	public Input2liquid input2liquid = new Input2liquid();
	public NeuronParametes neuronParam = new NeuronParametes();
	public NetworkParm networkParam = new NetworkParm();
	public Readout detector = new Readout();
	public testParam test_Param = new testParam();
	
	string TestName;
	
	//Misc
	public int iteration = 0; // mean the Voxel Number that the thread is learinig\testing
	public int Linux_OR_Windows; // 1= Windows(Debug) , 2 =Linux(Full Run)
	public int ThreadNum;
	public Random rnd;
	public randomIntArr rndA;
	
	
	[Serializable()]
	public struct Data{
		public double[,] Input;
		public bool[,] Binary_Input;
		public int MaxInputLength;
		public int Tag;
		public double[] Target;
		public bool[,] Binary_Target;
	}
	
	public Data[] LearnData,TestData;
	
	public int HowMenySessionsToTest=1;
	public int[] NumberOfSession;
	public int NumberOfTestIneachSesstion;
	public int NumberOfTimeInEachTest;
	public int[,] LearnGroup;
	public int[,] TestGrpup;
	public int CurrentGroup;
	public double[][] Groups;
	
	
	public globalParam(){
		TestName = "";
	}
	//----------------------------------------------------------------------------------
	public void SeedInitialization(){
		this.rnd = new Random(Convert.ToInt32((System.DateTime.UtcNow.Ticks)%int.MaxValue));
		this.rndA = new randomIntArr();
	}
	//----------------------------------------------------------------------------------
	public void LoadSeed(int seed){
		this.rnd = new Random(seed);
		this.rndA = new randomIntArr();
	}
	//----------------------------------------------------------------------------------
	public void initialization(){
		if (string.IsNullOrEmpty(TestName))
			Console.WriteLine("ERROR THERE IS NO NAME FOR THE TEST!");
		else
			initialization(TestName);
	}
	
	public void initialization(string TestName){
		//---------------------Operating System patams--------------------------------
		System.OperatingSystem osInfo = System.Environment.OSVersion;
		if ( osInfo.Platform== PlatformID.Win32NT)
		{
			// Windows (debug)
			Linux_OR_Windows=2;
			//ThreadNum = Environment.ProcessorCount;
			ThreadNum = 1;
		}else{
			// Linux
			Linux_OR_Windows=1;
//			ThreadNum = Environment.ProcessorCount -1;
			ThreadNum = 3;
			if (ThreadNum<=1) ThreadNum=1;
		}
		
		Dictionary<string, string> neuron,readout,recorentNet,input,test;
		ReadDictionaryFile("neuron.ini",out neuron);
		ReadDictionaryFile("Readout.ini",out readout);
		ReadDictionaryFile("recorentNet.ini",out recorentNet);
		ReadDictionaryFile("input.ini",out input);
		ReadDictionaryFile("test.ini",out test);
		
		neuronParam.parsing(ref neuron);
		detector.parsing(ref readout);
		networkParam.parsing(ref recorentNet);
		input2liquid.parsing(ref input);
		test_Param.parsing(ref test);
		
		
		SeedInitialization();
		globalParam deme = this.copy();
		neuronParam.initialization(ref deme);
		networkParam.initialization(ref recorentNet,ref rnd,neuronParam.Neuron_Model);
		
	}//----------------------------------------------------------------------------------
	
	public void loadTestName(string TestName){
	this.TestName = TestName;
	}
	//----------------------------------------------------------------------------------
	
	public void copyConfiguration(string TestName){
		this.TestName = TestName;
		string Source=@"..\..\Configuration\"+@TestName+@"\";
		string Target=@".\";
		DirectoryInfo di = new DirectoryInfo(Source);
		if (di.Exists){
			FileInfo[] rgFiles = di.GetFiles("*.ini");
			foreach (var element in rgFiles)
				File.Copy(Source+element.ToString(),Target+element.ToString(),true);
		}
	}
	//----------------------------------------------------------------------------------
	
	
	public globalParam copy(){
		globalParam source = (globalParam) this.MemberwiseClone();
		return(source);
	}//----------------------------------------------------------------------------------
	
	public void save(){
		Stream fileStream = new FileStream("globalParam"+this.iteration.ToString()+".dat", FileMode.Create,FileAccess.ReadWrite, FileShare.None);
		BinaryFormatter binaryFormater = new BinaryFormatter();
		binaryFormater.Serialize(fileStream,this.copy());
		fileStream.Flush();
		fileStream.Close();
	}//----------------------------------------------------------------------------------
	
	public void save(string filename){
		Stream fileStream = new FileStream(filename, FileMode.Create,FileAccess.ReadWrite, FileShare.None);
		BinaryFormatter binaryFormater = new BinaryFormatter();
		binaryFormater.Serialize(fileStream,this.copy());
		fileStream.Flush();
		fileStream.Close();
	}//----------------------------------------------------------------------------------
	
	public int[] ConvertStringToInt(string[] input){
		int size = input.Length;
		int[] output = new int[size];
		for(int i = 0 ; i < size ; i++) int.TryParse(input[i],out output[i]);
		return output;
	}//----------------------------------------------------------------------------------
	
	public globalParam load(string args){
		Stream fileStream = new FileStream(args, FileMode.Open,FileAccess.Read, FileShare.None);
		BinaryFormatter binaryFormater = new BinaryFormatter();
		globalParam Param = (globalParam) binaryFormater.Deserialize(fileStream);
		fileStream.Close();
		return Param;
	}//----------------------------------------------------------------------------------
	
	
	public void RandomaizeData(ref globalParam.Data[] Data){
		int size = Data.Length;
		int[] index = new int[size];
		this.rndA.select(0,size,ref index, ref this.rnd);
		
		globalParam.Data[] temp;
		temp = Data;
		Data = new globalParam.Data[size];
		
		for (int i = 0; i < size ; i++) {
			Data[i] = new globalParam.Data();
			
			Data[i].Input = new double[temp[index[i]].Input.GetLength(0),temp[index[i]].Input.GetLength(1)];
			for (int inputI = 0; inputI < temp[index[i]].Input.GetLength(0); inputI++)
				for (int inputJ = 0; inputJ < temp[index[i]].Input.GetLength(1); inputJ++)
					Data[i].Input[inputI,inputJ] = temp[index[i]].Input[inputI,inputJ];
			
			Data[i].Tag = temp[index[i]].Tag;
			
			Data[i].Target = new double[temp[index[i]].Target.Length ];
			for (int inputI = 0; inputI < temp[index[i]].Target.Length ; inputI++)
				Data[i].Target[inputI] = temp[index[i]].Target[inputI];
			
			Data[i].MaxInputLength = temp[index[i]].MaxInputLength;
			
			if (temp[index[i]].Binary_Input!=null)
				Data[i].Binary_Input = (bool[,]) temp[index[i]].Binary_Input.Clone();
			if (temp[index[i]].Binary_Target!=null)
				Data[i].Binary_Target = (bool[,]) temp[index[i]].Binary_Target.Clone();
			
			
		}
		
	}//----------------------------------------------------------------------------------
	
	public globalParam.Data[] ReturnCopy(ref globalParam.Data[] Data){
		int size = Data.Length;
		globalParam.Data[] temp = new globalParam.Data[size];
		
		
		for (int i = 0; i < size ; i++) {
			
			temp[i].Input = new double[Data[i].Input.GetLength(0),Data[i].Input.GetLength(1)];
			for (int inputI = 0; inputI < Data[i].Input.GetLength(0); inputI++)
				for (int inputJ = 0; inputJ < Data[i].Input.GetLength(1); inputJ++)
					temp[i].Input[inputI,inputJ] = Data[i].Input[inputI,inputJ];
			
			temp[i].Target = new double[Data[i].Target.Length ];
			for (int inputI = 0; inputI < Data[i].Target.Length ; inputI++)
				temp[i].Target[inputI] = Data[i].Target[inputI];
			
			temp[i].Tag = Data[i].Tag;
			
			if (Data[i].Binary_Input!=null)
				temp[i].Binary_Input = (bool[,]) Data[i].Binary_Input.Clone();
			if (Data[i].Binary_Target!=null)
				temp[i].Binary_Target = (bool[,]) Data[i].Binary_Target.Clone();
			temp[i].MaxInputLength = Data[i].MaxInputLength;
			
		}
		
		return temp;
	}//----------------------------------------------------------------------------------
	
	
	public void AddNoiseToData(ref globalParam Param, ref globalParam.Data[] Data, double increaseSize,double noise){
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Noise">1=100%,1=0% Noise add to the orginal input</param>
		/// <param name="increaseSize">By how much to incress the data set?</param>
		
		if (increaseSize==0) return;
		globalParam.Data[] returnData = new globalParam.Data[(int) Math.Round(Data.Length * increaseSize)];
		
		int count=0;
		for (int i = 0; i < Data.Length; i++) {
			for (int f = 0; f < increaseSize ; f++) {
				returnData[count].Input = new double[Data[i].Input.GetLength(0),Data[i].Input.GetLength(1)];
				returnData[count].Target = new double[Data[i].Target.Length];
				if (f==0){
					for (int inputI = 0; inputI < Data[i].Input.GetLength(0); inputI++)
						for (int inputJ = 0; inputJ < Data[i].Input.GetLength(1); inputJ++)
							returnData[count].Input[inputI,inputJ] = Data[i].Input[inputI,inputJ];
					returnData[count].Tag = Data[i].Tag;
					for (int inputI = 0; inputI < Data[i].Target.Length ; inputI++)
						returnData[count].Target[inputI] = Data[i].Target[inputI];
				}else{
					for (int inputI = 0; inputI < Data[i].Input.GetLength(0); inputI++) {
						for (int inputJ = 0; inputJ < Data[i].Input.GetLength(1); inputJ++) {
							double temp = rndA.NextDouble(ref Param,0,1);
							if (temp<noise){
								temp = Data[i].Input[inputI,inputJ] * rndA.NextDouble(ref Param,0,0.1);
								
								if(rndA.NextDouble(ref Param,0,1)<0.5){
									if (returnData[count].Input[inputI,inputJ] + temp > 200)
										returnData[count].Input[inputI,inputJ] -= temp;
									else
										returnData[count].Input[inputI,inputJ] += temp;
								}else{
									if (returnData[count].Input[inputI,inputJ] - temp < -200)
										returnData[count].Input[inputI,inputJ] += temp;
									else
										returnData[count].Input[inputI,inputJ] -= temp;
								}
							}else
								returnData[count].Input[inputI,inputJ] = Data[i].Input[inputI,inputJ];
						}
					}
					returnData[count].Tag = Data[i].Tag;
					for (int inputI = 0; inputI < Data[i].Target.Length ; inputI++)
						returnData[count].Target[inputI] = Data[i].Target[inputI];
				}
				count++;
			}
		}
		Data = returnData;
	}//----------------------------------------------------------------------------------
	
	
	public void ReadDictionaryFile(string fileName,out Dictionary<string, string> dictionary)
	{
		dictionary = new Dictionary<string, string>();
		foreach (string line in File.ReadAllLines(fileName))
		{
			if ((!string.IsNullOrEmpty(line)) &&
			    (!line.StartsWith(";")) &&
			    (!line.StartsWith("#")) &&
			    (!line.StartsWith("'")) &&
			    (line.Contains("=")))
			{
				int index = line.IndexOf('=');
				string key = line.Substring(0, index).Trim();
				string value = line.Substring(index + 1).Trim();
				index = value.IndexOf("//");
				if (index>=0)
					value = value.Substring(0,index);
				index = value.IndexOf(";");
				if (index>=0)
					value = value.Substring(0,index);
				
				if ((value.StartsWith("/") && value.EndsWith("/")) ||
				    (value.StartsWith("\"") && value.EndsWith("\"")) ||
				    (value.StartsWith("'") && value.EndsWith("'")))
				{
					value = value.Substring(1, value.Length - 2);
				}
				dictionary.Add(key, value);
			}
		}
//		return dictionary;
	}
	
	//----------------------------------------------------------------------------------
	
	
}


//----------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------

[Serializable()]
public class cumulateor{
	public double[] members;
	
	public cumulateor(){
//		this.members = new double[0];
	}
	
	public double Return_Sum(){
		double sum=0;
		for ( long i = 0 ; i < this.members.LongLength ; i++ ) { sum+=this.members[i];}
		return sum;
	}
	public long ReturnNamberOfMembers(){
		if (this.members==null)
			return 0;
		else
			return this.members.LongLength;
	}
	
	public void Add(double number){
		int length = 0 ;
		if (this.members!=null)
			length = this.members.Length;
		Array.Resize(ref this.members,length+1);
		this.members[length]=number;
	}
	
	public bool dec(double number){
		long counter=0;
		double[] temp = new double[this.members.LongLength-1];
		int flag=0;
		for ( long i = 0 ; i < this.members.LongLength ; i++ ) {
			if (number!= this.members[i]) { temp[counter]=this.members[i]; counter++;}
			else if ((number == this.members[i]) && (flag==0)){flag=1; continue;}
			else if ((number == this.members[i]) && (flag==1)){temp[counter]=this.members[i]; counter++;}
		}
		this.members = temp;
		// return 1 = seccsess , 0 = failed
		return(counter==this.members.LongLength);
	}
	
	public double Return_Average(){
		if (this.members.LongLength==0) return 0;
		else return (this.Return_Sum()/this.members.LongLength);
	}
	
	public double Return_Standard_Deviation(){
		double sum = 0;
		double avg = this.Return_Average();
		
		for ( long i = 0 ; i < this.members.LongLength ; i++ ) {
			sum+=Math.Pow((this.members[i]-avg),2);
		}
		
		if ( this.members.LongLength==0) sum*=0;
		else sum*=1/(double)this.members.LongLength;
		
		return (Math.Sqrt(sum));
	}
	
	public double Return_min(){
		if ( this.members.Length==0) {return 0;}
		double min = this.members[0];
		for ( long i = 0 ; i < this.members.LongLength ; i++ ) {
			if (this.members[i]<min) min = this.members[i];
		}
		
		return min;
	}
	
	public double Return_max(){
		if ( this.members.Length==0) {return 0;}
		double max = this.members[0];
		for ( long i = 0 ; i < this.members.LongLength ; i++ ) {
			if (this.members[i]>max) max = this.members[i];
		}
		
		return max;
	}
	
}
