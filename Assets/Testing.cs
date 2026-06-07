using UnityEngine;
using CCEC;
using System.IO;

public class Testing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //int frame = 0;
    //float[] hydrogenEnthalpies = new float[1000000];

    void Start()
    {
        Debug.Log($"H: {Species.properties["H"].molecularMass}");
        Debug.Log($"H2: {Species.properties["H2"].molecularMass}");
        Debug.Log($"C: {Species.properties["C"].molecularMass}");
        Debug.Log($"N: {Species.properties["N"].molecularMass}");
        Debug.Log($"N2: {Species.properties["N2"].molecularMass}");
        Debug.Log($"O: {Species.properties["O"].molecularMass}");
        Debug.Log($"O2: {Species.properties["O2"].molecularMass}");
        Debug.Log($"F: {Species.properties["F"].molecularMass}");
        Debug.Log($"F2: {Species.properties["F2"].molecularMass}");
        Debug.Log($"CH: {Species.properties["CH"].molecularMass}");
        Debug.Log($"CH2: {Species.properties["CH2"].molecularMass}");
        Debug.Log($"CH3: {Species.properties["CH3"].molecularMass}");
        Debug.Log($"CH4: {Species.properties["CH4"].molecularMass}");
        Debug.Log($"N2H4: {Species.properties["N2H4"].molecularMass}");
        Debug.Log($"NH: {Species.properties["NH"].molecularMass}");
        Debug.Log($"NH2: {Species.properties["NH2"].molecularMass}");
        Debug.Log($"NH3: {Species.properties["NH3"].molecularMass}");
        Debug.Log($"OH: {Species.properties["OH"].molecularMass}");
        Debug.Log($"H2O: {Species.properties["H2O"].molecularMass}");
        Debug.Log($"HF: {Species.properties["HF"].molecularMass}");
        Debug.Log($"CN: {Species.properties["CN"].molecularMass}");
        Debug.Log($"CO: {Species.properties["CO"].molecularMass}");
        Debug.Log($"CO2: {Species.properties["CO2"].molecularMass}");
        Debug.Log($"COF2: {Species.properties["COF2"].molecularMass}");
        Debug.Log($"CF2: {Species.properties["CF2"].molecularMass}");
        Debug.Log($"C2F4: {Species.properties["C2F4"].molecularMass}");
        Debug.Log($"CF4: {Species.properties["CF4"].molecularMass}");
        Debug.Log($"C2F6: {Species.properties["C2F6"].molecularMass}");
        Debug.Log($"NO: {Species.properties["NO"].molecularMass}");
        Debug.Log($"N2O: {Species.properties["N2O"].molecularMass}");
        Debug.Log($"NO2: {Species.properties["NO2"].molecularMass}");
        Debug.Log($"N2O4: {Species.properties["N2O4"].molecularMass}"); //!
        Debug.Log($"NF3: {Species.properties["NF3"].molecularMass}");
        Debug.Log($"F2O: {Species.properties["F2O"].molecularMass}");
        Debug.Log($"C12H26: {Species.properties["C12H26"].molecularMass}");
    }

    // Update is called once per frame
    void Update()
    {
        //frame++;
        //if(frame == 1)
        //{
        //    Debug.Log("Start");
        //}
        //if(frame == 2)
        //{
        //    for (int i=0; i<1000000; i++)
        //    {
        //        float[] a = Species.properties["H"].a;
        //        float[] b = Species.properties["H"].b;
        //        hydrogenEnthalpies[i] = Species.ThermodynamicProperties(a, b, i/100000+2000).enthalpy;
        //    }
        //    Debug.Log("Done!");
        //}
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
