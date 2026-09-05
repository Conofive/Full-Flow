using UnityEngine;
using CCEC;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using EnginePhysics;

public class Testing : MonoBehaviour
{
    void Start()
    {
        TestEngine("Hydrolox", 9700000, new (string, float)[]{("LH2", 1), ("LO2", 8)});
        TestEngine("Methalox", 9700000, new (string, float)[]{("LCH4", 1), ("LO2", 4)});
    }

    void TestEngine(string debugMessage, double pressurePa, (string name, float massPortion)[] _fuels)
    {
        Debug.Log($"{debugMessage}:");
        (string name, float massPortion)[] fuels = _fuels;
        RocketEngine engine = new RocketEngine(pressurePa, Chemistry.CreateComposition(fuels));
    }

    void Update()
    {
        
    }
}
