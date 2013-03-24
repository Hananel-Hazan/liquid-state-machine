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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiquidHRF
{
    class Comparator
    {

        public void CalculateMetrics(string f1, string f2) {
            
            DataSet d1 = new DataSet();
            d1.Load(f1);

            DataSet d2 = new DataSet();
            d2.Load(f2);

            GetCorrelations(d1, d2);
            GetRMSDs(d1, d2);
        }

        public void GetCorrelations(DataSet d1, DataSet d2)
        {
            int col = d1.GetCols();
            int row = d1.GetRows();

            Console.WriteLine("Correlations:");
            double max = 0;
            double min = 10; 
            double avg = 0;

            for (int i = 0; i < col; i++)
            {
                double corr = Math.Abs(GetCorrelation(ZScoreNormalize(d1.GetColumn(i)), ZScoreNormalize(d2.GetColumn(i))));
                if (corr > max)
                    max = corr;
                if (corr < min)
                    min = corr;
                avg += corr;
                Console.WriteLine("" + i + " " + corr);
            }

            Console.WriteLine("MAX: " + max + " MIN: " + min + " AVG: " + avg/col);
        }

        public void GetRMSDs(DataSet d1, DataSet d2)
        {
            int col = d1.GetCols();
            int row = d1.GetRows();

            Console.WriteLine("RMSD:");
            for (int i = 0; i < col; i++)
            {
                Console.WriteLine("" + i + " " + GetRMSD(ZScoreNormalize(d1.GetColumn(i)), ZScoreNormalize(d2.GetColumn(i))));
            }
        }

        public double GetCorrelation(double[] x, double[] y)
        {
            double mu_x = 0;
            double mu_y = 0;
            double mu_dev_x_y = 0;
            double mu_dev_x = 0;
            double mu_dev_y = 0;

            int startIdx = 0;
            int count = x.Length;

            for (int i = startIdx; i < startIdx + count; i++)
            {
                mu_x += x[i];
                mu_y += y[i];
            }

            mu_x = mu_x / count;
            mu_y = mu_y / count;

            for (int i = startIdx; i < startIdx + count; i++)
            {
                mu_dev_x += (x[i] - mu_x) * (x[i] - mu_x);
                mu_dev_y += (y[i] - mu_y) * (y[i] - mu_y);
                mu_dev_x_y += (x[i] - mu_x) * (y[i] - mu_y);
            }

            mu_dev_x = Math.Sqrt(mu_dev_x / (count - 1));
            mu_dev_y = Math.Sqrt(mu_dev_y / (count - 1));
            mu_dev_x_y = mu_dev_x_y / (count - 1);

            return mu_dev_x_y / (mu_dev_x * mu_dev_y);
        }

        public double GetRMSD(double[] x, double[] y)
        {
            double[] x_norm = x; // Normalize(x);
            double[] y_norm = y; // Normalize(y);
            double diff_xy_sqrt = 0;
            double diff_xy = 0;
            for (int i = 0; i < x.Length; i++)
            {
                diff_xy = Math.Abs(x_norm[i] - y_norm[i]);
                diff_xy_sqrt += diff_xy * diff_xy;
            }

            return Math.Sqrt(diff_xy_sqrt / x.Length);
        }

        public static double[] ZScoreNormalize(double[] x)
        {
            double mean = 0;
            for (int i = 0; i < x.Length; i++)
            {
                mean += x[i];
            }

            mean = mean / x.Length;

            double sd_x = 0;
            double local_var = 0;
            for (int i = 0; i < x.Length; i++)
            {
                local_var = Math.Abs(x[i] - mean);
                sd_x += local_var * local_var;
            }

            sd_x = Math.Sqrt(sd_x / x.Length);

            double[] x_norm = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                x_norm[i] = (x[i] - mean) / sd_x;
            }

            return x_norm;
        }
    }
}
