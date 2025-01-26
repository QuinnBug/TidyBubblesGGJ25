using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour {
    public UnityEvent<Bullet> OnHit;
    public UnityEvent<Bullet> OnMiss;
    public UnityEvent<Bullet, DirtObject, Vector2> OnHitDirt;
    private float lifetime = 3.5f;
    public Rigidbody Rb;
    [Space]
    public ParticleSystem bubbles;

    [SerializeField] private float rayDeviation = 0.8f;
    private Vector3 deviation;

    private void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) {
            OnMiss.Invoke(this);
            Destroy(gameObject);
        }
    }
    public void Launch(float force) {
        Rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent(out DirtObject dirt)) {
            var contactPoint = collision.GetContact(0).point;
            var rayDirection = contactPoint - transform.position;
            deviation = new Vector3(Random.Range(-rayDeviation, rayDeviation), 0, Random.Range(-rayDeviation, rayDeviation));
            var rayDirDeviated = rayDirection + deviation;
            //Debug.DrawRay(transform.position, rayDirection, Color.red, 5f);
            if (Physics.Raycast(transform.position, rayDirDeviated, out var hit)) {
                var textureCoord = hit.textureCoord;
                OnHitDirt.Invoke(this, dirt, textureCoord);
            }
            else {
                //  The ray deviated too much and missed the object
                if (Physics.Raycast(transform.position, rayDirection, out var backupHit)) {
                    var textureCoord = backupHit.textureCoord;
                    OnHitDirt.Invoke(this, dirt, textureCoord);
                }
                else OnMiss.Invoke(this);
            }
        }
        else OnHit.Invoke(this);

        SpawnParticles(collision.GetContact(0).point);
        Destroy(gameObject);
    }

    void SpawnParticles(Vector3 pos) 
    {
        ParticleSystem ps = Instantiate<ParticleSystem>(bubbles);
        ps.transform.position = pos;
        Destroy(ps.gameObject, ps.main.startLifetime.constantMax);
    }
}