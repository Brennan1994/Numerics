﻿using System;
using System.Collections.Generic;

namespace Numerics.Data
{
    /// <summary>
    /// A class for cubic spline interpolation.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// References:
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// "Numerical Recipes: The art of Scientific Computing, Third Edition. Press et al. 2017.
    /// </remarks>
    public class CubicSpline : Interpolater
    {

        /// <summary>
        /// Construct new linear interpolation. 
        /// </summary>
        /// <param name="xValues">List of x-values.</param>
        /// <param name="yValues">List of y-values.</param>
        /// <param name="sortOrder">The sort order of the x-values, either ascending or descending. Default = Ascending. </param>
        public CubicSpline(IList<double> xValues, IList<double> yValues, SortOrder sortOrder = SortOrder.Ascending) : base(xValues, yValues, sortOrder)
        {
            SetSecondDerivatives();
        }

        /// <summary>
        /// Stores the array of second derivatives.
        /// </summary>
        private double[] y2;

        /// <summary>
        /// Auxiliary routine to set the second derivatives. If you make changes to the x- or y-values, then you need to call this routine afterwards.
        /// </summary>
        public void SetSecondDerivatives()
        {
            // This routine stores an array y2[0...n-1] with second derivatives of the interpolating unction
            // at the tabulated points. if yp1 and or yp2 are equal to 1E99 or larger, the routine is signaled to set
            // the corresponding boundary condition for a natural spline, with zero second derivative on that boundary.

            double yp1 = 1E99, ypn = 1E99;
            int n = XValues.Count;
            y2 = new double[n];
            double p, qn, sig, un;
            var u = new double[n - 1];
            if (yp1 > 0.99e99)
                y2[0] = u[0] = 0.0;
            else
            {
                y2[0] = -0.5;
                u[0] = (3.0 / (XValues[1] - XValues[0])) * ((YValues[1] - YValues[0]) / (XValues[1] - XValues[0]) - yp1);
            }
            for (int i = 1; i < n - 1; i++)
            {
                sig = (XValues[i] - XValues[i - 1]) / (XValues[i + 1] - XValues[i - 1]);
                p = sig * y2[i - 1] + 2.0;
                y2[i] = (sig - 1.0) / p;
                u[i] = (YValues[i + 1] - YValues[i]) / (XValues[i + 1] - XValues[i]) - (YValues[i] - YValues[i - 1]) / (XValues[i] - XValues[i - 1]);
                u[i] = (6.0 * u[i] / (XValues[i + 1] - XValues[i - 1]) - sig * u[i - 1]) / p;
            }
            if (ypn > 0.99e99)
                qn = un = 0.0;
            else
            {
                qn = 0.5;
                un = (3.0 / (XValues[n - 1] - XValues[n - 2])) * (ypn - (YValues[n - 1] - YValues[n - 2]) / (XValues[n - 1] - XValues[n - 2]));
            }
            y2[n - 1] = (un - qn * u[n - 2]) / (qn * y2[n - 2] + 1.0);
            for (int k = n - 2; k >= 0; k--)
                y2[k] = y2[k] * y2[k + 1] + u[k];
        }

        /// <summary>
        /// Given a value x, returns an interpolated value.
        /// </summary>
        /// <param name="x">The value to interpolate.</param>
        /// <param name="start">The zero-based index to start the search from.</param>
        public override double RawInterpolate(double x, int start)
        {
            if (start < 0 || start >= Count) start = 0;
            int klo = start, khi = start + 1;
            double y, h, b, a;
            h = XValues[khi] - XValues[klo];
            a = (XValues[khi] - x) / h;
            b = (x - XValues[klo]) / h;
            y = a * YValues[klo] + b * YValues[khi] + ((a * a * a - a) * y2[klo]
                + (b * b * b - b) * y2[khi]) * (h * h) / 6.0;
            return y;
        }

    }
}
