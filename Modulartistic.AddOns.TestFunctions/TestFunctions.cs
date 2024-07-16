

namespace Modulartistic.AddOns.TestFunctions
{
    [AddOn]
    public class TestFunctions
    {
        #region return Tests
        public static double TestReturnDouble()
        {
            return 1.5;
        }

        public static int TestReturnInt()
        {
            return -1;
        }

        public static bool TestReturnBool()
        {
            return true;
        }

        public static long TestReturnLong()
        {
            return 1L;
        }

        public static float TestReturnFloat() 
        {
            return 0.1f;
        }

        public static ushort TestReturnUshort()
        {
            return 1;
        }

        public static char TestReturnChar()
        {
            return 'a';
        }

        public static string TestReturnString()
        {
            return "Hello World";
        }

        public static CustomType TestReturnCustomType()
        {
            return new CustomType(3, 4);
        }
        #endregion

        #region Parameter Tests
        public static double TestTakeDouble(double x)
        {
            return 0;
        }

        public static double TestTakeInt(int x)
        {
            return 0;
        }

        public static double TestTakeString(string x)
        {
            return 0;
        }

        public static double TestTakeBool(bool x)
        {
            return 0;
        }

        public static double TestTakeChar(char x)
        {
            return 0;
        }

        public static double TestTakeCustomType(CustomType x)
        {
            return 0;
        }
        #endregion

        #region Overload Test
        public double TestOverloading()
        {
            return 0;
        }
        public double TestOverloading(double x)
        {
            return x;
        }

        public int TestOverloading(int x)
        {
            return x;
        }

        public char TestOverloading(string s, int i)
        {
            return s[i];
        }
        #endregion

        #region test processing custom Type
        public int TestProcessingCustomType(int a, int b)
        {
            CustomType x = new CustomType(a, b);
            return x.a + x.b;
        }

        public int TestProcessingCustomType(CustomType x)
        {
            return x.a + x.b;
        }
        #endregion
    }

    public class CustomType
    {
        public int a; 
        public int b;
        public CustomType(int a, int b) { this.a = a; this.b = b; }
    }
}
