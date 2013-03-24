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
using System.Collections.Generic;
using System.Text;



/// <summary>
/// Description of Program.
/// </summary>
public class Program
{
	
	public static void Main(string[] args)
	{

//		string Source=@"D:\Rimon.Docs\Programing\C#\2011\Liquid Detector\Configuration\DamageLSM\";
//		string Target=@"D:\Rimon.Docs\Programing\C#\2011\Liquid Detector\Liquid Detector\bin\Debug\";
//		DirectoryInfo di = new DirectoryInfo(Source);
//		FileInfo[] rgFiles = di.GetFiles("*.ini");
//		foreach (var element in rgFiles) {
//			File.Copy(Source+element.ToString(),Target+element.ToString(),true);
//		}
//		Liquid_Detector.Tests.DamageLSM.Program.DmageLSM_Main(args);
		


//		string Source=@"D:\Rimon.Docs\Programing\C#\2011\Liquid Detector\Configuration\TIMIT\";
//		string Target=@"D:\Rimon.Docs\Programing\C#\2011\Liquid Detector\Liquid Detector\bin\Debug\";
//		DirectoryInfo di = new DirectoryInfo(Source);
//		FileInfo[] rgFiles = di.GetFiles("*.ini");
//		foreach (var element in rgFiles) {
//			File.Copy(Source+element.ToString(),Target+element.ToString(),true);
//		}
		Liquid_Detector.Tests.Sound.TIMIT.TIMIT.MainProgram(args);
		
		
		
		
//		string Source=@"D:\Rimon.Docs\Programing\C#\2011\Liquid Detector\Configuration\Kore\";
//		string Target=@"D:\Rimon.Docs\Programing\C#\2011\Liquid Detector\Liquid Detector\bin\Debug\";
//		DirectoryInfo di = new DirectoryInfo(Source);
//		FileInfo[] rgFiles = di.GetFiles("*.ini");
//		foreach (var element in rgFiles) {
//			File.Copy(Source+element.ToString(),Target+element.ToString(),true);
//		}
//
//		args= new string[]{
//			@"D:\Downloads\1\block_stimuli.txt",
//			@"D:\Downloads\1\Activation.Pattern"
//		};
//		Liquid_Detector.Tests.TrentoKore.TrentoKore.Liquid(args);
//
//		args= new string[]{
//			@"D:\Downloads\1\Activation.Pattern",
//			@"D:\Downloads\1\1.txt",
//			@"D:\Downloads\1\1.weights",
//			"train",
//		};
//		Liquid_Detector.Tests.TrentoKore.TrentoKore.MLP(args);
//
//		args= new string[]{
//			@"D:\Downloads\1\Activation.Pattern",
//			@"D:\Downloads\1\1.weights",
//			@"D:\Downloads\1\1.out.txt",
//			"test",
//		};
//		Liquid_Detector.Tests.TrentoKore.TrentoKore.MLP(args);
//		//		Console.ReadKey();
		
		
//		string Source=@"D:\Rimon.Docs\Programing\C#\2011\Liquid Detector\Configuration\Kore\";
//		string Target=@"D:\Rimon.Docs\Programing\C#\2011\Liquid Detector\Liquid Detector\bin\Debug\";
//		DirectoryInfo di = new DirectoryInfo(Source);
//		FileInfo[] rgFiles = di.GetFiles("*.ini");
//		foreach (var element in rgFiles) {
//			File.Copy(Source+element.ToString(),Target+element.ToString(),true);
//		}
		
//		if (args.Length==2){
//			Liquid_Detector.Tests.TrentoKore.TrentoKore.Liquid(args);
//		}else if (args.Length==3){
//			Liquid_Detector.Tests.TrentoKore.TrentoKore.MLP(args);
//		}else if (args.Length==4){
//			Liquid_Detector.Tests.TrentoKore.TrentoKore.MLP(args);
//		}else{
//			Console.WriteLine(" cannot run witout arguments ");
//		}
	}
}