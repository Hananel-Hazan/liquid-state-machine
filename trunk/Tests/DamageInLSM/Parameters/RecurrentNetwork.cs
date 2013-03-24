/*
 * Created by SharpDevelop.
 * User: hhazan01
 * Date: 16/06/2011
 * Time: 12:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Liquid_Detector.Tests.DamageLSM.Parameters
{
	/// <summary>
	/// Description of RecurrentNetwork.
	/// </summary>
	[Serializable()]
	public class RecurrentNetwork4DamageLSM : NetworkParm
	{
		public RecurrentNetwork4DamageLSM(int Neuron_Model, ref globalParam Param)
		{
			/* 
		 	0  = random ,
			1  = Old Mathod,
			2  = FeedForward with Hubs,
			3  = Maass et al. ,
			4  = Power Law on groups
			5  = Uncorrelated Scale Free ,
			6  = Uncorrelated Scale Free Powerlaw ,
			7  = Power Law Selections (one:PowerLaw sec:Gaussian)
			8  = Two Ways Power Law ,
			9  = Power law x<-->y ,
			10 = Two Ways Linear Descent
			11 = Power Law Method 2
			12 = Combine Methods (11 + 2 without internal groups)
			13 = combine Methods (8 + 2 without internal groups)
			14 = combine Methods (5 + 2 without internal groups)
			15 = combine Methods (0 + 2 without internal groups)
			16 = Arrange the neuron in a Mesh
			17 = Click --> full connectivity
			 */
			Liquid_Architecture = 4	;
			GroupSize =  new int[0];
			Liquid_Readout_Units = 0 ;  // 0 = random, 1 = minimum input/output but bigger then zero, 2=maximum,3=middle
			Liquid_Update_Sync_Or_ASync = 0 ; // 0 = Synchrony update , 1 = Asynchrony update (random order)
			Dynamic_OR_Static_synapses = 2 ; // 1 = Dynamic , 2= Static
			synapseStrength_tamplate = new double[]{1,0.5,0}; // if Dynamic_OR_Static_synapses = Dynamic (1) then use tamplate
			
			// parameters of Neurons in the Liquid and Liquid structure
			IncreaseSelectedChanceBy = 5 ;
			Number_Of_Neurons = 50 ;
			GeneralConnectivity = 0.15; // Maass configuration is 15%(0.15) ref 'make_liquid.m' and 'http://www.lsm.tugraz.at/circuits/usermanual/node10.html'
			LSM_Percent_Of_Negative_Weights = 0.2; // Mass Parameter is 20%
			LSM_Input_Percent_Connectivity = 0.3 ; // Maass Parameter is 30%
			Liquid_Weights_Resistend = 1.01; // 1.1 = 10 % every delay interval
			Neuron_propotional_weight_Update = 1 ; // update of the weights will be in propotonal (place = 0.1 rest= 0.1/number of rest) or not? 
			Neuron_propotional_weight_Initialize = 0 ; // 0=min/max weight is valid,1=initilize and learn will be propotoinal if one increase other will decreas
			
			//----------------Neuron Modle-------------------------------------
			if (Neuron_Model==1){ // Normal LIF
				this.LSM_Max_Init_Weight_NegN = 0.2; //1.01;
				this.LSM_Min_Init_Weight_NegN = 0.2; //1;
				this.LSM_Max_Init_Weight_PosN = 0.2; //1.01;
				this.LSM_Min_Init_Weight_PosN = 0.2; //1;
			}
			else if (Neuron_Model==2){ // Izhikevich
				this.LSM_Max_Init_Weight_NegN = 0.11; //1.01;
				this.LSM_Min_Init_Weight_NegN = 0.1; //1;
				this.LSM_Max_Init_Weight_PosN = 0.11; //1.01;
				this.LSM_Min_Init_Weight_PosN = 0.1; //1;
			}
			else if (Neuron_Model==3){ // LIF + Hananel improvments
				this.LSM_Max_Init_Weight_NegN = 0.51; //1.01;
				this.LSM_Min_Init_Weight_NegN = 0.5; //1;
				this.LSM_Max_Init_Weight_PosN = 0.51; //1.01;
				this.LSM_Min_Init_Weight_PosN = 0.5; //1;
			}
			else if (Neuron_Model==4){ // McCulloch–Pitts
				this.LSM_Max_Init_Weight_NegN = 0.21; //1.01;
				this.LSM_Min_Init_Weight_NegN = 0.2; //1;
				this.LSM_Max_Init_Weight_PosN = 0.21; //1.01;
				this.LSM_Min_Init_Weight_PosN = 0.2; //1;
			}
			//-----------------------------------------------------
			//-----------------------------------------------------
			//-----------------------------------------------------
			//-----------------------------------------------------
			
			this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			
			if (Liquid_Architecture==2){ //Old Mathod
				Min_Neuron_Connection = 2 ; // random in or out
				Min_Group_Connections = 1 ; // in and out
				Group_size = 3;  // number of nurons need to be divided by Group Size
			}
			
			if (Liquid_Architecture==2){ // FeadForward Net
				int minimumGroupSize = 50;
				int totalgroups = Math.Max(5,Number_Of_Neurons/minimumGroupSize);
				while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
					Number_Of_Neurons++;
				
				Group_size_Min = Number_Of_Neurons/totalgroups;
				Group_size_Max = Group_size_Min+Group_size_Min/2;
				Group_interconnections = new int[2]{Group_size_Min/2,Group_size_Min-3};
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			
			else if (Liquid_Architecture==3) { // Maass et al.
				
				Maass_Lambda = 2; // Maass parameters are 0,2,4,8

				int x=1,y=4;
				while(x*x*y<Number_Of_Neurons){
					if (x*x<y) {x++;}
					else {y++; x=1;}
				}
				
				Maass_column = new int[3]{x,x,y};
				
				Number_Of_Neurons=Maass_column[0];
				for (int i = 1; i < Maass_column.Length; i++) {
					Number_Of_Neurons*=Maass_column[i];
				}
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			
			else if (Liquid_Architecture==4) { // Power Law on groups
				int minimumGroupSize = 10;
				int totalgroups = Math.Max(2,Number_Of_Neurons/minimumGroupSize);
				while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
					Number_Of_Neurons++;
				
				Group_size_Min = Number_Of_Neurons/3;
				Group_size_Max = Group_size_Min+Param.rnd.Next(0,Number_Of_Neurons/10);
				Group_interconnections = new int[2]{Group_size_Min/3,Group_size_Min/2};
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			
			else if (Liquid_Architecture==5) { // Uncorrelated Scale Free
				
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			
			else if (Liquid_Architecture==6) { // Uncorrelated Scale Free Powerlaw ,
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			
			else if (Liquid_Architecture==7) { // Power Law Selections (one:PowerLaw sec:Gaussian)
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			
			else if (Liquid_Architecture==8) { // Two Ways Power Law
				int minimumGroupSize = 12;
				int totalgroups = Math.Max(1,Number_Of_Neurons/minimumGroupSize);
				while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
					Number_Of_Neurons++;
				
				Group_size_Min = Number_Of_Neurons/totalgroups;
				Group_size_Max = Group_size_Min+Param.rnd.Next(0,Group_size_Min/4);
				Group_interconnections = new int[2]{Group_size_Min/3,Group_size_Min/2};
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==9) { // Power law x<-->y
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==10) { // Two Ways Linear Descent
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==11) { //Power Law Method 2
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==12) { // Combine Methods (11 + 2 without internal groups)
				int minimumGroupSize = 50;
				int totalgroups = Math.Max(3,Number_Of_Neurons/minimumGroupSize);
				while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
					Number_Of_Neurons++;
				
				Group_size_Min = (Number_Of_Neurons/totalgroups)-1;
				Group_size_Max = Group_size_Min+Param.rnd.Next(0,Group_size_Min/10);
				Group_interconnections = new int[2]{Group_size_Min/2,Group_size_Min};
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==13) { // combine Methods (8 + 2 without internal groups)
				int minimumGroupSize = 80;
				int totalgroups = Math.Max(3,Number_Of_Neurons/minimumGroupSize);
				while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
					Number_Of_Neurons++;
				
				Group_size_Min = Number_Of_Neurons/totalgroups;
				Group_size_Max = Group_size_Min+1;
				Group_interconnections = new int[2]{Group_size_Min/2, Group_size_Min/1};
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==14) { // combine Methods (5 + 2 without internal groups)
				int minimumGroupSize = 80;
				int totalgroups = Math.Max(3,Number_Of_Neurons/minimumGroupSize);
				while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
					Number_Of_Neurons++;
				
				Group_size_Min = (Number_Of_Neurons/totalgroups)-1;
				Group_size_Max = Group_size_Min+1;
				Group_interconnections = new int[2]{Group_size_Min/2,Group_size_Min};
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==15) { //combine Methods (0 + 2 without internal groups)
				int minimumGroupSize = 20;
				int totalgroups = Math.Max(25,Number_Of_Neurons/minimumGroupSize);
				while (!((Number_Of_Neurons%totalgroups==0)&&(Number_Of_Neurons/totalgroups>=minimumGroupSize)))
					Number_Of_Neurons++;
				
				Group_size_Min = (Number_Of_Neurons/totalgroups)-1;
				Group_size_Max = Group_size_Min+1;
				Group_interconnections = new int[2]{Group_size_Min/8,Group_size_Min/6};
				
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==16) { //Arrange the neuron in a Mesh
				while(Math.Sqrt(Number_Of_Neurons)%10==0){
					Number_Of_Neurons++;
				}
				this.Connections = (int) Math.Floor(Number_Of_Neurons*(Number_Of_Neurons-1)*this.GeneralConnectivity);
			}//-------------------------------------------------------------------------
			else if (Liquid_Architecture==17) { //Click --> full connectivity
				Connections = Number_Of_Neurons * (Number_Of_Neurons-1);
			}
		}
		
		
		public RecurrentNetwork4DamageLSM copy(){
			RecurrentNetwork4DamageLSM source = (RecurrentNetwork4DamageLSM) this.MemberwiseClone();
			return(source);
		}//----------------------------------------------------------------------------------
		
		public void save(int iteration){
			Stream fileStream = new FileStream("RecurrentNetworkParam"+iteration.ToString()+".dat", FileMode.Create,FileAccess.ReadWrite, FileShare.None);
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
		
		public RecurrentNetwork4DamageLSM load(string args){
			Stream fileStream = new FileStream(args, FileMode.Open,FileAccess.Read, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			RecurrentNetwork4DamageLSM Param = (RecurrentNetwork4DamageLSM) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			return Param;
		}//----------------------------------------------------------------------------------

		
		
	}
}
