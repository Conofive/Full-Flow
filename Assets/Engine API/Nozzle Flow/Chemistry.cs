using UnityEngine;
using CCEC;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace EnginePhysics
{
    public static class Chemistry
    {
        public static Dictionary<string, (string name, double baseTempK, double massPortion)[]> nameMap = new()
        {
            //fuels --------------------

            ["H2"] = new[] {("H2", 295.0, 1.0)},
            ["LH2"] = new[] {("H2", 100.0, 1.0)}, //cryogenics are 100 because the polynomial def won't support lower; barely a difference anyhow

            ["N2"] = new[] {("N2", 295.0, 1.0)},
            ["LN2"] = new[] {("N2", 100.0, 1.0)},

            ["CH4"] = new[] {("CH4", 295.0, 1.0)},
            ["LCH4"] = new[] {("CH4", 100.0, 1.0)},

            ["NH3"] = new[] {("NH3", 295.0, 1.0)},
            ["LNH3"] = new[] {("NH3", 100.0, 1.0)},

            ["N2H4"] = new[] {("N2H4", 295.0, 1.0)},

            //oxidizers ---------------------
            
            ["O2"] = new[] {("O2", 295.0, 1.0)},
            ["LO2"] = new[] {("O2", 100.0, 1.0)},

            ["F2"] = new[] {("F2", 295.0, 1.0)},
            ["LF2"] = new[] {("F2", 100.0, 1.0)},

            ["HTP-98"] = new[] {("H2O2", 295.0, 0.98), ("H2O", 295.0, 0.02)},
            ["HTP-90"] = new[] {("H2O2", 295.0, 0.90), ("H2O", 295.0, 0.10)},
            ["HTP-85"] = new[] {("H2O2", 295.0, 0.85), ("H2O", 295.0, 0.15)},
            ["HTP-75"] = new[] {("H2O2", 295.0, 0.75), ("H2O", 295.0, 0.25)},

            ["NTO"] = new[] {("N2O4", 295.0, 1.0)},

            ["IRFNA"] = new[] {("HNO3", 295.0, 0.834), ("NO2", 295.0, 0.14), ("H2O", 295.0, 0.02), ("HF", 295.0, 0.006)},
        };

        public static FuelComposition CreateComposition((string name, float massPortion)[] fuels)
        {
            //instantiate main chemical tuple array
            //convert names into chemical tuple arrays
            //add each index of each array to the main array

            (string name, double baseTempK, double massPortion)[] chemicals = new (string name, double baseTempK, double massPortion)[0];

            foreach((string name, float massPortion) fuel in fuels)
            {
                (string name, double baseTempK, double massPortion)[] chemicalArray = nameMap[fuel.name];
                foreach((string name, double baseTempK, double massPortion) chemical in chemicalArray)
                {
                    (string name, double baseTempK, double massPortion) chemToAdd = (chemical.name, chemical.baseTempK, chemical.massPortion * fuel.massPortion / Species.properties[chemical.name].molecularMass);
                    chemicals = chemicals.Append(chemToAdd).ToArray();
                }
            }

            FuelComposition composition = new FuelComposition(chemicals);
            return composition;
        }
    }

    public class RocketEngine
    {
        //input info
        public double chamberPressurePascals;
        Vector<double> B;

        //output info
        double gamma;
        double meanMolarMass;
        double tempK;

        public RocketEngine(double _chamberPressurePascals, FuelComposition fuels)
        {
            chamberPressurePascals = _chamberPressurePascals;
            B = Vector<double>.Build.DenseOfArray(fuels.BData);

            //Debug
            //var list = Species.properties.ToList();
            //Debug.Log("Species Amounts Before:");
            //for(int i=0; i<B.Count(); i++)
            //{
            //    if(B[i] > 0)
            //    {
            //        Debug.Log($"{list[i].Key}: {B[i]}");
            //    }
            //}

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            
            (Vector<double> species, double tempK) data = ChemicalAdjuster.SolveChemistry(B, chamberPressurePascals, fuels.reactantTemperature);
            B = data.species;
            double temperature = data.tempK;
            stopwatch.Stop();

            //Debug
            //Matrix<double> A = ChemicalAdjuster.A;
            //Debug.Log("Species Amounts After:");
            //for(int i=0; i<B.Count(); i++)
            //{
            //    if(B[i] > 0)
            //    {
            //        Debug.Log($"{list[i].Key}: {B[i]}");
            //    }
            //}
            //Debug.Log("Molar Fractions After:");
            //double totalMoles = 0;
            //for(int i=0; i<B.Count(); i++)
            //{
            //    totalMoles += B[i];
            //}
            //for(int i=0; i<B.Count(); i++)
            //{
            //    if(B[i] > 0)
            //    {
            //        Debug.Log($"{list[i].Key}: {B[i]/totalMoles}");
            //    }
            //}

            tempK = temperature;
            gamma = ChemicalAdjuster.ExhaustProperties(B, temperature).gamma;
            meanMolarMass = ChemicalAdjuster.ExhaustProperties(B, temperature).meanMolarMass;

            //Debug
            Debug.Log($"{stopwatch.Elapsed} elapsed");
            Debug.Log($"Combustion temperature of {tempK}K");
            Debug.Log($"Gamma: {gamma}");
            Debug.Log($"MMM: {meanMolarMass}");
        }
    }

    public class FuelComposition
    {
        public double[] BData = new double[Species.properties.Count];
        public double[] reactantTemperature = new double[Species.properties.Count()];

        public FuelComposition((string name, double baseTempK, double massPortion)[] _chemicals)
        {
            var speciesNames = Species.properties.Keys.ToList();

            foreach ((string name, double baseTempK, double massPortion) chemical in _chemicals)
            {
                int index = speciesNames.IndexOf(chemical.name);

                if (index == -1)
                {
                    Debug.LogError($"Species '{chemical.name}' was not found in Species.properties!");
                    continue;
                }

                BData[index] += chemical.massPortion;
                reactantTemperature[index] = chemical.baseTempK;
            }
        }
    }
}