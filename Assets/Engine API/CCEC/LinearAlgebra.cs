using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System;

namespace CCEC
{
    public static class LinearAlgebra
    {
        public static Matrix<double> A;

        //create atom data matrix
        static LinearAlgebra()
        {
            double[,] atomData = new double[5, Species.properties.Count]; //rows are elements HCNOF, collumns are the species
            //fill atomData
            var list = Species.properties.ToList();
            for(int c=0; c<Species.properties.Count; c++)
            {
                atomData[0, c] = Species.properties[list[c].Key].H;
                atomData[1, c] = Species.properties[list[c].Key].C;
                atomData[2, c] = Species.properties[list[c].Key].N;
                atomData[3, c] = Species.properties[list[c].Key].O;
                atomData[4, c] = Species.properties[list[c].Key].F;
            }

            A = Matrix<double>.Build.DenseOfArray(atomData); //matrix mapping species to their element counts
        }
    }
}