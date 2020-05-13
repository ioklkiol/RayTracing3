using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Material
{      
    private float _refIdx;        //第一介质折射率ni和第二介质nt的比值，可能是ni/nt也可能是nt/ni
    public float RefIdx { get => _refIdx; set => _refIdx = value; }
    protected Material() { }
    protected Material(float ri) 
    {
        RefIdx = ri;
    }
    public abstract bool Scatter(Ray rIn, HitRecord hRec,out ScatterRecord sRec);
    public virtual float ScatteringPDF(Ray rIn, HitRecord rec, Ray scattered) { return 0; }
    public virtual Vector3D Emitted(Ray rIn, HitRecord rec, float u, float v, Vector3D p)
    {
        return new Vector3D(0, 0, 0);
    }
    //得到反射光线
    public Vector3D Reflect(Vector3D v, Vector3D n)
    {
        return v - 2 * v * n * n;
    }
    //引入反射系数
    public  float Schlick(float cosine, float ref_idx)
    {
        float r0 = (1 - RefIdx) / (1 + RefIdx);
        r0 = r0 * r0;
        return r0 + (1 - r0) * Mathf.Pow((1 - cosine), 5);
    }
    public bool Refract(Vector3D v, Vector3D n, float ni_over_nt,out Vector3D refracted)
    {
        n.Normalize();                          //得到单位向量
        v.Normalize(); 
                                                //设入射角为a，折射角为b，
        float dt = v * n;                      //得到cos(PI-a)
        float discriminant = 1 - ni_over_nt * ni_over_nt*(1 - dt * dt);    //得到cos(b)，同时也是判别式
        if (discriminant > 0)                                               //如果判别式大于0则说明折射角小于90度，即没有发生全反射
        {
            refracted = ni_over_nt * (v -n*dt) - n * Mathf.Sqrt(discriminant);
            return true;
        }
        else
        {
            refracted = new Vector3D();
            return false;
        }
    }

   
}
//Lambert材质
public class Lambertian : Material
{
    private Texture _albedo;

    public Texture Albedo { get => _albedo; set => _albedo = value; }

    public Lambertian() { }
    public Lambertian(Texture albedo) 
    {
        Albedo = albedo;
    }


    public override bool Scatter(Ray rIn, HitRecord hRec,out ScatterRecord sRec)
    {
        sRec = new ScatterRecord();
        sRec.isSpecular = false;
        sRec.attenuation = Albedo.Value(hRec.u, hRec.v, hRec.p);
        sRec.pdfPtr = new CosinePDF(hRec.normal);
        return true;
    }

    public override float ScatteringPDF(Ray rIn, HitRecord rec,Ray scattered)
    {
        float cosine = rec.normal * scattered.Direction.UnitVector();
        if (cosine < 0)
            cosine = 0;
        return cosine / Mathf.PI;
    }
}
//金属材质
public class Metal : Material
{
    private float _fuzz;      //镜面模糊系数
    private Vector3D albedo;
    public float Fuzz { get => _fuzz; set => _fuzz = value; }
  

    public Metal(Vector3D albedo, float f) 
    {
        this.albedo = albedo;
        Fuzz = Mathf.Min(f, 1);
    }


    public override bool Scatter(Ray rIn, HitRecord hRec, out ScatterRecord sRec)
    {
        sRec = new ScatterRecord();
        Vector3D reflected = Reflect(rIn.Direction.UnitVector(), hRec.normal);
        sRec.specularRay = new Ray(hRec.p, reflected + Fuzz * Mathf.RandomInUnitShpere(), rIn.Time);     //模糊镜面反射 = 镜面反射 + 模糊系数 * 单位球随机点漫反射
        sRec.attenuation = albedo;
        sRec.isSpecular = true;
        sRec.pdfPtr = null;
        return true;


    }
}
//电介质材质(折射)
public class Dielectric : Material
{


    public Dielectric(float ri) : base(ri)
    {

    }


    public override bool Scatter(Ray rIn, HitRecord hRec, out ScatterRecord sRec)
    {
        sRec = new ScatterRecord();
        Vector3D outwardNormal;
        Vector3D reflected = Reflect(rIn.Direction, hRec.normal);
        float ni_over_nt;      //第一介质与第二介质的折射率的比值ni/nt
        sRec.attenuation = new Vector3D(1, 1, 1);
        Vector3D refracted;

        float reflect_prob;
        float cosine;

        if (rIn.Direction * hRec.normal > 0)    //如果大于0则说明法向量反了，需要翻转法向量
        {
            outwardNormal = -hRec.normal;
            ni_over_nt = RefIdx;
            cosine = RefIdx * (rIn.Direction * hRec.normal) / rIn.Direction.Length();
        }
        else
        {
            outwardNormal = hRec.normal;
            ni_over_nt = 1 / RefIdx;
            cosine = -(rIn.Direction * hRec.normal) / rIn.Direction.Length();
        }
        if (Refract(rIn.Direction, outwardNormal, ni_over_nt, out refracted))
        {
            reflect_prob = Schlick(cosine, RefIdx);
        }
        else
        {
            reflect_prob = 1;
        }
        if (Mathf.Randomfloat() < reflect_prob)
        {
            sRec.specularRay = new Ray(hRec.p, reflected, rIn.Time);
            sRec.isSpecular = true;
        }
        else
        {
            sRec.specularRay = new Ray(hRec.p, refracted, rIn.Time);
            sRec.isSpecular = true;
        }
        return true;
    }
}
//自发光材质
public class DiffuseLight : Material
{
    private Texture emit;
    public DiffuseLight(Texture albedo) 
    {
        emit = albedo;
    }

    public override bool Scatter(Ray rIn, HitRecord hRec, out ScatterRecord sRec)
    {
        sRec = new ScatterRecord();   
        sRec.attenuation = new Vector3D();
        return false;
    }
    public override Vector3D Emitted(Ray rIn,HitRecord rec, float u, float v, Vector3D p)
    {
        if (rec.normal * rIn.Direction < 0)         
            return emit.Value(u, v, p);
        else
            return new Vector3D(0, 0, 0);
    }
}
//各向异性材质
public class Isotropic : Material
{
    private Texture albedo;
    public Texture Albedo { get => albedo; set => albedo = value; }
    public Isotropic(Texture albedo) 
    {
        this.albedo = albedo;
    }


    public override bool Scatter(Ray rIn, HitRecord hRec, out ScatterRecord sRec)
    {
        sRec = new ScatterRecord();
        sRec.specularRay = new Ray(hRec.p, Mathf.RandomInUnitShpere(), rIn.Time);
        sRec.attenuation = Albedo.Value(hRec.u, hRec.v, hRec.p);
        sRec.isSpecular = true;
        return true;
    }
}