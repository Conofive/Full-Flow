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
            {"H", GetData("H", "1000.000")},
            {"H2", GetData("H2", "1000.000")},

            {"C", GetData("C", "1000.000")},

            {"N", GetData("N", "1000.000")},
            {"N2", GetData("N2", "1000.000")},
            
            {"O", GetData("O", "1000.000")},
            {"O2", GetData("O2", "1000.000")},
            {"O3", GetData("O3", "1000.000")},
            
            {"F", GetData("F", "1000.000")},
            {"F2", GetData("F2", "1000.000")},

            //hydrogen-carbon compounds
            {"CH4", GetData("CH4", "1000.000")}, //methane

            //hydrogen-nitrogen compounds
            {"N2H4", GetData("N2H4", "1000.000")}, //hydrazine
            {"NH", GetData("NH", "1000.000")},
            {"NH2", GetData("NH2", "1000.000")},
            {"NH3", GetData("NH3", "1000.000")}, //ammonia

            //hydrogen-nitrogen-oxygen compounds
            {"HNO3", GetData("HNO3", "1000.000")}, //nitric acid

            //hydrogen-oxygen compounds
            {"OH", GetData("OH", "1000.000")},
            {"HO2", GetData("HO2", "1000.000")},
            {"H2O", GetData("H2O", "1000.000")},
            {"H2O2", GetData("H2O2", "1000.000")},

            //hydrogen-fluorine compounds
            {"HF", GetData("HF", "1000.000")},
            {"H2F2", GetData("H2F2", "1000.000")},

            //carbon-nitrogen compounds
            {"CN", GetData("CN", "1000.000")},

            //carbon-oxygen compounds
            {"CO", GetData("CO", "1000.000")},
            {"CO2", GetData("CO2", "1000.000")},

            //carbon-oxygen-fluorine compounds
            {"COF2", GetData("COF2", "1000.000")},

            //carbon-fluorine compounds
            {"CF2", GetData("CF2", "1000.000")},
            {"C2F4", GetData("C2F4", "1000.000")},
            {"CF4", GetData("CF4", "1000.000")},
            {"C2F6", GetData("C2F6", "1000.000")},
            
            //nitrogen-oxygen compounds
            {"NO", GetData("NO", "1000.000")},
            {"N2O", GetData("N2O", "1000.000")},
            {"NO2", GetData("NO2", "1000.000")},
            {"N2O4", GetData("N2O4", "200.000")}, //NTO

            //nitrogen-fluorine compounds
            {"NF3", GetData("NF3", "200.000")},

            //oxygen-fluorine compounds
            {"F2O", GetData("F2O", "200.000")},

            //reactant only - implement elsewhere because it won't work having this in the main dictionary
            //{"C12H26", (26,12,0,0,0, 170.33f, new double[0], new double[0], true)}, //RP1 surrogate
            //{"C2H8N2", (8,2,2,0,0, 60.0983f, new double[0], new double[0], true)}, //UDMH
            //{"CH6N2", (6,1,2,0,0, 46.073f, new double[0], new double[0], true)}, //MMH
            //{"C6H5NH2", (7,6,1,0,0, 93.129f, new double[0], new double[0], true)}, //Aniline


        };

        //kinda dumb to have this as a whole separate dictionary but whatever
        public static Dictionary<string, (byte H, byte C, byte N, byte O, byte F, double molecularMass, double[] a, double[] b, bool reactantOnly)> propertiesLowTemp = new()
        {
            {"H", GetData("H", "200.000")},
            {"H2", GetData("H2", "200.000")},

            {"C", GetData("C", "200.000")},

            {"N", GetData("N", "200.000")},
            {"N2", GetData("N2", "200.000")},
            
            {"O", GetData("O", "200.000")},
            {"O2", GetData("O2", "200.000")},
            {"O3", GetData("O3", "200.000")},
            
            {"F", GetData("F", "200.000")},
            {"F2", GetData("F2", "200.000")},

            //hydrogen-carbon compounds
            {"CH4", GetData("CH4", "200.000")}, //methane

            //hydrogen-nitrogen compounds
            {"N2H4", GetData("N2H4", "200.000")}, //hydrazine
            {"NH", GetData("NH", "200.000")},
            {"NH2", GetData("NH2", "200.000")},
            {"NH3", GetData("NH3", "200.000")}, //ammonia

            //hydrogen-nitrogen-oxygen compounds
            {"HNO3", GetData("HNO3", "200.000")}, //nitric acid

            //hydrogen-oxygen compounds
            {"OH", GetData("OH", "200.000")},
            {"HO2", GetData("HO2", "200.000")},
            {"H2O", GetData("H2O", "200.000")},
            {"H2O2", GetData("H2O2", "200.000")},

            //hydrogen-fluorine compounds
            {"HF", GetData("HF", "200.000")},
            {"H2F2", GetData("H2F2", "200.000")},

            //carbon-nitrogen compounds
            {"CN", GetData("CN", "200.000")},

            //carbon-oxygen compounds
            {"CO", GetData("CO", "200.000")},
            {"CO2", GetData("CO2", "200.000")},

            //carbon-oxygen-fluorine compounds
            {"COF2", GetData("COF2", "200.000")},

            //carbon-fluorine compounds
            {"CF2", GetData("CF2", "200.000")},
            {"C2F4", GetData("C2F4", "200.000")},
            {"CF4", GetData("CF4", "200.000")},
            {"C2F6", GetData("C2F6", "200.000")},
            
            //nitrogen-oxygen compounds
            {"NO", GetData("NO", "200.000")},
            {"N2O", GetData("N2O", "200.000")},
            {"NO2", GetData("NO2", "200.000")},
            {"N2O4", GetData("N2O4", "200.000")}, //NTO

            //nitrogen-fluorine compounds
            {"NF3", GetData("NF3", "200.000")},

            //oxygen-fluorine compounds
            {"F2O", GetData("F2O", "200.000")},
        };

        public static (byte H, byte C, byte N, byte O, byte F, double molecularMass, double[] a, double[] b, bool reactantOnly) GetData(string elementName, string startTemp) //notably doesn't return correct element values if any element exceeds a count of 9
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
                if(textLines[i] == startTemp)
                {
                    dataLine = i+3;
                    break;
                }
            }

            //Doesn't work because sometimes the line starts with a negative and it ruins everything so i added the "+ add stuff"; zip ties ts
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