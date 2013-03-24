/*
 * Created by SharpDevelop.
 * User: hhazan01
 * Date: 16/06/2011
 * Time: 12:57
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
	/// Description of TestParameters.
	/// </summary>
	
	[Serializable()]
	public class TestParameters4DamageLSM : testParam
	{
		public TestParameters4DamageLSM()
		{
			//Damage in the LSM
			Repetition = 50;
			Numbers_of_Inputs = 20;
			Input_Pattern_Size = 100;
			DirName = @Directory.GetCurrentDirectory()+@"//Noise=";
			NoisePercent = 11;
						
			
			// Activity Test in Liquid
			Activity_Test_Running_Time = 5000 ;
			Activity_Test_Time_Between_Inputs = 50 ;
			Activity_Test_Greace_Time = 50 ;
			Activity_Test_MinSignalLength = 1 ;
			Activity_Test_MaxSignalLength = 20 ;
			Numbers_of_Inputs_For_Activity_Check=3;

		}
		
		public TestParameters4DamageLSM copy(){
			TestParameters4DamageLSM source = (TestParameters4DamageLSM) this.MemberwiseClone();
			return(source);
		}//----------------------------------------------------------------------------------
		
		public void save(int iteration){
			Stream fileStream = new FileStream("TestParam"+iteration.ToString()+".dat", FileMode.Create,FileAccess.ReadWrite, FileShare.None);
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
		
		public TestParameters4DamageLSM load(string args){
			Stream fileStream = new FileStream(args, FileMode.Open,FileAccess.Read, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			TestParameters4DamageLSM Param = (TestParameters4DamageLSM) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			return Param;
		}//----------------------------------------------------------------------------------

		
		
		
	}
}
