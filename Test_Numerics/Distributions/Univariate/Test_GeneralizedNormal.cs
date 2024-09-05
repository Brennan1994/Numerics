﻿/**
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from Numerics.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* **/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;
using Numerics.Mathematics;
using System.Diagnostics;

namespace Distributions.Univariate
{
    /// <summary>
    /// Testing the Generalized Normal distribution algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Tiki Gonzalez, USACE Risk Management Center, julian.t.gonzalez@usace.army.mil</item>
    ///     </list> 
    /// </para>
    /// <para>
    /// <b> References: </b>
    /// </para>
    /// <para>
    /// <see href = "https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics.Tests/DistributionTests" />
    /// </para>
    /// </remarks>
    [TestClass]
    public class Test_GeneralizedNormal
    {
        [TestMethod]
        public void Test_GNO_LMOM()
        {
            // Air quality - wind data from R
            var data = new double[] { 7.4, 8, 12.6, 11.5, 14.3, 14.9, 8.6, 13.8, 20.1, 8.6, 6.9, 9.7, 9.2, 10.9, 13.2, 11.5, 12, 18.4, 11.5, 9.7, 9.7, 16.6, 9.7, 12, 16.6, 14.9, 8, 12, 14.9, 5.7, 7.4, 8.6, 9.7, 16.1, 9.2, 8.6, 14.3, 9.7, 6.9, 13.8, 11.5, 10.9, 9.2, 8, 13.8, 11.5, 14.9, 20.7, 9.2, 11.5, 10.3, 6.3, 1.7, 4.6, 6.3, 8, 8, 10.3, 11.5, 14.9, 8, 4.1, 9.2, 9.2, 10.9, 4.6, 10.9, 5.1, 6.3, 5.7, 7.4, 8.6, 14.3, 14.9, 14.9, 14.3, 6.9, 10.3, 6.3, 5.1, 11.5, 6.9, 9.7, 11.5, 8.6, 8, 8.6, 12, 7.4, 7.4, 7.4, 9.2, 6.9, 13.8, 7.4, 6.9, 7.4, 4.6, 4, 10.3, 8, 8.6, 11.5, 11.5, 11.5, 9.7, 11.5, 10.3, 6.3, 7.4, 10.9, 10.3, 15.5, 14.3, 12.6, 9.7, 3.4, 8, 5.7, 9.7, 2.3, 6.3, 6.3, 6.9, 5.1, 2.8, 4.6, 7.4, 15.5, 10.9, 10.3, 10.9, 9.7, 14.9, 15.5, 6.3, 10.9, 11.5, 6.9, 13.8, 10.3, 10.3, 8, 12.6, 9.2, 10.3, 10.3, 16.6, 6.9, 13.2, 14.3, 8, 11.5 };
            var gno = new GeneralizedNormal();
            gno.Estimate(data, ParameterEstimationMethod.MethodOfLinearMoments);
            double xi = gno.Xi;
            double a = gno.Alpha;
            double k = gno.Kappa;
            double true_xi = 9.7285364;
            double true_a = 3.4885029;
            double true_k = -0.1307169;

            Assert.AreEqual(xi, true_xi, 0.0001d);
            Assert.AreEqual(a, true_a, 0.0001d);
            Assert.AreEqual(k, true_k, 0.0001d);

            var lmom = gno.LinearMomentsFromParameters(gno.GetParameters);
            Assert.AreEqual(lmom[0], 9.95751634, 0.0001d);
            Assert.AreEqual(lmom[1], 1.98224114, 0.0001d);
            Assert.AreEqual(lmom[2], 0.06380804, 0.0001d);
            Assert.AreEqual(lmom[3], 0.1258014, 0.0001d);

        }

        /// <summary>
        /// Verification using R lmom package. 
        /// </summary>
        [TestMethod]
        public void Test_GNO_CDF()
        {
            var x = new double[] { 5, 10, 12, 15, 18 };
            var p = new double[5];
            var true_p = new double[] { 0.07465069, 0.53400804, 0.73775928, 0.92073519, 0.98333335 };
            var gno = new GeneralizedNormal(9.7, 3.5, -0.1);
            for (int i = 0; i < 5; i++)
            {
                p[i] = gno.CDF(x[i]);
                Assert.AreEqual(true_p[i], p[i], 1E-7);
            }
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(x[i], gno.InverseCDF(p[i]), 1E-7);
            }

        }

