using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

public enum LinkDirection 
{ 
    LinkTrue = 0,
    LinkFalse = 1,
    LinkTrueAction = 2,
    LinkFalseAction = 3
}

public enum LinkTypeToSet
{
    None = 0,
    NodeTrue = 1,
    NodeFalse = 2,
    ActionTrue = 3,
    ActionFalse = 4
}

public enum LinkClearType
{
    NodeTrue = 0,
    NodeFalse = 1,
    ActionTrue = 2,
    ActionFalse = 3,
    All = 4
}

public class DecisionTreeEditorWindow : EditorWindow
{

    private bool OneTimeSetupOnTree = true;
    private bool OneTimeSetupNoRepeat = true;
    
    [Header("Decision Tree To Edit")]
    [SerializeField] private EditableTree mTreeToEdit;

    [Header("Connector Icon")]
    [SerializeField] private Texture2D mDecisionConnectorIcon;
    [SerializeField] private Texture2D mActionConnectorIcon;

    [Header("Connector Colors")]
    [SerializeField] private Color mTrueConnectorColor = new Color(130.0f / 255.0f, 245.0f / 255.0f, 142.0f / 255.0f, 1);
    [SerializeField] private Color mFalseConnectorColor = new Color(252.0f / 255.0f, 124.0f / 255.0f, 124.0f / 255.0f, 1);

    [Header("Node Background Colors")]
    [SerializeField] private Color mRootNodeColor = new Color(181.0f / 255.0f, 181.0f / 255.0f, 252.0f / 255.0f, 1);
    [SerializeField] private Color mTrueDecisionColor = new Color(181.0f / 255.0f, 252.0f / 255.0f, 181.0f / 255.0f, 1);
    [SerializeField] private Color mFalseDecisionColor = new Color(252.0f / 255.0f, 181.0f / 255.0f, 181.0f / 255.0f, 1);
    [SerializeField] private Color mOriginalBackgroundColor;

    List<NodeWindow> mCreateDecisionNodeWindows = new List<NodeWindow>();
    List<NodeWindow> mCreateActionNodeWindows = new List<NodeWindow>();

    float size = 10f;
    int WindowID = 0;

    System.Type[] typesToLoop;

    List<Type> PossibleDecisionTypes;
    List<Type> PossibleActionTypes;

    [SerializeField] public EditableDecision mNodeToSetValueFor = null;
    [SerializeField] public LinkTypeToSet mLinkTypeToSet = LinkTypeToSet.None;

    private Vector2 mScreenOffset = Vector2.zero;
    Vector2 MousePressPosition = Vector2.zero;
    private bool mCanDrag = false;
    private bool mNeedsReset = false;
    private bool mLockResetState = false;

    [MenuItem("Window/DecisionMaking/DecisionTreeEditor")]
    static void ShowEditor()
    {
        int windowWidth = (int)(Screen.currentResolution.width * 0.75f);
        int windowHeight = (int)(Screen.currentResolution.height * 0.75);

        float windowXPosition = (Screen.currentResolution.width - windowWidth) / 2;
        var windowYPosition = (Screen.currentResolution.height - windowHeight) / 2;

        EditorWindow window = GetWindow<DecisionTreeEditorWindow>("Decision Tree Editor");
        window.position = new Rect(windowXPosition, windowYPosition, windowWidth, windowHeight);
    }

    private void Update()
    {
        
    }

    public void EditorQuittingFunction()
    {
        SaveAllNodeChanges();

        EditorUtility.SetDirty(mTreeToEdit);
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
    }

    private void OnLostFocus()
    {
        EditorQuittingFunction();
    }

