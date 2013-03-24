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

namespace LiquidHRF
{
    /// <summary>
    /// Description of DataSet.
    /// </summary>
    public class DataSet
    {
        double[][] m_dataValues;

        public DataSet() { }

        public bool Load(string dataPath)
        {
            if (!InitArraySize(dataPath))
                return false;

            int rowIdx = 0;
            string line;
            try
            {
                using (StreamReader sr = new StreamReader(dataPath))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }

                        string[] cols = line.Split(line.Contains("\t") ? '\t' : ' ');
                        //Console.WriteLine("" + rowIdx + ":" + cols.Length + " " + line);
                        int colIdx = 0;
                        foreach (string col in cols)
                        {
                            //Console.WriteLine("Converting " + col);
                            if (col.Length == 0)
                                continue;
                            m_dataValues[colIdx++][rowIdx] = Convert.ToDouble(col);
                        }
                        rowIdx++;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Error: The data file could not be read.");
                Console.WriteLine(e.Message);
                return false;
            }

        }

        public double GetValue(int col, int row)
        {
            return m_dataValues[col][row];
        }

        public double[] GetColumn(int col)
        {
            return m_dataValues[col];
        }

        public int GetCols()
        {
            return m_dataValues.Length;
        }

        public int GetRows()
        {
            return m_dataValues[0].Length;
        }

        private bool InitArraySize(string dataPath)
        {
            int rowCount = 0;
            int columnCount = 0;
            try
            {
                using (StreamReader sr = new StreamReader(dataPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }

                        if (columnCount == 0)
                        {
                            columnCount = GetDataCount(line);
                        }

                        rowCount++;
                    }
                }

                if (columnCount == 0 || rowCount == 0)
                {
                    Console.WriteLine("Error: The data file " + dataPath + " is empty.");
                    return false;
                }

                m_dataValues = new double[columnCount][];
                for (int i = 0; i < columnCount; i++)
                    m_dataValues[i] = new double[rowCount];
                return true;
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Error: The data file could not be read.");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private int GetDataCount(string line)
        {
            string[] words = line.Split( line.Contains("\t") ? '\t' : ' ');
            return words.Length;
        }
    }
}
