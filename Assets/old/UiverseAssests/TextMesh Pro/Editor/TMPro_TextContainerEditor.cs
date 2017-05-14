using UnityEngine;
using UnityEditor;
using System.Collections;


namespace TMPro.EditorUtilities
{
    [CustomEditor(typeof(TextContainer)), CanEditMultipleObjects]
    public class TMPro_TextContainerEditor : Editor
    {
        
        // Serialized Properties 
        private SerializedProperty anchorPosition_prop;
        private SerializedProperty pivot_prop;
        private SerializedProperty rectangle_prop;
        private SerializedProperty margins_prop;
       

        private TextContainer m_textContainer;
        private Transform m_transform;
        private Vector3[] m_Rect_handlePoints = new Vector3[4];
        private Vector3[] m_Margin_handlePoints = new Vector3[4];

        private Vector2 m_anchorPosition;

        private Vector3 m_mousePreviousPOS;
        private Vector2 m_previousStartPOS;
        //private int m_mouseDragFlag = 0;

        private static Transform m_visualHelper;


        void OnEnable()
        {
         
            // Serialized Properties         
            anchorPosition_prop = serializedObject.FindProperty("m_anchorPosition");
            pivot_prop = serializedObject.FindProperty("m_pivot");              
            rectangle_prop = serializedObject.FindProperty("m_rect");          
            margins_prop = serializedObject.FindProperty("m_margins");

            m_textContainer = (TextContainer)target;
            m_transform = Selection.activeGameObject.transform;


            // Get the UI Skin and Styles for the various Editors
            TMP_UIStyleManager.GetUIStyles();
            

            /*
            if (m_visualHelper == null)
            {
                m_visualHelper = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                m_visualHelper.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
            */
        }

        void OnDisable()
        {
            /*
            if (m_visualHelper != null)
                DestroyImmediate (m_visualHelper.gameObject);
            */
        }

     


        public override void OnInspectorGUI()
        {
                                 
            serializedObject.Update();

            GUILayout.Label("<b>TEXT CONTAINER</b>", TMP_UIStyleManager.Section_Label, GUILayout.Height(23));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(anchorPosition_prop);
            if (anchorPosition_prop.enumValueIndex == 9)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(pivot_prop, new GUIContent("Pivot Position"));
                EditorGUI.indentLevel -= 1;
            }


            DrawDimensionProperty(rectangle_prop, "Dimensions");
            DrawMaginProperty(margins_prop, "Margins");
            if (EditorGUI.EndChangeCheck())
            {
                // Re-compute pivot position when changes are made.                            
                if (anchorPosition_prop.enumValueIndex != 9)            
                    pivot_prop.vector2Value = GetAnchorPosition(anchorPosition_prop.enumValueIndex);
                
                m_textContainer.hasChanged = true;
            }
                    
            serializedObject.ApplyModifiedProperties();
        }



