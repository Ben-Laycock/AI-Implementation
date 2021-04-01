using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject mMainCanvas;
    private UIHandler mUIHandlerScript;

    private GameObject currentlyHoveringOverObj;

    // Start is called before the first frame update
    void Start()
    {
        if (null == mMainCanvas)
            return;

        mUIHandlerScript = mMainCanvas.GetComponent<UIHandler>();
        if (null == mUIHandlerScript)
            return;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentlyHoveringOverObj = eventData.pointerCurrentRaycast.gameObject;

        if (null == currentlyHoveringOverObj)
            return;

        if (currentlyHoveringOverObj.name != this.gameObject.name)
            return;
        
        mUIHandlerScript.MenuButtonMouseEnter(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentlyHoveringOverObj = null;
    }
}
