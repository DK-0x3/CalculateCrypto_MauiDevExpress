using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate_MauiDevExpress_1._0.Class
{
    public static class ConvertX
    {
        static Dictionary<string, double> ConvertSquareDm = new Dictionary<string, double>()
        {
            {"км", 100000000},
            {"га",  1000000},
            {"м", 100},
            {"дм", 1},
            {"см", 0.01},
            {"мм", 0.0001}
        };
        static Dictionary<string, double> ConvertSquareDm2 = new Dictionary<string, double>()
        {
            {"км", 0.00000001},
            {"га",  0.000001},
            {"м", 0.01},
            {"дм", 1},
            {"см", 100},
            {"мм", 10000}
        };

        static Dictionary<string, double> ConvertData1 = new Dictionary<string, double>()
        {
            {"б", 0.000000953674316},
            {"кб",  0.0009765625},
            {"мб", 1},
            {"гб", 1024},
            {"тб", 1048576},
            {"пб", 0.00000000107374182}
        };
        static Dictionary<string, double> ConvertData2 = new Dictionary<string, double>()
        {
            {"б", 1048576},
            {"кб", 1024},
            {"мб", 1},
            {"гб", 0.0009765625},
            {"тб", 0.000000953674316},
            {"пб", 0.000000000931322575}
        };

        static Dictionary<string, double> ConvertLenght1 = new Dictionary<string, double>()
        {
            {"км", 10_000},
            {"м",  10},
            {"дм", 1},
            {"см", 0.1},
            {"мм", 0.01}
        };
        static Dictionary<string, double> ConvertLenght2 = new Dictionary<string, double>()
        {
            {"км", 0.0001},
            {"м",  0.1},
            {"дм", 1},
            {"см", 10},
            {"мм", 100}
        };

        static Dictionary<string, double> ConvertWeight1 = new Dictionary<string, double>()
        {
            {"т", 1_000_000},
            {"ц",  100_000},
            {"кг", 1000},
            {"г", 1},
            {"мг", 0.001}
        };
        static Dictionary<string, double> ConvertWeight2 = new Dictionary<string, double>()
        {
            {"т", 0.000001},
            {"ц",  0.00001},
            {"кг", 0.001},
            {"г", 1},
            {"мг", 1000}
        };

        static Dictionary<string, double> ConvertSpeed2 = new Dictionary<string, double>()
        {
            {"км/ч", 1},
            {"км/с",  0.00027777778},
            {"м/с", 0.27777778}
        };
        static Dictionary<string, double> ConvertSpeed1 = new Dictionary<string, double>()
        {
            {"км/ч", 1},
            {"км/с",  3600},
            {"м/с", 3.6}
        };

        private static void CountIf(ref string count)
        {
            if (count == "")
            {
                count = "0";
            }
            else if (count[^1] == ',')
            {
                count += "0";
            }

        }

        public static string ConvertSquare(string From, string To, string Count)
        {
            From = From.Replace("²", "");
            To = To.Replace("²", "");
            CountIf(ref Count);
            return Math.Round((ConvertSquareDm[From] * double.Parse(Count)) * ConvertSquareDm2[To], 6).ToString();
        }

        public static string ConvertData(string From, string To, string Count)
        {
            CountIf(ref Count);
            return Math.Round((ConvertData1[From] * double.Parse(Count)) * ConvertData2[To], 6).ToString();
        }
        public static string ConvertLenght(string From, string To, string Count)
        {
            CountIf(ref Count);
            return Math.Round((ConvertLenght1[From] * double.Parse(Count)) * ConvertLenght2[To], 6).ToString();
        }
        public static string ConvertWeight(string From, string To, string Count)
        {
            CountIf(ref Count);
            return Math.Round((ConvertWeight1[From] * double.Parse(Count)) * ConvertWeight2[To], 6).ToString();
        }

        public static string ConvertNumberS(string valueX, int sourceBase, int targetBase)
        {
            CountIf(ref valueX);
            try
            {
                // Преобразуем значение X из исходной системы счисления в десятичную
                int decimalValue = Convert.ToInt32(valueX, sourceBase);

                // Преобразуем десятичное значение в целевую систему счисления
                string result = Convert.ToString(decimalValue, targetBase);

                return result;
            }
            catch (FormatException)
            {
                return "неверный ввод числа в СС";
            }



        }

        public static string ConvertSpeed(string From, string To, string Count)
        {
            CountIf(ref Count);
            return Math.Round((ConvertSpeed1[From] * double.Parse(Count)) * ConvertSpeed2[To], 6).ToString();
        }
    }
}
