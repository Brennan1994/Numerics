﻿using System;

namespace Numerics.Mathematics.Optimization
{

    /// <summary>
    /// The Brent optimization algorithm. The function need not be differentiable, and no derivatives are taken.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// This class contains a shared function for finding the minimum or maximum of a function using Brent's method.
    /// </para>
    /// <para>
    /// References:
    /// </para>
    /// <para>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// <see href="https://en.wikipedia.org/wiki/Brent%27s_method"/>
    /// </para>
    /// </remarks>
    public class BrentSearch : Optimizer
    {
        /// <summary>
        /// Construct a new Brent optimization method. 
        /// </summary>
        /// <param name="objectiveFunction">The objective function to evaluate.</param>
        /// <param name="lowerBound">The lower bound (inclusive) of the interval containing the optimal point.</param>
        /// <param name="upperBound">The upper bound (inclusive) of the interval containing the optimal point.</param>
        public BrentSearch(Func<double, double> objectiveFunction, double lowerBound, double upperBound) : base((x) => objectiveFunction(x[0]), 1)
        {
            // validate inputs
            if (upperBound < lowerBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", "The upper bound cannot be less than the lower bound.");
            }
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        /// <summary>
        /// The lower bound (inclusive) of the interval containing the optimal point. 
        /// </summary>
        public double LowerBound { get; private set; }

        /// <summary>
        /// The upper bound (inclusive) of the interval containing the optimal point.
        /// </summary>
        public double UpperBound { get; private set; }

        /// <summary>
        /// Implements the actual optimization algorithm. This method should minimize the objective function. 
        /// </summary>
        protected override void Optimize()
        {
            // REFERENCE: "Numerical Recipes, Routines and Examples in BASIC    
            // Companion Manual to Numerical Recipes, the 
            // art of scientific computing.
            // J. C. Sprott,         
            // Cambridge University Press, 1991"  

            // Define variables
            bool cancel = false;
            double ax = LowerBound, bx = 0.5 * (UpperBound + LowerBound), cx = UpperBound;
            // Golder ratio and a small number which protects against trying to achieve 
            // fractional accuracy for a minimum that happens to be exactly zero.
            double CGOLD = 0.381966d;
            double ZEPS = Tools.DoubleMachineEpsilon * 1.0e-3;
            double a, b, d = 0.0, etemp, fu, fv, fw, fx;
            double p, q, r, tol1, tol2, u, v, w, x, xm;
            double e = 0.0;

            a = (ax < cx ? ax : cx);
            b = (ax > cx ? ax : cx);
            x = w = v = bx;
            fw = fv = fx = Evaluate(new double[] { x }, ref cancel);
            for (int i = 1; i <= MaxIterations; i++)
            {
                xm = 0.5 * (a + b);
                tol2 = 2.0 * (tol1 = RelativeTolerance * Math.Abs(x) + ZEPS);
                // Test for done here
                if (Math.Abs(x - xm) <= tol2 - 0.5d * (b - a))
                {
                    UpdateStatus(OptimizationStatus.Success);
                    return;
                }
                // Construct a trial parabolic fit
                if (Math.Abs(e) > tol1)
                {
                    r = (x - w) * (fx - fv);
                    q = (x - v) * (fx - fw);
                    p = (x - v) * q - (x - w) * r;
                    q = 2.0 * (q - r);
                    if (q > 0.0) p = -p;
                    q = Math.Abs(q);
                    etemp = e;
                    e = d;
                    if (Math.Abs(p) >= Math.Abs(0.5 * q * etemp) || p <= q * (a - x) || p >= q * (b - x))
                        d = CGOLD * (e = (x >= xm ? a - x : b - x));
                    // The above conditions determine the acceptability of the parabolic fit. 
                    // here we take the golden section step into the larger of the two segments. 
                    else
                    {
                        d = p / q;
                        u = x + d;
                        if (u - a < tol2 || b - u < tol2)
                            d = Tools.Sign(tol1, xm - x);
                    }
                }
                else
                {
                    d = CGOLD * (e = (x >= xm ? a - x : b - x));
                }

                u = (Math.Abs(d) >= tol1 ? x + d : x + Tools.Sign(tol1, d));
                fu = Evaluate(new double[] { u }, ref cancel);
                if (cancel == true) return;
                // This is the one function evaluation per iteration.
                if (fu <= fx)
                {
                    if (u >= x) a = x; else b = x;
                    v = w;
                    w = x;
                    x = u;
                    fv = fw;
                    fw = fx;
                    fx = fu;
                }
                else
                {
                    if (u < x) a = u; else b = u;
                    if (fu <= fw || w == x)
                    {
                        v = w;
                        w = u;
                        fv = fw;
                        fw = fu;
                    }
                    else if (fu <= fv || v == x || v == w)
                    {
                        v = u;
                        fv = fu;
                    }
                }
                // Done with housekeeping. Back for another iteration
            }

            // If we made it to here, the maximum iterations were reached.
            UpdateStatus(OptimizationStatus.MaximumIterationsReached);
        }

        /// <summary>
        /// Bracket the objective function minimum.
        /// </summary>
        /// <param name="s">Starting step size. Default = 1E-2.</param>
        /// <param name="k">Expansion factor. Default = 2.</param>
        public void Bracket(double s = 1E-2, double k = 2d)
        {
            double a = LowerBound, b = a + s;
            double fa = ObjectiveFunction(new double[] { a });
            double fb = ObjectiveFunction(new double[] { b });
            double c, fc, temp;
            if (fb > fa)
            {
                temp = a;
                a = b;
                b = temp;
                temp = fa;
                fa = fb;
                fb = temp;
                s *= -1;
            }
            while (true)
            {
                c = b + s;
                fc = ObjectiveFunction(new double[] { c });
                if (fc > fb) break;
                a = b;          
                b = c;
                fa = fb;
                fb = fc;
            }
            if (a < c)
            {
                LowerBound = a;
                UpperBound = c;
            }
            else
            {
                LowerBound = c;
                UpperBound = a;
            }

        }

    }
}