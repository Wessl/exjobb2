using System;
using System.Collections.Generic;
using UnityEditor.Scripting.Python;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Net.Sockets;

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
                if (elem[0] == '[')
                {
                    // This is the beginning of a function call - handle it. 
                    // Previous token should be the name of the function
                    string funcCall = RemoveChar("'", splitLine[i - 1]);
                }
                // Check if element is a keyword
                if (keywordList.Contains(elem))
                {
                   
                }
                else
                {
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
                            HandleFuncCall(elem, i, splitLine);
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

    /*
     * If HandleFuncCall gets a function and a list of arguments that is not another function,
     * it should EXECUTE this function. Else, it should try to pass this along to GetTopLevelList. 
     */
    private List<string> HandleFuncCall(string funcName, int funcIndex, string[] splitLine)
    {
        currFuncDepth=0;
        // Start right after the function call
        // Combine tokens from start index
        string asdf123 = "";
        for (int i = funcIndex; i < splitLine.Length; i++)
        {
            asdf123 += RemoveChar("'", splitLine[i]) + ",";
        }
    
        //Debug.Log("here is asdf: "+asdf123);
        string functionContains = "";
        for (int i = funcIndex; i < splitLine.Length; i++)
        {
            splitLine[i] += ",";
            for (int y = 0; y < splitLine[i].Length; y++)
            {
                var currElem = splitLine[i][y];
                if (currElem == '[')
                {
                    currFuncDepth++;
                }
                if (currFuncDepth >= 1)
                {
                    functionContains += (currElem);
                }
                if (currElem == ']')
                {
                    currFuncDepth--;
                }
            }
        }

        List<string> topLevelList = new List<string>();
        if (currFuncDepth == 0)
        {
            //Debug.Log("finished handling. currFundCepth is at " + currFuncDepth + " and the function contains" + functionContains);
            // Now turn it into top level argument array
            topLevelList = GetTopLevelList(functionContains);
            //EvaluateFuncArgs(functionContains);
        }
        
        bool hasFunction = false;
        foreach (var arg in topLevelList)
        {
            // If we never encounter a function call in the top level list, then we can execute the functionName with the arguments! 
            Debug.Log("arg before funcName extraction: " + arg);
            string argFuncName = GetFunctionName(arg);
            Debug.Log("arg after funcName extraction: " + argFuncName);
            if (functionList.Contains(argFuncName))
            {
                Debug.Log("argfunname:" + argFuncName);
                hasFunction = true;
            }
        }

        if (!hasFunction)
        {
            ExecuteFunctionFromName(funcName, topLevelList);
        }
        
        return topLevelList;


    }

    private List<string> GetTopLevelList(string functionContains)
    {
        List<string> topLevelArguments = new List<string>();
        functionContains = functionContains.Substring(1, functionContains.Length - 2); // remove first and last char
        var funcSplitted = functionContains.Split(',');
        for (int i = 0; i < funcSplitted.Length; i++)
        {
            var currFuncArg = RemoveChar("'", funcSplitted[i]);
            currFuncArg = RemoveChar(" ", currFuncArg);
            if (currFuncArg.Contains('['))
            {
                // Belongs to previous funcSplitted
                var topLevelArg = RemoveChar("'", funcSplitted[i - 1]) + currFuncArg;
                if (currFuncArg.Contains(']'))
                {
                    // It's also ending here
                    // Remove previous token from list...?
                    topLevelArguments.RemoveAt(topLevelArguments.Count - 1);
                    // And then add current element
                    topLevelArguments.Add(topLevelArg);
                }
                else
                {
                    // Find where it ends
                    var topLevel = DetermineFunctionEncompassment(funcSplitted.Skip(i).ToArray());
                    // Debug.Log("here is the new top level argument: " + topLevelArguments[topLevelArguments.Count - 1] + topLevel.Item1);
                    topLevelArguments.Add(topLevelArguments[topLevelArguments.Count - 1] + topLevel.Item1);
                    // We've added the function along with it's arguments, so now, let's remove just the function name.
                    topLevelArguments.RemoveAt(topLevelArguments.Count - 2);    
                    // Also use where the function encompassment ends so we continue from that point
                    i += topLevel.Item2;
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
                topLevelArguments.Add(currFuncArg);
            }
        }
        
        for (int i = 0; i < topLevelArguments.Count; i++)
        {
            string dynArg = topLevelArguments[i];
            
            // If at this point there are functions within the top level arguments, pass those to handleFuncCall..?
            string potentialFuncName = GetFunctionName(dynArg);
            if (functionList.Contains(potentialFuncName))
            {
                // This is where the real shit starts. we have to recursively call HandleFuncCall

                HandleFuncCall(dynArg, 0, topLevelArguments.Skip(i-1).ToArray());
            }
        }

        return topLevelArguments;
    }

    // Find all arguments encompassed by function call as string, as well as char index of ending point
    private Tuple<string,int> DetermineFunctionEncompassment(string[] splitLine)
    {
        currFuncDepth = 0;
        string funcContains = "";
        int charCount = 0;
        for (int i = 0; i < splitLine.Length; i++)
        {
            string currLineElem = RemoveChar(" ", splitLine[i]) +",";
            for (int y = 0; y < currLineElem.Length; y++)
            {
                var currElem = currLineElem[y];
                if (currElem == '\'')
                {
                    continue;
                }
                if (currElem == '[')
                {
                    currFuncDepth++;
                }
                if (currFuncDepth >= 1)
                {
                    funcContains += currElem;
                    charCount++;
                }
                if (currElem == ']')
                {
                    currFuncDepth--;
                    funcContains += ',';    // I am assuming we should always add a comma to the end?
                    charCount++;
                }
                
            }
        }
        return Tuple.Create(funcContains, charCount);
    }

    private string GetFunctionName(string fullFunction)
    {
        string funcName = "";
        for (int i = 0; i < fullFunction.Length; i++)
        {
            if (fullFunction[i] == '[')
            {
                // Function opening starts here, time to return
                break;
            }
            funcName += fullFunction[i];
        }
        return funcName;
    }

    private void ExecuteFunctionFromName(string name, List<string> arguments)
    {
        Debug.Log("Just got " + name + " with some arguments. Cool");
        foreach (var arg in arguments)
        {
            Debug.Log(arg);
        }
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
            "toGlobalY", "toGlobalX",                                                                 // 
            "include", "shareCorner", "finalRoof",                                                  // 
            "toParentX", "toParentY", "toShapeX", "toShapeY", "toLocalX", "toLocalY", "queryCorner",// Other utility functions
            "count", "last", "indexRange", "index", "numRows", "numCols", "rowLast", "colLast",     // 
            "rowRange", "colRange",                                                                 // 
            "constrain", "snap2", "sym2region", "dist2layout", "dist2region", "dist2left",          // Constraint functions
            "dist2right", "dist2bottom", "dist2top", "validIntersect",                              // 
            "randint", "rand", "randSelect",                                                        // Math functions
            "if", "eval",                                                                           // Flow control and stochastic variations
            "list"                                                                                  // Not defined as functions by SELEX, but added by me (list is easier to parse when it's a function)
        };
        return functions.ToList();
    }
}

