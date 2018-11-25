using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sinhronize : Photon.MonoBehaviour
{
    Vector3 oldPos = Vector3.zero;
    Vector3 newPos = Vector3.zero;
    Quaternion oldRot = Quaternion.identity;
    Quaternion newRot = Quaternion.identity;
    float offsetTimePos = 0f;
    float offsetTimeRot = 0f;
    bool isSinch = false;
    // Use this for initialization
    public bool isMine;
	void Start () {
        isMine = photonView.isMine;
        transform.Find("mine").GetComponent<Animation>().animatePhysics = isMine;
        
        if (!isMine)
        {
            transform.Find("Main Camera").GetComponent<Camera>().enabled = false;
            transform.Find("Main Camera").GetComponent<AudioListener>().enabled = false;
            GetComponent<Player>().enabled = false;
           // Debug.Log(transform.Find("mine").GetComponent<Animation>().animatePhysics);
            transform.Find("Name").GetComponent<TextMesh>().text = photonView.owner.NickName;
        }
        else
        {
            transform.Find("Name").GetComponent<TextMesh>().text = "";

        }
    }
	
	// Update is called once per frame
	void Update () {
        if(!isMine)
        {
            if (isSinch)
            {
                transform.Find("Name").GetComponent<TextMesh>().text = photonView.owner.NickName;
                if (Vector3.Distance(oldPos, newPos) > 3f)
                {
                    transform.position = oldPos = newPos;
                }
                else
                {
                    offsetTimePos += Time.deltaTime * 9f;
                    transform.position = Vector3.Lerp(oldPos, newPos, offsetTimePos);
                }
                if (Quaternion.Angle(oldRot, newRot) > 40f)
                {
                    transform.rotation = oldRot = newRot;
                }
                else
                {
                    offsetTimeRot += Time.deltaTime * 9f;
                    transform.rotation = Quaternion.Lerp(oldRot, newRot, offsetTimeRot);
                }
            }
        }
    }
    [PunRPC]
    void DoDamage(int damage, float x, float y, float z)
    {
        Player pl = transform.GetComponent<Player>();
        if (pl)
        {
            // Debug.Log("Have done " + damage + " damage to " + pl.name);
            try
            {
                Vector3 vct = new Vector3(x,y,z);
                pl.GetDamage(damage,vct);
            }
            catch
            { pl.GetDamage(damage); }

        }
    }
    [PunRPC]
    void DoDamage(int damage)
    {
        Player pl = transform.GetComponent<Player>();
        if (pl)
        {
           // Debug.Log("Have done " + damage + " damage to " + pl.name);
            pl.GetDamage(damage);
        }
    }
    [PunRPC]
    void Shot(string str, int type)
    {
        Player pl = transform.GetComponent<Player>();
        if (pl)
        {
            pl.ShotSound(str, type);
        }

    }
    [PunRPC]
    void MakeRed()
    {
        Player pl = transform.GetComponent<Player>();
        if (pl)
        {
            pl.UpdateLook(pl.clrDmg);
        }
    }
    [PunRPC]
    void MakeBack()
    {
        Player pl = transform.GetComponent<Player>();
        if (pl)
        {
            pl.UpdateLook(pl.NormClr);
        }
    }
    /*[PunRPC]
    void Shot()
    {
        Debug.Log("Have done shot  at ");
    }


    [PunRPC]
    void Chat(string NewMessage)
    {
        Debug.Log(NewMessage);
        Player pl = transform.GetComponent<Player>();
        //pl.AmmoNow += 50; 
        pl.GetDamage(3);
    }*/



    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        stream.Serialize(ref pos);
        stream.Serialize(ref rot);
        if (stream.isReading)
        {
            oldPos = transform.position;
            newPos = pos;
            offsetTimePos = offsetTimeRot = 0;
            isSinch = true;
            //transform.position = pos;
            //transform.rotation = rot;
            oldRot = transform.rotation;
            newRot = rot;
        }

    }
}
