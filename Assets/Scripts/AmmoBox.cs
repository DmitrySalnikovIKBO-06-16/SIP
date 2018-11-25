using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour {

    public int ammoAdd = 40;
    public string AmmoType = "556x45";

    void PinUp(Player pl)
    {
        pl.AddAmmo(ammoAdd, AmmoType);

        GameObject gm = GameObject.Find("Base");
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
