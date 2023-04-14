using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    public Transform parentObject;
    public ShopManager shopManager;
    public GameObject playerController;
    public int damageToTake;
    public SwordItemSO[] swordSO;
    public CameraShake cameraShake;
    public Animator iceAttackAnim;
    public GameObject iceExplosion;

    void Start()
    {
        shopManager = FindObjectOfType<ShopManager>();
        for(int i=1; i<=6; i++)
        {
            if(gameObject.name.Contains(i.ToString()))
            {
                Debug.Log("damage is: " + swordSO[i - 1].damage);
                damageToTake = swordSO[i-1].damage;
            }
        }
        playerController = GameObject.FindGameObjectWithTag("Player");
    }

    public IEnumerator PlayExplosion()
    {
        GameObject newIceExplosion = Instantiate(iceExplosion, playerController.transform.position, Quaternion.identity);
        iceAttackAnim = iceExplosion.gameObject.GetComponent<Animator>();
        cameraShake = FindObjectOfType<CameraShake>();
        iceAttackAnim.Play("IceAttack");
        StartCoroutine(cameraShake.Shake(0.15f, 0.4f));
        playerController.GetComponent<PlayerController>().CheckForDestructibles();
        yield return new WaitForSeconds(0.5f);
        Destroy(newIceExplosion);
    }

    public void PlayExplosionFunction()
    {
        StartCoroutine(PlayExplosion());
    }
}
