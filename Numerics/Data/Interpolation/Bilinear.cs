﻿using Numerics.Distributions;
using System;
using System.Linq;

namespace Numerics.Data
{

    /// <summary>
    /// A class for bilinear interpolation.
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
    public class Bilinear
    {

        /// <summary>
        /// Construct new bilinear interpolation. 
        /// </summary>
        /// <param name="x1Values">Array of x1-values.</param>
        /// <param name="x2Values">Array of x2-values.</param>
        /// <param name="yValues">2-Column array of y-values; Yij = y(x1i, x2j). The first column is the same length as the x1 array, and the second column is the same length as the x2 array.</param>
        /// <param name="sortOrder">The sort order of the x1- and x2-values, either ascending or descending. Both need to have the same sort order. Default = Ascending. </param>
        public Bilinear(double[] x1Values, double[] x2Values, double[,] yValues, SortOrder sortOrder = SortOrder.Ascending)
        {
            // Validate 
            if (yValues.GetLength(0) != x1Values.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(yValues), "The number of columns in the y-array must be the same length as the x1 array.");
            }

            if (yValues.GetLength(1) != x2Values.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(yValues), "The number of rows in the y-array must be the same length as the x2 array.");
            }

            this.X1Values = x1Values;
            this.X2Values = x2Values;
            this.YValues = yValues;
            X1LI = new Linear(x1Values, x1Values, sortOrder);
            X2LI = new Linear(x2Values, x2Values, sortOrder);
            SortOrder = sortOrder;
        }

        private readonly Linear X1LI;
        private readonly Linear X2LI;
        private bool _useSmartSearch = true;

        /// <summary>
        /// The array of x1-values.
        /// </summary>
        public double[] X1Values { get; protected set; }

        /// <summary>
        /// The array of x2-values.
        /// </summary>
        public double[] X2Values { get; protected set; }

        /// <summary>
        /// The array of y-values.
        /// </summary>
        public double[,] YValues { get; protected set; }

        /// <summary>
        /// Determines whether to use a smart searching algorithm or just sequential search.
        /// </summary>
        public bool UseSmartSearch
        {
            get { return _useSmartSearch; }
            set
            {
                _useSmartSearch = value;
                X1LI.UseSmartSearch = value;
                X1LI.UseSmartSearch = value;
            }
        }

        /// <summary>
        /// The transform for the x1-values. Default = None.
        /// </summary>
        public Transform X1Transform { get; set; } = Transform.None;

        /// <summary>
        /// The transform for the x2-values. Default = None.
        /// </summary>
        public Transform X2Transform { get; set; } = Transform.None;

        /// <summary>
        /// The transform for the y-values. Default = None.
        /// </summary>
        public Transform YTransform { get; set; } = Transform.None;

        /// <summary>
        /// The sort order of the x1-values, either ascending or descending. Default = Ascending. 
        /// </summary>
        public SortOrder SortOrder { get; private set; } = SortOrder.Ascending;

        /// <summary>
        /// Given a value x1 and x2, returns an interpolated value for y. 
        /// </summary>
        /// <param name="x1">The x1-value to interpolate.</param>
        /// <param name="x2">The x2-value to interpolate.</param>
        public double Interpolate(double x1, double x2)
        {
            // Handle edge cases where x1 and/or x2 are out of range. 
            // Both out of range
            if ((SortOrder == SortOrder.Ascending && x1 < X1Values[0] && x2 < X2Values[0]) ||
                (SortOrder == SortOrder.Descending && x1 > X1Values[0] && x2 > X2Values[0])) return YValues[0, 0]; // Top left
            if ((SortOrder == SortOrder.Ascending && x1 < X1Values[0] && x2 > X2Values[X2LI.Count - 1]) ||
                (SortOrder == SortOrder.Descending && x1 > X1Values[0] && x2 < X2Values[X2LI.Count - 1])) return YValues[0, X2LI.Count - 1]; // Top Right
            if ((SortOrder == SortOrder.Ascending && x1 > X1Values[X1LI.Count - 1] && x2 > X2Values[X2LI.Count - 1]) ||
                (SortOrder == SortOrder.Descending && x1 < X1Values[X1LI.Count - 1] && x2 < X2Values[X2LI.Count - 1])) return YValues[X1LI.Count - 1, X2LI.Count - 1]; // Bottom Right
            if ((SortOrder == SortOrder.Ascending && x1 > X1Values[X1LI.Count - 1] && x2 < X2Values[0]) ||
                (SortOrder == SortOrder.Descending && x1 < X1Values[X1LI.Count - 1] && x2 > X2Values[0])) return YValues[X1LI.Count - 1, 0]; // Bottom Left

            double y, t, u, x1i, x1ii, x2j, x2jj, yij, yiij, yiijj, yijj, x1lb, x1ub, x2lb, x2ub;        
            int i = X1LI.Search(x1);
            int j = X2LI.Search(x2);

            // Get x1 transform
            x1i = X1LI.XValues[i];
            x1ii = X1LI.XValues[i + 1];
            x1lb = X1Values[0];
            x1ub = X1Values[X1LI.Count - 1];
            if (X1Transform == Transform.Logarithmic)
            {
                x1 = Math.Log10(x1);
                x1i = Math.Log10(x1i);
                x1ii = Math.Log10(x1ii);
                x1lb = Math.Log10(x1lb);
                x1ub = Math.Log10(x1ub);
            }
            else if (X1Transform == Transform.NormalZ)
            {
                x1 = Normal.StandardZ(x1);
                x1i = Normal.StandardZ(x1i);
                x1ii = Normal.StandardZ(x1ii);
                x1lb = Normal.StandardZ(x1lb);
                x1ub = Normal.StandardZ(x1ub);
            }

            // Get x2 transform
            x2j = X2LI.YValues[j];
            x2jj = X2LI.YValues[j + 1];
            x2lb = X2Values[0];
            x2ub = X2Values[X2LI.Count - 1];
            if (X2Transform == Transform.Logarithmic)
            {
                x2 = Math.Log10(x2);
                x2j = Math.Log10(x2j);
                x2jj = Math.Log10(x2jj);
                x2lb = Math.Log10(x2lb);
                x2ub = Math.Log10(x2ub);
            }
            else if (X2Transform == Transform.NormalZ)
            {
                x2 = Normal.StandardZ(x2);
                x2j = Normal.StandardZ(x2j);
                x2jj = Normal.StandardZ(x2jj);
                x2lb = Normal.StandardZ(x2lb);
                x2ub = Normal.StandardZ(x2ub);
            }

            // Get y transform
            yij = YValues[i, j];
            yiij = YValues[i + 1, j];
            yiijj = YValues[i + 1, j + 1];
            yijj = YValues[i, j + 1];
            if (YTransform == Transform.Logarithmic)
            {
                yij = Math.Log10(yij);
                yiij = Math.Log10(yiij);
                yiijj = Math.Log10(yiijj);
                yijj = Math.Log10(yijj);
            }
            else if (YTransform == Transform.NormalZ)
            {
                yij = Normal.StandardZ(yij);
                yiij = Normal.StandardZ(yiij);
                yiijj = Normal.StandardZ(yiijj);
                yijj = Normal.StandardZ(yijj);
            }

            // Interpolate
            t = (x1 - x1i) / (x1ii - x1i);
            u = (x2 - x2j) / (x2jj - x2j);
            y = (1d - t) * (1d - u) * yij + t * (1d - u) * yiij + t * u * yiijj + (1d - t) * u * yijj;

            // x1 out of range - 1D linear interpolation
            if ((SortOrder == SortOrder.Ascending && x1 < x1lb) || 
                (SortOrder == SortOrder.Descending && x1 > x1lb)) y = yij + (x2 - x2j) / (x2jj - x2j) * (yijj - yij);
            if ((SortOrder == SortOrder.Ascending && x1 > x1ub) || 
                (SortOrder == SortOrder.Descending && x1 < x1ub)) y = yiij + (x2 - x2j) / (x2jj - x2j) * (yiijj - yiij);

            // x2 out of range - 1D linear interpolation
            if ((SortOrder == SortOrder.Ascending && x2 < x2lb) || 
                (SortOrder == SortOrder.Descending && x2 > x2lb)) y = yij + (x1 - x1i) / (x1ii - x1i) * (yiij - yij);
            if ((SortOrder == SortOrder.Ascending && x2 > x2ub) || 
                (SortOrder == SortOrder.Descending && x2 < x2ub)) y = yijj + (x1 - x1i) / (x1ii - x1i) * (yiijj - yijj);

            // Back transform y
            if (YTransform == Transform.Logarithmic)
            {
                y = Math.Pow(10d, y);
            }
            else if (YTransform == Transform.NormalZ)
            {
                y = Normal.StandardCDF(y);
            }

            return y;
        }
    }
}
