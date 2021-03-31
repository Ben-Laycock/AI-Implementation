using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------//
//--------------------- INFORMATION CLASS ------------------------//
//----------------------------------------------------------------//
public class AgentInformation
{
    public float mAgentHealth { get; set; }
    public float mAgentEnergy { get; set; }
}

//----------------------------------------------------------------//
//---------------------- DECISION CLASSES ------------------------//
//----------------------------------------------------------------//
public abstract class DecisionTree
{
    
    public string mDecisionTreeName = "Default Decision Tree";

    public Decision mRoot;

    public abstract void SetUpDecisionTree();

}

public abstract class Decision
{
    public Decision TrueNode = null;
    public Decision FalseNode = null;

    public Action TrueAction = null;
    public Action FalseAction = null;

    public abstract void MakeDecision(AgentInformation agentInformation);

    public void RunChildDecision(AgentInformation agentInformation, bool value)
    {
        if(value)
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
        Debug.Log("No Further Decisions Or Actions!");
    }

}

public abstract class Action
{
    public abstract void TakeAction(AgentInformation agentInformation);
}