        [TestMethod]
        public void Test_GNO_MLE()
        {
            // Air quality - wind data from R
            var data = new double[] { 7.4, 8, 12.6, 11.5, 14.3, 14.9, 8.6, 13.8, 20.1, 8.6, 6.9, 9.7, 9.2, 10.9, 13.2, 11.5, 12, 18.4, 11.5, 9.7, 9.7, 16.6, 9.7, 12, 16.6, 14.9, 8, 12, 14.9, 5.7, 7.4, 8.6, 9.7, 16.1, 9.2, 8.6, 14.3, 9.7, 6.9, 13.8, 11.5, 10.9, 9.2, 8, 13.8, 11.5, 14.9, 20.7, 9.2, 11.5, 10.3, 6.3, 1.7, 4.6, 6.3, 8, 8, 10.3, 11.5, 14.9, 8, 4.1, 9.2, 9.2, 10.9, 4.6, 10.9, 5.1, 6.3, 5.7, 7.4, 8.6, 14.3, 14.9, 14.9, 14.3, 6.9, 10.3, 6.3, 5.1, 11.5, 6.9, 9.7, 11.5, 8.6, 8, 8.6, 12, 7.4, 7.4, 7.4, 9.2, 6.9, 13.8, 7.4, 6.9, 7.4, 4.6, 4, 10.3, 8, 8.6, 11.5, 11.5, 11.5, 9.7, 11.5, 10.3, 6.3, 7.4, 10.9, 10.3, 15.5, 14.3, 12.6, 9.7, 3.4, 8, 5.7, 9.7, 2.3, 6.3, 6.3, 6.9, 5.1, 2.8, 4.6, 7.4, 15.5, 10.9, 10.3, 10.9, 9.7, 14.9, 15.5, 6.3, 10.9, 11.5, 6.9, 13.8, 10.3, 10.3, 8, 12.6, 9.2, 10.3, 10.3, 16.6, 6.9, 13.2, 14.3, 8, 11.5 };
            var gno = new GeneralizedNormal();
            gno.Estimate(data, ParameterEstimationMethod.MaximumLikelihood);
            double xi = gno.Xi;
            double a = gno.Alpha;
            double k = gno.Kappa;

            // L-Moment values:
            // double true_xi = 9.7285364;
            // double true_a = 3.4885029;
            // double true_k = -0.1307169;

        }


        [TestMethod]
        public void Test_GNO_Dist()
        {
            // Air quality - wind data from R
            var data = new double[] { 7.4, 8, 12.6, 11.5, 14.3, 14.9, 8.6, 13.8, 20.1, 8.6, 6.9, 9.7, 9.2, 10.9, 13.2, 11.5, 12, 18.4, 11.5, 9.7, 9.7, 16.6, 9.7, 12, 16.6, 14.9, 8, 12, 14.9, 5.7, 7.4, 8.6, 9.7, 16.1, 9.2, 8.6, 14.3, 9.7, 6.9, 13.8, 11.5, 10.9, 9.2, 8, 13.8, 11.5, 14.9, 20.7, 9.2, 11.5, 10.3, 6.3, 1.7, 4.6, 6.3, 8, 8, 10.3, 11.5, 14.9, 8, 4.1, 9.2, 9.2, 10.9, 4.6, 10.9, 5.1, 6.3, 5.7, 7.4, 8.6, 14.3, 14.9, 14.9, 14.3, 6.9, 10.3, 6.3, 5.1, 11.5, 6.9, 9.7, 11.5, 8.6, 8, 8.6, 12, 7.4, 7.4, 7.4, 9.2, 6.9, 13.8, 7.4, 6.9, 7.4, 4.6, 4, 10.3, 8, 8.6, 11.5, 11.5, 11.5, 9.7, 11.5, 10.3, 6.3, 7.4, 10.9, 10.3, 15.5, 14.3, 12.6, 9.7, 3.4, 8, 5.7, 9.7, 2.3, 6.3, 6.3, 6.9, 5.1, 2.8, 4.6, 7.4, 15.5, 10.9, 10.3, 10.9, 9.7, 14.9, 15.5, 6.3, 10.9, 11.5, 6.9, 13.8, 10.3, 10.3, 8, 12.6, 9.2, 10.3, 10.3, 16.6, 6.9, 13.2, 14.3, 8, 11.5 };
            var gno = new GeneralizedNormal();
            gno.Estimate(data, ParameterEstimationMethod.MethodOfLinearMoments);

            double pdf = gno.PDF(14.9);
            double cdf = gno.CDF(14.9);
            double invcdf = gno.InverseCDF(cdf);
            double true_pdf = 0.03825155;
            double true_cdf = 0.912294;
            double true_invcdf = 14.9;

            Assert.AreEqual(pdf, true_pdf, 0.0001d);
            Assert.AreEqual(cdf, true_cdf, 0.0001d);
            Assert.AreEqual(invcdf, true_invcdf, 0.0001d);

        }

