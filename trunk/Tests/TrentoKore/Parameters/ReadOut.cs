/*
 * Created by SharpDevelop.
 * User: hhazan01
 * Date: 16/06/2011
 * Time: 12:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Liquid_Detector.Tests.TrentoKore.Parameters
{
	/// <summary>
	/// Description of ReadOut.
	/// </summary>
	[Serializable()]
	public class ReadOut4TrentoKore : Readout
	{
		public ReadOut4TrentoKore (ref Input2liquid inputParam, int input_duration,int addtional_duration){
			/*
		 MODEL
		 1 = Perceptron
		 2 = ADELINE
		 3 = Regilar Back-Propagation
		 4 = Manhattan Back-Propagation
		 5 = ResilientPropagation Back-Propagation
		 6 = Scaled Conjugate Gradient
		 7 = SVM
		 8 = LIF-Tempatron-Like
	 	 9 = Encog SVM
			 */
			ReadOut_Unit = new int[]{5};
			ReadOut_unit_Model_AP_or_Sequ = new int[]{1}; // 1 = Activation pattern, 2 = Sequence pattern
			ReadOut4EveryWindow = new int[]{1}; // 1 = One readout to all window, 2 = every window have different readout
			
			// Readout Unit parapeters
			LSM_1sec_interval = 50;
			LSM_Runing_Interval = 10; // in seconds
			
			LSM_AdjustmentTime= 0 ;
			ReadoutU_Window_Size = 50 ;
			ReadoutU_Disctance_Between_Windows = 20 ;
			ReadoutU_Window_Shifting = 0 ; // 0= no shifting , for any number, exmple 4 the window will shift random from -4 to +4
			ReadoutU_epoch = 200 ;
			Readout_Max_Error = 0.1;
			Readout_Negative = -1;
			ReadOut_Unit_HiddenLayerSize = 0.35 ; // 0.3 = 30% from Liquid output size
			ReadOut_Unit_outputSize = 1;
			Readout_Activity_during_Input_time = 1 ; // the readout will receve the activity of the liquid during the stimuli input? 0==no , 1 == yes
			Approximate_OR_Classification = 1 ; // 1 = Aproximate (output must be equal to the number of windows), 2 = classifay
			
			// set to ignore the activity during the input phase
			//------------------Read Out Params-------------------
			
			
			//--------------------------------------------
		}
		
		public ReadOut4TrentoKore copy(){
			ReadOut4TrentoKore source = (ReadOut4TrentoKore) this.MemberwiseClone();
			return(source);
		}//----------------------------------------------------------------------------------
		
		public void save(int iteration){
			Stream fileStream = new FileStream("ReadOutParam"+iteration.ToString()+".dat", FileMode.Create,FileAccess.ReadWrite, FileShare.None);
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
		
		public ReadOut4TrentoKore load(string args){
			Stream fileStream = new FileStream(args, FileMode.Open,FileAccess.Read, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			ReadOut4TrentoKore Param = (ReadOut4TrentoKore) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			return Param;
		}//----------------------------------------------------------------------------------

		
		
	}
}
