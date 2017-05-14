// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;


namespace TMPro.EditorUtilities
{

    public class TMPro_FontAssetCreatorWindow : EditorWindow
    {

        [MenuItem("Window/TextMeshPro - Font Asset Creator")]
        public static void ShowFontAtlasCreatorWindow()
        {
            var window = GetWindow<TMPro_FontAssetCreatorWindow>();
            window.title = "Asset Creator";
            window.Focus();
        }

        private string[] FontSizingOptions = { "Auto Sizing", "Custom Size" };
        private int FontSizingOption_Selection = 0;
        private string[] FontResolutionLabels = { "16","32", "64", "128", "256", "512", "1024", "2048", "4096", "8192" };
        private int[] FontAtlasResolutions = { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
        private string[] FontCharacterSets = { "ASCII", "Extended ASCII", "ASCII Lowercase", "ASCII Uppercase", "Numbers + Symbols", "Custom Range", "Custom Characters", "Characters from File" }; //, "Unicode" };
        private enum FontPackingModes { Fast = 0, Optimum = 4 };      
        private FontPackingModes m_fontPackingSelection = 0;

        private int font_CharacterSet_Selection = 0;
        private enum PreviewSelectionTypes { PreviewFont, PreviewTexture, PreviewDistanceField };
        private PreviewSelectionTypes previewSelection;

        private string characterSequence = "";
        private string output_feedback = "";
        private string output_name_label = "Font: ";
        private string output_size_label = "Pt. Size: ";
        private string output_count_label = "Characters packed: ";
        private int m_character_Count;

        //private GUISkin TMP_GUISkin;
        //private GUIStyle TextureAreaBox;
        //private GUIStyle TextAreaBox;
        //private GUIStyle Section_Label;


        private Thread MainThread;
        private Color[] Output;
        private bool isDistanceMapReady = false;
        private bool isRepaintNeeded = false;

        private Rect progressRect;
        public static float ProgressPercentage;
        private float m_renderingProgress;
        private bool isRenderingDone = false;
        private bool isProcessing = false;

        private Object font_TTF;
        private TextAsset characterList;
        private int font_size;

        private int font_padding = 5;
        private FaceStyles font_style = FaceStyles.Normal;
        private float font_style_mod = 2;
        private RenderModes font_renderMode = RenderModes.DistanceField16;
        private int font_atlas_width = 512;
        private int font_atlas_height = 512;

        private int font_scaledownFactor = 1;
        private int font_spread = 4;

        private FT_FaceInfo m_font_faceInfo;
        private FT_GlyphInfo[] m_font_glyphInfo;
        private byte[] m_texture_buffer;
        private Texture2D m_font_Atlas;
        //private Texture2D m_texture_Atlas;
        //private int m_packingMethod = 0;

        private Texture2D m_destination_Atlas;
        private bool includeKerningPairs = false;
        private int[] m_kerningSet;

        // Image Down Sampling Fields
        private Texture2D sdf_Atlas;
        private int downscale;

        //private Object prev_Selection;

        private EditorWindow m_editorWindow;
        private Vector2 m_previewWindow_Size = new Vector2(768, 768);
        private Rect m_UI_Panel_Size;


        public void OnEnable()
        {
           
            m_editorWindow = this;
            UpdateEditorWindowSize(768, 768);


            // Get the UI Skin and Styles for the various Editors
            TMP_UIStyleManager.GetUIStyles();


            // Locate the plugin files & move them to root of project if that hasn't already been done.
#if !UNITY_5
            // Find to location of the TextMesh Pro Asset Folder (as users may have moved it)
            string tmproAssetFolderPath = TMPro_EditorUtility.GetAssetLocation();

            string projectPath = Path.GetFullPath("Assets/..");
        
            if (System.IO.File.Exists(projectPath + "/TMPro_Plugin.dll") == false)
            {                           
                FileUtil.ReplaceFile(tmproAssetFolderPath + "/Plugins/TMPro_Plugin.dll", projectPath + "/TMPro_Plugin.dll"); // Copy the .dll
                FileUtil.ReplaceFile(tmproAssetFolderPath + "/Plugins/TMPro_Plugin.dylib", projectPath + "/TMPro_Plugin.dylib"); // Copy Mac .dylib
                FileUtil.ReplaceFile(tmproAssetFolderPath + "/Plugins/vcomp120.dll", projectPath + "/vcomp120.dll"); // Copy OpemMP .dll                 
            } 
            else // Check if we are using the latest versions
            {                               
                if (System.IO.File.GetLastWriteTime(tmproAssetFolderPath + "/Plugins/TMPro_Plugin.dylib") > System.IO.File.GetLastWriteTime(projectPath + "/TMPro_Plugin.dylib"))
                    FileUtil.ReplaceFile(tmproAssetFolderPath + "/Plugins/TMPro_Plugin.dylib", projectPath + "/TMPro_Plugin.dylib");

                if (System.IO.File.GetLastWriteTime(tmproAssetFolderPath + "/Plugins/TMPro_Plugin.dll") > System.IO.File.GetLastWriteTime(projectPath + "/TMPro_Plugin.dll"))
                    FileUtil.ReplaceFile(tmproAssetFolderPath + "/Plugins/TMPro_Plugin.dll", projectPath + "/TMPro_Plugin.dll");

                if (System.IO.File.GetLastWriteTime(tmproAssetFolderPath + "/Plugins/vcomp120.dll") > System.IO.File.GetLastWriteTime(projectPath + "/vcomp120.dll"))
                    FileUtil.ReplaceFile(tmproAssetFolderPath + "/Plugins/vcomp120.dll", projectPath + "/vcomp120.dll");
            }
#endif

            // Add Event Listener related to Distance Field Atlas Creation.
            TMPro_EventManager.COMPUTE_DT_EVENT += ON_COMPUTE_DT_EVENT;

            // Debug Link to received message from Native Code
            //TMPro_FontPlugin.LinkDebugLog(); // Link with C++ Plugin to get Debug output
        }


        public void OnDisable()
        {
            //Debug.Log("TextMeshPro Editor Window has been disabled.");

            TMPro_EventManager.COMPUTE_DT_EVENT -= ON_COMPUTE_DT_EVENT;

            // Destroy Enging only if it has been initialized already
            if (TMPro_FontPlugin.Initialize_FontEngine() == 99)
            {
                TMPro_FontPlugin.Destroy_FontEngine();
            }

            // Cleaning up allocated Texture2D
            if (m_destination_Atlas != null && EditorUtility.IsPersistent(m_destination_Atlas) == false)
            {
                //Debug.Log("Destroying destination_Atlas!");
                DestroyImmediate(m_destination_Atlas);
            }

            if (m_font_Atlas != null && EditorUtility.IsPersistent(m_font_Atlas) == false)
            {
                //Debug.Log("Destroying font_Atlas!");
                DestroyImmediate(m_font_Atlas);
            }
        }


        public void OnGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(310));
            DrawControls();

