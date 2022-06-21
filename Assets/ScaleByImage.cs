using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScaleByImage : MonoBehaviour
{
    [SerializeField] private TMP_InputField lowerBound;
    [SerializeField] private TMP_InputField upperBound;
    [SerializeField] private TMP_InputField selectFileInputField;

    public void Execute()
    {
        var lower = float.Parse(lowerBound.text);
        var higher = float.Parse(upperBound.text);
        string path = selectFileInputField.text;
        
        Texture2D texture = new Texture2D(512, 512);

        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            texture.LoadImage(fileContent);
        }
        
        // now just get the positions of each selected object, and then get the same position on the input image
        // depending on how black or white it is, scale by a linearly interpolated value between lower and higher with rgb b/w value as t value
        // then literally just apply that as the scale... almost sounds too simple to be true! :D 
    }
}
