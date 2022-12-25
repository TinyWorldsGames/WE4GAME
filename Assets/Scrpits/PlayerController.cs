using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Deathmatch.io.Packets;

public class PlayerController : MonoBehaviour,IDamageable
{
    [SerializeField]
    public GameObject bulletPrefab;
    [SerializeField]
    public Transform bulletSpawnPoint;

    [SerializeField]
    FireButton fireButton;
    [SerializeField]
    DashButton dashgButton;

    [SerializeField]
    float fireRate;
    float damage;
    int magazine;
    bool unlimitedMagazine;
    bool defaultGun;
    [SerializeField]
    GameObject magazineIcon;
    [SerializeField]
    Image magazineBar;
    [SerializeField]
    Text magazineCountText;
    [SerializeField]
    char infinity;
    [SerializeField]
    GameObject muzzleFlash;
    

    [SerializeField]
    Gun defaultPistol;
    Gun activeGun;
    float timer;

    [SerializeField]
    GameObject tailRenderer;
    [SerializeField]
    float dashCooldown;
    float dashTimer;
    [SerializeField]
    Image dashButton;

    [SerializeField]
    Rigidbody2D rigidbody2D;


    [SerializeField]
    float health;


    private void Start()
    {
        ChangeGun(defaultPistol);

    }



    private void Update()
    {
        if (Input.GetKey(KeyCode.Space)||fireButton.isPressed)
        {
            Fire();
        }

        if (dashTimer < Time.time)
        {

          
          

            if (Input.GetKeyDown(KeyCode.A) || dashgButton.isPressed)
            {
                dashButton.fillAmount = 1;
                dashTimer = Time.time + dashCooldown;
                StartCoroutine(Dash());

            }
            else
            {
                dashButton.fillAmount = 0;
            }

           
        }

        else
        {
            dashButton.fillAmount -= Time.deltaTime / dashCooldown;
        }


       
       
    }
    IEnumerator Dash()
    {
        rigidbody2D.velocity = (transform.up * 15);
        tailRenderer.SetActive(true);
        yield return new WaitForSeconds(0.12f);
        rigidbody2D.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);
        tailRenderer.SetActive(false);

    }

    public void ChangeGun(Gun newGun)
    {
        fireRate = newGun.fireRate;
        damage = newGun.damage;
        magazine = newGun.magazine;
        unlimitedMagazine = newGun.unLimited;
        defaultGun = newGun.defaultGun;

    }

    public void TakeGun(Gun newGun)
    {

    }

    IEnumerator FillMagazineBar(float fireRate)
    {

       

        yield return new WaitForSeconds(0.01f);
       
        magazineBar.fillAmount +=  0.01f/fireRate;
      
        magazineIcon.SetActive(true);

       
        if (magazineBar.fillAmount<1)
        {
           
            StartCoroutine(FillMagazineBar(fireRate));
         
        }

        else
        {
            if (!unlimitedMagazine)
            {
                magazine--;
                magazineCountText.text = magazine.ToString();
            }
            else
            {
                magazineCountText.text = infinity.ToString();
            }
            magazineIcon.SetActive(false);
            magazineBar.fillAmount = 0;
           
        }

    }


    public void Fire()
    {
       
        if (timer<Time.time)
        {
            

            if (unlimitedMagazine)
            {
                GameObject newBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

                timer = Time.time + fireRate;

                StartCoroutine(FillMagazineBar(fireRate));

                muzzleFlash.SetActive(true);

                if (NetworkManager.instance.server != null)
                {
                    NetworkManager.instance.netPacketProcessor.Send(NetworkManager.instance.server, new NewBulletPacket
                    {
                        PositionX = bulletSpawnPoint.position.x,
                        PositionY = bulletSpawnPoint.position.y,
                        Rotation = bulletSpawnPoint.transform.eulerAngles.z
                        

                    }, LiteNetLib.DeliveryMethod.ReliableOrdered);
                }
            }

            else if(magazine>0)
            {
                GameObject newBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

                timer = Time.time + fireRate;

                StartCoroutine(FillMagazineBar(fireRate));

                muzzleFlash.SetActive(true);

                if (NetworkManager.instance.server != null)
                {
                    NetworkManager.instance.netPacketProcessor.Send(NetworkManager.instance.server, new NewBulletPacket
                    {
                        PositionX = bulletSpawnPoint.position.x,
                        PositionY = bulletSpawnPoint.position.y,
                        Rotation = bulletSpawnPoint.transform.eulerAngles.z

                    }, LiteNetLib.DeliveryMethod.ReliableOrdered);
                }
            }

            else
            {
                ChangeGun(defaultPistol);
            }

           

          

        }
       
      
        

    }

    public void TakeHit(float damage)
    {
        health -= damage;

        if (health<=0)
        {
            Destroy(gameObject);
        }
    }
}
