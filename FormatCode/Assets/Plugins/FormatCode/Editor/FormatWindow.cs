using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FormatWindow : EditorWindow
{
    private static readonly Rect progressBarRect = new Rect(15f, 15f, 700f, 120f);

    #region private members

    private bool needFormat;
    private bool isInsertSpaces;
    private int needFormatFileCount;
    private int selectLineEndingIndex;
    private int selectEncodingIndex;
    private int spaceCount = 4;
    private string filePath = "Assets/Plugins/FormatCode/TestFolder/";
    private string formattingFileName;
    private string fileExtension = "*.cs";
    private string[] fileSuffixs;
    private readonly List<string> formattingFiles = new List<string>();

    // 以下两个数组一一对应
    private readonly string[] encodingTypes =
    {
            "UTF-8",
            "UTF-32",
            "Unicode",
            "GB2312",
    };

    private readonly Encoding[] encodings =
    {
        Encoding.UTF8,
        Encoding.UTF32,
        Encoding.Unicode,
        Encoding.GetEncoding("GB2312"),
    };

    // 以下两个数组一一对应
    private readonly string[] lineEndingTypes =
    {
            "Windows(CRLF)",
            "Unix(LF)",
    };

    private readonly string[] lineEndings =
    {
        "\r\n",
        "\n"
    };

    #endregion private members

    #region unity

    [MenuItem("Window/FormatWindow &c")]
    private static void Init()
    {
        var window = GetWindow<FormatWindow>();
        window.titleContent = new GUIContent("FormatWindow");
        window.maximized = true;
        window.Show();
    }

    private void Update()
    {
        if (!needFormat || needFormatFileCount == 0)
            return;

        if (formattingFiles.Count == 0)
        {
            needFormat = false;
            EditorUtility.DisplayDialog("FormatDialog", "Format Finish", "ok");
            return;
        }

        formattingFileName = formattingFiles[0];
        HandleCurFile(formattingFileName);
        formattingFiles.RemoveAt(0);
        Repaint();
    }

    private void OnGUI()
    {
        if (needFormat)
        {
            DrawProgress();
        }
        else
        {
            DrawControlPad();
        }
    }

    #endregion unity

    #region private imp

    private void HandleCurFile(string fileName)
    {
        try
        {
            string content = File.ReadAllText(fileName);

            // 替换换行符
            content = content.Replace("\r", "");
            content = content.Replace("\n", lineEndings[selectLineEndingIndex]);

            // 处理制表符
            content = isInsertSpaces ? content.Replace("\t", new string(' ', spaceCount)) : content.Replace(new string(' ', spaceCount), "\t");

            // 按对应编码写入文件
            File.WriteAllText(fileName, content, encodings[selectEncodingIndex]);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("FormatWindow HandleCurFile Format file faild, fileName: {0} msg: {1}", fileName, ex.Message);
        }
    }

    private void DrawProgress()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        EditorGUI.ProgressBar(
            progressBarRect,
            (float)(needFormatFileCount - formattingFiles.Count) / needFormatFileCount,
            string.Format("Formatting:\n{0}", formattingFileName)
        );

        EditorGUILayout.EndVertical();
    }

    private void DrawControlPad()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();
        HandleFilePath();

        EditorGUILayout.Space();
        HandleFileSuffix();

        EditorGUILayout.Space();
        HandleFileEncoding();

        EditorGUILayout.Space();
        HandleFileLineEnding();

        EditorGUILayout.Space();
        HandleFileTab();

        EditorGUILayout.Space();
        if (GUILayout.Button("Start"))
        {
            StartFormat();
        }

        EditorGUILayout.EndVertical();
    }

    private void StartFormat()
    {
        if (string.IsNullOrEmpty(filePath) || !Directory.Exists(filePath))
        {
            EditorUtility.DisplayDialog("FormatDialog", "You need select a exist folder", "ok");
            return;
        }
        if (string.IsNullOrEmpty(fileExtension))
        {
            EditorUtility.DisplayDialog("FormatDialog", "You need select some file suffix", "ok");
            return;
        }
        fileSuffixs = fileExtension.Split(';');
        formattingFiles.Clear();
        foreach (var curSuffix in fileSuffixs)
        {
            if (string.IsNullOrEmpty(curSuffix))
            {
                continue;
            }
            string[] filenames = Directory.GetFiles(filePath, curSuffix, SearchOption.AllDirectories);
            formattingFiles.AddRange(filenames);
        }
        needFormatFileCount = formattingFiles.Count;
        if (needFormatFileCount == 0)
        {
            EditorUtility.DisplayDialog("FormatDialog", "Can't find any file in seleted folder", "ok");
            return;
        }
        needFormat = true;
    }

    private void HandleFilePath()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Folder, e.g. Assets/Scripts/");
        filePath = EditorGUILayout.TextField(filePath);

        EditorGUILayout.EndHorizontal();
    }

    private void HandleFileSuffix()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("File extension，e.g. *.txt;*.cs");
        fileExtension = EditorGUILayout.TextField(fileExtension);

        EditorGUILayout.EndHorizontal();
    }

    private void HandleFileEncoding()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Encoding");
        selectEncodingIndex = EditorGUILayout.Popup(selectEncodingIndex, encodingTypes);

        EditorGUILayout.EndHorizontal();
    }

    private void HandleFileLineEnding()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("LineEnding");
        selectLineEndingIndex = EditorGUILayout.Popup(selectLineEndingIndex, lineEndingTypes);

        EditorGUILayout.EndHorizontal();
    }

    private void HandleFileTab()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("InsertSpaces");
        isInsertSpaces = EditorGUILayout.Toggle(isInsertSpaces);
        EditorGUILayout.EndHorizontal();

        if (isInsertSpaces)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("SpaceCount");
            spaceCount = EditorGUILayout.IntField(spaceCount);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    #endregion private imp
}