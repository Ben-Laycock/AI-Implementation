using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum fsmTransition
{
    DefaultTransition = 0,
    ToChase = 1,
    ToAttack = 2,
    ToLostPlayer = 3,
}

public enum fsmStateID
{
    DefaultStateID = 0,
    ChasePlayer = 1,
    AttackPlayer = 2,
    FollowPath = 3
}

public abstract class FSMState
{

    public Dictionary<fsmTransition, fsmStateID> mTransitionStateIDMap = new Dictionary<fsmTransition, fsmStateID>();
    public fsmStateID mStateID;

    public void AddTransitionToState(fsmTransition transition, fsmStateID stateID)
    {
        // Log error if the passed transition or state are incorrect
        if (transition == fsmTransition.DefaultTransition || stateID == fsmStateID.DefaultStateID)
        {
            Debug.LogError("FSMState ERROR: Transition or State are incorrect!");
            return;
        }

        if (mTransitionStateIDMap.ContainsKey(transition))
        {
            Debug.LogError("FSMState ERROR: That transition has already been used to direct to another state! Transition: " + transition.ToString());
            return;
        }

        mTransitionStateIDMap.Add(transition, stateID);
    }

    public void DeleteTransitionToState(fsmTransition transition)
    {
        if (transition == fsmTransition.DefaultTransition)
        {
            Debug.LogError("FSMState ERROR: Cannot Delete Transition on Default Transition!");
            return;
        }

        if (mTransitionStateIDMap.ContainsKey(transition))
        {
            mTransitionStateIDMap.Remove(transition);
            return;
        }

        Debug.LogError("FSMState ERROR: Transition to State not found! Please check your remove command is correct!");
    }

    public fsmStateID GetOutputState(fsmTransition transition)
    {
        if (mTransitionStateIDMap.ContainsKey(transition))
        {
            return mTransitionStateIDMap[transition];
        }

        return fsmStateID.DefaultStateID;
    }

    public abstract void DoBeforeEntering();
    public abstract void DoBeforeLeaving();
    public abstract void Reason(GameObject player, AgentHandler agent);
    public abstract void Act(GameObject player, AgentHandler agent);

} // Class FSMState

public class FiniteStateMachine
{

    //Will Store all States in the system
    private List<FSMState> mStates;

    private fsmStateID mCurrentStateID;
    public fsmStateID currentStateID { get { return mCurrentStateID; } }

    private FSMState mCurrentState;
    public FSMState currentState { get { return mCurrentState; } }

    public FiniteStateMachine()
    {
        mStates = new List<FSMState>();
    }

    public void AddState(FSMState s)
    {

        if (s == null)
        {
            Debug.LogError("FSM ERROR: Attempted to add Null State");
            return;
        }

        //Add the State if its the First state in the system!
        if (mStates.Count == 0)
        {
            mStates.Add(s);
            mCurrentState = s;
            mCurrentStateID = s.mStateID;
            return;
        }

        //Check to see if the State is already in the Finite State Machine System!
        foreach (FSMState state in mStates)
        {
            if (state.mStateID == s.mStateID)
            {
                Debug.LogError("FSM ERROR: State already exists in Finite State Machine System!");
                return;
            }
        }

        mStates.Add(s);
    }

    public void DeleteState(fsmStateID stateID)
    {
        if (stateID == fsmStateID.DefaultStateID)
        {
            Debug.LogError("FSM ERROR: Cannot Delete Default State!");
            return;
        }

        foreach (FSMState state in mStates)
        {
            if (state.mStateID == stateID)
            {
                mStates.Remove(state);
                return;
            }
        }

        Debug.LogError("FSM ERROR: State not found! Please check you are removing the correct state!");
    }

    public void PerformTransition(fsmTransition transition)
    {
        if (transition == fsmTransition.DefaultTransition)
        {
            Debug.LogError("FSM ERROR: Cannot transition to the Default Transition!");
            return;
        }

        fsmStateID id = currentState.GetOutputState(transition);
        if (id == fsmStateID.DefaultStateID)
        {
            Debug.LogError("FSM ERROR: Current State cannot use provided transition!");
            return;
        }
	
        mCurrentStateID = id;
        foreach (FSMState state in mStates)
        {
            if (state.mStateID == currentStateID)
            {
                currentState.DoBeforeLeaving();

                mCurrentState = state;

                currentState.DoBeforeEntering();
                break;
            }
        }

    }

} // Class FiniteStateMachine