            DrawPreview();
            GUILayout.EndHorizontal();
        }

        public void ON_COMPUTE_DT_EVENT(object Sender, Compute_DT_EventArgs e)
        {
            if (e.EventType == Compute_DistanceTransform_EventTypes.Completed)
            {
                Output = e.Colors;
                isProcessing = false;
                isDistanceMapReady = true;
            }
            else if (e.EventType == Compute_DistanceTransform_EventTypes.Processing)
            {
                ProgressPercentage = e.ProgressPercentage;
                isRepaintNeeded = true;
            }
        }


        public void Update()
        {
            if (isDistanceMapReady)
            {
                if (m_font_Atlas != null)
                {
                    m_destination_Atlas = new Texture2D(m_font_Atlas.width / font_scaledownFactor, m_font_Atlas.height / font_scaledownFactor, TextureFormat.Alpha8, false, true);
                    m_destination_Atlas.SetPixels(Output);
                    m_destination_Atlas.Apply(false, true);
                }
                //else if (m_texture_Atlas != null)
                //{
                //    m_destination_Atlas = new Texture2D(m_texture_Atlas.width / font_scaledownFactor, m_texture_Atlas.height / font_scaledownFactor, TextureFormat.Alpha8, false, true);
                //    m_destination_Atlas.SetPixels(Output);
                //    m_destination_Atlas.Apply(false, true);
                //}

                isDistanceMapReady = false;
                Repaint();

                // Saving File for Debug
                //var pngData = destination_Atlas.EncodeToPNG();	     
                //File.WriteAllBytes("Assets/Textures/Debug SDF.png", pngData);	
            }

            if (isRepaintNeeded)
            {
                //Debug.Log("Repainting...");
                isRepaintNeeded = false;
                Repaint();
            }

            // Update Progress bar is we are Rendering a Font.
            if (isProcessing)
            {
                m_renderingProgress = TMPro_FontPlugin.Check_RenderProgress();

                isRepaintNeeded = true;
            }

            // Update Feedback Window & Create Font Texture once Rendering is done.
            if (isRenderingDone)
            {
                isProcessing = false;
                isRenderingDone = false;
                UpdateRenderFeedbackWindow();
                CreateFontTexture();
            }
        }



        int[] ParseNumberSequence(string sequence)
        {
            List<int> unicode_list = new List<int>();
            string[] sequences = sequence.Split(',');

            foreach (string seq in sequences)
            {
                string[] s1 = seq.Split('-');

                if (s1.Length == 1)
                    try
                    {
                        unicode_list.Add(int.Parse(s1[0]));
                    }
                    catch
                    {
                        Debug.Log("No characters selected or invalid format.");
                    }
                else
                {
                    for (int j = int.Parse(s1[0]); j < int.Parse(s1[1]) + 1; j++)
                    {
                        unicode_list.Add(j);
                    }
                }
            }

            return unicode_list.ToArray();
        }


        void DrawControls()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("<b>TextMeshPro - Font Asset Creator</b>", TMP_UIStyleManager.Section_Label, GUILayout.Width(300));
            GUILayout.Label("Font Settings", TMP_UIStyleManager.Section_Label, GUILayout.Width(300));

            GUILayout.BeginVertical(TMP_UIStyleManager.TextureAreaBox, GUILayout.Width(300));
            EditorGUIUtility.LookLikeControls(120f, 160f);

            // FONT TTF SELECTION
            font_TTF = EditorGUILayout.ObjectField("Font Source", font_TTF, typeof(Font), false, GUILayout.Width(290)) as Font;

            // FONT SIZING
            if (FontSizingOption_Selection == 0)
            {
                FontSizingOption_Selection = EditorGUILayout.Popup("Font Size", FontSizingOption_Selection, FontSizingOptions, GUILayout.Width(290));            
            }
            else
            {
                EditorGUIUtility.LookLikeControls(120f, 40f);
                GUILayout.BeginHorizontal(GUILayout.Width(290));
                FontSizingOption_Selection = EditorGUILayout.Popup("Font Size", FontSizingOption_Selection, FontSizingOptions, GUILayout.Width(225));
                font_size = EditorGUILayout.IntField(font_size);
                GUILayout.EndHorizontal();
            }

