﻿using Numerics.Distributions;
using System;
using System.Collections.Generic;

namespace Numerics.Data
{
    /// <summary>
    /// A class for linear interpolation.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    ///     Methods are designed to be compatible with the VBA Macro 'Interpolate Version 2.0.0' November 2017, USACE Risk Management Center
    /// </para>
    /// </remarks>
    public class Linear : Interpolater
    {
        /// <summary>
        /// Construct new linear interpolation. 
        /// </summary>
        /// <param name="xValues">List of x-values.</param>
        /// <param name="yValues">List of y-values.</param>
        /// <param name="sortOrder">The sort order of the x-values, either ascending or descending. Default = Ascending. </param>
        public Linear(IList<double> xValues, IList<double> yValues, SortOrder sortOrder = SortOrder.Ascending) : base(xValues, yValues, sortOrder){}

        /// <summary>
        /// The transform for the x-values. Default = None.
        /// </summary>
        public Transform XTransform { get; set; } = Transform.None;

        /// <summary>
        /// The transform for the y-values. Default = None.
        /// </summary>
        public Transform YTransform { get; set; } = Transform.None;

        /// <summary>
        /// Given a value x, returns an interpolated value.
        /// </summary>
        /// <param name="x">The value to interpolate.</param>
        /// <param name="start">The zero-based index to start the search from.</param>
        public override double RawInterpolate(double x, int start)
        {
            // See if x is out of range
            if ((SortOrder == SortOrder.Ascending && x <= XValues[0]) || 
                (SortOrder == SortOrder.Descending && x >= XValues[0])) return YValues[0];
            if ((SortOrder == SortOrder.Ascending && x >= XValues[Count - 1]) || 
                (SortOrder == SortOrder.Descending && x <= XValues[Count - 1])) return YValues[Count - 1];

            double y, x1 = 0, x2 = 0, y1 = 0, y2 = 0;
            int xlo = start, xhi = start + 1;

            // Get X transform
            if (XTransform == Transform.None)
            {
                x1 = XValues[xlo];
                x2 = XValues[xhi];
            }
            else if (XTransform == Transform.Logarithmic)
            {
                x = Tools.Log10(x);
                x1 = Tools.Log10(XValues[xlo]);
                x2 = Tools.Log10(XValues[xhi]);
            }
            else if (XTransform == Transform.NormalZ)
            {
                x = Normal.StandardZ(x);
                x1 = Normal.StandardZ(XValues[xlo]);
                x2 = Normal.StandardZ(XValues[xhi]);
            }

            // Get Y transform
            if (YTransform == Transform.None)
            {
                y1 = YValues[xlo];
                y2 = YValues[xhi];
            }
            else if (YTransform == Transform.Logarithmic)
            {
                y1 = Tools.Log10(YValues[xlo]);
                y2 = Tools.Log10(YValues[xhi]);
            }
            else if (YTransform == Transform.NormalZ)
            {
                y1 = Normal.StandardZ(YValues[xlo]);
                y2 = Normal.StandardZ(YValues[xhi]);
            }

            // Interpolate
            // Check for division by zero
            if ((x2 - x1) == 0)
            {
                y = y1;
            }
            else
            {
                y = y1 + (x - x1) / (x2 - x1) * (y2 - y1);
            }
            //
            if (YTransform == Transform.None)
            {
                return y;
            }
            else if (YTransform == Transform.Logarithmic)
            {
                return Math.Pow(10d, y);
            }
            else if (YTransform == Transform.NormalZ)
            {
                return Normal.StandardCDF(y);
            }

            // return NaN if we get to here
            return double.NaN;
        }

        /// <summary>
        /// Given a value x, returns an extrapolated value. 
        /// </summary>
        /// <param name="x">The value to interpolate.</param>
        public double Extrapolate(double x)
        {
            double y, x1 = 0, x2 = 0, y1 = 0, y2 = 0;

            // See if x is out of range
            if ((SortOrder == SortOrder.Ascending && x < XValues[0]) || (SortOrder == SortOrder.Descending && x > XValues[0]))
            {
                x1 = XValues[0];
                x2 = XValues[1];
                y1 = YValues[0];
                y2 = YValues[1];
            }            
            else if ((SortOrder == SortOrder.Ascending && x > XValues[Count - 1]) || (SortOrder == SortOrder.Descending && x < XValues[Count - 1]))
            {
                x1 = XValues[Count - 1];
                x2 = XValues[Count - 2];
                y1 = YValues[Count - 1];
                y2 = YValues[Count - 2];
            }
            else
            {
                return Interpolate(x);
            }

            // Get X transform
            if (XTransform == Transform.Logarithmic)
            {
                x = Tools.Log10(x);
                x1 = Tools.Log10(x1);
                x2 = Tools.Log10(x2);
            }
            else if (XTransform == Transform.NormalZ)
            {
                x = Normal.StandardZ(x);
                x1 = Normal.StandardZ(x1);
                x2 = Normal.StandardZ(x2);
            }

            // Get Y transform
            if (YTransform == Transform.Logarithmic)
            {
                y1 = Tools.Log10(y1);
                y2 = Tools.Log10(y2);
            }
            else if (YTransform == Transform.NormalZ)
            {
                y1 = Normal.StandardZ(y1);
                y2 = Normal.StandardZ(y2);
            }

            // Extrapolate
            // Check for division by zero
            if ((y2 - y1) == 0)
            {
                y = y1;
            }
            else
            {
                y = y1 - (x1 - x) * (y2 - y1) / (x2 - x1);
            }
            //
            if (YTransform == Transform.None)
            {
                return y;
            }
            else if (YTransform == Transform.Logarithmic)
            {
                return Math.Pow(10d, y);
            }
            else if (YTransform == Transform.NormalZ)
            {
                return Normal.StandardCDF(y);
            }

            // return NaN if we get to here
            return double.NaN;
        }

    }

}
