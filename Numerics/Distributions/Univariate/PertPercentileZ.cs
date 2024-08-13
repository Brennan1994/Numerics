﻿/*
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
*/

using System;
using System.Collections.Generic;
using Numerics.Mathematics.Optimization;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Pert percentile z distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    ///     In probability and statistics, the PERT distribution is a family of continuous probability distributions
    ///     defined by the minimum (a), most likely (b) and maximum (c) values that a variable can take.
    ///     It is a transformation of the four-parameter Beta distribution.
    /// </para>
    /// <para>
    ///     This version of the PERT is parameterized using the 5th, 50th, and 95th percentiles in Normal Z space.
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/PERT_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class PertPercentileZ : UnivariateDistributionBase
    {

        /// <summary>
        /// Constructs a PERT distribution with 5th = 0.05 (z = -1.64), 50th = 0.5 (z = 0.0), and 95th = 0.95 (z = 1.64).
        /// </summary>
        public PertPercentileZ()
        {
            SetParameters(0.05, 0.5d, 0.95d);
        }

        /// <summary>
        /// Constructs a PERT distribution with specified 5th, 50th, and 95th percentiles.
        /// </summary>
        /// <param name="fifth">The 5th percentile value of the distribution, in Z-space.</param>
        /// <param name="fiftieth">The 50th percentile value of the distribution, in Z-space..</param>
        /// <param name="ninetyFifth">The 95th percentile value of the distribution, in Z-space..</param>
        public PertPercentileZ(double fifth, double fiftieth, double ninetyFifth)
        {
            SetParameters(fifth, fiftieth, ninetyFifth);
        }

        private bool _parametersValid = true;
        private bool _parametersSolved = false;
        private GeneralizedBeta _beta = new GeneralizedBeta();
        private double _5th;
        private double _50th;
        private double _95th;

        /// <summary>
        /// Gets and sets the 5th percentile.
        /// </summary>
        public double Percentile5th
        {
            get { return _5th; }
            set
            {
                if (_5th != value)
                {
                    _5th = value;
                    // validate parameters
                    _parametersValid = ValidateParameters(_5th, _50th, _95th, false) is null;
                    _parametersSolved = false;
                }
            }
        }

        /// <summary>
        /// Gets and sets the 50th percentile.
        /// </summary>
        public double Percentile50th
        {
            get { return _50th; }
            set
            {
                if (_50th != value)
                {
                    _50th = value;
                    // validate parameters
                    _parametersValid = ValidateParameters(_5th, _50th, _95th, false) is null;
                    _parametersSolved = false;
                }
            }
        }

        /// <summary>
        /// Gets and sets the 95th percentile.
        /// </summary>
        public double Percentile95th
        {
            get { return _95th; }
            set
            {
                if (_95th != value)
                {
                    _95th = value;
                    // validate parameters
                    _parametersValid = ValidateParameters(_5th, _50th, _95th, false) is null;
                    _parametersSolved = false;
                }
            }
        }

        /// <inheritdoc/>
        public override int NumberOfParameters
        {
            get { return 3; }
        }

        /// <inheritdoc/>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.PertPercentileZ; }
        }

        /// <inheritdoc/>
        public override string DisplayName
        {
            get { return "PERT-Percentile Z"; }
        }

        /// <inheritdoc/>
        public override string ShortDisplayName
        {
            get { return "PERT-% Z"; }
        }

        /// <inheritdoc/>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[3, 2];
                parmString[0, 0] = "5%";
                parmString[1, 0] = "50%";
                parmString[2, 0] = "95%";
                parmString[0, 1] = Percentile5th.ToString();
                parmString[1, 1] = Percentile50th.ToString();
                parmString[2, 1] = Percentile95th.ToString();
                return parmString;
            }
        }

        /// <inheritdoc/>
        public override string[] ParameterNamesShortForm
        {
            get { return ["5%", "50%", "95%"]; }
        }

        /// <inheritdoc/>
        public override string[] GetParameterPropertyNames
        {
            get { return [nameof(Percentile5th), nameof(Percentile50th), nameof(Percentile95th)]; }
        }

        /// <inheritdoc/>
        public override double[] GetParameters
        {
            get { return [Percentile5th, Percentile50th, Percentile95th]; }
        }

        /// <inheritdoc/>
        public override bool ParametersValid
        {
            get { return _parametersValid; }
        }

        /// <inheritdoc/>
        public override double Mean
        {
            get 
            {
                if (_beta.Min == _beta.Max) return Normal.StandardCDF(_beta.Min);
                return Normal.StandardCDF(_beta.Mean); 
            }
        }

        /// <inheritdoc/>
        public override double Median
        {
            get 
            {
                if (_beta.Min == _beta.Max) return Normal.StandardCDF(_beta.Min);
                return Normal.StandardCDF(_beta.Median); 
            }
        }

        /// <inheritdoc/>
        public override double Mode
        {
            get 
            {
                if (_beta.Min == _beta.Max) return Normal.StandardCDF(_beta.Min);
                return Normal.StandardCDF(_beta.Mode); 
            }
        }

        /// <inheritdoc/>
        public override double StandardDeviation
        {
            get { return _beta.StandardDeviation; }
        }

        /// <inheritdoc/>
        public override double Skewness
        {
            get { return _beta.Skewness; }
        }

        /// <inheritdoc/>
        public override double Kurtosis
        {
            get { return _beta.Kurtosis; }
        }

        /// <inheritdoc/>
        public override double Minimum
        {
            get { return Normal.StandardCDF(_beta.Minimum); }
        }

        /// <inheritdoc/>
        public override double Maximum
        {
            get { return Normal.StandardCDF(_beta.Maximum); }
        }

        /// <inheritdoc/>
        public override double[] MinimumOfParameters
        {
            get { return [0d, Percentile5th, Percentile50th]; }
        }

        /// <inheritdoc/>
        public override double[] MaximumOfParameters
        {
            get { return [Percentile50th, Percentile95th, 1d]; }
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="fifth">The 5th percentile value of the distribution.</param>
        /// <param name="fiftieth">The 50th percentile value of the distribution.</param>
        /// <param name="ninetyFifth">The 95th percentile value of the distribution.</param>
        public void SetParameters(double fifth, double fiftieth, double ninetyFifth)
        {
            Percentile5th = fifth;
            Percentile50th = fiftieth;
            Percentile95th = ninetyFifth;
        }

        /// <inheritdoc/>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0], parameters[1], parameters[2]);
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="fifth">The 5th percentile value of the distribution.</param>
        /// <param name="fiftieth">The 50th percentile value of the distribution.</param>
        /// <param name="ninetyFifth">The 95th percentile value of the distribution.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        private ArgumentOutOfRangeException ValidateParameters(double fifth, double fiftieth, double ninetyFifth, bool throwException)
        {
            if (double.IsNaN(fifth) || double.IsInfinity(fifth) ||
                double.IsNaN(ninetyFifth) || double.IsInfinity(ninetyFifth) || fifth > ninetyFifth)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Percentile5th), "The 5% cannot be greater than the 95%.");
                return new ArgumentOutOfRangeException(nameof(Percentile5th), "The 5% cannot be greater than the 95%.");
            }
            else if (double.IsNaN(fiftieth) || double.IsInfinity(fiftieth) || fiftieth < fifth || fiftieth > ninetyFifth)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Percentile50th), "The 50% must be between the 5% and 95%.");
                return new ArgumentOutOfRangeException(nameof(Percentile50th), "The 50% must be between the 5% and 95%.");
            }
            else if (fifth < 0 || fifth > 1)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Percentile5th), "The percentiles must be between 0 and 1.");
                return new ArgumentOutOfRangeException(nameof(Percentile5th), "The percentiles must be between 0 and 1.");
            }
            else if (fiftieth < 0 || fiftieth > 1)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Percentile50th), "The percentiles must be between 0 and 1.");
                return new ArgumentOutOfRangeException(nameof(Percentile50th), "The percentiles must be between 0 and 1.");
            }
            else if (ninetyFifth < 0 || ninetyFifth > 1)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Percentile95th), "The percentiles must be between 0 and 1.");
                return new ArgumentOutOfRangeException(nameof(Percentile95th), "The percentiles must be between 0 and 1.");
            }
            return null;
        }

        /// <inheritdoc/>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            return ValidateParameters(parameters[0], parameters[1], parameters[2], throwException);
        }

        
        /// <summary>
        /// Solve the PERT parameters (min, mode, max) given the percentiles.
        /// </summary>
        public void SolveParameters()
        {
            if (_parametersValid == false) return;
            if (_parametersSolved == true) return;

            double fifth =  Percentile5th;
            double fiftieth = Percentile50th;
            double ninetyFifth = Percentile95th;

            //_pert = new Pert(fifth, fiftieth, ninetyFifth);
            if (fifth == fiftieth && fiftieth == ninetyFifth)
            {
                _beta = GeneralizedBeta.PERT(Normal.StandardZ(fifth), Normal.StandardZ(fiftieth), Normal.StandardZ(ninetyFifth));
                _parametersSolved = true;
                return;
            }
          
            fifth = Normal.StandardZ(Percentile5th);
            fiftieth = Normal.StandardZ(Percentile50th);
            ninetyFifth = Normal.StandardZ(Percentile95th);

            _beta = GeneralizedBeta.PERT(fifth, fiftieth, ninetyFifth);
            double min = Normal.StandardZ(1E-16);
            double max = Normal.StandardZ(1 - 1E-16);
            var Initials = new double[] { _beta.Alpha, _beta.Beta, _beta.Min, _beta.Max };
            var Lowers = new double[] { Tools.DoubleMachineEpsilon, Tools.DoubleMachineEpsilon, min, min };
            var Uppers = new double[] { _beta.Alpha * 100, _beta.Beta * 100, max, max };

            // Solve using Nelder-Mead (Downhill Simplex)
            double sse(double[] x)
            {
                var dist = new GeneralizedBeta();
                try
                {
                    dist = new GeneralizedBeta(x[0], x[1], x[2], x[3]);
                }
                catch
                {
                    return double.MaxValue;
                }
                if (dist.ParametersValid == false) return double.MaxValue;

                double SSE = 0d;
                SSE += Math.Pow(fifth - dist.InverseCDF(0.05), 2d);
                SSE += Math.Pow(fiftieth - dist.InverseCDF(0.5), 2d);
                SSE += Math.Pow(ninetyFifth - dist.InverseCDF(0.95), 2d);
                return SSE;
            }
            var solver = new NelderMead(sse, 4, Initials, Lowers, Uppers);
            solver.RelativeTolerance = 1E-8;
            solver.AbsoluteTolerance = 1E-8;
            solver.ReportFailure = false;
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            _beta = new GeneralizedBeta(solution[0], solution[1], solution[2], solution[3]);
            _parametersSolved = true;
        }

        /// <inheritdoc/>
        public override double PDF(double x)
        {
            // Validate parameters
            if (x < 0.0d || x > 1.0d) throw new ArgumentOutOfRangeException(nameof(x), "X must be between 0 and 1.");
            if (_parametersValid == false) ValidateParameters(Percentile5th, Percentile50th, Percentile95th, true);
            if (_parametersSolved == false) SolveParameters();
            // These checks are done specifically for an application where a 
            // user inputs min = max = mode
            if (_beta.Min == _beta.Max) return 0.0d;
            if (double.IsNaN(_beta.Mode)) return 0.0d;
            //
            return _beta.PDF(Normal.StandardZ(x));
        }

        /// <inheritdoc/>
        public override double CDF(double x)
        {
            // Validate parameters
            if (x < 0.0d || x > 1.0d) throw new ArgumentOutOfRangeException(nameof(x), "X must be between 0 and 1.");
            if (_parametersValid == false) ValidateParameters(Percentile5th, Percentile50th, Percentile95th, true);
            if (_parametersSolved == false) SolveParameters();
            // These checks are done specifically for an application where a 
            // user inputs min = max = mode
            if (_beta.Min == _beta.Max) return 1d;
            if (double.IsNaN(_beta.Mode)) return 1d;
            //
            return _beta.CDF(Normal.StandardZ(x));
        }

        /// <inheritdoc/>
        public override double InverseCDF(double probability)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameters(Percentile5th, Percentile50th, Percentile95th, true);
            if (_parametersSolved == false) SolveParameters();
            var x = Normal.StandardCDF(_beta.Min);
            // These checks are done specifically for an application where a 
            // user inputs min = max = mode
            if (_beta.Min == _beta.Max) return x;
            if (double.IsNaN(_beta.Mode)) return x;
            //
            try
            {
                x = Normal.StandardCDF(_beta.InverseCDF(probability));
            }
            catch (ArithmeticException)
            {
                return x;
            }
            return x;
        }

        /// <inheritdoc/>
        public override UnivariateDistributionBase Clone()
        {
            return new PertPercentileZ(Percentile5th, Percentile50th, Percentile95th) { _parametersSolved = _parametersSolved, _parametersValid = _parametersValid, _beta = (GeneralizedBeta)_beta.Clone()};
        }

        /// <summary>
        /// Return the Pert-PercentileZ as a Pert distribution.
        /// </summary>
        public Pert ToPert()
        {
            return new Pert(_beta.Min, _beta.Mode, _beta.Max);
        }

    }
}