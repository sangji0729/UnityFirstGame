using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject[] weapons;
    public bool[] hasWeapon;
    public GameObject[] grenades;
    public int maxHasGrenades;

    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;
    public GameObject grenadeObj;

    public int maxAmmo; //탄약 최대 소지량
    public int maxCoin;
    public int maxHealth;

    float speed = 1.7f;
    float verticalInput;
    float horizontalInput;
    Vector3 moveVec;
    bool isjump;
    bool infiniteJump;
    bool mouseDown;
    bool mouseDown2;
    bool walkMouseDown;//마우스 왼쪽버튼 이동
    bool fDown; //????
    bool gDown; //수류탄 투척
    bool rDown;//재장전
    bool isWalk;
    bool isRun;
    bool sDown1;//????????
    bool sDown2;
    bool sDown3;
    bool sDown4;
    bool isSwap;
    bool isReload; 
    bool isFireReady = true; //???? ????
    bool isBorder;
    

    private Camera camera;
    private bool isMove; //이 3개는 마우스 이동시 필요한 변수
    private Vector3 destination;
   // private Vector3 moveVec;

    float mouseLocation;
    float jumpPower = 5;

    bool iDown;//??????????

    Animator anim;
    Rigidbody playerRB;
    Camera characterCamera;
    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;//???? ??????

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        characterCamera = GetComponentInChildren<Camera>();
        camera = Camera.main;   
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        Attack();
        Reload();
        Grenade();
        //ReloadOut();

        walkMouseDown = Input.GetMouseButton(0);
        MouseWalk();

        mouseDown = Input.GetMouseButton(0);
        LookMouseCursor();

        mouseDown2 = Input.GetMouseButton(1);
        LookMouseCursor2();

        look();
        Interaction();
        Swap();
        


    }

    void Move()
    {
           
        if(destination != Vector3.zero) { 
            Vector3 moveVec = destination - transform.position;
            moveVec.Normalize();
            transform.position += moveVec.normalized * Time.deltaTime * speed;
        }
        else
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");

            moveVec = new Vector3(horizontalInput, 0, verticalInput).normalized;
        }   

        if (Input.GetButtonDown("Run"))
        {
            isRun = !isRun; //달리기 토글 설정
            anim.SetBool("isRun", isRun);
        }
            //isRun = Input.GetButton("Run");
            fDown = Input.GetButton("Fire2");//????
            gDown = Input.GetButtonDown("Fire1");
            rDown = Input.GetButtonDown("Reload");
            iDown = Input.GetButtonDown("Interaction");//??????????
            sDown1 = Input.GetButtonDown("Swap1");//????????
            sDown2 = Input.GetButtonDown("Swap2");
            sDown3 = Input.GetButtonDown("Swap3");
            sDown4 = Input.GetButtonDown("Swap4");

            anim.SetBool("isWalk", moveVec != Vector3.zero);
            //anim.SetBool("isRun", isRun);

            

            if (isRun) //?????? ????
            {
                speed = 2;
                transform.position += moveVec.normalized * speed * Time.deltaTime;
            }
            if (isSwap || !isFireReady || isReload) // ???? ???? ?? ???????? ????????
            {
                moveVec = Vector3.zero;
            }
            if(!isBorder)
                transform.position += moveVec.normalized * speed * Time.deltaTime;


        // transform.Translate(Vector3.forward * verticalInput * Time.deltaTime * speed);
        // transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);

        if (destination != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, destination) <= 0.1f)
            {
                destination = Vector3.zero;
            }
        }   


    }

    void Jump()
    {
        isjump = Input.GetButtonDown("Jump");
        anim.SetBool("isJump", isjump);
        anim.SetTrigger("doJump");

        if (isjump && !infiniteJump && !isSwap) //????
        {
            playerRB.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            infiniteJump = true;
            
        }
    }

    void Grenade()
    {
        if(hasGrenades == 0)
        {
            return;
        }
        if(gDown && !isReload && !isSwap)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitResult;
            if (Physics.Raycast(ray, out hitResult, 100))
            {
                Vector3 nextVec = hitResult.point - transform.position;
                nextVec.y = 2;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
            
        }
        
    }

    void Attack()
    {
        if(equipWeapon == null)
            return;
        
        //???? ???????? ?????? ???????? ???????? ?????? ????
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isSwap)
        {
            equipWeapon.Use();//Weapon ???????? ???? Use ???? ????
            //???? ???????? ?????? ?????????? doSwing, ???????????? doShot ?????????? ????(??????????)
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if(equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (ammo <= 0)
            return;
        if(rDown && !isjump && !isSwap && isFireReady && !isReload)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 1.7f);//재장전속도
        }
        
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo; //reAmmo = 현재 들고있는 무기의 탄창속 탄약이 최대 장탄수보다 적다는 조건이 참일땐 현재 장탄수, 거짓일땐 최대 장탄수
        reAmmo -= equipWeapon.curAmmo;
        equipWeapon.curAmmo += reAmmo;
        ammo -= reAmmo;
        //equipWeapon.curAmmo = reAmmo;//현재 들고있는 무기의 탄창속 탄약수는 reAmmo  
        //ammo -= reAmmo; //재장전시 플레이어가 갖고있는 탄약은 그만큼 줄어듬
        isReload = false;
    }

    void OnCollisionEnter(Collision collision)
    {   //???? ????
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            infiniteJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();

            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value; //???? ????
                    if(ammo > maxAmmo)  //???? ?????????? ???? ?????? ?????? ????
                    {
                        ammo = maxAmmo;
                    }
                    break;

                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                    {
                        coin = maxCoin;
                    }
                    break;

                case Item.Type.Health:
                    health += item.value;
                    if (health > maxHealth)
                    {
                        health = maxHealth;
                    }
                    break;

                case Item.Type.Grenade:
                    if (hasGrenades == maxHasGrenades)
                        return;
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    break;
            }
            Destroy(other.gameObject); // ?????? ?????? ?????? ????
        }
    }



    void OnTriggerStay(Collider other)
    {
     if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }  
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
    void Swap()
    { //????????
        if (sDown1 && (!hasWeapon[0] || equipWeaponIndex == 0))                                            
        {                                                       
            return;
        }
        if (sDown2 && (!hasWeapon[1] || equipWeaponIndex == 1))
        {
            return;
        }
        if (sDown3 && (!hasWeapon[2] || equipWeaponIndex == 2))
        {
            return;
        }
        if (sDown4 && (!hasWeapon[3] || equipWeaponIndex == 3))
        {
            return;
        } 


        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
        if (sDown4) weaponIndex = 3;

        if ((sDown1 || sDown2 || sDown3 || sDown4) && !isjump)
        {
            if(equipWeapon != null)
                 equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        speed += 0.5f;
        isSwap = false;
    }
    void Interaction() // ????????
    {
        if (iDown && nearObject != null)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void look()
    {
        //???????? ???????? ???????? ????????
        transform.LookAt(transform.position + moveVec);
    }

    void LookMouseCursor()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitResult;
        if (mouseDown)
        {
            Debug.Log("mouse down");
            if (Physics.Raycast(ray, out hitResult))
            {
                Vector3 mourDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                mourDir.y = 0;
                transform.forward = mourDir;
                Debug.Log("hit result : " +  hitResult);
            }
        }
    }

    void LookMouseCursor2()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        if (mouseDown2)
        {
            Debug.Log("mouse down2");
            if (Physics.Raycast(ray, out hitResult))
            {
                Vector3 mourDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                mourDir.y = 0;
                transform.forward = mourDir;
                Debug.Log("hit result : " + hitResult);
            }
        }
    }

    void MouseWalk()
    {
          if (!isSwap || isFireReady || !isReload || !fDown)
            {
                if (Input.GetMouseButtonDown(0))
                {
                     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                     RaycastHit hitResult2;
                     if (Physics.Raycast(ray, out hitResult2))
                     {
                        SetDestination(hitResult2.point);
                     }

                     moveVec = destination - transform.position;

                   
                    //transform.position += des.normalized * Time.deltaTime * speed;
                    //Quaternion toRotation = Quaternion.FromToRotation(transform.forward, des.normalized);
                    //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
                }
                
               
           }
        
    }

   
    private void SetDestination(Vector3 dest)
    {
        destination = dest;
    }

    void FreezeRotation()
    {
        playerRB.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 1, LayerMask.GetMask("Wall")); //"Wall"태그 근처에서는 이동 불가 
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
}
