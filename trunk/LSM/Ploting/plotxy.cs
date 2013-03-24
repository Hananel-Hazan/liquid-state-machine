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
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;


/// <summary>
/// Description of plotxy.
/// </summary>
public partial class plotxy : Form
{
	public PointPairList list1,list2,list3;
	
	public plotxy()
	{
		//
		// The InitializeComponent() call is required for Windows Forms designer support.
		//
		InitializeComponent();
		
		//
		// TODO: Add constructor code after the InitializeComponent() call.
		//
	}
	
	public void plotxy_Load( object sender, EventArgs e )
	{
		// get a reference to the GraphPane
		GraphPane myPane = zgc.GraphPane;

		// Set the Titles
		myPane.Title.Text = "Liquid Activity";
		myPane.YAxis.Title.Text = "Neurons";
		myPane.XAxis.Title.Text = "Time";
		
		// Generate a red curve with diamond
		// symbols, and "Porsche" in the legend
		LineItem myCurve = myPane.AddCurve( "Output Units", list1, Color.Red, SymbolType.Diamond );
		

		// Generate a blue curve with circle
		// symbols, and "Piper" in the legend
		LineItem myCurve2 = myPane.AddCurve( "Input Units", list2, Color.Blue, SymbolType.Diamond );
		
		// Generate a blue curve with circle
		// symbols, and "Piper" in the legend
		LineItem myCurve3 = myPane.AddCurve( "Stimuli", list3, Color.Black, SymbolType.Diamond );
		
		myCurve.Symbol.Size = 3;
		myCurve2.Symbol.Size = 3;
		myCurve3.Symbol.Size = 3;


		// Don't display the line (This makes a scatter plot)
		myCurve.Line.IsVisible = false;
		myCurve2.Line.IsVisible = false;
		myCurve3.Line.IsVisible = false;
		// Hide the symbol outline
		myCurve.Symbol.Border.IsVisible = false;
		myCurve2.Symbol.Border.IsVisible = false;
		myCurve3.Symbol.Border.IsVisible = false;
		// Fill the symbol interior with color
		myCurve.Symbol.Fill = new Fill( Color.Firebrick );
		myCurve2.Symbol.Fill = new Fill( Color.Green );
		myCurve3.Symbol.Fill = new Fill( Color.Black );

		
		// Tell ZedGraph to refigure the
		// axes since the data have changed
		zgc.AxisChange();
		// Make sure the Graph gets redrawn
		zgc.Invalidate();

	}
	
	private void plotxy_Resize( object sender, EventArgs e )
	{
		SetSize();
	}

	private void SetSize()
	{
		zgc.Location = new Point( 10, 10 );
		// Leave a small margin around the outside of the control
		zgc.Size = new Size( this.ClientRectangle.Width - 20,this.ClientRectangle.Height - 20 );
	}
	
	
	public void loadData(ref bool[][,] matrix1,ref bool[][,] matrix2 , int count,ref globalParam Param){
		
		list1 = new PointPairList();
		list2 = new PointPairList();

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix1[count].GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix1[count].GetLength(1) ; t++) {
			if (matrix1[count][i,t]==true) list1.Add(t,i);
		}
		
