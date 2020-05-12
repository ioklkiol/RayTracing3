using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mathf
{
    private static long seed = 1;

    //得到一个0到1的随机数
    public static float Randomfloat()
    {
        //var seed = Guid.NewGuid().GetHashCode();    //使用Guid类得到一个接近唯一的随机数种子
        //Random r = new Random(seed);
        //int i = r.Next(0, 100000);
        //return (float)i / 100000;
        seed = (0x5DEECE66DL * seed + 0xB16) & 0xFFFFFFFFFFFFL;
        return (seed >> 16) / (float)0x100000000L;
    }
    public static Vector3D RandomCosineDirection()
    {
        float r1 = Mathf.Randomfloat();
        float r2 = Mathf.Randomfloat();
        float z = Mathf.Sqrt(1 - r2);
        float phi = 2 * Mathf.PI * r1;
        float x = Mathf.Cos(phi) * 2 * Mathf.Sqrt(r2);
        float y = Mathf.Sin(phi) * 2 * Mathf.Sqrt(r2);
        return new Vector3D(x, y, z);
    }
    public static Vector3D RandomInUnitShpere()
    {
        Vector3D p = new Vector3D();
        do
        {
            p = 2 * new Vector3D(Mathf.Randomfloat(), Mathf.Randomfloat(), Mathf.Randomfloat()) - new Vector3D(1, 1, 1);
        } while (p.SquaredMagnitude() >= 1);
        return p;
    }

    public const float PI = 3.14159274f;
    public static float Sqrt(float v) => (float)Math.Sqrt(Convert.ToDouble(v));

    public static float Range(float v, float min, float max) => (v <= min) ? min :
        v >= max ? max : v;

    public static int Range(int v, int min, int max) => (v <= min) ? min :
        v >= max ? max : v;
    public static float Tan(float f) => (float)Math.Tan(f);
    public static float Min(float a, float b) { return a < b ? a : b; }
    public static float Max(float a, float b) { return a > b ? a : b; }

    public static void Swap(ref float a, ref float b)
    {
        var c = a;
        a = b;
        b = c;
    }
    public static float Pow(float f, float p) => (float)Math.Pow(f, p);

    public static float Sin(float f) => (float)Math.Sin(f);

    public static float Cos(float f) => (float)Math.Cos(f);

    public static float Asin(float f) => (float)Math.Asin(f);

    public static float Floor(float f) => (float)Math.Floor(f);
    public static int Floor2int(float f) => (int)Math.Floor(f);


    public static float Abs(float f) => (float)Math.Abs(f);
    public static float Acos(float f) => (float)Math.Acos(f);


    public static float Atan(float f) => (float)Math.Atan(f);

    public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);

    public static float Log(float f) => (float)Math.Log(f);

}