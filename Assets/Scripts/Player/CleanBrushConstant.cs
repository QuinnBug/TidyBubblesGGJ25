using UnityEngine;

public class CleanBrushConstant : DirtBrush
{
    [SerializeField] private Collider brushCollider;
    private string myName;
    private DirtObject currentDirt;
    [SerializeField] private float drawSpeed = 0.08f;
    private bool waitForTick = false;
    private float timer = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        myName = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitForTick) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                waitForTick = false;
                timer = drawSpeed;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!enabled) return;
        if (other.gameObject.TryGetComponent<DirtObject>(out currentDirt)) {
            if (currentDirt.IsClean) return;
            Ray ray = new Ray(brushCollider.bounds.center, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                CleanDirt(currentDirt, hit.textureCoord);
            }
        }
    }
    private void OnTriggerStay(Collider other) {
        if (!enabled || waitForTick) return;
        if (currentDirt is null || currentDirt.IsClean) {
            if (other.gameObject.TryGetComponent<DirtObject>(out currentDirt)) {
                if (currentDirt.IsClean) return;
            }
            else return;
        }

        // add a raycast going down
        Ray ray = new Ray(brushCollider.bounds.center, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            CleanDirt(currentDirt, hit.textureCoord);
        }
        waitForTick = true;

    }
    private void OnTriggerExit(Collider other) {
        if (!enabled) return;
        currentDirt = null;
    }
    private void OnDisable() {
        currentDirt = null;
        gameObject.name = $"{myName} (Disabled)";
    }
    private void OnEnable() {
        gameObject.name = $"{myName} (Enabled)";
    }
}
