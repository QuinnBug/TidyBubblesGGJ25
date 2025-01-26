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
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent(out DirtObject dirt)) {
            var contactPoint = collision.GetContact(0).point;
            var rayDirection = contactPoint - transform.position;
            //Debug.DrawRay(transform.position, rayDirection, Color.red, 5f);
            if (Physics.Raycast(transform.position, rayDirection, out var hit)) {
                var textureCoord = hit.textureCoord;
                OnHitDirt.Invoke(this, dirt, textureCoord);
            }
            else {
                Debug.Log($"Fuck you");
                OnMiss.Invoke(this);
            }

        }
        else OnHit.Invoke(this);
        Destroy(gameObject);
    }

}