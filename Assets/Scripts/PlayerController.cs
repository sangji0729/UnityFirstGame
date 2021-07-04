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

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;

    private float speed = 1.7f;
    private float verticalInput;
    private float horizontalInput;
    private Vector3 moveVec;
    private bool isjump;
    private bool infiniteJump;
    private bool mouseDown;
    private bool isWalk;
    private bool isRun;
    private bool sDown1;//무기교체
    private bool sDown2;
    private bool sDown3;
    private bool sDown4;
    private bool isSwap;


    private float mouseLocation;
    private float jumpPower = 5;

    bool iDown;//아이템입수

    Animator anim;
    Rigidbody playerRB;
    Camera characterCamera;

    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponIndex = -1;
    private void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        characterCamera = GetComponentInChildren<Camera>();
        mouseDown = Input.GetMouseButtonDown(0);
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
       // LookMouseCursor();
        look();
        Interaction();
        Swap();


    }

    void Move()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        isRun = Input.GetButton("Run");
        iDown = Input.GetButtonDown("Interaction");//아이템획득
        sDown1 = Input.GetButtonDown("Swap1");//무기교체
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        //sDown4 = Input.GetButtonDown("Swap4");

        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", isRun);

        moveVec = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (isRun) //달리기 상태
        {
            speed = 2;
            transform.position += moveVec * speed * Time.deltaTime;
            isRun = false;
           
        }
        if (isSwap)
        {
            moveVec = Vector3.zero;
        }
        transform.position += moveVec * speed * Time.deltaTime;
        

      // transform.Translate(Vector3.forward * verticalInput * Time.deltaTime * speed);
      // transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);
    }

    void Jump()
    {
        isjump = Input.GetButtonDown("Jump");
        anim.SetBool("isJump", isjump);
        anim.SetTrigger("doJump");

        if (isjump && !infiniteJump && !isSwap) //점프
        {
            playerRB.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            infiniteJump = true;
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {   //바닥 접촉
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
                    ammo += item.value; //총알 획득
                    if(ammo > maxAmmo)  //총알 최대치에서 총알 획득시 최대값 유지
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
            Destroy(other.gameObject); // 아이템 획득후 아이템 파괴
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
    { //무기교체
        if (sDown1 && (!hasWeapon[0] || equipWeaponIndex == 0)) // 무기를 미획득시 1,2,3,번을 눌러도 무반응효과                                                  
        {                                                        // 현재 무기1개 획득시 다른무기도 획득되는 버그 있음. 무기 교체시 이동속도 빨라지는 버그 존재. 현재 수정중
            return;
        }
        if (sDown2 && (!hasWeapon[0] || equipWeaponIndex == 1))
        {
            return;
        }
        if (sDown3 && (!hasWeapon[0] || equipWeaponIndex == 2))
        {
            return;
        }
       /* if (sDown4 && (!hasWeapon[0] || equipWeaponIndex == 3))
        {
            return;
        } */


        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
       // if (sDown4) weaponIndex = 3;

        if ((sDown1 || sDown2 || sDown3) && !isjump)
        {
            if(equipWeapon != null)
                 equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

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
    void Interaction() // 상호작용
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
        //움직이는 방향으로 캐릭터가 바라보기
        transform.LookAt(transform.position + moveVec);
    }

    void LookMouseCursor()
    {
            Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitResult;
        if (mouseDown)
        {
            if (Physics.Raycast(ray, out hitResult))
            {
                Vector3 mourDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                transform.forward = mourDir;
            }
        }
    }
}
