using UnityEngine;

[RequireComponent(typeof(DirtBrush))]
public class Weapon : MonoBehaviour
{
    private DirtBrush dirtBrush; // Will be attached to the bullet
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] int ammo = 6;
    private bool canFire => ammo > 0;
    [SerializeField] private float fireRate = 0.24f;
    private float shotTimer = 0;

    private void Awake() {
        dirtBrush = GetComponent<DirtBrush>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shotTimer > 0) {
            shotTimer -= Time.deltaTime;
        }

        if (Input.GetMouseButton(0) && shotTimer <= 0) {
            Shoot();
            shotTimer = fireRate;
        }
    }

    private void Shoot() {
        if (!canFire) {
            return;
        }
        ammo--;
        var newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        newBullet.OnHit.AddListener(OnBulletHit);
        newBullet.OnHitDirt.AddListener(OnBulletHitDirt);
        newBullet.OnMiss.AddListener(OnBulletMiss);

        newBullet.transform.position = bulletSpawn.position;
        newBullet.transform.forward = transform.right;

        newBullet.Launch(30);
    }
    private void OnBulletHit(Bullet bullet) {
        ammo++;
        bullet.OnMiss.RemoveListener(OnBulletMiss);
        bullet.OnHit.RemoveListener(OnBulletHit);
        bullet.OnHitDirt.RemoveListener(OnBulletHitDirt);
    }
    private void OnBulletHitDirt(Bullet bullet, DirtObject dirt, Vector2 textureCoords) {
        ammo++;
        bullet.OnMiss.RemoveListener(OnBulletMiss);
        bullet.OnHit.RemoveListener(OnBulletHit);
        bullet.OnHitDirt.RemoveListener(OnBulletHitDirt);
        dirtBrush.CleanDirt(dirt, textureCoords);
    }
    private void OnBulletMiss(Bullet bullet) {
        ammo++;
        bullet.OnMiss.RemoveListener(OnBulletMiss);
        bullet.OnHit.RemoveListener(OnBulletHit);
        bullet.OnHitDirt.RemoveListener(OnBulletHitDirt);


    }
}