		for ( int i = 0; i < matrix2[count].GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix2[count].GetLength(1) ; t++) {
			if (matrix2[count][i,t]==true) list2.Add(t,matrix1[count].GetLength(0)+i);
		}

		
	}
	
	public void loadData(ref double[,] matrix1,ref double[][] matrix2,ref globalParam Param){
		
		list1 = new PointPairList();
		list2 = new PointPairList();

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix1.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix1.GetLength(1) ; t++) {
			if (matrix1[i,t]>0) list1.Add(t,i);
		}
		
		for ( int i = 0; i < matrix2.Length ; i++ )
			for (int t = 0 ; t < matrix2[i].Length ; t++) {
			if (matrix2[i][t]>0)
				list2.Add(t,matrix1.GetLength(0)+i+5);
		}

		
	}
	
	
	public void loadData(ref bool[][,] matrix1,ref bool[][,] matrix2,ref bool[][,] matrix3, int count, ref globalParam Param){
		
		list1 = new PointPairList();
		list2 = new PointPairList();
		list3 = new PointPairList();

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix1[count].GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix1[count].GetLength(1) ; t++) {
			if (matrix1[count][i,t]==true) list1.Add(t,i);
		}
		
		for ( int i = 0; i < matrix2[count].GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix2[count].GetLength(1) ; t++) {
			if (matrix2[count][i,t]==true) list2.Add(t,matrix1[count].GetLength(0)+i);
		}
		
		for ( int i = 0; i < matrix3[count].GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix3[count].GetLength(1) ; t++) {
			if (matrix3[count][i,t]==true) list3.Add(t,matrix1[count].GetLength(0)+matrix2[count].GetLength(0)+i);
		}
		
	}
	
	public void loadData(ref bool[][,] matrix1,ref bool[][] matrix2,ref bool[][,] matrix3, int count, ref globalParam Param){
		
		list1 = new PointPairList();
		list2 = new PointPairList();
		list3 = new PointPairList();

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix1[count].GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix1[count].GetLength(1) ; t++) {
			if (matrix1[count][i,t]){
				if (matrix2[count][i])
					list1.Add(t,i);
				else
					list2.Add(t,i);
			}
		}
		
		for ( int i = 0; i < matrix3[count].GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix3[count].GetLength(1) ; t++) {
			if (matrix3[count][i,t]) list3.Add(t,matrix1[0].GetLength(0)+i);
		}
		
	}
	
	public void loadData(ref double[,] matrix1,ref double[,] matrix2,ref double[][] matrix3, ref globalParam Param){
		
		list1 = new PointPairList();
		list2 = new PointPairList();
		list3 = new PointPairList();

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix1.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix1.GetLength(1) ; t++) {
			if (matrix1[i,t]>0) list1.Add(t,i);
		}
		
		for ( int i = 0; i < matrix2.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix2.GetLength(1) ; t++) {
			if (matrix2[i,t]>0) list2.Add(t,matrix1.GetLength(0)+i);
		}
		
		for ( int i = 0; i < matrix3.Length ; i++ )
			for (int t = 0 ; t < matrix3[i].Length ; t++) {
			if (matrix3[i][t]>0) list3.Add(t,matrix1.GetLength(0)+matrix2.GetLength(0)+i+5);
		}
		
	}
	
	public void loadData(ref double[,] matrix1,ref double[,] matrix2,ref double[] matrix3, ref globalParam Param){
		
		list1 = new PointPairList();
		list2 = new PointPairList();
		list3 = new PointPairList();

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix1.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix1.GetLength(1) ; t++) {
			if (matrix1[i,t]>0) list1.Add(t,i);
		}
		
		for ( int i = 0; i < matrix2.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix2.GetLength(1) ; t++) {
			if (matrix2[i,t]>0) list2.Add(t,matrix1.GetLength(0)+i);
		}
		
		for (int t = 0 ; t < matrix3.Length ; t++) {
			if (matrix3[t]>0) list3.Add(t,matrix1.GetLength(0)+matrix2.GetLength(0)+5);
		}
		
	}
	
	
	public void loadData(ref bool[,] matrix1,ref bool[,] matrix2,ref bool[,] matrix3, ref globalParam Param){
		
		list1 = new PointPairList();
		list2 = new PointPairList();
		list3 = new PointPairList();

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix1.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix1.GetLength(1) ; t++) {
			if (matrix1[i,t]) list1.Add(t,i);
		}
		
		for ( int i = 0; i < matrix2.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix2.GetLength(1) ; t++) {
			if (matrix2[i,t]) list2.Add(t,matrix1.GetLength(0)+i);
		}
		
		for ( int i = 0; i < matrix3.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix3.GetLength(1) ; t++) {
			if (matrix3[i,t]) list3.Add(t,matrix1.GetLength(0)+matrix2.GetLength(0)+5);
		}

	}
	
	public void loadData(ref bool[,] matrix1,ref bool[] matrix2,ref bool[,] matrix3, ref globalParam Param){
		
		list1 = new PointPairList();
		list2 = new PointPairList();
		list3 = new PointPairList();

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix1.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix1.GetLength(1) ; t++) {
			if (matrix1[i,t]){
				if (matrix2[i])
					list2.Add(t,i);
				else
					list1.Add(t,i);
			}
		}
		
		for ( int i = 0; i < matrix3.GetLength(0) ; i++ )
			for (int t = 0 ; t < matrix3.GetLength(1) ; t++) {
			if (matrix3[i,t]) list3.Add(t,matrix1.GetLength(0)+1);
		}

	}
	
	
	
}

