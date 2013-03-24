/*
 * Created by SharpDevelop.
 * User: hhazan01
 * Date: 16/06/2011
 * Time: 14:00
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
	/// Description of Input2Liquid.
	/// </summary>
	[Serializable()]
	public class Input4Sound : Input2liquid
	{
		public Input4Sound()
		{
			LSM_Ratio_Of_Input_Interval = new int[]{1,10} ;  // {1,10} = 1 input every 10 cycle of the liquid
			silence_or_repeted_Input_between_ratio = 1 ; // what to do in the gaps that cause the input_raio {1,10} -> 0 = silent all 10 cycle or 1 = repetition of the previous input bit all the 10 cycle
			Looping_Input = 1 ; // -1 = inf looping on the input untill lsm finish run, 1 = the input will be given 1 time during the running of the LSM, 2 =  TWO times that the input will be repeated
			is_a_live_signal = new double[]{0,0.1} ;  // [ 1==yes , 0==no ; to how much input neurons? in %,0.3=30% from input neurons]
			inverse_Signal = new double[]{1,0.2};  //[ 1==yes , 0==no ; to how much input neurons? in %,0.3=30% from input neurons]

		}
		
		public override int ComputeInputTime(int time){
			return (LSM_Ratio_Of_Input_Interval[0] * LSM_Ratio_Of_Input_Interval[1] * time);
		}
		
		public Input4Sound copy(){
			Input4Sound source = (Input4Sound) this.MemberwiseClone();
			return(source);
		}//----------------------------------------------------------------------------------
		
		public void save(int iteration){
			Stream fileStream = new FileStream("Input2LiquidParam"+iteration.ToString()+".dat", FileMode.Create,FileAccess.ReadWrite, FileShare.None);
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
		
		public Input4Sound load(string args){
			Stream fileStream = new FileStream(args, FileMode.Open,FileAccess.Read, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			Input4Sound Param = (Input4Sound) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			return Param;
		}//----------------------------------------------------------------------------------

		
	}
}
