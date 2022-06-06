using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static Texture2D CalculatePerlinNoise(Texture2D source, float scale)
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
}
