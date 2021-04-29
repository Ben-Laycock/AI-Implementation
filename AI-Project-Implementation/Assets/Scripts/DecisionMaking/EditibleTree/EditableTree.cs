using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EditableTree", order = 1)]
[System.Serializable]
public class EditableTree : ScriptableObject
{

    //Tree Name
    [SerializeField] public string mDecisionTreeName = "Default Decision Tree";

    //Node Storage
    [SerializeField] public List<EditableDecision> mUnconnectedDecisions;
    [SerializeField] public List<EditableAction> mUnconnectedActions;

    [SerializeField] public List<EditableDecision> mConnectedDecisions;
    [SerializeField] public List<EditableAction> mConnectedActions;

    //Tree Root Node
    [SerializeField] public EditableDecision mRoot;

    //Return Decision or Action based on Node ID
    public EditableDecision GetUnconnectedDecisionWithID(int id)
    {
        for (int i = 0; i < mUnconnectedDecisions.Count; i++)
        {
            if (id == mUnconnectedDecisions[i].mWindowID)
            {
                return mUnconnectedDecisions[i];
            }
        }
        return null;
    }

    public EditableAction GetUnconnectedActionWithID(int id)
    {
        for (int i = 0; i < mUnconnectedActions.Count; i++)
        {
            if (id == mUnconnectedActions[i].mWindowID)
            {
                return mUnconnectedActions[i];
            }
        }
        return null;
    }

    public EditableDecision GetConnectedDecisionWithID(int id)
    {
        for (int i = 0; i < mConnectedDecisions.Count; i++)
        {
            if (id == mConnectedDecisions[i].mWindowID)
            {
                return mConnectedDecisions[i];
            }
        }
        return null;
    }

    public EditableAction GetConnectedActionWithID(int id)
    {
        for (int i = 0; i < mConnectedActions.Count; i++)
        {
            if (id == mConnectedActions[i].mWindowID)
            {
                return mConnectedActions[i];
            }
        }
        return null;
    }

    //Remove Functions for Decision and Action Nodes with ID's
    public void RemoveUnconnectedDecisionWithID(int id)
    {
        for (int i = 0; i < mUnconnectedDecisions.Count; i++)
        {
            if (id == mUnconnectedDecisions[i].mWindowID)
            {
                mUnconnectedDecisions.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveUnconnectedActionWithID(int id)
    {
        for (int i = 0; i < mUnconnectedActions.Count; i++)
        {
            if (id == mUnconnectedActions[i].mWindowID)
            {
                mUnconnectedActions.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveConnectedDecisionWithID(int id)
    {
        for (int i = 0; i < mConnectedDecisions.Count; i++)
        {
            if (id == mConnectedDecisions[i].mWindowID)
            {
                mConnectedDecisions.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveConnectedActionWithID(int id)
    {
        for (int i = 0; i < mConnectedActions.Count; i++)
        {
            if (id == mConnectedActions[i].mWindowID)
            {
                mConnectedActions.RemoveAt(i);
                break;
            }
        }
    }

}

[System.Serializable]
public abstract class EditableDecision : ScriptableObject
{

    //DO NOT EDIT
    [SerializeField] public string mEditibleNodeName = "Default Editible Decision Node";
    [SerializeField] public int mWindowID = -1;
    [SerializeField] public Rect mEditableDecisionRect;
    [SerializeField] public EditableDecision mParentDecision = null;

    public EditableDecision TrueNode = null;
    public EditableDecision FalseNode = null;

    public EditableAction TrueAction = null;
    public EditableAction FalseAction = null;

    public virtual void MakeDecision(AgentHandler agentInformation) { }
    
    public void RunChildDecision(AgentHandler agentInformation, bool value)
    {
        if (value)
        {
            if (TrueNode != null)
            {
                TrueNode.MakeDecision(agentInformation);
                return;
            }
            else if (TrueAction != null)
            {
                TrueAction.TakeAction(agentInformation);
                return;
            }
        }
        else
        {
            if (FalseNode != null)
            {
                FalseNode.MakeDecision(agentInformation);
                return;
            }
            else if (FalseAction != null)
            {
                FalseAction.TakeAction(agentInformation);
                return;
            }
        }
        //Debug.Log("No Further Decisions Or Actions!");
    }

    public virtual void MakeDecision(DMTestObjectScript agentInformation) { }

    public void RunChildDecision(DMTestObjectScript agentInformation, bool value)
    {
        if (value)
        {
            if (TrueNode != null)
            {
                TrueNode.MakeDecision(agentInformation);
                return;
            }
            else if (TrueAction != null)
            {
                TrueAction.TakeAction(agentInformation);
                return;
            }
        }
        else
        {
            if (FalseNode != null)
            {
                FalseNode.MakeDecision(agentInformation);
                return;
            }
            else if (FalseAction != null)
            {
                FalseAction.TakeAction(agentInformation);
                return;
            }
        }
        //Debug.Log("No Further Decisions Or Actions!");
    }
}

[System.Serializable]
public abstract class EditableAction : ScriptableObject
{
    //DO NOT EDIT
    [SerializeField] public string mEditibleNodeName = "Default Editible Action Node";
    [SerializeField] public int mWindowID = -1;
    [SerializeField] public Rect mEditableActionRect;
    [SerializeField] public EditableDecision mParentDecision = null;

    public virtual void TakeAction(AgentHandler agentInformation) { }
    public virtual void TakeAction(DMTestObjectScript agentInformation) { }
}