﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.Mathematics.Optimization
{
    /// <summary>
    /// Interface for constraints.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// The constraint type. 
        /// </summary>
        ConstraintType Type { get; }

        /// <summary>
        /// The number of parameters to evaluate in the function.
        /// </summary>
        int NumberOfParameters { get; }

        /// <summary>
        /// Calculates the left hand side of the constraint.
        /// </summary>
        Func<double[], double> Function { get; }

        /// <summary>
        /// The value on the right hand side of the constraint equation. 
        /// </summary>
        double Value { get; }

        /// <summary>
        /// The violation tolerance for the constraint. Default = 1E-8. 
        /// </summary>
        double Tolerance { get; }

    }
}
