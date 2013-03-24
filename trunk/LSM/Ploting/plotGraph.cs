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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;


/// <summary>
/// Description of plotGraph.
/// </summary>
public partial class plotGraph : Form
{
	public PointPairList[] list1;
	
	public plotGraph()
	{
		//
		// The InitializeComponent() call is required for Windows Forms designer support.
		//
		InitializeComponent();
		
		//
		// TODO: Add constructor code after the InitializeComponent() call.
		//
	}
	
//	void DisplayWaveGraph(ZedGraphControl graphControl, double[] waveData)
//	{
//		var pane = graphControl.GraphPane;
//		pane.Title.IsVisible = false;
//
//		var xAxis = pane.XAxis;
//		xAxis.Title.IsVisible = false;
//		xAxis.Scale.Min = 0;
//		xAxis.Scale.Max = waveData.Length - 1;
//
//		var yAxis = pane.YAxis;
//		yAxis.Title.IsVisible = false;
//		yAxis.Scale.Min = -1;
//		yAxis.Scale.Max = 1;
//
//		var timeData = Enumerable.Range(0, waveData.Length)
//			.Select(i => (double) i)
//			.ToArray();
//		pane.AddCurve(null, timeData, waveData, Color.Red, SymbolType.None);
//		graphControl.AxisChange();
//	}

	
	public void plotGraph_Load( object sender, EventArgs e )
	{
		// get a reference to the GraphPane
		GraphPane myPane = zgc.GraphPane;

		// Set the Titles
		myPane.Title.Text = "Neuron Activity";
		myPane.XAxis.Title.Text = "Time";
		myPane.YAxis.Title.Text = "Activity";
		
		// Generate a red curve with diamond
		// symbols, and "Porsche" in the legend
		ColorSymbolRotator colorPatter = new ColorSymbolRotator();
		LineItem[] myCurve = new LineItem[list1.Length];
		for (int i = 0 ; i<list1.Length ; i++){
			myCurve[i] = myPane.AddCurve( "#"+i.ToString(), list1[i], colorPatter.NextColor, SymbolType.Diamond );
			
			// Don't display the line (This makes a scatter plot)
			myCurve[i].Line.IsVisible = true;
			// Hide the symbol outline
			myCurve[i].Symbol.Border.IsVisible = true;
			// Fill the symbol interior with color
			//myCurve[i].Symbol.Fill = new Fill( Color.Firebrick );
			myCurve[i].Symbol.Size = 1;
		}
		
		// Tell ZedGraph to refigure the
		// axes since the data have changed
		zgc.AxisChange();
		// Make sure the Graph gets redrawn
		zgc.Invalidate();

	}
	
	private void plotGraph_Resize( object sender, EventArgs e )
	{
		SetSize();
	}

	private void SetSize()
	{
		zgc.Location = new Point( 10, 10 );
		// Leave a small margin around the outside of the control
		zgc.Size = new Size( this.ClientRectangle.Width - 20,this.ClientRectangle.Height - 20 );
	}
	
	public void loadData(ref double[,] matrix){
		
		list1 = new PointPairList[matrix.GetLength(0)];

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix.GetLength(0) ; i++ ){
			list1[i] = new PointPairList();
			for (int t = 0 ; t < matrix.GetLength(1)  ; t++) {
				list1[i].Add(t,matrix[i,t]+i);
			}
		}

		
	}
	
	public void loadData(ref double[][] matrix){
		
		list1 = new PointPairList[matrix.GetLength(0)];

		// Make up some data arrays based on the Sine function
		
		for ( int i = 0; i < matrix.GetLength(0) ; i++ ){
			list1[i] = new PointPairList();
			for (int t = 0 ; t < matrix[i].Length  ; t++) {
				list1[i].Add(t,matrix[i][t]+i);
			}
		}

		
	}
}