        void OnSceneGUI()
        {
                       
            Event evt = Event.current;
                                         
            Vector3 rectPos = m_transform.position;
            Vector3 lossyScale = new Vector3(1, 1, 1); // m_transform.lossyScale;
            Rect rectangle = m_textContainer.rect;

            Vector2 pivot = m_textContainer.pivot; // GetAnchorPosition((int)m_textContainer.anchorPosition);
            Vector3 pivotOffset = new Vector3((0.5f - pivot.x) * rectangle.width * lossyScale.x, (0.5f - pivot.y) * rectangle.height * lossyScale.y, 0);

            m_Rect_handlePoints[0] = m_transform.TransformPoint(pivotOffset + new Vector3(-rectangle.width / 2 * lossyScale.x, -rectangle.height / 2 * lossyScale.y, 0)); // BL
            m_Rect_handlePoints[1] = m_transform.TransformPoint(pivotOffset + new Vector3(-rectangle.width / 2 * lossyScale.x, +rectangle.height / 2 * lossyScale.y, 0)); // TL
            m_Rect_handlePoints[2] = m_transform.TransformPoint(pivotOffset + new Vector3(+rectangle.width / 2 * lossyScale.x, +rectangle.height / 2 * lossyScale.y, 0)); // TR
            m_Rect_handlePoints[3] = m_transform.TransformPoint(pivotOffset + new Vector3(+rectangle.width / 2 * lossyScale.x, -rectangle.height / 2 * lossyScale.y, 0)); // BR
           
            Handles.DrawSolidRectangleWithOutline(m_Rect_handlePoints, new Color32(255, 255, 255, 0), new Color32(200, 200, 200, 255));                       

            if (evt.mousePosition.x > HandleUtility.WorldToGUIPoint(m_Rect_handlePoints[0]).x &&
                evt.mousePosition.x < HandleUtility.WorldToGUIPoint(m_Rect_handlePoints[2]).x &&
                evt.mousePosition.y > HandleUtility.WorldToGUIPoint(m_Rect_handlePoints[2]).y &&
                evt.mousePosition.y < HandleUtility.WorldToGUIPoint(m_Rect_handlePoints[0]).y)
            {
                HandleUtility.AddDefaultControl(512);
            }

            lossyScale = m_transform.lossyScale;
            /*
            if (HandleUtility.nearestControl == 512 && evt.type == EventType.mouseDown && evt.button == 0 || m_mouseDragFlag == 1)
            {
                m_mouseDragFlag = 1;

                //Vector3 intersection = EditorHandleUtilities.GetIntersectingPoint(m_Rect_handlePoints[0], m_Rect_handlePoints[1], Camera.current, evt.mousePosition);
                //Debug.DrawLine(intersection + new Vector3(-1, 0, 0), intersection + new Vector3(1, 0, 0), Color.green);
                //Debug.DrawLine(intersection + new Vector3(0, 1, 0), intersection + new Vector3(0, -1, 0), Color.green);
                //Vector3 currentPos = Camera.current.ScreenToWorldPoint(evt.mousePosition);
                //Debug.Log("Delta: " + currentPos + "  " + m_mousePreviousPOS + "  Delta: " + (m_mousePreviousPOS - currentPos));
                //m_transform.position += new Vector3(currentPos.x - m_mousePreviousPOS.x, m_mousePreviousPOS.y - currentPos.y, 0);
                //m_mousePreviousPOS = currentPos;
            }
            */

            //Vector4 rectScreenSpace = new Vector4 (HandleUtility.WorldToGUIPoint(m_Rect_handlePoints[0],HandleUtility.WorldToGUIPoint(m_Rect_handlePoints[1], HandleUtility.WorldToGUIPoint(m_Rect_handlePoints[2], HandleUtility.WorldToGUIPoint(m_Rect_handlePoints[3]); 
            //Vector3 intersection = EditorHandleUtilities.GetIntersectingPoint(m_Rect_handlePoints[2], m_Rect_handlePoints[3], Camera.current, evt.mousePosition);
            //Debug.DrawLine(Camera.main.transform.position, intersection, Color.red, 0.1f);

            //if (m_visualHelper != null)
            //    m_visualHelper.position = intersection;

            //Debug.DrawLine(intersection + new Vector3(-1, 0, 0), intersection + new Vector3(1, 0, 0), Color.green, 0.1f);
            //Debug.DrawLine(intersection + new Vector3(0, 1, 0), intersection + new Vector3(0, -1, 0), Color.green, 0.1f);


            // Draw & process FreeMoveHandles
            float handleSize = HandleUtility.GetHandleSize(rectPos) * 0.2f;
            Handles.color = new Color(0, .4f, 1f, 1f);         
            bool hasChanged = false;
            bool isShiftKey = evt.shift;
            

            // BOTTOM LEFT HANDLE
            Vector3 old_BottomLeft = m_Rect_handlePoints[0];
            Vector3 new_BottomLeft = Handles.FreeMoveHandle(old_BottomLeft, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);        
            if (old_BottomLeft != new_BottomLeft)
            {
                Undo.RecordObjects(new Object[] { m_textContainer, m_transform }, "Rectangle Dimension Changes");
                Vector2 delta = old_BottomLeft - new_BottomLeft;
                rectangle.width += delta.x / lossyScale.x;
                rectangle.height += delta.y / lossyScale.y;
                if (!isShiftKey) m_transform.position += m_transform.TransformDirection(new Vector3(-delta.x * (1 - pivot.x), -delta.y * (1 - pivot.y), 0));
                               
                hasChanged = true;              
            }

            // LEFT HANDLE            
            Vector3 old_Left = (m_Rect_handlePoints[0] + m_Rect_handlePoints[1]) / 2;          
            Vector3 new_Left = Handles.FreeMoveHandle(old_Left, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);
            if (old_Left != new_Left)
            {
                Undo.RecordObjects(new Object[] { m_textContainer, m_transform }, "Rectangle Dimension Changes");
                Vector3 delta = old_Left - new_Left;
                rectangle.width += delta.x / lossyScale.x;    
                if (!isShiftKey) m_transform.position += m_transform.TransformDirection(new Vector3(-delta.x * (1 - pivot.x), 0, 0));
                
                hasChanged = true;               
            }
         
            // TOP LEFT HANDLE
            Vector3 old_TopLeft = m_Rect_handlePoints[1];
            Vector3 new_TopLeft = Handles.FreeMoveHandle(old_TopLeft, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);
            if (old_TopLeft != new_TopLeft)
            {
                Undo.RecordObjects(new Object[] { m_textContainer, m_transform }, "Rectangle Dimension Changes");
                Vector2 delta = old_TopLeft - new_TopLeft;
                rectangle.width += delta.x / lossyScale.x;
                rectangle.height -= delta.y / lossyScale.y;
                if (!isShiftKey) m_transform.position += m_transform.TransformDirection(new Vector3(-delta.x * (1 - pivot.x), -delta.y * (pivot.y), 0));
               
                hasChanged = true;
            }

            // TOP HANDLE            
            Vector3 old_Top = (m_Rect_handlePoints[1] + m_Rect_handlePoints[2]) / 2;
            Vector3 new_Top = Handles.FreeMoveHandle(old_Top, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);
            if (old_Top != new_Top)
            {
                Undo.RecordObjects(new Object[] { m_textContainer, m_transform }, "Rectangle Dimension Changes");
                Vector2 delta = old_Top - new_Top;
                rectangle.height -= delta.y / lossyScale.y;
                if (!isShiftKey) m_transform.position += m_transform.TransformDirection(new Vector3(0, -delta.y * (pivot.y), 0));
                
                hasChanged = true;
            }

            // TOP RIGHT HANDLE
            Vector3 old_TopRight = m_Rect_handlePoints[2];
            Vector3 new_TopRight = Handles.FreeMoveHandle(old_TopRight, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);
            if (old_TopRight != new_TopRight)
            {
                Undo.RecordObjects(new Object[] { m_textContainer, m_transform }, "Rectangle Dimension Changes");
                Vector2 delta = old_TopRight - new_TopRight;
                rectangle.width -= delta.x / lossyScale.x;
                rectangle.height -= delta.y / lossyScale.y;
                if (!isShiftKey) m_transform.position += m_transform.TransformDirection(new Vector3(-delta.x * pivot.x, -delta.y * (pivot.y), 0));
                    
                hasChanged = true;
            }

            // RIGHT HANDLE            
            Vector3 old_Right = (m_Rect_handlePoints[2] + m_Rect_handlePoints[3]) / 2;
            Vector3 new_Right = Handles.FreeMoveHandle(old_Right, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);
            if (old_Right != new_Right)
            {
                Undo.RecordObjects(new Object[] { m_textContainer, m_transform }, "Rectangle Dimension Changes");
                Vector2 delta = old_Right - new_Right;
                rectangle.width -= delta.x / lossyScale.x;
                if (!isShiftKey) m_transform.position += m_transform.TransformDirection(new Vector3(-delta.x * pivot.x, 0, 0));
               
                hasChanged = true;
            }

            // BOTTOM RIGHT HANDLE
            Vector3 old_BottomRight = m_Rect_handlePoints[3];
            Vector3 new_BottomRight = Handles.FreeMoveHandle(old_BottomRight, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);
            if (old_BottomRight != new_BottomRight)
            {
                Undo.RecordObjects(new Object[] { m_textContainer, m_transform }, "Rectangle Dimension Changes");
                Vector2 delta = old_BottomRight - new_BottomRight;
                rectangle.width -= delta.x / lossyScale.x;
                rectangle.height += delta.y / lossyScale.y;
                if (!isShiftKey) m_transform.position += m_transform.TransformDirection(new Vector3(-delta.x * pivot.x, -delta.y * (1 - pivot.y), 0));
                           
                hasChanged = true;             
            }

            // BOTTOM HANDLE            
            Vector3 old_Bottom = (m_Rect_handlePoints[0] + m_Rect_handlePoints[3]) / 2;
            Vector3 new_Bottom = Handles.FreeMoveHandle(old_Bottom, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);
            if (old_Bottom != new_Bottom)
            {
                Undo.RecordObjects(new Object[] { m_textContainer, m_transform }, "Rectangle Dimension Changes");
                Vector2 delta = old_Bottom - new_Bottom;
                rectangle.height += delta.y / lossyScale.y;
                if (!isShiftKey) m_transform.position += m_transform.TransformDirection(new Vector3(0, -delta.y * (1 - pivot.y), 0));
                
                hasChanged = true;
            }

                           
            //if (evt.type == EventType.mouseUp)
            //    m_mouseDragFlag = 0;

           
            // Margin Frame & Handles               
            Vector4 textMargins = m_textContainer.margins;

            m_Margin_handlePoints[0] = new_BottomLeft + m_transform.TransformDirection(new Vector3(textMargins.x * lossyScale.x, textMargins.w * lossyScale.y, 0)); // BL
            m_Margin_handlePoints[1] = new_TopLeft + m_transform.TransformDirection(new Vector3(textMargins.x * lossyScale.x, - textMargins.y * lossyScale.y, 0)); // TL
            m_Margin_handlePoints[2] = new_TopRight + m_transform.TransformDirection(new Vector3(- textMargins.z * lossyScale.x, - textMargins.y * lossyScale.y, 0)); // TR
            m_Margin_handlePoints[3] = new_BottomRight + m_transform.TransformDirection(new Vector3(- textMargins.z * lossyScale.x,  textMargins.w * lossyScale.y, 0));   // BR

            Handles.color = Color.yellow;
            Handles.DrawSolidRectangleWithOutline(m_Margin_handlePoints, new Color32(255, 255, 255, 0), new Color32(255, 255, 0, 255));

            // Draw & process FreeMoveHandles
            handleSize = HandleUtility.GetHandleSize(rectPos) * 0.05f;
            Handles.color = Color.yellow;

            // LEFT HANDLE
            Vector3 old_left = (m_Margin_handlePoints[0] + m_Margin_handlePoints[1]) * 0.5f;
            Vector3 new_left = Handles.FreeMoveHandle(old_left, Quaternion.identity, handleSize, Vector3.zero, Handles.DotCap);            
            if (old_left != new_left)
            {
                Undo.RecordObject(target, "Margin Changes");
                float delta = old_left.x - new_left.x;                               
                textMargins.x -= delta / lossyScale.x;              
                //Debug.Log("Left Margin H0:" + handlePoints[0] + "  H1:" + handlePoints[1]);
                hasChanged = true;
            }

            // TOP HANDLE
            Vector3 old_top = (m_Margin_handlePoints[1] + m_Margin_handlePoints[2]) * 0.5f;
            Vector3 new_top = Handles.FreeMoveHandle(old_top, Quaternion.identity, handleSize, Vector3.zero, Handles.DotCap);
            if (old_top != new_top)
            {
                Undo.RecordObject(target, "Margin Changes");
                float delta = old_top.y - new_top.y;             
                textMargins.y += delta / lossyScale.y;
                //Debug.Log("Top Margin H1:" + handlePoints[1] + "  H2:" + handlePoints[2]);   
                hasChanged = true;
            }

            // RIGHT HANDLE
            Vector3 old_right = (m_Margin_handlePoints[2] + m_Margin_handlePoints[3]) * 0.5f;
            Vector3 new_right = Handles.FreeMoveHandle(old_right, Quaternion.identity, handleSize, Vector3.zero, Handles.DotCap);
            if (old_right != new_right)
            {
                Undo.RecordObject(target, "Margin Changes");
                float delta = old_right.x - new_right.x;
                textMargins.z += delta / lossyScale.x;               
                hasChanged = true;
                //Debug.Log("Right Margin H2:" + handlePoints[2] + "  H3:" + handlePoints[3]);
            }

            // BOTTOM HANDLE
            Vector3 old_bottom = (m_Margin_handlePoints[3] + m_Margin_handlePoints[0]) * 0.5f;
            Vector3 new_bottom = Handles.FreeMoveHandle(old_bottom, Quaternion.identity, handleSize, Vector3.zero, Handles.DotCap);
            if (old_bottom != new_bottom)
            {
                Undo.RecordObject(target, "Margin Changes");
                float delta = old_bottom.y - new_bottom.y;
                textMargins.w -= delta / lossyScale.y;              
                hasChanged = true;
                //Debug.Log("Bottom Margin H0:" + handlePoints[0] + "  H3:" + handlePoints[3]);
            }

            if (hasChanged)
            {               
                hasChanged = false;
                m_textContainer.rect = rectangle;
                m_textContainer.margins = textMargins;             
                m_textContainer.hasChanged = true;
                EditorUtility.SetDirty(m_transform);
            }
                       
        }



