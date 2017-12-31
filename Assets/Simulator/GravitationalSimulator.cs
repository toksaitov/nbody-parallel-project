using System;
using System.Collections.Generic;

using UnityEngine;

public class GravitationalSimulator : MonoBehaviour {
    [Header("Simulation Options")]

    public bool IntegrateMovement =
        true;
    public bool PreferBarnesHut =
        true;
    public bool EnableLog =
        true;
    public bool ShowQuadTree =
        true;
    public bool ShowInteractions =
        true;

    [Header("Simulation Parameters")]

    public float UniverseSize =
        140.0f;
    public float SimulationSofteningLength =
        100.0f;
    private float simulationSofteningLengthSquared;

    public float SimulationTimePeriod =
        float.PositiveInfinity;
    public float SimulationDeltaTime =
        -1.0f; /* Set any negative value to use Unity's fixed update DeltaTime */

    [Header("Simulation Files")]

    public bool ReplaySimulationFromFile =
        false;
    public TextAsset SimulationFile;

    // ---

    private List<PlanetController> planets =
        new List<PlanetController>();

    private QuadTreeNode quadtree;

    private long interactions = 
        0;

    private Vector2[] simulationAccelerations =
        null;
    private int simulationAccelerationsCursor =
        0;

    public void AddPlanet (PlanetController planet) {
        planets.Add (planet);
    }

    void Start () {
        if (ReplaySimulationFromFile) {
            LoadSimulationData ();
        }

        simulationSofteningLengthSquared =
            SimulationSofteningLength * SimulationSofteningLength;

        WireframeController[] wireframeControllers =
            FindObjectsOfType<WireframeController> ();

        foreach (var wireframeController in wireframeControllers) {
            wireframeController.Planets =
                planets;
        }

        quadtree =
            new QuadTreeNode (UniverseSize);

        SetupSimulationTime ();
    }

    void FixedUpdate () {
        if (SimulationTimePeriod != float.PositiveInfinity) {
            if (SimulationTimePeriod <= 0.0f) {
                foreach (PlanetController planet in planets) {
                    planet.Acceleration =
                        Vector2.zero;
                    planet.GetComponent<Rigidbody> ().velocity =
                        Vector3.zero;
                }

                return;
            }
            SimulationTimePeriod -= SimulationDeltaTime;
        }

        if (ReplaySimulationFromFile) {
            ReplaySimulation ();

            return;
        }

        interactions =
            0;

        if (PreferBarnesHut) {
            SimulateWithBarnesHut ();
        } else {
            SimulateWithBruteforce ();
        }

        if (EnableLog) {
            Debug.Log (
                "Barnes-Hut: " + PreferBarnesHut +
                "; Planets: "  + planets.Count +
                "; Interactions: " + interactions
            );
        }
    }

    void Update () {
        planets.RemoveAll (planet => !planet.IsAlive);
    }

    void OnDrawGizmos () {
        if (ShowQuadTree) {
            DrawDebugGizmos ();
        }
    }

    private void ReplaySimulation () {
        if (simulationAccelerationsCursor >= simulationAccelerations.Length) {
            return;
        }

        foreach (PlanetController planet in planets) {
            planet.Acceleration =
                simulationAccelerations [simulationAccelerationsCursor++];
        }
    }

    private void SimulateWithBarnesHut () {
        quadtree.Clear ();
        quadtree.TryAddRange (planets);

        if (!IntegrateMovement)
            return;

        foreach (PlanetController planet in planets) {
            planet.Acceleration =
                quadtree.CalculateForce (
                    planet, CalculateNewtonGravityAcceleration
                );
        }
    }

    private void SimulateWithBruteforce () {
        if (!IntegrateMovement)
            return;

        foreach (PlanetController planet in planets) {
            if (!planet.IsAlive)
                continue;

            Vector2 acceleration = Vector2.zero;
            foreach (PlanetController anotherPlanet in planets) {
                if (planet == anotherPlanet || !anotherPlanet.IsAlive)
                    continue;

                acceleration +=
                    CalculateNewtonGravityAcceleration (
                        planet, anotherPlanet
                    );
            }

            planet.Acceleration =
                acceleration;
        }
    }

