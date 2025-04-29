using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class RemoveCapitalLetterComments : MonoBehaviour
{
    [MenuItem("Tools/Remove Capital Letter Comments")]
    public static void RemoveComments()
    {
        // Get all C# files in the Assets folder
        string[] csharpFiles = Directory.GetFiles(
            Application.dataPath,
            "*.cs",
            SearchOption.AllDirectories
        );
        int totalComments = 0;
        int totalFiles = 0;

        foreach (string filePath in csharpFiles)
        {
            bool fileModified = false;
            string[] lines = File.ReadAllLines(filePath);
            List<string> newLines = new List<string>();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Skip XML documentation comments (///)
                if (trimmedLine.StartsWith("///"))
                {
                    newLines.Add(line);
                    continue;
                }

                // Check for inline comments that start with a capital letter
                int commentIndex = line.IndexOf("//");
                if (commentIndex >= 0)
                {
                    string commentText = line.Substring(commentIndex + 2).TrimStart();
                    if (commentText.Length > 0 && char.IsUpper(commentText[0]))
                    {
                        // Remove the comment part, keep the code
                        string codePart = line.Substring(0, commentIndex).TrimEnd();
                        if (string.IsNullOrEmpty(codePart))
                        {
                            // Line was only a comment, skip it entirely
                            fileModified = true;
                            totalComments++;
                            continue;
                        }
                        else
                        {
                            // Line had code and a comment, keep only the code
                            newLines.Add(codePart);
                            fileModified = true;
                            totalComments++;
                            continue;
                        }
                    }
                }

                // Keep lines without capital letter comments
                newLines.Add(line);
            }

            // Write changes back to the file if it was modified
            if (fileModified)
            {
                File.WriteAllLines(filePath, newLines);
                totalFiles++;
                Debug.Log($"Removed comments from: {filePath}");
            }
        }

        Debug.Log($"Complete! Removed {totalComments} comments from {totalFiles} files.");
    }
}
