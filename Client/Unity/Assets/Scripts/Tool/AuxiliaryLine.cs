using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// 辅助线
/// </summary>
public class AuxiliaryLine : MonoBehaviour
{
    public bool isGlobal;
    public bool useDistance;
    public float distance = 10f;

    Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };

    Matrix4x4[] matrices = new Matrix4x4[] {
            Matrix4x4.TRS(Vector3.right, Quaternion.AngleAxis(90f, Vector3.back), Vector3.one),
            Matrix4x4.TRS(Vector3.up, Quaternion.identity, Vector3.one),
            Matrix4x4.TRS(Vector3.forward, Quaternion.AngleAxis(90f, Vector3.right), Vector3.one)
        };

    protected const string shaderFilePath = "Hidden/Internal-Colored";
    public float threshold = 20f;//阈
    public float handler_Size = 0.2f;

    private Mesh cone;

    private Material _material;
    protected Material material
    {
        get
        {
            if (_material == null)
            {
                var shader = Shader.Find(shaderFilePath);
                if (shader == null)
                    Debug.LogErrorFormat("Shader not found : {0}", shaderFilePath);
                _material = new Material(shader);
            }
            return _material;
        }
    }


    void Awake()
    {
        cone = CreateCone(5, 0.1f, handler_Size);
    }

    void OnRenderObject()
    {
        GL.PushMatrix();

        GL.MultMatrix(GetTranform());
        DrawTranslate();

        GL.PopMatrix();
    }

    void DrawTranslate()
    {
        material.SetInt("_ZTest", (int)CompareFunction.Always);
        material.SetPass(0);

        var color = colors[2];
        DrawLine(Vector3.zero, Vector3.forward, color);
        DrawMesh(cone, matrices[2], color);
    }

    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(start);
        GL.Vertex(end);
        GL.End();
    }

    void DrawMesh(Mesh mesh, Matrix4x4 m, Color color)
    {
        GL.Begin(GL.TRIANGLES);
        GL.Color(color);

        var vertices = mesh.vertices;
        for (int i = 0, n = vertices.Length; i < n; i++)
        {
            vertices[i] = m.MultiplyPoint(vertices[i]);
        }

        var triangles = mesh.triangles;
        for (int i = 0, n = triangles.Length; i < n; i += 3)
        {
            int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];
            GL.Vertex(vertices[a]);
            GL.Vertex(vertices[b]);
            GL.Vertex(vertices[c]);
        }
        GL.End();
    }

    Mesh CreateCone(int subdivisions, float radius, float height)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[subdivisions + 2];
        int[] triangles = new int[(subdivisions * 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
        {
            float ratio = (float)i / n;
            float r = ratio * (Mathf.PI * 2f);
            float x = Mathf.Cos(r) * radius;
            float z = Mathf.Sin(r) * radius;
            vertices[i + 1] = new Vector3(x, 0f, z);
        }
        vertices[subdivisions + 1] = new Vector3(0f, height, 0f);

        // construct bottom
        for (int i = 0, n = subdivisions - 1; i < n; i++)
        {
            int offset = i * 3;
            triangles[offset] = 0;
            triangles[offset + 1] = i + 1;
            triangles[offset + 2] = i + 2;
        }

        // construct sides
        int bottomOffset = subdivisions * 3;
        for (int i = 0, n = subdivisions - 1; i < n; i++)
        {
            int offset = i * 3 + bottomOffset;
            triangles[offset] = i + 1;
            triangles[offset + 1] = subdivisions + 1;
            triangles[offset + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }
    Matrix4x4 GetTranform()
    {
        float scale = 1f;
        if (useDistance)
        {
            var d = (Camera.main.transform.position - transform.position).magnitude;
            scale = d / distance;
        }
        return Matrix4x4.TRS(transform.position, isGlobal ? Quaternion.identity : transform.rotation, Vector3.one * scale);
    }
}
