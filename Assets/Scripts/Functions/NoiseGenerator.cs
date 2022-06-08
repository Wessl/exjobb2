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
        float frequency = 32f;
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
                
                sourceTex.SetPixel(x, y, Color.white * Value2D(point, frequency));
            }
        }

        sourceTex.Apply();
        return sourceTex;
    }

    public static float Value1D(Vector3 point, float frequency)
    {
        point *= frequency;
        int i = Mathf.FloorToInt(point.x);
        i &= 15;
        return hash[i] * (1f / hashMask);
    }
    
    public static float Value2D(Vector3 point, float frequency)
    {
        point *= frequency;
        int ix = Mathf.FloorToInt(point.x);
        int iy = Mathf.FloorToInt(point.y);
        ix &= hashMask;
        iy &= hashMask;
        return hash[(ix + hash[iy]) & hashMask] * (1f / hashMask);
    }

    private static int[] hash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
        57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
        74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
        60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
        65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
        52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
        81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };

    private const int hashMask = 255;
}
