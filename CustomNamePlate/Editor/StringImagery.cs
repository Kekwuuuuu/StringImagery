using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class StringImagery : EditorWindow
{
    private string inputText = "Sample Text";
    private Font selectedFont;
    private int fontSize = 128;
    private const int texSize = 512;

    // Fixed save path relative to Assets folder
    private static readonly string saveFolder = "Assets/CustomNamePlate/Names";

    [MenuItem("Window/StringImagery")]
    public static void ShowWindow()
    {
        GetWindow<StringImagery>("StringImagery");
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        
        inputText = EditorGUILayout.TextField("Text", inputText);
        selectedFont = (Font)EditorGUILayout.ObjectField("Font", selectedFont, typeof(Font), false);
        fontSize = EditorGUILayout.IntSlider("Font Size", fontSize, 8, 256);

        EditorGUILayout.HelpBox($"Image will be saved to:\n{saveFolder}", MessageType.Info);

        if (GUILayout.Button("Generate Image", GUILayout.Height(40)))
        {
            GenerateTexture();
        }
    }

    private void GenerateTexture()
    {
        if (string.IsNullOrEmpty(inputText))
        {
            EditorUtility.DisplayDialog("Error", "Please enter some text.", "OK");
            return;
        }

        // Use default font if none assigned
        if (selectedFont == null)
        {
            selectedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (selectedFont == null)
                selectedFont = GUI.skin.font;
        }

        // Create temporary GameObject with Canvas and Text
        GameObject tempGO = new GameObject("_TempStringImagery");
        tempGO.hideFlags = HideFlags.HideAndDontSave;
        
        Canvas canvas = tempGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        Text uiText = tempGO.AddComponent<Text>();
        uiText.font = selectedFont;
        uiText.fontSize = fontSize;
        uiText.text = inputText;
        uiText.color = Color.white;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
        uiText.verticalOverflow = VerticalWrapMode.Truncate;

        uiText.rectTransform.sizeDelta = new Vector2(texSize, texSize);
        uiText.rectTransform.anchoredPosition = Vector2.zero;

        // Camera to render the text
        GameObject camGO = new GameObject("_TempCamera");
        camGO.hideFlags = HideFlags.HideAndDontSave;
        Camera cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = texSize / 2f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); // transparent
        cam.cullingMask = 1 << LayerMask.NameToLayer("UI");
        camGO.transform.position = new Vector3(0, 0, -10);
        
        tempGO.layer = LayerMask.NameToLayer("UI");
        canvas.worldCamera = cam;
        canvas.planeDistance = 1;

        // RenderTexture setup
        RenderTexture rt = RenderTexture.GetTemporary(texSize, texSize, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        cam.Render();

        // Read pixels
        RenderTexture.active = rt;
        Texture2D finalTex = new Texture2D(texSize, texSize, TextureFormat.ARGB32, false);
        finalTex.ReadPixels(new Rect(0, 0, texSize, texSize), 0, 0);
        finalTex.Apply();
        
        // Cleanup
        RenderTexture.active = null;
        cam.targetTexture = null;
        RenderTexture.ReleaseTemporary(rt);
        DestroyImmediate(tempGO);
        DestroyImmediate(camGO);

        // Prepare save path
        string fullFolder = Path.Combine(Application.dataPath, "CustomNamePlate", "Names");
        if (!Directory.Exists(fullFolder))
            Directory.CreateDirectory(fullFolder);

        // Sanitize filename from input text
        string safeName = SanitizeFileName(inputText);
        string basePath = Path.Combine(fullFolder, safeName);
        string finalPath = basePath + ".png";
        int counter = 1;
        while (File.Exists(finalPath))
        {
            finalPath = basePath + $"_{counter++}.png";
        }

        // Save PNG
        byte[] pngData = finalTex.EncodeToPNG();
        File.WriteAllBytes(finalPath, pngData);
        AssetDatabase.Refresh();

        // Convert to asset path and import as Sprite
        string assetPath = "Assets" + finalPath.Substring(Application.dataPath.Length);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.SaveAndReimport();
        }
        
        Object createdAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        EditorGUIUtility.PingObject(createdAsset);
        EditorUtility.DisplayDialog("Success", $"Image saved to:\n{assetPath}", "OK");
    }

    private string SanitizeFileName(string name)
    {
        // Remove invalid filename characters
        string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        string sanitized = Regex.Replace(name, $"[{Regex.Escape(invalidChars)}]", "");
        // Replace spaces with underscores
        sanitized = sanitized.Replace(' ', '_');
        // Limit length
        if (sanitized.Length > 50)
            sanitized = sanitized.Substring(0, 50);
        return sanitized;
    }
}