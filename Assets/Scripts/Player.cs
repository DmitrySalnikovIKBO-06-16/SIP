using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using UnityEngine;

public delegate void Del();

public struct ButtonDimas
{
    public ButtonDimas(string name, Del ptr)
    {
        buttonName = name;
        pointer = ptr;
    }
    public string buttonName;
    public Del pointer;
}

public class Player : Photon.MonoBehaviour {

    public GameObject basePlane;
    public struct Weapon_Param
    {
        public int Slot;
        public string Ammo;
        public int ClipSize;// размер магазина
        public int Damage;// урон за выстрел
        public int Dist;// радиус стрельбы
        public int RPM;//скорострельность
        public int Burst;// режим стрельбы (true очереди)
        public double ReloadTime; // Время перезарядки
        public AudioClip clip_reload; // звук перезарядки
        public AudioClip clip_shot; // звук выстрела
                                    // public string clip_burst; // Время перезарядки

    }
    public struct WeaponSlot
    {
        public bool have; //есть ли оружие в этом слоте
        public bool active; //активен ли слот (можно ли в него сунуть что-либо и достать, если есть)
        public string wName; //Название оружия
        public string wAmmoName; //Название патронов
        public int wAmmoNow; //Патронов в магазине
    }
    public int HP = 100;
    public float HPreg = 1f;
    public float HPregDelay = 1.5f;
    public Rect rect = new Rect(x: 30, y: 50, width: 300, height: 75);
    static readonly int BtnH = 80;
    static readonly int BtnW = 200;
    static readonly int BtnO = 20;//отступ
    private  bool pause;

    private PhotonView phView;
    private Renderer rend;
    private CharacterController _controller;

    public Texture2D TxtScope_Texture;
    public Texture2D TxtScope_TextureHit;
    public Texture2D TxtGotDamage;

    /*Регистрация попаданий*/
    public Texture2D guiTexture;
    public List<float> guiTextures;
    public double gotDirectTimerOffset = 0.8;
    public List<double> gotDirectTimers;



    public Dictionary<string, Weapon_Param> weaponList;
    private List<ButtonDimas> list;

    /*Что касается стрельбы:*/

    Weapon_Param Primary;