            EditorGUIUtility.LookLikeControls(120f, 160f);

            
            // FONT PADDING   
            font_padding = EditorGUILayout.IntField("Font Padding", font_padding, GUILayout.Width(290));
            font_padding = (int)Mathf.Clamp(font_padding, 0f, 64f);

            // FONT PACKING METHOD SELECTION
            m_fontPackingSelection = (FontPackingModes)EditorGUILayout.EnumPopup("Packing Method", m_fontPackingSelection, GUILayout.Width(225));
            
            //font_renderingMode = (FontRenderingMode)EditorGUILayout.EnumPopup("Rendering Mode", font_renderingMode, GUILayout.Width(290));

            // FONT ATLAS RESOLUTION SELECTION
            GUILayout.BeginHorizontal(GUILayout.Width(290));
            GUI.changed = false;
            EditorGUIUtility.LookLikeControls(120f, 40f);

            GUILayout.Label("Atlas Resolution:", GUILayout.Width(116));
            font_atlas_width = EditorGUILayout.IntPopup(font_atlas_width, FontResolutionLabels, FontAtlasResolutions); //, GUILayout.Width(80));
            font_atlas_height = EditorGUILayout.IntPopup(font_atlas_height, FontResolutionLabels, FontAtlasResolutions); //, GUILayout.Width(80));      

            GUILayout.EndHorizontal();


            // FONT CHARACTER SET SELECTION
            GUI.changed = false;
            font_CharacterSet_Selection = EditorGUILayout.Popup("Character Set", font_CharacterSet_Selection, FontCharacterSets, GUILayout.Width(290));
            if (GUI.changed)
            {
                characterSequence = "";
                //Debug.Log("Resetting Sequence!");
            }

            switch (font_CharacterSet_Selection)
            {
                case 0: // ASCII
                    //characterSequence = "32 - 126, 130, 132 - 135, 139, 145 - 151, 153, 155, 161, 166 - 167, 169 - 174, 176, 181 - 183, 186 - 187, 191, 8210 - 8226, 8230, 8240, 8242 - 8244, 8249 - 8250, 8252 - 8254, 8260, 8286";
                    characterSequence = "32 - 126, 8230";
                    break;

                case 1: // EXTENDED ASCII
                    characterSequence = "32 - 126, 161 - 255, 8210 - 8226, 8230, 8240, 8242 - 8244, 8249 - 8250, 8252 - 8254, 8260, 8286";                   
                    break;

                case 2: // Lowercase                          
                    characterSequence = "32 - 64, 91 - 126";
                    break;

                case 3: // Uppercase                      
                    characterSequence = "32 - 96, 123 - 126";
                    break;

                case 4: // Numbers & Symbols                      
                    characterSequence = "32 - 64, 91 - 96, 123 - 126";
                    break;

                case 5: // Custom Range           
                    GUILayout.BeginHorizontal(GUILayout.Width(290));
                    GUILayout.Label("Custom Range", GUILayout.Width(116));

                    // Filter out unwanted characters.
                    char chr = Event.current.character;
                    if ((chr < '0' || chr > '9') && (chr < ',' || chr > '-'))
                    {
                        Event.current.character = '\0';
                    }
                    characterSequence = EditorGUILayout.TextArea(characterSequence, TMP_UIStyleManager.TextAreaBoxWindow, GUILayout.Height(32), GUILayout.MaxWidth(170));

                    GUILayout.EndHorizontal();
                    break;

                case 6: // Custom Characters
                    GUILayout.BeginHorizontal(GUILayout.Width(290));

                    GUILayout.Label("Custom Characters", GUILayout.Width(116));
                    characterSequence = EditorGUILayout.TextArea(characterSequence, TMP_UIStyleManager.TextAreaBoxWindow, GUILayout.Height(32), GUILayout.MaxWidth(170));
                    GUILayout.EndHorizontal();
                    break;

                case 7: // Character List from File
                    characterList = EditorGUILayout.ObjectField("Character File", characterList, typeof(TextAsset), false, GUILayout.Width(290)) as TextAsset;
                    if (characterList != null)
                    {
                        characterSequence = characterList.text;
                    }
                    break;
            }

            EditorGUIUtility.LookLikeControls(120f, 40f);

            // FONT STYLE SELECTION
            GUILayout.BeginHorizontal(GUILayout.Width(290));
            font_style = (FaceStyles)EditorGUILayout.EnumPopup("Font Style:", font_style, GUILayout.Width(225));
            font_style_mod = EditorGUILayout.IntField((int)font_style_mod);
            GUILayout.EndHorizontal();

            // Render Mode Selection   
            font_renderMode = (RenderModes)EditorGUILayout.EnumPopup("Font Render Mode:", font_renderMode, GUILayout.Width(290));

            includeKerningPairs = EditorGUILayout.Toggle("Get Kerning Pairs?", includeKerningPairs, GUILayout.MaxWidth(290));

            EditorGUIUtility.LookLikeControls(120f, 160f);

            GUILayout.Space(20);

