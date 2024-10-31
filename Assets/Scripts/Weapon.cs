using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public GameObject gunSound;
    UnityEngine.Vector3 mousePosition;
    public void Shoot()
    {
        Instantiate(gunSound, firePoint.position, firePoint.rotation);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.right * fireForce, ForceMode2D.Impulse);
    }

    public void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        UnityEngine.Vector2 aimDirection = mousePosition - transform.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = UnityEngine.Quaternion.Euler(0, 0, aimAngle);
    }
}
