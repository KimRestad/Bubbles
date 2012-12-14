using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bubbles
{
    class LevelDetails
    {
        public float BallSize { get; private set; }
        public int NumColours { get; private set; }
        public float AddRowModifier { get; private set; }
        public int NumRowsStart { get; private set; }

        public LevelDetails(float ballSize, int numColours, float modifier, int numRowsStart)
        {
            BallSize = ballSize;
            NumColours = numColours;
            AddRowModifier = modifier;
            NumRowsStart = numRowsStart;
        }
    }
    
    static class LevelInformation
    {
        public static LevelDetails Easy(Level level)
        {
            switch (level)
            {
                case Level.Deca:
                    return new LevelDetails(1.25f, 4, 0.075f, 3);
                case Level.Hecto:
                    return new LevelDetails(1.20f, 5, 0.080f, 3);
                case Level.Kilo:
                    return new LevelDetails(1.15f, 6, 0.085f, 3);
                case Level.Mega:
                    return new LevelDetails(1.10f, 7, 0.090f, 3);
                case Level.Giga:
                    return new LevelDetails(1.05f, 8, 0.095f, 3);
                case Level.Tera:
                    return new LevelDetails(1.0f, 9, 0.100f, 3);
            }

            return null;
        }

        public static LevelDetails Normal(Level level)
        {
            switch (level)
            {
                case Level.Deca:
                    return new LevelDetails(1.1f, 4, 0.08f, 4);
                case Level.Hecto:
                    return new LevelDetails(1.05f, 5, 0.09f, 4);
                case Level.Kilo:
                    return new LevelDetails(1.0f, 6, 0.10f, 4);
                case Level.Mega:
                    return new LevelDetails(0.95f, 7, 0.11f, 4);
                case Level.Giga:
                    return new LevelDetails(0.9f, 8, 0.12f, 4);
                case Level.Tera:
                    return new LevelDetails(0.85f, 9, 0.13f, 4);
            }

            return null;
        }

        public static LevelDetails Hard(Level level)
        {
            switch (level)
            {
                case Level.Deca:
                    return new LevelDetails(1.00f, 4, 0.09f, 5);
                case Level.Hecto:
                    return new LevelDetails(0.95f, 5, 0.11f, 5);
                case Level.Kilo:
                    return new LevelDetails(0.90f, 6, 0.13f, 5);
                case Level.Mega:
                    return new LevelDetails(0.85f, 7, 0.15f, 5);
                case Level.Giga:
                    return new LevelDetails(0.80f, 8, 0.17f, 5);
                case Level.Tera:
                    return new LevelDetails(0.75f, 9, 0.19f, 5);
            }

            return null;
        }
    }
}
