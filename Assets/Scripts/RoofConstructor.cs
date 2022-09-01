using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public static class RoofConstructor
{
    public enum RoofType
    {
        Flat,
        Pyramid,
        Tent,
        Fancy
    }

    public static void ConstructRoof(GameObject root, GameObject[] walls, Vector2 extent, RoofType roofType, List<Material> roofMaterials)
    {
        // First, remove old roof objects as they are no longer needed
        var roofs = GameObject.FindGameObjectsWithTag("Roof");
        foreach (var roof in roofs)
        {
            Object.Destroy(roof);
        }
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
            case RoofType.Fancy:
                CreateFancyRoof(extent, walls, edgePoints, roofMaterials);
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
    
    /// <summary>
    /// This method creates a more complicated "fancy" roof, designed to work with any custom designed base.
    /// Uses math to find angles between walls, then constructs polygons out of the created "inset points", which - should then be triangulated to allow mesh construction between them. 
    /// </summary>
    /// <param name="extent">The wall's shape extent in 2d</param>
    /// <param name="walls">The array of wall gameObjects to create a roof between</param>
    /// <param name="edgePoints">The array of points that make up the edges (likely not necessary...)</param>
    /// <param name="roofMaterials">A list of materials to use for the roof<param>
    static void CreateFancyRoof(Vector2 extent, GameObject[] walls, Vector3[] edgePoints, List<Material> roofMaterials)
    {
        float yValIncreasePercent = 100f; // as a percent of the wall Y value
        float extendInwards = 2.5f;
        List<Vector3> newPoints = new List<Vector3>();
        List<Vector3> sideQuadPoints = new List<Vector3>();
        // Assuming walls are in order, we can always take the "left corner", compare against angle of next wall (facing inwards)
        for (int i = 0; i < walls.Length; i++)
        {
            // Get the "top left corner"
            var wall = walls[i];
            var wallMesh = wall.GetComponent<MeshRenderer>(); 
            Mesh mesh = wall.GetComponent<MeshFilter>().mesh;
            Matrix4x4 localToWorld = wall.transform.localToWorldMatrix;
            
            // Assuming every plane has 4 vertices, going from bottom right, bottom left, top left - we want third
            Vector3 world_v = localToWorld.MultiplyPoint3x4(mesh.vertices[2]);    // get the real time world pos
            var angleBetweenWall = Vector3.SignedAngle(wall.transform.right, walls[(i+1) % walls.Length].transform.right, wall.transform.up);
            angleBetweenWall = Mathf.Abs(180 - angleBetweenWall) / 2f;  // if you have corner, in between corner, outward facing corner, between it also :)
            var outwardFromCorner = Quaternion.AngleAxis(-angleBetweenWall, Vector3.up) * (wall.transform.right) * extendInwards;
            Vector3 resultPoint = world_v + outwardFromCorner +
                                  wall.transform.up * mesh.bounds.extents.y * 2 * (yValIncreasePercent)/100f;
            Debug.DrawLine(world_v, resultPoint, Color.green, 100);
            
            // Add points to lists
            newPoints.Add(resultPoint);
            sideQuadPoints.Add(world_v);
            sideQuadPoints.Add(resultPoint);
        }
        
        // At this point, all vertices in 3d space are achieved. Start constructing roof. 
        for (int i = 0; i < walls.Length; i++)
        {
            var ln = sideQuadPoints.Count;
            Debug.Log("tryna makin a roof'n stuff");
            CreateQuadFromPoints(sideQuadPoints[i*2 % ln], sideQuadPoints[(i*2+1) % ln], sideQuadPoints[( i*2+2) % ln], sideQuadPoints[ (i*2+3) % ln], roofMaterials);
        }
        // Triangulate the polygon making up the upper part of the roof
        Triangulator triangulator = new Triangulator(newPoints.ToArray());
        var resultIndices = triangulator.Triangulate();
        for (int i = 0; i < resultIndices.Length-2; i += 3)
        {
            CreateTriFromPoints(newPoints[resultIndices[i]], newPoints[resultIndices[i+1]], newPoints[resultIndices[i+2]], roofMaterials);
            
        }

        // then for the flat roof, we should be able to construct polygons using the new points, which we then later triangulate, and build triangles out of... I hope there is some triangulation library.. i dont wanna math
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
        newShape.tag = "Roof";
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
        newShape.tag = "Roof";
    }
    static void CreateQuadFromPoints(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, List<Material> roofMaterials)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]{ p1, p2, p3, p4};
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
        // newShape.transform.position = new Vector3(centerX, centerY, centerZ);
        // newShape.transform.Rotate(rot, Space.World);

        newShape.AddComponent<MeshFilter>();
        newShape.AddComponent<MeshRenderer>();
        newShape.GetComponent<MeshFilter>().sharedMesh = mesh;
        newShape.GetComponent<MeshRenderer>().material = roofMaterials[0];
        newShape.tag = "Roof";
    }
    static void CreateTriFromPoints(Vector3 p1, Vector3 p2, Vector3 p3, List<Material> roofMaterials)
    {
        Mesh mesh = new Mesh();
        // Just create a triangle
        Vector3[] vertices = new Vector3[] {p1, p2, p3};
        mesh.vertices = vertices;
        int[] tri = new int[3]
        {
            // same order or clockwise..?
            0, 1, 2 // 0, 2, 1
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
        
        GameObject newShape = new GameObject();
        newShape.AddComponent<Shape>();
        newShape.AddComponent<MeshFilter>();
        newShape.AddComponent<MeshRenderer>();
        newShape.GetComponent<MeshFilter>().sharedMesh = mesh;
        newShape.GetComponent<MeshRenderer>().material = roofMaterials[0];
        newShape.tag = "Roof";
    }
}
