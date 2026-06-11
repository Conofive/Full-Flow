using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Unity.VisualScripting;
using UnityEngine;

namespace CCEC
{
    public static class ChemicalAdjuster
    {
        public static Vector<double> Adjust(Vector<double> amounts, int index, double change) //the species amounts, the species to change, and how much to change it by.
        {
            Matrix<double> elementMatrix = LinearAlgebra.A;
            double tolerance = 1e-5;
            int maxIterations = 1000;

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
                    return null;
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

        static double ComputeEnergy(Vector<double> species, double pressurePa, double tempK)
        {
            double R = 8.314462618; //gas constant
            double referencePressure = 101325; //1 atmosphere

            var speciesList = Species.properties.ToList(); //list of species names corresponding to the species vector

            //make vector of the standard state energies of each species
            double[] standardStateEnergies = new double[species.Count()];
            for(int i=0; i<species.Count(); i++)
            {
                string speciesName = speciesList[i].Key;
                double[] a = Species.properties[speciesName].a;
                double[] b = Species.properties[speciesName].b;
                double entropy = Species.ThermodynamicProperties(a, b, tempK).entropy;
                double enthalpy = Species.ThermodynamicProperties(a, b, tempK).enthalpy;
                standardStateEnergies[i] = enthalpy - tempK*entropy;
            }

            //make vector giving the mole fraction of each species out of the total
            double total = 0;
            foreach(double mole in species)
            {
                total += mole;
            }
            double[] moleFractions = new double[species.Count()];
            for(int i=0; i<moleFractions.Count(); i++)
            {
                moleFractions[i] = species[i] / total;
            }

            //make vector of the chemical potential of each species
            double[] chemicalPotentials = new double[species.Count()];
            for(int i=0; i<species.Count(); i++)
            {
                moleFractions[i] = Math.Max(moleFractions[i], 0.0001); //cant be zero; number wont matter anyway
                chemicalPotentials[i] = standardStateEnergies[i] + R * tempK * Math.Log(moleFractions[i] * (pressurePa/referencePressure));
            }

            //add up the product of the amount of each species and its chemical potential to find the total gibbs free energy
            double energy = 0;
            for(int i=0; i<species.Count(); i++)
            {
                energy += species[i] * chemicalPotentials[i];
            }
            return energy;
        }
        public static Vector<double> MinimizeGibbsEnergy(Vector<double> species, double pressurePa, double tempK)
        {
            double minimumChange = 0.001;
            int maxIterations = 100;
            Vector<double> currentAmounts = species.Clone();
            for(int loop=0; loop<maxIterations; loop++)
            {
                double largestShift = 0;
                for(int chemical=0; chemical<species.Count(); chemical++)
                {
                    Vector<double> tempAmounts = currentAmounts.Clone();
                    double change = minimumChange;
                    double direction = 0;

                    //find direction to shift
                    double beforeEnergy = ComputeEnergy(tempAmounts, pressurePa, tempK);
                    tempAmounts = Adjust(tempAmounts, chemical, tempAmounts[chemical]*change);

                    double afterEnergy = 9999999;
                    if(tempAmounts != null)
                    {
                        afterEnergy = ComputeEnergy(tempAmounts, pressurePa, tempK);
                    }

                    if(afterEnergy < beforeEnergy)
                    {
                        direction = 1;
                    }
                    else
                    {
                        tempAmounts = currentAmounts.Clone();
                        Debug.Log($"Loop {loop}");
                        Debug.Log($"Chemical {chemical}");
                        if(tempAmounts == null)
                        {
                            Debug.Log("Null!");
                        }
                        else
                        {
                            Debug.Log(tempAmounts[chemical]);
                        }
                        
                        tempAmounts = Adjust(tempAmounts, chemical, -tempAmounts[chemical]*change);
                        afterEnergy = 9999999;
                        if(tempAmounts != null)
                        {
                            afterEnergy = ComputeEnergy(tempAmounts, pressurePa, tempK);
                        }

                        if(afterEnergy < beforeEnergy)
                        {
                            direction = -1;
                        }
                        else
                        {
                            continue; //can't move in either direction, give up
                        }
                    }
                    tempAmounts = currentAmounts.Clone();

                    //find largest amount it can shift in that direction
                    change = 0;
                    for(double i = 0.1; i>=minimumChange; i*=0.1)
                    {
                        tempAmounts = Adjust(tempAmounts, chemical, tempAmounts[chemical] * direction * i);
                        afterEnergy = 9999999;
                        if(tempAmounts != null)
                        {
                            afterEnergy = ComputeEnergy(tempAmounts, pressurePa, tempK);
                            currentAmounts = tempAmounts.Clone();
                        }
                        else
                        {
                            tempAmounts = currentAmounts.Clone();
                        }
                        if(afterEnergy < beforeEnergy)
                        {
                            change = i;
                            break;
                        }
                    }
                    currentAmounts = tempAmounts.Clone();

                    //report shift as largest if so
                    Debug.Log(change);
                    if(change > largestShift)
                    {
                        largestShift = change;
                    }
                }
                if(largestShift < minimumChange)
                {
                    break;
                }
            }
            return currentAmounts.Clone();
        }
    }
}