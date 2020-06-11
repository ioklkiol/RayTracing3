using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcDx;

public class Preview : DxWindow
{
    public override void Update()
    {
        for (int i = 0; i < Buff.Length; i++)
            //Buff[i] = (byte)Mathf.Range(Mathf.Sqrt(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4]), 0, 255);
            Buff[i] = (byte)Mathf.Range(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4], 0, 255);
    }
}

public class Renderer
{
    private int width = 2000;
    private int height = 1000;
    private HitableList world = new HitableList();
    private Preview preview = new Preview();
    private bool isSky = true;


    public static Renderer main;
    public int samples = 1000;
    public float[] buff;
    public int[] changes;
    public Bitmap bmp;
    public Camera camera;
    public void Init()
    {
        main = this;
        buff = new float[width * height * 4];
        changes = new int[width * height];
        bmp = new Bitmap(width, height);
        InitScene();

        Start();

        preview.Run(new DxConfiguration("Preview", width, height));
        System.Environment.Exit(0);
    }
    private void InitScene()
    {
        CornellBox();
        //RandomScene();
    }
    private void CornellBox()
    {
        this.width = 512;
        this.height = 512;
        isSky = false;

        Vector3D lookFrom = new Vector3D(278, 278, -800);
        Vector3D lookAt = new Vector3D(278, 278, 0);
        float diskToFocus = 10;
        float aperture = 0;
        float vfov = 40;
        camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), vfov,
            (float)width / (float)height, aperture, diskToFocus, 0, 1);

        List<Hitable> list = new List<Hitable>();

        Material red = new Lambertian(new ConstantTexture(new Vector3D(0.65f, 0.05f, 0.05f)));
        Material white = new Lambertian(new ConstantTexture(new Vector3D(0.73f, 0.73f, 0.73f)));
        Material green = new Lambertian(new ConstantTexture(new Vector3D(0.2f, 0.45f, 0.15f)));
        Material light = new DiffuseLight(new ConstantTexture(new Vector3D(50, 50, 50)));
        Material aluminum = new Metal(new Vector3D(0.8f, 0.85f, 0.88f), 0);
        Material glass = new Dielectric(1.5f);

        //list.Add(new FlipNormals(new XZRect(213, 343, 227, 332, 554, light)));
        list.Add(new FlipNormals(new XZTriangle(213, 343,278,227,227, 332, 554, light)));
        list.Add(new FlipNormals(new YZRect(0, 555, 0, 555, 555, green)));
        list.Add(new YZRect(0, 555, 0, 555, 0, red));
        //list.Add(new YZTriangle(0, 555, 555, 0, 0, 555, 0, red));
        list.Add(new FlipNormals(new XZRect(0, 555, 0, 555, 555, white)));
        list.Add(new XZRect(0, 555, 0, 555, 0, white));
        list.Add(new FlipNormals(new XYRect(0, 555, 0, 555, 555, white)));

        //list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
        //  new Vector3D(165, 165, 165), white), -18), new Vector3D(130, 0, 65)));
        list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
            new Vector3D(165, 330, 165), white), 15), new Vector3D(265, 0, 295)));

        // list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0), 
        //    new Vector3D(165, 330, 165), aluminum), 15), new Vector3D(265, 0, 295)));
        list.Add(new Sphere(new Vector3D(190, 90, 190), 90, glass));
        BVHNode b = new BVHNode(list, list.Count, 0, 1);
        world.list.Add(b);
    }
    private void RandomScene()
    {
        this.width = 2000;
        this.height = 1000;
        isSky = true;

        Vector3D lookFrom = new Vector3D(13, 2, 3);
        Vector3D lookAt = new Vector3D(0, 0, 0);
        float diskToFocus = (lookFrom - lookAt).Length();
        float aperture = 0;
        camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), 20,
            (float)width / (float)height, aperture, 0.7f * diskToFocus, 0, 1);

        List<Hitable> list = new List<Hitable>();
        list.Add(new Sphere(new Vector3D(0, -1000, 0), 1000, new Lambertian(new ConstantTexture(new Vector3D(0.5f, 0.5f, 0.5f)))));
        for (int a = -11; a < 11; a++)
        {
            for (int b = -11; b < 11; b++)
            {
                double chooseMat = Mathf.Randomfloat();
                Vector3D center = new Vector3D(a + 0.9f * Mathf.Randomfloat(), 0.2f, b + 0.9f * Mathf.Randomfloat());
                if ((center - new Vector3D(4, 0.2f, 0)).Length() > 0.9)
                {
                    if (chooseMat < 0.8)
                    {
                        list.Add(new Sphere(center, 0.2f, new Lambertian(new ConstantTexture(new Vector3D(
                                                        Mathf.Randomfloat() * Mathf.Randomfloat(),
                                                        Mathf.Randomfloat() * Mathf.Randomfloat(),
                                                        Mathf.Randomfloat() * Mathf.Randomfloat())))));
                    }
                    else if (chooseMat < 0.95)
                    {
                        list.Add(new Sphere(center, 0.2f, new Metal(new Vector3D(
                                                            0.5f * (1 + Mathf.Randomfloat()),
                                                            0.5f * (1 + Mathf.Randomfloat()),
                                                            0.5f * (1 + Mathf.Randomfloat())),
                                                            0.5f * (1 + Mathf.Randomfloat()))));
                    }
                    else
                    {
                        list.Add(new Sphere(center, 0.2f, new Dielectric(1.5f)));
                    }
                }
            }
        }
        list.Add(new Sphere(new Vector3D(0, 1, 0), 1, new Dielectric(1.5f)));
        list.Add(new Sphere(new Vector3D(-4, 1, 0), 1, new Lambertian(new ConstantTexture(new Vector3D(0.4f, 0.2f, 0.1f)))));
        list.Add(new Sphere(new Vector3D(4, 1, 0), 1, new Metal(new Vector3D(0.7f, 0.6f, 0.5f), 0)));
        BVHNode bb = new BVHNode(list, list.Count, 0, 1);
        world.list.Add(bb);
    }
    private class ScannerCofig
    {
        public int width;
        public int height;

        public ScannerCofig(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
    private async void Start()
    {
        ThreadPool.SetMaxThreads(15, 15);
        await Task.Factory.StartNew(
            delegate { LinearScanner(new ScannerCofig(width, height)); });
        for (int i = 1; i < samples; i++)
        {
            ThreadPool.QueueUserWorkItem(LinearScanner, new ScannerCofig(width, height));
        }
    }
    //测试用
    private void Start2()
    {
        for (int i = 1; i < samples; i++)
        {
            LinearScanner(new ScannerCofig(width, height));
        }
    }

    private Vector3D DeNaN(Vector3D c)
    {
        Vector3D temp = c;
        if (!(temp[0] == temp[0])) temp[0] = 0;
        if (!(temp[1] == temp[1])) temp[1] = 0;
        if (!(temp[2] == temp[2])) temp[2] = 0;
        return temp;
    }
    private void LinearScanner(object o)
    {
        ScannerCofig config = (ScannerCofig)o;
        Hitable lightShape = new XZRect(213, 343, 227, 332, 554, null);
        Hitable glassSphere = new Sphere(new Vector3D(190, 90, 190), 90, null);
        for (int j = 0; j < config.height; j++)
        {
            for (int i = 0; i < config.width; i++)
            {
                Vector3D color = new Vector3D(0, 0, 0);
                float u = (float)(i + Mathf.Randomfloat()) / (float)width;
                float v = 1 - (float)(j + Mathf.Randomfloat()) / (float)height;
                Ray ray = camera.GetRay(u, v);
                List<Hitable> a = new List<Hitable>();
                a.Add(lightShape);
                a.Add(glassSphere);
                HitableList hitableList = new HitableList(a);
                color = DeNaN(GetColor(ray, world, hitableList, 0));
                color = new Vector3D(Mathf.Sqrt(color.X), Mathf.Sqrt(color.Y), Mathf.Sqrt(color.Z));                                                                                        
                SetPixel(i, j, color);
            }
        }
    }


    private Vector3D GetColor(Ray r, HitableList world, Hitable lightShape, int depth)
    {
        HitRecord hRec;
        /*这里的0.001不能改为0，当tmin设0的时候会导致，遍历hitlist时候，ray的t求解出来是0，
         * hit的时候全走了else，导致递归到50层的时候，最后return的是0，* attenuation结果还是0。
         * 距离越远，散射用到random_in_unit_sphere生成的ray误差越大
         */
        if (world.Hit(r, 0.001f, float.MaxValue, out hRec))
        {
            ScatterRecord sRec;
            Vector3D emitted = hRec.matPtr.Emitted(r, hRec, hRec.u, hRec.v, hRec.p);
            if (depth < 50 && hRec.matPtr.Scatter(r, hRec, out sRec))
            {
                if (sRec.isSpecular)
                {
                    Vector3D c = GetColor(sRec.specularRay, world, lightShape, depth + 1);
                    return new Vector3D(sRec.attenuation.X * c.X, sRec.attenuation.Y * c.Y, sRec.attenuation.Z * c.Z);
                }
                HitablePDF p0 = new HitablePDF(lightShape, hRec.p);
                MixturePDF p = new MixturePDF(p0, sRec.pdfPtr);
                //CosinePDF p = new CosinePDF(hRec.normal);
                Ray scattered = new Ray(hRec.p, p.Generate(), r.Time);
                float pdfVal = p.Value(scattered.Direction);
                Vector3D color = GetColor(scattered, world, lightShape, depth + 1);      //每次光线衰减之后深度加一
                return emitted + hRec.matPtr.ScatteringPDF(r, hRec, scattered)
                   * new Vector3D(sRec.attenuation.X * color.X, sRec.attenuation.Y
                   * color.Y, sRec.attenuation.Z * color.Z) / pdfVal;
            }
            else
            {
                return emitted;
            }
        }
        else
        {
            if (isSky)
            {
                Vector3D unitDirection = r.Direction.UnitVector();
                float t = 0.5f * (unitDirection.Y + 1f);
                return (1 - t) * new Vector3D(1, 1, 1) + t * new Vector3D(0.5f, 0.7f, 1);

            }
            return new Vector3D(0, 0, 0);
        }
    }

    private void SetPixel(int x, int y, Vector3D color)
    {
        var i = width * 4 * y + x * 4;
        changes[width * y + x]++;
        buff[i] += color.X;
        buff[i + 1] += color.Y;
        buff[i + 2] += color.Z;
        buff[i + 3] += 1;
    }
}