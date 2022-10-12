using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class NoiseGenerator
{
    public static Texture2D ApplyPerlinNoiseToTexture(Texture2D source, float scale, float strength)
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
                float xCoord = (xOrg + x / noiseTex.width);
                float yCoord = ( yOrg + y / noiseTex.height);
                float originalSource = sourcePixels[(int)y * noiseTex.width + (int)x].a;
                float noiseImpact = (1 - Mathf.PerlinNoise(xCoord * scale, yCoord * scale)) * originalSource;
                float sample = Mathf.Lerp(originalSource, noiseImpact, strength);
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

    public static Texture2D ApplyNoiseToTexture(Texture2D sourceTex, float scale, float strength)
    {
        int octaves = 6;
        int pixWidth = sourceTex.width;
        int pixHeight = sourceTex.height;
        float stepSize = 1f / pixHeight;
        float frequency = 5;
        Vector3 point00 = new Vector3(-0.5f,-0.5f);
        Vector3 point10 = new Vector3( 0.5f,-0.5f);
        Vector3 point01 = new Vector3(-0.5f, 0.5f);
        Vector3 point11 = new Vector3( 0.5f, 0.5f);
        Random.seed = 42;
        Texture2D noiseTex = new Texture2D(pixWidth, pixHeight);
        Color[] pix = new Color[noiseTex.width * noiseTex.height];
        
        var sourcePixels = sourceTex.GetPixels();
        for (int y = 0; y < pixHeight; y++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (int x = 0; x < pixWidth; x++)
            {
	            Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
	            float originalSource = sourcePixels[(int)y * noiseTex.width + (int)x].a;
	            float noiseImpact = Sum(point * scale, frequency, octaves) * originalSource;
	            float sample = Mathf.Lerp(originalSource, noiseImpact, strength);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample, sample);
            }
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        return noiseTex;
    }

    public static float Value1D(Vector3 point, float frequency)
    {
        point *= frequency;
        int i0 = Mathf.FloorToInt(point.x);
        i0 &= hashMask;
        int i1 = i0 + 1;
        float t = point.x - i0;
        float h0 = hash[i0];
        float h1 = hash[i1];
        t = Smooth(t);
        return Mathf.Lerp(h0, h1, t) * (1f / hashMask);
    }
    
    public static float Value2D(Vector3 point, float frequency)
    {
        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        float tx0 = point.x - ix0;
        float ty0 = point.y - iy0;
        float tx1 = tx0 - 1f;
        float ty1 = ty0 = -1f;
        ix0 &= hashMask;
        iy0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        Vector2 g00 = gradients2D[hash[h0 + iy0] & gradientsMask2D];
        Vector2 g10 = gradients2D[hash[h1 + iy0] & gradientsMask2D];
        Vector2 g01 = gradients2D[hash[h0 + iy1] & gradientsMask2D];
        Vector2 g11 = gradients2D[hash[h1 + iy1] & gradientsMask2D];

        float v00 = Dot(g00, tx0, ty0);
        float v10 = Dot(g10, tx1, ty0);
        float v01 = Dot(g01, tx0, ty1);
        float v11 = Dot(g11, tx1, ty1);

        float tx = Smooth(tx0);
        float ty = Smooth(ty0);
        
        return Mathf.Lerp(
            Mathf.Lerp(v00, v10, tx),
            Mathf.Lerp(v01, v11, tx),
            ty) * sqr2;
    }
    
    private static float Dot (Vector3 g, float x, float y, float z) {
        return g.x * x + g.y * y + g.z * z;
    }
	
    public static float Value3D (Vector3 point, float frequency) {
        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        int iz0 = Mathf.FloorToInt(point.z);
        float tx = point.x - ix0;
        float ty = point.y - iy0;
        float tz = point.z - iz0;
        ix0 &= hashMask;
        iy0 &= hashMask;
        iz0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;
        int iz1 = iz0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        int h00 = hash[h0 + iy0];
        int h10 = hash[h1 + iy0];
        int h01 = hash[h0 + iy1];
        int h11 = hash[h1 + iy1];
        int h000 = hash[h00 + iz0];
        int h100 = hash[h10 + iz0];
        int h010 = hash[h01 + iz0];
        int h110 = hash[h11 + iz0];
        int h001 = hash[h00 + iz1];
        int h101 = hash[h10 + iz1];
        int h011 = hash[h01 + iz1];
        int h111 = hash[h11 + iz1];

        tx = Smooth(tx);
        ty = Smooth(ty);
        tz = Smooth(tz);
        return Mathf.Lerp(
            Mathf.Lerp(Mathf.Lerp(h000, h100, tx), Mathf.Lerp(h010, h110, tx), ty),
            Mathf.Lerp(Mathf.Lerp(h001, h101, tx), Mathf.Lerp(h011, h111, tx), ty),
            tz) * (1f / hashMask);
    }
    
    private static Vector3[] gradients3D = {
        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 1f,-1f, 0f),
        new Vector3(-1f,-1f, 0f),
        new Vector3( 1f, 0f, 1f),
        new Vector3(-1f, 0f, 1f),
        new Vector3( 1f, 0f,-1f),
        new Vector3(-1f, 0f,-1f),
        new Vector3( 0f, 1f, 1f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f, 1f,-1f),
        new Vector3( 0f,-1f,-1f),
		
        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f,-1f,-1f)
    };
	
    private const int gradientsMask3D = 15;
    
    private static Vector2[] gradients2D = {
        new Vector2( 1f, 0f),
        new Vector2(-1f, 0f),
        new Vector2( 0f, 1f),
        new Vector2( 0f,-1f),
        new Vector2( 1f, 1f).normalized,
        new Vector2(-1f, 1f).normalized,
        new Vector2( 1f,-1f).normalized,
        new Vector2(-1f,-1f).normalized
    };

    private static float sqr2 = Mathf.Sqrt(2f);
	
    private const int gradientsMask2D = 3;
    
    private static float Dot (Vector2 g, float x, float y) {
        return g.x * x + g.y * y;
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
		222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

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
    
    private static float Smooth (float t) {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }
    
    public static float Sum (Vector3 point, float frequency, int octaves)
    {
        float sum = Value3D(point, frequency);
        float amplitude = 1f;
        float range = 1f;
        for (int o = 1; o < octaves; o++)
        {
            frequency *= 3f;
            amplitude *= 0.5f;
            range += amplitude;
            sum += Value3D(point, frequency) * amplitude;
        }
        return sum / range;
    }

    private const int hashMask = 255;
}
