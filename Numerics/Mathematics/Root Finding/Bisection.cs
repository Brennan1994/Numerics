﻿using System;

namespace Numerics.Mathematics.RootFinding
{

    /// <summary>
    /// Contains the bisection root-finding algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// This class contains a shared function for finding the solution to the equation f(x)=0 using the bisection method.
    /// </para>
    /// <para>
    /// References:
    /// </para>
    /// <para>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// <see href="https://en.wikipedia.org/wiki/Root-finding_algorithm"/>
    /// <see href="https://en.wikipedia.org/wiki/Bisection_method"/>
    /// </para>
    /// </remarks>
    public class Bisection
    {

        /// <summary>
        /// Use the bisection method to find a solution of the equation f(x)=0.
        /// </summary>
        /// <param name="f">The function to solve.</param>
        /// <param name="firstGuess">This is the first guess at the root.</param>
        /// <param name="lowerBound">The lower bound of the interval containing the root.</param>
        /// <param name="upperBound">The upper bound of the interval containing the root.</param>
        /// <param name="tolerance">Optional. Desired tolerance for both the root and the function value at the root.
        /// The root will be refined until the tolerance is achieved or the maximum number of iterations is reached. Default = 1e-12.</param>
        /// <param name="maxIterations">Optional. Maximum number of iterations. Default = 1000.</param>
        /// <param name="reportFailure">Optional. If set to true, an exception will be thrown if the routine fails to converge.
        /// If set to false, the root from the last iteration will be returned if the routine fails to converge. Default = True.</param>
        /// <returns>
        /// The root to the equation f(x)=0 given the specified tolerance.
        /// </returns>
        public static double Solve(Func<double, double> f, double firstGuess, double lowerBound, double upperBound, double tolerance = 0.000000000001d, int maxIterations = 1000, bool reportFailure = true)
        {

            // INPUT: Function f(), endpoint values a, b, tolerance TOL, maximum iterations NMAX
            // CONDITIONS: a < b, either f(a) < 0 and f(b) > 0 or f(a) > 0 and f(b) <0
            // OUTPUT: value which differs from a root of f(x)=0 by less than TOL

            // Define variables
            double root = firstGuess;
            bool solutionFound = false;
            double xl = lowerBound;
            double xh = upperBound;
            double fl = f(xl);
            double fmid = f(xh);
            double dx, xmid;

            // validate inputs
            if (upperBound < lowerBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", "The upper bound (b) cannot be less than the lower bound (a).");
            }
            if (root < lowerBound || root > upperBound)
            {
                throw new ArgumentOutOfRangeException("firstGuess", "The first guess must be between the upper and lower bound.");
            }
            if (fl*fmid >=0.0)
            {
                throw new ArgumentException("Bisection method failed because the root is not bracketed.");
            }
            if (fl < 0d)
            {
                // Orient the search so that f>0 lies at X+DX
                dx = xh - xl;
            }
            else
            {
                dx = xl - xh;
            }
            // Bisection loop
            for (int i = 1; i <= maxIterations; i++)
            {
                fmid = f(xmid = root + (dx *= 0.5));
                if (fmid <= 0) root = xmid;
                // check if the solution meets required tolerance
                if (Math.Abs(dx) <= tolerance || fmid == 0.0)
                {
                    // a solution has been achieved, so exit loop
                    solutionFound = true;
                    break;
                }
            }

            // return results of solver
            if (solutionFound == false && reportFailure == true)
            {
                throw new ArgumentException("Bisection method failed to find root.");
            }
            else
            {
                return root;
            }
        }
    }
}