            GUI.enabled = font_TTF == null || isProcessing ? false : true;    // Enable Preview if we are not already rendering a font.
            if (GUILayout.Button("Generate Font Atlas", GUILayout.Width(290)) && characterSequence.Length != 0 && GUI.enabled)
            {
                if (font_TTF != null)
                {
                    int error_Code;

                    error_Code = TMPro_FontPlugin.Initialize_FontEngine(); // Initialize Font Engine
                    if (error_Code != 0)
                    {
                        if (error_Code == 99)
                        {
                            //Debug.Log("Font Library was already initialized!");
                            error_Code = 0;
                        }
                        else
                            Debug.Log("Error Code: " + error_Code + "  occurred while Initializing the FreeType Library.");
                    }

                    string fontPath = AssetDatabase.GetAssetPath(font_TTF); // Get file path of TTF Font.

                    if (error_Code == 0)
                    {
                        error_Code = TMPro_FontPlugin.Load_TrueType_Font(fontPath); // Load the selected font.

                        if (error_Code != 0)
                        {
                            if (error_Code == 99)
                            {
                                //Debug.Log("Font was already loaded!");
                                error_Code = 0;
                            }
                            else
                                Debug.Log("Error Code: " + error_Code + "  occurred while Loading the font.");
                        }
                    }

                    if (error_Code == 0)
                    {
                        if (FontSizingOption_Selection == 0) font_size = 72; // If Auto set size to 72 pts.

                        error_Code = TMPro_FontPlugin.FT_Size_Font(font_size); // Load the selected font and size it accordingly.
                        if (error_Code != 0)
                            Debug.Log("Error Code: " + error_Code + "  occurred while Sizing the font.");                      
                    }

                    // Define an array containing the characters we will render.
                    if (error_Code == 0)
                    {
                        int[] character_Set = null;
                        if (font_CharacterSet_Selection == 6 || font_CharacterSet_Selection == 7)
                        {
                            List<int> char_List = new List<int>();
                            
                            for (int i = 0; i < characterSequence.Length; i++)
                            {
                                // Check to make sure we don't include duplicates
                                if (char_List.FindIndex(item => item == characterSequence[i]) == -1)
                                    char_List.Add(characterSequence[i]);
                                else
                                {
                                    //Debug.Log("Character [" + characterSequence[i] + "] is a duplicate.");
                                }                              
                            }

                            character_Set = char_List.ToArray();
                        }
                        else
                        {
                            character_Set = ParseNumberSequence(characterSequence);
                        }

                        m_character_Count = character_Set.Length;

                        m_texture_buffer = new byte[font_atlas_width * font_atlas_height];

                        m_font_faceInfo = new FT_FaceInfo();

                        m_font_glyphInfo = new FT_GlyphInfo[m_character_Count];

                        int padding = font_padding;

                        bool autoSizing = FontSizingOption_Selection == 0 ? true : false;

                        float strokeSize = font_style_mod;
                        if (font_renderMode == RenderModes.DistanceField16) strokeSize = font_style_mod * 16;
                        if (font_renderMode == RenderModes.DistanceField32) strokeSize = font_style_mod * 32;
                        
                        isProcessing = true;
                        
                        ThreadPool.QueueUserWorkItem(SomeTask =>
                        {
                            isRenderingDone = false;
                            
                            error_Code = TMPro_FontPlugin.Render_Characters(m_texture_buffer, font_atlas_width, font_atlas_height, padding, character_Set, m_character_Count, font_style, strokeSize, autoSizing, font_renderMode,(int)m_fontPackingSelection, ref m_font_faceInfo, m_font_glyphInfo);
                            isRenderingDone = true;
                            //Debug.Log("Font Rendering is completed.");
                        });
                        
                        previewSelection = PreviewSelectionTypes.PreviewFont;
                        
                    }
                }
            }


            // FONT RENDERING PROGRESS BAR
            GUILayout.Space(1);
            progressRect = GUILayoutUtility.GetRect(288, 20, TMP_UIStyleManager.TextAreaBoxWindow, GUILayout.Width(288), GUILayout.Height(20));

            GUI.BeginGroup(progressRect);
            GUI.DrawTextureWithTexCoords(new Rect(2, 0, 288, 20), TMP_UIStyleManager.progressTexture, new Rect(1 - m_renderingProgress, 0, 1, 1));
            GUI.EndGroup();


            // FONT STATUS & INFORMATION
            GUILayout.Space(5);
            EditorGUILayout.LabelField(output_feedback, TMP_UIStyleManager.TextAreaBoxWindow, GUILayout.Height(48), GUILayout.MaxWidth(290));

            GUILayout.Space(10);


            // SAVE TEXTURE & CREATE and SAVE FONT XML FILE
            GUI.enabled = m_font_Atlas != null ? true : false;    // Enable Save Button if font_Atlas is not Null.
            if (GUILayout.Button("Save TextMeshPro Font Asset", GUILayout.Width(290)) && GUI.enabled)
            {
                string filePath = string.Empty;

                if (font_renderMode < RenderModes.DistanceField16) // == RenderModes.HintedSmooth || font_renderMode == RenderModes.RasterHinted)
                {
                    filePath = EditorUtility.SaveFilePanel("Save TextMesh Pro! Font Asset File", new FileInfo(AssetDatabase.GetAssetPath(font_TTF)).DirectoryName, font_TTF.name, "asset");

                    if (filePath.Length == 0)
                        return;

                    Save_Normal_FontAsset(filePath);
                }
                else if (font_renderMode >= RenderModes.DistanceField16)
                {
                    filePath = EditorUtility.SaveFilePanel("Save TextMesh Pro! Font Asset File", new FileInfo(AssetDatabase.GetAssetPath(font_TTF)).DirectoryName, font_TTF.name + " SDF", "asset");

                    if (filePath.Length == 0)
                        return;

                    Save_SDF_FontAsset(filePath);
                }

            }

            GUI.enabled = true; // Re-enable GUI

            GUILayout.Space(5);

            GUILayout.EndVertical();

            GUILayout.Space(25);

