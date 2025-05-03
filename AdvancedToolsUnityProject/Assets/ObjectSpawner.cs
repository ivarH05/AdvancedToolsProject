using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefab;
    public Mesh mesh;
    public Material material;
    public static int spawnCount = 0;

    private Matrix4x4[] GPUInstances;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] positions = new Vector3[spawnCount];
        int dims = Mathf.CeilToInt(Mathf.Pow(spawnCount, 1f / 3f));
        int count = 0;

        Vector3 offset = new Vector3(dims, dims, dims) * -0.5f;

        for (int z = 0; z < dims; z++)
        {
            for (int y = 0; y < dims; y++)
            {
                for (int x = 0; x < dims; x++)
                {
                    if (count == spawnCount)
                        goto loopEnd;

                    positions[count] = (new Vector3(x, y, z) + offset) * 0.2f;
                    count++;
                }
            }
        }
        loopEnd:

        //SetupIndividualObjects(positions);
        //SetupGPUInstancing(positions);
        SetupCombinedMesh(positions);
    }

    private void Update()
    {
        //HandleGPUInstancing();
    }

    void SetupCombinedMesh(Vector3[] positions)
    {
        Vector3[] verts = new Vector3[spawnCount * mesh.vertexCount];
        List<int> tris = new List<int>(spawnCount * mesh.triangles.Length);
        int counter = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            for (int j = 0; j < mesh.vertexCount; j++)
            {
                verts[counter] = mesh.vertices[j] + positions[i];
                counter++;
            }

            for (int k = 0; k < mesh.triangles.Length; k++)
            {
                tris.Add(mesh.triangles[k] + i * mesh.vertices.Length);
            }
        }

        Mesh result = new Mesh();
        result.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        result.vertices = verts;
        result.triangles = tris.ToArray();
        result.RecalculateNormals();

        var filter = gameObject.AddComponent<MeshFilter>();
        var renderer = gameObject.AddComponent<MeshRenderer>();

        filter.sharedMesh = result;
        renderer.material = material;
    }

    void SetupGPUInstancing(Vector3[] positions)
    {
        GPUInstances = new Matrix4x4[spawnCount];
        for (int i = 0; i < positions.Length; i++)
            GPUInstances[i] = Matrix4x4.TRS(positions[i], Quaternion.identity, Vector3.one);
    }

    void SetupIndividualObjects(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
            SummonObject(positions[i]);
    }

    void HandleGPUInstancing()
    {
        Graphics.DrawMeshInstanced(mesh, 0, material, GPUInstances);
    }

    void SummonObject(Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }
}
