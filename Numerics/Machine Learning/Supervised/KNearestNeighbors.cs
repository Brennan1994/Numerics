﻿using Numerics.Data.Statistics;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Numerics.MachineLearning
{
    /// <summary>
    /// The k-Nearest Neighbors (k-NN) algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/K-nearest_neighbors_algorithm" />
    /// </para>
    /// </remarks>
    public class KNearestNeighbors
    {

        #region Construction

        /// <summary>
        /// Create new k-NN method.
        /// </summary>
        /// <param name="x">The training 1D array of predictor values.</param>
        /// <param name="y">The training response vector.</param>
        /// <param name="k">The number of nearest neighbors.</param>
        public KNearestNeighbors(double[] x, double[] y, int k)
        {
            K = k;
            Y = new Vector(y);
            X = new Matrix(x);
            if (Y.Length != X.NumberOfRows) throw new ArgumentException("The y vector must be the same length as the x matrix.");
            if (Y.Length < 10) throw new ArgumentException("There must be at least ten training data points.");
        }

        /// <summary>
        /// Create new k-NN method.
        /// </summary>
        /// <param name="x">The training 2D array of predictor values.</param>
        /// <param name="y">The training response vector.</param>
        /// <param name="k">The number of nearest neighbors.</param>
        public KNearestNeighbors(double[,] x, double[] y, int k)
        {
            K = k;
            Y = new Vector(y);
            X = new Matrix(x);
            if (Y.Length != X.NumberOfRows) throw new ArgumentException("The y vector must be the same length as the x matrix.");
            if (Y.Length < 10) throw new ArgumentException("There must be at least ten training data points.");
        }

        /// <summary>
        /// Create new k-NN method.
        /// </summary>
        /// <param name="x">The training matrix of predictor values.</param>
        /// <param name="y">The training response vector.</param>
        /// <param name="k">The number of nearest neighbors.</param>
        public KNearestNeighbors(Matrix x, Vector y, int k)
        {
            K = k;
            Y = y;
            X = x;
            if (Y.Length != X.NumberOfRows) throw new ArgumentException("The y vector must be the same length as the x matrix.");
            if (Y.Length < 10) throw new ArgumentException("There must be at least ten training data points.");
        }

        #endregion

        #region Members

        /// <summary>
        /// The number of clusters.
        /// </summary>
        public int K { get; private set; }

        /// <summary>
        /// The training vector of response values.
        /// </summary>
        public Vector Y { get; private set; }

        /// <summary>
        /// The training matrix of predictor values. 
        /// </summary>
        public Matrix X { get; private set; }

        /// <summary>
        /// Determines whether this is for regression or classification. Default = regression.
        /// </summary>
        public bool IsRegression { get; set; } = true;

        #endregion

        #region Methods

        /// <summary>
        /// Returns the prediction from k-Nearest neighbors.
        /// </summary>
        /// <param name="X">The 1D array of predictors.</param>
        public double[] Predict(double[] X)
        {
            return kNNPredict(this.X, this.Y, new Matrix(X));
        }

        /// <summary>
        /// Returns the prediction from k-Nearest neighbors.
        /// </summary>
        /// <param name="X">The 2D array of predictors.</param>
        public double[] Predict(double[,] X)
        {
            return kNNPredict(this.X, this.Y, new Matrix(X));         
        }

        /// <summary>
        /// Returns the prediction from k-Nearest neighbors.
        /// </summary>
        /// <param name="X">The matrix of predictors.</param>
        public double[] Predict(Matrix X)
        {
            return kNNPredict(this.X, this.Y, X);
        }

        /// <summary>
        /// Returns the bootstrapped prediction from k-Nearest neighbors.
        /// </summary>
        /// <param name="X">The 1D array of predictors.</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        public double[] BootstrapPredict(double[] X, int seed = -1)
        {
            return kNNBootstrapPredict(this.X, this.Y, new Matrix(X), seed);
        }

        /// <summary>
        /// Returns the bootstrapped prediction from k-Nearest neighbors.
        /// </summary>
        /// <param name="X">The 2D array of predictors.</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        public double[] BootstrapPredict(double[,] X, int seed = -1)
        {
            return kNNBootstrapPredict(this.X, this.Y, new Matrix(X), seed);
        }

        /// <summary>
        /// Returns the bootstrapped prediction from k-Nearest neighbors.
        /// </summary>
        /// <param name="X">The matrix of predictors.</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        public double[] BootstrapPredict(Matrix X, int seed = -1)
        {
            return kNNBootstrapPredict(this.X, this.Y, X, seed);
        }

        /// <summary>
        /// Returns the bootstrapped prediction intervals in a 2D array with columns: lower, median, upper, mean. 
        /// </summary>
        /// <param name="X">The 1D array of predictors.</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        /// <param name="realizations">The number of bootstrap realizations. Default = 1,000.</param>
        /// <param name="alpha">The confidence level; Default = 0.1, which will result in the 90% confidence intervals.</param>
        public double[,] PredictionIntervals(double[] X, int seed = -1, int realizations = 1000, double alpha = 0.1)
        {
            return kNNPredictionIntervals(this.X, this.Y, new Matrix(X), seed, realizations, alpha);
        }

        /// <summary>
        /// Returns the bootstrapped prediction intervals in a 2D array with columns: lower, median, upper, mean. 
        /// </summary>
        /// <param name="X">The 2D array of predictors.</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        /// <param name="realizations">The number of bootstrap realizations. Default = 1,000.</param>
        /// <param name="alpha">The confidence level; Default = 0.1, which will result in the 90% confidence intervals.</param>
        public double[,] PredictionIntervals(double[,] X, int seed = -1, int realizations = 1000, double alpha = 0.1)
        {
            return kNNPredictionIntervals(this.X, this.Y, new Matrix(X), seed, realizations, alpha);
        }

        /// <summary>
        /// Returns the bootstrapped prediction intervals in a 2D array with columns: lower, median, upper, mean. 
        /// </summary>
        /// <param name="X">The matrix of predictors.</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        /// <param name="realizations">The number of bootstrap realizations. Default = 1,000.</param>
        /// <param name="alpha">The confidence level; Default = 0.1, which will result in the 90% confidence intervals.</param>
        public double[,] PredictionIntervals(Matrix X, int seed = -1, int realizations = 1000, double alpha = 0.1)
        {
            return kNNPredictionIntervals(this.X, this.Y, X, seed, realizations, alpha);
        }

        /// <summary>
        /// Returns the prediction from k-Nearest neighbors.
        /// </summary>
        /// <param name="xTrain">The training matrix of predictors.</param>
        /// <param name="yTrain">The training response vector.</param>
        /// <param name="xTest">The test matrix of predictors</param>
        private double[] kNNPredict(Matrix xTrain, Vector yTrain, Matrix xTest)
        {
            if (xTest.NumberOfColumns != xTrain.NumberOfColumns) return null;
            int R = xTest.NumberOfRows;
            var result = new double[R];
            for (int i = 0; i < R; i++)
            {
                var point = xTest.Row(i);

                // Get distances
                var items = new kNNItem[xTrain.NumberOfRows];
                Parallel.For(0, xTrain.NumberOfRows, idx =>
                {
                    items[idx].Index = idx;
                    items[idx].Distance = Tools.Distance(point, xTrain.Row(idx));
                });

                // Sort items and find the k-nearest neighbors
                Array.Sort(items, (a, b) => a.Distance.CompareTo(b.Distance));
                var knn = new double[K];

                // Record results
                if (IsRegression == true)
                {
                    // get the inverse distance weighted average of the values of k nearest neighbors
                    double sum = 0;
                    double avg = 0;
                    for (int j = 0; j < K; j++)
                    {
                        double w = items[j].Distance > 0 ? 1d / Tools.Sqr(items[j].Distance) : 1;
                        knn[j] = yTrain[items[j].Index] * w;
                        sum += w;
                    }
                    for (int j = 0; j < K; j++)
                        avg += knn[j] / sum;

                    result[i] = avg;
                }
                else
                {
                    // get the most common value
                    for (int j = 0; j < K; j++)
                        knn[j] = yTrain[items[j].Index];
                    
                    result[i] = knn.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the bootstrapped prediction from k-Nearest neighbors.
        /// </summary>
        /// <param name="xTrain">The training matrix of predictors.</param>
        /// <param name="yTrain">The training response vector.</param>
        /// <param name="xTest">The test matrix of predictors</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        private double[] kNNBootstrapPredict(Matrix xTrain, Vector yTrain, Matrix xTest, int seed = -1)
        {
            var rnd = seed > 0 ? new MersenneTwister(seed) : new MersenneTwister();
            var idxs = rnd.NextIntegers(0, xTrain.NumberOfRows, xTrain.NumberOfRows);
            var bootX = new Matrix(xTrain.NumberOfRows, xTrain.NumberOfColumns);
            var bootY = new Vector(yTrain.Length);
            for (int i = 0; i < xTrain.NumberOfRows; i++)
            {
                for (int j = 0; j < xTrain.NumberOfColumns; j++)
                {
                    bootX[i, j] = xTrain[idxs[i], j];
                }
                bootY[i] = yTrain[idxs[i]];
            }
            return kNNPredict(bootX, bootY, xTest);
        }

        /// <summary>
        /// Returns the bootstrapped prediction intervals in a 2D array with columns: lower, median, upper, mean. 
        /// </summary>
        /// <param name="xTrain">The training matrix of predictors.</param>
        /// <param name="yTrain">The training response vector.</param>
        /// <param name="xTest">The test matrix of predictors</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        /// <param name="realizations">The number of bootstrap realizations. Default = 1,000.</param>
        /// <param name="alpha">The confidence level; Default = 0.1, which will result in the 90% confidence intervals.</param>
        private double[,] kNNPredictionIntervals(Matrix xTrain, Vector yTrain, Matrix xTest, int seed = -1, int realizations = 1000, double alpha = 0.1)
        {
            var percentiles = new double[] { alpha / 2d, 0.5, 1d - alpha / 2d };
            var output = new double[xTest.NumberOfRows, 4]; // lower, median, upper, mean

            var bootResults = new double[xTest.NumberOfRows, realizations];
            var rnd = seed > 0 ? new MersenneTwister(seed) : new MersenneTwister();
            var seeds = rnd.NextIntegers(realizations);

            // Bootstrap the predictions
            Parallel.For(0, realizations, idx => { bootResults.SetColumn(idx, kNNBootstrapPredict(xTrain, yTrain, xTest, seeds[idx]));});

            // Process results
            Parallel.For(0, xTest.NumberOfRows, idx => 
            {
                var values = bootResults.GetRow(idx);
                Array.Sort(values);

                // Record percentiles for CIs
                for (int j = 0; j < percentiles.Length; j++)
                    output[idx, j] = Statistics.Percentile(values, percentiles[j], true);

                output[idx, 3] = Statistics.ParallelMean(values);
            });

            return output;
        }

        /// <summary>
        /// A structure for storing a k-NN item.
        /// </summary>
        private struct kNNItem
        {
            /// <summary>
            /// The index of the item.
            /// </summary>
            public int Index;

            /// <summary>
            /// The distance of the item.
            /// </summary>
            public double Distance;
        }

        #endregion

    }
}
