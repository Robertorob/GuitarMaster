using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarMaster
{
    public enum ScaleName { Minor, Major, Flamenco, Blues, Other, Flamenco2 }

    public class MyScale
    {
        public ScaleName scaleName;
        public int[] scaleIntervals;

        public MyScale(ScaleName _scaleName, int[] _scale)
        {
            scaleName = _scaleName;
            scaleIntervals = _scale;
        }

        public static ScaleName GetScaleName(string text)
        {
            ScaleName scaleName = ScaleName.Other;
            switch (text)
            {
                case "Minor":
                    scaleName = ScaleName.Minor;
                    break;
                case "Major":
                    scaleName = ScaleName.Major;
                    break;
                case "Blues":
                    scaleName = ScaleName.Blues;
                    break;
                case "Flamenco":
                    scaleName = ScaleName.Flamenco;
                    break;
                case "Flamenco2":
                    scaleName = ScaleName.Flamenco2;
                    break;

            }
            return scaleName;
        }
    }
}
