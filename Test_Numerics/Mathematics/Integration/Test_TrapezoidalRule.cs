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

using Mathematics.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;
using Numerics.Mathematics.Integration;
using System;

namespace Mathematics.Integration
{
    /// <summary>
    /// Unit test for the Trapezoidal rule of integration
    /// </summary>
    [TestClass]
    public class Test_TrapezoidalRule
    {
        /// <summary>
        /// Test Trapezoidal rule with a one dimensional function
        /// </summary>
        [TestMethod]
        public void Test_FX3()
        {
            var tr = new TrapezoidalRule(Integrands.FX3, 0d, 1d);
            tr.Integrate();
            double result = tr.Result;
            double trueResult = 0.25d;
            Assert.AreEqual(result, trueResult, 1E-3);
        }

        /// <summary>
        /// Test Trapezoidal rule with the cos(x) function
        /// </summary>
        [TestMethod]
        public void Test_Cosine()
        {
            var tr = new TrapezoidalRule(Integrands.Cosine, -1, 1);
            tr.Integrate();
            double result = tr.Result;
            double trueResult = 1.6829419d;
            Assert.AreEqual(result, trueResult, 1E-3);
        }

        /// <summary>
        /// Test Trapezoidal rule with the sin(x) function
        /// </summary>
        [TestMethod]
        public void Test_Sine()
        {
            var tr = new TrapezoidalRule(Integrands.Sine, 0, 1);
            tr.Integrate();
            double result = tr.Result;
            double trueResult = 0.459697694131d;
            Assert.AreEqual(result, trueResult, 1E-3);
        }

        /// <summary>
        /// Test Trapezoidal rule with a 2nd order polynomial
        /// </summary>
        [TestMethod]
        public void Test_FXX()
        {
            var tr = new TrapezoidalRule(Integrands.FXX, 0, 2);
            tr.Integrate();
            double result = tr.Result;
            double trueResult = 57;
            Assert.AreEqual(result, trueResult, 1E-3);
        }

        /// <summary>
        /// Test Trapezoidal rule with a 3rd order polynomial
        /// </summary>
        [TestMethod]
        public void Test_FXXX()
        {
            var tr = new TrapezoidalRule(Integrands.FXXX, 0, 2);
            tr.Integrate();
            double result = tr.Result;
            double trueResult = 89;
            Assert.AreEqual(result, trueResult, 1E-3);
        }

        /// <summary>
        /// Test Trapezoidal rule with the Gamma function
        /// </summary>
        [TestMethod]
        public void Test_Gamma()
        {
            var gamma = new GammaDistribution(10, 5);
            var tr = new TrapezoidalRule(x => x * gamma.PDF(x), gamma.InverseCDF(1E-16), gamma.InverseCDF(1 - 1E-16));
            tr.Integrate();
            double result = tr.Result;
            double trueResult = 50;
            Assert.AreEqual(result, trueResult, 1E-3);
        }

        /// <summary>
        /// Test Trapezoidal rule with the CVaR function
        /// </summary>
        [TestMethod]
        public void Test_CVaR()
        {
            var ln = new LnNormal(10, 2);
            double alpha = 0.99;
            var tr = new TrapezoidalRule(x => x * ln.PDF(x), ln.InverseCDF(alpha), ln.InverseCDF(1 - 1E-16));
            tr.Integrate();
            double result = tr.Result / (1 - alpha);
            double trueResult = Math.Exp(ln.Mu + 0.5 * ln.Sigma * ln.Sigma) / (1d - alpha) * (1 - Normal.StandardCDF(Normal.StandardZ(alpha) - ln.Sigma));
            Assert.AreEqual(result, trueResult, 1E-3);
        }
    }
}
