using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra.Factorization;
using UnityEngine;

namespace CCEC
{
    public static class ChemicalAdjuster
    {
        public static Vector<double> Adjust(Vector<double> amounts, int index, double change) //the species amounts, the species to change, and how much to change it by.
        {
            Matrix<double> elementMatrix = LinearAlgebra.A;
            double tolerance = 1e-5;
            int maxIterations = 10000;

            int m = elementMatrix.RowCount;
            int n = elementMatrix.ColumnCount;

            Vector<double> targetTotals = elementMatrix * amounts; //the total amount of HCNOF
            Vector<double> result = amounts.Clone();
            double forcedValue = amounts[index] + change; //fix the chosen species to change amount to the inputted value
            if(forcedValue < 0)
            {
                forcedValue = 0;
                Debug.Log("below-zero error");
            }

            result[index] = forcedValue;

            bool[] fixedMask = new bool[n]; //mask of species amounts to lock in place
            fixedMask[index] = true;

            for(int iter=0; iter < maxIterations; iter++)
            {
                List<int> freeIndeces = new List<int>(n);
                List<int> fixedIndeces = new List<int>(n);

                for(int i=0; i<n; i++)
                {
                    if(fixedMask[i]) //add all fixed indeces to the list and free ones to their list.
                    {
                        fixedIndeces.Add(i);
                    }
                    else
                    {
                        freeIndeces.Add(i);
                    }
                }

                Vector<double> rhs = targetTotals.Clone();
                foreach(int i in fixedIndeces)
                {
                    rhs -= elementMatrix.Column(i) * result[i];
                }
                
                if(freeIndeces.Count == 0)
                {
                    Vector<double> residual = elementMatrix*result - targetTotals;
                    if(residual.L2Norm() <= tolerance)
                    {
                        Debug.Log("All values locked & problem not solved");
                    }
                    return Clean(result, tolerance);
                }
                
                Matrix<double> AFree = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix.Create(m, freeIndeces.Count, 0.0);
                for (int c=0; c < freeIndeces.Count; c++)
                {
                    int colIndex = freeIndeces[c];

                    for (int r = 0; r < m; r++)
                    {
                        AFree[r, c] = elementMatrix[r, colIndex];
                    }
                }

                Vector<double> xFree = MathNet.Numerics.LinearAlgebra.Double.DenseVector.Create(freeIndeces.Count, 0.0);
                for (int i = 0; i < freeIndeces.Count; i++)
                {
                    xFree[i] = amounts[freeIndeces[i]];
                }

                Vector<double> correctionTarget = rhs - (AFree * xFree);
                Matrix<double> gram = AFree * AFree.Transpose();
                Svd<double> svd = gram.Svd();
                Vector<double> lambda = svd.Solve(correctionTarget);

                Vector<double> gramResidual = gram*lambda - correctionTarget;
                if(gramResidual.L2Norm() > tolerance)
                {
                    Debug.Log("Problem");
                }

                Vector<double> correction = AFree.TransposeThisAndMultiply(lambda);
                Vector<double> candidateFree = xFree + correction;

                bool addedNewFixed = false;
                for (int i = 0; i < freeIndeces.Count; i++)
                {
                    if (candidateFree[i] < -tolerance)
                    {
                        int badIndex = freeIndeces[i];
                        fixedMask[badIndex] = true;
                        result[badIndex] = 0.0;
                        addedNewFixed = true;
                    }
                }

                if(addedNewFixed)
                {
                    continue;
                }
                for(int i=0; i<freeIndeces.Count; i++)
                {
                    result[freeIndeces[i]] = candidateFree[i] < 0.0 ? 0.0 : candidateFree[i];
                }
                Vector<double> finalResidual = elementMatrix*result - targetTotals;
                 if (finalResidual.L2Norm() <= tolerance)
                {
                    return Clean(result, tolerance);
                }
            }
            Debug.Log("Badd!!");
            return null;
        }
        
        static Vector<double> Clean(Vector<double> v, double tolerance) //make really tiny values in a vector zero
        {
            Vector<double> cleaned = v.Clone();
            for(int i=0; i<cleaned.Count; i++)
            {
                if(Math.Abs(cleaned[i]) < tolerance)
                {
                    cleaned[i] = 0;
                }
            }
            return cleaned;
        }
    }
}