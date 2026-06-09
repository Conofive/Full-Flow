using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CCEC
{
    public static class Species
    {
        //match chemical names to characteristics
        public static Dictionary<string, (byte H, byte C, byte N, byte O, byte F, double molecularMass, double[] a, double[] b, bool reactantOnly)> properties = new()
        {
            {"H", GetData("H")},
            {"H2", GetData("H2")},

            {"C", GetData("C")},

            {"N", GetData("N")},
            {"N2", GetData("N2")},
            
            {"O", GetData("O")},
            {"O2", GetData("O2")},
            
            {"F", GetData("F")},
            {"F2", GetData("F2")},

            //hydrogen-carbon compounds
            {"CH", GetData("CH")},
            {"CH2", GetData("CH2")},
            {"CH3", GetData("CH3")},
            {"CH4", GetData("CH4")}, //methane

            //hydrogen-nitrogen compounds
            {"N2H4", GetData("N2H4")}, //hydrazine
            {"NH", GetData("NH")},
            {"NH2", GetData("NH2")},
            {"NH3", GetData("NH3")}, //ammonia

            //hydrogen-nitrogen-oxygen compounds
            {"HNO3", GetData("HNO3")}, //nitric acid

            //hydrogen-oxygen compounds
            {"OH", GetData("OH")},
            {"H2O", GetData("H2O")},
            {"H2O2", GetData("H2O2")},

            //hydrogen-fluorine compounds
            {"HF", GetData("HF")},

            //carbon-nitrogen compounds
            {"CN", GetData("CN")},

            //carbon-oxygen compounds
            {"CO", GetData("CO")},
            {"CO2", GetData("CO2")},

            //carbon-oxygen-fluorine compounds
            {"COF2", GetData("COF2")},

            //carbon-fluorine compounds
            {"CF2", GetData("CF2")},
            {"C2F4", GetData("C2F4")},
            {"CF4", GetData("CF4")},
            {"C2F6", GetData("C2F6")},
            
            //nitrogen-oxygen compounds
            {"NO", GetData("NO")},
            {"N2O", GetData("N2O")},
            {"NO2", GetData("NO2")},
            {"N2O4", GetData("N2O4")}, //NTO

            //nitrogen-fluorine compounds
            {"NF3", GetData("NF3")},

            //oxygen-fluorine compounds
            {"F2O", GetData("F2O")},

            //reactant only - implement elsewhere because it won't work having this in the main dictionary
            //{"C12H26", (26,12,0,0,0, 170.33f, new double[0], new double[0], true)}, //RP1 surrogate
            //{"C2H8N2", (8,2,2,0,0, 60.0983f, new double[0], new double[0], true)}, //UDMH
            //{"CH6N2", (6,1,2,0,0, 46.073f, new double[0], new double[0], true)}, //MMH
            //{"C6H5NH2", (7,6,1,0,0, 93.129f, new double[0], new double[0], true)}, //Aniline


        };

        public static (byte H, byte C, byte N, byte O, byte F, double molecularMass, double[] a, double[] b, bool reactantOnly) GetData(string elementName) //notably doesn't return correct element values if any element exceeds a count of 9
        {
            string path = Path.Combine(Application.streamingAssetsPath, "NasaData.txt");
            string[] textLines = File.ReadAllLines(path);
            
            int elementLine = 0; //what line the desired element is stored on in the dataset

            for(int i=0; i<textLines.Length; i++)
            {
                if (textLines[i] == elementName)
                {
                    elementLine = i;
                    break;
                }
            }

            int dataLine = 0; //where the desired data starts (temp 1000-6000K)
            
            for(int i=elementLine; i<textLines.Length; i++)
            {
                if(textLines[i] == "1000.000")
                {
                    dataLine = i+3;
                    break;
                }
            }

            //Doesn't work because sometimes the line starts with a negative and it ruins everything so i added the "+ add stuff"; zip ties
            string[] dataString = new string[9];
            int add = 0;
            if(textLines[dataLine][0] == '-')
            {
                add = 1;
            }
            dataString[0] = textLines[dataLine].Substring(0, 15 + add);
            dataString[1] = textLines[dataLine].Substring(15 + add, 16);
            dataString[2] = textLines[dataLine].Substring(31 + add, 16);
            dataString[3] = textLines[dataLine].Substring(47 + add, 16);
            dataString[4] = textLines[dataLine].Substring(63 + add, 16);
            add = 0;
            if(textLines[dataLine+1][0] == '-')
            {
                add = 1;
            }
            dataString[5] = textLines[dataLine+1].Substring(0, 15 + add);
            dataString[6] = textLines[dataLine+1].Substring(15 + add, 16);
            add = 0;
            if(textLines[dataLine+2][0] == '-')
            {
                add = 1;
            }
            dataString[7] = textLines[dataLine+2].Substring(0, 15 + add);
            dataString[8] = textLines[dataLine+2].Substring(15 + add, 16);

            //ridiculous
            for(int i=0; i<9; i++)
            {
                for(int j=0; j<dataString[i].Length; j++)
                {
                    if(dataString[i][j] == 'D')
                    {
                        char[] charArray = new char[dataString[i].Length];
                        for(int k=0; k<charArray.Length; k++)
                        {
                            charArray[k] = dataString[i][k];
                        }
                        charArray[j] = 'e';
                        dataString[i] = new string(charArray);
                    }
                }
            }

            double[] a = new double[7];
            
            for(int i=0; i<7; i++)
            {
                a[i] = Convert.ToSingle(dataString[i]);
            }
            double[] b = new double[2];
            for(int i=0; i<2; i++)
            {
                b[i] = Convert.ToSingle(dataString[i+7]);
            }

            //initialize element counts
            byte H = 0;
            byte C = 0;
            byte N = 0;
            byte O = 0;
            byte F = 0;

            for(int i=0; i<5; i++)
            {
                char elementChar = textLines[elementLine+2+i][textLines[elementLine+2+i].Length-1];
                byte number = Convert.ToByte(Convert.ToString(textLines[elementLine+3+i][0])); //have to convert to string cause char converts to the ascii code

                if(elementChar == '0')
                {
                    i=5;
                    break;
                }

                switch (elementChar)
                {
                    case 'H':
                    H += number;
                    break;
                    case 'C':
                    C += number;
                    break;
                    case 'N':
                    N += number;
                    break;
                    case 'O':
                    O += number;
                    break;
                    case 'F':
                    F += number;
                    break;
                    default:
                    Debug.Log($"Element '{elementChar}' not permitted");
                    break;
                }
            }

            return (H,C,N,O,F, Convert.ToSingle(textLines[elementLine+8]), a, b, false);
        }

        public static (double heatCapacity, double enthalpy, double entropy) ThermodynamicProperties(double[] a, double[] b, double T)
        {
            double R = 8.314462618f; //gas constant

            double heatCapacity = R*(a[0]*Math.Pow(T, -2) + a[1]*Math.Pow(T, -1) + a[2] + a[3]*T + a[4]*Math.Pow(T, 2) + a[5]*Math.Pow(T, 3) + a[6]*Math.Pow(T, 4));
            double enthalpy = R*T*(-a[0]*Math.Pow(T, -2) + a[1]*Math.Log(T)/T + a[2] + a[3]*T/2 + a[4]*Math.Pow(T, 2)/3 + a[5]*Math.Pow(T, 3)/4 + a[6]*Math.Pow(T, 4)/5 + b[0]/T);
            double entropy = R*(-a[0]*Math.Pow(T, -2)/2 - a[1]*Math.Pow(T, -1) + a[2]*Math.Log(T) + a[3]*T + a[4]*Math.Pow(T, 2)/2 + a[5]*Math.Pow(T, 3)/3 + a[6]*Math.Pow(T, 4)/4 + b[1]);

            return (heatCapacity, enthalpy, entropy);
        }
    }
}   