    private WeaponSlot[] slots;
    //slots[0] - melee
    //slots[1] - pistol
    //slots[2] - first primary
    //slots[3] - second primary
    public bool addWeaponToSlot(string wName)
    {
        Weapon_Param wp;
        if (weaponList.TryGetValue(wName,out wp))
        {
            Debug.Log("Loaded weapon: " + wName);
            if (wp.Slot == 1)
            {
                //Пока забейте
            }
            else if (wp.Slot==2)
            {
                WeaponSlot sl = slots[1];
                if (sl.active) 
                {
                    if (sl.have)
                    {
                        Drop(sl.wName); //выбрасиваем оружие
                        sl.have = false; //на всякий
                    }
                    sl.wName = wName;
                    sl.wAmmoNow = 0;
                    sl.wAmmoName = wp.Ammo;
                    sl.have = true;
                    ActiveSlot = ActiveSlot; //Передергивание затвора (в переносном смысле), в случае, если оружие у нас в руках
                    slots[1] = sl;
                    Debug.Log("Written1 " + sl.wName);
                    Debug.Log("Written2 " + slots[1].wName);
                    return true;
                }
                else
                {
                    Debug.Log("Slot is unactive, cannot get weapon");
                    return false;
                }
            }
            else if (wp.Slot == 3)
            {
                WeaponSlot sl = slots[2];

                if (sl.active && !sl.have) //Пытаемся запхать в первый слот, если он пустой
                {
                    sl.wName = wName;
                    sl.wAmmoNow = 0;
                    sl.wAmmoName = wp.Ammo;
                    sl.have = true;
                    slots[2] = sl;
                    ActiveSlot = ActiveSlot; //Передергивание затвора (в переносном смысле), в случае, если оружие у нас в руках
                    return true;
                }
                else if (slots[3].active && !slots[3].have) //проверяем второй слот на то, что он пустой
                {
                    sl = slots[3]; 
                    sl.wName = wName;
                    sl.wAmmoNow = 0;
                    sl.wAmmoName = wp.Ammo;
                    sl.have = true;
                    ActiveSlot = ActiveSlot;
                    slots[3] = sl;
                    return true;
                }
                else if ((activeSlot==2 || activeSlot==3)&&slots[activeSlot].active) //если все забито, смотрим, можем ли мы запхать оружие в текущий слот
                {
                    sl = slots[activeSlot];
                    if (sl.have)
                    {
                        Drop(sl.wName); //выбрасиваем оружие
                        sl.have = false; //на всякий
                    }
                    sl.wName = wName;
                    sl.wAmmoNow = 0;
                    sl.wAmmoName = wp.Ammo;
                    sl.have = true;
                    ActiveSlot = ActiveSlot;
                    slots[activeSlot] = sl;
                    return true;
                }
                else if (sl.active) //Если уж в текущий слот не получилось, автоматически запихиваем в первый слот
                {
                    if (sl.have)
                    {
                        Drop(sl.wName); //выбрасиваем оружие
                        sl.have = false; //на всякий
                    }
                    sl.wName = wName;
                    sl.wAmmoNow = 0;
                    sl.wAmmoName = wp.Ammo;
                    sl.have = true;
                    ActiveSlot = ActiveSlot;
                    slots[2] = sl;
                    return true;
                }
                else if (slots[3].active) //Если уж и в первый слот не получилось, автоматически запихиваем во второй слот, это последний шанс
                {
                    sl = slots[3];
                    if (sl.have)
                    {
                        Drop(sl.wName); //выбрасиваем оружие
                        sl.have = false; //на всякий
                    }
                    sl.wName = wName;
                    sl.wAmmoNow = 0;
                    sl.wAmmoName = wp.Ammo;
                    sl.have = true;
                    ActiveSlot = ActiveSlot;
                    slots[3] = sl;
                    return true;
                }
                else
                {
                    Debug.Log("Slots are unactive, cannot get weapon");
                    return false;
                }
            }
        }
        return false;

    }
   
    private int activeSlot;
    public int ActiveSlot
    {
        get
        {
            return activeSlot;
        }

        set
        {
            if (value >= 0 && value < 4 && slots[value].active && slots[value].have) {
                slots[activeSlot].wAmmoNow = AmmoNow;
                activeSlot = value;
                WeaponName = slots[value].wName;
                AmmoNow = slots[value].wAmmoNow;
                updateBurst();

            }
        }
    }
    //slots[0] - melee
    //slots[1] - pistol
    //slots[2] - first primary
    //slots[3] - second primarys

    public Dictionary<string, int> ammoList;


    private int AmmoClip = 100;
    private int AmmoCur = 40;
    public int AmmoNow = 0; //Кол-во патронов в магазине 
    public int AmmoMax = 1000; //максимальное кол-во патронов

    private double ReloadTime=0.0;
    private int burst = 1;
    private double burstDelay=0.0;
    private double hitTimer;
    public double hitTimerOffset = 0.7;
    private bool hitRed=false;
    private double HPregTimer;
    private double HPregDelayTimer;
    private bool HPadd;
    private double gothitTimer;
    public double gothitTimerOffset = 0.4;
    public double pinUpR=3;

    /*GUI*/
    Vector3 centerRotateOfGUI;//center point of Rotate
    public float angle;
    public Rect mainRect;

    /*Звуки*/
    private AudioSource audioSource;
    public AudioClip clip_reload;
    public AudioClip clip_shot;
    public AudioClip clip_hit;
    public AudioClip clip_myHit;
    public AudioClip clip_criticalHit;

