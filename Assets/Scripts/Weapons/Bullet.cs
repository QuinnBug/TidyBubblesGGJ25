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
            if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out var hit)) {
                var textureCoord = hit.textureCoord;
                OnHitDirt.Invoke(this, dirt, textureCoord);
            }
            else {
                if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out var backupHit)) {

                    var textureCoord = backupHit.textureCoord;
                    OnHitDirt.Invoke(this, dirt, textureCoord);
                }
                else {
                    if (Physics.SphereCast(transform.position, 0.5f, Vector3.forward, out var backupHit2)) {
                        var textureCoord = backupHit2.textureCoord;
                        OnHitDirt.Invoke(this, dirt, textureCoord);
                    }
                    else {
                        OnMiss.Invoke(this);
                        Debug.Log("No texture hit, what");
                    }
                }
            }
        }
        else OnHit.Invoke(this);
        Destroy(gameObject);
    }

}