    private Vector2 CalculateNewtonGravityAcceleration (
                        IBody firstBody,
                        IBody secondBody
                    ) {
        ++interactions;
        if (ShowInteractions)
            DrawDebugLines (firstBody, secondBody);

        Vector2 acceleration =
            Vector2.zero;

        Vector2 galacticPlaneR =
            secondBody.Position - firstBody.Position;

        float distanceSquared =
            galacticPlaneR.sqrMagnitude + simulationSofteningLengthSquared;
        float distanceSquaredCubed =
            distanceSquared * distanceSquared * distanceSquared;
        float inverse =
            1.0f / Mathf.Sqrt (distanceSquaredCubed);
        float scale =
            secondBody.Mass * inverse;

        acceleration +=
            galacticPlaneR * scale;

        return acceleration;
    }

    private void DrawDebugGizmos () {
        if (!Application.isPlaying)
            return;

        Gizmos.matrix =
            Matrix4x4.TRS (
                Vector3.zero,
                Quaternion.AngleAxis (
                    90.0f, Vector3.right
                ),
                Vector3.one
            );

        quadtree.Visit (node => {
            bool shouldContinue =
                true;

            Gizmos.DrawWireCube (
                node.Bounds.Center,
                node.Bounds.Size
            );

            return shouldContinue;
        });
    }

    private void DrawDebugLines (
                     IBody firstBody,
                     IBody secondBody
                 ) {
        Vector2 from =
            firstBody.Position;
        Vector2 to =
            secondBody.Position;
        Color debugLineColor =
            secondBody is QuadTreeNode ? Color.green : Color.red;

        Debug.DrawLine (
            new Vector3 (from.x, 0.0f, from.y),
            new Vector3 (to.x,   0.0f, to.y),
            debugLineColor
        );
    }

    private void SetupSimulationTime () {
        if (SimulationDeltaTime > 0.0f) {
            Time.fixedDeltaTime =
                SimulationDeltaTime;
        }
    }

    private void LoadSimulationData () {
        foreach (PlanetController planet in planets) {
            Destroy (planet.gameObject);
        }
        planets.Clear ();

        try {
            string[] lines = SimulationFile.text.Trim ().Split (
                new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries
            );

            int cursor =
                0;
            int planetCount =
                int.Parse (lines [cursor++]);
            SimulationTimePeriod =
                float.Parse (lines [cursor++]);
            SimulationDeltaTime  =
                float.Parse (lines [cursor++]);

            int iterations =
                (int) (SimulationTimePeriod / SimulationDeltaTime);
            simulationAccelerations =
                new Vector2 [iterations * planetCount * 2];

            PlayerController controller =
                FindObjectOfType<PlayerController> ();
            PlanetController template =
                controller.PlanetTemplate;

            for (int i = 0; i < planetCount; ++i) {
                string[] components =
                    lines [cursor++].Trim ().Split (null);

                float x =
                    float.Parse (components [0]);
                float y =
                    float.Parse (components [1]);

                PlanetController planet =
                    Instantiate (
                        template,
                        new Vector3 (
                            x,
                            controller.galacticPlaneY,
                            y
                        ),
                        Quaternion.identity
                    );

                components =
                    lines [cursor++].Trim ().Split (null);

                float ax =
                    float.Parse (components [0]);
                float ay =
                    float.Parse (components [1]);

                planet.Acceleration =
                    new Vector2 (
                        ax, ay
                    );

                components =
                    lines [cursor++].Trim ().Split (null);

                float vx =
                    float.Parse (components [0]);
                float vy =
                    float.Parse (components [1]);

                planet.GetComponent<Rigidbody> ().velocity =
                    new Vector3 (
                        vx,
                        0.0f,
                        vy
                    );

                float initialMass =
                    planet.Mass;

                float mass =
                    float.Parse(lines [cursor++]);
                planet.Mass =
                    mass;

                float scale =
                    (planet.Mass / (initialMass * 1.5f)) + 0.1f;
                planet.transform.localScale =
                    new Vector3 (scale, scale, scale);

                AddPlanet(planet);
            }

            for (int i = 0; cursor < lines.Length; ++i) {
                string[] components =
                    lines [cursor++].Trim ().Split (null);

                float ax =
                    float.Parse (components [0]);
                float ay =
                    float.Parse (components [1]);

                simulationAccelerations[i] =
                    new Vector2 (ax, ay);
            }
        } catch(Exception exception) {
            Debug.Log ("Failed to load simulation data: " + exception.Message);
        }
    }
}

