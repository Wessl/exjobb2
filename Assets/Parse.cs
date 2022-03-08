using System.Collections.Generic;
using UnityEditor.Scripting.Python;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class Parse : MonoBehaviour
{
    // Todo: Change 'string' to File type and allow user to pick with file explorer?
    [SerializeField] private string selexTextFile;
    [SerializeField] private string pythonParser;
    [SerializeField] private string parseResults;

    private List<string> keywordList;
    private Dictionary<string, dynamic> assignedVariables = new Dictionary<string, dynamic>(); 
    public void ParseFile()
    {
        // This needs to be runnable via editor button, so change Start() to something else
        //Debug.Log(Application.dataPath + "/" + pythonParser);
        // PythonRunner.RunFile(Application.dataPath + "/" + pythonParser);
    }

    public void UseParsedFile()
    {
        // Get the parse results stored in a file
        string parsedFilePath = Application.dataPath + "/" + parseResults;
        Debug.Log(parsedFilePath);
        StreamReader reader = new StreamReader(parsedFilePath);
        // Misc preparations
        keywordList = PopulateKeywordList();
        // Start working with the results.
        while (!reader.EndOfStream)
        {
            // Preprocess
            string line = reader.ReadLine();
            line = line.Substring(1, line.Length - 2); // remove first and last char
            string[] splitLine = line.Split(',');
            // Handle each line
            for (int i = 0; i < splitLine.Length; i++)
            {
                string elem = splitLine[i];
                // Remove quotes..? how do we handle []
                elem = RemoveChar("'", elem).Trim();
                
                // Check if element is a keyword
                if (keywordList.Contains(elem))
                {
                    //Debug.Log(elem + "is a keyword");
                }
                else
                {
                    //Debug.Log(elem + " isn't a keyword");
                }

                Debug.Log("elem is " + elem);
                if (elem.Equals("="))
                {
                    Debug.Log("oh shit whaddup");
                    HandleAssignment(splitLine, i);
                }
            }
        }
        reader.Close();
    }

    private string RemoveChar(string c, string elem)
    {
        return elem.Replace(c, string.Empty);
    }

    private void HandleAssignment(string[] line, int index)
    {
        string variableName = line[index - 1];
        assignedVariables[variableName] = line[index + 1];
        Debug.Log(variableName + " = " + line[index + 1]);
    }

    private List<string> PopulateKeywordList()
    {
        var keywords = new string[] {
            "child", "descedant", "parent", "root", "self", "neighbor",
            "label", "type", "rowIdx", "colIdx", "rowLabel", "colLabel",
            "last", "rowLast", "colLast", "groupRows", "groupCols",
            "groupRegions", "if", "randomSelect", "eval"};
        return keywords.ToList();
    }
}

