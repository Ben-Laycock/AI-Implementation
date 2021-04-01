using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgentFSM : MonoBehaviour
{

    private FiniteStateMachine fsm;

    private void Start()
    {
        SetupFSM();
    }

    private void Update()
    {
        //fsm.currentState.Reason(playerObject, agentObject);
        //fsm.currentState.Act(playerObject, agentObject);
    }

    public void SetupFSM()
    {
        /*FollowPathState followState = new FollowPathState();
        followState.AddTransitionToState(fsmTransition.SawPlayer, fsmStateID.ChasePlayer);
        followState.AddTransitionToState(fsmTransition.Charge, fsmStateID.ChargeAttacks);

        ChargeAgentAttackState chargeState = new ChargeAgentAttackState();
        chargeState.AddTransitionToState(fsmTransition.LostPlayer, fsmStateID.FollowPath);
        chargeState.AddTransitionToState(fsmTransition.SawPlayer, fsmStateID.ChasePlayer);

        ChasePlayerState chaseState = new ChasePlayerState();
        chaseState.AddTransitionToState(fsmTransition.LostPlayer, fsmStateID.FollowPath);
        chaseState.AddTransitionToState(fsmTransition.Charge, fsmStateID.ChargeAttacks);

        fsm = new FiniteStateMachine();
        fsm.AddState(followState);
        fsm.AddState(chargeState);
        fsm.AddState(chaseState);*/
    }

    public void FSMTransitionPassthrough(fsmTransition transition) { fsm.PerformTransition(transition); }

}

/*
public class FollowPathState : FSMState
{
    //private int currentWayPoint;
    //private Transform[] waypoints;

    public FollowPathState() //Transform[] wp - Pass in List or Array of waypoints
    {
        //waypoints = wp;
        //currentWayPoint = 0;
        mStateID = fsmStateID.FollowPath;
    }

    public override void Reason(GameObject player, AgentHandler agent)
    {
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, (player.transform.position - agent.transform.position).normalized, out hit, 15F))
        {
            if (hit.transform.gameObject.tag == "Player")
                agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.SawPlayer);
        }
    }

    public override void Act(GameObject player, AgentHandler agent)
    {

    }

}

public class ChargeAgentAttackState : FSMState
{
    public ChargeAgentAttackState()
    {
        mStateID = fsmStateID.ChargeAttacks;
    }

    public override void Reason(GameObject player, AgentHandler agent)
    {

        //if (Vector3.Distance(agent.transform.position, player.transform.position) >= 30)
            //agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.LostPlayer);

        //Check Player Charge
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        //Charge NPC Attack
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

        if (Vector3.Distance(agent.transform.position, player.transform.position) >= 30)
            agent.GetComponent<TestAgentFSM>().FSMTransitionPassthrough(fsmTransition.Charge);
    }

    public override void Act(GameObject player, AgentHandler agent)
    {
        //Charge NPC Attack
    }

}
*/