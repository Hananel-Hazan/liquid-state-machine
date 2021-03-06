﻿/*
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


namespace Liquid_Detector.Tests.Sound.Parameters
{
	/// <summary>
	/// Description of TestParameters.
	/// </summary>
	
	[Serializable()]
	public class TestParameters4Sound : testParam
	{
		public TestParameters4Sound()
		{
			// Sound Files
			soundFiles_toLearn = new System.String[]{ @"D:\Rimon.Docs\Data To Process\Sound\Learn" };
			soundFiles_toTest = new System.String[]{ @"D:\Rimon.Docs\Data to Process\Sound\Test" };
			MaxLevel = 50;
			dataQunta = 1;
//			Input_Pattern_Size = input_pattern;

		}
		
		public TestParameters4Sound copy(){
			TestParameters4Sound source = (TestParameters4Sound) this.MemberwiseClone();
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
		
		public TestParameters4Sound load(string args){
			Stream fileStream = new FileStream(args, FileMode.Open,FileAccess.Read, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			TestParameters4Sound Param = (TestParameters4Sound) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			return Param;
		}//----------------------------------------------------------------------------------

		
		
		
	}
}
