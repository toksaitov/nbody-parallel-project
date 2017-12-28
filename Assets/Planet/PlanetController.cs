using UnityEngine;

public enum PlanetState {
    Alive,
    Leaving,
    Left,
    Exploding,
    Exploded
}

public class PlanetController : MonoBehaviour, IBody {
    public Vector2 Position {
        get {
            Vector3 position = transform.localPosition;
            return new Vector2(position.x, position.z);
        }
    }

    public float mass = 100000.0f;
    public float Mass {
        get {
            return mass;
        }
        set {
            mass = value;
        }
    }

    [HideInInspector]
    public Vector2 Acceleration =
        Vector2.zero;

    [HideInInspector]
    public PlanetState State =
        PlanetState.Alive;

    public bool IsAlive {
        get { return State == PlanetState.Alive; }
    }

    private Rigidbody rigidBody;

    public void Leave () {
        if (IsAlive) {
            State =
                PlanetState.Leaving;

            // ToDo
        }
    }

    public void Explode () {
        if (IsAlive) {
            State =
                PlanetState.Exploding;

            // ToDo
        }
    }

    void Start () {
        rigidBody =
            GetComponent<Rigidbody> ();
    }

    void FixedUpdate () {
        if (State == PlanetState.Alive) {
            Vector3 force = new Vector3 (Acceleration.x, 0.0f, Acceleration.y);
            rigidBody.AddForce(force);
        } else if (State == PlanetState.Leaving) {
            // ToDo

            State = PlanetState.Left;
            Destroy (this.gameObject);
        } else if (State == PlanetState.Exploding) {
            // ToDo

            State = PlanetState.Exploded;
            Destroy (this.gameObject);
        }
    }

    void OnCollisionEnter (Collision collision) {
        const string CollisionGameObjectTag =
            "Planet";

        if (collision.gameObject.CompareTag (CollisionGameObjectTag)) {
            Explode ();
        }
    }

    void OnTriggerExit (Collider other) {
        const string TriggerGameObjectTag =
            "GameField";

        if (other.CompareTag (TriggerGameObjectTag)) {
            Leave ();
        }
    }
}

