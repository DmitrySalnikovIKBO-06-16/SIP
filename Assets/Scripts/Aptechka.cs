using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aptechka : MonoBehaviour {

    public int healthAdd= 100;
    public int healthMax = 100;
    // Use this for initialization

    void PinUp(Player pl)
    {
        if (pl.HP + healthAdd <= healthMax)
            pl.Heal(healthAdd);
        else
            pl.Heal(healthMax-pl.HP);
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
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
