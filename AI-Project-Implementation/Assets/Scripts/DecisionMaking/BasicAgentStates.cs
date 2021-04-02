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
                agent.FSMTransitionPassthrough(fsmTransition.ToAttack);
            }
            else
            {
                agent.FSMTransitionPassthrough(fsmTransition.ToChase);
            }
        }
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        //Follow the Found Path
        agent.BoidController.SetTarget(agent.BoidController.GetManager().GetFlockTarget());
        agent.BoidController.SetShouldFlock(true);

        // Find paths for boids using spatial grid
        // give boids some mechanic that allows them to follow a path (Setting target to furthest visible point?)
        // state machine handles the rest..
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
            agent.FSMTransitionPassthrough(fsmTransition.ToLostPlayer);
        }
        else
        {
            if(directionToPlayer.magnitude >= agent.GetAttackRange())
                agent.FSMTransitionPassthrough(fsmTransition.ToChase);
        }
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        //Attack the Player
        agent.BoidController.SetShouldFlock(false);
        float range = agent.GetTargetTooCloseRange();
        if ((range * range) > (player.transform.position - agent.transform.position).sqrMagnitude)
            agent.BoidController.FleePoint(player.transform.position, 5f);

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
            agent.FSMTransitionPassthrough(fsmTransition.ToLostPlayer);
        }
        else
        {
            if (directionToPlayer.magnitude <= agent.GetAttackRange())
                agent.FSMTransitionPassthrough(fsmTransition.ToAttack);
        }
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        //Chase Player
        agent.BoidController.SetTarget(player);
        agent.BoidController.SetShouldFlock(true);
    }

}