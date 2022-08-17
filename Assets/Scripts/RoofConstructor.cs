using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoofConstructor
{
    public enum RoofType
    {
        Flat,
        Pyramid,
        Tent
    }

    public static void ConstructRoof(GameObject root, GameObject[] walls, Vector2 extent, RoofType roofType, List<Material> roofMaterials)
    {
        // Get edges
        Vector3[] edgePoints = new Vector3[5];
        edgePoints[0] = new Vector3(0, extent.y, 0);
        edgePoints[1] = new Vector3(extent.x, extent.y, 0);
        edgePoints[2] = new Vector3(extent.x, extent.y, extent.x);
        edgePoints[3] = new Vector3(0, extent.y, extent.x);
        // middle point (for pyramid roof)
        edgePoints[4] = new Vector3(edgePoints[2].x / 2, extent.y * 1.5f, edgePoints[2].z / 2);

        switch (roofType)
        {
            case RoofType.Flat:
                CreateFlatRoof(extent, walls, edgePoints, roofMaterials);
                break;
            case RoofType.Pyramid:
                CreatePyramidRoof(extent, walls, edgePoints, roofMaterials);
                break;
            case RoofType.Tent:
                CreateTentRoof(extent, walls, edgePoints, roofMaterials);
                break;
            default:
                CreatePyramidRoof(extent, walls, edgePoints, roofMaterials);
                break;
        }

    }

    static void CreatePyramidRoof(Vector2 extent, GameObject[] walls, Vector3[] edgePoints, List<Material> roofMaterials)
    {
        CreateTriRoofMesh(extent.x, extent.y, walls[0].transform.position.x, walls[0].transform.position.z, edgePoints[4].z, 0, roofMaterials);
        CreateTriRoofMesh(extent.x, extent.y, walls[1].transform.position.x, walls[1].transform.position.z, edgePoints[4].z, -90, roofMaterials);
        CreateTriRoofMesh(extent.x, extent.y, walls[2].transform.position.x, walls[2].transform.position.z, edgePoints[4].z, -180, roofMaterials);
        CreateTriRoofMesh(extent.x, extent.y, walls[3].transform.position.x, walls[3].transform.position.z, edgePoints[4].z, -270, roofMaterials);   
    }
    
    static void CreateFlatRoof(Vector2 extent, GameObject[] walls, Vector3[] edgePoints, List<Material> roofMaterials)
    {
        CreateQuadRoofMesh(extent.x, extent.y, walls[0].transform.position.x, walls[1].transform.position.z, extent.y, new Vector3(90, 0, 0), roofMaterials);
    }

    static void CreateTentRoof(Vector2 extent, GameObject[] walls, Vector3[] edgePoints, List<Material> roofMaterials)
    {
        CreateTriRoofMesh(extent.x, extent.y, walls[0].transform.position.x, walls[0].transform.position.z, 0, 0, roofMaterials);
        CreateQuadRoofMesh(extent.x, extent.x * 0.75f, walls[1].transform.position.x * 0.75f, walls[1].transform.position.z, extent.y * 1.25f ,new Vector3(45,-90,0), roofMaterials);
        CreateTriRoofMesh(extent.x, extent.y, walls[2].transform.position.x, walls[2].transform.position.z, 0, -180, roofMaterials);
        CreateQuadRoofMesh(extent.x, extent.x * 0.75f, walls[1].transform.position.x * 0.25f, walls[3].transform.position.z, extent.y * 1.25f, new Vector3(45,-270,0), roofMaterials);
    }
    
    static void CreateTriRoofMesh(float width, float height, float centerX, float centerZ, float z, float rot, List<Material> roofMaterials)
    {
        Mesh mesh = new Mesh();
        // Just create a triangle
        Vector3[] vertices = new Vector3[3]
        {
            new Vector3(-width/2, 0, 0),
            new Vector3(width/2, 0, 0),
            new Vector3(0, height / 2, z)
        };
        mesh.vertices = vertices;
        int[] tri = new int[3]
        {
            // clockwise
            0, 2, 1
        };
        mesh.triangles = tri;
        // Normals
        Vector3[] normals = new Vector3[3]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;
        Vector2[] uv = new Vector2[3]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0.5f, 1)
        };
        mesh.uv = uv;
        
        GameObject newShape;
        newShape = new GameObject();
        newShape.transform.position = new Vector3(centerX, height, centerZ);
        newShape.transform.Rotate(new Vector3(0,rot,0), Space.World);
        newShape.AddComponent<Shape>();
        newShape.AddComponent<MeshFilter>();
        newShape.AddComponent<MeshRenderer>();
        newShape.GetComponent<MeshFilter>().sharedMesh = mesh;
        newShape.GetComponent<MeshRenderer>().material = roofMaterials[0];
        
    }
    
    static void CreateQuadRoofMesh(float width, float height, float centerX, float centerZ, float centerY, Vector3 rot, List<Material> roofMaterials)
    {
        Mesh mesh = new Mesh();
        // Just create a quad
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-width / 2, -height / 2, 0),
            new Vector3(width / 2, -height / 2, 0),
            new Vector3(-width / 2, height / 2, 0),
            new Vector3(width / 2, height / 2, 0)
        };
        mesh.vertices = vertices;
        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;
        // Normals
        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;
        
        // The shape created by AddShape should end up underneath the parent if parent is Construction type
        GameObject newShape = new GameObject();
        newShape.transform.position = new Vector3(centerX, centerY, centerZ);
        newShape.transform.Rotate(rot, Space.World);

        newShape.AddComponent<MeshFilter>();
        newShape.AddComponent<MeshRenderer>();
        newShape.GetComponent<MeshFilter>().sharedMesh = mesh;
        newShape.GetComponent<MeshRenderer>().material = roofMaterials[0];
        // Shape extent setup}
        
    }
}