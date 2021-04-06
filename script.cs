using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class script : MonoBehaviour
{
   Shader shad1;
   Shader shad2;
   Shader shad3;
   Shader shad0;
   GameObject obj;
   GameObject obj2;
   GameObject obj3;
   GameObject obj4;
    // Start is called before the first frame update
    void Start()
    {
        shad1 = Shader.Find("Custom/Flat");
        shad2 = Shader.Find("Custom/Gouraud");
        shad3 = Shader.Find("Custom/Phong");
        obj = VerticesPlaneta(new Vector3(-2.5f,3,0), 2, 8, 16);
        shad0 = obj.GetComponent<Renderer>().material.shader;
        //obj.GetComponent<Renderer>().material.color = RGBToPercent(new Vector3(255,0,0));
        obj2 = VerticesPlaneta(new Vector3(2.5f,3,0), 2, 8, 16);
        obj2.GetComponent<Renderer>().material.shader = shad1;
        obj3 = VerticesPlaneta(new Vector3(7.5f,3,0), 2, 8, 16);
        obj3.GetComponent<Renderer>().material.shader = shad2;
        obj4 = VerticesPlaneta(new Vector3(12.5f,3,0), 2, 8, 16);
        obj4.GetComponent<Renderer>().material.shader = shad3;
    }
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.N)){
            obj.GetComponent<Renderer>().material.shader = shad0;
        }else if(Input.GetKey(KeyCode.F)){
            obj.GetComponent<Renderer>().material.shader = shad1;
        }else if(Input.GetKey(KeyCode.G)){
            obj.GetComponent<Renderer>().material.shader = shad2;
        }else if(Input.GetKey(KeyCode.P)){
            obj.GetComponent<Renderer>().material.shader = shad3;
        }
    }
     /*
     * Dibuja el mesh del objeto
     */
    GameObject dibujarMesh(Vector3[] vert, int[] tri) {
        GameObject obj = new GameObject("obj", typeof(MeshFilter), typeof(MeshRenderer));
        Mesh mesh = new Mesh();
        obj.GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        mesh.vertices = vert;
        mesh.triangles = tri;
        mesh.RecalculateNormals();
        return obj;
    }
    GameObject VerticesPlaneta(Vector3 origen, float r, int np, int nt){
        Vector3[] vert = new Vector3[2+nt*(np-1)];
        vert[0] = SphericalToCartesian(new Vector3(r,0,0), origen);
        vert[1] = SphericalToCartesian(new Vector3(r,0,(float)System.Math.PI), origen);
        float dp = (float)((System.Math.PI)/np);
        float dt = (float)((2*System.Math.PI)/nt);
        int c = 2;
        for(int i = 0;i<np-1;i++){
            float phi = (i+1)*dp;
            for(int j = 0;j<nt;j++){
                float theta = j*dt;
                vert[c] = SphericalToCartesian(new Vector3(r,theta,phi), origen);
                c++;
            }
        } 
       return triPlaneta(vert,np, nt);
    }
    GameObject triPlaneta(Vector3[] vert, int np, int nt) {
        int[] tri = new int[6*nt*(np-1)];

        //Triangulos de la parte de arriba de la esfera
        int j = nt+1;
        for(int i=0;i<3*nt;i++){
            if(i%3==0){
                tri[i] = 0; 
            }else if(i%3==1){
                tri[i] = j;
                j--;
            }else{
                if(j==1){
                    j=nt+1;
                }
                tri[i] = j;
            }
        }
        int t = 3*nt;
        //Triangulos centrales
        for(int p = 0;p<(np-2);p++){
            //Base Abajo
            int inf = (p+1)*nt+2;
            int sup = p*nt+2;
            for(int i= 0; i<3*nt ;i++){
                if(t%3==0){
                    tri[t] = inf;
                    inf++;
                    t++;
                }else if(t%3==1){
                    tri[t] = sup;
                    sup++;
                    t++;
                }else{
                    if(inf==(2+nt*(p+2))){
                        inf = (p+1)*nt+2;
                    }
                    tri[t] = inf;
                    t++;
                }
            }
            sup = (p+1)*nt+1;
            inf = (p+2)*nt+1;
            for(int i = 0; i<3*nt;i++){
                if(t%3==0){
                    tri[t] = sup;
                    t++;
                    sup--;
                }else if(t%3==1){
                    tri[t] = inf;
                    inf--;
                    t++;
                }else{
                    if(sup == nt*p+1){
                        sup = (p+1)*nt+1;
                    }
                    tri[t] = sup;
                    t++;
                }
            }
            //Base Arriba
        }

        j = 2+nt*(np-2);
        for(int i = 3*nt*(2*np-3);i<(6*nt*(np-1));i++){
            if(i%3==0){
                tri[i] = 1;
            }else if(i%3==1){
                tri[i] = j;
                j++;
            }else{
                if(j==(2+nt*(np-1))){
                    j = 2+nt*(np-2);
                }
                tri[i] = j;
            }
        }
        return dibujarMesh(vert, tri);
    }
    /*
     *Convierte el Vector3 de coordenadas esfericas en un Vector3 de coordenadas cartesianas. esferica = (r, t, p).
     */
    Vector3 SphericalToCartesian(Vector3 sph, Vector3 orgn){
        float r1 =(float)((sph.x)*System.Math.Cos((System.Math.PI/2)-sph.z));
        float x = (float)((r1)*System.Math.Cos(sph.y));
        float y = (float)((sph.x)*System.Math.Cos(sph.z));
        float z = (float)((r1)*System.Math.Sin(sph.y));
        x += orgn.x;
        y += orgn.y;
        z += orgn.z;
        return new Vector3(x,y,z);
    }
    Color RGBToPercent(Vector3 colores){
        Vector3 p = new Vector3((float)(colores.x/255),(float)(colores.y/255),(float)(colores.z/255));
        return new Color(p.x,p.y,p.z);
    }
    /*
     * Convierte el vector 3 de coordenadas polares en un Vector3 de coordenadas cartesianas y mueve las coordenadas al nuevo origen
     */
    Vector3 PolarToCartesian(Vector3 polar, Vector3 origen){
        float x = (float)((polar.x)*(System.Math.Cos(polar.y)));
        float z = (float)((polar.x)*(System.Math.Sin(polar.y)));
        float y = (float)(polar.z);
        return new Vector3(x +origen.x,y + origen.y,z + origen.z);
    }
}