    //public string weaponName = "M16";
    private string weaponName; //объявление свойства
    public string WeaponName //объявление свойства
    {
        get // аксессор чтения поля
        {
            return weaponName;
        }
        set // аксессор записи в поле
        {
            Debug.Log("weaponName SET: " + value);
            //weaponName = value;
            /*для смены оружия достаточно изменить текстовую переменную, 
            и ее аксессор сам подгрузит из словаря все необходимое*/
            if (weaponList.TryGetValue(value, out Primary))
                weaponName = value;            
            else if (!weaponList.TryGetValue(weaponName, out Primary))
                Debug.Log("Strange");
            else Debug.Log("Really bad");
        }
    }

    public Color NormClr
    {
        get
        {
            return normClr;
        }
    }


    private bool dying=false;


    public Color clrDmg = Color.red;
    private Color normClr;
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 normal)
    {
        return Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }
    // Use this for initialization
    public void GetDamage(int damage)
    {
        if (!rend)
        {
            rend = GetComponent<Renderer>();
            normClr = rend.material.color;
        }
        if (!dying&&GetComponent<Sinhronize>().isMine)
        {
            HP = HP - damage;
            HPregDelayTimer = HPregDelay;
            GetComponent<AudioSource>().PlayOneShot(clip_hit);
            gothitTimer = gothitTimerOffset;
            if (!hitRed)
            {
                if (phView)
                    phView.RPC("MakeRed", PhotonTargets.Others);
                hitRed = true;
            }
            double x = gotDirectTimerOffset;
            gotDirectTimers.Add(x);
            float angle = 0;
            guiTextures.Add(angle);
            if (HP < 0)
            {
                dying = true;
                GameObject.Find("Base").GetComponent<ConnectScript>().ReSpawn(gameObject);
            }
        }
    }
    public void GetDamage(int damage, Vector3 from)
    {
        if (!rend)
        {
            rend = GetComponent<Renderer>();
            normClr = rend.material.color;
        }
        if (!dying && GetComponent<Sinhronize>().isMine)
        {
            HP = HP - damage;
            HPregDelayTimer = HPregDelay;
            if (!hitRed)
            {
                if (phView)
                    phView.RPC("MakeRed", PhotonTargets.Others);
                hitRed = true;
            }
            Vector3 result = (transform.position - from);
            result.y = 0;

            double x = gotDirectTimerOffset;
            gotDirectTimers.Add(x);
            //Quaternion vect = new Quaternion();
            //vect = Quaternion.FromToRotation(transform.position, from);
            
            float angle = 180 + AngleSigned(transform.forward, result.normalized,Vector3.up);//AngleSigned(transform.rotation.eulerAngles, vect.eulerAngles, Vector3.up);//90- Quaternion.Angle(transform.rotation,vect);
            //Debug.Log(angle);
            guiTextures.Add(angle);
            //if (!_controller)
            //    _controller = GetComponent<CharacterController>();
           // _controller.SimpleMove(damage *(50f * result.normalized + 100f * Vector3.up));
            GetComponent<AudioSource>().PlayOneShot(clip_hit);
            gothitTimer = gothitTimerOffset;
            if (HP < 0)
            {
                dying = true;

                GameObject.Find("Base").GetComponent<ConnectScript>().ReSpawn(gameObject);
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    public void Cont()
    {
        pause = !pause;
        UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
        GetComponent<MouseLook>().enabled = !GetComponent<MouseLook>().enabled;
    }
    public void Exit()
    {
        Application.Quit();
    }
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();
        _controller = GetComponent<CharacterController>();
        normClr = rend.material.color;
        clrDmg = Color.red;
        hitRed = false;
        if (GetComponent<Sinhronize>().isMine)
        {
            basePlane = GameObject.Find("Base");
            ammoList = new Dictionary<string, int>();
            XMLLoad load = basePlane.GetComponent<XMLLoad>();
            foreach (KeyValuePair<string,XMLLoad.Ammo> am in load.ammoList)
            {
                ammoList.Add(am.Key, 0);
            }
            slots = new WeaponSlot[4];
            for (int i=0;i<4;i++)
            {
                slots[i].active = true;
                slots[i].have = false;
            }
            //slots[0] - melee
            //slots[1] - pistol
            //slots[2] - first primary
            //slots[3] - second primary

            weaponList = load.weaponList;
            //WeaponName = "Beretta";
            addWeaponToSlot("Beretta");
            ActiveSlot = 1;


            mainRect = new Rect(Screen.width/4, Screen.height / 4, Screen.width / 2, Screen.height / 2);
            centerRotateOfGUI = new Vector2(mainRect.x + mainRect.width / 2, mainRect.y + mainRect.height / 2);
            guiTexture = Resources.Load<Texture2D>("Textures/HurtSector");
            guiTextures = new List<float>();
            gotDirectTimers = new List<double>();
            angle = 0f;

            phView = GetComponent<PhotonView>();
            UnityEngine.Cursor.visible = false;
            Del ptr = Cont;
            pause = false;
            list = new List<ButtonDimas>();
            list.Add(new ButtonDimas("Cont", ptr));
            ptr = Exit;
            list.Add(new ButtonDimas("Exit", ptr));

            //AmmoCur = 450;

            AmmoNow = Primary.ClipSize;

            TxtScope_Texture = Resources.Load<Texture2D>("Textures/aim");
            TxtScope_TextureHit = Resources.Load<Texture2D>("Textures/aimHit");
            TxtGotDamage = Resources.Load<Texture2D>("Textures/HurtImage70%");
            /*clip_reload = Resources.Load<AudioClip>("reload");
            clip_shot = Resources.Load<AudioClip>("Sounds/shot");
            clip_hit = Resources.Load<AudioClip>("Sounds/decoder");*/


            if (Primary.Burst == 1) burst = -1;
            else if (Primary.Burst == 0) burst = 1;
            else if (Primary.Burst > 0) burst = Primary.Burst;
        }
    }
	
    public void Heal(int x)
    {
        HP += x;
    }
    public void Drop(string path)
    {
        //TODO: make dropping different prefabs (only spawning)
        basePlane.GetComponent<ConnectScript>().Spawn(path,transform.position-transform.forward,Quaternion.identity);
    }
    public void AddAmmo(int x, string type)
    {
        
        if (ammoList[type] + x <= AmmoMax)
            ammoList[type] += x;
        else
            ammoList[type] = AmmoMax;

    }
    // Update is called once per frame
    private void updateBurst()
    {

        if (Primary.Burst == 1) burst = -1;
        else if (Primary.Burst == 0) burst = 1;
        else if (Primary.Burst > 0) burst = Primary.Burst;
    }




    void Update () {
        if (GetComponent<Sinhronize>().isMine)
        {
            int ammo;
            WeaponSlot sl = slots[activeSlot];
            if (HP < 75 && HPregDelayTimer <= 0)
            {
                HPadd = true;
            }
            else
            {
                HPadd = false;
            }
            //angle += Time.deltaTime * 25f;
            if (Input.GetMouseButton(0) && AmmoNow > 0 && ((burst > 0) || (burst < 0)) && burstDelay <= 0 && ReloadTime <= 0)
            {
                Shoot();
                AmmoNow--;
                if (burst > 0)
                    burst--;
                burstDelay = 60.0 / Primary.RPM;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                updateBurst();
            }
            if (Input.GetButtonDown("Cancel"))
            {
                Cont();
            }
            else if (Input.GetButtonDown("Use"))
            {
                GameObject[] gms = GameObject.FindGameObjectsWithTag("PinUp");
                foreach (GameObject gm in gms)
                {
                    if (Vector3.Distance(gm.transform.position, transform.position) <= pinUpR)
                    {
                        gm.SendMessage("PinUp", this);
                        //basePlane.GetComponent<ConnectScript>().DeSpawn(gm);
                    }
                }
            }
            else if (Input.GetButtonDown("Pistol"))
            {
                if (slots[1].active && slots[1].have)
                {
                    ActiveSlot = 1;
                }
            }
            else  if (Input.GetButtonDown("FirstPrimary"))
            {
                if (slots[2].active && slots[2].have)
                {
                    ActiveSlot = 2;
                }
            }
            else if (Input.GetButtonDown("SecondPrimary"))
            {
                if (slots[3].active && slots[3].have)
                {
                    ActiveSlot = 3;
                }
            }
            else if (Input.GetButtonDown("Reload") && ammoList.TryGetValue(sl.wAmmoName,out ammo) && ammo != Primary.ClipSize && ammo != 0 && ReloadTime <= 0 && AmmoNow != Primary.ClipSize)
            {
                ReloadTime = Primary.ReloadTime;
                if (ammo + AmmoNow < Primary.ClipSize)
                {
                    AmmoNow = ammo + AmmoNow;
                    ammo = 0;
                }
                else
                {
                    ammo = ammo + AmmoNow - Primary.ClipSize;
                    AmmoNow = Primary.ClipSize;
                }
                ammoList[sl.wAmmoName] = ammo;
                //audioSource.PlayOneShot(Primary.clip_reload);
                if (phView)
                    phView.RPC("Shot", PhotonTargets.All, WeaponName, 2);

            }
            { //таймеры и т.д. Эта скобка не несет смысловой нагрузки и нужна просто чтобы можно было удобно сворачивать все таймеры
                for (int i = 0; i < gotDirectTimers.Count; i++)
                {
                    if (gotDirectTimers[i] > 0)
                        gotDirectTimers[i] -= Time.deltaTime;
                    else
                    {
                        gotDirectTimers.RemoveAt(i);
                        if (guiTextures.Count > i)
                            guiTextures.RemoveAt(i);
                        else
                            Debug.LogError("Not sync with guitextures and timers!");
                        i--;
                    }

                }
                if (HPregTimer > 0)
                {
                    HPregTimer -= Time.deltaTime;
                }
                else
                {
                    if (HPadd)
                    {
                        HPregTimer = 1 / HPreg;
                        HP++;
                    }
                }
                if (HPregDelayTimer > 0)
                {
                    HPregDelayTimer -= Time.deltaTime;
                }
                if (ReloadTime > 0)
                {
                    ReloadTime -= Time.deltaTime;
                }
                if (burstDelay > 0)
                {
                    burstDelay -= Time.deltaTime;
                }

                if (hitTimer > 0)
                {
                    hitTimer -= Time.deltaTime;
                }
                if (gothitTimer > 0)
                {
                    gothitTimer -= Time.deltaTime;
                }
                else
                {
                    //Debug.Log("Timer Stopped");
                    if (hitRed)
                    {
                        //if (!rend) rend = GetComponent<Renderer>();
                        hitRed = false;
                        if (phView)
                            phView.RPC("MakeBack", PhotonTargets.Others);
                        // rend.material.color = normClr;
                    }
                }
            }
        
        }
    }

    public void ShotSound(string weaponName, int type) 
        //type == 1 - shot
        //type == 2 - reload
    {
        if (!basePlane)
        {
            basePlane = GameObject.Find("Base");
        }
        if (weaponList==null)
            weaponList = basePlane.GetComponent<XMLLoad>().weaponList;
        
        if (string.Compare(WeaponName,"")==-1)
        {
            //Debug.Log("Step 4");
            WeaponName = weaponName;
            weaponList.TryGetValue(weaponName, out Primary);
        }
        if (type==1)
            GetComponent<AudioSource>().PlayOneShot(Primary.clip_shot);
        else if (type==2)
            GetComponent<AudioSource>().PlayOneShot(Primary.clip_reload);
    }
    public void UpdateLook(Color clr)
    {
        if (!rend) rend = GetComponent<Renderer>();
        rend.material.color = clr;
    }

    private void Shoot()
    {

        if (phView)
            phView.RPC("Shot", PhotonTargets.All,WeaponName,1);
        //GetComponent<AudioSource>().PlayOneShot(Primary.clip_shot);
        Ray ray = new Ray(transform.Find("FirePoint").transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Primary.Dist))
        {
            if (hit.collider.tag == "Player" || hit.collider.tag == "Enemy")
            {
                PhotonView pView = hit.transform.GetComponent<PhotonView>(); 
                if(pView)
                {
                    hitTimer = hitTimerOffset;
                    pView.RPC("DoDamage", PhotonTargets.All, Primary.Damage, transform.position.x, transform.position.y, transform.position.z);
                }
                //hit.collider.SendMessage("SelfDestroying", 5);
                //aimHitBool = true;
            }
        }

    }

    void OnGUI()
    {
        if (GetComponent<Sinhronize>().isMine)
        {
            //GUI.skin = MySkin;
            if (pause)
            {
                GUI.Box(new Rect((Screen.width) / 3, (Screen.height) / 6, (Screen.width) / 3, (Screen.height) * 2 / 3), "");
                string str = GUI.TextField(new Rect((Screen.width) / 2 - 100, (Screen.width) / 2 - 200, 200, 30), basePlane.GetComponent<ConnectScript>().PlName);
                if (basePlane.GetComponent<ConnectScript>().PlName != str) 
                {
                    if (GetComponent<PhotonView>())
                        Debug.Log("New NickName:"+ GetComponent<PhotonView>().owner.NickName);
                    basePlane.GetComponent<ConnectScript>().SetName(str);
                }

                int Summ = list.Count * (BtnH + BtnO) - BtnO;
                //Debug.Log("Summ:" + Summ);
                for (int i = 0; i < list.Count; i++)
                {
                    if (GUI.Button(new Rect((Screen.width - BtnW) / 2, (Screen.height / 2 + Summ / 2 - BtnH - i * (BtnO + BtnH)), BtnW, BtnH), list[i].buttonName))
                    {
                        list[i].pointer();
                    }
                    //Debug.Log("Screen.height + Summ/2 - BtnH - " + i + "*(BtnO + BtnH)" + (Screen.height + Summ / 2 - BtnH - i * (BtnO + BtnH)));
                }

            }
            else
            {
                int ammo;
                ammoList.TryGetValue(slots[ActiveSlot].wAmmoName,out ammo);
                GUI.Box(rect, WeaponName+"\nAmmo:" + AmmoNow + "/" + ammo + "\n HP:" + HP+"\nNickName: "+basePlane.GetComponent<ConnectScript>().PlName);
                if (hitTimer<=0) GUI.DrawTexture(new Rect((Screen.width - TxtScope_Texture.width) / 2, (Screen.height - TxtScope_Texture.height) / 2, TxtScope_Texture.width, TxtScope_Texture.height), TxtScope_Texture);
                else GUI.DrawTexture(new Rect((Screen.width - TxtScope_TextureHit.width) / 2, (Screen.height - TxtScope_TextureHit.height) / 2, TxtScope_TextureHit.width, TxtScope_TextureHit.height), TxtScope_TextureHit);
                if (gothitTimer > 0)
                {
                    //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), TxtGotDamage);
                    foreach (float angle1 in guiTextures)
                    {
                        Matrix4x4 iniMatrix = GUI.matrix;
                        GUIUtility.RotateAroundPivot(angle1, centerRotateOfGUI);//Change GUI matrix
                        GUI.DrawTexture(mainRect, guiTexture);
                        ///There restore the initial GUI.matrix for future elements from iniMatrix;
                        GUI.matrix = iniMatrix;
                    }
                }
                if (HP<20)
                {
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), TxtGotDamage);
                }
            }
        }
    }
}
