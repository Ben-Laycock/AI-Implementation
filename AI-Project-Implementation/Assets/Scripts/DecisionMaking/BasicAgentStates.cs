using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : FSMState
{
    AgentHandler mAgent = null;
    List<Vector3Int> mPathToFollow = null;
    List<Vector3> mCondensedPath = null;

    public FollowPathState()
    {
        mPathToFollow = new List<Vector3Int>();
        mCondensedPath = new List<Vector3>();
        mStateID = fsmStateID.FollowPath;
    }

    public override void DoBeforeEntering()
    {
        CalculateNewPath();
    }

    public override void DoBeforeLeaving()
    {
        if (mPathToFollow != null) mPathToFollow.Clear();
        if (mCondensedPath != null) mCondensedPath.Clear();
        mAgent.BoidController.SetTarget(GameConstants.Instance.PlayerObject.transform.position);
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
        agent.mState = "Path";
        float distanceThresholdSqrd = 25f;
        //Follow the Found Path
        if (mCondensedPath != null && mCondensedPath.Count > 0)
        {
            if (Vector3.SqrMagnitude(agent.transform.position - agent.BoidController.GetTarget()) < distanceThresholdSqrd)
            {
                int furthestVisisbleIndex = PathHelpers.FindFurthestVisiblePointIndex(mCondensedPath, agent.transform.position, LayerMask.GetMask("Default"));
                agent.BoidController.SetTarget(mCondensedPath[furthestVisisbleIndex]);
                if (Vector3.Angle(agent.transform.forward, (mCondensedPath[furthestVisisbleIndex] - agent.transform.position)) > 30)
                {
                    agent.transform.forward = (mCondensedPath[furthestVisisbleIndex] - agent.transform.position);
                }
            }
            if (Vector3.SqrMagnitude(agent.transform.position - mCondensedPath[mCondensedPath.Count-1]) < distanceThresholdSqrd)
            {
                agent.BoidController.SetTarget(player.transform.position);
                CalculateNewPath();
            }
        }
        agent.BoidController.SetShouldFlock(true);

        // Find paths for boids using spatial grid
        // give boids some mechanic that allows them to follow a path (Setting target to furthest visible point?)
        // state machine handles the rest..
    }

    public void SetAgentHandler(AgentHandler argAgentHandler)
    {
        mAgent = argAgentHandler;
    }

    void CalculateNewPath()
    {
        if (mPathToFollow == null) mPathToFollow = new List<Vector3Int>();
        if (mCondensedPath == null) mCondensedPath = new List<Vector3>();

        mPathToFollow.Clear();
        mCondensedPath.Clear();

        mPathToFollow = PathFindingData.Instance.GetPath(GameConstants.Instance.PlayerObject, mAgent.transform.position);
        //mCondensedPath = PathHelpers.CondensePathPoints(mPathToFollow, Mathf.Infinity, 0.45f, LayerMask.GetMask("Default"));
        foreach (Vector3Int v in mPathToFollow)
        {
            mCondensedPath.Add(v);
        }

        if (mCondensedPath != null && mCondensedPath.Count > 0)
        {
            int furthestVisisbleIndex = PathHelpers.FindFurthestVisiblePointIndex(mCondensedPath, mAgent.transform.position, LayerMask.GetMask("Default"));
            mAgent.BoidController.SetTarget(mCondensedPath[furthestVisisbleIndex]);
        }
    }
}

public class AttackState : FSMState
{
    public AttackState()
    {
        mStateID = fsmStateID.AttackPlayer;
    }

    public override void DoBeforeEntering()
    {
        
    }

    public override void DoBeforeLeaving()
    {
        
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
        agent.mState = "Attack";
        //Attack the Player
        agent.BoidController.SetTarget(player.transform.position);
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

    public override void DoBeforeEntering()
    {
        
    }

    public override void DoBeforeLeaving()
    {

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
            EditableTree SwitchToAttackDecisionTree = agent.GetSwitchToAttackDecisionTree();

            if (SwitchToAttackDecisionTree != null)
            {
                SwitchToAttackDecisionTree.mRoot.MakeDecision(agent);
            }

            //if (directionToPlayer.magnitude <= agent.GetAttackRange())
                //agent.FSMTransitionPassthrough(fsmTransition.ToAttack);
        }
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        agent.mState = "Chase";
        //Chase Player
        //agent.BoidController.SetTarget(player.transform.position);
        //agent.BoidController.SetShouldFlock(true);
    }

}