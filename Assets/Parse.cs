using System;
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

    private List<string> keywordList;   // Contains the keywords defined by SELEX   
    private List<string> functionList;  // Contains the functions defined by SELEX
    
    private Dictionary<string, dynamic> assignedVariables = new Dictionary<string, dynamic>();
    private int currFuncDepth = 0;
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
        functionList = PopulateFunctionList();
        
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
                
                // Remove quotes
                elem = RemoveChar("'", elem);
                //Debug.Log(elem);
                if (elem[0] == '[')
                {
                    // This is the beginning of a function call - handle it. 
                    // Previous token should be the name of the function
                    string funcCall = RemoveChar("'", splitLine[i - 1]);
                }
                // Check if element is a keyword
                if (keywordList.Contains(elem))
                {
                    //Debug.Log(elem + "is a keyword");
                }
                else
                {
                    //Debug.Log(elem + " isn't a keyword");
                }
                
                // Check if element is a function
                if (functionList.Contains(elem))
                {
                    //Debug.Log(elem + " is a function");
                    var nextElem = RemoveChar("'", splitLine[i + 1]);
                    if (nextElem[0] == '[')
                    {
                        if (nextElem[1] == ']')
                        {
                            // No arguments found for the function
                        }
                        else
                        {
                            HandleFuncCall(elem, i, splitLine, line);
                        }
                    }
                }
                else
                {
                    
                }
                
                if (elem.Equals("="))
                {
                    HandleAssignment(splitLine, i);
                    foreach (var kvpair in assignedVariables)
                    {
                        //Debug.Log(kvpair.ToString());
                    }
                }
            }
        }
        reader.Close();
    }

    private void HandleFuncCall(string funcName, int funcIndex, string[] splitLine, string unSplitLine)
    {
        currFuncDepth=0;
        // Start right after the function call
        List<dynamic> topLevelArguments = new List<dynamic>();
        // Combine tokens from start index
        string asdf123 = "";
        for (int i = funcIndex; i < splitLine.Length; i++)
        {
            asdf123 += RemoveChar("'", splitLine[i]) + ",";
        }
    
        Debug.Log("here is asdf: "+asdf123);
        string functionContains = "";
        for (int i = funcIndex; i < splitLine.Length; i++)
        {
            for (int y = 0; y < splitLine[i].Length; y++)
            {
                var currElem = splitLine[i][y];
                if (currElem == '[')
                {
                    currFuncDepth++;
                }
                if (currFuncDepth >= 1)
                {
                    functionContains += currElem;
                }
                if (currElem == ']')
                {
                    currFuncDepth--;
                }
            }
        }

        if (currFuncDepth == 0)
        {
            Debug.Log("finished handling. currFundCepth is at " + currFuncDepth + " and the function contains" + functionContains);
            // Now turn it into top level argument array
            GetTopLevelList(functionContains);
            //EvaluateFuncArgs(functionContains);
        }
    }

    private void GetTopLevelList(string functionContains)
    {
        List<string> topLevelArguments = new List<string>();
        functionContains = functionContains.Substring(1, functionContains.Length - 2); // remove first and last char
        var funcSplitted = functionContains.Split(' ');
        for (int i = 0; i < funcSplitted.Length; i++)
        {
            var currFuncArg = RemoveChar("'", funcSplitted[i]);
            if (currFuncArg.Contains('['))
            {
                // Belongs to previous funcSplitted
                var topLevelArg = RemoveChar("'", funcSplitted[i - 1]) + currFuncArg;
                if (currFuncArg.Contains(']'))
                {
                    // It's also ending here
                    topLevelArguments.Add(topLevelArg);
                }
                else
                {
                    // Find where it ends
                    var topLevel = DetermineFunctionEncompassment(funcSplitted.Skip(i).ToArray());
                    topLevelArguments.Add(topLevel);
                    
                }
            }
            else if ( ! functionList.Contains(currFuncArg))
            {
                // We can assume it's just a number or something
                topLevelArguments.Add(currFuncArg);
            }
            else
            {
                // This should be a function call that we are going to use later...?
                Debug.Log("this should be a function: " + currFuncArg);
            }
        }
        
        Debug.Log("Here cometh topLevelArguments");
        foreach (var dynArg in topLevelArguments)
        {
            Debug.Log(dynArg);
        }
    }

    private string DetermineFunctionEncompassment(string[] splitLine)
    {
        currFuncDepth = 0;
        string funcContains = "";
        for (int i = 0; i < splitLine.Length; i++)
        {
            for (int y = 0; y < splitLine[i].Length; y++)
            {
                var currElem = splitLine[i][y];
                if (currElem == '[')
                {
                    currFuncDepth++;
                }
                if (currFuncDepth >= 1)
                {
                    funcContains += currElem;
                }
                if (currElem == ']')
                {
                    currFuncDepth--;
                }
            }
        }

        return funcContains;
    }

    private void EvaluateFuncArgs(string functionContains)
    {
        // Is this a function? If so, recursively call me again.
        functionContains = functionContains.Substring(1, functionContains.Length - 2); // remove first and last char
        var funcSplitted = functionContains.Split(' ');
        
        for (int i = 0; i < funcSplitted.Length; i++)
        {
            var element = RemoveChar("'", funcSplitted[i]);
            if (functionList.Contains(element))
            {
                Debug.Log("internal function spotted");
                // Start from next element, if we have toGlobalX next element is e.g. [0.5]
                var nextElem = funcSplitted[i + 1];
                // From that element, run it through an encompassment function to see what it contains. Could be just [ 0.5 ].
                string contains = DetermineFunctionEncompassment(funcSplitted.Skip(i + 1).ToArray());
                Debug.Log("contains: "+ contains);
                // Or it could be dist2bottom[ dist2center [...]], in which case the recursive call should recognize it as a function, and run through the arguments again, etc...
                EvaluateFuncArgs(contains);
            }
        }
        // If not, return the value. 
        
    }

    private string RemoveChar(string c, string elem)
    {
        return elem.Replace(c, string.Empty).Trim();
    }

    private void HandleAssignment(string[] line, int index)
    {
        string variableName = RemoveChar("'", line[index - 1]).Trim();
        assignedVariables[variableName] = RemoveChar("'",line[index + 1]).Trim();
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
    private List<string> PopulateFunctionList()
    {
        // Here goes a list of all the functions we can use
        var functions = new string[]
        {
            "child", "descendant", "parent", "root", "neighbor", "left", "right",                   // topology selectors
            "isEmpty", "pattern", "isEven", "isOdd",                                                // Attribute testing functions
            "groupRows", "groupCols", "groupRegions", "groupEach", "groupPair", "cell", "sortBy",   // Grouping functions    ('cell' might be 'cells' not sure)
            "addShape", "add2ProjectedLeafShape", "attachShape", "connectShape", "coverShape",      // Shape functions
            "copyShape", "polygon", "addVolume", "lineElem", "group", "createGrid", "rows",         // 
            "cols", "setAttrib", "exchange", "transform", "rotate", "translate", "scale",           // 
            "include", "shareCorner", "finalRoof",                                                  // 
            "toParentX", "toParentY", "toShapeX", "toShapeY", "toLocalX", "toLocalY", "queryCorner",// Other utility functions
            "count", "last", "indexRange", "index", "numRows", "numCols", "rowLast", "colLast",     // 
            "rowRange", "colRange",                                                                 // 
            "constrain", "snap2", "sym2region", "dist2layout", "dist2region", "dist2left",          // Constraint functions
            "dist2right", "dist2bottom", "dist2top", "validIntersect",                              // 
            "randint", "rand", "randSelect",                                                        // Math functions
            "if", "eval"                                                                            // Flow control and stochastic variations
        };
        return functions.ToList();
    }
}
