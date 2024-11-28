using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class StageShape : MonoBehaviour
{
    StageData stageData;
    public void Create(StageData stageData, int vertexNum, int posNum)
    {
        this.stageData = stageData;
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[3];
        int[] triangles = new int[3];

        vertices[0] = Vector3.zero;
        float angleStep = Mathf.PI * 2.0f / vertexNum;
        for (int i = 0; i < 2; i++)
        {
            vertices[i + 1] = new Vector3(10 * Mathf.Sin((posNum + i - 0.5f) * angleStep), 10 * Mathf.Cos((posNum + i - 0.5f) * angleStep), 0);
        }

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public StageData GetStageData()
    {
        return stageData;
    }

    // private void Awake()
    // {
    //     Mesh mesh = new Mesh();
    //     GetComponent<MeshFilter>().mesh = mesh;

    //     Vector3[] vertices = new Vector3[vertexNum + 1];
    //     int[] triangles = new int[vertexNum * 3];

    //     vertices[0] = Vector3.zero;
    //     float angleStep = Mathf.PI * 2.0f / vertexNum;
    //     for (int i = 0; i < 3; i++)
    //     {
    //         vertices[i + 1] = new Vector3(Mathf.Sin(i * angleStep), Mathf.Cos(i * angleStep), 0);
    //     }
    //     for (int i = 0; i < 1; i++)
    //     {
    //         triangles[i * 3] = 0;
    //         triangles[i * 3 + 1] = i + 1;
    //         triangles[i * 3 + 2] = i + 2 != vertexNum + 1 ? i + 2 : 1;
    //     }

    //     mesh.vertices = vertices;
    //     mesh.triangles = triangles;
    //     mesh.RecalculateNormals();
    // }
}
