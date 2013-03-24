/*
 * Created by SharpDevelop.
 * User: hhazan01
 * Date: 16/06/2011
 * Time: 12:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Liquid_Detector.Tests.Sound.Parameters
{
	/// <summary>
	/// Description of NeuronParametes.
	/// </summary>
	[Serializable()]
	public class Neuron4Sound: NeuronParametes
	{
		//----------
		
		public Neuron4Sound()
		{
			// ---------Singal Neuron parameters
			// 1= LIF 2=Izhikevich 3=LIF+Hananel add-ons 4=McCulloch–Pitts
			Neuron_Model = 1 ;
			
			Neuron_Min_Delay =1;
			Neuron_Max_Delay =1;
			
			STDPwindow = new int[]{5,1};// past,future ==> pre , post
			STDPMaxChange = new double[]{0.0001,0.0001}; // past,future // pre , post
			Active_STDP_rule = 0 ; // 0 no 1 yes
			
			Liquid_Sliding_Treshold_arangment = 0; // 0 = random, 1 = highst input connection,2== highest output connection
			Liquid_amount_of_neurons_Threshold = 1 ; // how meny neurons will be with slide threshold? 1=100%
			Random_Factor_Sliding_Treshold = 0.1 ; // each neuron have random element that this factor multiplay it
			Randomize_initilaization = 0 ; // upon rest the neuron will randomize the Volt and Threshold

			//----------------Neuron Modle-------------------------------------
			if (this.Neuron_Model==1){ // Normal LIF
				this.decayFactor = 0.6 ; //   0.99 = 99% decay 0 = 0% decay
				this.Int_Neuron_Spike = 60 ;
				this.Proportional_Threshold = 0 ;  // proportional? 0 = no , 1 = yes
				this.Neuron_Threshold = 30 ; //if Proportional_Threshold =  NO
				this.Neuron_Threshold_Proportion = 0.05 ; // else, 0.2 meen 0.2% of the inputs have spike then fire
				this.Slideing_Threshold = 1 ; // 1 = Yes , 0 = No
				this.initV = -65.0 ;
			}
			else if (this.Neuron_Model==2){ // Izhikevich
				this.decayFactor =  1 ; // Some time the input(internal and external)is too strong, decay is dived the input
				this.Int_Neuron_Spike = 60 ;
				this.Proportional_Threshold = 0 ;  // proportional? 0 = no , 1 = yes
				this.Neuron_Threshold = 30 ; //if Proportional_Threshold =  NO
				this.Neuron_Threshold_Proportion = 0.5 ; // else, 0.2 meen 0.2% of the inputs have spike then fire
				this.Slideing_Threshold = 0 ;// 1 = Yes , 0 = No
				this.initV = -65.0 ;
			}
			else if (this.Neuron_Model==3){ // LIF + Hananel improvments
				this.decayFactor = 0.4; // 0.01 = 1% decay, 1 = 100% decay
				this.Int_Neuron_Spike = 60 ;
				this.Proportional_Threshold = 0 ;  // proportional? 0 = no , 1 = yes
				this.Neuron_Threshold = 30 ; //if Proportional_Threshold =  NO
				this.Neuron_Threshold_Proportion = 0.05 ; // else, 0.2 meen 0.2% of the inputs have spike then fire
				this.Slideing_Threshold = 1 ;// 1 = Yes , 0 = No
				this.initV = -65.0 ;
			}
			else if (this.Neuron_Model==4){ // McCulloch–Pitts
				this.decayFactor = 0 ;  // Not relevent here
				this.Int_Neuron_Spike = 1 ;
				this.Proportional_Threshold = 0  ;  // proportional? 0 = no , 1 = yes
				this.Neuron_Threshold = 1 ; //its relevent ONLY if Proportional_Threshold =  NO
				this.Neuron_Threshold_Proportion = 0.1 ; // else, 0.2 meen 0.2% of the inputs have spike then fire
				this.Slideing_Threshold = 0 ;// 1 = Yes , 0 = No
				this.initV = 0 ;
			}
			// Input from External MUST be bigger then INT_SPIKE
			this.Ext_Inp_Neuron_Spike = ((Neuron_Threshold-initV)+1);
			if (this.Int_Neuron_Spike>=this.Ext_Inp_Neuron_Spike)
				Ext_Inp_Neuron_Spike = 2*Int_Neuron_Spike;
			//-----------------------------------------------------
		}
		
		override public void initNueronMaxParameters(int one_Second){
			//-------------------Prefurm Testing on single neuron------------------------------------
			globalParam param = new globalParam();
			param.neuronParam = new Liquid_Detector.Tests.DamageLSM.Parameters.Neuron4DamageLSM();;
			NeuronArch.Unit neu = (NeuronArch.LIFNeuron) new NeuronArch.LIFNeuron(ref param.neuronParam);
			switch (this.Neuron_Model)
			{
					case 1: neu = (NeuronArch.LIFNeuron) new NeuronArch.LIFNeuron(ref param.neuronParam); 		break;
					case 2: neu = (NeuronArch.IzakNeuron) new NeuronArch.IzakNeuron(ref param.neuronParam); 		break;
					case 3: neu = (NeuronArch.LIFmodle2) new NeuronArch.LIFmodle2(ref param.neuronParam); 		break;
					case 4: neu = (NeuronArch.McCulloch_Pitts) new NeuronArch.McCulloch_Pitts(ref param.neuronParam); 	break;
			}
			
			this.Neuron_Firing_Rate_Max = 0 ;
			for (int i = 0; i < one_Second ; i++) {
				double output = 0;
				neu.step(ref output,0,this.Ext_Inp_Neuron_Spike);
				if (output>0)
					this.Neuron_Firing_Rate_Max++;
			}
			this.Neuron_Slideing_Threshold_Recommended_Firing_Rate_Max = 0.9 * this.Neuron_Firing_Rate_Max;
			this.Neuron_Slideing_Threshold_Recommended_Firing_Rate_Min = 0.0001 * this.Neuron_Firing_Rate_Max;
			this.Neuron_STDP_Recommended_Firing_Rate_Max = 0.5 * this.Neuron_Firing_Rate_Max;
			this.Neuron_STDP_Recommended_Firing_Rate_Min = 0.1 * this.Neuron_Firing_Rate_Max;
			this.STDPfrqu = new double[]{this.Neuron_Firing_Rate_Max/5,this.Neuron_Firing_Rate_Max/10};
		}
		
		
		public Neuron4Sound copy(){
			Neuron4Sound source = (Neuron4Sound) this.MemberwiseClone();
			return(source);
		}//----------------------------------------------------------------------------------
		
		public void save(int iteration){
			Stream fileStream = new FileStream("NeuronParam"+iteration.ToString()+".dat", FileMode.Create,FileAccess.ReadWrite, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			binaryFormater.Serialize(fileStream,this.copy());
			fileStream.Flush();
			fileStream.Close();
		}//----------------------------------------------------------------------------------
		
		public void save(string filename){
			Stream fileStream = new FileStream("obj"+filename+".dat", FileMode.Create,FileAccess.ReadWrite, FileShare.None);
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
		
		public Neuron4Sound load(string args){
			Stream fileStream = new FileStream(args, FileMode.Open,FileAccess.Read, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			Neuron4Sound Param = (Neuron4Sound) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			return Param;
		}//----------------------------------------------------------------------------------

		
		
	}
}