            /*
            // GENERATE DISTANCE FIELD TEXTURE
            GUILayout.Label("Distance Field Options", SectionLabel, GUILayout.Width(300));

            GUILayout.BeginVertical(textureAreaBox, GUILayout.Width(300));

            GUILayout.Space(5);


            font_spread = EditorGUILayout.IntField("Spread", font_spread, GUILayout.Width(280));
            font_scaledownFactor = EditorGUILayout.IntField("Scale down factor", font_scaledownFactor, GUILayout.Width(280));
            if (GUI.changed)
            {
                EditorPrefs.SetInt("Font_Spread", font_spread);
                EditorPrefs.SetInt("Font_ScaleDownFactor", font_scaledownFactor);
            }

            GUILayout.Space(20);

            GUI.enabled = m_font_Atlas != null ? true : false;    // Enable Save Button if font_Atlas is not Null.
            if (GUILayout.Button("Preview Distance Field Font Atlas", GUILayout.Width(290)))
            {

                if (m_font_Atlas != null && isProcessing == false)
                {
                    // Generate Distance Field	                 
                    int width = m_font_Atlas.width;
                    int height = m_font_Atlas.height;
                    Color[] colors = m_font_Atlas.GetPixels(); // Should modify this to use Color32 instead

                    isProcessing = true;

                    ThreadPool.QueueUserWorkItem(SomeTask => { TMPro_DistanceTransform.Generate(colors, width, height, font_spread, font_scaledownFactor); });

                    previewSelection = PreviewSelectionTypes.PreviewDistanceField;
                }
            }

            GUILayout.Space(1);

            progressRect = GUILayoutUtility.GetRect(290, 20, textAreaBox, GUILayout.Width(290), GUILayout.Height(20));

            GUI.BeginGroup(progressRect);

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, 290, 20), progressTexture, new Rect(1 - ProgressPercentage, 0, 1, 1));
            GUI.EndGroup();

            //GUILayout.Space(5);

            GUI.enabled = m_destination_Atlas != null ? true : false;    // Enable Save Button if font_Atlas is not Null.
            if (GUILayout.Button("Save TextMeshPro (SDF) Font Asset", GUILayout.Width(290)))
            {
                string filePath = EditorUtility.SaveFilePanel("Save TextMesh Pro! Font Asset File", new FileInfo(AssetDatabase.GetAssetPath(font_TTF)).DirectoryName, font_TTF.name + " SDF", "asset");

                if (filePath.Length == 0)
                    return;

                Save_SDF_FontAsset(filePath);

            } 
                         
            GUILayout.EndVertical();
            */
             
            // Figure out the size of the current UI Panel
            Rect rect = EditorGUILayout.GetControlRect(false, 5);
            if (Event.current.type == EventType.Repaint)
                m_UI_Panel_Size = rect;

