using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//XY平面的矩形
public class XYRect : Hitable
{
    private float x0;
    private float x1;
    private float y0;
    private float y1;
    private float k;
    private Material mat;
    public XYRect() { }
    public XYRect(float x0, float x1, float y0, float y1, float k, Material mat)
    {
        this.X0 = x0;
        this.X1 = x1;
        this.Y0 = y0;
        this.Y1 = y1;
        this.k = k;
        this.mat = mat;
    }

    public float X0 { get => x0; set => x0 = value; }
    public float X1 { get => x1; set => x1 = value; }
    public float Y0 { get => y0; set => y0 = value; }
    public float Y1 { get => y1; set => y1 = value; }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(X0, Y0, k - 0.0001f), new Vector3D(X1, Y1, k + 0.0001f));
        return true;
    }
    //判断是否击中
    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        float t = (k - r.Origin.Z) / r.Direction.Z;
        if (t < tMin || t > tMax)
            return false;
        float x = r.Origin.X + t * r.Direction.X;
        float y = r.Origin.Y + t * r.Direction.Y;
        if (x < X0 || x > X1 || y < Y0 || y > Y1)
            return false;
        rec.u = (x - X0) / (X1 - X0);
        rec.v = (y - Y0) / (Y1 - Y0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(0, 0, 1);
        return true;
    }
}
//XZ平面上的矩形
public class XZRect : Hitable
{
    private float x0;
    private float x1;
    private float z0;
    private float z1;
    private float k;
    private Material mat;
    public XZRect() { }
    public XZRect(float x0, float x1, float z0, float z1, float k, Material mat)
    {
        this.X0 = x0;
        this.X1 = x1;
        this.Z0 = z0;
        this.Z1 = z1;
        this.k = k;
        this.mat = mat;
    }

    public float X0 { get => x0; set => x0 = value; }
    public float X1 { get => x1; set => x1 = value; }
    public float Z0 { get => z0; set => z0 = value; }
    public float Z1 { get => z1; set => z1 = value; }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(X0, k - 0.0001f, Z0), new Vector3D(X1, k + 0.0001f, Z1));
        return true;
    }
    //判断是否击中
    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        float t = (k - r.Origin.Y) / r.Direction.Y;
        if (t < tMin || t > tMax)
            return false;
        float x = r.Origin.X + t * r.Direction.X;
        float z = r.Origin.Z + t * r.Direction.Z;
        if (x < X0 || x > X1 || z < Z0 || z > Z1)
            return false;
        rec.u = (x - X0) / (X1 - X0);
        rec.v = (z - Z0) / (Z1 - Z0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(0, 1, 0);
        return true;
    }
    public override float PDFValue(Vector3D o, Vector3D v)
    {
        HitRecord rec;
        if (this.Hit(new Ray(o, v), 0.001f, float.MaxValue, out rec))
        {
            float area = (X1 - X0) * (Z1 - Z0);
            float distanceSquared = rec.t * rec.t * v.SquaredMagnitude();
            float cosine = Mathf.Abs((v * rec.normal) / v.Length());
            return distanceSquared / (cosine * area);
        }
        else
            return 0;
    }
    public override Vector3D Random(Vector3D o)
    {
        Vector3D randomPoint = new Vector3D(X0 + Mathf.Randomfloat()
            * (X1 - X0), k, Z0 + Mathf.Randomfloat() * (Z1 - Z0));
        return randomPoint - o;
    }
}
//YZ平面上的矩形
public class YZRect : Hitable
{
    private float y0;
    private float y1;
    private float z0;
    private float z1;
    private float k;
    private Material mat;
    public YZRect() { }
    public YZRect(float y0, float y1, float z0, float z1, float k, Material mat)
    {
        this.Y0 = y0;
        this.Y1 = y1;
        this.Z0 = z0;
        this.Z1 = z1;
        this.k = k;
        this.mat = mat;
    }

    public float Y0 { get => y0; set => y0 = value; }
    public float Y1 { get => y1; set => y1 = value; }
    public float Z0 { get => z0; set => z0 = value; }
    public float Z1 { get => z1; set => z1 = value; }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(k - 0.0001f,Y0, Z0 ), new Vector3D(k + 0.0001f,Y1, Z1));
        return true;
    }
    //判断是否击中
    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        float t = (k - r.Origin.X) / r.Direction.X;
        if (t < tMin || t > tMax)
            return false;
        float y = r.Origin.Y + t * r.Direction.Y;
        float z = r.Origin.Z + t * r.Direction.Z;
        if (y < Y0 || y > Y1 || z < Z0 || z > Z1)
            return false;
        rec.u = (y - Y0) / (Y1 - Y0);
        rec.v = (z - Z0) / (Z1 - Z0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(1, 0, 0);
        return true;
    }
  
}
