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

using System;
using System.Linq;

namespace Numerics.Mathematics.Optimization
{
    /// <summary>
    /// A class for storing an optimization trial parameter set.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public struct ParameterSet
    {
        /// <summary>
        /// Constructs an empty parameter set.
        /// </summary>
        public ParameterSet() { }

        /// <summary>
        /// Constructs a parameter set.
        /// </summary>
        /// <param name="values">The parameter values.</param>
        /// <param name="fitness">The objective function result (or fitness) given the parameter set.</param>
        public ParameterSet(double[] values, double fitness)
        {
            Values = values.ToArray();
            Fitness = fitness;
        }

        /// <summary>
        /// Constructs a parameter set.
        /// </summary>
        /// <param name="values">The parameter values.</param>
        /// <param name="fitness">The objective function result (or fitness) given the parameter set.</param>
        /// <param name="weight">The weight given to the parameter set.</param>
        public ParameterSet(double[] values, double fitness, double weight)
        {
            Values = (double[])values.Clone();
            Fitness = fitness;
            Weight = weight;
        }

        /// <summary>
        /// The trial parameter set values.
        /// </summary>
        public double[] Values;

        /// <summary>
        /// The objective function result (or fitness) given the trial parameter set. 
        /// </summary>
        public double Fitness;

        /// <summary>
        /// An optional weight given to the parameter set values.
        /// </summary>
        public double Weight;

        /// <summary>
        /// Returns a clone of the point.
        /// </summary>
        public ParameterSet Clone()
        {
            return new ParameterSet(Values, Fitness, Weight);
        }
    }
}
