using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    const float PIVOT_OFFSET = 90.0f;
    private float rotation = 60;
    public float speed = 4;
    public NetworkIdentity networkIdentity;
    [SerializeField]
    private CircleCollider2D circleCol;

    [SerializeField]
    private Transform barrelPivot;

    [SerializeField]
    private Transform bulletSpawnPoint;
    private float lastRotation;


    // shoting
    private CoolDown shootingCoolDown;
    private BulletData bulletData;

    //  private BoxCollider2D boxCollider;
    private void Start()
    {
        //   boxCollider = GetComponent<BoxCollider2D>();
        shootingCoolDown = new CoolDown(1);
        bulletData = new BulletData();
        bulletData.position = new Position();
        bulletData.direction = new Position();
    }
    private void FixedUpdate()
    {
        if (networkIdentity.IsControlling())
        {
            CheckMove();
            checkAiming();
            checkShooting();
        }
    }
    public float GetLastRotation()
    {
        return lastRotation;
    }
    public void SetRotaion(float rot)
    {
        barrelPivot.rotation = Quaternion.Euler(0, 0, rot + PIVOT_OFFSET);
    }

    void CheckMove()
    {
        //  hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Charactor", "Blocking"));

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        //if (!circleCol.IsTouchingLayers())
        //{

        transform.position += (-transform.up * vertical * speed * Time.deltaTime);
        transform.Rotate(new Vector3(0, 0, -horizontal * rotation * Time.deltaTime));
        //}
        //  transform.position += ;


    }
    void checkAiming()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dif = mousePosition - transform.position;
        dif.Normalize();
        float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;

        lastRotation = rot;
        barrelPivot.rotation = Quaternion.Euler(0, 0, rot + PIVOT_OFFSET);
    }
    void checkShooting()
    {
        shootingCoolDown.CoolDownUpdate();
        if (Input.GetMouseButton(0) && !shootingCoolDown.IsOnCooldown())
        {
            shootingCoolDown.StarCoolDown();

            // defined bullet
            bulletData.activator = Netwoking.ClientID;
            bulletData.position.x = Mathf.Round(bulletSpawnPoint.position.x * 1000.0f) / 1000.0f;
            bulletData.position.y = Mathf.Round(bulletSpawnPoint.position.y * 1000.0f) / 1000.0f;
            bulletData.direction.y = -bulletSpawnPoint.up.y;
            bulletData.direction.x = -bulletSpawnPoint.up.x;


            //send bullet

            networkIdentity.GetSocket().Emit("fireBullet", new JSONObject(JsonUtility.ToJson(bulletData)));
        }
    }
}
