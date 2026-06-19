using UnityEngine;
using CCEC;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;

public class Testing : MonoBehaviour
{
    void Start()
    {
        var list = Species.properties.ToList();
        double[] BData = new double[Species.properties.Count];
        double[] reactantTemperature = new double[Species.properties.Count()];

        //CH4 - O2
        //BData[10] = 1;
        //reactantTemperature[10] = 300;
        //BData[6] = 2;
        //reactantTemperature[6] = 90;
        
        //H2 - O2
        BData[1] = 2;
        reactantTemperature[1] = 300;
        BData[6] = 1;
        reactantTemperature[6] = 90;

        //H2 - O2 J2
        //BData[1] = 1;
        //reactantTemperature[1] = 300;
        //BData[6] = 0.3394;
        //reactantTemperature[6] = 90;

        //H2 - F2
        //BData[1] = 1;
        //reactantTemperature[1] = 300;
        //BData[9] = 1;
        //reactantTemperature[9] = 90;

        //Aerozine50 - N2O4
        //BData[11] = 0.278;
        //reactantTemperature[11] = 298.15;
        //BData[10] = 0.296;
        //reactantTemperature[10] = 298.15;
        //BData[4] = 0.148;
        //reactantTemperature[4] = 298.15;
        //BData[33] = 0.574;
        //reactantTemperature[33] = 298.15;

        Matrix<double> A = ChemicalAdjuster.A;
        Vector<double> B = Vector<double>.Build.DenseOfArray(BData);

        Debug.Log("Species Amounts Before:");
        for(int i=0; i<B.Count(); i++)
        {
            if(B[i] > 0)
            {
                Debug.Log($"{list[i].Key}: {B[i]}");
            }
        }

        Debug.Log("Element Amounts Before:");
        Vector<double> elements = A * B;
        Debug.Log($"H: {elements[0]}");
        Debug.Log($"C: {elements[1]}");
        Debug.Log($"N: {elements[2]}");
        Debug.Log($"O: {elements[3]}");
        Debug.Log($"F: {elements[4]}");

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        (Vector<double> species, double tempK) data = ChemicalAdjuster.SolveChemistry(B, 9700000, reactantTemperature);
        B =data.species;
        double temperature = data.tempK;
        stopwatch.Stop();
        
        Debug.Log("Species Amounts After:");
        for(int i=0; i<B.Count(); i++)
        {
            if(B[i] > 0)
            {
                Debug.Log($"{list[i].Key}: {B[i]}");
            }
        }

        Debug.Log("Element Amounts After:");
        elements = A * B;
        Debug.Log($"H: {elements[0]}");
        Debug.Log($"C: {elements[1]}");
        Debug.Log($"N: {elements[2]}");
        Debug.Log($"O: {elements[3]}");
        Debug.Log($"F: {elements[4]}");

        Debug.Log("Molar Fractions After:");
        double totalMoles = 0;
        for(int i=0; i<B.Count(); i++)
        {
            totalMoles += B[i];
        }
        for(int i=0; i<B.Count(); i++)
        {
            if(B[i] > 0)
            {
                Debug.Log($"{list[i].Key}: {B[i]/totalMoles}");
            }
        }
        Debug.Log($"Combustion temperature of {temperature}K");
        Debug.Log($"{stopwatch.Elapsed} elapsed");

    }

    void Update()
    {
        
    }
}
