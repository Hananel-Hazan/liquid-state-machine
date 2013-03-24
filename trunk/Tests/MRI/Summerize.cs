
using System;
using System.IO;
using System.Text.RegularExpressions;


public class Reports
{
	public Reports()
	{
		
	}
	//---------------------------------------------------------------------
	
	
	
	public void SmallSummery(ref globalParam Param){
		string path=Param.test.OutputDir.ToString()+Param.CurrentGroup.ToString()+"//";
		
		TextWriter foe = new StreamWriter(@Directory.GetCurrentDirectory()+"//sum-Error-"+Param.CurrentGroup.ToString()+".txt");
		TextWriter fo = new StreamWriter(@Directory.GetCurrentDirectory()+"//sum-P-"+Param.CurrentGroup.ToString()+".txt");
		
		for (int i=0; i< Param.test.NumOfVoxels ; i++)
		{
			TextReader tr = new StreamReader(path+"Voxel#-"+i.ToString()+".txt");
			string input = null;
			int negCount=0,posCount=0,lineCounter=0;
			double ansPos=0,ansNeg=0;
			while ((input = tr.ReadLine()) != null)
			{
				if (lineCounter==0) {lineCounter++; continue;}
				input.Replace(',',' ');
				string[] word = input.Split(' ');
				if (Convert.ToDouble(word[2])==1) {ansPos+=Convert.ToDouble(word[8]); posCount++;}
				if (Convert.ToDouble(word[2])==-1) {ansNeg+=Convert.ToDouble(word[8]); negCount++;}
				lineCounter++;
			}
			//fo.Write("{0}\t",(group1+group2)/4);
			
			double ans=0;
			if ((posCount==0)||(negCount==0)) ans = 0;
			else{
				if (((ansPos/posCount)>0)&&((ansNeg/negCount)<0))
					ans = ((Math.Abs(ansPos/posCount)+Math.Abs(ansNeg/negCount))/2);
				if (((ansPos/posCount)<0)&&((ansNeg/negCount)>0))
					ans = ((Math.Abs(ansPos/posCount)+Math.Abs(ansNeg/negCount))/2);
			}
			
			fo.Write("{0}\t",ans);
			tr.Close();
		}
		foe.Close();
		fo.Close();
	}
	//---------------------------------------------------------------------
	
	
	
	
	public void SummerizeAll(ref globalParam Param)
	{
		string path=@Directory.GetCurrentDirectory()+"//";
		DirectoryInfo di = new DirectoryInfo(path);
		FileInfo[] rgFiles = di.GetFiles("sum-P-*.txt");
		double[] sum = new double[Param.test.NumOfVoxels];
		for (int i=0;i<Param.test.NumOfVoxels ; i++ ) {	sum[i] = 0;	}
		int counter=0;
		foreach(FileInfo fi in rgFiles)
		{
			Console.Write("{0} => ",fi.Name);
			TextReader tr = new StreamReader(fi.FullName);
			string input = null;
			counter++;
			while ((input = tr.ReadLine()) != null)
			{
				string[] rows = input.Split("\t".ToCharArray());
				for (int i=0;i<rows.Length ; i++ ) {
					if (rows[i]=="") Console.WriteLine(i); 
					else sum[i]+= Convert.ToDouble(rows[i]);
				}
			}
		}
		
		TextWriter fo = new StreamWriter(@Directory.GetCurrentDirectory()+"//SumOfAll.txt");
		int good=0,bad=0;
		for (int i=0;i<sum.Length ; i++ ) {
			sum[i]=sum[i]/counter;
			fo.Write("{0}\t",sum[i]);
			if (sum[i]>=1) good++;
			if (sum[i]<=0) bad++;
		}
		fo.WriteLine("");
		fo.WriteLine("Nomber of 100% is = {0}",good);
		fo.WriteLine("Nomber of 0% is = {0}",bad);
		
		fo.Close();
	}
}