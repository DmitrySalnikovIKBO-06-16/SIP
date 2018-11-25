using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{

    // скорость ходьбы и скорость поворота в секунду

    private NavMeshAgent _controller;
    private SynchEnemy _synch;
    private Renderer rend;
    public Color clrDmg = Color.red;
    private Color normClr;
    private Transform _thisTransform;
    private Transform _playerTransform;
    public  int R = 90;
    public  int r = 3;
    private double timer;
    public double timerOffset = 4;
    private double timerDmg;
    public double timerDmgOffset = 0.5;
    public int Damage = 9;
    private double timerGotHit;
    public static readonly double timerGotHitOffset = 0.2;

    public int Health=25;
    private bool OutOfBorder = false;
    // Use this for initialization
    void Start()
    {
        // Получаем контроллер
        _controller = GetComponent<NavMeshAgent>();
        _synch = GetComponent<SynchEnemy>();
        rend = GetComponent<Renderer>();
        normClr = rend.material.color;
        // Получаем компонент трансформации объекта, к которому привязан данный компонент
        _thisTransform = transform;
        timer = timerOffset;
        //Health = 25;

        // Получаем компонент трансформации игрока
        //Player player = (Player)FindObjectOfType(typeof(Player));
        
    }

    // Update is called once per frame

    public void GetDamage(int damage, Vector3 from)
    {
        //Debug.Log("Health left: " + Health);
        Health = Health - damage;
        timerGotHit = timerGotHitOffset;
        rend.material.color = clrDmg;
        Vector3 result =  (transform.position - from);
        result.y = 0;

        _controller.velocity = _controller.velocity + 12f * result.normalized + 25f * Vector3.up;
        if (Health < 0)
        {
            // if (GetComponent<SynchEnemy>().isMine)
            //{
            //gameObject.SetActive(false);
            GameObject.Find("Base").GetComponent<ConnectScript>().DeSpawn(gameObject);
            //}
        }
    }

    void FixedUpdate()
    {
        //if (_synch && _synch.isMine)
       // {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else timer = timerOffset;
            if (timerDmg > 0)
            {
                timerDmg -= Time.deltaTime;
            }
            if (timerGotHit > 0)
            {
                timerGotHit -= Time.deltaTime;
                if (timerGotHit <= 0) rend.material.color = normClr;
                    //TODO
            }
            if (player.Length <= 0) return;
            float closest = R + 1;
            int closestId = -1;
            Transform pl2;
            for (int i = 0; i < player.Length; i++)
            {
                pl2 = player[i].transform;
                if (Vector3.Distance(pl2.position, _thisTransform.position) < closest)
                {
                    Ray ray = new Ray(transform.position, (pl2.position - _thisTransform.position).normalized);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, R))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            closestId = i;
                            closest = Vector3.Distance(pl2.position, _thisTransform.position);
                        }
                    }
                }
            }
            //Debug.Log("Length: "+player.Length+" returned "+i);
            if (closestId < 0) return;
            GameObject pl = player[closestId];
            _playerTransform = pl.transform;
            if (Vector3.Distance(_playerTransform.position, _thisTransform.position) > 3.0f)
            {
                // двигаемся к игроку
                //if (Vector3.Distance(_playerTransform.position, _thisTransform.position) < 150.0f)


               
                        if ((_playerTransform.position.x - _thisTransform.position.x) * (_playerTransform.position.x - _thisTransform.position.x) +
                          (_playerTransform.position.y - _thisTransform.position.y) * (_playerTransform.position.y - _thisTransform.position.y) < R * R)
                            if (!_controller.SetDestination(_playerTransform.position))
                            {
                                if (!OutOfBorder)
                                {
                                    Debug.Log("Player out of Borders");
                                    OutOfBorder = true;
                                }
                            }
                            else OutOfBorder = false;
                    //    Debug.Log("Can See!!");
                    //else
                    //    Debug.Log("Can't See");
                    //else
                    //	Destroy(gameObject);
                
            }
            else // если меньше или равна трем метрам
            {
                if (timerDmg <= 0)
                {
                    PhotonView pView = pl.transform.GetComponent<PhotonView>();
                    if (pView)
                    {
                        timerDmg = timerDmgOffset;
                        GetComponent<AudioSource>().Play();
                        pView.RPC("DoDamage", PhotonTargets.All, Damage,transform.position.x, transform.position.y, transform.position.z);
                    }
                }
                // здесь например стреляем в игрока
            }

        //}
    }
}