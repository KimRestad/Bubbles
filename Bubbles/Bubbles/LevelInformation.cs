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
        private const int C_NUM_BALLS_DECA = 4;
        private const int C_NUM_BALLS_HECTO = 6;
        private const int C_NUM_BALLS_KILO = 7;
        private const int C_NUM_BALLS_MEGA = 8;
        private const int C_NUM_BALLS_GIGA = 9;
        private const int C_NUM_BALLS_TERA = 10;

        public static LevelDetails Easy(Level level)
        {
            int startNumRows = 4;
            switch (level)
            {
                case Level.Deca:
                    return new LevelDetails(1.25f, C_NUM_BALLS_DECA - 1, 0.075f, startNumRows);
                case Level.Hecto:
                    return new LevelDetails(1.20f, C_NUM_BALLS_HECTO - 1, 0.080f, startNumRows);
                case Level.Kilo:
                    return new LevelDetails(1.15f, C_NUM_BALLS_KILO - 1, 0.085f, startNumRows);
                case Level.Mega:
                    return new LevelDetails(1.10f, C_NUM_BALLS_MEGA - 1, 0.090f, startNumRows);
                case Level.Giga:
                    return new LevelDetails(1.05f, C_NUM_BALLS_GIGA - 1, 0.095f, startNumRows);
                case Level.Tera:
                    return new LevelDetails(1.0f, C_NUM_BALLS_TERA - 1, 0.100f, startNumRows);
            }

            return null;
        }

        public static LevelDetails Normal(Level level)
        {
            int startNumRows = 5;
            switch (level)
            {
                case Level.Deca:
                    return new LevelDetails(1.05f, C_NUM_BALLS_DECA, 0.10f, startNumRows);
                case Level.Hecto:
                    return new LevelDetails(1.0f, C_NUM_BALLS_HECTO, 0.11f, startNumRows);
                case Level.Kilo:
                    return new LevelDetails(0.95f, C_NUM_BALLS_KILO, 0.12f, startNumRows);
                case Level.Mega:
                    return new LevelDetails(0.9f, C_NUM_BALLS_MEGA, 0.13f, startNumRows);
                case Level.Giga:
                    return new LevelDetails(0.85f, C_NUM_BALLS_GIGA, 0.14f, startNumRows);
                case Level.Tera:
                    return new LevelDetails(0.8f, C_NUM_BALLS_TERA, 0.15f, startNumRows);
            }

            return null;
        }

        public static LevelDetails Hard(Level level)
        {
            int startNumRows = 6;
            switch (level)
            {
                case Level.Deca:
                    return new LevelDetails(0.85f, C_NUM_BALLS_DECA, 0.14f, startNumRows);
                case Level.Hecto:
                    return new LevelDetails(0.80f, C_NUM_BALLS_HECTO, 0.16f, startNumRows);
                case Level.Kilo:
                    return new LevelDetails(0.75f, C_NUM_BALLS_KILO, 0.18f, startNumRows);
                case Level.Mega:
                    return new LevelDetails(0.70f, C_NUM_BALLS_MEGA, 0.20f, startNumRows);
                case Level.Giga:
                    return new LevelDetails(0.65f, C_NUM_BALLS_GIGA, 0.22f, startNumRows);
                case Level.Tera:
                    return new LevelDetails(0.60f, C_NUM_BALLS_TERA, 0.24f, startNumRows);
            }

            return null;
        }
    }
}