            GUILayout.EndVertical();
        }


        void UpdateRenderFeedbackWindow()
        {
            font_size = m_font_faceInfo.pointSize;

            string colorTag = m_font_faceInfo.characterCount == m_character_Count ? "<color=#C0ffff>" : "<color=#ffff00>";
            string colorTag2 = "<color=#C0ffff>";

            output_feedback = output_name_label + "<b>" + colorTag2 + m_font_faceInfo.name + "</color></b>";

            if (output_feedback.Length > 60)
                output_feedback += "\n" + output_size_label + "<b>" + colorTag2 + m_font_faceInfo.pointSize + "</color></b>";
            else
                output_feedback += "  " + output_size_label + "<b>" + colorTag2 + m_font_faceInfo.pointSize + "</color></b>";

            output_feedback += "\n" + output_count_label + "<b>" + colorTag + m_font_faceInfo.characterCount + "/" + m_character_Count + "</color></b>";
        }


        void CreateFontTexture()
        {
            m_font_Atlas = new Texture2D(font_atlas_width, font_atlas_height, TextureFormat.Alpha8, false, true);
            m_font_Atlas.hideFlags = HideFlags.DontSave;

            Color32[] colors = new Color32[font_atlas_width * font_atlas_height];

            for (int i = 0; i < (font_atlas_width * font_atlas_height); i++)
            {
                byte c = m_texture_buffer[i];
                colors[i] = new Color32(c, c, c, c);
            }

            if (font_renderMode == RenderModes.RasterHinted)
                m_font_Atlas.filterMode = FilterMode.Point;

            m_font_Atlas.SetPixels32(colors, 0);
            m_font_Atlas.Apply(false, false);

            // Saving File for Debug
            //var pngData = m_font_Atlas.EncodeToPNG();
            //File.WriteAllBytes("Assets/Textures/Debug Font Texture.png", pngData);	

            UpdateEditorWindowSize(m_font_Atlas.width, m_font_Atlas.height);
            //previewSelection = PreviewSelectionTypes.PreviewFont;
        }


        void Save_Normal_FontAsset(string filePath)
        {
            filePath = filePath.Substring(0, filePath.Length - 6); // Trim file extension from filePath.         

            string dataPath = Application.dataPath;

            if (filePath.IndexOf(dataPath) == -1)
            {
                Debug.LogError("You're saving the font asset in a directory outside of this project folder. This is not supported. Please select a directory under \"" + dataPath + "\"");
                return;
            }

            string relativeAssetPath = filePath.Substring(dataPath.Length - 6);
            string tex_DirName = Path.GetDirectoryName(relativeAssetPath);
            string tex_FileName = Path.GetFileNameWithoutExtension(relativeAssetPath);
            string tex_Path_NoExt = tex_DirName + "/" + tex_FileName;

            // Check if TextMeshPro font asset already exists. If not, create a new one. Otherwise update the existing one.
            TextMeshProFont font_asset = AssetDatabase.LoadAssetAtPath(tex_Path_NoExt + ".asset", typeof(TextMeshProFont)) as TextMeshProFont;
            if (font_asset == null)
            {
                //Debug.Log("Creating TextMeshPro font asset!");
                font_asset = ScriptableObject.CreateInstance<TextMeshProFont>(); // Create new TextMeshPro Font Asset.     
                AssetDatabase.CreateAsset(font_asset, tex_Path_NoExt + ".asset");

                // Add FaceInfo to Font Asset
                FaceInfo face = GetFaceInfo(m_font_faceInfo, 1);
                font_asset.AddFaceInfo(face);

                // Add GlyphInfo[] to Font Asset
                GlyphInfo[] glyphs = GetGlyphInfo(m_font_glyphInfo, 1);
                font_asset.AddGlyphInfo(glyphs);

                // Get and Add Kerning Pairs to Font Asset
                if (includeKerningPairs)
                {
                    string fontFilePath = AssetDatabase.GetAssetPath(font_TTF);
                    KerningTable kerningTable = GetKerningTable(fontFilePath, (int)face.PointSize);
                    font_asset.AddKerningInfo(kerningTable);
                }


                // Add Font Atlas as Sub-Asset
                font_asset.atlas = m_font_Atlas;
                m_font_Atlas.name = tex_FileName + " Atlas";
                m_font_Atlas.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(m_font_Atlas, font_asset);

                // Create new Material and Add it as Sub-Asset
                Shader default_Shader = Shader.Find("TMPro/Bitmap");
                Material tmp_material = new Material(default_Shader);
                tmp_material.name = tex_FileName + " Material";
                tmp_material.SetTexture(ShaderUtilities.ID_MainTex, m_font_Atlas);
                font_asset.material = tmp_material;
                tmp_material.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(tmp_material, font_asset);

            }
            else
            {
                // Find all Materials referencing this font atlas.
                Material[] material_references = TMPro_EditorUtility.FindMaterialReferences(font_asset.material);

                // Destroy Assets that will be replaced.    
                DestroyImmediate(font_asset.atlas, true);

                // Add FaceInfo to Font Asset
                FaceInfo face = GetFaceInfo(m_font_faceInfo, 1);
                font_asset.AddFaceInfo(face);

                // Add GlyphInfo[] to Font Asset
                GlyphInfo[] glyphs = GetGlyphInfo(m_font_glyphInfo, 1);
                font_asset.AddGlyphInfo(glyphs);

                // Get and Add Kerning Pairs to Font Asset
                if (includeKerningPairs)
                {
                    string fontFilePath = AssetDatabase.GetAssetPath(font_TTF);
                    KerningTable kerningTable = GetKerningTable(fontFilePath, (int)face.PointSize);
                    font_asset.AddKerningInfo(kerningTable);
                }

                // Add Font Atlas as Sub-Asset
                font_asset.atlas = m_font_Atlas;
                m_font_Atlas.name = tex_FileName + " Atlas";
                m_font_Atlas.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(m_font_Atlas, font_asset);

                // Update the Texture reference on the Material
                for (int i = 0; i < material_references.Length; i++)
                {
                    material_references[i].SetTexture(ShaderUtilities.ID_MainTex, font_asset.atlas);
                }
            }
            AssetDatabase.SaveAssets();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(font_asset));  // Re-import font asset to get the new updated version.

            //EditorUtility.SetDirty(font_asset);
            font_asset.ReadFontDefinition();

            AssetDatabase.Refresh();

            // NEED TO GENERATE AN EVENT TO FORCE A REDRAW OF ANY TEXTMESHPRO INSTANCES THAT MIGHT BE USING THIS FONT ASSET      
            TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, font_asset);
        }


        void Save_SDF_FontAsset(string filePath)
        {
            filePath = filePath.Substring(0, filePath.Length - 6); // Trim file extension from filePath.     

            string dataPath = Application.dataPath;

            if (filePath.IndexOf(dataPath) == -1)
            {
                Debug.LogError("You're saving the font asset in a directory outside of this project folder. This is not supported. Please select a directory under \"" + dataPath + "\"");
                return;
            }

            string relativeAssetPath = filePath.Substring(dataPath.Length - 6);
            string tex_DirName = Path.GetDirectoryName(relativeAssetPath);
            string tex_FileName = Path.GetFileNameWithoutExtension(relativeAssetPath);
            string tex_Path_NoExt = tex_DirName + "/" + tex_FileName;


            // Check if TextMeshPro font asset already exists. If not, create a new one. Otherwise update the existing one.
            TextMeshProFont font_asset = AssetDatabase.LoadAssetAtPath(tex_Path_NoExt + ".asset", typeof(TextMeshProFont)) as TextMeshProFont;
            if (font_asset == null)
            {
                //Debug.Log("Creating TextMeshPro font asset!");
                font_asset = ScriptableObject.CreateInstance<TextMeshProFont>(); // Create new TextMeshPro Font Asset.     
                AssetDatabase.CreateAsset(font_asset, tex_Path_NoExt + ".asset");

                if (m_destination_Atlas != null)
                    m_font_Atlas = m_destination_Atlas;

                // If using the C# SDF creation mode, we need the scaledown factor.
                int scaleDownFactor = font_renderMode >= RenderModes.DistanceField16 ? 1 : font_scaledownFactor;

                // Add FaceInfo to Font Asset
                FaceInfo face = GetFaceInfo(m_font_faceInfo, scaleDownFactor);
                font_asset.AddFaceInfo(face);

                // Add GlyphInfo[] to Font Asset
                GlyphInfo[] glyphs = GetGlyphInfo(m_font_glyphInfo, scaleDownFactor);
                font_asset.AddGlyphInfo(glyphs);

                // Get and Add Kerning Pairs to Font Asset
                if (includeKerningPairs)
                {
                    string fontFilePath = AssetDatabase.GetAssetPath(font_TTF);
                    KerningTable kerningTable = GetKerningTable(fontFilePath, (int)face.PointSize);
                    font_asset.AddKerningInfo(kerningTable);
                }

                // Add Line Breaking Rules
                //LineBreakingTable lineBreakingTable = new LineBreakingTable();
                //

                // Add Font Atlas as Sub-Asset
                font_asset.atlas = m_font_Atlas;
                m_font_Atlas.name = tex_FileName + " Atlas";
                m_font_Atlas.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(m_font_Atlas, font_asset);

                // Create new Material and Add it as Sub-Asset
                Shader default_Shader = Shader.Find("TMPro/Distance Field");
                Material tmp_material = new Material(default_Shader);
                //tmp_material.shaderKeywords = new string[] { "BEVEL_OFF", "GLOW_OFF", "UNDERLAY_OFF" };
                tmp_material.name = tex_FileName + " Material";
                tmp_material.SetTexture(ShaderUtilities.ID_MainTex, m_font_Atlas);
                tmp_material.SetFloat(ShaderUtilities.ID_TextureWidth, m_font_Atlas.width);
                tmp_material.SetFloat(ShaderUtilities.ID_TextureHeight, m_font_Atlas.height);


                tmp_material.SetFloat(ShaderUtilities.ID_WeightNormal, font_asset.NormalStyle);
                tmp_material.SetFloat(ShaderUtilities.ID_WeightBold, font_asset.BoldStyle);

                int spread = font_renderMode >= RenderModes.DistanceField16 ? font_padding + 1 : font_spread;
                tmp_material.SetFloat(ShaderUtilities.ID_GradientScale, spread); // Spread = Padding for Brute Force SDF.

                font_asset.material = tmp_material;
                tmp_material.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(tmp_material, font_asset);

            }
            else
            {
                // Find all Materials referencing this font atlas.
                Material[] material_references = TMPro_EditorUtility.FindMaterialReferences(font_asset.material);

                // Destroy Assets that will be replaced.        
                DestroyImmediate(font_asset.atlas, true);

                int scaleDownFactor = font_renderMode >= RenderModes.DistanceField16 ? 1 : font_scaledownFactor;
                // Add FaceInfo to Font Asset  
                FaceInfo face = GetFaceInfo(m_font_faceInfo, scaleDownFactor);
                font_asset.AddFaceInfo(face);

                // Add GlyphInfo[] to Font Asset
                GlyphInfo[] glyphs = GetGlyphInfo(m_font_glyphInfo, scaleDownFactor);
                font_asset.AddGlyphInfo(glyphs);

                // Get and Add Kerning Pairs to Font Asset
                if (includeKerningPairs)
                {
                    string fontFilePath = AssetDatabase.GetAssetPath(font_TTF);
                    KerningTable kerningTable = GetKerningTable(fontFilePath, (int)face.PointSize);
                    font_asset.AddKerningInfo(kerningTable);
                }

                // Add Font Atlas as Sub-Asset
                font_asset.atlas = m_font_Atlas;
                m_font_Atlas.name = tex_FileName + " Atlas";
                m_font_Atlas.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(m_font_Atlas, font_asset);

                // Update the Texture reference on the Material
                for (int i = 0; i < material_references.Length; i++)
                {
                    material_references[i].SetTexture(ShaderUtilities.ID_MainTex, font_asset.atlas);
                    material_references[i].SetFloat(ShaderUtilities.ID_TextureWidth, m_font_Atlas.width);
                    material_references[i].SetFloat(ShaderUtilities.ID_TextureHeight, m_font_Atlas.height);

                    material_references[i].SetFloat(ShaderUtilities.ID_WeightNormal, font_asset.NormalStyle);
                    material_references[i].SetFloat(ShaderUtilities.ID_WeightBold, font_asset.BoldStyle);

                    int spread = font_renderMode >= RenderModes.DistanceField16 ? font_padding + 1 : font_spread;
                    material_references[i].SetFloat(ShaderUtilities.ID_GradientScale, spread); // Spread = Padding for Brute Force SDF.           
                }
            }

            // Saving File for Debug
            //var pngData = destination_Atlas.EncodeToPNG();
            //File.WriteAllBytes("Assets/Textures/Debug Distance Field.png", pngData);	
            //font_asset.fontCreationSettings = SaveFontCreationSettings();


            AssetDatabase.SaveAssets();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(font_asset));  // Re-import font asset to get the new updated version.

            font_asset.ReadFontDefinition();

            AssetDatabase.Refresh();

            // NEED TO GENERATE AN EVENT TO FORCE A REDRAW OF ANY TEXTMESHPRO INSTANCES THAT MIGHT BE USING THIS FONT ASSET    
            TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, font_asset);
        }


        FontCreationSetting SaveFontCreationSettings()
        {
            FontCreationSetting settings = new FontCreationSetting();
            settings.fontSourcePath = AssetDatabase.GetAssetPath(font_TTF);
            settings.fontSizingMode = FontSizingOption_Selection;
            settings.fontSize = font_size;
            settings.fontPadding = font_padding;
            settings.fontPackingMode = (int)m_fontPackingSelection;
            settings.fontAtlasWidth = font_atlas_width;
            settings.fontAtlasHeight = font_atlas_height;
            settings.fontCharacterSet = font_CharacterSet_Selection;
            settings.fontStyle = (int)font_style;
            settings.fontStlyeModifier = font_style_mod;
            settings.fontRenderMode = (int)font_renderMode;
            settings.fontKerning = includeKerningPairs;

            return settings;
        }



        void UpdateEditorWindowSize(float width, float height)
        {
            m_previewWindow_Size = new Vector2(768, 768);

            if (width > height)
            {
                m_previewWindow_Size = new Vector2(768, height / (width / 768));
            }
            else if (height > width)
            {
                m_previewWindow_Size = new Vector2(width / (height / 768), 768);
            }

            m_editorWindow.minSize = new Vector2(m_previewWindow_Size.x + 330, Mathf.Max(m_UI_Panel_Size.y + 20f, m_previewWindow_Size.y + 20f));
            m_editorWindow.maxSize = m_editorWindow.minSize + new Vector2(.25f, 0);
        }


        void DrawPreview()
        {

            // Display Texture Area
            GUILayout.BeginVertical(TMP_UIStyleManager.TextureAreaBox);

            Rect pixelRect = GUILayoutUtility.GetRect(m_previewWindow_Size.x, m_previewWindow_Size.y, TMP_UIStyleManager.Section_Label);

            if (m_destination_Atlas != null && previewSelection == PreviewSelectionTypes.PreviewDistanceField)
            {
                EditorGUI.DrawTextureAlpha(new Rect(pixelRect.x, pixelRect.y, m_previewWindow_Size.x, m_previewWindow_Size.y), m_destination_Atlas, ScaleMode.ScaleToFit);
            }
            //else if (m_texture_Atlas != null && previewSelection == PreviewSelectionTypes.PreviewTexture)
            //{
            //    GUI.DrawTexture(new Rect(pixelRect.x, pixelRect.y, m_previewWindow_Size.x, m_previewWindow_Size.y), m_texture_Atlas, ScaleMode.ScaleToFit); 
            //}
            else if (m_font_Atlas != null && previewSelection == PreviewSelectionTypes.PreviewFont)
            {
                EditorGUI.DrawTextureAlpha(new Rect(pixelRect.x, pixelRect.y, m_previewWindow_Size.x, m_previewWindow_Size.y), m_font_Atlas, ScaleMode.ScaleToFit);
            }

            GUILayout.EndVertical();
        }


        // Convert from FT_FaceInfo to FaceInfo
        FaceInfo GetFaceInfo(FT_FaceInfo ft_face, int scaleFactor)
        {
            FaceInfo face = new FaceInfo();

            face.Name = ft_face.name;
            face.PointSize = (float)ft_face.pointSize / scaleFactor;
            face.Padding = 0; // ft_face.padding / scaleFactor;
            face.LineHeight = ft_face.lineHeight / scaleFactor;
            face.Baseline = 0;
            face.Ascender = ft_face.ascender / scaleFactor;
            face.Descender = ft_face.descender / scaleFactor;     
            face.CenterLine = ft_face.centerLine / scaleFactor;
            face.Underline = ft_face.underline / scaleFactor;
            face.UnderlineThickness = ft_face.underlineThickness == 0 ? 5 : ft_face.underlineThickness / scaleFactor; // Set Thickness to 5 if TTF value is Zero.
            face.SuperscriptOffset = face.Ascender;
            face.SubscriptOffset = face.Underline;
            face.SubSize = 0.5f;
            face.CharacterCount = ft_face.characterCount;
            face.AtlasWidth = ft_face.atlasWidth / scaleFactor;
            face.AtlasHeight = ft_face.atlasHeight / scaleFactor;

            return face;
        }


        // Convert from FT_GlyphInfo[] to GlyphInfo[]
        GlyphInfo[] GetGlyphInfo(FT_GlyphInfo[] ft_glyphs, int scaleFactor)
        {
            GlyphInfo[] glyphs = new GlyphInfo[ft_glyphs.Length];
            m_kerningSet = new int[ft_glyphs.Length];

            for (int i = 0; i < ft_glyphs.Length; i++)
            {
                GlyphInfo g = new GlyphInfo();

                g.id = ft_glyphs[i].id;
                g.x = ft_glyphs[i].x / scaleFactor;
                g.y = ft_glyphs[i].y / scaleFactor;
                g.width = ft_glyphs[i].width / scaleFactor;
                g.height = ft_glyphs[i].height / scaleFactor;
                g.xOffset = ft_glyphs[i].xOffset / scaleFactor;
                g.yOffset = ft_glyphs[i].yOffset / scaleFactor;
                g.xAdvance = ft_glyphs[i].xAdvance / scaleFactor;

                glyphs[i] = g;
                m_kerningSet[i] = g.id;
            }

            return glyphs;
        }


        // Get Kerning Pairs
        public KerningTable GetKerningTable(string fontFilePath, int pointSize)
        {
            KerningTable kerningInfo = new KerningTable();
            kerningInfo.kerningPairs = new List<KerningPair>();

            // Temporary Array to hold the kerning paris from the Native Plugin.
            FT_KerningPair[] kerningPairs = new FT_KerningPair[1000];

            int kpCount = TMPro_FontPlugin.FT_GetKerningPairs(fontFilePath, m_kerningSet, m_kerningSet.Length, kerningPairs);

            for (int i = 0; i < kpCount; i++)
            {
                // Proceed to add each kerning pairs.
                KerningPair kp = new KerningPair(kerningPairs[i].ascII_Left, kerningPairs[i].ascII_Right, kerningPairs[i].xAdvanceOffset * pointSize);
                kerningInfo.kerningPairs.Add(kp);
            }

            return kerningInfo;
        }

    }
}