using UnityEngine;

public class PlayerController : MonoBehaviour {
    public PlanetController PlanetTemplate;
    public GravitationalSimulator SimulatorInstance;

    public bool TreatBoundsAsCircle =
        true;
    public bool CreateDebugData =
        true;

    [HideInInspector]
    public float galacticPlaneY =
        0.0f;

    void Start () {
        galacticPlaneY =
            PlanetTemplate.transform.position.y;

        GravitationalSimulator gravitationalSimulator =
            FindObjectOfType<GravitationalSimulator> ();

        if (CreateDebugData && !gravitationalSimulator.ReplaySimulationFromFile) {
            GenerateDebugData();
        }
    }

    void Update () {
        const float MaximumRaycastDistance =
            1000.0f;
        const string RayHitGameObjectTag =
            "GameField";

        if (Input.GetMouseButtonDown(0)) {
            Ray ray =
                Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit[] hits =
                Physics.RaycastAll (ray, MaximumRaycastDistance);

            foreach (RaycastHit hit in hits) {
                Collider collider =
                    hit.collider;
                Transform transform =
                    hit.transform;

                if (collider.isTrigger ||
                        !transform.CompareTag (RayHitGameObjectTag)) {
                    continue;
                }

                Vector3 position =
                    hit.point;
                position.y =
                    galacticPlaneY;

                Bounds bounds =
                    collider.bounds;
                Vector3 center =
                    bounds.center;
                center.y =
                    galacticPlaneY;
                Vector3 extents =
                    bounds.extents;
                float radiusSquared =
                    Mathf.Max(extents.x, extents.z);
                radiusSquared *=
                    radiusSquared;

                if ((position - center).sqrMagnitude < radiusSquared) {
                    AddPlanet (position);

                    break;
                }
            }
        }
    }

    private void AddPlanet (Vector3 position) {
        PlanetController planet =
            Instantiate(
                PlanetTemplate,
                position,
                Quaternion.identity
            );

        SimulatorInstance.AddPlanet (planet);
    }

    private void GenerateDebugData () {
        const int planetCount =
            180;
        const float accelerationScale =
            100.0f;

        for (int i = 0; i < planetCount; ++i) {
            float angle =
                ((float) i / planetCount) * 2.0f * Mathf.PI +
                    ((Random.value - 0.5f) * 0.5f);

            PlanetController planet =
                Instantiate (
                    PlanetTemplate,
                    new Vector3 (
                        Random.value,
                        galacticPlaneY,
                        Random.value
                    ),
                    Quaternion.identity
                );

            planet.GetComponent<Rigidbody> ().velocity =
                new Vector3 (
                    Mathf.Cos (angle),
                    0.0f,
                    Mathf.Sin (angle)
                ) * accelerationScale * Random.value;

            float initialMass =
                planet.Mass;
            planet.Mass =
                initialMass * Random.value + initialMass * 0.5f;

            float scale =
                (planet.Mass / (initialMass * 1.5f)) + 0.1f;
            planet.transform.localScale =
                new Vector3 (scale, scale, scale);

            SimulatorInstance.AddPlanet(planet);
        }
    }
}

