using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinWeapon : MonoBehaviour {

    public string wName="AK47";
    void PinUp(Player pl)
    {
        GameObject gm = GameObject.Find("Base");
        pl.addWeaponToSlot(wName);
        ConnectScript CS;
        if (gm)
        {
            CS = gm.GetComponent<ConnectScript>();

            if (CS)
            {
                CS.DeSpawn(gameObject);
            }
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
