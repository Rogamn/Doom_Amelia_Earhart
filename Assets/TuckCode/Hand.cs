using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{

    [SerializeField] GameObject[] guns;
    [SerializeField] GameObject currentGun;
    [SerializeField] GameObject zoomGun;
    public int selectedGun;
    private bool gunExist;
    private PlayerControls playerControls;
    private int layermask;
    private Transform camTransform;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip gunShot;

    void Start()
    {
        camTransform = Camera.main.transform;
        layermask = 1 << 3;
        layermask = ~layermask;
        selectedGun = 0;
        gunExist = false;
        ChangeGun();

        playerControls = GetComponentInParent<PlayerScript>().getInputs();
    }

    void Update()
    {
        Fire();
        CycleGuns();
        ADS(); 
    }

    private void CycleGuns()
    {
        if(playerControls.Game.GunUp.WasPerformedThisFrame())
        {
            if(selectedGun + 1 >= guns.Length)
            {
                selectedGun = 0;
                ChangeGun();
            }
            else
            {
                selectedGun++;
                ChangeGun();
            }
        }

        if(playerControls.Game.GunDown.WasPerformedThisFrame())
        {
            if (selectedGun - 1 <= 0)
            {
                selectedGun = guns.Length - 1;
                ChangeGun();
            }
            else
            {
                selectedGun--;
                ChangeGun();
            }
        }
    }

    private void ChangeGun()
    {
        if (gunExist)
        {
            Destroy(currentGun);
            gunExist = false;
        }
        currentGun = Instantiate(guns[selectedGun], gameObject.transform);
        currentGun.transform.position = gameObject.transform.position;
        gunExist = true;
    }

    private void ADS()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            Camera.main.fieldOfView -= 30;
            currentGun.transform.position = zoomGun.transform.position;
        }

        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            Camera.main.fieldOfView += 30;
            currentGun.transform.position = gameObject.transform.position;
        }
    }

    private void Fire()
    {
        if (playerControls.Game.Fire.WasPerformedThisFrame())
        {
            source.PlayOneShot(gunShot);
            currentGun.GetComponent<Gun>().Raycast(camTransform, layermask);
        }
    }
}
