using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    private WaitForSeconds twoSeconds;
    [SerializeField] private GameObject deciduousLeavesFallingPrefab = null;
    [SerializeField] private GameObject reapingPrefab = null;

    protected override void Awake()
    {
        base.Awake();
        twoSeconds = new WaitForSeconds(2f);
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= displayHavestActionEffect;
    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += displayHavestActionEffect;
    }

    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }

    private void displayHavestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.deciduousLeavesFalling:
                GameObject deciduousLeavesFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity);
                deciduousLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(deciduousLeavesFalling, twoSeconds));
                break;

            case HarvestActionEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;

            case HarvestActionEffect.none:
                break;

            default:
                break;
        }
    }
}
