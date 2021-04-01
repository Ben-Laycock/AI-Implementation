using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : FSMState
{
    public FollowPathState()
    {
        mStateID = fsmStateID.FollowPath;
    }

    public override void Reason(GameObject player, AgentHandler agent)
    {
        Vector3 directionToPlayer = GameConstants.Instance.PlayerObject.transform.position - agent.transform.position;
        if (!Physics.Raycast(agent.transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, LayerMask.GetMask("Default")))
        {
            if(directionToPlayer.magnitude <= agent.GetAttackRange())
            {
                agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.ToAttack);
            }
            else
            {
                agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.ToChase);
            }
        }
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        //Follow the Found Path
    }

}

public class AttackState : FSMState
{
    public AttackState()
    {
        mStateID = fsmStateID.AttackPlayer;
    }

    public override void Reason(GameObject player, AgentHandler agent)
    {
        Vector3 directionToPlayer = GameConstants.Instance.PlayerObject.transform.position - agent.transform.position;
        if (Physics.Raycast(agent.transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, LayerMask.GetMask("Default")))
        {
            agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.ToLostPlayer);
        }
        else
        {
            if(directionToPlayer.magnitude >= agent.GetAttackRange())
                agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.ToChase);
        }
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        //Attack the Player
        EditableTree BasicAgentAttackTree = agent.GetBasicAgentDecisionTree();

        if (BasicAgentAttackTree != null)
        {
            BasicAgentAttackTree.mRoot.MakeDecision(agent);
        }
    }

}

public class ChasePlayerState : FSMState
{
    public ChasePlayerState()
    {
        mStateID = fsmStateID.ChasePlayer;
    }

    public override void Reason(GameObject player, AgentHandler agent)
    {
        Vector3 directionToPlayer = GameConstants.Instance.PlayerObject.transform.position - agent.transform.position;
        if (Physics.Raycast(agent.transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, LayerMask.GetMask("Default")))
        {
            agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.ToLostPlayer);
        }
        else
        {
            if (directionToPlayer.magnitude <= agent.GetAttackRange())
                agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.ToAttack);
        }
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        //Chase Player
    }

}