using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace CCEC
{
    public static class Species
    {
        //match chemical names to characteristics
        public static Dictionary<string, (byte H, byte C, byte N, byte O, byte F, float molecularMass, float[] a, float[] b)> properties = new() //TODO: Instead of this, make a different system that takes the numbers from the database. Make an array to define the allowed elements.
        {
            //{"H", (1,0,0,0,0},
            //{"H2", (2,0,0,0,0)},
//
            //{"C", (0,1,0,0,0)},
            //{"N", (0,0,1,0,0)},
            //{"N2", (0,0,2,0,0)},
            //
            //{"O", (0,0,0,1,0)},
            //{"O2", (0,0,0,2,0)},
            //
            //{"F", (0,0,0,0,1)},
            //{"F2", (0,0,0,0,2)},
//
            ////hydrogen-carbon compounds
            //{"CH", (1,1,0,0,0)},
            //{"CH2", (2,1,0,0,0)},
            //{"CH3", (3,1,0,0,0)},
            //{"CH4", (4,1,0,0,0)}, //methane
            //{"C12H26", (26,12,0,0,0)}, //RP1 approximation
//
            ////hydrogen-carbon-nitrogen compounds
            //{"H8N2C2", (8,2,2,0,0)}, //UDMH
//
            ////hydrogen-nitrogen compounds
            //{"N2H4", (4,0,2,0,0)}, //hydrazine
            //{"NH", (1,0,1,0,0)},
            //{"NH2", (2,0,1,0,0)},
            //{"NH3", (3,0,1,0,0)}, //ammonia
//
            ////hydrogen-oxygen compounds
            //{"OH", (1,0,0,1,0)},
            //{"H2O", (2,0,0,1,0)},
//
            ////hydrogen-fluorine compounds
            //{"HF", (1,0,0,0,1)},
//
            ////carbon-nitrogen compounds
            //{"CN", (0,1,1,0,0)},
//
            ////carbon-oxygen compounds
            //{"CO", (0,1,0,1,0)},
            //{"CO2", (0,1,0,2,0)},
//
            ////carbon-oxygen-fluorine compounds
            //{"COF2", (0,1,0,1,2)},
//
            ////carbon-fluorine compounds
            //{"CF2", (0,1,0,0,2)},
            //{"C2F4", (0,2,0,0,4)},
            //{"CF4", (0,1,0,0,4)},
            //{"C2F6", (0,2,0,0,6)},
            //
            ////nitrogen-oxygen compounds
            //{"NO", (0,0,1,1,0)},
            //{"N2O", (0,0,2,1,0)},
            //{"NO2", (0,0,1,2,0)},
            //{"N2O4", (0,0,2,4,0)}, //NTO
//
            ////nitrogen-fluorine compounds
            //{"NF3", (0,0,1,0,3)},
//
            ////oxygen-fluorine compounds
            //{"OF2", (0,0,0,1,2)},
        };

        public static (float molecularMass, float[] a, float[] b) GetData(string elementName)
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

            //Doesn't work because sometimes the line starts with a negative and it ruins everything so i added the "add" crap
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
                        char[] charArray = new char[dataString[i].Length]; //surely theres a better wayyy
                        for(int k=0; k<charArray.Length; k++)
                        {
                            charArray[k] = dataString[i][k];
                        }
                        charArray[j] = 'e';
                        dataString[i] = new string(charArray);
                    }
                }
            }

            float[] a = new float[7];
            
            for(int i=0; i<7; i++)
            {
                a[i] = Convert.ToSingle(dataString[i]);
            }
            float[] b = new float[2];
            for(int i=0; i<2; i++)
            {
                b[i] = Convert.ToSingle(dataString[i+7]);
            }

            return (Convert.ToSingle(textLines[elementLine+8]), a, b);
        }
    }
}