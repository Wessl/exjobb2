using UnityEditor.Scripting.Python;
using UnityEditor;
using UnityEngine;
using System.IO;

public class Parse : MonoBehaviour
{
    // Todo: Change 'string' to File type and allow user to pick with file explorer?
    [SerializeField] private string selexTextFile;
    [SerializeField] private string pythonParser;
    [SerializeField] private string parseResults;
    public void ParseFile()
    {
        // This needs to be runnable via editor button, so change Start() to something else
        //Debug.Log(Application.dataPath + "/" + pythonParser);
        // PythonRunner.RunFile(Application.dataPath + "/" + pythonParser);
    }

    public void UseParsedFile()
    {
        string parsedFilePath = Application.dataPath + "/" + parseResults;
        Debug.Log(parsedFilePath);
        StreamReader reader = new StreamReader(parsedFilePath);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}
