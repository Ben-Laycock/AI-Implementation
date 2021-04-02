using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicHandler : MonoBehaviour
{

    [Header("Prefabs for use!")]
    [SerializeField] private GameObject mRelicCollectionVFX;

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            GameObject relicCollection = PoolSystem.Instance.GetObjectFromPool(mRelicCollectionVFX, argActivateObject: true, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
            relicCollection.transform.position = transform.position;

            DeactivateVFX relicCollectionVFXScript = relicCollection.GetComponent<DeactivateVFX>();
            relicCollectionVFXScript.WaitForDeactivation();

            PlayerHandler playerHandler = other.gameObject.GetComponent<PlayerHandler>();
            playerHandler.IncreaseCollectedRelicCount(1);

            Destroy(gameObject);
        }
    }

}