        [TestMethod]
        public void Test_GNO_PartialDerivatives()
        {

            // Air quality - wind data from R
            var data = new double[] { 7.4, 8, 12.6, 11.5, 14.3, 14.9, 8.6, 13.8, 20.1, 8.6, 6.9, 9.7, 9.2, 10.9, 13.2, 11.5, 12, 18.4, 11.5, 9.7, 9.7, 16.6, 9.7, 12, 16.6, 14.9, 8, 12, 14.9, 5.7, 7.4, 8.6, 9.7, 16.1, 9.2, 8.6, 14.3, 9.7, 6.9, 13.8, 11.5, 10.9, 9.2, 8, 13.8, 11.5, 14.9, 20.7, 9.2, 11.5, 10.3, 6.3, 1.7, 4.6, 6.3, 8, 8, 10.3, 11.5, 14.9, 8, 4.1, 9.2, 9.2, 10.9, 4.6, 10.9, 5.1, 6.3, 5.7, 7.4, 8.6, 14.3, 14.9, 14.9, 14.3, 6.9, 10.3, 6.3, 5.1, 11.5, 6.9, 9.7, 11.5, 8.6, 8, 8.6, 12, 7.4, 7.4, 7.4, 9.2, 6.9, 13.8, 7.4, 6.9, 7.4, 4.6, 4, 10.3, 8, 8.6, 11.5, 11.5, 11.5, 9.7, 11.5, 10.3, 6.3, 7.4, 10.9, 10.3, 15.5, 14.3, 12.6, 9.7, 3.4, 8, 5.7, 9.7, 2.3, 6.3, 6.3, 6.9, 5.1, 2.8, 4.6, 7.4, 15.5, 10.9, 10.3, 10.9, 9.7, 14.9, 15.5, 6.3, 10.9, 11.5, 6.9, 13.8, 10.3, 10.3, 8, 12.6, 9.2, 10.3, 10.3, 16.6, 6.9, 13.2, 14.3, 8, 11.5 };
            var gno = new GeneralizedNormal();
            gno.Estimate(data, ParameterEstimationMethod.MethodOfLinearMoments);
            double p = 0.999;

            var pd1 = gno.PartialDerivatives(p);
            var pd2 = NumericalDerivative.Gradient(x =>
            {
                var gn = new GeneralizedNormal();
                gn.SetParameters(x);
                return gn.InverseCDF(p);
            }, gno.GetParameters);


            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var ps = new double[] { 0.1, 0.01, 0.001 };
            var J = gno.Jacobian(ps);


            // Write out results
            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10d);
            Debug.WriteLine("Runtime: " + elapsedTime);



        }

        /// <summary>
        /// Testing that parameters can create generalized normal distribution.
        /// </summary>
        [TestMethod()]
        public void CanCreateGeneralizedNormal()
        {
            var n = new GeneralizedNormal();
            Assert.AreEqual(n.Xi, 100);
            Assert.AreEqual(n.Alpha, 10);
            Assert.AreEqual(n.Kappa, 0);

            var n2 = new GeneralizedNormal(-100, 1, 1);
            Assert.AreEqual(n2.Xi, -100);
            Assert.AreEqual(n2.Alpha, 1);
            Assert.AreEqual(n2.Kappa, 1);
        }

        /// <summary>
        /// Testing Generalized normal distribution with bad parameters.
        /// </summary>
        [TestMethod()]
        public void GeneralizedNormalFails()
        {
            var n = new GeneralizedNormal(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Assert.IsFalse(n.ParametersValid);

            var n2 = new GeneralizedNormal(double.NaN, double.NaN, double.NaN);
            Assert.IsFalse(n2.ParametersValid);

            var n3 = new GeneralizedNormal(100, 0, 1);
            Assert.IsFalse(n3.ParametersValid);
        }

        /// <summary>
        /// Testing ParametersToString() function.
        /// </summary>
        [TestMethod()]
        public void ValidateParametersToString()
        {
            var n = new GeneralizedNormal();
            Assert.AreEqual(n.ParametersToString[0, 0], "Location (ξ)");
            Assert.AreEqual(n.ParametersToString[1, 0], "Scale (α)");
            Assert.AreEqual(n.ParametersToString[2, 0], "Shape (κ)");
            Assert.AreEqual(n.ParametersToString[0, 1], "100");
            Assert.AreEqual(n.ParametersToString[1, 1], "10");
            Assert.AreEqual(n.ParametersToString[2, 1], "0");
        }

        /// <summary>
        /// Testing mean function. 
        /// </summary>
        [TestMethod()]
        public void ValidateMean()
        {
            var n = new GeneralizedNormal();
            Assert.AreEqual(n.Mean, 100,1e-04);

            var n2 = new GeneralizedNormal(1, 5,0.42);
            var mean = n2.Mean;
            Assert.AreEqual(n2.Mean, 1, 1e-04);
        }

        /// <summary>
        /// Testing median function.
        /// </summary>
        [TestMethod()]
        public void ValidateMedian()
        {
            var n = new GeneralizedNormal();
            Assert.AreEqual(n.Median, 100);

            var n2 = new GeneralizedNormal(1, 5, 0.42);
            Assert.AreEqual(n2.Median, 1);
        }

        /// <summary>
        /// Testing mode is NaN.
        /// </summary>
        [TestMethod()]
        public void ValidateMode()
        {
            var n = new GeneralizedNormal();
            Assert.AreEqual(n.Mode, double.NaN);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //[TestMethod()]
        //public void ValidateStandardDeviation()
        //{
        //    var n = new GeneralizedNormal();
        //    //Assert.AreEqual(n.StandardDeviation, );

        //    var n2 = new GeneralizedNormal(1, 5, 0.42);
        //    Assert.AreEqual(n2.StandardDeviation, 138.566885, 1e-04);
        //}

    }
}
