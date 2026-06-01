using UnityEngine;
using CCEC;
using System.IO;

public class Testing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   Debug.Log("Atomic Hydrogen:");
        Debug.Log($"Mean molecular mass: {Species.GetData("H").molecularMass}");
        for(int i=0; i<7; i++)
        {
            Debug.Log($"a{i}: {Species.GetData("H").a[i]}");
        }
        for(int i=0; i<2; i++)
        {
            Debug.Log($"b{i}: {Species.GetData("H").b[i]}");
        }

        Debug.Log("Water:");
        Debug.Log($"Mean molecular mass: {Species.GetData("H2O").molecularMass}");
        for(int i=0; i<7; i++)
        {
            Debug.Log($"a{i}: {Species.GetData("H2O").a[i]}");
        }
        for(int i=0; i<2; i++)
        {
            Debug.Log($"b{i}: {Species.GetData("H2O").b[i]}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