    void OnGUI()
    {
        if(OneTimeSetupNoRepeat)
        {
            OneTimeSetupNoRepeat = false;
            mOriginalBackgroundColor = GUI.backgroundColor;
            UnityEditor.EditorApplication.quitting += EditorQuittingFunction;
            //UnityEngine.Application.quitting += EditorQuittingFunction;
        }

        //Request Selection of Decision Tree To Edit
        if(mTreeToEdit == null)
        {
            GUILayout.Label("Select A Editable Decision Tree!", EditorStyles.boldLabel);
            return;
        }

        //Check for mouse drag event
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            //Debug.Log("Mouse Pressed!");
            if (!GetNodeRectContainsMouse(Event.current.mousePosition))
            {
                //Debug.Log("Not Contained!");
                MousePressPosition = Event.current.mousePosition;
            }
            else
            {
                //Debug.Log("Contained");
                mNeedsReset = true;
            }
        }

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
            if(mNeedsReset == false)
            {
                if(!mLockResetState)
                {
                    if (!GetNodeRectContainsMouse(Event.current.mousePosition))
                    {
                        mCanDrag = true;
                    }
                    else
                    {
                        mNeedsReset = true;
                    }
                    mLockResetState = true;
                }

                if (mCanDrag)
                {
                    GUI.changed = true;
                    Vector3 MouseDifference = Event.current.mousePosition - MousePressPosition;
                    MousePressPosition = Event.current.mousePosition;
                    mScreenOffset += new Vector2(MouseDifference.x, MouseDifference.y);
                }
            }
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            mCanDrag = false;
            mNeedsReset = false;
            mLockResetState = false;
        }

        //Render Node Graph Editor
        if (GUILayout.Button("Reset Tree"))
        {
            mTreeToEdit.mUnconnectedActions.Clear();
            mTreeToEdit.mUnconnectedDecisions.Clear();
            mTreeToEdit.mConnectedActions.Clear();
            mTreeToEdit.mConnectedDecisions.Clear();

            mTreeToEdit.mRoot = null;

            if(AssetDatabase.IsValidFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName))
            {
                if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName + "/DecisionNodes"))
                {
                    bool success = AssetDatabase.DeleteAsset("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName + "/DecisionNodes");
                }

                if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName + "/ActionNodes"))
                {
                    bool success = AssetDatabase.DeleteAsset("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName + "/ActionNodes");
                }
            }

            EditorUtility.SetDirty(mTreeToEdit);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();

            WindowID = 0;
        }

        if (GUILayout.Button("Reload Tree"))
        {
            AssetDatabase.Refresh();

            WindowID = 0;
            OneTimeSetupOnTree = true;
        }

        if (GUILayout.Button("Save Changes To Tree"))
        {
            SaveAllNodeChanges();

            EditorUtility.SetDirty(mTreeToEdit);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Refactor Tree Layout"))
        {
            if(mTreeToEdit.mRoot != null)
            {
                int TreeDepth = CalculateMaxTreeDepth(mTreeToEdit.mRoot);
                //Debug.Log(TreeDepth);

                mTreeToEdit.mRoot.mEditableDecisionRect.position = new Vector2((position.width / 2) -100, (position.height / 2) - 100);

                RefactorTreeNodePositions(mTreeToEdit.mRoot, TreeDepth);
            } 
        }

        if (OneTimeSetupOnTree)
        {
            WindowID = 0;

            for (int i = 0; i < mTreeToEdit.mConnectedDecisions.Count; i++)
            {
                mTreeToEdit.mConnectedDecisions[i].mWindowID = WindowID;
                WindowID++;
            }

            for (int i = 0; i < mTreeToEdit.mConnectedActions.Count; i++)
            {
                mTreeToEdit.mConnectedActions[i].mWindowID = WindowID;
                WindowID++;
            }

            for (int i = 0; i < mTreeToEdit.mUnconnectedDecisions.Count; i++)
            {
                mTreeToEdit.mUnconnectedDecisions[i].mWindowID = WindowID;
                WindowID++;
            }

            for (int i = 0; i < mTreeToEdit.mUnconnectedActions.Count; i++)
            {
                mTreeToEdit.mUnconnectedActions[i].mWindowID = WindowID;
                WindowID++;
            }

            OneTimeSetupOnTree = false;
        }

        mTreeToEdit.mDecisionTreeName = EditorGUILayout.TextField("Decision Tree Name: ", mTreeToEdit.mDecisionTreeName);

        if (GUILayout.Button("Create Decision Node"))
        {
            PossibleDecisionTypes = new List<Type>();

            //types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            typesToLoop = System.Reflection.Assembly.GetAssembly(typeof(EditableIsHealthTooLow)).GetTypes();

            foreach(Type type in typesToLoop)
            {
                if(type.IsSubclassOf(typeof(EditableDecision)))
                {
                    PossibleDecisionTypes.Add(type);
                }
            }

            //Debug.Log(PossibleDecisionTypes.Count);

            Rect mWindowRect = new Rect((position.width / 2) - (mScreenOffset.x + 100), (position.height / 2) - (mScreenOffset.y + 50), 200, 100);

            NodeWindow newNodeWindow = new NodeWindow(mWindowRect);
            mCreateDecisionNodeWindows.Add(newNodeWindow);
        }

        if (GUILayout.Button("Create Action Node"))
        {
            PossibleActionTypes = new List<Type>();

            typesToLoop = System.Reflection.Assembly.GetAssembly(typeof(EditableTradeEnergyForHealth)).GetTypes();

            foreach (Type type in typesToLoop)
            {
                if (type.IsSubclassOf(typeof(EditableAction)))
                {
                    PossibleActionTypes.Add(type);
                }
            }

            Rect mWindowRect = new Rect((position.width / 2) - (mScreenOffset.x + 100), (position.height / 2) - (mScreenOffset.y + 50), 200, 100);

            NodeWindow newNodeWindow = new NodeWindow(mWindowRect);
            mCreateActionNodeWindows.Add(newNodeWindow);
        }

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        //Render Graph
        int ScreenStartOffsetValue = 150;
        GUI.BeginGroup(new Rect(0, ScreenStartOffsetValue, position.width, position.height));

        //Draw Popup Windows
        BeginWindows();

        mScreenOffset -= new Vector2(0, ScreenStartOffsetValue);

        GUI.backgroundColor = mOriginalBackgroundColor;

        for (int i = 0; i < mCreateDecisionNodeWindows.Count; i++)
        {
            if (mCreateDecisionNodeWindows[i].mWindowID == -1)
            {
                mCreateDecisionNodeWindows[i].mWindowID = WindowID;
                WindowID++;
            }

            mCreateDecisionNodeWindows[i].mWindowRect.position += mScreenOffset;
            mCreateDecisionNodeWindows[i].mWindowRect = GUI.Window(mCreateDecisionNodeWindows[i].mWindowID, mCreateDecisionNodeWindows[i].mWindowRect, DrawCreateDecisionNodeWindow, "Decision Node Creation");
            mCreateDecisionNodeWindows[i].mWindowRect.position -= mScreenOffset;
        }

        for (int i = 0; i < mCreateActionNodeWindows.Count; i++)
        {
            if (mCreateActionNodeWindows[i].mWindowID == -1)
            {
                mCreateActionNodeWindows[i].mWindowID = WindowID;
                WindowID++;
            }

            mCreateActionNodeWindows[i].mWindowRect.position += mScreenOffset;
            mCreateActionNodeWindows[i].mWindowRect = GUI.Window(mCreateActionNodeWindows[i].mWindowID, mCreateActionNodeWindows[i].mWindowRect, DrawCreateActionNodeWindow, "Action Node Creation");
            mCreateActionNodeWindows[i].mWindowRect.position -= mScreenOffset;
        }

        for(int i = 0; i < mTreeToEdit.mUnconnectedDecisions.Count; i++)
        {
            if (mTreeToEdit.mUnconnectedDecisions[i].mWindowID == -1)
            {
                mTreeToEdit.mUnconnectedDecisions[i].mWindowID = WindowID;
                WindowID++;
            }

            GUI.backgroundColor = mOriginalBackgroundColor;

            Vector2 mOriginalPosition = mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect.position;

            mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect.position += mScreenOffset;
            mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect = GUI.Window(mTreeToEdit.mUnconnectedDecisions[i].mWindowID, mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect, DrawUnconnectedDecisions, mTreeToEdit.mUnconnectedDecisions[i].mEditibleNodeName);
            mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect.position -= mScreenOffset;

            ApplyDragMoveToSubNodes(mTreeToEdit.mUnconnectedDecisions[i], (mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect.position - mOriginalPosition));
            DrawNodeRecursive(mTreeToEdit.mUnconnectedDecisions[i]);
        }

        GUI.backgroundColor = mOriginalBackgroundColor;

        for (int i = 0; i < mTreeToEdit.mUnconnectedActions.Count; i++)
        {
            if (mTreeToEdit.mUnconnectedActions[i].mWindowID == -1)
            {
                mTreeToEdit.mUnconnectedActions[i].mWindowID = WindowID;
                WindowID++;
            }

            mTreeToEdit.mUnconnectedActions[i].mEditableActionRect.position += mScreenOffset;
            mTreeToEdit.mUnconnectedActions[i].mEditableActionRect = GUI.Window(mTreeToEdit.mUnconnectedActions[i].mWindowID, mTreeToEdit.mUnconnectedActions[i].mEditableActionRect, DrawUnconnectedActions, mTreeToEdit.mUnconnectedActions[i].mEditibleNodeName);
            mTreeToEdit.mUnconnectedActions[i].mEditableActionRect.position -= mScreenOffset;
        }

        DrawEditableDecisionTree();

        EndWindows();

        mScreenOffset += new Vector2(0, ScreenStartOffsetValue);

        GUI.EndGroup();

        if(GUI.changed) Repaint();

    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        Vector3 newOffset = new Vector3(mScreenOffset.x % gridSpacing, mScreenOffset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height + gridSpacing, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width + gridSpacing, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    void DrawCreateDecisionNodeWindow(int id)
    {
        int MaxPossibleTypes = PossibleDecisionTypes.Count;

        int selected = 0;
        string[] options = new string[MaxPossibleTypes];

        for(int i = 0; i < MaxPossibleTypes; i++)
        {
            options[i] = PossibleDecisionTypes[i].ToString();
        }

        selected = EditorGUILayout.Popup("Node Type", selected, options);

        if (GUILayout.Button("Finalise Creation"))
        {

            if(!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects/DecisionTreeNodes", mTreeToEdit.mDecisionTreeName);
            }

            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName + "/DecisionNodes"))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName, "DecisionNodes");
            }

            EditableDecision NodeToCreate = (EditableDecision)ScriptableObject.CreateInstance(PossibleDecisionTypes[selected]);
            NodeToCreate.mEditibleNodeName = PossibleDecisionTypes[selected].ToString().Remove(0, 8);

            string UniqueLocation = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName + "/DecisionNodes/" + NodeToCreate.mEditibleNodeName + ".asset");
            AssetDatabase.CreateAsset(NodeToCreate, UniqueLocation);

            mTreeToEdit.mUnconnectedDecisions.Add(NodeToCreate);

            //Delete Current Node
            for (int i = 0; i < mCreateDecisionNodeWindows.Count; i++)
            {
                if (id == mCreateDecisionNodeWindows[i].mWindowID)
                {
                    NodeToCreate.mEditableDecisionRect = new Rect(mCreateDecisionNodeWindows[i].mWindowRect.x, mCreateDecisionNodeWindows[i].mWindowRect.y, 200, 200);
                    mCreateDecisionNodeWindows.RemoveAt(i);
                    break;
                }
            }

            EditorUtility.SetDirty(NodeToCreate);
            AssetDatabase.SaveAssets();
        }

        if(GUILayout.Button("Delete"))
        {
            for (int i = 0; i < mCreateDecisionNodeWindows.Count; i++)
            {
                if(id == mCreateDecisionNodeWindows[i].mWindowID)
                {
                    mCreateDecisionNodeWindows.RemoveAt(i);
                    break;
                }
            }
        }

        GUI.DragWindow();
    }

    void DrawCreateActionNodeWindow(int id)
    {
        int MaxPossibleTypes = PossibleActionTypes.Count;

        int selected = 0;
        string[] options = new string[MaxPossibleTypes];

        for (int i = 0; i < MaxPossibleTypes; i++)
        {
            options[i] = PossibleActionTypes[i].ToString();
        }

        selected = EditorGUILayout.Popup("Node Type", selected, options);

        if (GUILayout.Button("Finalise Creation"))
        {

            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects/DecisionTreeNodes", mTreeToEdit.mDecisionTreeName);
            }

            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName + "/ActionNodes"))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName, "ActionNodes");
            }

            EditableAction NodeToCreate = (EditableAction)ScriptableObject.CreateInstance(PossibleActionTypes[selected]);
            NodeToCreate.mEditibleNodeName = PossibleActionTypes[selected].ToString().Remove(0, 8);

            string name = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects/DecisionTreeNodes/" + mTreeToEdit.mDecisionTreeName + "/ActionNodes/" + NodeToCreate.mEditibleNodeName + ".asset");
            AssetDatabase.CreateAsset(NodeToCreate, name);

            mTreeToEdit.mUnconnectedActions.Add(NodeToCreate);

            //Delete Current Node
            for (int i = 0; i < mCreateActionNodeWindows.Count; i++)
            {
                if (id == mCreateActionNodeWindows[i].mWindowID)
                {
                    NodeToCreate.mEditableActionRect = new Rect(mCreateActionNodeWindows[i].mWindowRect.x, mCreateActionNodeWindows[i].mWindowRect.y, 200, 200);
                    mCreateActionNodeWindows.RemoveAt(i);
                    break;
                }
            }

            EditorUtility.SetDirty(NodeToCreate);
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Delete"))
        {
            for (int i = 0; i < mCreateActionNodeWindows.Count; i++)
            {
                if (id == mCreateActionNodeWindows[i].mWindowID)
                {
                    mCreateActionNodeWindows.RemoveAt(i);
                    break;
                }
            }
        }

        GUI.DragWindow();
    }

    void DrawNodeCurve(Rect start, Rect end, LinkDirection direction)
    {
        Vector3 startPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;
        Vector3 startTan = Vector3.zero;
        Vector3 endTan = Vector3.zero;
        Rect endTexturePos = new Rect(0,0,0,0);
        Color colorToUse = mTrueConnectorColor;

        Texture2D textureToUse = mDecisionConnectorIcon;

        switch (direction)
        { 
            case LinkDirection.LinkTrue:
                startPos = new Vector3(start.x, start.y + start.height / 2, 0);
                endPos = new Vector3(end.x + end.width / 2, end.y - size, 0);
                startTan = startPos + Vector3.left * 50;
                endTan = endPos + Vector3.down * 50;
                endTexturePos = new Rect(endPos.x - size / 2, endPos.y, size, size);
                colorToUse = mTrueConnectorColor;
                textureToUse = mDecisionConnectorIcon;
                break;

            case LinkDirection.LinkFalse:
                startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
                endPos = new Vector3(end.x + end.width / 2, end.y - size, 0);
                startTan = startPos + Vector3.right * 50;
                endTan = endPos + Vector3.down * 50;
                endTexturePos = new Rect(endPos.x - size / 2, endPos.y, size, size);
                colorToUse = mFalseConnectorColor;
                textureToUse = mDecisionConnectorIcon;
                break;

            case LinkDirection.LinkTrueAction:
                startPos = new Vector3(start.x, start.y + start.height / 2, 0);
                endPos = new Vector3(end.x + end.width / 2, end.y - size, 0);
                startTan = startPos + Vector3.left * 50;
                endTan = endPos + Vector3.down * 50;
                endTexturePos = new Rect(endPos.x - size / 2, endPos.y, size, size);
                colorToUse = mTrueConnectorColor;
                textureToUse = mActionConnectorIcon;
                break;

            case LinkDirection.LinkFalseAction:
                startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
                endPos = new Vector3(end.x + end.width / 2, end.y - size, 0);
                startTan = startPos + Vector3.right * 50;
                endTan = endPos + Vector3.down * 50;
                endTexturePos = new Rect(endPos.x - size / 2, endPos.y, size, size);
                colorToUse = mFalseConnectorColor;
                textureToUse = mActionConnectorIcon;
                break;

            default:
                break;
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 5);

        var color = GUI.color;
        GUI.color = colorToUse;
        GUI.DrawTexture(endTexturePos, textureToUse, ScaleMode.StretchToFill);
        GUI.color = color;
    }

    void DrawUnconnectedDecisions(int id)
    {
        EditableDecision mCurrentDecision = mTreeToEdit.GetUnconnectedDecisionWithID(id);

        if(mCurrentDecision != null)
        {
            GUILayout.Label(mCurrentDecision.mEditibleNodeName);
        }

        if (GUILayout.Button("Set As Root"))
        {
            if(mTreeToEdit.mRoot == null)
            {
                mTreeToEdit.mRoot = mCurrentDecision;
                mTreeToEdit.mConnectedDecisions.Add(mCurrentDecision);
                mTreeToEdit.RemoveUnconnectedDecisionWithID(id);
            }
            else
            {
                mTreeToEdit.mUnconnectedDecisions.Add(mTreeToEdit.mRoot);
                mTreeToEdit.RemoveConnectedDecisionWithID(mTreeToEdit.mRoot.mWindowID);
                mTreeToEdit.mConnectedDecisions.Add(mCurrentDecision);
                mTreeToEdit.mRoot = mCurrentDecision;
                mTreeToEdit.RemoveUnconnectedDecisionWithID(id);
            }
        }

        if(mNodeToSetValueFor != null && (mLinkTypeToSet == LinkTypeToSet.NodeTrue || mLinkTypeToSet == LinkTypeToSet.NodeFalse))
        {
            if(!GetIsNodeIDInSubTree(mCurrentDecision, mNodeToSetValueFor.mWindowID))
            {
                if (GUILayout.Button("Select"))
                {
                    mTreeToEdit.mConnectedDecisions.Add(mCurrentDecision);
                    if (mLinkTypeToSet == LinkTypeToSet.NodeTrue)
                    {
                        mNodeToSetValueFor.TrueNode = mCurrentDecision;
                    }
                    else if (mLinkTypeToSet == LinkTypeToSet.NodeFalse)
                    {
                        mNodeToSetValueFor.FalseNode = mCurrentDecision;
                    }

                    mCurrentDecision.mParentDecision = mNodeToSetValueFor;

                    mTreeToEdit.RemoveUnconnectedDecisionWithID(id);
                    mNodeToSetValueFor = null;
                    mLinkTypeToSet = LinkTypeToSet.None;
                    return;
                }
            }  
        }

        if (GUILayout.Button("Delete"))
        {
            //Debug.Log(AssetDatabase.GetAssetPath(mTreeToEdit.mUnconnectedDecisions[i]));
            //Debug.Log(AssetDatabase.GetAssetPath(mTreeToEdit.mUnconnectedDecisions[i].GetInstanceID()));
            ClearSubNodeBindingsToParent(mCurrentDecision, LinkClearType.All);

            mTreeToEdit.RemoveUnconnectedDecisionWithID(id);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mCurrentDecision.GetInstanceID()));
        }

        GUI.DragWindow();
    }

    void DrawUnconnectedActions(int id)
    {
        EditableAction mCurrentAction = mTreeToEdit.GetUnconnectedActionWithID(id);;

        if(mCurrentAction != null)
        {
            GUILayout.Label(mCurrentAction.mEditibleNodeName);
        }

        if (mNodeToSetValueFor != null && (mLinkTypeToSet == LinkTypeToSet.ActionTrue || mLinkTypeToSet == LinkTypeToSet.ActionFalse))
        {
            if (GUILayout.Button("Select"))
            {
                mTreeToEdit.mConnectedActions.Add(mCurrentAction);
                if (mLinkTypeToSet == LinkTypeToSet.ActionTrue)
                {
                    mNodeToSetValueFor.TrueAction = mCurrentAction;
                }
                else if (mLinkTypeToSet == LinkTypeToSet.ActionFalse)
                {
                    mNodeToSetValueFor.FalseAction = mCurrentAction;
                }

                mCurrentAction.mParentDecision = mNodeToSetValueFor;

                mTreeToEdit.RemoveUnconnectedActionWithID(id);
                mNodeToSetValueFor = null;
                mLinkTypeToSet = LinkTypeToSet.None;
                return;
            }
        }

        if (GUILayout.Button("Delete"))
        {
            mTreeToEdit.RemoveUnconnectedActionWithID(id);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mCurrentAction.GetInstanceID()));
        }

        GUI.DragWindow();
    }

    void DrawEditableDecisionTree()
    {
        if(mTreeToEdit.mRoot != null)
        {
            GUI.backgroundColor = mRootNodeColor;

            Vector2 mOriginalPosition = mTreeToEdit.mRoot.mEditableDecisionRect.position;

            mTreeToEdit.mRoot.mEditableDecisionRect.position += mScreenOffset;
            mTreeToEdit.mRoot.mEditableDecisionRect = GUI.Window(mTreeToEdit.mRoot.mWindowID, mTreeToEdit.mRoot.mEditableDecisionRect, DrawDecisionNode, "Root Tree Node");
            mTreeToEdit.mRoot.mEditableDecisionRect.position -= mScreenOffset;

            ApplyDragMoveToSubNodes(mTreeToEdit.mRoot, (mTreeToEdit.mRoot.mEditableDecisionRect.position - mOriginalPosition));

            DrawNodeRecursive(mTreeToEdit.mRoot);
        }
    }

    void DrawNodeRecursive(EditableDecision mDecisionToDraw)
    {
        GUI.backgroundColor = mTrueDecisionColor;

        if(mDecisionToDraw.TrueNode != null)
        {
            Vector2 mOriginalPosition = mDecisionToDraw.TrueNode.mEditableDecisionRect.position;

            mDecisionToDraw.TrueNode.mEditableDecisionRect.position += mScreenOffset;
            mDecisionToDraw.mEditableDecisionRect.position += mScreenOffset;

            mDecisionToDraw.TrueNode.mEditableDecisionRect = GUI.Window(mDecisionToDraw.TrueNode.mWindowID, mDecisionToDraw.TrueNode.mEditableDecisionRect, DrawDecisionNode, "True Decision");
            DrawNodeCurve(mDecisionToDraw.mEditableDecisionRect, mDecisionToDraw.TrueNode.mEditableDecisionRect, LinkDirection.LinkTrue);

            mDecisionToDraw.mEditableDecisionRect.position -= mScreenOffset;
            mDecisionToDraw.TrueNode.mEditableDecisionRect.position -= mScreenOffset;

            ApplyDragMoveToSubNodes(mDecisionToDraw.TrueNode, (mDecisionToDraw.TrueNode.mEditableDecisionRect.position - mOriginalPosition));
            DrawNodeRecursive(mDecisionToDraw.TrueNode);
        }

        GUI.backgroundColor = mFalseDecisionColor;

        if (mDecisionToDraw.FalseNode != null)
        {
            Vector2 mOriginalPosition = mDecisionToDraw.FalseNode.mEditableDecisionRect.position;

            mDecisionToDraw.FalseNode.mEditableDecisionRect.position += mScreenOffset;
            mDecisionToDraw.mEditableDecisionRect.position += mScreenOffset;

            mDecisionToDraw.FalseNode.mEditableDecisionRect = GUI.Window(mDecisionToDraw.FalseNode.mWindowID, mDecisionToDraw.FalseNode.mEditableDecisionRect, DrawDecisionNode, "False Decision");
            DrawNodeCurve(mDecisionToDraw.mEditableDecisionRect, mDecisionToDraw.FalseNode.mEditableDecisionRect, LinkDirection.LinkFalse);

            mDecisionToDraw.mEditableDecisionRect.position -= mScreenOffset;
            mDecisionToDraw.FalseNode.mEditableDecisionRect.position -= mScreenOffset;

            ApplyDragMoveToSubNodes(mDecisionToDraw.FalseNode, (mDecisionToDraw.FalseNode.mEditableDecisionRect.position - mOriginalPosition));
            DrawNodeRecursive(mDecisionToDraw.FalseNode);
        }

        GUI.backgroundColor = mTrueDecisionColor;

        if (mDecisionToDraw.TrueAction != null)
        {
            mDecisionToDraw.TrueAction.mEditableActionRect.position += mScreenOffset;
            mDecisionToDraw.mEditableDecisionRect.position += mScreenOffset;

            mDecisionToDraw.TrueAction.mEditableActionRect = GUI.Window(mDecisionToDraw.TrueAction.mWindowID, mDecisionToDraw.TrueAction.mEditableActionRect, DrawActionNode, "True Action");
            DrawNodeCurve(mDecisionToDraw.mEditableDecisionRect, mDecisionToDraw.TrueAction.mEditableActionRect, LinkDirection.LinkTrueAction);

            mDecisionToDraw.mEditableDecisionRect.position -= mScreenOffset;
            mDecisionToDraw.TrueAction.mEditableActionRect.position -= mScreenOffset;
        }

        GUI.backgroundColor = mFalseDecisionColor;

        if (mDecisionToDraw.FalseAction != null)
        {
            mDecisionToDraw.FalseAction.mEditableActionRect.position += mScreenOffset;
            mDecisionToDraw.mEditableDecisionRect.position += mScreenOffset;

            mDecisionToDraw.FalseAction.mEditableActionRect = GUI.Window(mDecisionToDraw.FalseAction.mWindowID, mDecisionToDraw.FalseAction.mEditableActionRect, DrawActionNode, "False Action");
            
            DrawNodeCurve(mDecisionToDraw.mEditableDecisionRect, mDecisionToDraw.FalseAction.mEditableActionRect, LinkDirection.LinkFalseAction);

            mDecisionToDraw.mEditableDecisionRect.position -= mScreenOffset;
            mDecisionToDraw.FalseAction.mEditableActionRect.position -= mScreenOffset;
        }
    }

    void DrawDecisionNode(int id)
    {

        EditableDecision mCurrentDecision = mTreeToEdit.GetConnectedDecisionWithID(id);

        if(mCurrentDecision != null)
        {
            GUILayout.Label(mCurrentDecision.mEditibleNodeName);

            bool mShowConnectorOptions = false;

            if(mNodeToSetValueFor == null)
            {
                mShowConnectorOptions = true;
            }
            else
            {
                if (mNodeToSetValueFor.mWindowID != mCurrentDecision.mWindowID)
                {
                    mShowConnectorOptions = true;
                }
            }

            if(mShowConnectorOptions)
            {
                if(mCurrentDecision.mParentDecision != null)
                {
                    if (GUILayout.Button("Disconnect from Parent"))
                    {
                        ClearBindingToParent(mCurrentDecision);
                        mTreeToEdit.RemoveConnectedDecisionWithID(id);
                        mTreeToEdit.mUnconnectedDecisions.Add(mCurrentDecision);
                    }
                }

                //Setting Value for true node
                if(mCurrentDecision.TrueNode == null && mCurrentDecision.TrueAction == null)
                { 
                    if (GUILayout.Button("Set True Decision"))
                    {
                        mLinkTypeToSet = LinkTypeToSet.NodeTrue;
                        mNodeToSetValueFor = mCurrentDecision;
                    }
                    if (GUILayout.Button("Set True Action"))
                    {
                        mLinkTypeToSet = LinkTypeToSet.ActionTrue;
                        mNodeToSetValueFor = mCurrentDecision;
                    }
                }
                else
                {
                    if(mCurrentDecision.TrueNode != null)
                    {
                        if (GUILayout.Button("Disconnect True Decision"))
                        {
                            ClearSubNodeBindingsToParent(mCurrentDecision, LinkClearType.NodeTrue);
                        }
                    }
                    if(mCurrentDecision.TrueAction != null)
                    {
                        if (GUILayout.Button("Disconnect True Action"))
                        {
                            ClearSubNodeBindingsToParent(mCurrentDecision, LinkClearType.ActionTrue);
                        }
                    }
                }

                //Setting Value for false node
                if (mCurrentDecision.FalseNode == null && mCurrentDecision.FalseAction == null)
                {
                    if (GUILayout.Button("Set False Decision"))
                    {
                        mLinkTypeToSet = LinkTypeToSet.NodeFalse;
                        mNodeToSetValueFor = mCurrentDecision;
                    }
                    if (GUILayout.Button("Set False Action"))
                    {
                        mLinkTypeToSet = LinkTypeToSet.ActionFalse;
                        mNodeToSetValueFor = mCurrentDecision;
                    }
                }
                else
                {
                    if (mCurrentDecision.FalseNode != null)
                    {
                        if (GUILayout.Button("Disconnect False Decision"))
                        {
                            ClearSubNodeBindingsToParent(mCurrentDecision, LinkClearType.NodeFalse);
                        }
                    }
                    if (mCurrentDecision.FalseAction != null)
                    {
                        if (GUILayout.Button("Disconnect False Action"))
                        {
                            ClearSubNodeBindingsToParent(mCurrentDecision, LinkClearType.ActionFalse);
                        }
                    }
                }

                //Show Overrides
                if ((mLinkTypeToSet == LinkTypeToSet.NodeTrue || mLinkTypeToSet == LinkTypeToSet.NodeFalse))
                {
                    //Check if the Current Node is the Root Node - If so, do not allow parent override. Root cannot have a parent node
                    bool AttemptingToOverrideRootNode = false;

                    if(mTreeToEdit.mRoot != null)
                    {
                        if((mCurrentDecision.mWindowID == mTreeToEdit.mRoot.mWindowID))
                        {
                            AttemptingToOverrideRootNode = true;
                        }
                    }

                    //Continue past if the node is not the root node
                    if(mNodeToSetValueFor.mParentDecision != null && !AttemptingToOverrideRootNode)
                    {
                        if(mNodeToSetValueFor.mParentDecision.mWindowID == mCurrentDecision.mWindowID)
                        {
                            if (GUILayout.Button("Override Parent Link"))
                            {
                                ClearBindingToParent(mCurrentDecision);

                                if(mCurrentDecision.TrueNode != null)
                                {
                                    if(mCurrentDecision.TrueNode.mWindowID == mNodeToSetValueFor.mWindowID)
                                    {
                                        mTreeToEdit.RemoveConnectedDecisionWithID(mCurrentDecision.TrueNode.mWindowID);
                                        mTreeToEdit.mUnconnectedDecisions.Add(mCurrentDecision.TrueNode);

                                        mCurrentDecision.TrueNode.mParentDecision = null;
                                        mCurrentDecision.TrueNode = null;
                                    }
                                }

                                if (mCurrentDecision.FalseNode != null)
                                {
                                    if (mCurrentDecision.FalseNode.mWindowID == mNodeToSetValueFor.mWindowID)
                                    {
                                        mTreeToEdit.RemoveConnectedDecisionWithID(mCurrentDecision.FalseNode.mWindowID);
                                        mTreeToEdit.mUnconnectedDecisions.Add(mCurrentDecision.FalseNode);

                                        mCurrentDecision.FalseNode.mParentDecision = null;
                                        mCurrentDecision.FalseNode = null;
                                    }
                                }

                                if (mLinkTypeToSet == LinkTypeToSet.NodeTrue)
                                {
                                    mNodeToSetValueFor.TrueNode = mCurrentDecision;
                                }
                                else if (mLinkTypeToSet == LinkTypeToSet.NodeFalse)
                                {
                                    mNodeToSetValueFor.FalseNode = mCurrentDecision;
                                }

                                mCurrentDecision.mParentDecision = mNodeToSetValueFor;

                                mLinkTypeToSet = LinkTypeToSet.None;
                                mNodeToSetValueFor = null;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Override Parent Link"))
                            {
                                ClearBindingToParent(mCurrentDecision);

                                if (mLinkTypeToSet == LinkTypeToSet.NodeTrue)
                                {
                                    mNodeToSetValueFor.TrueNode = mCurrentDecision;
                                }
                                else if (mLinkTypeToSet == LinkTypeToSet.NodeFalse)
                                {
                                    mNodeToSetValueFor.FalseNode = mCurrentDecision;
                                }

                                mCurrentDecision.mParentDecision = mNodeToSetValueFor;

                                mLinkTypeToSet = LinkTypeToSet.None;
                                mNodeToSetValueFor = null;
                            }
                        }
                    }   
                }
            }
            else
            {
                if (GUILayout.Button("Cancel"))
                {
                    mLinkTypeToSet = LinkTypeToSet.None;
                    mNodeToSetValueFor = null;
                }
            }
        }

        if (GUILayout.Button("Delete"))
        {
            ClearSubNodeBindingsToParent(mCurrentDecision, LinkClearType.All);
            ClearBindingToParent(mCurrentDecision);

            mTreeToEdit.RemoveConnectedDecisionWithID(id);

            if(mTreeToEdit.mRoot != null)
            {
                if(mTreeToEdit.mRoot.mWindowID == mCurrentDecision.mWindowID)
                {
                    mTreeToEdit.mRoot = null;
                }
            }

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mCurrentDecision.GetInstanceID()));
        }

        GUI.DragWindow();
    }

    void DrawActionNode(int id)
    {
        EditableAction mCurrentAction = mTreeToEdit.GetConnectedActionWithID(id);

        if (mCurrentAction != null)
        {
            GUILayout.Label(mCurrentAction.mEditibleNodeName);

            if (mCurrentAction.mParentDecision != null)
            {
                if (GUILayout.Button("Disconnect from Parent"))
                {
                    ClearBindingToParent(mCurrentAction);
                    mTreeToEdit.RemoveConnectedActionWithID(id);
                    mTreeToEdit.mUnconnectedActions.Add(mCurrentAction);
                }
            }

            if (mNodeToSetValueFor != null)
            {
                //Overrides
                if (mLinkTypeToSet == LinkTypeToSet.ActionTrue || mLinkTypeToSet == LinkTypeToSet.ActionFalse)
                {
                    if (GUILayout.Button("Override Parent Link"))
                    {
                        ClearBindingToParent(mCurrentAction);

                        if (mLinkTypeToSet == LinkTypeToSet.ActionTrue)
                        {
                            mNodeToSetValueFor.TrueAction = mCurrentAction;
                        }
                        else if (mLinkTypeToSet == LinkTypeToSet.ActionFalse)
                        {
                            mNodeToSetValueFor.FalseAction = mCurrentAction;
                        }

                        mCurrentAction.mParentDecision = mNodeToSetValueFor;

                        mLinkTypeToSet = LinkTypeToSet.None;
                        mNodeToSetValueFor = null;
                    }
                }
            }
        }

        if (GUILayout.Button("Delete"))
        {
            ClearBindingToParent(mCurrentAction);

            mTreeToEdit.RemoveConnectedActionWithID(id);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mCurrentAction.GetInstanceID()));
        }

        GUI.DragWindow();
    }

    //Utility Functions For Node Control
    public void ClearBindingToParent(EditableDecision nodeToUnbind)
    {
        if(nodeToUnbind.mParentDecision != null)
        {
            if(nodeToUnbind.mParentDecision.TrueNode != null)
            {
                if(nodeToUnbind.mParentDecision.TrueNode.mWindowID == nodeToUnbind.mWindowID)
                {
                    nodeToUnbind.mParentDecision.TrueNode = null;   
                }
            }
            if (nodeToUnbind.mParentDecision.FalseNode != null)
            {
                if (nodeToUnbind.mParentDecision.FalseNode.mWindowID == nodeToUnbind.mWindowID)
                {
                    nodeToUnbind.mParentDecision.FalseNode = null;
                }
            }
            nodeToUnbind.mParentDecision = null;
        }
    }

    public void ClearBindingToParent(EditableAction nodeToUnbind)
    {
        if (nodeToUnbind.mParentDecision != null)
        {
            if (nodeToUnbind.mParentDecision.TrueAction != null)
            {
                if (nodeToUnbind.mParentDecision.TrueAction.mWindowID == nodeToUnbind.mWindowID)
                {
                    nodeToUnbind.mParentDecision.TrueAction = null;
                }
            }
            if (nodeToUnbind.mParentDecision.FalseAction != null)
            {
                if (nodeToUnbind.mParentDecision.FalseAction.mWindowID == nodeToUnbind.mWindowID)
                {
                    nodeToUnbind.mParentDecision.FalseAction = null;
                }
            }
            nodeToUnbind.mParentDecision = null;
        }
    }

    //Manages Editable Tree Lists
    public void ClearSubNodeBindingsToParent(EditableDecision editableDecision, LinkClearType clearType)
    {
        if(editableDecision.TrueNode != null && (clearType == LinkClearType.NodeTrue || clearType == LinkClearType.All))
        {
            editableDecision.TrueNode.mParentDecision = null;

            mTreeToEdit.RemoveConnectedDecisionWithID(editableDecision.TrueNode.mWindowID);
            mTreeToEdit.mUnconnectedDecisions.Add(editableDecision.TrueNode);

            editableDecision.TrueNode = null;
        }

        if (editableDecision.FalseNode != null && (clearType == LinkClearType.NodeFalse || clearType == LinkClearType.All))
        {
            editableDecision.FalseNode.mParentDecision = null;

            mTreeToEdit.RemoveConnectedDecisionWithID(editableDecision.FalseNode.mWindowID);
            mTreeToEdit.mUnconnectedDecisions.Add(editableDecision.FalseNode);

            editableDecision.FalseNode = null;
        }

        if (editableDecision.TrueAction != null && (clearType == LinkClearType.ActionTrue || clearType == LinkClearType.All))
        {
            editableDecision.TrueAction.mParentDecision = null;

            mTreeToEdit.RemoveConnectedActionWithID(editableDecision.TrueAction.mWindowID);
            mTreeToEdit.mUnconnectedActions.Add(editableDecision.TrueAction);

            editableDecision.TrueAction = null;
        }

        if (editableDecision.FalseAction != null && (clearType == LinkClearType.ActionFalse || clearType == LinkClearType.All))
        {
            editableDecision.FalseAction.mParentDecision = null;

            mTreeToEdit.RemoveConnectedActionWithID(editableDecision.FalseAction.mWindowID);
            mTreeToEdit.mUnconnectedActions.Add(editableDecision.FalseAction);

            editableDecision.FalseAction = null;
        }
    }

    public void SaveAllNodeChanges()
    {
        for(int i = 0; i < mTreeToEdit.mUnconnectedDecisions.Count; i++)
        {
            EditorUtility.SetDirty(mTreeToEdit.mUnconnectedDecisions[i]);
        }

        for (int i = 0; i < mTreeToEdit.mUnconnectedActions.Count; i++)
        {
            EditorUtility.SetDirty(mTreeToEdit.mUnconnectedActions[i]);
        }

        for (int i = 0; i < mTreeToEdit.mConnectedDecisions.Count; i++)
        {
            EditorUtility.SetDirty(mTreeToEdit.mConnectedDecisions[i]);
        }

        for (int i = 0; i < mTreeToEdit.mConnectedActions.Count; i++)
        {
            EditorUtility.SetDirty(mTreeToEdit.mConnectedActions[i]);
        }

        AssetDatabase.SaveAssets();
    }

    public bool GetIsNodeIDInSubTree(EditableDecision editableDecision, int idToCheckFor)
    {
        bool ValueToReturn = false;

        if(editableDecision.TrueNode != null)
        {
            if(editableDecision.TrueNode.mWindowID == idToCheckFor)
            {
                ValueToReturn = true;
            }
            else
            {
                ValueToReturn = GetIsNodeIDInSubTree(editableDecision.TrueNode, idToCheckFor);
            }
        }

        if(ValueToReturn) { return true; }

        if (editableDecision.FalseNode != null)
        {
            if (editableDecision.FalseNode.mWindowID == idToCheckFor)
            {
                ValueToReturn = true;
            }
            else
            {
                ValueToReturn = GetIsNodeIDInSubTree(editableDecision.FalseNode, idToCheckFor);
            }
        }

        return ValueToReturn;

    }

    public void ApplyDragMoveToSubNodes(EditableDecision editableDecision, Vector2 mDragChangeValue)
    {
        if (editableDecision.TrueNode != null)
        {
            editableDecision.TrueNode.mEditableDecisionRect.position += mDragChangeValue;
            ApplyDragMoveToSubNodes(editableDecision.TrueNode, mDragChangeValue);
        }

        if (editableDecision.FalseNode != null)
        {
            editableDecision.FalseNode.mEditableDecisionRect.position += mDragChangeValue;
            ApplyDragMoveToSubNodes(editableDecision.FalseNode, mDragChangeValue);
        }

        if (editableDecision.TrueAction != null)
        {
            editableDecision.TrueAction.mEditableActionRect.position += mDragChangeValue;
        }

        if (editableDecision.FalseAction != null)
        {
            editableDecision.FalseAction.mEditableActionRect.position += mDragChangeValue;
        }
    }

    public bool GetNodeRectContainsMouse(Vector2 positionToCheck)
    {
        bool printInfo = false;

        for (int i = 0; i < mCreateDecisionNodeWindows.Count; i++)
        {
            mCreateDecisionNodeWindows[i].mWindowRect.position += mScreenOffset;
            if (mCreateDecisionNodeWindows[i].mWindowRect.Contains(positionToCheck))
            {
                mCreateDecisionNodeWindows[i].mWindowRect.position -= mScreenOffset;
                if(printInfo)
                    Debug.Log("Create Decision Window: " + mCreateDecisionNodeWindows[i].mWindowID);
                return true;
            }
            mCreateDecisionNodeWindows[i].mWindowRect.position -= mScreenOffset;
        }

        for (int i = 0; i < mCreateActionNodeWindows.Count; i++)
        { 
            mCreateActionNodeWindows[i].mWindowRect.position += mScreenOffset;
            if (mCreateActionNodeWindows[i].mWindowRect.Contains(positionToCheck))
            {
                mCreateActionNodeWindows[i].mWindowRect.position -= mScreenOffset;
                if(printInfo)
                    Debug.Log("Create Action Window: " + mCreateActionNodeWindows[i].mWindowID);
                return true;
            }
            mCreateActionNodeWindows[i].mWindowRect.position -= mScreenOffset;
        }

        for (int i = 0; i < mTreeToEdit.mUnconnectedDecisions.Count; i++)
        {
            mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect.position += mScreenOffset;
            if (mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect.Contains(positionToCheck))
            {
                mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect.position -= mScreenOffset;
                if(printInfo)
                    Debug.Log("Unconnected Decision: " + mTreeToEdit.mUnconnectedDecisions[i].mEditibleNodeName);
                return true;
            }
            mTreeToEdit.mUnconnectedDecisions[i].mEditableDecisionRect.position -= mScreenOffset;
        }

        for (int i = 0; i < mTreeToEdit.mUnconnectedActions.Count; i++)
        {
            mTreeToEdit.mUnconnectedActions[i].mEditableActionRect.position += mScreenOffset;
            if (mTreeToEdit.mUnconnectedActions[i].mEditableActionRect.Contains(positionToCheck))
            {
                mTreeToEdit.mUnconnectedActions[i].mEditableActionRect.position -= mScreenOffset;
                if(printInfo)
                    Debug.Log("Unconnected Action: " + mTreeToEdit.mUnconnectedActions[i].mEditibleNodeName);
                return true;
            }
            mTreeToEdit.mUnconnectedActions[i].mEditableActionRect.position -= mScreenOffset;
        }

        for (int i = 0; i < mTreeToEdit.mConnectedDecisions.Count; i++)
        {
            mTreeToEdit.mConnectedDecisions[i].mEditableDecisionRect.position += mScreenOffset;
            if (mTreeToEdit.mConnectedDecisions[i].mEditableDecisionRect.Contains(positionToCheck))
            {
                mTreeToEdit.mConnectedDecisions[i].mEditableDecisionRect.position -= mScreenOffset;
                if(printInfo)
                    Debug.Log("Connected Decision: " + mTreeToEdit.mConnectedDecisions[i].mEditibleNodeName);
                return true;
            }
            mTreeToEdit.mConnectedDecisions[i].mEditableDecisionRect.position -= mScreenOffset;
        }

        for (int i = 0; i < mTreeToEdit.mConnectedActions.Count; i++)
        {
            mTreeToEdit.mConnectedActions[i].mEditableActionRect.position += mScreenOffset;
            if (mTreeToEdit.mConnectedActions[i].mEditableActionRect.Contains(positionToCheck))
            {
                mTreeToEdit.mConnectedActions[i].mEditableActionRect.position -= mScreenOffset;
                if(printInfo)
                    Debug.Log("Connected Action: " + mTreeToEdit.mConnectedActions[i].mEditibleNodeName);
                return true;
            }
            mTreeToEdit.mConnectedActions[i].mEditableActionRect.position -= mScreenOffset;
        }

        return false;
    }

    public int CalculateMaxTreeDepth(EditableDecision editableDecision)
    {
        if(editableDecision == null)
        {
            return 0;
        }

        if(editableDecision.TrueNode != null && editableDecision.FalseNode != null)
        {
            return Mathf.Max(1 + CalculateMaxTreeDepth(editableDecision.TrueNode), 1 + CalculateMaxTreeDepth(editableDecision.FalseNode));
        }

        if(editableDecision.TrueNode != null)
        {
            return 1 + CalculateMaxTreeDepth(editableDecision.TrueNode);
        }

        if (editableDecision.FalseNode != null)
        {
            return 1 + CalculateMaxTreeDepth(editableDecision.FalseNode);
        }

        if (editableDecision.TrueAction != null)
        {
            return 1;
        }

        if (editableDecision.FalseAction != null)
        {
            return 1;
        }

        return 0;
    }

    public void RefactorTreeNodePositions(EditableDecision editableDecision, int TreeDepthToAdjustFor)
    {
        if(editableDecision.TrueNode != null)
        {
            int spaceToApply;

            if(TreeDepthToAdjustFor >= 2)
            {
                int amountToRemove = TreeDepthToAdjustFor - 1;
                spaceToApply = ((int)Mathf.Pow(2, TreeDepthToAdjustFor) * 100) - (amountToRemove * 100);
            }
            else
            {
                spaceToApply = 200;
            }

            editableDecision.TrueNode.mEditableDecisionRect.position = editableDecision.mEditableDecisionRect.position - new Vector2(spaceToApply, -200);

            RefactorTreeNodePositions(editableDecision.TrueNode, TreeDepthToAdjustFor - 1);
        }

        if (editableDecision.FalseNode != null)
        {
            int spaceToApply;

            if (TreeDepthToAdjustFor >= 2)
            {
                int amountToRemove = TreeDepthToAdjustFor - 1;
                spaceToApply = ((int)Mathf.Pow(2, TreeDepthToAdjustFor) * 100) - (amountToRemove * 100);
            }
            else
            {
                spaceToApply = 200;
            }

            editableDecision.FalseNode.mEditableDecisionRect.position = editableDecision.mEditableDecisionRect.position + new Vector2(spaceToApply, 200);

            RefactorTreeNodePositions(editableDecision.FalseNode, TreeDepthToAdjustFor - 1);
        }

        if (editableDecision.TrueAction != null)
        {
            editableDecision.TrueAction.mEditableActionRect.position = editableDecision.mEditableDecisionRect.position - new Vector2(200, -200);
        }

        if (editableDecision.FalseAction != null)
        {
            editableDecision.FalseAction.mEditableActionRect.position = editableDecision.mEditableDecisionRect.position + new Vector2(200, 200);
        }
    }

    public void SaveAssetChanges(EditableDecision editableDecision)
    {
        EditorUtility.SetDirty(editableDecision);
        AssetDatabase.SaveAssets();
    }

    public void SaveAssetChanges(EditableAction editableAction)
    {
        EditorUtility.SetDirty(editableAction);
        AssetDatabase.SaveAssets();
    }

}

class NodeWindow
{
 
    public Rect mWindowRect;
    public int mWindowID = -1;

    public NodeWindow()
    {
        mWindowRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100);
    }

    public NodeWindow(Rect windowRect)
    {
        mWindowRect = windowRect;
    }

}