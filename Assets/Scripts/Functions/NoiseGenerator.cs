using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static Texture2D ApplyPerlinNoiseToTexture(Texture2D source, float scale)
    {
        // Width and height of the texture in pixels.
        int pixWidth = source.width;
        int pixHeight = source.height;

        // The origin of the sampled area in the plane.
        float xOrg = 0; // Potentially randomize or do something else with this?
        float yOrg = 0;
        
        // Set up the texture and a Col or array to hold pixels during processing.
        Texture2D noiseTex = new Texture2D(pixWidth, pixHeight);
        Color[] pix = new Color[noiseTex.width * noiseTex.height];
        
        // For each pixel in the texture...
        float y = 0.0F;
        var sourcePixels = source.GetPixels();

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord) * sourcePixels[(int)y * noiseTex.width + (int)x].a;
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        return noiseTex;
    }

    public static Texture2D ApplyNoiseToTexture(Texture2D sourceTex)
    {
        
        int pixWidth = sourceTex.width;
        int pixHeight = sourceTex.height;
        float stepSize = 1f / pixHeight;
        sourceTex = new Texture2D(pixWidth, pixHeight);
        float frequency = 10f;
        Vector3 point00 = new Vector3(-0.5f,-0.5f);
        Vector3 point10 = new Vector3( 0.5f,-0.5f);
        Vector3 point01 = new Vector3(-0.5f, 0.5f);
        Vector3 point11 = new Vector3( 0.5f, 0.5f);
        Random.seed = 42;
        for (int y = 0; y < pixHeight; y++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (int x = 0; x < pixWidth; x++)
            {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                Debug.Log(Value(point, frequency));
                sourceTex.SetPixel(x, y, Color.white * Value(point, frequency));
            }
        }
        sourceTex.Apply();
        return sourceTex;
    }

    public static float Value(Vector3 point, float frequency)
    {
        point *= frequency;
        float i = point.x;
        return i % 2;
    }
}
