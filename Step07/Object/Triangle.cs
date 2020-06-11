using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//XY平面的三角形
public class XYTriangle : Hitable
{
    private float x0;
    private float x1;
    private float x2;
    private float y0;
    private float y1;
    private float y2;
    private XYRect rect;
    private float k;
    private Material mat;
    public XYTriangle() { }
    public XYTriangle(float x0, float x1, float x2, float y0, float y1, float y2, float k, Material mat)
    {
        this.x0 = x0;
        this.x1 = x1;
        this.y0 = y0;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.k = k;
        this.mat = mat;
        float xMin = Mathf.Min(Mathf.Min(x0, x1), x2);
        float xMax = Mathf.Max(Mathf.Max(x0, x1), x2);
        float yMin = Mathf.Min(Mathf.Min(y0, y1), y2);
        float yMax = Mathf.Max(Mathf.Max(y0, y1), y2);
        rect = new XYRect(xMin, xMax, yMin, yMax, k, mat);
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(rect.X0, rect.Y0, k - 0.0001f), new Vector3D(rect.X1, rect.Y1, k + 0.0001f));
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

        Vector3D a = new Vector3D(x0, y0, k);
        Vector3D b = new Vector3D(x1, y1, k);
        Vector3D c = new Vector3D(x2, y2, k);
        Vector3D p = new Vector3D(x, y, k);

        //使用同向法判断击中点是否在三角形内
        if (!Mathf.SameSide(a, b, c, p) || !Mathf.SameSide(b, c, a, p) || !Mathf.SameSide(c, a, b, p))
            return false;

        rec.u = (x - rect.X0) / (rect.X1 - rect.X0);
        rec.v = (y - rect.Y0) / (rect.Y1 - rect.Y0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(0, 0, 1);
        return true;
    }
}

//XZ平面的三角形
public class XZTriangle : Hitable
{
    private float x0;
    private float x1;
    private float x2;
    private float z0;
    private float z1;
    private float z2;
    private XZRect rect;
    private float k;
    private Material mat;
    public XZTriangle() { }
    public XZTriangle(float x0, float x1, float x2, float z0, float z1, float z2, float k, Material mat)
    {
        this.x0 = x0;
        this.x1 = x1;
        this.z0 = z0;
        this.z1 = z1;
        this.x2 = x2;
        this.z2 = z2;
        this.k = k;
        this.mat = mat;
        float xMin = Mathf.Min(Mathf.Min(x0, x1), x2);
        float xMax = Mathf.Max(Mathf.Max(x0, x1), x2);
        float zMin = Mathf.Min(Mathf.Min(z0, z1), z2);
        float zMax = Mathf.Max(Mathf.Max(z0, z1), z2);
        rect = new XZRect(xMin, xMax, zMin, zMax, k, mat);
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(rect.X0, k - 0.0001f, rect.Z0), new Vector3D(rect.X1, k + 0.0001f, rect.Z1));
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

        Vector3D a = new Vector3D(x0, k, z0);
        Vector3D b = new Vector3D(x1, k, z1);
        Vector3D c = new Vector3D(x2, k, z2);
        Vector3D p = new Vector3D(x, k, z);

        //使用同向法判断击中点是否在三角形内
        //if (!Mathf.Synthetic(a,b,c,p))
         //   return false;

        //使用重心法判断击中点是否在三角形内
       if (!Mathf.GravityCenter(a, b, c, p))
            return false;

        rec.u = (x - rect.X0) / (rect.X1 - rect.X0);
        rec.v = (z - rect.Z0) / (rect.Z1 - rect.Z0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(0,1,0);
        return true;
    }
}

//YZ平面的三角形
public class YZTriangle : Hitable
{
    private float y0;
    private float y1;
    private float y2;
    private float z0;
    private float z1;
    private float z2;
    private YZRect rect;
    private float k;
    private Material mat;
    public YZTriangle() { }
    public YZTriangle(float y0, float y1, float y2, float z0, float z1,float z2, float k, Material mat)
    {
        this.y0 = y0;
        this.y1 = y1;
        this.z0 = z0;
        this.z1 = z1;
        this.y2 = y2;
        this.z2 = z2;
        this.k = k;
        this.mat = mat;
        float yMin = Mathf.Min(Mathf.Min(y0, y1), y2);
        float yMax = Mathf.Max(Mathf.Max(y0, y1), y2);
        float zMin = Mathf.Min(Mathf.Min(z0, z1), z2);
        float zMax = Mathf.Max(Mathf.Max(z0, z1), z2);
        rect = new YZRect(yMin, yMax, zMin, zMax, k, mat);
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(k - 0.0001f, rect.Y0, rect.Z0), new Vector3D(k + 0.0001f, rect.Y1, rect.Z1));
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

        Vector3D a = new Vector3D(k, y0, z0);
        Vector3D b = new Vector3D(k, y1, z1);
        Vector3D c = new Vector3D(k, y1, z2);
        Vector3D p = new Vector3D(k, y, z);

        //使用同向法判断击中点是否在三角形内
        if (!Mathf.SameSide(a, b, c, p) || !Mathf.SameSide(b, c, a, p) || !Mathf.SameSide(c, a, b, p))
            return false;

        rec.u = (y - rect.Y0) / (rect.Y1 - rect.Y0);
        rec.v = (z - rect.Z0) / (rect.Z1 - rect.Z0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(1,0, 0);
        return true;
    }
}
