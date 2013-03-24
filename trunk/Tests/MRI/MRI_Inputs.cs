
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace Inputs
{
	[Serializable()]
	public class MRI_Input
	{
		// vers
		int[] timeline;
		globalParam.Data[][] data;
		//
		
		
		public MRI_Input(){
		}
		
		public void estimateInput(ref globalParam Param){
//		int TestInEachSession;
			TextReader InputReader = new StreamReader(@Param.test.InputDir+@Param.test.inputFiles);
			TextReader SesstionReader = new StreamReader(@Param.test.InputDir+@Param.test.SessionFile);
			TextReader StimuliReader = new StreamReader(@Param.test.InputDir+@Param.test.StimulyFile);
			
			// estimate the all expirament
			string input;
			int lines=0,mark=0,count=0;

			// read the number of sesstion from session file
			while ((input = SesstionReader.ReadLine()) != null){
				if (lines==0){count=1; mark= Convert.ToInt32(input); }
				if (Convert.ToInt32(input)!=mark){ count++; mark=Convert.ToInt32(input);}
				lines++;
			}
			this.timeline = new int[lines];
			Param.NumberOfSession = new int[count];
			for(int i=0 ; i < count ; i++) Param.NumberOfSession[i] = i ;
			SesstionReader = new StreamReader(@Param.test.InputDir+@Param.test.SessionFile);
			//
			
			// Read the number of test that made
			lines=0; mark=0; count=0;
			while ((input = StimuliReader.ReadLine()) != null){
				input=input.Replace(" ","");
				if (lines==0){count=0; mark = Convert.ToInt32(input); }
				if (Convert.ToInt32(input)!=mark){ count++; mark=Convert.ToInt32(input);}
				//else Param.NumberOfTimeInEachTest++;
				timeline[lines]=mark;
				lines++;
			}
			Param.NumberOfTestIneachSesstion = (count/Param.NumberOfSession.Length);
			Param.NumberOfTimeInEachTest=(lines/count);
			StimuliReader = new StreamReader(@Param.test.InputDir+@Param.test.StimulyFile);
			//
			
			// Read how meny Voxels there is in one shot
			input = InputReader.ReadLine();
			string[] rows = input.Split(" ".ToCharArray());
			Param.test.NumOfVoxels = rows.Length;
			InputReader = new StreamReader(@Param.test.InputDir+@Param.test.inputFiles);
			//
		}


		public void ReadAllData(ref globalParam Param){
			
			TextReader InputReader = new StreamReader(@Param.test.InputDir+@Param.test.inputFiles);
			
			int flag=0;
			int[] testTimeline =  new int[this.timeline.Length];
			this.timeline.CopyTo(testTimeline,0);
			int time=0,time2=0,sessionMod=(timeline.Length/Param.NumberOfSession.Length),dataSet=(timeline.Length/Param.NumberOfTestIneachSesstion);
			double previewsGroup = 0;
			
			for(int j=0 ; j<testTimeline.Length ; j++){
				flag=0;
				for(int i =0 ; i<Param.Groups.Length ; i++)
					for (int t=1 ; t<Param.Groups[i].Length ; t++){
					if (testTimeline[j]==Param.Groups[i][t]) {
						flag++;
						previewsGroup=Param.Groups[i][t];
					}
//					if ((testTimeline[j]==1000000)&&(previewsGroup>0)) {
//						flag++;
//						testTimeline[j]=9; // just a number~!
//					}
				}
				if (flag==0) {previewsGroup=0; testTimeline[j]=0;}
			}

			
			int[] test_in_each_session = new int[timeline.Length/sessionMod];
			for(int session = 0 ; session < test_in_each_session.Length ; session++){
				for(int timeInSession = 0 ; timeInSession < sessionMod ; timeInSession++){
					if ((session==0)&&(timeInSession<Param.test.IgnoreUntilLine)) continue;
					time = (session*sessionMod)+timeInSession;
					if (testTimeline[time-1] == 0) continue;
//					if (testTimeline[time-1] == 9) continue;
					if (testTimeline[time]!=testTimeline[time-1])
						test_in_each_session[session]++;
				}
			}
			time=0;
			
			
			this.data = new globalParam.Data[Param.NumberOfSession.Length][];
			
			int testNumber=-1,oldsessionNumber=0,Gr=0,firstTime=0;
			string input;
			
			while ((input = InputReader.ReadLine()) != null){
				if (time==0) {
					firstTime=1;
				}
				
				if (time>Param.test.IgnoreUntilLine) {
					time2++;
					if (testTimeline[time] > 0 ){
						if ((oldsessionNumber!=(time2/sessionMod))||(firstTime==1)){
							oldsessionNumber=(time2/sessionMod);
							Console.WriteLine("sesstion = {0}",time2/sessionMod);
							//this.data[time2/SesstionMod] = new globalParam.DataStruct[GroupToTest];
							this.data[time2/sessionMod] = new globalParam.Data[test_in_each_session[time2/sessionMod]];
							Gr=-1;
						}
//						if ((testNumber==((Param.NumberOfTimeInEachTest*2)-1))||(firstTime==1)) {
						if ((testNumber==((Param.NumberOfTimeInEachTest)-1))||(firstTime==1)) {
							testNumber=0;
							firstTime=0;
							Gr++;
//							this.data[time2/sessionMod][Gr].Input = new double[Param.NumberOfTimeInEachTest*2,Param.NumOfVoxels];
							this.data[time2/sessionMod][Gr].Input = new double[Param.NumberOfTimeInEachTest,Param.test.NumOfVoxels];
//							this.data[time2/sessionMod][Gr].Target;
							this.data[time2/sessionMod][Gr].Tag = new int();
							this.data[time2/sessionMod][Gr].Tag = testTimeline[time];
							for(int i =0 ; i<Param.Groups.Length ; i++)
								for (int t=1 ; t<Param.Groups[i].Length ; t++)
									if (testTimeline[time]==Param.Groups[i][t]) data[time2/sessionMod][Gr].Target = new double[]{Param.Groups[i][0]};
							Console.WriteLine("Test = {0}",Gr);
						}else{
							testNumber++;
						}
						string[] rows = input.Split(" ".ToCharArray());
						for(int val=0; val<rows.Length ; val++)
							this.data[time2/sessionMod][Gr].Input[testNumber,val]=Convert.ToDouble(rows[val]);
						
					}
				}
				time++;
			}
			InputReader.Close();
		}
		
		public void ReturnEpicData(ref globalParam Param){
			
			if (Param.TestData == null){
				//int Factor = Param.InputFactor;
				Utils_Functions.Manipulation_On_Inputs inputTrasform = new Utils_Functions.Manipulation_On_Inputs();
				Param.TestData = new globalParam.Data[(Param.TestGrpup.GetLength(1)*this.data[Param.TestGrpup[Param.CurrentGroup,0]].Length)];
				Param.LearnData = new globalParam.Data[Param.LearnGroup.GetLength(1)*this.data[Param.LearnGroup[Param.CurrentGroup,0]].Length];
				int GroupToTest=0;
				
				for(int i =0 ; i<Param.Groups.Length ; i++)
					for (int t=1 ; t<Param.Groups[i].Length ; t++){
					GroupToTest++;
				}
				
				int[] TestDataSesstions = new int[Param.TestGrpup.GetLength(1)];
				int[] LearnDataSessions = new int[Param.LearnGroup.GetLength(1)];
				
				for (int i = 0; i < LearnDataSessions.Length ; i++) {
					LearnDataSessions[i] = Param.LearnGroup[Param.CurrentGroup,i];
				}
				for (int i = 0; i < TestDataSesstions.Length ; i++) {
					TestDataSesstions[i] = Param.TestGrpup[Param.CurrentGroup,i];
				}
//				int[] TestDataSesstions = Param.TestGrpup[Param.CurrentGroup];
//				int[] LearnDataSessions = Param.LearnGroup[Param.CurrentGroup];
				
				int TestDataMone=0,LearnDataMone=0;
				
				for (int session=0; session<this.data.Length ; session++ ) {
					
					int flag=0;
					for(int check=0 ; check<TestDataSesstions.Length ; check++)
						if (TestDataSesstions[check]==session) flag=1;
					
					for (int testInSesstion=0 ; testInSesstion<this.data[session].Length; testInSesstion++){
						if (flag==1){
							// session is TestData
							Param.TestData[TestDataMone].Tag = new int();
							Param.TestData[TestDataMone].Target = new double[1];
							
							inputTrasform.MRIinput_Method_2(ref Param,out Param.TestData[TestDataMone].Input,ref this.data[session][testInSesstion].Input);
							
							Param.TestData[TestDataMone].Tag =session; // this.data[session][testInSesstion].Tag;
							Param.TestData[TestDataMone].Target = this.data[session][testInSesstion].Target;
							
							TestDataMone++;
						}else{
							// session is LearnData
							Param.LearnData[LearnDataMone].Tag = new int();
							Param.LearnData[LearnDataMone].Target = new double[1];
							
							inputTrasform.MRIinput_Method_2(ref Param,out Param.LearnData[LearnDataMone].Input,ref this.data[session][testInSesstion].Input);
							
							Param.LearnData[LearnDataMone].Tag = session; // this.data[session][testInSesstion].Tag;
							Param.LearnData[LearnDataMone].Target = this.data[session][testInSesstion].Target;
							
							LearnDataMone++;
						}
					}
				}
			}else{
				globalParam.Data[] tempTestData = new globalParam.Data[Param.TestData.Length*Param.test.howMenyMoreVoxles];
//				globalParam.Data[] tempTestData = new globalParam.Data[Param.TestData.Length];
				Param.TestData.CopyTo(tempTestData,0);
				
				globalParam.Data[] tempLearnData = new globalParam.Data[Param.LearnData.Length*Param.test.howMenyMoreVoxles];
				int OldIteration = Param.iteration;
				
				for (int i = 0 ; i < Param.test.howMenyMoreVoxles ; i++) {
					Param.iteration++;
					if (Param.iteration>=Param.test.NumOfVoxels) Param.iteration=0;
					Param.TestData.CopyTo(tempTestData,i*Param.TestData.Length);
					Param.LearnData.CopyTo(tempLearnData,i*Param.LearnData.Length);
					Param.TestData = null;
					Param.LearnData = null;
					this.ReturnEpicData(ref Param);
				}
				Param.TestData = tempTestData;
				Param.LearnData = tempLearnData;
				Param.iteration = OldIteration;
			}
			
			Console.WriteLine("Finish Fatchig Voxel");
		}
		
		public void NormelizeInput(ref globalParam Param){
			for (int i = 0 ; i < this.data.Length ; i++ ) {
				for (int j = 0 ; j < this.data[i].Length ; j++)
					for (int pixel = 0 ; pixel < this.data[i][j].Input.GetLength(1) ; pixel++ ){
					cumulateor group = new cumulateor();
					for (int time = 0 ; time < this.data[i][j].Input.GetLength(0) ; time++ )
						group.Add(this.data[i][j].Input[time,pixel]);
					
					double avg = group.Return_Average();
					double std = group.Return_Standard_Deviation();

					for (int time = 0 ; time < this.data[i][j].Input.GetLength(0) ; time++ )
						this.data[i][j].Input[time,pixel] = (avg-this.data[i][j].Input[time,pixel])/std;
				}
			}
		}
		
		public void CollectVoxlsFromSomeSession(ref globalParam.Data[] data)
		{
			// if there is more voxel in the same sesstion with the same target, collect them together
			
			int[] sesstios =  new int[0];
			for (int i = 0; i < data.Length; i++){
				int flag=0;
				for (int t = 0; t < sesstios.Length; t++) {
					if (data[i].Tag == sesstios[t]) {flag =1; continue;}
				}
				if (flag==0)
				{
					int[] temp = (int[]) sesstios.Clone();
					sesstios = new int[temp.Length+1];
					temp.CopyTo(sesstios,0);
					sesstios[temp.Length]=data[i].Tag;
				}
			}
			
			globalParam.Data[] tempData = new globalParam.Data[data.Length];
			data.CopyTo(tempData,0);
			
			data = new globalParam.Data[sesstios.Length*2];
			
			int currentSesstion=0;
			for (int i = 0; i < sesstios.Length ; i++){
				for (int d = 0; d < tempData.Length; d++) {
					if (tempData[d].Tag==sesstios[i]) {
						if (tempData[d].Target[0]==1) {
							data[currentSesstion].Tag=sesstios[i];
							data[currentSesstion].Target[0] = 1;
							if(data[currentSesstion].Input==null){
								data[currentSesstion].Input = (double[,]) tempData[d].Input.Clone();
							}else{
								double[,] tempInput = (double[,]) data[currentSesstion].Input.Clone();
//								data[currentSesstion].Input = new double[tempInput.Length+ tempData[d].Input.Length][];
//								int tempCounter=0;
//								for (int inputCounter = 0; inputCounter < data[currentSesstion].Input.Length; inputCounter++) {
//									if (inputCounter<tempData[d].Input.Length) {
//										data[currentSesstion].Input[inputCounter] = new double[tempData[d].Input[inputCounter].Length];
//										tempData[d].Input[inputCounter].CopyTo(data[currentSesstion].Input[inputCounter],0);
//									}else{
//										data[currentSesstion].Input[inputCounter] = new double[tempInput[tempCounter].Length];
//										tempInput[tempCounter].CopyTo(data[currentSesstion].Input[inputCounter],0);
//										tempCounter++;
//									}
//									
//								}
							}
						}else if(tempData[d].Target[0]==-1) {
							data[currentSesstion+1].Tag=sesstios[i];
							data[currentSesstion+1].Target[0] = -1;
							if(data[currentSesstion+1].Input==null){
								data[currentSesstion+1].Input = (double[,]) tempData[d].Input.Clone();
							}else{
								double[,] tempInput = (double[,]) data[currentSesstion+1].Input.Clone();
//								data[currentSesstion+1].Input = new double[tempInput.Length+ tempData[d].Input.Length][];
//								int tempCounter=0;
//								for (int inputCounter = 0; inputCounter < data[currentSesstion+1].Input.Length; inputCounter++) {
//									if (inputCounter<tempData[d].Input.Length) {
//										data[currentSesstion+1].Input[inputCounter] = new double[tempData[d].Input[inputCounter].Length];
//										tempData[d].Input[inputCounter].CopyTo(data[currentSesstion+1].Input[inputCounter],0);
//									}else{
//										data[currentSesstion+1].Input[inputCounter] = new double[tempInput[tempCounter].Length];
//										tempInput[tempCounter].CopyTo(data[currentSesstion+1].Input[inputCounter],0);
//										tempCounter++;
//									}
//									
//								}
							}
						}
						
					}
				}
				currentSesstion+=2;
			}
			
		}
		
		
		public MRI_Input copy(){
			MRI_Input source = (MRI_Input) this.MemberwiseClone();
			return(source);
		}
		
		public void save(int num){
			Stream fileStream = new FileStream("MRI_Readout_obj"+num.ToString()+".dat", FileMode.Create,FileAccess.ReadWrite, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			binaryFormater.Serialize(fileStream,this.copy());
			fileStream.Flush();
			fileStream.Close();
		}
		
		public MRI_Input load(int num)
		{
			Stream fileStream = new FileStream("MRI_Readout_obj"+num.ToString()+".dat", FileMode.Open,FileAccess.Read, FileShare.None);
			BinaryFormatter binaryFormater = new BinaryFormatter();
			MRI_Input Value = (MRI_Input) binaryFormater.Deserialize(fileStream);
			fileStream.Close();
			return Value;
		}
		
		public void deleteFile(int num)
		{
			File.Delete("MRI_Readout_obj"+num.ToString()+".dat");
		}
		
	}//---------------------
}