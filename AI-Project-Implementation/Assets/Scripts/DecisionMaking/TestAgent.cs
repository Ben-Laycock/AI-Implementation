using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------//
//----------------------- USE OF DECISION ------------------------//
//----------------------------------------------------------------//
public class TestAgent : MonoBehaviour
{

    private BasicEnemyDecisionTree mAgentDecisionTree;

    private void Start()
    {
        mAgentDecisionTree = new BasicEnemyDecisionTree("Basic Enemy Decision Tree");
        mAgentDecisionTree.SetUpDecisionTree();

        AgentInformation agentInformation = new AgentInformation();
        agentInformation.mAgentHealth = 5;
        agentInformation.mAgentEnergy = 20;

        Debug.Log("Agent Health: " + agentInformation.mAgentHealth + "        Agent Energy: " + agentInformation.mAgentEnergy);

        mAgentDecisionTree.mRoot.MakeDecision(agentInformation);

        Debug.Log("Agent Health: " + agentInformation.mAgentHealth + "        Agent Energy: " + agentInformation.mAgentEnergy);

        mAgentDecisionTree.mRoot.MakeDecision(agentInformation);

        Debug.Log("Agent Health: " + agentInformation.mAgentHealth + "        Agent Energy: " + agentInformation.mAgentEnergy);
    }

    public BasicEnemyDecisionTree GetBasicEnemyDecisionTree()
    {
        return mAgentDecisionTree;
    }

}

//----------------------------------------------------------------//
//------------------ DECISION IMPLEMENTATIONS --------------------//
//----------------------------------------------------------------//
public class BasicEnemyDecisionTree : DecisionTree
{
    public BasicEnemyDecisionTree(string mName = "Default Decision Tree")
    {
        mDecisionTreeName = mName;
    }

    public override void SetUpDecisionTree()
    {
        var IsHealthTooLowDecision = new IsHealthTooLow();
        var IsThereEnoughEnergyForHealthSwapDecision = new IsThereEnoughEnergyForHealthSwap();
        var TradeEnergyForHealthAction = new TradeEnergyForHealth();

        mRoot = IsHealthTooLowDecision;

        IsHealthTooLowDecision.TrueNode = IsThereEnoughEnergyForHealthSwapDecision;

        IsThereEnoughEnergyForHealthSwapDecision.TrueAction = TradeEnergyForHealthAction;
    }
}

public class IsHealthTooLow : Decision
{
    public override void MakeDecision(AgentInformation agentInformation)
    {
        if (agentInformation.mAgentHealth < 10)
        {
            RunChildDecision(agentInformation, true);
        }
        else
        {
            RunChildDecision(agentInformation, false);
        }
    }
}

public class IsThereEnoughEnergyForHealthSwap : Decision
{
    public override void MakeDecision(AgentInformation agentInformation)
    {
        if (agentInformation.mAgentEnergy > 15)
        {
            RunChildDecision(agentInformation, true);
        }
        else
        {
            RunChildDecision(agentInformation, false);
        }
    }
}

//----------------------------------------------------------------//
//------------------- ACTION IMPLEMENTATIONS ---------------------//
//----------------------------------------------------------------//
public class TradeEnergyForHealth : Action
{
    public override void TakeAction(AgentInformation agentInformation)
    {
        Debug.Log("Trading Energy For Health!");
        agentInformation.mAgentEnergy -= 10;
        agentInformation.mAgentHealth += 10;
    }
}