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
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out DirtObject dirt)) {
            if (Physics.Raycast(transform.position, Vector3.down, out var hit)) {
                var textureCoord = hit.textureCoord;
                OnHitDirt.Invoke(this, dirt, textureCoord);
            }
            
        }
        else OnHit.Invoke(this);
        Destroy(gameObject);
    }

}