        private void DrawDimensionProperty(SerializedProperty property, string label)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;
                     
            Rect rect = EditorGUILayout.GetControlRect(false, 18);          
            Rect pos0 = new Rect(rect.x, rect.y + 2, rect.width, 18);

            float width = rect.width + 3;
            pos0.width = old_LabelWidth;                       
            GUI.Label(pos0, label);

            Rect rectangle = property.rectValue;
            
            float width_B = width - old_LabelWidth;
            float fieldWidth = width_B / 4;
            pos0.width = fieldWidth - 5;

            pos0.x = old_LabelWidth + 15;
            GUI.Label(pos0, "Width");

            pos0.x += fieldWidth; 
            rectangle.width = EditorGUI.FloatField(pos0, GUIContent.none, rectangle.width);

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Height");

            pos0.x += fieldWidth; 
            rectangle.height = EditorGUI.FloatField(pos0, GUIContent.none, rectangle.height);

            property.rectValue = rectangle;
            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }


        private void DrawMaginProperty(SerializedProperty property, string label)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;
          
            Rect rect = EditorGUILayout.GetControlRect(false, 2 * 18);
            Rect pos0 = new Rect(rect.x, rect.y + 2, rect.width, 18);

            float width = rect.width + 3;
            pos0.width = old_LabelWidth;
            GUI.Label(pos0, label);

            //Vector4 vec = property.vector4Value;
            Vector4 vec = Vector4.zero;
            vec.x = property.FindPropertyRelative("x").floatValue;
            vec.y = property.FindPropertyRelative("y").floatValue;
            vec.z = property.FindPropertyRelative("z").floatValue;
            vec.w = property.FindPropertyRelative("w").floatValue;


            float widthB = width - old_LabelWidth;
            float fieldWidth = widthB / 4;
            pos0.width = fieldWidth - 5;

            // Labels
            pos0.x = old_LabelWidth + 15;
            GUI.Label(pos0, "Left");

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Top");

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Right");

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Bottom");

            pos0.y += 18;

            pos0.x = old_LabelWidth + 15;
            vec.x = EditorGUI.FloatField(pos0, GUIContent.none, vec.x);

            pos0.x += fieldWidth;
            vec.y = EditorGUI.FloatField(pos0, GUIContent.none, vec.y);

            pos0.x += fieldWidth;
            vec.z = EditorGUI.FloatField(pos0, GUIContent.none, vec.z);

            pos0.x += fieldWidth;
            vec.w = EditorGUI.FloatField(pos0, GUIContent.none, vec.w);

            //property.vector4Value = vec;         
            property.FindPropertyRelative("x").floatValue = vec.x;
            property.FindPropertyRelative("y").floatValue = vec.y;
            property.FindPropertyRelative("z").floatValue = vec.z;
            property.FindPropertyRelative("w").floatValue = vec.w;

            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }


        Vector2 GetAnchorPosition(int index)
        {
            Vector2 anchorPosition = Vector2.zero;
                   
            switch (index)
            {
                case 0: // TOP LEFT
                    anchorPosition = new Vector2(0, 1);
                    break;
                case 1: // TOP
                    anchorPosition = new Vector2(0.5f, 1);
                    break;
                case 2: // TOP RIGHT
                    anchorPosition = new Vector2(1, 1);
                    break;
                case 3: // LEFT
                    anchorPosition = new Vector2(0, 0.5f);
                    break;
                case 4: // MIDDLE
                    anchorPosition = new Vector2(0.5f, 0.5f);
                    break;
                case 5: // RIGHT
                    anchorPosition = new Vector2(1, 0.5f);
                    break;
                case 6: // BOTTOM LEFT
                    anchorPosition = new Vector2(0, 0);
                    break;
                case 7: // BOTTOM
                    anchorPosition = new Vector2(0.5f, 0);
                    break;
                case 8: // BOTTOM RIGHT
                    anchorPosition = new Vector2(1, 0);
                    break;
            }
          
            return anchorPosition;
        }


    }
}
