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
        for(int i=0; i<Species.properties.Count; i++)
        {
            BData[i] = 0;
        }
        BData[17] = 4;

        Matrix<double> A = LinearAlgebra.A;
        Vector<double> B = Vector<double>.Build.DenseOfArray(BData);

        Debug.Log("Species Amounts Before:");
        for(int i=0; i<B.Count(); i++)
        {
            Debug.Log($"{list[i].Key}: {B[i]}");
        }

        Debug.Log("Element Amounts Before:");
        Vector<double> elements = A * B;
        Debug.Log($"H: {elements[0]}");
        Debug.Log($"C: {elements[1]}");
        Debug.Log($"N: {elements[2]}");
        Debug.Log($"O: {elements[3]}");
        Debug.Log($"F: {elements[4]}");

        B = ChemicalAdjuster.Adjust(B, 17, -3);
        
        Debug.Log("Species Amounts After:");
        for(int i=0; i<B.Count(); i++)
        {
            Debug.Log($"{list[i].Key}: {B[i]}");
        }

        Debug.Log("Element Amounts After:");
        elements = A * B;
        Debug.Log($"H: {elements[0]}");
        Debug.Log($"C: {elements[1]}");
        Debug.Log($"N: {elements[2]}");
        Debug.Log($"O: {elements[3]}");
        Debug.Log($"F: {elements[4]}");
    }

    void Update()
    {
        
    }

    void PrintSpeciesInfo(string species)
    {
        Debug.Log($"Species {species}:");
        Debug.Log($"H:{Species.properties[species].H}");
        Debug.Log($"C:{Species.properties[species].C}");
        Debug.Log($"N:{Species.properties[species].N}");
        Debug.Log($"O:{Species.properties[species].O}");
        Debug.Log($"F:{Species.properties[species].F}");
        Debug.Log($"Molecular Mass: {Species.properties[species].molecularMass}");
        Debug.Log($"a1: {Species.properties[species].a[0]}");
        Debug.Log($"a2: {Species.properties[species].a[1]}");
        Debug.Log($"a3: {Species.properties[species].a[2]}");
        Debug.Log($"a4: {Species.properties[species].a[3]}");
        Debug.Log($"a5: {Species.properties[species].a[4]}");
        Debug.Log($"a6: {Species.properties[species].a[5]}");
        Debug.Log($"a7: {Species.properties[species].a[6]}");
        Debug.Log($"b1: {Species.properties[species].b[0]}");
        Debug.Log($"b2: {Species.properties[species].b[1]}");
    }
}
