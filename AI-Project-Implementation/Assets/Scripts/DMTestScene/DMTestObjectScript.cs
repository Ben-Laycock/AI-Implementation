using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DMTestObjectScript : MonoBehaviour
{
    [SerializeField] public int mAmount = 0;
    [SerializeField] public bool mAltActive = false;

    [SerializeField] public Text mObjectTextScript;

    [SerializeField] private EditableTree mDeicsionTree;
    [SerializeField] private bool mUseCanvas = false;

    private Vector3 mOriginalPosition;

    private void Start()
    {
        mOriginalPosition = transform.position;
    }

    private void Update()
    {
        if(mUseCanvas)
            mObjectTextScript.text = "" + mAmount;

        mDeicsionTree.mRoot.MakeDecision(this);
    }

    public void ChangeValueBy(int amount)
    {
        mAmount += amount;
    }
}